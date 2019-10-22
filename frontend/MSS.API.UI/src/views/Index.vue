<template>
  <div class="wrap">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img class="icon" src="../common/images/icon-home.svg" alt=""> 首页
      </h2>
      <li class="listtitle">
        <span class="title">上海轨道交通18号线智能运维系统</span>
      </li>
      <a href="#/monitorCenter/eqpmonitor/list">进入系统</a>
    </div>
    <div class="con-padding-horizontal content">
      <div class="right">
        <div class="charts-wrap">
              <el-col :span="12" id="gaugeChart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="gaugeChart"  class="echart"></div>
              </el-col>
        </div>
        <div class="charts-wrap"><el-col :span="12" id="radarChart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:270px;" ref="radarChart"  class="echart"></div>
            </el-col>
        </div>
        <div class="charts-wrap">
                <span style="font-size: 16px;font-family: cursive;">我的待办</span>
                      <li class="list" v-for="(item) in DataList" :key="item.key">
              <div class="list-content">
                <div class="name"><a href="#">{{ item.appName }}</a></div>
                <div class="name">{{ item.activityName }}</div>
                <div class="name">{{ item.createdDateTime }}</div>
              </div>
            </li>
        </div>
      </div>
      <div class="right1">
        <div class="charts-wrap1">
          <div class="innerwrap">
            <div class="innerdiv1">
            <span class="innerspan1">累计无故障运营时间(min)</span>
            </div>
            <div class="innerdiv2">
            <span class="innerspan2">    <ICountUp
      :delay="delay"
      :endVal="endVal"
      :options="options"
    /></span>
            </div>
          </div>
          <div class="innerwrap">
                                      <el-col :span="12" id="hbarChart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:200px;" ref="hbarChart"  class="echart"></div>
            </el-col>
          </div>
        </div>
        <div class="charts-wrap">
                          <el-col :span="12" id="countChart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:290px;" ref="countChart"  class="echart"></div>
            </el-col>
        </div>
        <div class="charts-wrap">
                                <el-col :span="12" id="avgTimeChart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="avgTimeChart"  class="echart"></div>
              </el-col>
                   <!-- <span>我的申请</span>
            <li class="list" v-for="(item) in DataList1" :key="item.key">
              <div class="list-content">
                <div class="name">{{ item.appName }}</div>
                <div class="name">{{ item.processState }}</div>
                <div class="name">{{ item.createdDateTime }}</div>
              </div>
            </li> -->
        </div>
      </div>
            <div class="right">
        <div class="charts-wrap">
                      <el-col :span="12" id="pieChart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:290px;" ref="pieChart"  class="echart"></div>
            </el-col>
        </div>
        <div class="charts-wrap">
            <el-col :span="12" id="barChart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:260px;" ref="barChart"  class="echart"></div>
            </el-col>
        </div>
        <div class="charts-wrap">
            <el-col :span="12" id="lineChart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:290px;" ref="lineChart"  class="echart"></div>
            </el-col>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import mychart from './StatisticsReport/alarm/children/chart'
