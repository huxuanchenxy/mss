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
            <label for="name">施工计划名称</label>
            <div class="inp">
              <el-input v-model.trim="planName" placeholder="请输入施工计划名称" clearable></el-input>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
            <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="add"><x-button>添加</x-button></li>
        <!-- <li class="list" @click="remove"><x-button :disabled="btn.delete">删除</x-button></li> -->
        <!-- <li class="list" @click="edit"><x-button :disabled="btn.update">修改</x-button></li> -->
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox" v-model="bCheckAll" style="visibility: hidden;"></li>
        <li class="list number c-pointer" @click="changeOrder('id')">
          ID
          <i :class="[{ 'el-icon-d-caret': headOrder.id === 0 }, { 'el-icon-caret-top': headOrder.id === 1 }, { 'el-icon-caret-bottom': headOrder.id === 2 }]"></i>
       </li>
        <li class="list name c-pointer" @click="changeOrder('planName')">
          计划名称
          <i :class="[{ 'el-icon-d-caret': headOrder.planName === 0 }, { 'el-icon-caret-top': headOrder.planName === 1 }, { 'el-icon-caret-bottom': headOrder.planName === 2 }]"></i>
       </li>
       <li class="list number c-pointer" @click="changeOrder('appInstanceID')" style="display:none;">
          业务ID
          <i :class="[{ 'el-icon-d-caret': headOrder.appInstanceID === 0 }, { 'el-icon-caret-top': headOrder.appInstanceID === 1 }, { 'el-icon-caret-bottom': headOrder.appInstanceID === 2 }]"></i>
       </li>
        <li class="list name c-pointer" @click="changeOrder('createdTime')">创建日期
          <i :class="[{ 'el-icon-d-caret': headOrder.createdTime === 0 }, { 'el-icon-caret-top': headOrder.createdTime === 1 }, { 'el-icon-caret-bottom': headOrder.createdTime === 2 }]"></i>
        </li>
          <li class="list number c-pointer" style="display:none;">
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in DataList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="IDS" :value="item.id" @change="emitEditID">
                </div>
                <div class="number">{{ item.id }}</div>
                <div class="name">{{ item.planName }}</div>
                <!--<div class="name">
                  <router-link :to="{ name: 'SeeActionList', params: { id: item.id } }">{{ item.appInstanceID }}</router-link>
                </div>-->
                <!-- <div class="name">{{ item.mac_add }}</div> -->
                <div class="name">{{ item.createdTime }}</div>
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
import { transformDate } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/workflowApi'
export default {
  name: 'SeeConstructionPlan',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 施工计划',
      time: '',
      id: '',
      startTime: '',
      endTime: '',
      planName: '',
      roleList: [],
      DataList: [],
      IDS: [],
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
        id: 0,
        processState: 0,
        planName: 0,
        appInstanceID: 0,
        createdTime: 0
      }
    }
  },
  created () {
    this.$emit('title', '| 我的申请')
    this.init()
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  methods: {
    init () {
      // this.bCheckAll = false  :disabled='item.processState'
      // this.checkAll()
      this.currentPage = 1
      this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.processState = 0
        this.headOrder.appInstanceID = 0
        this.headOrder.planName = 0
        this.headOrder.createdTime = 0
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
      // this.bCheckAll = false
      // this.checkAll()
      this.searchResult(1)
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
        AppName: this.planName
      }
      api.getConstructionPlanPage(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.createdTime = transformDate(item.createdTime)
        })
        this.DataList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    // 搜索功能
    searchRes () {
      this.$emit('title', '| 我的申请')
      this.loading = true
      this.init()
      this.searchResult(1)
    },
    add () {
      this.$router.push({name: 'AddConstructionPlan', query: { type: 'Add' }})
    },
    edit () {
      if (!this.IDS.length) {
        this.$message({
          message: '请选择记录',
          type: 'warning'
        })
      } else if (this.IDS.length > 1) {
        this.$message({
          message: '查看的记录不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'SeeProcessHis',
          params: {
            ProcessInstanceID: this.IDS[0]
          }
        })
      }
    },
    // 全选
    // checkAll () {
    //   this.bCheckAll ? this.DataList.map(val => this.editUserID.push(val.id)) : this.editUserID = []
    //   this.emitEditID()
    // },
    emitEditID () {
      this.$emit('IDS', this.IDS)
    },
    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.bCheckAll = false
      // this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 上一页
    prevPage (val) {
      this.bCheckAll = false
      // this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 下一页
    nextPage (val) {
      this.bCheckAll = false
      // this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },
    refreshpage () {
      // this.$router.go(0)
      this.currentPage = 1
      this.loading = true
      this.searchResult(1)
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
      padding: 13px 6px;
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
    width: 13%;
  }

  .name{
    a{
      color: #42abfd;
    }
    width:30%;
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
.content-header{
  height: 7.45455% !important;
}.content-wrap {
    height: 83.45455% !important;
}.inp {
  width: 200px !important;
}
</style>
