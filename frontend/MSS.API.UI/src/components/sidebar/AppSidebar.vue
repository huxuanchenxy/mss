<template>
  <div class="sidebar">
    <!-- 左边导航 -->
    <div class="left-nav-wrap" @mouseleave="hideSubNav">
    <!-- <div class="left-nav-wrap"> -->
      <!-- <div class="sidebar-title" data-direction="bottom">菜单管理</div> -->
      <div class="nav-wrap" :style="`height: ${navHeight}px`">
        <!-- <el-scrollbar :style="`height: ${navHeight}px`"> -->
          <ul id="nav-move-wrap" class="nav-move-wrap" ref="navMoveWrap">
            <router-link v-for="(item, index) in navList" :key="item.key" :to="{ path: item.path }" class="list" :ref="'navs'+index" tag="li" @mouseenter.native="showSubNav(index)">
              <a class="nav-link">
                <img :src="item.iconCls" width="18" height="18" class="icon icon-info vertical-middle" alt="">
                <img :src="item.iconClsActive" width="18" height="18" class="hide icon icon-active vertical-middle" alt="">
                <span class="text vertical-middle">{{ item.name }}</span>
              </a>
            </router-link>
          </ul>
        <!-- </el-scrollbar> -->
      </div>
      <!-- 二级菜单 -->
      <!-- <div class="sub-nav-show active" :class="{ active: bShowSubNav }" :style="`height: ${navHeight - 25}px`">
        <div class="sub-nav-wrap height-full">
          <el-scrollbar>
            <ul class="sub-nav active" v-for="item in navList" :key="item.key">
              <li class="sub-nav-list" v-for="child in item.children" :key="child.key">
                <router-link class="block" :to="child.path">{{ child.name }}</router-link>
              </li>
            </ul>
          </el-scrollbar>
        </div>
      </div> -->
      <div class="sub-nav-show" :class="{ active: bShowSubNav }" :style="`left: ${navChdleft}px`">
        <div class="sub-nav-wrap height-full">
          <el-scrollbar>
            <ul class="sub-nav" v-if="item.isShowSubNav" v-for="item in navList" :key="item.key" :class="{ active: item.isShowSubNav }">
              <li class="sub-nav-list" v-for="child in item.children" :key="child.key">
                <router-link class="block" :to="child.path">{{ child.name }}</router-link>
              </li>
            </ul>
          </el-scrollbar>
        </div>
      </div>
    </div>
    <!-- 右边警告提示 -->
    <div class="right-wrap" :class="{ active: !bMsgShrink }">
      <div class="right-arrow-wrap" :class="{ on: !bMsgShrink }" @click="msgShrink">
        <i class="iconfont icon-arrow-right"></i>
      </div>
      <div v-if="bMsgShrink" class="right-msg-wrap">
        <div class="sidebar-title"><i class="iconfont icon-warning"
          style="color:#f91333;"></i> 事件中心</div>
        <ul class="msg-wrap">
          <li class="list" v-for="item in EventTypeList" :key="item.key">
            <!-- <p>{{ item.region }}</p> -->
            <a @click="gotoMonitor(item.type)" class="list-link">
              <!-- <i class="dot" :class="item.state"></i> -->
              <el-tooltip :content="item.tooptip_content" :disabled="item.tooptip_disable" placement="top">
                <span v-if="item.type === 'alarm'">
                  {{ item.title }}(
                  <font style="color:red;">{{item.type === 'alarm' ? item.count1 : ''}}</font>
                  <font>{{item.type === 'alarm' ? '| ' + item.count2 : item.count}}</font> )
                </span>
                <span v-else>
                  {{ item.title }} ( {{item.count}} )
                </span>
              </el-tooltip>
            </a>
          </li>
          <li class="list not-bg">
            <a @click="gotoAlarmHistory">查看历史<i class="iconfont icon-arrow-right color-blue"></i></a>
          </li>
        </ul>
      </div>
      <div v-else class="msg-shrink">
        <p>
          <i class="iconfont icon-warning" :style="{ color: activeColor }">
          </i> 事件中心
        </p>
      </div>
    </div>
  </div>
