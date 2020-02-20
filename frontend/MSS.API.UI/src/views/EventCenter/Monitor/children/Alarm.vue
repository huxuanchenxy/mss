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
    <!-- 内容 -->
    <div class="content-wrap">

      <el-tabs class="tab-height" v-model="activeName">
        <el-tab-pane label="报警" name="alarm">
          <el-container style="height:100%;">
            <el-header height="200px" style="padding:0px">
              <el-container style="height:100%;">
                <div><span class="headtip">一级报警</span></div>
                <el-header height="40px" style="padding:0px">
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
                      属性
                    </li>
                    <li class="list number">
                      内容
                    </li>
                    <li class="list last-update-time">
                      发生时间
                    </li>
                  </ul>
                </el-header>
                <el-main style="padding:0px">
                  <ul class="list-wrap">
                    <li class="list" v-for="(item) in AlarmList1" :key="item.key">
                      <div class="list-content">
                        <div class="number">{{ item.eqpCode }}</div>
                        <div class="number">{{ item.eqpName }}</div>
                        <div class="number">{{ item.eqpTypeName }}</div>
                        <div class="number">{{ item.eLevel }}</div>
                        <div class="number">{{ item.pidDes }}</div>
                        <div class="number">{{ item.des }}</div>
                        <div class="last-update-time">{{ transformDate(item.originTime) }}</div>
                      </div>
                    </li>
                  </ul>
                </el-main>
              </el-container>
            </el-header>
            <el-main style="padding:0px;padding-top: 28px;">
              <!-- level 大于2 -->
              <el-container style="height:100%;">
                <div><span class="headtip">其他级别报警</span></div>
                <el-header height="40px" style="padding:0px">
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
                      属性
                    </li>
                    <li class="list number">
                      内容
                    </li>
                    <li class="list last-update-time">
                      发生时间
                    </li>
                  </ul>
                </el-header>
                <el-main style="padding:0px">
                  <ul class="list-wrap">
                    <li class="list" v-for="(item) in AlarmList2" :key="item.key">
                      <div class="list-content">
                        <div class="number">{{ item.eqpCode }}</div>
                        <div class="number">{{ item.eqpName }}</div>
                        <div class="number">{{ item.eqpTypeName }}</div>
                        <div class="number">{{ item.eLevel }}</div>
                        <div class="number">{{ item.pidDes }}</div>
                        <div class="number">{{ item.des }}</div>
                        <div class="last-update-time">{{ transformDate(item.originTime) }}</div>
                      </div>
                    </li>
                  </ul>
                </el-main>
              </el-container>
            </el-main>
          </el-container>
        </el-tab-pane>
        <el-tab-pane class="pane-height pane-notification" label="预警" name="warnning">
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
            <li class="list content">
              内容
            </li>
            <li class="list last-update-time">
              发生时间
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
                    <div class="content">{{ item.content }}</div>
                    <div class="last-update-time">{{ transformDate(item.createdTime) }}</div>
                  </div>
                </li>
              </ul>
            </el-scrollbar>
          </div>
        </el-tab-pane>
        <el-tab-pane class="pane-height pane-notification" label="通知" name="notification">
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
            <li class="list content">
              内容
            </li>
            <li class="list last-update-time">
              发生时间
            </li>
            <li class="list number">
              操作
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
                    <div class="content">{{ item.content }}</div>
                    <div class="last-update-time">{{ transformDate(item.createdTime) }}</div>
                    <div class="number">
                      <li @click="confirmNotification(item.id)"><x-button>确认</x-button></li>
                    </div>
                  </div>
                </li>
              </ul>
            </el-scrollbar>
          </div>
        </el-tab-pane>
        <el-tab-pane class="pane-height pane-notification" label="点位预警" name="notificationpidcount">
          <ul class="content-header">
            <li class="list number">
              车站
            </li>
            <li class="list content">
              内容
            </li>
            <li class="list last-update-time">
              发生时间
            </li>
            <li class="list number">
              操作
            </li>
          </ul>
          <div class="scroll">
            <el-scrollbar>
              <ul class="list-wrap">
                <li class="list" v-for="(item) in NotificationPidCountList" :key="item.key">
                  <div class="list-content">
                    <div class="content">{{ item.pidCountName }}</div>
                    <div class="content">{{ item.content }}</div>
                    <div class="last-update-time">{{ transformDate(item.createdTime) }}</div>
                    <div class="number">
                      <li @click="confirmNotification(item.id)"><x-button>确认</x-button></li>
                    </div>
                  </div>
                </li>
              </ul>
            </el-scrollbar>
          </div>
        </el-tab-pane>
      </el-tabs>
    </div>
  </div>
