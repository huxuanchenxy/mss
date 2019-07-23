<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <!-- <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }} -->
        <img src="/MenuIcon/09警报管理-1.svg" alt="" class="icon"> 事件中心 {{ title }}
      </h2>
    </div>
    <div class="box">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
          <div class="input-group">
            <label for="name">设备类型</label>
            <div class="inp">
              <el-select v-model="eqpTypeID" placeholder="请选择">
                <el-option label="所有设备类型" value=''></el-option>
                <el-option
                  v-for="item in eqpTypeList"
                  :key="item.key"
                  :label="item.tName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="list">
            <span class="lable">发生时间</span>
            <el-date-picker
              v-model="time"
              type="daterange"
              prefix-icon="el-icon-date"
              :unlink-panels="true"
              range-separator="至"
              start-placeholder="开始日期"
              end-placeholder="结束日期">
            </el-date-picker>
          </div>
        </div>
        <div class="search-btn" @click="search">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">

      <el-tabs v-model="activeName">
        <el-tab-pane label="报警历史" name="alarm">
          <div>
            <ul class="content-header">
              <li class="list number">
                设备编号
              </li>
              <li class="list number">
                设备名称
              </li>
              <li class="list number">
                设备类型
              </li>
              <li class="list number">
                等级
              </li>
              <li class="list number">
                内容
              </li>
              <li class="list last-update-time">
                发生时间
              </li>
            </ul>
            <div class="scroll">
              <el-scrollbar>
                <ul class="list-wrap">
                  <li class="list" v-for="(item) in AlarmList" :key="item.key">
                    <div class="list-content">
                      <div class="number">{{ item.eqpCode }}</div>
                      <div class="number">{{ item.eqpName }}</div>
                      <div class="number">{{ item.eqpTypeName }}</div>
                      <div class="number">{{ item.level }}</div>
                      <div class="number">{{ item.content }}</div>
                      <div class="last-update-time">{{ transformDate(item.createdTime) }}</div>
                    </div>
                  </li>
                </ul>
              </el-scrollbar>
            </div>
          </div>
        </el-tab-pane>
        <el-tab-pane label="预警历史" name="warnning">
          <div>
            <ul class="content-header">
              <li class="list number">
                设备编号
              </li>
              <li class="list number">
                设备名称
              </li>
              <li class="list number">
                设备类型
              </li>
              <li class="list number">
                内容
              </li>
              <li class="list last-update-time c-pointer" @click="changeOrder('created_time')">
                发生时间
                <i :class="[{ 'el-icon-d-caret': headOrder.created_time === 0 }, { 'el-icon-caret-top': headOrder.created_time === 1 }, { 'el-icon-caret-bottom': headOrder.created_time === 2 }]"></i>
              </li>
            </ul>
            <div class="scroll">
              <el-scrollbar>
                <ul class="list-wrap">
                  <li class="list" v-for="(item) in WarnList" :key="item.key">
                    <div class="list-content">
                      <div class="number">{{ item.eqpCode }}</div>
                      <div class="number">{{ item.eqpName }}</div>
                      <div class="number">{{ item.eqpTypeName }}</div>
                      <div class="number">{{ item.content }}</div>
                      <div class="last-update-time">{{ transformDate(item.createdTime) }}</div>
                    </div>
                  </li>
                </ul>
              </el-scrollbar>
            </div>
          </div>
        </el-tab-pane>
        <el-tab-pane label="通知历史" name="notification">
          <div>
            <ul class="content-header">
              <li class="list number">
                设备编号
              </li>
              <li class="list number">
                设备名称
              </li>
              <li class="list number">
                设备类型
              </li>
              <li class="list number">
                通知类型
              </li>
              <li class="list number">
                内容
              </li>
              <li class="list last-update-time c-pointer" @click="changeOrder('created_time')">
                发生时间
                <i :class="[{ 'el-icon-d-caret': headOrder.created_time === 0 }, { 'el-icon-caret-top': headOrder.created_time === 1 }, { 'el-icon-caret-bottom': headOrder.created_time === 2 }]"></i>
              </li>
            </ul>
            <div class="scroll">
              <el-scrollbar>
                <ul class="list-wrap">
                  <li class="list" v-for="(item) in NotificationList" :key="item.key">
                    <div class="list-content">
                      <div class="number">{{ item.eqpCode }}</div>
                      <div class="number">{{ item.eqpName }}</div>
                      <div class="number">{{ item.eqpTypeName }}</div>
                      <div class="number">{{ item.notificationTypeName }}</div>
                      <div class="number">{{ item.content }}</div>
                      <div class="last-update-time">{{ transformDate(item.createdTime) }}</div>
                    </div>
                  </li>
                </ul>
              </el-scrollbar>
            </div>
          </div>
        </el-tab-pane>
      </el-tabs>
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
    </div>
  </div>
