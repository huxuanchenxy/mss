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

      <el-tabs v-model="activeName">
        <el-tab-pane label="报警" name="alarm">
          <el-container style="height:100%;">
            <el-header height="100px" style="padding:0px">
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
                      <li class="list" v-for="(item) in AlarmList1" :key="item.key">
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
            </el-header>
            <el-main style="padding:0px">
              <!-- level 大于2 -->
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
                      <li class="list" v-for="(item) in AlarmList2" :key="item.key">
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
            </el-main>
          </el-container>
        </el-tab-pane>
        <el-tab-pane label="预警" name="warnning">
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
                      <div class="number">{{ item.content }}</div>
                      <div class="last-update-time">{{ transformDate(item.createdTime) }}</div>
                    </div>
                  </li>
                </ul>
              </el-scrollbar>
            </div>
          </div>
        </el-tab-pane>
        <el-tab-pane label="通知" name="notification">
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
              <li class="list last-update-time">
                发生时间
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
    getAlarm () {
      // this.loading = true
      api.getAlarm().then(res => {
        // this.loading = false
        if (res.code === ApiRESULT.Success) {
          this.AlarmList1 = res.data.filter(item => item.level < 2)
          this.AlarmList2 = res.data.filter(item => item.level >= 2)
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
</style>
