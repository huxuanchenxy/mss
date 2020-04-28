<template>
  <div class="wrap">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img class="icon" src="../common/images/icon-home.svg" alt=""> 首页
      </h2>
      <li class="listtitle">
        <span class="title">上海轨道交通18号线维修支持系统</span>
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
              <div style="width:100%; height:270px;" ref="gaugeChart"  class="echart"></div>
              </el-col>
        </div>
        <div class="charts-wrap"><el-col :span="12" id="radarChart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:240px;" ref="radarChart"  class="echart"></div>
            </el-col>
        </div>
        <div class="charts-wrap">
            <el-col :span="12" id="leftBottomChart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:240px;" ref="leftBottomChart"  class="echart"></div>
            </el-col>
        </div>
      </div>
      <div class="right1">
        <div class="charts-wrap1">
          <div class="innerwrap">
            <div class="innerdiv1">
            <span class="innerspan1">累计无故障运营时间(min)</span>
            <div>
              <svg t="1572317585470" class="iconarrowup" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="1560" width="32" height="32"><path d="M703.280488 365.511671c-4.329609 10.697643-13.921058 17.040095-25.236777 17.040095L544.234145 382.551766 544.234145 835.109955c0 17.838274-14.398942 32.296567-32.232099 32.296567-17.838274 0-32.236192-14.458293-32.236192-32.296567L479.765855 382.551766l-134.206608 0c-11.314697 0-20.906146-6.342452-25.234731-17.040095-4.330632-10.69969-2.081405-22.148439 5.92085-30.334889l166.241208-170.104189c5.333472-5.459339 11.773138-8.1711 19.315928-8.1711 7.543813 0 13.982456 2.742461 19.316951 8.201799l166.241208 170.068374C705.358823 343.358115 707.610096 354.813005 703.280488 365.511671z" p-id="1561" fill="#d4237a"></path></svg>
              <span class="spantongbi">同比增加</span>
              <span class="spantongbi1"><ICountUp
      :delay="delay"
      :endVal="endVal1"
      :options="options"
    />%</span>
            </div>
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
              <div style="width:100%; height:260px;" ref="pieChart"  class="echart"></div>
            </el-col>
        </div>
        <div class="charts-wrap">
            <!--设备健康度-->
            <el-col :span="12" id="lineChart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:240px;" ref="lineChart"  class="echart"></div>
            </el-col>
        </div>
        <div class="charts-wrap">
          <el-col :span="12" id="pie2Chart"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:280px;" ref="pie2Chart"  class="echart"></div>
            </el-col>
        </div>
      </div>
    </div>
    <app-footer></app-footer>
  </div>
