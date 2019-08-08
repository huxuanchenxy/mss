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
            <label for="name">服务名称</label>
            <div class="inp">
              <el-input v-model.trim="serviceName" placeholder="请输入服务名称"></el-input>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="look"><x-button>查看详情</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox" v-model="bCheckAll" style="visibility: hidden;"></li>
        <li class="list number c-pointer" @click="changeOrder('serviceName')">
          服务名称
          <i :class="[{ 'el-icon-d-caret': headOrder.serviceName === 0 }, { 'el-icon-caret-top': headOrder.serviceName === 1 }, { 'el-icon-caret-bottom': headOrder.serviceName === 2 }]"></i>
       </li>
        <li class="list number c-pointer" @click="changeOrder('serviceAddr')">
          主机ip
          <i :class="[{ 'el-icon-d-caret': headOrder.serviceAddr === 0 }, { 'el-icon-caret-top': headOrder.serviceAddr === 1 }, { 'el-icon-caret-bottom': headOrder.serviceAddr === 2 }]"></i>
       </li>
       <li class="list number c-pointer" @click="changeOrder('servicePort')">
          服务端口
          <i :class="[{ 'el-icon-d-caret': headOrder.servicePort === 0 }, { 'el-icon-caret-top': headOrder.servicePort === 1 }, { 'el-icon-caret-bottom': headOrder.servicePort === 2 }]"></i>
       </li>
        <li class="list number c-pointer" @click="changeOrder('healthStatus')" >
          健康状态
          <i :class="[{ 'el-icon-d-caret': headOrder.healthStatus === 0 }, { 'el-icon-caret-top': headOrder.healthStatus === 1 }, { 'el-icon-caret-bottom': headOrder.healthStatus === 2 }]"></i>
       </li>
        <li class="list number c-pointer" @click="changeOrder('servicePID')" >进程pid
          <i :class="[{ 'el-icon-d-caret': headOrder.servicePID === 0 }, { 'el-icon-caret-top': headOrder.servicePID === 1 }, { 'el-icon-caret-bottom': headOrder.servicePID === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('updatedTime')">最后启动时间
          <i :class="[{ 'el-icon-d-caret': headOrder.updatedTime === 0 }, { 'el-icon-caret-top': headOrder.updatedTime === 1 }, { 'el-icon-caret-bottom': headOrder.updatedTime === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in UserList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="lookOperlogID" :value="item.id" @change="emitEditID">
                </div>
                <div class="name">{{ item.serviceName }}</div>
                <div class="name">{{ item.serviceAddr }}</div>
                <!--<div class="name">
                  <router-link :to="{ name: 'SeeActionList', params: { id: item.id } }">{{ item.servicePort }}</router-link>
                </div>-->
                <div class="name">{{ item.servicePort }}</div>
                <div class="name">
                  <el-switch
                    v-model="item.healthStatus"
                    :disabled='item.healthStatus'
                    active-color="#13ce66"
                    inactive-color="#ff4949" @change='changeStatus($event,item)'>
                  </el-switch>
                </div>
                <div class="name">{{ item.servicePID }}</div>
                <!-- <div class="name">{{ item.mac_add }}</div> -->
                <div class="name">{{ item.updatedTime }}</div>
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
import api from '@/api/processConsulApi'
export default {
  name: 'SeeProcessConsul',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 服务监控',
      time: '',
      serviceName: '',
      startTime: '',
      endTime: '',
      role: '',
      roleList: [],
      UserList: [],
      lookOperlogID: [],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'service_name',
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
        healthStatus: 0,
        serviceName: 0,
        action_name: 0,
        serviceAddr: 0,
        servicePort: 0,
        servicePID: 0,
        mac_add: 0,
        updatedTime: 0
      }
    }
  },
  created () {
    this.$emit('title', '| 服务监控')
    this.init()
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  methods: {
    init () {
      // this.bCheckAll = false
      // this.checkAll()
      this.currentPage = 1
      this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.healthStatus = 0
        this.headOrder.servicePort = 0
        this.headOrder.serviceName = 0
        this.headOrder.serviceAddr = 0
        this.headOrder.servicePID = 0
        this.headOrder.mac_add = 0
        this.headOrder.updatedTime = 0
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
        serviceName: this.serviceName
      }
      api.getPage(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.updatedTime = transformDate(item.updatedTime)
          item.healthStatus = item.healthStatus === 1
        })
        this.UserList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    // 搜索功能
    searchRes () {
      this.$emit('title', '| 操作日志')
      this.loading = true
      this.init()
      this.searchResult(1)
    },
    // 获取修改用户id
    emitEditID () {
      this.$emit('lookOperlogID', this.lookOperlogID)
    },
    // 全选
    // checkAll () {
    //   this.bCheckAll ? this.UserList.map(val => this.editUserID.push(val.id)) : this.editUserID = []
    //   this.emitEditID()
    // },
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
    look () {
      if (!this.lookOperlogID.length) {
        this.$message({
          message: '请选择需要查看的日志详情',
          type: 'warning'
        })
      } else if (this.lookOperlogID.length > 1) {
        this.$message({
          message: '查看的日志详情不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddOperlog',
          params: {
            id: this.lookOperlogID[0],
            mark: 'look'
          }
        })
      }
    },
    changeStatus: function ($event, item) {
      alert($event)
      alert(item.id)
      if ($event) {
        api.startProcess(item.id).then(res => {
          this.loading = false
        }).catch(err => console.log(err))
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
