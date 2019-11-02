<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
    </div>
    <div class="box">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
          <div class="input-group">
            <label for="name">故障现象关键字</label>
            <div class="inp">
              <el-input v-model.trim="desc" placeholder="请输入故障现象关键字"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">故障状态</label>
            <div class="inp">
              <el-select v-model="troubleStatus" clearable filterable placeholder="请选择故障状态">
                <el-option
                  v-for="item in troubleStatusList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="name">发生日期</label>
            <div class="inp">
              <el-date-picker
                v-model="searchDate"
                type="daterange"
                prefix-icon="el-icon-date"
                :unlink-panels="true"
                value-format="yyyy-MM-dd"
                range-separator="至"
                start-placeholder="开始日期"
                end-placeholder="结束日期">
              </el-date-picker>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list">
          <x-button :disabled="btn.save">
            <router-link :to="{ name: 'AddTrouble', params: { type: 'Add' } }">添加</router-link>
          </x-button>
        </li>
        <li class="list" @click="remove" :disabled="btn.delete"><x-button>取消</x-button></li>
        <li class="list" @click="edit" :disabled="btn.update"><x-button>修改</x-button></li>
        <li class="list" @click="detail" ><x-button>查看明细</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox" v-model="bCheckAll" @change="checkAll"></li>
        <li class="list name c-pointer" @click="changeOrder('code')">
          故障编号
          <i :class="[{ 'el-icon-d-caret': headOrder.code === 0 }, { 'el-icon-caret-top': headOrder.code === 1 }, { 'el-icon-caret-bottom': headOrder.code === 2 }]"></i>
        </li>
        <li class="list last-update-time c-pointer" @click="changeOrder('happening_time')">
          发生时间
          <i :class="[{ 'el-icon-d-caret': headOrder.happening_time === 0 }, { 'el-icon-caret-top': headOrder.happening_time === 1 }, { 'el-icon-caret-bottom': headOrder.happening_time === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('line')">
          线路
          <i :class="[{ 'el-icon-d-caret': headOrder.line === 0 }, { 'el-icon-caret-top': headOrder.line === 1 }, { 'el-icon-caret-bottom': headOrder.line === 2 }]"></i>
        </li>
        <li class="list name">起始位置</li>
        <li class="list last-update-time">故障现象</li>
        <li class="list name">报修单位</li>
        <li class="list name">报修人</li>
        <li class="list name c-pointer" @click="changeOrder('status')">
          状态
          <i :class="[{ 'el-icon-d-caret': headOrder.status === 0 }, { 'el-icon-caret-top': headOrder.status === 1 }, { 'el-icon-caret-bottom': headOrder.status === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in troubleList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="editTroubleID" :value="item.id" @change="emitEditID">
                </div>
                <div class="name">{{ item.code }}</div>
                <div class="last-update-time color-white">{{ item.happeningTime }}</div>
                <div class="name">{{ item.lineName }}</div>
                <div class="name">{{ item.startLocationName }}</div>
                <div class="last-update-time color-white">{{ item.desc }}</div>
                <div class="name">{{ item.reportedCompanyName }}</div>
                <div class="name">{{ item.reportedByName }}</div>
                <div class="last-maintainer color-white">{{ item.statusName }}</div>
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
import { transformDate } from '@/common/js/utils.js'
import { troubleStatus, dictionary } from '@/common/js/dictionary.js'
import { btn } from '@/element/btn.js'
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
import apiAuth from '@/api/authApi'
export default {
  name: 'SeeTroubleList',
  components: {
    XButton
  },
  data () {
    return {
      btn: {
        save: false,
        delete: false,
        update: false
      },
      title: ' | 故障报修',
      desc: '',
      searchDate: [],
      troubleStatus: '',
      troubleStatusList: [],
      troubleList: [],
      troubleListByID: {},
      editTroubleID: [],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'id',
        order: 'asc'
      },
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      headOrder: {
        id: 1,
        happening_time: 0,
        line: 0,
        status: 0
      }
    }
  },
  created () {
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    if (!user.is_super) {
      let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
      this.btn.save = !actions.some((item, index) => {
        return item.actionID === btn.troubleReport.save
      })
      this.btn.delete = !actions.some((item, index) => {
        return item.actionID === btn.troubleReport.delete
      })
      this.btn.update = !actions.some((item, index) => {
        return item.actionID === btn.troubleReport.update
      })
    }
    // 故障状态列表
    apiAuth.getSubCode(dictionary.troubleStatus).then(res => {
      this.troubleStatusList = res.data
    }).catch(err => console.log(err))
    this.init()
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  methods: {
    detail () {
      if (!this.editTroubleID.length) {
        this.$message({
          message: '请选择需要查看明细的报修故障',
          type: 'warning'
        })
      } else if (this.editTroubleID.length > 1) {
        this.$message({
          message: '查看明细的报修故障不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'DetailTroubleReport',
          params: {
            id: this.editTroubleID[0],
            sourceName: 'SeeTroubleList'
          }
        })
      }
    },
    init () {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = 1
      // this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.happening_time = 0
        this.headOrder.line = 0
        this.headOrder.status = 0
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
      let st, et
      if (this.searchDate != null && this.searchDate.length !== 0) {
        st = this.searchDate[0] + ' 00:00:00'
        et = this.searchDate[1] + ' 23:59:59'
      }
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        TroubleReportDesc: this.desc,
        TroubleStatus: this.troubleStatus,
        StartTime: st,
        EndTime: et
      }
      api.getTroubleReportPage(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          this.troubleListByID[item.id] = item
          item.happeningTime = transformDate(item.happeningTime)
        })
        this.troubleList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },

    // 修改报修故障
    edit () {
      if (!this.editTroubleID.length) {
        this.$message({
          message: '请选择需要修改的报修故障',
          type: 'warning'
        })
      } else if (this.editTroubleID.length > 1) {
        this.$message({
          message: '修改的报修故障不能超过1个',
          type: 'warning'
        })
      } else if (!this.isNewTrouble()) {
        this.$message({
          message: '非新报修的故障不允许修改',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddTrouble',
          params: {
            id: this.editTroubleID[0],
            type: 'edit'
          }
        })
      }
    },
    isNewTrouble () {
      return this.troubleListByID[this.editTroubleID].status === troubleStatus.newTrouble
    },
    // 取消报修故障
    remove () {
      if (!this.editTroubleID.length) {
        this.$message({
          message: '请选择需要取消的报修故障',
          type: 'warning'
        })
      } else if (!this.isNewTrouble()) {
        this.$message({
          message: '非新报修的故障不允许取消',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定取消该条报修故障信息?'
      }
    },
    // 弹框确认是否取消
    dialogEnter () {
      api.UpdateTroubleStatus(this.editTroubleID.join(','), troubleStatus.canceled).then(res => {
        if (res.code === 0) {
          this.editTroubleID = []
          this.$message({
            message: '取消成功',
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
      this.loading = true
      this.init()
      this.searchResult(1)
    },

    // 获取修改报修故障id
    emitEditID () {
      this.$emit('editTroubleID', this.editTroubleID)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.troubleList.map(val => this.editTroubleID.push(val.id)) : this.editTroubleID = []
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

  .number{
    width: 6%;
  }
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
