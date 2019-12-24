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
              <el-cascader
                filterable
                change-on-select
                @change="position_change"
                :props="areaParams"
                :show-all-levels="true"
                :options="areaList"
                v-model="areaStart"
              ></el-cascader>
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
      areaStart: { text: [], tips: "" },
      areaList: [],
      areaEnd: { text: "", tips: "" },
      areaEndList: [],
      areaParams: {
        label: 'areaName',
        value: 'id',
        children: 'children'
      },
    };
  },
  created() {
    this.InitSelect();
  },
  mounted() {
    var img = $(".img");
    var callBack = function(album) {
      //alert(album);
      console.log("album:" + album);
      console.log("img:" + img);
      album.forEach(function(val, idx) {
        var div = $("<div></div>");
        var img = $("<img/>");
        img.attr("src", val.data);
        console.log("src=" + JSON.stringify(val.data));
        div.append(img);
        $(".album").append(div);
      });
    };
    var carera = new $.Pgater($("#btn"), callBack);
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
          this.areaList = res.data.dicAreaList;
        })
        .catch(err => console.log(err));
    },
    position_change () {
      this.repairCompany.text = ''
      this.eqpList = []
      // 起始位置的线路和结束位置的线路必须一致
      if (this.areaStart.text.length < 3) {
        this.areaStart.tips = '起始线路必须是站点或区域'
        this.areaStart.text = []
        return false
      } else {
        apiArea.ListBigAreaByLine(this.areaStart.text[0]).then(res => {
          this.areaEndList = res.data
        }).catch(err => console.log(err))
        this.areaStart.tips = ''
        // return true
      }
      if (this.repairCompany.text !== '') {
        this.repair_change(this.repairCompany.text)
      }
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
</style>