</template>
<script>
// import { transformDate } from '@/common/js/utils.js'
import { ApiRESULT } from '@/common/js/utils.js'
import Bus from '@/components/Bus'
import api from '@/api/authApi'
import apiEvent from '@/api/eventCenterApi'
import * as signalR from '@aspnet/signalr'
export default {
  name: 'sidebar',
  data () {
    return {
      navHeight: '67px',
      navMoveNum: 0,
      bMsgShrink: false,
      bShowSubNav: false,
      navList: [
        {
          GroupName: '系统管理',
          isShowSubNav: false,
          url: '/system'
        }
      ],
      EventTypeList: [{
        type: 'alarm',
        title: '报警',
        state: '',
        count1: 0, // 严重警报
        count2: 0,
        tooptip_content: '',
        tooptip_disable: true
      }, {
        type: 'warnning',
        title: '预警',
        state: '',
        count: 0,
        tooptip_content: '',
        tooptip_disable: true
      }, {
        type: 'notification',
        title: '通知',
        state: '',
        count: 0,
        tooptip_content: '',
        tooptip_disable: true
      }
      ],
      activeColor: '#f91333',
      timer: null,
      Conn: null,
      navChdleft: -600
    }
  },
  created () {
    this.getMenu()
    this.getConfig()
    this.getAlarm()
    this.getWarnning()
    this.getNotification()
  },
  methods: {
    refresh (type) {
      switch (type) {
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
      apiEvent.getAllWarning().then(res => {
        if (res.code === ApiRESULT.Success) {
          let count = res.data.length
          this.EventTypeList[1].count = count
          if ((count + '').length > 6) {
            this.EventTypeList[1].tooptip_disable = false
            this.EventTypeList[1].tooptip_content = `有${count}预警事件`
          }
        }
      }).catch(err => console.log(err))
    },
    getNotification () {
      apiEvent.getAllNotification().then(res => {
        if (res.code === ApiRESULT.Success) {
          let count = res.data.length
          this.EventTypeList[2].count = count
          if ((count + '').length > 6) {
            this.EventTypeList[2].tooptip_disable = false
            this.EventTypeList[2].tooptip_content = `有${count}通知事件`
          }
        }
      }).catch(err => console.log(err))
    },
    getAlarm () {
      apiEvent.getAlarm().then(res => {
        if (res.code === ApiRESULT.Success) {
          let count1 = res.data.filter(item => item.eLevel < 2).length
          let count2 = res.data.length - count1
          this.EventTypeList[0].count1 = count1
          this.EventTypeList[0].count2 = count2
          if ((count1 + '').length + (count2 + '').length > 5) {
            this.EventTypeList[0].tooptip_disable = false
            this.EventTypeList[0].tooptip_content = `有${count1}严重警报，${count2}非严重警报`
          }
        }
      }).catch(err => console.log(err))
    },
    gotoMonitor (type) {
      this.$router.push({
        name: 'alarm',
        params: {
          type: type
        }
      })
    },
    gotoAlarmHistory () {
      this.$router.push({
        name: 'alarmHistory',
        params: {
          type: 'alarm'
        }
      })
    },

    // test () {
    //   if (this.Conn) {
    //     this.Conn.invoke('SendMessage', 'message').catch(function (err) {
    //       return console.error(err.toString())
    //     })
    //   }
    // },

    ChangeColor () {
      if (this.activeColor === '#f91333') {
        this.activeColor = '#fff'
      } else {
        this.activeColor = '#f91333'
      }
    },

    startTimer () {
      if (!this.timer) {
        this.timer = setInterval(this.ChangeColor, 1000)
      }
    },

    StopTimer () {
      if (this.timer) {
        clearInterval(this.timer)
        this.timer = null
        this.activeColor = '#f91333'
      }
    },
    getConfig () {
      apiEvent.getConfig().then(res => {
        if (res.code === ApiRESULT.Success) {
          this.initEvents(res.data.hub)
        }
      }).catch(err => console.log(err))
    },
    initEvents (hub) {
      let thisObj = this
      let token = window.sessionStorage.getItem('token')
      if (this.Conn) {
        this.Conn.close()
      }
      // let ip = 'http://localhost:3851/eventHub'
      let ip = hub
      var connection = new signalR.HubConnectionBuilder().withUrl(ip,
        { accessTokenFactory: () => token }).build()

      connection.on('RecieveMsg', function (message) {
        console.log(message)
        if (!thisObj.bMsgShrink && message.msg !== 'off') {
          thisObj.startTimer()
        }

        if (message.type === 0) {
          Bus.$emit('eqp_monitor', message)
          thisObj.refresh('alarm')
        } else if (message.type === 1) {
          thisObj.refresh('warnning')
        } else {
          thisObj.refresh('notification')
        }
        Bus.$emit('monitor', message)
      })
      connection.start().then(res => {
        this.Conn = connection
      }).catch(function (err) {
        return console.error(err.toString())
      })
    },

    // 获取菜单
    getMenu () {
      api.getMenu().then(res => {
        let _data = {}
        for (let [, value] of Object.entries(res)) {
          value.isShowSubNav = false
          _data[value.path] = value
        }
        this.navList = res
        this.$router.navList = _data
        Bus.$emit('navList', _data)
      }).catch(err => console.log(err))
    },

    // 显示二级菜单
    showSubNav (index) {
      // console.log(this.$refs['navs'+index][0].$el.offsetLeft)
      this.navChdleft = this.$refs['navs' + index][0].$el.offsetLeft
      // this.$refs.nav[0].$el.offsetHeight * 4

      for (let [, value] of Object.entries(this.navList)) {
        value.isShowSubNav = false
      }
      // 显示二级菜单容器，同时需要满足是否有子级
      this.bShowSubNav = true && this.navList[index].children
      this.navList[index].isShowSubNav = true
    },

    // 隐藏二级菜单
    hideSubNav () {
      this.bShowSubNav = false
      this.navChdleft = -600
      for (let [, value] of Object.entries(this.navList)) {
        value.isShowSubNav = false
      }
    },

    // 右边警告收缩
    msgShrink () {
      let oWrap = document.querySelector('#main')
      Array.from(document.querySelectorAll('#main .el-scrollbar__wrap')).forEach(item => {
        item.style.height = `${Math.random() + 100}%`
      })
      if (this.bMsgShrink) {
        oWrap.className = 'shrink'
      } else {
        oWrap.className = oWrap.className.replace('shrink', '')
      }
      this.bMsgShrink = !this.bMsgShrink
      if (this.bMsgShrink && this.timer) {
        this.StopTimer()
      }
    }
  },
  mounted () {
    // test
    // this.navHeight = this.$refs.nav[0].$el.offsetHeight * 4
  },
  destroyed () {
    if (this.Conn) {
      this.Conn.stop()
    }
  },
  watch: {
    $route () {
      if (!this.bMsgShrink) document.querySelector('#main').className = 'shrink'
      this.bMsgShrink = this.bMsgShrink
    }
  }
}
</script>
<style lang="scss" scoped>
$width: 180;

.sidebar {
  position: relative;
}
.left-nav-wrap,
.right-wrap{
  position: fixed;
  z-index: 99;
  top: 50%;
  width: 180px;
  transform: translateY(-43%);
  font-size: PXtoEm(18);

  .sidebar-title{
    height: PXtoEm(70);
    font-size: 16px;
    font-weight: bold;
    line-height: PXtoEm(70);
    text-align: center;
  }

  .list{
    display: flex;
    justify-content: center;
    align-items: center;
    flex-flow: column;
    position: relative;
    height: PXtoEm(85);
    border-top: 1px solid transparent;

    &:before{
      content: "";
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 1px;
      background: url(./images/list-top-line.png) no-repeat top right;
    }

    &:hover{
      border-top-color: #1b7ec9;
    }
  }

  .not-bg{
    background: none !important;
  }
}

// test
.left-nav-wrap {
  position: inherit;
  width: 100%;
  top: auto;
  transform: translateY(0%);
  background: #fff;
  height: 50px;
  .nav-wrap {
height: 50px;
    .nav-move-wrap {
      width: 100%;
      height: 50px;
      display: flex;
      justify-content: flex-start;
      align-items: center;
      box-sizing: border-box;
      padding-left: 4.86111%;
      li{
        // background: #fff;
        margin: 0 5px;
      }
    }
  }
}

// 左边菜单
.left-nav-wrap{
  left: 0;
  background: url(./images/left.png) no-repeat 0 0/100% 100%;

  // 第一行
  .sidebar-title{
    text-align: left;
    text-indent: PXtoEm(30);
  }

  // 菜单容器
  .nav-wrap{
    overflow: hidden;

    .nav-move-wrap{
      transition: .7s;
    }

    /deep/ .el-scrollbar__bar.is-vertical{
      height: 92%;
    }
  }

  // 一级菜单列表
  .list{
    // 上边的边框线
    &:before{
      background: url(./images/line_left.png) no-repeat top left;
    }

    .nav-link{
      display: flex;
      align-items: center;
      width: 100%;
      height: 100%;

      .icon{
        margin: 0 5px 0 30px;
      }

      // 箭头
      &:after{
        content: "\e605";
        @include opacity(0);
        font-family: "iconfont";
      }
    }

    &:hover,
    &.router-link-active{
      border-top-color: #1b7ec9;
      .nav-link{
        color: $color-highlight-text;

        &:after{
          @include opacity(1);
        }
      }

      .icon-info{
        display: none;
      }

      .icon-active{
        display: initial;
      }
    }
  }

  // 二级菜单
  .sub-nav-show{
    position: absolute;
    left: 0;
    // top: PXtoEm(70);
    top: 43px;
    z-index: -1;
    overflow: hidden;
    @include opacity(0);
    width: 160px;
    // width: 100%;
    height: 280px;
    // transition: .7s;

    &.active{
      left: 0;
      @include opacity(1);
    }
  }
  .sub-nav-wrap{
    background: #0f0e19;
    .sub-nav{
      width: 100%;
      @include opacity(0);
      // transition: .7s;
      // display: flex; justify-content: flex-start; align-items: center;
      // background: #1b7ec9;
      &.active{
        @include opacity(1);
      }

      .sub-nav-list{
        display: flex;
        align-items: center;
        position: relative;
        height: PXtoEm(70);
        padding: 0 10px;

        &:after{
          content: "";
          position: absolute;
          top: 0;
          left: 0;
          z-index: -1;
          width: 100%;
          height: 100%;
        }

        &:hover{
          &:after{
            @include opacity(.7);
            background: #1b7ec9;
          }
        }
      }
    }
  }
}

// 右边消息
.right-wrap{
  right: 0;
  // background: url(./images/right-msg-bg.png) no-repeat 0 0/100% 100%;
  background: url(./images/right.png) no-repeat 0 0/100% 100%;
  transition: .4s;

  &.active{
    transform: translateX(65%) translateY(-43%);
  }

  // 箭头容器
  .right-arrow-wrap{
    position: absolute;
    top: 50%;
    left: 0;
    z-index: 9;
    width: 16px;
    height: PXtoEm(85);
    margin-top: PXtoEm(-49);
    background: url(./images/right-arrow-bg.png) no-repeat 0 0/100% 100%;
    line-height: PXtoEm(85);
    cursor: pointer;

    .icon-arrow-right{
      transition: .7s;
    }

    &.on{
      .icon-arrow-right{
        display: inline-block;
        transform: rotateZ(-180deg);
      }
    }
  }

  // 收起之后的状态
  .msg-shrink{
    display: flex;;
    align-items: center;
    width: $font-size-base;
    height: PXtoEm(414);
    margin-left: 30px;
  }

  // 警告显示图标
  .icon-warning{
    color: $color-warning;
  }

  .list{
    &:before{
      // background: url(./images/list-top-line.png) no-repeat top right;
      background: url(./images/line_right.png) no-repeat top right;
    }

    p{
      color: $color-sub-text;
    }

    .list-link{
      position: relative;
      cursor: pointer;
      span{
        display: inline-block;
        width: 120px;
        @extend %ellipsis;
      }
    }

    // 状态显示小圆点
    .dot{
      position: absolute;
      top: 4.5px;
      left: -15px;
      width: 9px;
      height: 9px;
      border-radius: 50%;

      &.warning{
        background: $color-dot-warning;

        &:after{
          content: "";
          position: absolute;
          top: 0;
          left: 0;
          width: 9px;
          height: 9px;
          border-radius: 50%;
          box-shadow: 0 0 20px 3px $color-dot-warning;
          // background: url(./images/dot-error-1.png) no-repeat;
          animation: twinkle .8s linear infinite alternate;
        }
        // background: url(./images/dot-error.png) no-repeat 0 0/100% 100%;
      }

      &.success{
        background: $color-dot-success;
        // background: url(./images/dot-success.png) no-repeat 0 0/100% 100%;
      }
    }
  }
}
@keyframes twinkle {
  to{
    @include opacity(0);
  }
}
</style>