import indexchart from './StatisticsReport/alarm/children/chartIndex'
import staticsapi from '@/api/statisticsApi'
import { getNowFormatDate, ApiRESULT, transformDate } from '@/common/js/utils.js'
import workflowapi from '@/api/workflowApi'
import ICountUp from 'vue-countup-v2'
export default {
  name: 'Index',
  components: {
    ICountUp
  },
  data () {
    return {
      name: '',
      delay: 1000,
      endVal: 5896328,
      options: {
        useEasing: true,
        useGrouping: true,
        separator: ',',
        decimal: '.',
        prefix: '',
        suffix: ''
      },
      cycle: {id: 1, name: '管廊有分区修改'},
      tunnelList: [],
      tunnel: '',
      time: {
        text: '',
        tips: ''
      },
      DataList: [],
      DataList1: [],
      dateType: 0,
      dateChartCount: null,
      dateChartAvg: null,
      dateChartPie: null,
      dateChartRadar: null,
      dateChartGauge: null,
      dateChartBar: null,
      dateChartLine: null,
      dateChartHBar: null,
      loading_count: false,
      groupby: ['sub_system_id', 'eqp_type_id', 'manufacturer_id', 'team_id'],
      groupidxForCount: 0,
      groups: {
        sub_system_id: {
          modelID: 'subSystemID',
          modelName: 'subSystemName',
          reqParamKey: 'SubSystemIDs',
          legend: []
        },
        eqp_type_id: {
          modelID: 'eqpTypeID',
          modelName: 'eqpTypeName',
          reqParamKey: 'EqpTypeIDs',
          legend: []
        },
        manufacturer_id: {
          modelID: 'manufacturerID',
          modelName: 'manufacturerName',
          reqParamKey: 'ManufacturerIDs',
          legend: []
        },
        team_id: {
          modelID: 'teamID',
          modelName: 'teamName',
          reqParamKey: 'TeamIDs',
          legend: []
        }
      },
      bottomDesForCount: '',
      bottomDesForAvg: '',
      loading: false
    }
  },
  created () {
    var nowDate = getNowFormatDate()
    var d = new Date()
    d.setDate(d.getDate() - 60)
    var startDate = getNowFormatDate(d)
    this.time.text = [startDate, nowDate]
    this.searchResult()
  },
  mounted () {
    this.myMission()
    // this.myapply()
    this.drawPie()
    this.drawRadar()
    this.drawGauge()
    this.drawBar()
    this.drawLine()
    this.drawHBar()
  },
  methods: {
    drawCountChart (param, data, store) {
      if (store) {
        this.resultCountHistory.push({param: param, data: data})
      }

      this.dateChartCount = this.$echarts.init(this.$refs.countChart)
      this.dateChartCount.clear()
      // this.dateChartCount.on('click', this.goCount)
      // this.dateChartCount.on('magicTypeChanged', this.magicTypeChanged)
      // mychart.optionCount.toolbox.feature.myTool.onclick = this.backCount
      mychart.optionCount.title.subtext = this.subTitleCount.join('->')
      let groupModel = this.groups[this.groupby[this.groupidxForCount]]
      let cursor = 'pointer'
      if (this.groupidxForCount === 3) {
        cursor = 'default'
      }
      switch (this.groupidxForCount) {
        case 0:
          this.bottomDesForCount = '以子系统统计'
          break
        case 1:
          this.bottomDesForCount = '以设备类型统计'
          break
        case 2:
          this.bottomDesForCount = '以供应商统计'
          break
        case 3:
          this.bottomDesForCount = '以班组统计'
      }
      mychart.prepareChartData(data, groupModel, cursor)
      this.dateChartCount.setOption(mychart.optionCount)
    },
    drawAvgChart (param, data, store) {
      if (store) {
        this.resultAvgHistory.push({param: param, data: data})
      }

      this.dateChartAvg = this.$echarts.init(this.$refs.avgTimeChart)
      this.dateChartAvg.on('click', this.goAvg)
      mychart.optionAvg.toolbox.feature.myTool.onclick = this.backAvg
      mychart.optionAvg.title.subtext = this.subTitleAvg.join('->')
      let groupModel = this.groups[this.groupby[this.groupidxForAvg]]
      let cursor = 'pointer'
      if (this.groupidxForAvg === 3) {
        cursor = 'default'
      }
      switch (this.groupidxForAvg) {
        case 0:
          this.bottomDesForAvg = '以子系统统计'
          break
        case 1:
          this.bottomDesForAvg = '以设备类型统计'
          break
        case 2:
          this.bottomDesForAvg = '以供应商统计'
          break
        case 3:
          this.bottomDesForAvg = '以班组统计'
      }
      mychart.prepareChartData(data, groupModel, cursor)
      this.dateChartAvg.clear()
      this.dateChartAvg.setOption(mychart.optionAvg)
    },
    search (param, callbacks) {
      // this.loading = true
      callbacks.forEach(item => {
        if (item === this.drawCountChart) {
          this.loading_count = true
        }
        if (item === this.drawAvgChart) {
          this.loading_avg = true
        }
      })
      staticsapi.reportAlarm(param).then(res => {
        // this.loading = false
        callbacks.forEach(item => {
          if (item === this.drawCountChart) {
            this.loading_count = false
          }
          if (item === this.drawAvgChart) {
            this.loading_avg = false
          }
        })
        if (res.code === ApiRESULT.Success) {
          for (let callback of callbacks) {
            callback(param, res.data, true)
          }
        }
      }).catch(err => console.log(err))
    },
    drawPie () {
      this.dateChartPie = this.$echarts.init(this.$refs.pieChart)
      this.dateChartPie.clear()
      this.dateChartPie.setOption(indexchart.optionPie)
    },
    drawRadar () {
      this.dateChartRadar = this.$echarts.init(this.$refs.radarChart)
      this.dateChartRadar.clear()
      this.dateChartRadar.setOption(indexchart.optionRadar)
    },
    drawGauge () {
      this.dateChartGauge = this.$echarts.init(this.$refs.gaugeChart)
      this.dateChartGauge.clear()
      this.dateChartGauge.setOption(indexchart.optionGauge)
    },
    drawBar () {
      this.dateChartBar = this.$echarts.init(this.$refs.barChart)
      this.dateChartBar.clear()
      this.dateChartBar.setOption(indexchart.optionBar)
    },
    drawLine () {
      this.dateChartLine = this.$echarts.init(this.$refs.lineChart)
      this.dateChartLine.clear()
      this.dateChartLine.setOption(indexchart.optionLine)
    },
    drawHBar () {
      this.dateChartHBar = this.$echarts.init(this.$refs.hbarChart)
      this.dateChartHBar.clear()
      this.dateChartHBar.setOption(indexchart.optionHBar)
    },
    myMission () {
      let parm = {
        order: 'asc',
        sort: 'id',
        rows: 4,
        page: 1
      }
      workflowapi.getPage(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.createdDateTime = transformDate(item.createdDateTime)
        })
        this.DataList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    myapply () {
      let parm = {
        order: 'asc',
        sort: 'id',
        rows: 4,
        page: 1
      }
      workflowapi.getMyApplyPage(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.createdDateTime = transformDate(item.createdDateTime)
          if (item.processState === 0) {
            item.processState = 'NotStart'
          } else if (item.processState === 1) {
            item.processState = 'Ready'
          } else if (item.processState === 2) {
            item.processState = 'Running'
          } else if (item.processState === 4) {
            item.processState = 'Completed'
          } else if (item.processState === 5) {
            item.processState = 'Suspended'
          } else if (item.processState === 6) {
            item.processState = 'Canceled'
          } else if (item.processState === 7) {
            item.processState = 'Discarded'
          } else if (item.processState === 8) {
            item.processState = 'Terminated'
          }
        })
        this.DataList1 = res.data.rows
      }).catch(err => console.log(err))
    },
    onResize (el) {
      if (this.dateChartCount && el.id === 'countChart') {
        this.dateChartCount.resize()
      }
      if (this.dateChartAvg && el.id === 'avgTimeChart') {
        this.dateChartAvg.resize()
      }
      if (this.eqpTypeChartCount && el.id === 'countChartByEqpType') {
        this.eqpTypeChartCount.resize()
      }
      if (this.eqpTypeChartAvg && el.id === 'avgTimeChartByEqpType') {
        this.eqpTypeChartAvg.resize()
      }
      if (this.supplierChartCount && el.id === 'countChartBySupplier') {
        this.supplierChartCount.resize()
      }
      if (this.supplierChartAvg && el.id === 'avgTimeChartBySupplier') {
        this.supplierChartAvg.resize()
      }
      if (this.manufacturerChartCount && el.id === 'countChartByManufacturer') {
        this.manufacturerChartCount.resize()
      }
      if (this.manufacturerChartAvg && el.id === 'avgTimeChartByManufacturer') {
        this.manufacturerChartAvg.resize()
      }
      if (this.subSystemChartCount && el.id === 'countChartBySubSystem') {
        this.subSystemChartCount.resize()
      }
      if (this.subSystemChartAvg && el.id === 'avgTimeChartBySubSystem') {
        this.subSystemChartAvg.resize()
      }
      if (this.locationChartCount && el.id === 'countChartByLocation') {
        this.locationChartCount.resize()
      }
      if (this.locationChartAvg && el.id === 'avgTimeChartByLocation') {
        this.locationChartAvg.resize()
      }
      if (this.orgChartCount && el.id === 'countChartByOrg') {
        this.orgChartCount.resize()
      }
      if (this.orgChartAvg && el.id === 'avgTimeChartByOrg') {
        this.orgChartAvg.resize()
      }
    },
    // 搜索
    searchResult () {
      var sTime = ''
      var eTime = ''
      if (this.time.text) {
        sTime = this.time.text[0]
        eTime = this.time.text[1] + ' 23:59:59'
      }
      var param = {
        // SubSystemIDs: this.subSystem.join(','),
        // EqpTypeIDs: this.eqpType.join(','),
        // LocationPath: this.area.join(','),
        // SupplierIDs: this.supplier.join(','),
        // ManufacturerIDs: this.manufacturer.join(','),
        // OrgPath: this.teamPath.text.join(','),
        startTime: sTime,
        endTime: eTime,
        dateType: this.dateType,
        groupby: this.groupby.slice(0, 1).join(',')
      }
      this.groupidxForCount = 0
      this.groupidxForAvg = 0
      this.resultCountHistory = []
      this.resultAvgHistory = []
      this.subTitleCount = []
      this.subTitleAvg = []
      // // 获取当前子系统类别
      // this.getSubSystemSelected()
      // // 获取当前设备类型
      // this.getEqpTypeSelected()
      // // 获取当前制造商
      // this.getManufacturerSelected()
      this.search(param, [this.drawCountChart, this.drawAvgChart])
    },
    onReady: function (instance, CountUp) {
      const that = this
      instance.update(that.endVal + 100)
    }
  }
}
</script>
<style lang="scss" scoped>
.wrap{
  height:72%;
}
.header{
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: relative;
  height: percent(56, $content-height);

  &:after{
    content: "";
    position: absolute;
    bottom: -7px;
    left: 0;
    width: 100%;
    height: 11px;
    background: url(../common/images/line.png) no-repeat 0 0/100% 100%;
  }
  .icon{
    vertical-align: middle;
  }
}