</template>
<script>
import { ApiRESULT, transformDate } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/eventCenterApi'
import eqpApi from '@/api/eqpApi'
export default {
  name: 'SeeUserList',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 消息历史',
      loading: false,
      eqpTypeID: null,
      eqpTypeList: [],
      time: '', // 时间
      AlarmList: [],
      WarnList: [],
      NotificationList: [],
      activeName: 'alarm',
      timer: null,

      total: 0,
      currentPage: 1,
      currentSort: {
        sort: 'created_time',
        order: 'desc'
      },
      headOrder: {
        id: 0,
        created_time: 2
      }
    }
  },
  created () {
    this.getAllEqpType()
  },
  watch: {
    $route () {
      this.activeName = this.$route.params.type
    },
    activeName () {
      this.eqpTypeID = ''
      this.time = ''
      this.search()
    }
  },
  activated () {
    this.activeName = this.$route.params.type
    this.search()
  },
  methods: {
    transformDate (val) {
      return transformDate(val)
    },
    otherStyleDate (date) {
      return date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate()
    },
    getAllEqpType () {
      eqpApi.getEqpTypeAll().then(res => {
        this.eqpTypeList = res.data
      }).catch(err => console.log(err))
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.created_time = 0
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
      this.search()
    },
    search () {
      switch (this.activeName) {
        case 'alarm':
          this.getAlarm()
          break
        case 'warnning':
          this.getWarnning()
          break
        case 'notification':
          this.getNotification()
          break
      }
    },
    getWarnning () {
      let param = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: this.currentPage,
        eqpTypeID: this.eqpTypeID,
        startTime: this.time ? this.time[0] : '',
        endTime: this.time ? this.otherStyleDate(this.time[1]) + ' 23:59:59' : ''
      }
      this.loading = true
      api.getWarningHistory(param).then(res => {
        this.loading = false
        if (res.code === ApiRESULT.Success) {
          this.WarnList = res.data.rows
          this.total = res.data.total
        }
      }).catch(err => console.log(err))
    },
    getNotification () {
      let param = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: this.currentPage,
        eqpTypeID: this.eqpTypeID,
        startTime: this.time ? this.time[0] : '',
        endTime: this.time ? this.otherStyleDate(this.time[1]) + ' 23:59:59' : ''
      }
      this.loading = true
      api.getNotificationHistory(param).then(res => {
        this.loading = false
        if (res.code === ApiRESULT.Success) {
          this.NotificationList = res.data.rows
          this.total = res.data.total
        }
      }).catch(err => console.log(err))
    },
    getAlarm () {
      // this.loading = true
      api.getAlarmHistory().then(res => {
        // this.loading = false
        if (res.code === ApiRESULT.Success) {
          this.AlarmList = res.data.rows
          this.total = res.data.total
        }
      }).catch(err => console.log(err))
    },

    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.currentPage = val
      this.search()
    },

    // 上一页
    prevPage (val) {
      this.currentPage = val
      this.search()
    },

    // 下一页
    nextPage (val) {
      this.currentPage = val
      this.search()
    }
  }
}
</script>
<style lang="scss" scoped>
$con-height: $content-height - 100;
// 内容区
.content-wrap{
  overflow: hidden;
  height: percent($con-height, $content-height);
  text-align: center;
  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 30px;
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
  /deep/ .el-checkbox__label{
    display: none;
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
// 时间选择
  .top-input-group{
    display: flex;
    justify-content: space-between;
    padding: 15px 0;
  }

/deep/ .el-date-editor{
  width: 380px;

  .el-input__icon{
    height: initial;
  }

  .el-range-separator{
    height: initial;
    color: $color-white;
  }

  .el-range-input{
    color: $color-white;
  }
}
</style>
