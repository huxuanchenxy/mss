<template>
  <div class="wrapper">
    <isheader class="header"></isheader>
    <tabs class="tab"></tabs>
    <el-collapse class="filtercollapse" v-model="activeName" accordion>
      <el-collapse-item title="筛选" name="1">
        <div class="troublefilter1">
          <!-- <el-input v-model.trim="desc" placeholder="输入关键字筛选"></el-input> -->
          <el-select
            v-model="troubleStatus"
            clearable
            filterable
            placeholder="按故障状态筛选"
          >
            <el-option
              v-for="item in troubleStatusList"
              :key="item.key"
              :label="item.name"
              :value="item.id"
            ></el-option>
          </el-select>
          <el-date-picker
            v-model="searchDate"
            type="daterange"
            prefix-icon="el-icon-date"
            :unlink-panels="true"
            value-format="yyyy-MM-dd"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
          >
          </el-date-picker>
          <el-button type="primary">筛选</el-button>
        </div>
      </el-collapse-item>
    </el-collapse>
    <div class="troublelistscroll">
      <mu-list class="troublemylist">
        <li class="liitem" v-for="item in troubleList" :key="item.key">
          <mu-list-item>
            <mu-icon value="label" class="muicon"></mu-icon>
            <span class="itemcode">{{ item.code }}</span>
            <span class="itemlineName">{{ item.lineName }}</span>
            <span class="itemstartLocationName">
              {{ item.startLocationName }}
            </span>
            <span class="itemdesc">{{ item.desc }}</span>
            <span class="itemreportedCompanyName">
              {{ item.reportedCompanyName }}
            </span>
            <span class="itemreportedByName">{{ item.reportedByName }}</span>
            <span class="itemstatusName">({{ item.statusName }})</span>
            <span class="itemlastOperationName"
              >最新操作: {{ item.lastOperationName }}</span
            >
            <span class="itemhappeningTime">{{ item.happeningTime }}</span>
          </mu-list-item>
          <mu-divider class="mudivider"></mu-divider>
        </li>
      </mu-list>
    </div>
    <BottomNavigation></BottomNavigation>
  </div>
</template>
<script>
import isheader from "./commom/header.vue";
import BottomNavigation from "./commom/bottom.vue";
import tabs from "./commom/tabs2.vue";
import axios from "axios";
import api from "@/api/DeviceMaintainRegApi.js";
import apiAuth from "@/api/authApi";
import { transformDate } from "@/common/js/utils.js";
import {
  troubleOperation,
  dictionary,
  troubleStatus
} from "@/common/js/dictionary.js";
export default {
  name: "trouble",
  components: {
    isheader,
    tabs,
    BottomNavigation
  },
  mounted() {
    this.InitSelect();
    this.searchResult(1);
  },
  data() {
    return {
      troubleList: [],
      troubleListByID: {},
      year: "",
      dateValue: "",
      isClicked: false,
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: "code",
        order: "asc"
      },
      searchDate: [],
      troubleStatus: "",
      troubleStatusList: [],
      activeName: '1',
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
    },
    InitSelect() {
      // 故障状态列表
      apiAuth
        .getSubCode(dictionary.troubleStatus)
        .then(res => {
          this.troubleStatusList = res.data;
        })
        .catch(err => console.log(err));
    },
    searchResult(page) {
      this.currentPage = page;
      let st, et;
      if (this.searchDate != null && this.searchDate.length !== 0) {
        st = this.searchDate[0] + " 00:00:00";
        et = this.searchDate[1] + " 23:59:59";
      }
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        TroubleReportDesc: this.desc,
        TroubleStatus: this.troubleStatus,
        StartTime: st,
        EndTime: et
      };
      api
        .getTroubleReportPage(parm)
        .then(res => {
          res.data.rows.map(item => {
            this.troubleListByID[item.id] = item;
            item.happeningTime = transformDate(item.happeningTime);
          });
          this.troubleList = res.data.rows;
          this.total = res.data.total;
        })
        .catch(err => console.log(err));
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
.troublemylist {
  /* top: 115px; */
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
.troublemylist .mu-divider {
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
.troublelistscroll {
  overflow: scroll;
  position: absolute;
  height: 100%;
  width: 100%;
  top: 33%;
}
.itemcode {
  position: absolute;
  color: aqua;
  left: 17%;
  top: 18%;
}
.itemlineName {
  position: absolute;
  left: 17%;
  top: 57%;
  font-size: 16px;
  color: #fff;
}
.itemstartLocationName {
  position: absolute;
  left: 38%;
  top: 57%;
  font-size: 16px;
  color: #fff;
}
.itemdesc {
  position: absolute;
  left: 17%;
  /* bottom: 3%; */
  top: 109%;
  font-size: 12px;
  color: #f56c6c;
  overflow: auto;
}
.itemreportedCompanyName {
  position: absolute;
  right: 20%;
  bottom: 5%;
  font-size: 12px;
  color: #f5f7fa;
  overflow: auto;
}
.itemreportedByName {
  position: absolute;
  right: 10%;
  bottom: 5%;
  font-size: 12px;
  color: #f5f7fa;
  overflow: auto;
}
.itemstatusName {
  position: absolute;
  left: 3%;
  top: 85%;
  font-size: 12px;
  color: #f5f7fa;
  overflow: auto;
}
.itemlastOperationName {
  position: absolute;
  top: 107%;
  right: 10%;
  font-size: 12px;
  color: #909399;
  overflow: auto;
}
.itemhappeningTime {
  position: absolute;
  top: 20%;
  right: 10%;
  font-size: 12px;
  color: #f5f7fa;
  overflow: auto;
}
.liitem .mu-item-wrapper {
  height: 80px;
}
.liitem .mu-icon {
  position: absolute;
  top: 28%;
}
.troublefilter {
  position: absolute;
  top: 118px;
}
.filtercollapse{
  position: absolute;
    top: 115px;
        width: 100%;
}
.filtercollapse .el-collapse-item__wrap{
    z-index: 1000;
    background-color: #474a4f !important;
    position: fixed;
    left: 0;
    top: 180px;
    bottom: -8%;
    width: 100%;
    height: 32%;
    background:none !important;
}
.filtercollapse .el-collapse{
    background:none !important;
}
</style>