.content{
  display: flex;
  justify-content: space-between;
  height: percent($content-height - 100, $content-height);
  padding: {
    top: 20px;
    bottom: 20px;
  }

  .left{
    width: percent(565, $content-width - 48);
    height: 100%;
    background: #28272E;
    border-radius: $border-radius;

    // .title{
    //   display: flex;
    //   align-items: center;
    //   height: percent(40, 565);
    //   padding-left: 15px;
    // }

    .picture-wrap{
      height: percent(565 - 40, 565);
      border-radius: 0 0 $border-radius $border-radius;
    }
  }

  .right{
    display: flex;
    flex-wrap: wrap;
    // width: percent(325, $content-width - 48);
    width: 25%;

    .charts-wrap{
      box-sizing: border-box;
      width: 100%;
      height: percent(215, $content-height - 100);
      padding: 0px;
      // background: #28272E;
      border-radius: $border-radius;
      border: 1px solid #14314e;
      margin:10px;
      &:last-of-type{
        align-self: flex-end;
      }
    }
  }

  .right1{
    display: flex;
    flex-wrap: wrap;
    // width: percent(325, $content-width - 48);
    width: 50%;

    .charts-wrap{
      box-sizing: border-box;
      width: 100%;
      height: 56%;
      padding: 0px;
      // background: #28272E;
      border-radius: $border-radius;
      margin:10px;
      border: 1px solid #14314e;
      &:last-of-type{
        align-self: flex-end;
      }
    }

    .charts-wrap1{
      box-sizing: border-box;
      width: 100%;
      height: 30%;
      padding: 0px;
      // background: #28272E;
      border-radius: $border-radius;
      margin:10px;
      border: 1px solid #14314e;
      &:last-of-type{
        align-self: flex-end;
      }
      .innerwrap{
          float: left;
          width: 50%;
          height: 100%;
          display: flex;
          justify-content:center;
          // align-items:center;
          .innerdiv1{
            display: inline-block;
            position: absolute;
            padding-top: 38px;
            font-size: 20px;
          }
          .innerdiv2{
            display: inline-block;
            // position: absolute;
            padding-top: 63px;
            font-size: 53px;
          }
      }
    }
  }
}

.el-col-12{
  width:100%
}
a{
 cursor: pointer;
}
.echart{
  height: 100%;
  width: 100%;
}
  .iCountUp {
    font-size: 12em;
    margin: 0;
    color: #4d63bc;
  }

.listtitle{
  // box-sizing: border-box;
  width: 456px;
  height: 127px;
  background: url(../components/header/images/title.png) no-repeat 0 13px/100% 100%;
  // text-align: center;
  // font-size: 1.5em;
  display: flex;
  justify-content:center;
  align-items:center;
  .title{
    font-size: 1.5em;
    font-weight: bold;
  }
}
.list{
  padding-left:10px;
  padding-top: 10px;
  list-style:unset;
  // margin:0;
  // padding:0;
  .list-content{
    padding-left: 13px;
    top: -17px;
    position: relative;
  }
}
</style>
<style>
#hbarChart>.echart>div{
  top:-16px;
  background-color: unset !important;
}
#barChart>.echart>div{
  background-color: unset !important;
}
</style>