</template>
<script>
import { ApiRESULT, transformDate } from '@/common/js/utils.js'
import XButton from '@/components/button'
import Bus from '@/components/Bus'
import api from '@/api/eventCenterApi'
export default {
  name: 'SeeUserList',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 事件监控',
      loading: false,
      AlarmList1: [],
      AlarmList2: [],
      WarnList: [],
      NotificationList: [],
      NotificationPidCountList: [],
      activeName: 'alarm',
      timer: null,
      newMsg: null
    }
  },
  created () {
  },
  watch: {
    $route () {
      this.activeName = this.$route.params.type
    },
    activeName () {
      this.refresh()
    },
    newMsg () {
      if (this.newMsg.type === 0 && this.activeName === 'alarm') {
        console.log('new alarm')
        this.getAlarm()
      }
      if (this.newMsg.type === 1 && this.activeName === 'warnning') {
        this.getWarnning()
      }
      if (this.newMsg.type === 2 && this.activeName === 'notification') {
        this.getNotification()
      }
    }
  },
  activated () {
    this.activeName = this.$route.params.type
    this.refresh()
    // if (!this.timer) {
    //   this.startTimer()
    // }
    Bus.$on('monitor', data => (this.newMsg = data))
  },
  unactivated () {
    // this.StopTimer()
  },
  methods: {
    startTimer () {
      this.refresh()
      this.timer = setInterval(this.refresh, 10000)
    },

    StopTimer () {
      if (this.timer) {
        clearInterval(this.timer)
      }
    },
    transformDate (val) {
      return transformDate(val)
    },
    refresh () {
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
        case 'notificationpidcount':
          this.getNotificationPidcount()
          break
      }
    },
    getWarnning () {
      // this.loading = true
      api.getAllWarning().then(res => {
        // this.loading = false
        if (res.code === ApiRESULT.Success) {
          this.WarnList = res.data
        }
      }).catch(err => console.log(err))
    },
    getNotification () {
      // this.loading = true
      api.getAllNotification().then(res => {
        // this.loading = false
        if (res.code === ApiRESULT.Success) {
          this.NotificationList = res.data
        }
      }).catch(err => console.log(err))
    },
    getNotificationPidcount () {
      // this.loading = true
      api.getAllNotificationPidcount().then(res => {
        // this.loading = false
        if (res.code === ApiRESULT.Success) {
          this.NotificationPidCountList = res.data.rows
        }
      }).catch(err => console.log(err))
    },
    getAlarm () {
      // this.loading = true
      api.getAlarm().then(res => {
        // this.loading = false
        if (res.code === ApiRESULT.Success) {
          this.AlarmList1 = res.data.filter(item => item.eLevel < 2)
          this.AlarmList2 = res.data.filter(item => item.eLevel >= 2)
        }
      }).catch(err => console.log(err))
    },
    confirmNotification (id) {
      api.deleteNotification(id).then(res => {
        if (res.code === ApiRESULT.Success) {
        }
      }).catch(err => console.log(err))
    }
  }
}
</script>
<style lang="scss" scoped>
$con-height: $content-height - 56;
// 内容区
.content-wrap{
  overflow: hidden;
  height: percent($con-height, $content-height);
  text-align: center;
  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 40px !important;
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);

    .last-update-time{
      color: $color-white;
    }
  }
  .tab-height{
    height: percent($con-height, $con-height);
  }
  /deep/ .el-tabs__header{
    height: percent(50, $con-height)
  }
  /deep/ .el-tabs__content{
    height: percent($con-height - 50, $con-height)
  }
  .pane-height{
    height: 100%
  }
  .pane-notification{
    .content-header{
      height: percent(50, $con-height)
    }
  }
  .scroll{
    height: percent($con-height - 50, $con-height)
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
  .content{
    width: 25%;
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

</style>
<style>
.headtip{
  float:left;
  color:red;
}
</style>
