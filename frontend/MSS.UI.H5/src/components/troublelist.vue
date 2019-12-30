<template>
  <div class="wrapper">
    <isheader class="header"></isheader>
    <tabs class="tab"></tabs>
    <div v-if="addattr === 'myCheckList'">
    <div>
      <span class="spanfilter">筛选</span>
      <el-collapse class="filtercollapse" v-model="activeName" accordion>
        <el-collapse-item title="" name="1">
          <div class="troublefilter">
            <li>
              <div class="troublefilterleft">
                <span>检修单名称:</span>
              </div>
              <div class="troublefilterright">
                <el-input class="troublerightele" v-model.trim="entityTitle" placeholder="请输入检修单名称"></el-input>
              </div>
            </li>
            <li>
              <div class="troublefilterleft">
                <span>检修单状态:</span>
              </div>
              <div class="troublefilterright">
                <el-select class="troublerightele" v-model="entityStatus" clearable filterable placeholder="按检修单状态筛选">
                  <el-option
                    v-for="item in entityStatusList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.id"
                  ></el-option>
                </el-select>
              </div>
            </li>
            <li>
              <div class="troublefilterleft">
                <span>最后保存/提交日期:</span>
              </div>
              <div class="troublefilterright">
                <span class="dateinput" placeholder="开始日期" @click="selectDate('sd')">{{sdCheck}}</span>
                <span class="dateinput2" placeholder="结束日期" @click="selectDate('ed')">{{edCheck}}</span>
              <mt-datetime-picker
                v-model="dateValue"
                type="date"
                ref="dateCheckPicker"
                year-format="{value} 年"
                month-format="{value} 月"
                date-format="{value} 日"
                :endDate="new Date()"
                @confirm="handleCheckConfirm"
              ></mt-datetime-picker>
                          <mt-datetime-picker
                v-model="dateValue"
                type="date"
                ref="dateCheckPicker2"
                year-format="{value} 年"
                month-format="{value} 月"
                date-format="{value} 日"
                :endDate="new Date()"
                @confirm="handleCheckConfirm2"
              ></mt-datetime-picker>
              </div>
            </li>
            <li>
              <div class="troublefilterright">
                <el-button type="primary" @click="filterCheckClick">确定</el-button>
              </div>
            </li>
          </div>
        </el-collapse-item>
      </el-collapse>
    </div>
    <div class="troublelistscroll">
      <mu-list class="troublemylist">
        <li class="liitem" v-for="item in mList" :key="item.key">
          <mu-list-item @click="inputCheck(item.id)">
            <mu-icon value="label" class="muicon"></mu-icon>
            <span class="itemcode">{{ item.title }}</span>
            <span class="itemlineName">{{ item.teamName }}</span>
            <span class="itemstartLocationName">{{ item.locationName }}</span>
            <span class="itemreportedByName">{{ item.updatedByName }}</span>
            <span class="itemstatusName">({{ item.statusName }})</span>
            <span class="itemhappeningTime">{{ item.updatedTime }}</span>
          </mu-list-item>
          <mu-divider class="mudivider"></mu-divider>
        </li>
      </mu-list>
    </div>
    </div>
    <div v-else>
    <div>
      <span class="spanfilter">筛选</span>
      <el-collapse class="filtercollapse" v-model="activeName" accordion>
        <el-collapse-item title="" name="1">
          <div class="troublefilter">
            <li>
              <div class="troublefilterleft">
                <span>关键字:</span>
              </div>
              <div class="troublefilterright">
                <el-input class="troublerightele" v-model.trim="desc" placeholder="这里输入关键字"></el-input>
              </div>
            </li>
            <li>
              <div class="troublefilterleft">
                <span>故障状态:</span>
              </div>
              <div class="troublefilterright">
                <el-select class="troublerightele" v-model="troubleStatus" clearable filterable placeholder="按故障状态筛选">
                  <el-option
                    v-for="item in troubleStatusList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.id"
                  ></el-option>
                </el-select>
              </div>
            </li>
            <li>
              <div class="troublefilterleft">
                <span>发生日期:</span>
              </div>
              <div class="troublefilterright">
                <span class="dateinput" placeholder="开始日期" @click="selectYear" v-model="startDate">{{date1}}</span>
                <span class="dateinput2" placeholder="结束日期" @click="selectYear2" v-model="endDate">{{date2}}</span>
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
                          <mt-datetime-picker
                v-model="dateValue"
                type="date"
                ref="datePicker2"
                year-format="{value} 年"
                month-format="{value} 月"
                date-format="{value} 日"
                :endDate="new Date()"
                @confirm="handleConfirm2"
              ></mt-datetime-picker>
              </div>
            </li>
            <li>
              <div class="troublefilterright">
                <el-button type="primary" @click="filterclick()">确定</el-button>
              </div>
            </li>
          </div>
        </el-collapse-item>
      </el-collapse>
    </div>
    <div class="troublelistscroll">
      <mu-list class="troublemylist">
        <li class="liitem" v-for="item in troubleList" :key="item.key">
          <mu-list-item>
            <mu-icon value="label" class="muicon"></mu-icon>
            <span class="itemcode">{{ item.code }}</span>
            <span class="itemlineName">{{ item.lineName }}</span>
            <span class="itemstartLocationName">{{ item.startLocationName }}</span>
            <span class="itemdesc">{{ item.desc }}</span>
            <span class="itemreportedCompanyName">{{ item.reportedCompanyName }}</span>
            <span class="itemreportedByName">{{ item.reportedByName }}</span>
            <span class="itemstatusName">({{ item.statusName }})</span>
            <span class="itemlastOperationName">最新操作: {{ item.lastOperationName }}</span>
            <span class="itemhappeningTime">{{ item.happeningTime }}</span>
          </mu-list-item>
          <mu-divider class="mudivider"></mu-divider>
        </li>
      </mu-list>
    </div>
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
import apiWork from '@/api/workflowApi'
import apiAuth from "@/api/authApi";
import { transformDate,dateFtt } from "@/common/js/utils.js";
import Bus from './commom/Bus'
import {
  troubleOperation,
  dictionary,
  troubleStatus
} from "@/common/js/dictionary.js";
export default {
  name: "troublelist",
  components: {
    isheader,
    tabs,
    BottomNavigation
  },
  data() {
    return {
      addattr: '',
      editEntity: [],
      entityTitle: '',
      entityStatus: '',
      entityStatusList: [],
      currentCheckPage: 1,
      totalCheck: 0,
      sdCheck: '',
      edCheck: '',
      mList: '',

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
      activeName: "0",
      desc: "",
      date1:'',
      date2:'',
      startDate:'',
      endDate:'',
    };
  },
  mounted () {
    this.InitSelect();
    this.searchResult(1);
    if (this.$route.params.attr !== undefined) {
      this.addattr = this.$route.params.attr
      Bus.$emit('addattr', this.addattr)
      this.searchCheckResult(1)
    }
    Bus.$on('addattr',(val)=>{
      this.addattr=val
      if (val === 'myCheckList') {
        this.searchCheckResult(1)
      }
    })
  },
  methods: {
    inputCheck (id) {
      let clientHeight = `${document.documentElement.clientHeight}`
      debugger
      if (clientHeight < 1024) {
        this.$message({
          showClose: true,
          message: '只有pad端才有填写功能',
          type: 'warning'
        })
        return
      }
      this.$router.push({
        name: 'inputCheck',
        params: {
          id: id,
          attr: this.addattr
        }
      })
    },
    filterCheckClick(){
      this.searchCheckResult(1)
      this.activeName = '0'
    },
    selectDate (type) {
      if (type === 'sd') {
        this.$refs.dateCheckPicker.open()
      } else {
        this.$refs.dateCheckPicker2.open()
      }
    },
    handleCheckConfirm (value) {
      this.sdCheck = dateFtt(value,'yyyy-MM-dd')
    },
    handleCheckConfirm2 (value) {
      this.edCheck = dateFtt(value,'yyyy-MM-dd')
    },
    searchCheckResult(page) {
      this.currentCheckPage = page;
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        title: this.entityTitle,
        status: this.entityStatus,
        start: this.sdCheck
      }
      if (this.edCheck !== '') {
        parm.end = this.edCheck + ' 23:59:59'
      }
      apiWork.getEntityList(parm).then(res => {
        this.loading = false
        if (res.data.total > 0) {
          res.data.rows.map(item => {
            item.updatedTime = transformDate(item.updatedTime)
          })
          this.mList = res.data.rows
          this.totalCheck = res.data.total
        } else {
          this.mList = []
          this.totalCheck = 0
        }
      }).catch(err => console.log(err))
    },
    filterclick(){
      this.searchResult(1)
      this.activeName = '0'
    },
    openPicker() {
      this.$refs.refhappeningTime.open();
    },
    selectYear() {
      this.$refs.datePicker.open();
    },
    selectYear2() {
      this.$refs.datePicker2.open();
    },
    handleConfirm(value) {
      // console.log('date1:' + value);
      // this.year = value.getFullYear();
      // this.month = value.getMonth() + 1;
      // this.date1 = value.getDate();
      this.date1 = dateFtt(value,'yyyy-MM-dd')
      this.startDate = this.date1
      this.isClicked = true;
    },
    handleConfirm2(value) {
      // console.log('date2:' + value);
      this.date2 = dateFtt(value,'yyyy-MM-dd')
      this.endDate = this.date2
    },
    InitSelect() {
      // 检修单状态加载
      apiAuth.getSubCode(dictionary.pmStatus).then(res => {
        this.entityStatusList = res.data
      }).catch(err => console.log(err))
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
      console.log('this.date1:' + this.date1)
      if (this.startDate !== '' && this.endDate !== '') {
        st = this.startDate + " 00:00:00";
        et = this.endDate + " 23:59:59";
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
  position: fixed !important;
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
  overflow-y: hidden;
  position: absolute;
  height: 100%;
  width: 100%;
  top: 145px;
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
  top: 10px;
  width: 94%;
  height:90%;
}
.troublefilter li {
  padding: 10px;
}
.troublefilter .troublefilterleft {
  display: inline-block;
}
.troublefilter .troublefilterright {
  display: inline-block;
}
.filtercollapse {
  position: absolute;
  top: 115px;
  width: 100%;
}
.filtercollapse .el-collapse-item__wrap {
  z-index: 300;
  background-color: #474a4f !important;
  position: fixed;
  left: 0;
  top: 156px;
  bottom: 0;
  width: 100%;
      height: 35%;
  padding-left: 0.5em !important;
  padding-right: 0.5em !important;
  /* background:none !important; */
}
.filtercollapse .el-collapse {
  background: none !important;
}
.filtercollapse .el-collapse-item__header {
  height: 40px;
  line-height: 40px;
}
.filtercollapse .el-icon-arrow-right {
  line-height: 40px;
}
.troublefilterright {
  right: 0;
  position: absolute;
      width: 57%;
}

.dateinput {
  width: 200px;
  height: 24px;
  line-height: 12px;
  font-size: 14px;
  padding: 5px 8px;
  border: 1px solid #ddd;
  border-radius: 5px;
  color:#8e9092;
}
.dateinput:empty::before {
  content: attr(placeholder);
}

.dateinput2 {
  width: 200px;
  height: 24px;
  line-height: 12px;
  font-size: 14px;
  padding: 5px 8px;
  border: 1px solid #ddd;
  border-radius: 5px;
  color:#8e9092;
}
.dateinput2:empty::before {
  content: attr(placeholder);
}
.troublefilterright .v-modal{
  z-index: 3000;
  position: fixed !important;
    left: 0!important;
    top: 0!important;
    width: 100%!important;
    height: 100%!important;
    opacity: 0.5!important;
}
.troublefilterright .mint-popup{
  position: fixed !important;
}
.troublefilterright .mint-popup-bottom{
top:156px;
}
.troublefilterright .dateinput{
width: 47%;
    position: absolute;
    right: 53%;
}
.troublefilterright .dateinput2{
  width: 47%;
    position: absolute;
    right: 0;
}
.troublefilterright .troublerightele{
      position: absolute;
    right: 0;
    width:100%;
}
.spanfilter{
  position: absolute;
    top: 123px;
    right: 16%;
    font-size: 16px;
    color: #fff;
}
.el-icon-close{
  display: inline-block!important;
}
</style>
