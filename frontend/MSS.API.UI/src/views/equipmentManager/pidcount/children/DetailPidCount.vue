<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <title-module></title-module>
    </div>
    <div class="box">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
          <div class="input-group">
            <label for="">车站编号:</label>
            <div class="inp" style="width:65px">
              <span>{{nodeId}}</span>
            </div>
          </div>
          <div class="input-group">
            <label for="">车站名称:</label>
            <div class="inp">
              <span>{{nodeName}}</span>
            </div>
          </div>
          <div class="input-group">
            <label for="">车站缩写:</label>
            <div class="inp" style="width:80px">
              <span>{{nodeTip}}</span>
            </div>
          </div>
          <div class="input-group">
            <label for="">点位容量:</label>
            <div class="inp">
              <span>{{capacityCount}}</span>
            </div>
          </div>
          <div class="input-group">
            <label for="">使用数量:</label>
            <div class="inp">
              <span>{{usedCount}}</span>
            </div>
          </div>
          <div class="input-group">
            <label for="">剩余数量:</label>
            <div class="inp">
              <span>{{remainCount}}</span>
            </div>
          </div>
          <div class="input-group">
            <label for="">预警线:</label>
            <div class="inp">
              <span>{{remindCount}}</span>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><router-link :to="{name:'SeePidCountList'}">返回</router-link></x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list"><span>填写点位变化情况:</span></li>
        <li class="list">
                    <el-select v-model="detailType.text" placeholder="请选择事件类型" filterable clearable>
                      <el-option
                      v-for="item in detailTypeList"
                      :key="item.key"
                      :value="item.key"
                      :label="item.value">
                      </el-option>
                    </el-select>
        </li>
        <li class="list"><p class="validate-tips">{{ detailType.tips }}</p></li>
        <li class="list"><el-input v-model.trim="changeCount.text" placeholder="请输入数字"></el-input></li>
        <li class="list"><p class="validate-tips">{{ changeCount.tips }}</p></li>
        <li class="list"><el-input v-model.trim="detailContent.text" placeholder="请填写描述" style="width: 350px;"></el-input></li>
        <li class="list"><p class="validate-tips">{{ detailContent.tips }}</p></li>
        <li class="list" @click="add"><x-button>提交</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list number c-pointer">
          事件类型
        </li>
        <li class="list number c-pointer">
          影响数量
        </li>
        <li class="list number c-pointer">
          事件描述
        </li>
        <li class="list last-update-time c-pointer" @click="changeOrder('updated_time')">
          最后更新时间
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_time === 0 }, { 'el-icon-caret-top': headOrder.updated_time === 1 }, { 'el-icon-caret-bottom': headOrder.updated_time === 2 }]"></i>
        </li>
        <li class="list last-maintainer c-pointer" @click="changeOrder('updated_by')">
          最后更新人
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_by === 0 }, { 'el-icon-caret-top': headOrder.updated_by === 1 }, { 'el-icon-caret-bottom': headOrder.updated_by === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in EqpList" :key="item.key">
              <div class="list-content">
                <div class="number">{{ item.detailType }}</div>
                <div class="number">{{ item.ccc }}</div>
                <div class="number">{{ item.detailContent }}</div>
                <div class="last-update-time color-white">{{ item.updatedTime }}</div>
                <div class="last-maintainer">{{ item.updatedName }}</div>
              </div>
            </li>
          </ul>
        <!-- 分页 -->
          <el-pagination
            :current-page.sync="currentPage"
            @current-change="handleCurrentChange"
            @prev-click="prevPage"
            @next-click="nextPage"
            layout="slot, jumper, prev, pager, next"
            prev-text="上一页"
            next-text="下一页"
            :total="total">
            <span>总共 {{ total }} 条记录</span>
          </el-pagination>
        </el-scrollbar>
      </div>
    </div>
    <!-- dialog对话框 -->
    <el-dialog
      :visible.sync="dialogVisible.isShow"
      :modal-append-to-body="false"
      :show-close="false">
      {{ dialogVisible.text }}
      <template slot="footer" class="dialog-footer">
        <template v-if="dialogVisible.btn">
          <el-button @click="dialogVisible.isShow = false">否</el-button>
          <el-button @click="dialogEnter">是</el-button>
        </template>
        <el-button v-else @click="dialogVisible.isShow = false" :class="{ on: !dialogVisible.btn }">知道了</el-button>
      </template>
    </el-dialog>
  </div>
</template>
<script>
// import { dictionary } from '@/common/js/dictionary.js'
import PidCountEnum from './PidCountEnum'
import { btn } from '@/element/btn.js'
import XButton from '@/components/button'
import { transformDate, validateInputCommon, validateNumberCommon } from '@/common/js/utils.js'
import api from '@/api/eqpApi'
import TitleModule from '@/components/TitleModule'
export default {
  name: 'DetailPidCount',
  components: {
    XButton,
    'title-module': TitleModule
  },
  data () {
    return {
      btn: {
        save: false,
        delete: false,
        update: false
      },
      parm: {
        pidCountId: 0
      },
      title: ' | 点位资源变动详情',
      pidCountId: this.$route.query.pidcountid,
      nodeId: '',
      nodeName: '',
      nodeTip: '',
      capacityCount: '',
      usedCount: '',
      remainCount: '',
      remindCount: '',
      EqpList: [],
      editObjID: [],
      changeCount: {
        text: '',
        tips: ''
      },
      detailType: {
        text: '',
        tips: ''
      },
      detailContent: {
        text: '',
        tips: ''
      },
      detailTypeList: [
        {
          key: 1,
          value: '新增点位'
        },
        {
          key: 2,
          value: '减少点位'
        },
        {
          key: 3,
          value: '分配点位'
        },
        {
          key: 4,
          value: '释放点位'
        }
      ],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'id',
        order: 'desc'
      },
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      headOrder: {
        node_id: 1,
        node_name: 0,
        node_tip: 0,
        updated_time: 0,
        updated_by: 0
      }
    }
  },
  created () {
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    if (!user.is_super) {
      let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
      this.btn.save = !actions.some((item, index) => {
        return item.actionID === btn.eqp.save
      })
      this.btn.delete = !actions.some((item, index) => {
        return item.actionID === btn.eqp.delete
      })
      this.btn.update = !actions.some((item, index) => {
        return item.actionID === btn.eqp.update
      })
    }
    if (this.$route.query.pidcountid !== '' && this.$route.query.pidcountid !== null) {
      this.pidCountId = this.$route.query.pidcountid
    }
    this.init()
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  methods: {
    init () {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = 1
      this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.updated_by = 0
        this.headOrder.updated_time = 0
        this.currentSort.order = 'asc'
        this.headOrder[sort] = 1
      } else if (this.headOrder[sort] === 2) { // 同一字段降序变升序
        this.currentSort.order = 'asc'
        this.headOrder[sort] = 1
      } else { // 同一字段升序变降序
        this.currentSort.order = 'desc'
        this.headOrder[sort] = 2
      }
      this.currentSort.sort = sort
      this.bCheckAll = false
      this.checkAll()
      this.searchResult(this.currentPage)
    },
    // 搜索
    searchResult (page) {
      this.currentPage = page
      this.loading = true
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        pidCountId: this.pidCountId
      }
      api.getPidCountDetail(parm).then(res => {
        this.loading = false
        // console.log(res)
        res.data.rows.map(item => {
          item.updatedTime = transformDate(item.updatedTime)
          item.detailType = PidCountEnum.detailType[item.detailType]
          item.ccc = item.countNew - item.countOld
          if (item.ccc > 0) {
            item.ccc = '+' + item.ccc
          }
        })
        this.EqpList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
      api.getPidCountByID(this.pidCountId).then(res => {
        this.loading = false
        let _res = res.data
        this.nodeId = _res.nodeId
        this.nodeName = _res.nodeName
        this.nodeTip = _res.nodeTip
        this.capacityCount = _res.capacityCount
        this.usedCount = _res.usedCount
        this.remainCount = _res.remainCount
        this.remindCount = _res.remindCount
      }).catch(err => console.log(err))
    },

    add () {
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      let obj = {
        pidCountId: this.pidCountId,
        changeCount: this.changeCount.text,
        detailType: this.detailType.text,
        detailContent: this.detailContent.text
      }
      api.addPidCountDetail(obj).then(res => {
        if (res.code === 0) {
          this.$message({
            message: '提交成功',
            type: 'success'
          })
          this.searchResult(1)
          this.changeCount.text = ''
          this.detailContent.text = ''
        } else {
          this.$message({
            message: res.msg === '' ? '提交失败' : res.msg,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    // 修改设备
    edit () {
      if (!this.editObjID.length) {
        this.$message({
          message: '请选择需要修改的设备',
          type: 'warning'
        })
      } else if (this.editObjID.length > 1) {
        this.$message({
          message: '修改的设备不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddPidCount',
          params: {
            id: this.editObjID[0],
            mark: 'edit'
          }
        })
      }
    },
    // 查看设备明细
    detail () {
      if (!this.editObjID.length) {
        this.$message({
          message: '请选择需要查看的设备',
          type: 'warning'
        })
      } else if (this.editObjID.length > 1) {
        this.$message({
          message: '查看的设备不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'DetailEqp',
          params: {
            id: this.editObjID[0],
            sourceName: 'SeePidCountList'
          }
        })
      }
    },
    // 删除设备
    remove () {
      if (!this.editObjID.length) {
        this.$message({
          message: '请选择需要删除的设备',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条设备信息?'
      }
    },
    // 弹框确认是否删除
    dialogEnter () {
      api.delEqp(this.editObjID.join(',')).then(res => {
        if (res.code === 0) {
          this.editObjID = []
          this.$message({
            message: '删除成功',
            type: 'success'
          })
          this.currentPage = 1
          this.searchResult(1)
        } else {
          this.$message({
            message: res.msg,
            type: 'error'
          })
        }
        // 隐藏dialog
        this.dialogVisible.isShow = false
      }).catch(err => console.log(err))
    },
    // 搜索功能
    searchRes () {
      this.$emit('title', '| 设备别')
      this.loading = true
      this.init()
      // this.searchResult(1)
    },

    // 获取修改设备id
    emitEditID () {
      this.$emit('editObjID', this.editObjID)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.EqpList.map(val => this.editObjID.push(val.id)) : this.editObjID = []
      this.emitEditID()
    },

    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 上一页
    prevPage (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
    },

    // 下一页
    nextPage (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
    },
    validateSelect (val) {
      if (val.text === '') {
        val.tips = '此项必选'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateAll () {
      if (!this.validateSelect(this.detailType)) return false
      if (!this.validateNumber(this.changeCount)) return false
      if (!this.validateInput(this.detailContent)) return false
      return true
    },
    validateNumber (val) {
      val.tips = ''
      if (val.text === '') {
        val.tips = '此项必选'
        return false
      } else {
        return validateNumberCommon(val)
      }
    },
    // 验证
    validateInput (val) {
      return validateInputCommon(val)
    }
  }
}
</script>
<style lang="scss" scoped>
$con-height: $content-height - 145 - 56;
// 内容区
.content-wrap{
  overflow: hidden;
  height: percent($con-height, $content-height);
  text-align: center;
  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: percent(50, $con-height);
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);

    .last-update-time{
      color: $color-white;
    }
  }

  .scroll{
    height: percent($con-height - 50, $con-height);
  }

  .list-wrap{
    .list{
      &:nth-of-type(even){
        .list-content{
          background: rgba(186,186,186,.5);
        }
      }
    }

    .list-content{
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: PXtoEm(15) PXtoEm(24);

      div{
        word-break: break-all;
      }
    }

    .left-title{
      margin-right: 10px;
      font-weight: bold;
    }

    // 隐藏内容
    .sub-content{
      overflow: hidden;
      height: 0;
      font-size: $font-size-small;
      text-align: left;
      color: $color-content-text;

      &.active{
        overflow: inherit;
        height: auto;
        transition: .7s .2s;
      }
    }

    .sub-con-list{
      display: flex;
      padding: PXtoEm(15) PXtoEm(24);
      border-top: 1px solid $color-main-background;
      background: rgba(0,0,0,.2);

      .right-wrap{
        display: flex;
        flex-wrap: wrap;
      }

      .list{
        margin-right: 10px;
      }
    }
  }

  .number,
  .name,
  .btn-wrap{
    width: 10%;
  }

  .name{
    a{
      color: #42abfd;
    }
  }

  .last-update-time{
    width: 18%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 10%;
  }

  .state{
    width: 5%;
  }
}
</style>
<style>
.textareaPidCountContent
{
    display: inline-block;
    width: 86%;
    vertical-align: bottom;
    font-size: 14px;
    padding-left: 5.4%;
    padding-top: 2%;
}
/* .el-input__inner {
    width: 220px;
    border-top-width: 0px;
    border-left-width: 0px;
    border-right-width: 0px;
    border-bottom-width: 1px;
    outline: medium;
} */
</style>
