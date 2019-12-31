<template>
  <div class="wrapper">
    <isheader class="header"></isheader>
    <div class="troublescroll">
      <ul>
        <li class="itemli">
          <div class="itemlabel">
            <span>发生时间:</span>
          </div>
          <div class="itemvalue">
            <div class="input">
              <span
                class="dateinputright"
                placeholder="发生时间"
                @click="selectYear"
                v-model="happeningTime"
                >{{ happeningTime }}</span
              >
            </div>
            <mt-datetime-picker
              v-model="dateValue"
              type="date"
              ref="datePicker"
              year-format="{value} 年"
              month-format="{value} 月"
              date-format="{value} 日"
              :endDate="new Date()"
              @confirm="handleConfirm"
              @visible-change="visbleChange"
            ></mt-datetime-picker>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>报修时间:</span>
          </div>
          <div class="itemvalue">
            <div class="input">
              <span
                class="dateinputright"
                placeholder="报修时间"
                @click="selectYear2"
                v-model="reportedTime"
                >{{ reportedTime }}</span
              >
            </div>
            <mt-datetime-picker
              v-model="dateValue"
              type="date"
              ref="datePicker2"
              year-format="{value} 年"
              month-format="{value} 月"
              date-format="{value} 日"
              :endDate="new Date()"
              @confirm="handleConfirm2"
              @visible-change="visbleChange"
            ></mt-datetime-picker>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>抢修令号:</span>
          </div>
          <div class="itemvalue">
            <div class="input">
              <el-input
                v-model="urgentOrder"
                placeholder="请输入抢修令号"
              ></el-input>
            </div>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>故障等级:</span>
          </div>
          <div class="itemvalue itemvaluepick">
            <div class="input">
              <mt-cell
                title
                :value="level"
                is-link
                @click.native="handlerLevel"
              ></mt-cell>
              <mt-popup
                v-model="levelVisible"
                class="area-class"
                position="bottom"
              >
                <mt-picker
                  ref="levelpicker1"
                  :slots="levelslot1"
                  value-key="name"
                  :show-toolbar="true"
                >
                  <mt-button @click="handlelevelpickConfirm" class="sure"
                    >确认</mt-button
                  >
                </mt-picker>
              </mt-popup>
            </div>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>起始站点/区域:</span>
          </div>
          <div class="itemvalue itemvaluepick">
            <div class="input">
              <mt-cell
                title
                :value="areaString"
                is-link
                @click.native="handlerArea"
              ></mt-cell>
              <mt-popup
                v-model="areaVisible"
                class="area-class"
                position="bottom"
              >
                <mt-picker
                  ref="picker1"
                  :slots="slot1"
                  value-key="areaName"
                  @change="onValuesChange1"
                ></mt-picker>
                <mt-picker
                  ref="picker2"
                  :slots="slot2"
                  value-key="areaName"
                  @change="onValuesChange2"
                ></mt-picker>
                <mt-picker
                  ref="picker3"
                  :slots="slot3"
                  value-key="areaName"
                  @change="onValuesChange3"
                ></mt-picker>
                <mt-picker
                  ref="picker4"
                  :slots="slot4"
                  value-key="areaName"
                  :show-toolbar="true"
                >
                  <mt-button @click="handlePickConfirm" class="sure"
                    >确认</mt-button
                  >
                </mt-picker>
              </mt-popup>
            </div>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>结束站点:</span>
          </div>
          <div class="itemvalue itemvaluepick">
            <div class="input">
              <mt-cell
                title
                :value="endlocation"
                is-link
                @click.native="handlerEndLocation"
              ></mt-cell>
              <mt-popup
                v-model="endlocationVisible"
                class="area-class"
                position="bottom"
              >
                <mt-picker
                  ref="endlocationpicker"
                  :slots="endlocationslot"
                  value-key="areaName"
                  :show-toolbar="true"
                >
                  <mt-button @click="handleEndLocationConfirm" class="sure"
                    >确认</mt-button
                  >
                </mt-picker>
              </mt-popup>
            </div>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>故障设备:</span>
          </div>
          <div class="itemvalue itemvaluepick">
            <div class="input">
              <el-input
                v-model="eqpcode"
                placeholder="请输入故障图纸号"
                clearable
                class="troubleeqpcode"
              ></el-input>
              <el-button round @click.native="choseeqp()">确定</el-button>
            </div>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>(已选故障设备)</span>
          </div>
          <div class="itemvalue itemvaluepick">
            <div class="input">
              <span v-model="eqpIDs">{{ eqpShow }}</span>
            </div>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>报修单位:</span>
          </div>
          <div class="itemvalue itemvaluepick">
            <div class="input">
              <mt-cell
                title
                :value="companyString"
                is-link
                @click.native="handlerComany"
              ></mt-cell>
              <mt-popup
                v-model="companyVisible"
                class="area-class"
                position="bottom"
              >
                <mt-picker
                  ref="companypicker1"
                  :slots="companyslot1"
                  value-key="label"
                  @change="onCompanyChange1"
                ></mt-picker>
                <mt-picker
                  ref="companypicker2"
                  :slots="companyslot2"
                  value-key="label"
                  @change="onCompanyChange2"
                ></mt-picker>
                <mt-picker
                  ref="companypicker3"
                  :slots="companyslot3"
                  value-key="label"
                  :show-toolbar="true"
                >
                  <mt-button @click="handleCompanyConfirm" class="sure"
                    >确认</mt-button
                  >
                </mt-picker>
              </mt-popup>
            </div>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>报修人:</span>
          </div>
          <div class="itemvalue itemvaluepick">
            <div class="input">
              <mt-cell
                title
                :value="reportby"
                is-link
                @click.native="handlerReportBy"
              ></mt-cell>
              <mt-popup
                v-model="reportbyVisible"
                class="area-class"
                position="bottom"
              >
                <mt-picker
                  ref="reportbypicker"
                  :slots="reportByslot"
                  value-key="userName"
                  :show-toolbar="true"
                >
                  <mt-button @click="handleReportByConfirm" class="sure"
                    >确认</mt-button
                  >
                </mt-picker>
              </mt-popup>
            </div>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>报修故障描述:</span>
          </div>
          <div class="itemvalue">
            <div class="input">
              <el-input
                type="textarea"
                :rows="2"
                placeholder="请输入内容"
                v-model="repairDesc"
              ></el-input>
            </div>
          </div>
        </li>
        <li class="itemli itemlipic">
          <div id="btn">点击拍照</div>
          <div class="album"></div>
          <!-- <mt-popup v-model="previewPicVisible" class="previewPic" position="bottom">
                <mt-picker
                  ref="reportbypicker"
                  :slots="reportByslot"
                  value-key="userName"
                  :show-toolbar="true"
                >
                  <mt-button @click="handleReportByConfirm" class="sure">确认</mt-button>
                </mt-picker>
              </mt-popup> -->
        </li>
        <li class="itemli itemsubmit">
          <div class="itemsingle">
            <el-button
              type="primary"
              @click.native="submitclick()"
              v-show="submitshow"
              >提交</el-button
            >
          </div>
        </li>
      </ul>
    </div>
  </div>
