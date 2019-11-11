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
        <li class="list" @click="operation('pass')" :disabled="btn.pass"><x-button>审核通过</x-button></li>
        <li class="list" @click="operation('detail')" ><x-button>查看明细</x-button></li>
        <li class="list" :disabled="btn.pass">
        <el-popover
            popper-class="my-pop"
            placement="bottom"
            width="260"
            v-model="popVisiable">
            <el-input type="textarea" :rows="4" v-model="content" placeholder="请输入不驳回(限100字以内且必填)"></el-input>
            <div style="text-align: right;">
                <el-button size="mini" type="text" @click="operation('unPass')">驳回</el-button>
            </div>
            <el-button class="btn1" slot="reference">驳回</el-button>
        </el-popover>
        </li>
        <li class="list" @click="operation('history')" ><x-button>操作历史</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list checkbox"></li>
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
        <li class="list name">最新操作</li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in troubleList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <el-radio v-model="editTroubleID" :label="item.id + ',' + item.code"></el-radio>
                </div>
                <div class="name">{{ item.code }}</div>
                <div class="last-update-time color-white">{{ item.happeningTime }}</div>
                <div class="name">{{ item.lineName }}</div>
                <div class="name">{{ item.startLocationName }}</div>
                <div class="last-update-time color-white">{{ item.desc }}</div>
                <div class="name">{{ item.reportedCompanyName }}</div>
                <div class="name">{{ item.reportedByName }}</div>
                <div class="last-maintainer color-white">{{ item.statusName }}</div>
                <div class="last-maintainer color-white">{{ item.lastOperationName }}</div>
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
  </div>
</template>
<script>
import { transformDate, TroubleMenu, vInput } from '@/common/js/utils.js'
import { btn } from '@/element/btn.js'
import { troubleOperation } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
export default {
  name: 'MyCheck',
  components: {
    XButton
  },
  data () {
    return {
      btn: {
        pass: false
      },
      title: ' | 我的审核',
      popVisiable: false,
      desc: '',
      content: '',
      topOrg: '',
      searchDate: [],
      troubleList: [],
      troubleListByID: {},
      editTroubleID: '',
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
      this.btn.pass = !actions.some((item, index) => {
        return item.actionID === btn.myCheck.pass
      })
    }
    this.init()
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  methods: {
    operation (type) {
      if (this.editTroubleID === '') {
        this.$message({
          message: '请选择需要操作的报修故障',
          type: 'warning'
        })
      } else {
        let operation = ''
        let content = 'null'
        let arr = this.editTroubleID.split(',')
        switch (type) {
          case 'pass':
            operation = troubleOperation.pass
            content = this.content
            break
          case 'detail':
            this.$router.push({
              name: 'DetailTroubleReport',
              params: {
                id: arr[0],
                sourceName: 'MyCheck'
              }
            })
            break
          case 'unPass':
            if (this.validateInput()) {
              operation = troubleOperation.unpass
              content = this.content
              break
            } else {
              return
            }
          case 'history':
            this.$router.push({
              name: 'TroubleHistory',
              params: {
                id: arr[0],
                code: arr[1],
                sourceName: 'MyCheck'
              }
            })
            break
        }
        if (operation !== '') {
          api.Operation(arr[0], operation, content).then(res => {
            if (res.code === 0) {
              this.$message({
                message: '操作成功',
                type: 'success'
              })
              this.popVisiable = false
              this.currentPage = 1
              this.searchResult(1)
            } else {
              this.$message({
                message: res.msg,
                type: 'error'
              })
            }
          }).catch(err => console.log(err))
        }
      }
    },
    init () {
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
        StartTime: st,
        EndTime: et,
        MenuView: TroubleMenu.myCheck
      }
      api.getTroubleReportPage(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          this.troubleListByID[item.id] = item
          item.happeningTime = transformDate(item.happeningTime)
        })
        this.troubleList = res.data.rows
        this.total = res.data.total
        this.topOrg = res.data.repairCompany
      }).catch(err => console.log(err))
    },
    // 搜索功能
    searchRes () {
      this.loading = true
      this.init()
      this.searchResult(1)
    },

    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.currentPage = val
      this.searchResult(val)
    },

    // 上一页
    prevPage (val) {
      this.currentPage = val
    },

    // 下一页
    nextPage (val) {
      this.currentPage = val
    },
    validateInput () {
      if (this.content === '') {
        this.$message({
          message: '驳回原因必填',
          type: 'warning'
        })
        return false
      } else if (!vInput(this.content)) {
        this.$message({
          message: '驳回原因中含有非法字符',
          type: 'warning'
        })
        return false
      } else {
        return true
      }
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
      /deep/
      .el-radio__label{
        display: none;
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
  .checkbox{
    width: 2%;
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
/deep/
.btn1{
  background: none;
  border: 1px solid $color-main-btn-border;
  border-radius: $border-radius;
  color: $color-white;
  cursor: pointer;
  &.active,
  &:hover{
    border-color: $color-main-btn!important;
    background: $color-main-btn!important;
  }
}
.el-button--text:hover{
  color: #4998d4;
  border-color: transparent;
  background-color: transparent;
  border: none;
}
</style>
<style>
.my-pop{
  background: #29282E!important;
  border-color: #29282E!important;
}
</style>
