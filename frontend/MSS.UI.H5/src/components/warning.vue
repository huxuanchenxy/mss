<template>
  <div class="wrapper">
    <isheader class="header"></isheader>
    <tabs class="tab"></tabs>
    <div class="warningscroll">
      <mu-list class="warningmylist">
          <li class="list" v-for="(item) in NotificationList" :key="item.key">
            <mu-list-item>
            <mu-icon value="feedback" class="muicon"></mu-icon>
            <span class="cssDeviceName">{{item.eqpName}}</span>
            <span class="cssCreateTime">2019-11-29 14:55:37</span>
            <span class="cssDeviceContent">设备中修时间超过60天</span>
            </mu-list-item>
            <mu-divider class="mudivider"></mu-divider>
        </li>
        </mu-list>
    </div>
    <BottomNavigation></BottomNavigation>
  </div>
</template>
<script>
import isheader from "./commom/header2.vue";
import BottomNavigation from "./commom/bottom.vue";
import tabs from "./commom/tabs3.vue";
import api from '@/api/eventCenterApi'
export default {
  name: "warning",
  components: {
    isheader,
    tabs,
    BottomNavigation,
  },
  mounted(){
      this.getNotification()
  },
  data() {
    return {
      msg: "Welcome to Your 上海18号线智能运维系统 App",
      NotificationList:[],
    };
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
  }
};
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
  top: 16%;
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
.cssDeviceName {
  position: absolute;
  top: 5%;
  left: 13%;
  font-size: 20px;
  color: #fff;
}
.cssDeviceContent {
  position: absolute;
  left: 13%;
  bottom: 16%;
  font-size: 12px;
  color: #c3bfbf;
}
.cssCreateTime {
  position: absolute;
  right: 5%;
  font-size: 12px;
  color: #c3bfbf;
}
.warningscroll{
    overflow: scroll;
}
</style>