</template>
<script>
import isheader from "./commom/header3.vue";
import BottomNavigation from "./commom/bottom.vue";
import tabs from "./commom/tabs2.vue";
import axios from "axios";
import cslCamera from "../assets/js/cslCamera.js";
import { transformDate, dateFtt } from "@/common/js/utils.js";
import { systemResource, orgType, dictionary } from "@/common/js/dictionary.js";
import api from "@/api/DeviceMaintainRegApi.js";
import apiArea from "@/api/AreaApi.js";
import apiOrg from "@/api/orgApi";
import apiEqp from "@/api/eqpApi";
import apiAuth from "@/api/authApi";

export default {
  name: "trouble",
  components: {
    isheader,
    tabs,
    BottomNavigation
  },
  data() {
    return {
      submitshow: false,
      happeningTime: "",
      reportedTime: "",
      urgentOrder: "",
      dateValue: "",
      // 故障等级
      level: "",
      levelValue: "",
      levelVisible: false,
      levelslot1: [
        {
          flex: 1,
          values: [],
          className: "levelslot1",
          textAlign: "center",
          defaultIndex: 0
        }
      ],
      //区域结束位置
      endlocation: "请选择",
      endlocationValue: "",
      endlocationVisible: false,
      endlocationslot: [
        {
          flex: 1,
          values: [],
          className: "endlocationslot1",
          textAlign: "center",
          defaultIndex: 0
        }
      ],
      //区域起始位置四级联动
      areaVisible: false,
      areaString: "",
      areaValue: "",
      line: 0,
      startlocation: 0,
      startlocationby: 0,
      slot1: [
        {
          flex: 1,
          values: [],
          className: "slot1",
          textAlign: "left",
          defaultIndex: 0
        }
      ],
      slot2: [
        {
          flex: 1,
          values: [],
          className: "slot2",
          textAlign: "left"
        }
      ],
      slot3: [
        {
          flex: 1,
          values: [],
          className: "slot3",
          textAlign: "left"
        }
      ],
      slot4: [
        {
          flex: 1,
          values: [],
          className: "slot4",
          textAlign: "left"
        }
      ],
      slotobj: [],
      //设备图纸
      eqpcode: "",
      eqpIDs: [],
      eqpShow: "",
      eqpShowArr: [],
      //报修单位
      companyslotobj: [],
      companyString: "",
      companyVisible: false,
      companyValue: "",
      companySel: 0,
      companyslot1: [
        {
          flex: 1,
          values: [],
          className: "companyslot1",
          textAlign: "left"
        }
      ],
      companyslot2: [
        {
          flex: 1,
          values: [],
          className: "companyslot2",
          textAlign: "left"
        }
      ],
      companyslot3: [
        {
          flex: 1,
          values: [],
          className: "companyslot3",
          textAlign: "left"
        }
      ],
      //报修人
      reportby: "请选择",
      reportByValue: "",
      reportbyVisible: false,
      reportByslot: [
        {
          flex: 1,
          values: [],
          className: "reportByslot1",
          textAlign: "center",
          defaultIndex: 0
        }
      ],
      repairDesc: "",
      fileIDsEdit: [],
      upfiles: [],
      previewPicVisible: true
    };
  },
  created() {},
  mounted() {
    var carera = new $.Pgater($("#btn"), this.callBack);
    this.InitSelect();

    this.loadTrouble();
  },
  methods: {
    callBack(album, fileid) {
      let upfilestmp = this.upfiles;

      let check = upfilestmp.filter(c => c === fileid)[0];
      // console.log('check:')
      // console.log(check)
      if (check === undefined) {
        this.upfiles.push(fileid);
      }
      // console.log(this.fileIDsEdit)
      var img = $(".img");
      album.forEach(function(val, idx) {
        var div = $("<div></div>");
        var img = $("<img/>");
        img.attr("src", val.data);
        img.attr("οnclick", "showPreViewPic()");
        // console.log("src=" + JSON.stringify(val.data));
        div.append(img);
        $(".album").append(div);
      });
    },
    showPreViewPic1() {
      this.previewPicVisible = true;
    },
    InitSelect() {
      apiAuth
        .getSubCode(dictionary.troubleLevel)
        .then(res => {
          // this.levelList = res.data;
          this.levelslot1[0].values = res.data;
          this.level = "请选择";
        })
        .catch(err => console.log(err));

      apiArea
        .SelectConfigAreaData()
        .then(res => {
          this.slotobj = res.data.dicAreaList;
          let slot1mp = res.data.dicAreaList;
          this.slot1[0].values = slot1mp;

          let slot2tmp = slot1mp[0].children;
          this.slot2[0].values = slot2tmp;

          let slot3tmp = slot2tmp[0].children;
          this.slot3[0].values = slot3tmp;

          if (slot3tmp != null) {
            let slot4tmp = slot3tmp[0].children;
            this.slot4[0].values = slot4tmp;
          }
          this.areaString = "请选择";
        })
        .catch(err => console.log(err));

      apiOrg
        .getOrgAll()
        .then(res => {
          this.reportCompanyList = res.data;
          this.companyslotobj = res.data;
          let slot1mp = res.data;
          this.companyslot1[0].values = slot1mp;

          let slot2tmp = slot1mp[0].children;
          this.companyslot2[0].values = slot2tmp;

          let slot3tmp = slot2tmp[0].children;
          this.companyslot3[0].values = slot3tmp;

          this.companyString = "请选择";
        })
        .catch(err => console.log(err));
    },
    selectYear() {
      if (this.happeningTime) {
        this.dateValue = this.happeningTime;
      } else {
        this.dateValue = new Date();
      }
      this.$refs.datePicker.open();
    },
    selectYear2() {
      if (this.reportedTime) {
        this.dateValue = this.reportedTime;
      } else {
        this.dateValue = new Date();
      }
      this.$refs.datePicker2.open();
    },
    handleConfirm(value) {
      this.happeningTime = dateFtt(value, "yyyy-MM-dd");
    },
    handleConfirm2(value) {
      this.reportedTime = dateFtt(value, "yyyy-MM-dd");
    },
    closeTouch() {
      document
        .getElementsByTagName("body")[0]
        .addEventListener("touchmove", this.handler, { passive: false }); //阻止默认事件
    },
    openTouch() {
      document
        .getElementsByTagName("body")[0]
        .removeEventListener("touchmove", this.handler, { passive: false }); //打开默认事件
    },
    visbleChange(val) {
      if (val) {
        this.closeTouch();
      } else {
        this.openTouch();
      }
    },
    handlePickConfirm() {
      let curpick1 = this.$refs.picker1.getValues()[0];
      let curpick2 = this.$refs.picker2.getValues()[0];
      let curpick3 = this.$refs.picker3.getValues()[0];
      let curpick4 = this.$refs.picker4.getValues()[0];
      let areaarr = [];
      let areaidarr = [];
      if (curpick1 != undefined) {
        areaarr.push(curpick1.areaName);
        areaidarr.push(curpick1.id);
        this.line = curpick1.id;
        apiArea
          .ListBigAreaByLine(curpick1.id)
          .then(res => {
            this.endlocationslot[0].values = res.data;
          })
          .catch(err => console.log(err));
      }
      if (curpick2 != undefined) {
        areaarr.push(curpick2.areaName);
        areaidarr.push(curpick2.id);
      }
      if (curpick3 != undefined) {
        areaarr.push(curpick3.areaName);
        areaidarr.push(curpick3.id);
        this.startlocation = curpick3.id;
        this.startlocationby = 1;
      }
      if (curpick4 != undefined) {
        areaarr.push(curpick4.areaName);
        areaidarr.push(curpick4.id);
        this.startlocationby = 2;
      }
      this.areaString = areaarr.join(",");
      this.areaValue = areaidarr.join(",");

      this.areaVisible = false;
    },
    handlelevelpickConfirm() {
      let curpick1 = this.$refs.levelpicker1.getValues()[0];
      this.level = curpick1.name;
      this.levelValue = curpick1.id;
      this.levelVisible = false;
    },
    handleEndLocationConfirm() {
      let curpick1 = this.$refs.endlocationpicker.getValues()[0];
      this.endlocation = curpick1.areaName;
      this.endlocationValue = curpick1.id;
      this.endlocationVisible = false;
    },
    handleReportByConfirm() {
      let curpick1 = this.$refs.reportbypicker.getValues()[0];
      this.reportby = curpick1.userName;
      this.reportByValue = curpick1.id;
      this.reportbyVisible = false;
    },
    handleCompanyConfirm() {
      let curpick1 = this.$refs.companypicker1.getValues()[0];
      let curpick2 = this.$refs.companypicker2.getValues()[0];
      let curpick3 = this.$refs.companypicker3.getValues()[0];
      let areaarr = [];
      let areaidarr = [];
      if (curpick1 != undefined) {
        areaarr.push(curpick1.label);
        areaidarr.push(curpick1.id);
        // console.log("curpick1.id");
        // console.log(curpick1.id);
        apiOrg
          .listUserByNode(curpick1.id)
          .then(res => {
            this.reportByslot[0].values = res.data;
          })
          .catch(err => console.log(err));
        this.companySel = curpick1.id;
      }
      if (curpick2 != undefined) {
        areaarr.push(curpick2.label);
        areaidarr.push(curpick2.id);
        apiOrg
          .listUserByNode(curpick2.id)
          .then(res => {
            this.reportByslot[0].values = res.data;
          })
          .catch(err => console.log(err));
        this.companySel = curpick2.id;
      }
      if (curpick3 != undefined) {
        areaarr.push(curpick3.label);
        areaidarr.push(curpick3.id);
        apiOrg
          .listUserByNode(curpick3.id)
          .then(res => {
            this.reportByslot[0].values = res.data;
          })
          .catch(err => console.log(err));
        this.companySel = curpick3.id;
      }
      this.companyString = areaarr.join(",");
      this.companyValue = areaidarr.join(",");
      this.companyVisible = false;
    },
    onValuesChange1(picker, values) {
      let s1 = this.slotobj;
      let s2 = s1.filter(c => c.id === values[0].id)[0];
      if (s2 != undefined) {
        this.slot2[0].values = s2.children;
      }
    },
    onValuesChange2(picker, values) {
      let dataall = this.slotobj;
      let curpick1 = this.$refs.picker1.getValues()[0];
      let s1 = dataall.filter(c => c.id === curpick1.id)[0];
      if (s1 != undefined) {
        let s2 = s1.children;
        let s3 = s2.filter(c => c.id === values[0].id)[0];
        this.slot3[0].values = s3.children;
      }
    },
    onValuesChange3(picker, values) {
      let dataall = this.slotobj;
      let curpick1 = this.$refs.picker1.getValues()[0];
      let curpick2 = this.$refs.picker2.getValues()[0];
      let s1 = dataall.filter(c => c.id === curpick1.id)[0];
      if (s1 != undefined) {
        let s2 = s1.children;
        if (s2 != undefined) {
          let s3 = s2.filter(c => c.id === curpick2.id)[0];
          if (s3 != undefined) {
            let s3c = s3.children;
            if (s3c != undefined) {
              let s4 = s3c.filter(c => c.id === values[0].id)[0];
              this.slot4[0].values = s4.children;
            }
          }
        }
      }
    },
    onCompanyChange1(picker, values) {
      let s1 = this.companyslotobj;
      let s2 = s1.filter(c => c.id === values[0].id)[0];
      if (s2 != undefined) {
        this.companyslot2[0].values = s2.children;
      }
    },
    onCompanyChange2(picker, values) {
      let dataall = this.companyslotobj;
      let curpick1 = this.$refs.companypicker1.getValues()[0];
      let s1 = dataall.filter(c => c.id === curpick1.id)[0];
      if (s1 != undefined) {
        let s2 = s1.children;
        let s3 = s2.filter(c => c.id === values[0].id)[0];
        this.companyslot3[0].values = s3.children;
      }
    },
    handlerArea() {
      this.areaVisible = true;
    },
    handlerLevel() {
      this.levelVisible = true;
    },
    handlerEndLocation() {
      this.endlocationVisible = true;
    },
    handlerComany() {
      this.companyVisible = true;
    },
    handlerReportBy() {
      this.reportbyVisible = true;
    },
    choseeqp() {
      // console.log(this.eqpcode)
      let parm = {
        order: "asc",
        sort: "eqp_code",
        rows: 10,
        page: 1,
        SearchCode: this.eqpcode
      };
      apiEqp
        .getEqp(parm)
        .then(res => {
          let eqplist = res.data.rows;
          if (eqplist != null && eqplist.length > 0) {
            let eqplistobj = this.eqpIDs;
            let c = eqplistobj.filter(c => c.eqp === eqplist[0].id);

            if (c.length === 0) {
              this.eqpIDs.push({
                eqp: eqplist[0].id,
                org: eqplist[0].topOrg,
                name: eqplist[0].name
              });
              this.eqpShowArr.push(eqplist[0].name);
            }
            this.eqpShow = this.eqpShowArr.join(",");
            console.log("eqpIDs:");
            console.log(this.eqpIDs);
          }
        })
        .catch(err => console.log(err));
    },
    submitclick() {
      let tfile = this.upfiles.join(",");
      this.fileIDsEdit.push({ type: "137", ids: tfile });
      let troubleReport = {
        HappeningTime: this.happeningTime,
        ReportedTime: this.reportedTime,
        Line: this.line,
        StartLocation: this.startlocation,
        StartLocationBy: this.startlocationby,
        StartLocationPath: this.areaValue,
        EndLocation: this.endlocationValue,
        EndLocationBy: 1,
        UrgentRepairOrder: this.urgentOrder,
        Eqps: JSON.stringify(this.eqpIDs),
        Level: this.levelValue,
        ReportedCompany: this.companySel,
        ReportedCompanyPath: this.companyValue,
        ReportedBy: this.reportByValue,
        Desc: this.repairDesc,
        UploadFiles:
          this.fileIDsEdit.length === 0 ? "" : JSON.stringify(this.fileIDsEdit)
      };
      // if (this.$route.params.type === 'Add') {
      api
        .SaveTroubleReport(troubleReport)
        .then(res => {
          if (res.code === 0) {
            this.$router.push({ name: "troublelist" });
            this.$message({
              message: "添加成功",
              type: "success"
            });
          } else {
            this.$message({
              message: res.msg === "" ? "添加失败" : res.msg,
              type: "error"
            });
          }
        })
        .catch(err => console.log(err));
      // }
    },
    loadTrouble() {
      let id = this.$route.query.id;
      if (id != 0) {
        this.submitshow = false;
        api
        .getTroubleReportByID(id)
        .then(res => {
          let data = res.data;
          this.happeningTime = dateFtt(
            new Date(data.happeningTime),
            "yyyy-MM-dd"
          );
          this.reportedTime = dateFtt(
            new Date(data.reportedTime),
            "yyyy-MM-dd"
          );
          let startlocationarr = [];
          let datastartLocationPath = data.startLocationPath;
          let slotdata = this.slotobj;
          let slot1 = this.slot1[0].values;
          let slot3 = this.slot3[0].values;
          let slot4 = this.slot4[0].values;

          let a1 = datastartLocationPath.split(",");
          if (a1[0] != undefined) {
            let s = slot1.filter(c => c.id == a1[0]);
            startlocationarr.push(s[0].areaName);
            if (a1[1] != undefined) {
              let slot2 = slotdata.filter(c => c.id == a1[0])[0];
              if (slot2 != undefined) {
                let cur2 = slot2.children;
                let s2 = cur2.filter(c => c.id == a1[1]);
                startlocationarr.push(s2[0].areaName);
                if (a1[2] != undefined) {
                  let cur3 = s2[0].children;
                  let s3 = cur3.filter(c => c.id == a1[2]);
                  startlocationarr.push(s3[0].areaName);
                  if (a1[3] != undefined) {
                    let cur4 = s3[0].children;
                    let s4 = cur4.filter(c => c.id == a1[3]);
                    startlocationarr.push(s4[0].areaName);
                  }
                }
              }
            }
          }
          this.areaString = startlocationarr.join(',')
          this.endlocation = data.endLocationName
          // EndLocationBy: 1,
          this.urgentOrder = data.urgentRepairOrder;
          // Eqps: JSON.stringify(this.eqpIDs),
          this.level = data.levelName;
          // ReportedCompany: this.companySel,
          // ReportedCompanyPath: this.companyValue,
          // ReportedBy: this.reportByValue,
          // Desc: this.repairDesc,
          // UploadFiles:
          //   this.fileIDsEdit.length === 0 ? "" : JSON.stringify(this.fileIDsEdit)
        })
        .catch(err => console.log(err));
      } else {
        this.submitshow = true;
      }
      
    
    
    
    
    }
  }
};
</script>
<style>
.wrapper {
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
  top: 70px;
  width: 100%;
  height: 85%;
  overflow: scroll;
  overflow-y: auto;
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
}

