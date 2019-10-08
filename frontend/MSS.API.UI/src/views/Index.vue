<template>
  <div class="wrap">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img class="icon" src="../common/images/icon-home.svg" alt=""> 首页
      </h2>
    </div>
    <div class="con-padding-horizontal content">
      <div class="right">
        <div class="charts-wrap">
                      <el-col :span="12" id="countChart" v-resize="onResize" v-loading="loading_count"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="countChart"  class="echart"></div>
            </el-col>
        </div>
        <div class="charts-wrap">
          <charts-ht-rt :tunelID="{TunnelCode: '01', TunnelName: '管廊有分区修改'}" isFirst='1' ></charts-ht-rt>
        </div>
      </div>
      <div class="right">
        <div class="charts-wrap">
          <charts-task-complete :tunelID="{id: 1, name: '管廊有分区修改'}" type='1'></charts-task-complete>
        </div>
        <div class="charts-wrap">
          <charts-ht-rt :tunelID="{TunnelCode: '01', TunnelName: '管廊有分区修改'}" isFirst='1' ></charts-ht-rt>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import mychart from './StatisticsReport/alarm/children/chart'
import staticsapi from '@/api/statisticsApi'
import { getNowFormatDate, ApiRESULT } from '@/common/js/utils.js'
export default {
  name: 'Index',
  data () {
    return {
      name: '',
      cycle: {id: 1, name: '管廊有分区修改'},
      tunnelList: [],
      tunnel: '',
      time: {
        text: '',
        tips: ''
      },
      dateType: 0,
      dateChartCount: null,
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
    search (param, callbacks) {
      // this.loading = true
      callbacks.forEach(item => {
        if (item === this.drawCountChart) {
          this.loading_count = true
        }
        // if (item === this.drawAvgChart) {
        //   this.loading_avg = true
        // }
      })
      staticsapi.reportAlarm(param).then(res => {
        // this.loading = false
        callbacks.forEach(item => {
          if (item === this.drawCountChart) {
            this.loading_count = false
          }
          // if (item === this.drawAvgChart) {
          //   this.loading_avg = false
          // }
        })
        if (res.code === ApiRESULT.Success) {
          for (let callback of callbacks) {
            callback(param, res.data, true)
          }
        }
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
    }
  }
}
</script>
<style lang="scss" scoped>
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

  .title{
    font-size: PXtoEm(18);
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

    .title{
      display: flex;
      align-items: center;
      height: percent(40, 565);
      padding-left: 15px;
    }

    .picture-wrap{
      height: percent(565 - 40, 565);
      border-radius: 0 0 $border-radius $border-radius;
    }
  }

  .right{
    display: flex;
    flex-wrap: wrap;
    // width: percent(325, $content-width - 48);
    width: 48%;

    .charts-wrap{
      box-sizing: border-box;
      width: 100%;
      height: percent(215, $content-height - 100);
      padding: 10px;
      background: #28272E;
      border-radius: $border-radius;

      &:last-of-type{
        align-self: flex-end;
      }
    }
  }
}

.el-col-12{
  width:100%
}
</style>