</template>
<script>
// import mychart from './StatisticsReport/alarm/children/chart'
import indexchart from './StatisticsReport/alarm/children/chartIndex'
import staticsapi from '@/api/statisticsApi'
import { getNowFormatDate, ApiRESULT } from '@/common/js/utils.js'
import expertapi from '@/api/ExpertApi'
import ICountUp from 'vue-countup-v2'
import AppFooter from '@/components/footer/AppFooter'
export default {
  name: 'Index',
  components: {
    ICountUp,
    AppFooter
  },
  data () {
    return {
      name: '',
      delay: 1000,
      endVal: 5896328,
      endVal1: 38,
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
      dateType: 1,
      dateChartCount: null,
      dateChartAvg: null,
      dateChartPie: null,
      dateChartRadar: null,
      dateChartGauge: null,
      dateChartBar: null,
      dateChartLine: null,
      dateChartHBar: null,
      dateChartPie2: null,
      dateChartLeftBottom: null,
      loading_count: false,
      groupby: ['sub_system_id', 'eqp_type_id', 'manufacturer_id', 'team_id', 'major_id'],
      groupidxForCount: 0,
      groupidxForAvg: 0,
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
      loading: false,
      activeIndex: 0,
      intnum: undefined
    }
  },
  created () {
    this.getRunningTime()
    var nowDate = getNowFormatDate()
    var d = new Date()
    d.setDate(d.getDate() - 60)
    var startDate = getNowFormatDate(d)
    this.time.text = [startDate, nowDate]
    this.searchResult()
    this.ScrollUp()
  },
  mounted () {
    this.myMission()
    // this.myapply()
    this.drawPie()
    this.drawRadar()
    this.drawGauge()
    // this.drawBar()
    this.drawLine()
    this.drawHBar()
    // this.drawCountChart()
    // this.drawAvgChart()
    this.drawPie2()
    this.drawleftbottom()
  },
  computed: {
    top () {
      return -this.activeIndex * 50 + 'px'
    }
  },
  methods: {
    drawCountChart (param, data) {
      this.dateChartCount = this.$echarts.init(this.$refs.countChart)
      this.dateChartCount.clear()
      let groupModel = this.groups[this.groupby[this.groupidxForCount]]
      let cursor = 'default'
      indexchart.prepareChartData(data, groupModel, cursor)
      this.dateChartCount.setOption(indexchart.middleOption1)
    },
    drawAvgChart () {
      this.dateChartAvg = this.$echarts.init(this.$refs.avgTimeChart)
      this.dateChartAvg.clear()
      this.dateChartAvg.setOption(indexchart.middleOption2)
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
      staticsapi.reportTroubleRankByStation().then(res => {
        this.dateChartPie = this.$echarts.init(this.$refs.pieChart)
        this.dateChartPie.clear()
        if (res.code === ApiRESULT.Success) {
          if (res.data != null) {
            let seriesdata = []
            let legenddata = []
            res.data.map(item => {
              let cur = {}
              cur.value = item.troublecount
              cur.name = item.name
              seriesdata.push(cur)
              legenddata.push(item.name)
            })
            indexchart.optionPie.series[0].data = seriesdata
            indexchart.optionPie.legend.data = legenddata
          }
        }
        this.dateChartPie.setOption(indexchart.optionPie)
      }).catch(err => console.log(err))
    },
    drawRadar () {
      this.dateChartRadar = this.$echarts.init(this.$refs.radarChart)
      this.dateChartRadar.clear()
      this.dateChartRadar.setOption(indexchart.optionRadar)
    },
    drawGauge () {
      staticsapi.getRunningCost().then(res => {
        this.dateChartGauge = this.$echarts.init(this.$refs.gaugeChart)
        this.dateChartGauge.clear()
        if (res.code === ApiRESULT.Success) {
          if (res.data != null) {
            indexchart.optionGauge.series[0].data[0].value = res.data
          }
        }
        this.dateChartGauge.setOption(indexchart.optionGauge)
      }).catch(err => console.log(err))
    },
    drawBar () {
      this.dateChartBar = this.$echarts.init(this.$refs.barChart)
      this.dateChartBar.clear()
      this.dateChartBar.setOption(indexchart.optionBar)
    },
    drawLine () {
      expertapi.GetHealthChart().then(res => {
        this.dateChartLine = this.$echarts.init(this.$refs.lineChart)
        this.dateChartLine.clear()
        if (res.code === ApiRESULT.Success) {
          if (res.data != null) {
            indexchart.optionLine.xAxis[0].data = res.data.xAxisData
            let legenddata = []
            res.data.seriesData.map(item => {
              let cur = {}
              cur.name = item.name
              legenddata.push(cur.name)
              cur.type = 'line'
              cur.data = item.dataAvg
              indexchart.optionLine.series.push(cur)
            })
            indexchart.optionLine.legend.data = legenddata
          }
        }
        this.dateChartLine.setOption(indexchart.optionLine)
      }).catch(err => console.log(err))
    },
    drawHBar () {
      staticsapi.reportPlanChart().then(res => {
        this.dateChartHBar = this.$echarts.init(this.$refs.hbarChart)
        this.dateChartHBar.clear()
        if (res.code === ApiRESULT.Success) {
          if (res.data != null) {
            let yAxisData = []
            let seriesData = []
            res.data.map(item => {
              seriesData.push(item.count)
              yAxisData.push(item.name)
            })
            indexchart.optionHBar.yAxis.data = yAxisData
            indexchart.optionHBar.series[0].data = seriesData
          }
        }
        this.dateChartHBar.setOption(indexchart.optionHBar)
      }).catch(err => console.log(err))
    },
    drawPie2 () {
      staticsapi.getCostChart().then(res => {
        this.dateChartPie2 = this.$echarts.init(this.$refs.pie2Chart)
        this.dateChartPie2.clear()
        if (res.code === ApiRESULT.Success) {
          if (res.data != null) {
            let seriesData = {}
            res.data.map(item => {
              if (item.costType === 210 || item.costType === 212) {
                if (!seriesData[item.year]) {
                  seriesData[item.year] = []
                }
                let obj = {}
                obj.value = item.value
                obj.name = item.costName
                seriesData[item.year].push(obj)
              }
            })
            console.log(seriesData)
            indexchart.pieOption2.series[0].name = Object.keys(seriesData)[0]
            indexchart.pieOption2.series[0].data = Object.values(seriesData)[0]
            indexchart.pieOption2.series[1].name = Object.keys(seriesData)[1]
            indexchart.pieOption2.series[1].data = Object.values(seriesData)[1]
          }
        }
        this.dateChartPie2.setOption(indexchart.pieOption2)
      }).catch(err => console.log(err))
    },
    drawleftbottom () {
      staticsapi.getPidChart().then(res => {
        this.dateChartLeftBottom = this.$echarts.init(this.$refs.leftBottomChart)
        this.dateChartLeftBottom.clear()
        if (res.code === ApiRESULT.Success) {
          if (res.data.rows != null) {
            let seriesData = []
            res.data.rows.map(item => {
              let curobj = {}
              curobj.name = item.nodeName
              curobj.value = item.capacityCount
              seriesData.push(curobj)
            })
            indexchart.LeftBottomOption.series[0].data = seriesData
          }
        }
        this.dateChartLeftBottom.setOption(indexchart.LeftBottomOption)
      }).catch(err => console.log(err))
    },
    myMission () {
      this.DataList = [
        {key: 1, name: '习近平发表大力发展基建的重要讲话'},
        {key: 2, name: '应勇等市领导调研18号线江浦路站'},
        {key: 3, name: '18号线11月29日起工作日早高峰增能'},
        {key: 4, name: '18号线正式进入轨道工程施工阶段'},
        {key: 5, name: '“不忘初心、牢记使命”主题教育全面启动'},
        {key: 6, name: '“城市荣光—庆祝上海解放70周年”主题展览'},
        {key: 7, name: '中央环保督察组交办问题处理情况'}
      ]
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
      staticsapi.getNow().then(res => {
        if (res.code === ApiRESULT.Success) {
          if (res.data != null) {
            var sTime = res.data.startTime
            var eTime = res.data.endTime
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
            this.search(param, [this.drawCountChart, this.drawAvgChart])
          }
        }
      }).catch(err => console.log(err))
    },
    onReady: function (instance, CountUp) {
      const that = this
      instance.update(that.endVal + 100)
      instance.update(that.endVal1 + 100)
    },
    Stop () {
      clearInterval(this.intnum)
    },
    Up () {
      this.ScrollUp()
    },
    ScrollUp () {
      this.intnum = setInterval(_ => {
        if (this.activeIndex < this.DataList.length) {
          this.activeIndex += 1
        } else {
          this.activeIndex = 0
        }
      }, 1000)
    },
    getRunningTime () {
      staticsapi.reportRunningtime().then(res => {
        if (res.code === ApiRESULT.Success) {
          if (res.data != null) {
            this.endVal = res.data
          }
        }
      }).catch(err => console.log(err))
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
      height: 46%;
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
      height: 54%;
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
            padding-top: 31px;
            font-size: 19px;
            .iconarrowup{
                position: absolute;
                right: -13%;
            }
            .spantongbi{
              position: absolute;
              font-size: 1px;
              right: -32%;
            }
            .spantongbi1{
              position: absolute;
              font-size: 1px;
              right: -21%;
              top: 125%;
            }
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
#radarChart>div>div>canvas{
  top:1%;
}
.scroll-wrap{
 height: 80%;
 overflow: hidden;
}
.scroll-content {
position: relative;
transition: top 0.5s;
}
.scroll-content p{
line-height: 50px;
text-align: center;
}
</style>