.itemlabel {
  display: inline-block;
  width: 30%;
  position: absolute;
  height: 30px;
}
.itemlabel span {
  top: 10%;
  position: absolute;
  color: #fff;
}
.itemvalue {
  display: inline-block;
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
#btn {
  width: 80px;
  height: 50px;
  background: #303133;
  text-align: center;
  line-height: 50px;
  margin: 0px auto;
  display: inline-block;
  position: absolute;
  color: #fff;
}
.album {
  width: 77%;
  display: -webkit-box;
  display: -ms-flexbox;
  display: inline-block;
  height: 50px;
  background: #999999;
  min-height: 51px;
  -ms-flex-pack: distribute;
  justify-content: space-around;
  -ms-flex-wrap: wrap;
  flex-wrap: wrap;
  left: 23%;
  position: absolute;
}
.album > div {
  width: 24%;
  height: 50px;
  display: inline-block;
  padding-left: 2%;
}
.album > div > img {
  width: 100%;
  height: 100%;
}
.itemli {
  position: relative;
  height: 50px;
  width: 100%;
}
.dateinputright {
  width: 100%;
  height: 30px;
  line-height: 18px;
  font-size: 14px;
  padding: 5px 8px;
  border: 1px solid #ddd;
  border-radius: 5px;
  color: #8e9092;
  position: absolute;
  margin: 0 auto;
}
.itemvaluepick .mint-popup-bottom {
  width: 100%;
}
.itemvaluepick .picker {
  display: inline-block;
  width: 24%;
}
.itemvaluepick .picker-item {
  font-size: 12px;
  padding: 0 0;
}
.itemvaluepick .mint-cell {
  min-height: 30px;
}
.itemvaluepick .mint-cell-wrapper {
  font-size: 12px;
}
.troublescroll ul::-webkit-scrollbar {
  display: none;
}
.itemlipic {
  padding-top: 20px;
}
.itemsingle {
  width: 100%;
  text-align: center;
}
.itemsubmit {
  top: 35px;
}
.previewPic {
  display: none;
}
.itemvalue .troubleeqpcode {
  width: 65%;
}
</style>
