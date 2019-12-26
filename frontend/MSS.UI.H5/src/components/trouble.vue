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
              >{{ happeningTime }}</span>
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
              >{{ reportedTime }}</span>
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
              <el-input v-model="urgentOrder" placeholder="请输入抢修令号"></el-input>
            </div>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>故障等级:</span>
          </div>
          <div class="itemvalue">
            <div class="input">
              <el-select v-model="level" clearable filterable placeholder="请选择故障等级">
                <el-option
                  v-for="item in levelList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id"
                ></el-option>
              </el-select>
            </div>
          </div>
        </li>
        <li class="itemli">
          <div class="itemlabel">
            <span>起始站点/区域:</span>
          </div>
          <div class="itemvalue itemvaluepick">
            <div class="input">
              <mt-cell title :value="areaString" is-link @click.native="handlerArea"></mt-cell>
              <mt-popup v-model="areaVisible" class="area-class" position="bottom">
                <mt-picker ref="picker1" :slots="slot1" value-key="areaName" @change="onValuesChange1">
                </mt-picker>
                <mt-picker ref="picker2" :slots="slot2" value-key="areaName" @change="onValuesChange2"></mt-picker>
                <mt-picker ref="picker3" :slots="slot3" value-key="areaName" @change="onValuesChange3"></mt-picker>
                <mt-picker ref="picker4" :slots="slot4" value-key="areaName" :show-toolbar="true">
                  <mt-button @click="handlePickConfirm" class="sure" >确认</mt-button>
                </mt-picker>
              </mt-popup>
            </div>
          </div>
        </li>
        <li class="itemli">
          <div id="btn">点击拍照</div>
          <div class="album"></div>
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
      happeningTime: "",
      reportedTime: "",
      urgentOrder: "",
      dateValue: "",
      level: "",
      levelList: [],
      //四级联动
      areaVisible: false,
      streetVisible: false,
      areaString: "",
      areaValue: "",
      streetString: "请选择",
      slot1: [
        {
          flex: 1,
          values: [],
          className: "slot1",
          textAlign: "left",
          defaultIndex: 0,
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
      slotobj:[],
      //四级联动
    };
  },
  created() {
    // this.InitSelect();
  },
  mounted() {
    var img = $(".img");
    var callBack = function(album) {
      //alert(album);
      // console.log("album:" + album);
      // console.log("img:" + img);
      album.forEach(function(val, idx) {
        var div = $("<div></div>");
        var img = $("<img/>");
        img.attr("src", val.data);
        // console.log("src=" + JSON.stringify(val.data));
        div.append(img);
        $(".album").append(div);
      });
    };
    var carera = new $.Pgater($("#btn"), callBack);
    this.InitSelect();
  },
  methods: {
    InitSelect() {
      apiAuth
        .getSubCode(dictionary.troubleLevel)
        .then(res => {
          this.levelList = res.data;
        })
        .catch(err => console.log(err));

      apiArea
        .SelectConfigAreaData()
        .then(res => {
          this.slotobj = res.data.dicAreaList
          let slot1mp = res.data.dicAreaList
          this.slot1[0].values = slot1mp

          let slot2tmp = slot1mp[0].children
          this.slot2[0].values = slot2tmp

          let slot3tmp = slot2tmp[0].children
          this.slot3[0].values = slot3tmp

          if(slot3tmp != null){
            let slot4tmp = slot3tmp[0].children
            this.slot4[0].values = slot4tmp
          }
        })
        .catch(err => console.log(err));
        this.areaString = '请选择'
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
    handlePickConfirm () {
      let curpick1 = this.$refs.picker1.getValues()[0]
      let curpick2 = this.$refs.picker2.getValues()[0]
      let curpick3 = this.$refs.picker3.getValues()[0]
      let curpick4 = this.$refs.picker4.getValues()[0]
      let areaarr = []
      if(curpick1 != undefined){
        areaarr.push(curpick1.areaName)
      }
      if(curpick2 != undefined){
        areaarr.push(curpick2.areaName)
      }
      if(curpick3 != undefined){
        areaarr.push(curpick3.areaName)
      }
      if(curpick4 != undefined){
        areaarr.push(curpick4.areaName)
      }
      this.areaString = areaarr.join(",")
      this.areaVisible = false
    },
    onValuesChange1(picker, values) {
        let s1 = this.slotobj
        let s2 = s1.filter(c => c.id === values[0].id)[0]
        if(s2 != undefined)
        {
          this.slot2[0].values = s2.children
        }
      
    },
    onValuesChange2(picker, values) {
        let dataall = this.slotobj
        let curpick1 = this.$refs.picker1.getValues()[0]
        let s1 = dataall.filter(c => c.id === curpick1.id)[0]
        if(s1 != undefined){
          let s2 = s1.children
          let s3 = s2.filter(c=>c.id === values[0].id)[0]
          this.slot3[0].values = s3.children
        }
    },
    onValuesChange3(picker, values) {
        let dataall = this.slotobj
        let curpick1 = this.$refs.picker1.getValues()[0]
        let curpick2 = this.$refs.picker2.getValues()[0]
        let s1 = dataall.filter(c => c.id === curpick1.id)[0]
        if(s1 != undefined){
          let s2 = s1.children
          if(s2 != undefined){
            let s3 = s2.filter(c=>c.id === curpick2.id)[0]
            if(s3 != undefined){
              let s3c = s3.children
              if(s3c != undefined){
                  let s4 = s3c.filter(c=>c.id === values[0].id)[0]
                  this.slot4[0].values = s4.children
              }
              
            }
          }
        }
    },
    joinArea(){
      console.log('111')
      console.log(this.slotflag)
      let areaarr = []
      this.areaString = '请选择'
      if(this.s1pick != undefined){
          areaarr.push(this.s1pick.areaName)
      }
      if(this.s2pick != undefined){
          areaarr.push(this.s2pick.areaName)
      }
      if(this.s3pick != undefined){
          areaarr.push(this.s3pick.areaName)
      }
      if(this.s4pick != undefined){
          areaarr.push(this.s4pick.areaName)
      }
      if(this.slotflag){
        this.areaString = areaarr.join(",")
      }else{
        this.slotflag = true
      }
      console.log('222')
      console.log(this.slotflag)
      console.log('joinArea')
      console.log(this.areaString)
    },
    handlerArea() {
      this.areaVisible = true;
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
  top: 90px;
  width: 100%;
  height: 100%;
}

/* .input {
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
} */
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
  width: 200px;
  height: 50px;
  background: deeppink;
  text-align: center;
  line-height: 50px;
  margin: 10px auto;
}
.album {
  width: 100%;
  display: flex;
  height: auto;
  background: #999999;
  min-height: 50px;
  justify-content: space-around;
  flex-wrap: wrap;
}
.album > div {
  width: 24%;
  height: auto;
}
.album > div > img {
  width: 100%;
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
.itemvaluepick .mint-popup-bottom{
  width:100%;
}
.itemvaluepick .picker{
  display:inline-block;
  width: 24%;
}
.itemvaluepick .picker-item{
  font-size: 12px;
  padding: 0 0;
}
.itemvaluepick .mint-cell{
  min-height: 30px;
}
.itemvaluepick .mint-cell-wrapper{
  font-size: 12px;
}
</style>
