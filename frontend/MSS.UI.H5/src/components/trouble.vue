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
          <div class="itemvalue">
            <div class="input">
              <mt-cell title :value="areaString" is-link @click.native="handlerArea"></mt-cell>
              <mt-popup v-model="areaVisible" class="area-class" position="bottom">
                <mt-picker :slots="slot1" value-key="areaName" @change="onValuesChange1"></mt-picker>
                <mt-picker :slots="slot2" value-key="areaName" @change="onValuesChange2"></mt-picker>
                <mt-picker :slots="slot3" value-key="areaName" @change="onValuesChange3"></mt-picker>
                <mt-picker :slots="slot4" value-key="areaName"></mt-picker>
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
import data from "../assets/data/data.json";
let index = 0;
let index2 = 0;
let index3 = 0;
// 初始化省
// var province = data.map(res => {
//   return res.name;
// });
let province = [];
let provinceId = [];
// console.log("main:");
// console.log(province);
// 初始化市
// let city = data[index].childs.map(res => {
//   return res.name
// })
let city = [];
let cityId = [];
// 初始化区
// let area = data[index].childs[index2].childs.map(res => {
//   return res.name
// })
let area = [];
let areaId = [];
// 初始化街
// let street = data[index].childs[index2].childs[index3].childs.map(res => {
//   return res.name
// })
let street = [];
let streetId = [];
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
      areaString: "请选择",
      areaValue: "",
      streetString: "请选择",
      slot1: [
        {
          flex: 1,
          values: [],
          className: "slot1",
          textAlign: "left"
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
      s1pick:{},
      s2pick:{},
      s3pick:{},
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
          this.s1pick = slot1mp[0]
          let slot2tmp = slot1mp[0].children
          this.slot2[0].values = slot2tmp
          // this.s2pick = slot2mp[0]
          let slot3tmp = slot2tmp[0].children
          this.slot3[0].values = slot3tmp
          // this.s3pick = slot3mp[0]
          if(slot3tmp != null){
            let slot4tmp = slot3tmp[0].children
            this.slot4[0].values = slot4tmp
          }
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
    // onValuesChange(picker, values) {
    //   // console.log('onValuesChange_start')
    //   // console.log('values:')
    //   // console.log(values)
    //   let one = values[0];
    //   let two = values[1];
    //   let three = values[2];
    //   index = province.indexOf(one);
    //   if (index >= 0 && province.length > 0) {
    //     city = data[index].childs.map(res => {
    //       return res.name;
    //     });
    //     picker.setSlotValues(1, city);
    //     two = values[1];
    //   }

    //   index2 = city.indexOf(two);
    //   if (index2 >= 0 && city.length > 0) {
    //     area = data[index].childs[index2].childs.map(res => {
    //       return res.name;
    //     });
    //     picker.setSlotValues(2, area);
    //     three = values[2];
    //   }
    //   index3 = area.indexOf(three);
    //   if (index >= 0 && index2 >= 0 && index3 >= 0) {
    //     street = data[index].childs[index2].childs[index3].childs.map(res => {
    //       return res.name;
    //     });
    //     this.slotstree[0].values = street;
    //   }

    //   if (index2 === -1 || index3 === -1) {
    //     this.streetString = "无可选街道";
    //   }
    //   this.areaString = values.join(",");
    // },
    onValuesChange1(picker, values) {
      this.s1pick = values[0]
      console.log('s1pick:')
      console.log(this.s1pick)
      let s1 = this.slotobj
      let s2 = s1.filter(c => c.id === values[0].id)[0]
      if(s2 != undefined)
      {
        this.slot2[0].values = s2.children
      }
      // this.areaString = values.join(",")
    },
    onValuesChange2(picker, values) {
      let dataall = this.slotobj
      let s1 = dataall.filter(c => c.id === this.s1pick.id)[0]
      if(s1 != undefined){
        let s2 = s1.children
        this.s2pick = s2.filter(c=>c.id === values[0].id)[0]
        this.slot3[0].values = this.s2pick.children
      }
    },
    onValuesChange3(picker, values) {
      let dataall = this.slotobj
      let s1 = dataall.filter(c => c.id === this.s1pick.id)[0]
      if(s1 != undefined){
        let s2 = s1.children
        if(s2 != undefined){
          let s3 = s2.filter(c=>c.id === this.s2pick.id)[0]
          if(s3 != undefined){
            console.log('s3')
            console.log(s3)
            let s3p = s3.children
            this.s3pick = s3p.filter(c=>c.id === values[0].id)[0]
            console.log('this.s3pick')
            console.log(this.s3pick)
            this.slot4[0].values = this.s3pick.children
          }

          
        }
        
      }
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
.itemvalue .mint-popup-bottom{
  width:100%;
}
.itemvalue .picker{
  display:inline-block;
  width: 24%;
}
.itemvalue .picker-item{
  font-size: 12px;
  padding: 0 0;
}
</style>
