<template>
  <div class="wrapper">
    <isheader class="header"></isheader>
    <tabs class="tab"></tabs>
    <div class="troublescroll">
      <ul>
        <li class="itemli">
          <div class="itemlabel"><span>发生时间:</span></div>
          <div class="itemvalue">
            <el-date-picker
              v-model="happeningTime.text"
              type="datetime"
              value-format="yyyy-MM-dd HH:mm:ss"
              :clearable="false"
              class="datetime-width"
              placeholder="请选择发生时间"
            >
            </el-date-picker>
          </div>
        </li>
      </ul>
    </div>
    <BottomNavigation></BottomNavigation>
  </div>
</template>
<script>
import isheader from "./commom/header.vue";
import BottomNavigation from "./commom/bottom.vue";
import tabs from "./commom/tabs2.vue";
import axios from "axios";
export default {
  name: "trouble",
  components: {
    isheader,
    tabs,
    BottomNavigation
  },
  data() {
    return {
      happeningTime: { text: "", tips: "" }
    };
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
.troublescroll {
  position: absolute;
  top: 17%;
}
</style>
