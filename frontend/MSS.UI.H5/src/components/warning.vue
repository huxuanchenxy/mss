<template>
  <div class="wrapper">
    <isheader class="header"></isheader>
    <tabs class="tab"></tabs>
    <div class="warningscroll">
      <mu-list class="warningmylist">
        <li class="list" v-show="addattr === 'notification'" v-for="(item) in NotificationList" :key="item.key">
          <mu-list-item>
          <mu-icon value="feedback" class="muicon"></mu-icon>
          <span class="cssDeviceName">{{item.eqpName}}</span>
          <span class="cssCreateTime">{{transformDate(item.createdTime)}}</span>
          <span class="cssDeviceContent">{{item.content}}</span>
          </mu-list-item>
          <mu-divider class="mudivider"></mu-divider>
        </li>
        <li class="list" v-show="addattr === 'prealarm'" v-for="(item) in WarnList" :key="item.key">
          <mu-list-item>
          <mu-icon value="feedback" class="muicon"></mu-icon>
          <span class="cssDeviceName">{{item.eqpName}}</span>
          <span class="cssCreateTime">{{transformDate(item.createdTime)}}</span>
          <span class="cssDeviceContent">{{item.content}}</span>
          </mu-list-item>
          <mu-divider class="mudivider"></mu-divider>
        </li>
        <li v-show="addattr === 'alarm'" class="list" v-for="(item) in AlarmList" :key="item.key">
          <mu-list-item>
          <mu-icon value="feedback" class="muicon"></mu-icon>
          <span class="cssDeviceName">{{item.eqpName}}</span>
          <span class="cssCreateTime">{{transformDate(item.originTime)}}</span>
          <span class="cssDeviceContent">{{item.des}}</span>
          </mu-list-item>
          <mu-divider class="mudivider"></mu-divider>
        </li>
      </mu-list>
    </div>
    <BottomNavigation></BottomNavigation>
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import isheader from "./commom/header2.vue"
import BottomNavigation from "./commom/bottom.vue"
import tabs from "./commom/tabs3.vue"
import Bus from './commom/Bus'
import api from '@/api/eventCenterApi'
export default {
  name: "warning",
  components: {
    isheader,
    tabs,
    BottomNavigation,
  },
  data() {
    return {
      msg: "Welcome to Your 上海18号线智能运维系统 App",
      addattr: 'notification',
      NotificationList: [],
      AlarmList: [],
      WarnList: []
    }
  },
  mounted(){
    Bus.$on('addattr',(val)=>{
      console.log('warning:' + val)
      this.addattr=val
      switch (val) {
        case 'notification':
          this.getNotification()
          break
        case 'prealarm':
          this.getWarnning()
          break
      }
    })
    this.getNotification()
  },
  methods:{
    getNotification () {
      // this.loading = true
      api.getAllNotification().then(res => {
        if (res.code === 0) {
          this.NotificationList = res.data
        }
      }).catch(err => console.log(err))
    },
    getWarnning () {
      // this.loading = true
      api.getAllWarning().then(res => {
        // this.loading = false
        if (res.code === 0) {
          this.WarnList = res.data
          // this.NotificationList = res.data
        }
      }).catch(err => console.log(err))
    },
    getAlarm () {
      // this.loading = true
      api.getAlarm().then(res => {
        // this.loading = false
        if (res.code === ApiRESULT.Success) {
          // this.AlarmList1 = res.data.filter(item => item.eLevel < 2)
          // this.AlarmList2 = res.data.filter(item => item.eLevel >= 2)
          this.AlarmList = res.data
        }
      }).catch(err => console.log(err))
    },
    transformDate (val) {
      return transformDate(val)
    }
  }
}
</script>
<style>
.wrapper {
  /* display: flex;
        flex-direction: column;
        height: 100vh; */
  position: fixed;
  width: 100%;
  height: 100%;
}
.tab {
  flex: 1;
  margin: 9rem 0 4rem 0;
}
.header {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1;
}
.warningmylist {
  top: 115px;
padding: 8px 0;
    width: 100%;
    height: 74%;
    position: relative;
    overflow-x: scroll;
    overflow-y: scroll;
    overflow: scroll;
    -webkit-overflow-scrolling: touch;
  position: absolute !important;
}
.warningmylist .mu-divider {
  background-color: #fff;
}
.warningmylist .cssDeviceName {
position: absolute;
    top: 5%;
    left: 13%;
    font-size: 15px;
    color: #fff;
}
.warningmylist .cssDeviceContent {
position: absolute;
    left: 13%;
    bottom: 7%;
    font-size: 12px;
    color: #c3bfbf;
}
.warningmylist .cssCreateTime {
position: absolute;
    right: 5%;
    font-size: 12px;
    color: #c3bfbf;
    top: 10%;
}
.warningscroll{
    overflow: scroll;
}
</style>
