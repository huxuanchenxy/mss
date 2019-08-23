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
            <label for="">子系统</label>
            <div class="inp">
              <el-select v-model="subSystem" clearable filterable placeholder="请选择">
                <el-option
                  v-for="item in subSystemList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">设备类型</label>
            <div class="inp">
              <el-select v-model="eqpType" clearable filterable placeholder="请选择">
                <el-option
                  v-for="item in eqpTypeList"
                  :key="item.key"
                  :label="item.tName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="name">设备图纸编码</label>
            <div class="inp">
              <el-input v-model.trim="eqpCode" placeholder="请输入设备图纸编码"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="">安装位置</label>
            <div class="inp">
              <el-cascader clearable
                change-on-select
                :props="areaParams"
                :show-all-levels="true"
                :options="areaList"
                v-model="area">
              </el-cascader>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="add"><x-button :disabled="btn.save">添加</x-button></li>
        <li class="list" @click="remove"><x-button :disabled="btn.delete">删除</x-button></li>
        <li class="list" @click="edit"><x-button :disabled="btn.update">修改</x-button></li>
        <li class="list" @click="detail"><x-button>查看明细</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox" v-model="bCheckAll" @change="checkAll"></li>
        <li class="list number c-pointer" @click="changeOrder('eqp_code')">
          图纸编号
          <i :class="[{ 'el-icon-d-caret': headOrder.eqp_code === 0 }, { 'el-icon-caret-top': headOrder.eqp_code === 1 }, { 'el-icon-caret-bottom': headOrder.eqp_code === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('eqp_name')">
          名称
          <i :class="[{ 'el-icon-d-caret': headOrder.eqp_name === 0 }, { 'el-icon-caret-top': headOrder.eqp_name === 1 }, { 'el-icon-caret-bottom': headOrder.eqp_name === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('sub_system')">
          子系统
          <i :class="[{ 'el-icon-d-caret': headOrder.sub_system === 0 }, { 'el-icon-caret-top': headOrder.sub_system === 1 }, { 'el-icon-caret-bottom': headOrder.sub_system === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('eqp_type')">
          类型
          <i :class="[{ 'el-icon-d-caret': headOrder.eqp_type === 0 }, { 'el-icon-caret-top': headOrder.eqp_type === 1 }, { 'el-icon-caret-bottom': headOrder.eqp_type === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('team')">
          管辖班组
          <i :class="[{ 'el-icon-d-caret': headOrder.team === 0 }, { 'el-icon-caret-top': headOrder.team === 1 }, { 'el-icon-caret-bottom': headOrder.team === 2 }]"></i>
        </li>
        <li class="list number">安装位置</li>
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
                <div class="checkbox">
                  <input type="checkbox" v-model="editEqpID" :value="item.id" @change="emitEditID">
                </div>
                <div class="number">{{ item.code }}</div>
                <div class="name">{{ item.name }}</div>
                <div class="name">{{ item.subSystemName }}</div>
                <div class="name">{{ item.tName }}</div>
                <div class="number">{{ item.teamName }}</div>
                <div class="number">{{ item.locationName }}</div>
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
import { transformDate } from '@/common/js/utils.js'
import { dictionary } from '@/common/js/dictionary.js'
import { btn } from '@/element/btn.js'
import XButton from '@/components/button'
import apiAuth from '@/api/authApi'
import api from '@/api/eqpApi'
import apiArea from '@/api/AreaApi.js'
export default {
  name: 'SeeEqpList',
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
      areaParams: {
        label: 'areaName',
        value: 'id',
        children: 'children'
      },
      title: ' | 设备定义',
      eqpCode: '',
      subSystem: '',
      subSystemList: [],
      eqpType: '',
      eqpTypeList: [],
      area: [],
      areaList: [],
      EqpList: [],
      editEqpID: [],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'eqp_code',
        order: 'asc'
      },
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      headOrder: {
        eqp_code: 1,
        eqp_name: 0,
        sub_system: 0,
        eqp_type: 0,
        team: 0,
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
    if (this.$route.params.id !== '' && this.$route.params.id !== null) {
      this.eqp = this.$route.params.id
    }
    this.init()

    // 子系统加载
    apiAuth.getSubCode(dictionary.subSystem).then(res => {
      this.subSystemList = res.data
    }).catch(err => console.log(err))

    // 设备类型加载
    api.getEqpTypeAll().then(res => {
      this.eqpTypeList = res.data
      if (this.$route.params.id !== undefined) {
        this.eqpType = this.$route.params.id
        this.searchResult(1)
      }
    }).catch(err => console.log(err))

    // 安装位置加载
    apiArea.SelectConfigAreaData().then(res => {
      this.areaList = res.data.dicAreaList
    }).catch(err => console.log(err))
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
        this.headOrder.eqp_code = 0
        this.headOrder.eqp_name = 0
        this.headOrder.sub_system = 0
        this.headOrder.eqp_type = 0
        this.headOrder.team = 0
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
      let l = this.area.length - 1
      if (l === -1) {
        l = ''
      }
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        SearchSubSystem: this.subSystem,
        SearchCode: this.eqpCode,
        SearchType: this.eqpType,
        SearchLocation: this.area[l],
        SearchLocationBy: l
      }
      api.getEqp(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.updatedTime = transformDate(item.updatedTime)
        })
        this.EqpList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },

    add () {
      this.$router.push({
        name: 'AddEqp',
        params: {
          mark: 'add'
        }
      })
    },
    // 修改设备
    edit () {
      if (!this.editEqpID.length) {
        this.$message({
          message: '请选择需要修改的设备',
          type: 'warning'
        })
      } else if (this.editEqpID.length > 1) {
        this.$message({
          message: '修改的设备不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddEqp',
          params: {
            id: this.editEqpID[0],
            mark: 'edit'
          }
        })
      }
    },
    // 查看设备明细
    detail () {
      if (!this.editEqpID.length) {
        this.$message({
          message: '请选择需要查看的设备',
          type: 'warning'
        })
      } else if (this.editEqpID.length > 1) {
        this.$message({
          message: '查看的设备不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'DetailEqp',
          params: {
            id: this.editEqpID[0],
            sourceName: 'SeeEqpList'
          }
        })
      }
    },
    // 删除设备
    remove () {
      if (!this.editEqpID.length) {
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
      api.delEqp(this.editEqpID.join(',')).then(res => {
        if (res.code === 0) {
          this.editEqpID = []
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
      this.$emit('editEqpID', this.editEqpID)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.EqpList.map(val => this.editEqpID.push(val.id)) : this.editEqpID = []
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
      this.searchResult(val)
    },

    // 下一页
    nextPage (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
      this.searchResult(val)
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
