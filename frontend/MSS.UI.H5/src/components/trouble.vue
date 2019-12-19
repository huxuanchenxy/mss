<template>
  <div class="wrapper">
    <isheader class="header"></isheader>
    <tabs class="tab"></tabs>
    <div class="troublescroll">
      <ul>
        <li class="itemli">
          <div class="itemlabel">
            <span>发生时间:</span>
          </div>
          <div class="itemvalue">
            <div class="input" placeholder="请输入文字" @click="selectYear">{{year}}</div>
            <mt-datetime-picker
              v-model="dateValue"
              type="date"
              ref="datePicker"
              year-format="{value} 年"
              month-format="{value} 月"
              date-format="{value} 日"
              :endDate="new Date()"
              @confirm="handleConfirm"
            ></mt-datetime-picker>
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
      happeningTime: "",
      year: "",
      dateValue: "",
      isClicked: false
    };
  },
  methods: {
    openPicker() {
      this.$refs.refhappeningTime.open();
    },
    selectYear() {
      this.$refs.datePicker.open();
    },
    handleConfirm(value) {
      console.log(value);
      this.year = value.getFullYear();
      this.month = value.getMonth() + 1;
      this.date = value.getDate();
      this.isClicked = true;
    }
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
  top: 115px;
  width:100%;
}

.input {
  width: 200px;
  height: 24px;
  line-height: 12px;
  font-size: 14px;
  padding: 5px 8px;
  border: 1px solid #ddd;
  border-radius: 5px;
}
.input:empty::before {
  content: attr(placeholder);
}
.itemlabel{
    display:inline-block;
    width: 30%;
    position:absolute;
    height: 30px;
}
.itemlabel span{
    top: 10%;
    position: absolute;
    color:#fff;
}
.itemvalue{
    display:inline-block;
    width: 70%;
    position: absolute;
    left: 30%;
}
.troublescroll ul {
        position: absolute;
    margin: 0 auto;
    left: 0;
    top: 0;
    right: 0;
    bottom: 0;
    width: 90%;
}

</style>
