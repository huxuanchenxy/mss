<template>
  <div class="height-full" v-loading="loading"
      element-loading-text="加载中"
      element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <!-- <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }} -->
      </h2>
    </div>
    <!-- 搜索 -->
    <div class="middle">
      <div class="middle-content-wrap height-full" :class="{ active: searchHideMore }" :style="{ height: searchHideMoreHeight }">
        <div class="content con-padding-horizontal">
          <div class="top-input-group" ref="middleTopInput">
            <div class="list">
              <span class="lable">时间</span>
              <el-date-picker
                v-model="time.text"
                type="daterange"
                prefix-icon="el-icon-date"
                :unlink-panels="true"
                value-format="yyyy-MM-dd"
                range-separator="至"
                start-placeholder="计划开始日期"
                end-placeholder="计划结束日期">
              </el-date-picker>
            </div>
            <div class="list" >
              <span class="lable">子系统</span>
              <el-select v-model="subSystem" multiple collapse-tags clearable filterable placeholder="请选择">
                <el-option
                  v-for="item in subSystemList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
            <div class="list" >
              <span class="lable">设备类型</span>
              <el-select v-model="eqpType" multiple collapse-tags  clearable filterable placeholder="请选择">
                <el-option
                  v-for="item in eqpTypeList"
                  :key="item.key"
                  :label="item.tName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
            <div class="list">
              <span class="lable">日/月统计</span>
              <el-radio-group v-model="dateType">
                <el-radio :label=0>日</el-radio>
                <el-radio :label=1>月</el-radio>
              </el-radio-group>
            </div>
            <div class="search-btn text-center">
              <x-button class="active" @click.native="searchResult()"><i class="iconfont icon-search"></i> 查询</x-button>
            </div>
          </div>
          <div class="more-btn" @click="editHideMoreHeight">
            <div class="text">
              {{ shrinkText }}<i class="el-icon-caret-bottom" :class="{ active: searchHideMore }"></i>
            </div>
          </div>

          <!-- 隐藏部分 -->
          <div class="hide-more" v-show="searchHideMore">
            <div class="select-wrap">
              <span class="lable">安装位置</span>
                <el-cascader clearable
                  change-on-select
                  :props="areaParams"
                  :show-all-levels="true"
                  :options="areaList"
                  v-model="area">
                </el-cascader>
            </div>
            <div class="select-wrap">
              <span class="lable">负责班组</span>
               <el-cascader class="cascader_width" clearable
                    :props="defaultParams"
                    change-on-select
                    @change="cascader_change_copy"
                    :show-all-levels="true"
                    :options="teamList"
                    v-model="teamPath.text">
                  </el-cascader>
            </div>
            <div class="select-wrap">
              <span class="lable">供应商</span>
              <el-select v-model="supplier" multiple collapse-tags  clearable filterable placeholder="请选择">
                <el-option
                  v-for="item in supplierList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
            <div class="select-wrap">
              <span class="lable">制造商</span>
              <el-select v-model="manufacturer" multiple collapse-tags  clearable filterable placeholder="请选择">
                <el-option
                  v-for="item in manufacturerList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="scroll">
      <el-container style="height:100%;">
        <el-main style="padding:0px">
          <el-row>
            <el-col :span="12"><div style="width:100%; height:300px;" ref="countChart" id="countChart" v-resize="onResize"></div></el-col>
            <el-col :span="12"><div style="width:100%; height:300px;" ref="avgTimeChart" id="avgTimeChart" v-resize="onResize"></div></el-col>
          </el-row>
          <el-row v-show="showEqpTypeChart">
            <el-col :span="12"><div style="width:100%; height:300px;" ref="countChartByEqpType" id="countChartByEqpType" v-resize="onResize"></div></el-col>
            <el-col :span="12"><div style="width:100%; height:300px;" ref="avgTimeChartByEqpType" id="avgTimeChartByEqpType" v-resize="onResize"></div></el-col>
          </el-row>
          <el-row v-show="showSupplierChart">
            <el-col :span="12"><div style="width:100%; height:300px;" ref="countChartBySupplier" id="countChartBySupplier" v-resize="onResize"></div></el-col>
            <el-col :span="12"><div style="width:100%; height:300px;" ref="avgTimeChartBySupplier" id="avgTimeChartBySupplier" v-resize="onResize"></div></el-col>
          </el-row>
          <el-row v-show="showManufacturerChart">
            <el-col :span="12"><div style="width:100%; height:300px;" ref="countChartByManufacturer" id="countChartByManufacturer" v-resize="onResize"></div></el-col>
            <el-col :span="12"><div style="width:100%; height:300px;" ref="avgTimeChartByManufacturer" id="avgTimeChartByManufacturer" v-resize="onResize"></div></el-col>
          </el-row>
          <el-row v-show="showSubSystemChart">
            <el-col :span="12"><div style="width:100%; height:300px;" ref="countChartBySubSystem" id="countChartBySubSystem" v-resize="onResize"></div></el-col>
            <el-col :span="12"><div style="width:100%; height:300px;" ref="avgTimeChartBySubSystem" id="avgTimeChartBySubSystem" v-resize="onResize"></div></el-col>
          </el-row>
          <el-row v-show="showLocationChart">
            <el-col :span="12"><div style="width:100%; height:300px;" ref="countChartByLocation" id="countChartByLocation" v-resize="onResize"></div></el-col>
            <el-col :span="12"><div style="width:100%; height:300px;" ref="avgTimeChartByLocation" id="avgTimeChartByLocation" v-resize="onResize"></div></el-col>
          </el-row>
        </el-main>
      </el-container>
    </div>
  </div>
</template>
<script>
import XButton from '@/components/button'
import { dictionary, firmType } from '@/common/js/dictionary.js'
import { ApiRESULT } from '@/common/js/utils.js'
import api from '@/api/statisticsApi'
import mychart from './chart'
import apiAuth from '@/api/authApi'
import apiEqp from '@/api/eqpApi'
import apiArea from '@/api/AreaApi.js'
import resize from 'vue-resize-directive'
import apiOrg from '@/api/orgApi'

export default {
  name: 'InspectionManagementList',
  components: {
    XButton
  },
  directives: {
    resize
  },
  data () {
    return {
      title: '| 报警统计',
      subSystem: [],
      subSystemList: [],
      eqpType: [],
      time: {
        text: '',
        tips: ''
      },
      dateType: 0,
      groups: {
        sub_system_id: {
          modelID: 'subSystemID',
          modelName: 'subSystemName',
          reqParamKey: 'SubSystemIDs'
        },
        eqp_type_id: {
          modelID: 'eqpTypeID',
          modelName: 'eqpTypeName',
          reqParamKey: 'EqpTypeIDs'
        },
        manufacturer_id: {
          modelID: 'manufacturerID',
          modelName: 'manufacturerName',
          reqParamKey: 'ManufacturerIDs'
        },
        team_id: {
          modelID: 'teamID',
          modelName: 'teamName',
          reqParamKey: 'TeamIDs'
        }
      },
      groupby: ['sub_system_id', 'eqp_type_id', 'manufacturer_id', 'team_id'],
      groupidxForCount: 0,
      groupidxForAvg: 0,
      resultCountHistory: [], // 穿透历史，返回时不需再次查询数据库
      resultAvgHistory: [],
      subTitleCount: [],
      subTitleAvg: [],

      dateChartCount: null,
      dateChartAvg: null,
      eqpTypeChartCount: null,
      eqpTypeChartAvg: null,
      supplierChartCount: null,
      supplierChartAvg: null,
      manufacturerChartCount: null,
      manufacturerChartAvg: null,
      subSystemChartCount: null,
      subSystemChartAvg: null,
      locationChartCount: null,
      locationChartAvg: null,

      showEqpTypeChart: false,
      showSupplierChart: false,
      showManufacturerChart: false,
      showSubSystemChart: false,
      showLocationChart: false,

      teamPath: {
        text: [],
        tips: ''
      },
      taskStatus: '',
      tunnel: '',
      partition: '',
      eqp: [],
      period: '',
      toPerson: '',
      statusList: [],
      tunnelList: [],
      partitionList: [],
      eqpTypeList: [],
      eqpList: [],
      periodList: [],
      userList: [],
      area: [],
      areaList: [],
      areaParams: {
        label: 'areaName',
        value: 'id',
        children: 'children'
      },
      supplier: [],
      supplierList: [],
      manufacturer: [],
      manufacturerList: [],
      team: '',
      teamList: [],
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      parDisable: true,
      eqpTypeDisable: true,
      eqpDisable: true,

      searchHideMore: false,
      searchHideMoreHeight: '100%',
      shrinkText: '更多',

      checkedID: [],
      condition: ['', '', '', '', '', ''],
      dataList: [],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'PMTaskOrderID',
        order: 'asc'
      },
      headOrder: {
        PMTaskOrderID: 1,
        Status: 0,
        IsInvalid: 0,
        TunnelName: 0,
        PartionName: 0,
        EqpTypeName: 0,
        EqpName: 0,
        PeriodID: 0
      }, // 默认UserID升序asc，箭头朝上，同时只可能按照一个字段排序，不排序的字段不出现箭头,0不排序、1升序、2降序，切换时默认升序
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      }
    }
  },
  created () {
    this.initSelect()
    this.searchResult()
    // this.init()
  },
  activated () {
    // this.searchResult(this.currentPage)
    // this.drawChart()
  },
  methods: {
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
    },
    onResize0 () {},
    onResize1 () {},
    onResize2 () {},
    onResize3 () {},
    onResize4 () {},
    onResize5 () {},
    onResize6 () {},
    onResize7 () {},
    onResize8 () {},
    showStatus (status) {
      var des = '--'
      switch (status) {
        case 'NotStart':
          des = '未开始'
          break
        case 'Starting':
          des = '未完成'
          break
        case 'Finished':
          des = '已完成'
          break
      }
      return des
    },
    closeCondition (index) {
      switch (index) {
        case 0:
          this.tunnel = {TunnelID: ''}
          this.condition.splice(0, 4, '', '', '', '')
          this.choseTunnel(this.tunnel.TunnelID)
          break
        case 1:
          this.partition = {PartitionID: ''}
          this.condition.splice(1, 3, '', '', '')
          // this.choseTunnel(this.tunnel.TunnelID)
          this.chosePartition(this.partition.PartitionID)
          break
        case 2:
          this.eqpType = {eqpTypeID: ''}
          this.condition.splice(2, 2, '', '')
          // this.chosePartition(this.partition.PartitionID)
          this.choseEqpType(this.eqpType.eqpTypeID)
          break
        case 3:
          this.eqp = {eqpID: ''}
          this.condition.splice(3, 1, '')
          // this.choseEqpType(this.eqpType.eqpTypeID)
          break
        case 4:
          this.period = {PeriodID: ''}
          this.condition.splice(4, 1, '')
          break
        case 5:
          this.toPerson = {UserID: ''}
          this.condition.splice(5, 1, '')
          break
      }
    },

    // 展开收起查询条件
    editHideMoreHeight () {
      if (this.searchHideMore) {
        this.searchHideMoreHeight = '100%'
        this.shrinkText = '更多'
      } else {
        this.searchHideMoreHeight = `${document.querySelector('#main').offsetHeight - this.$refs.header.offsetHeight}px`
        this.shrinkText = '收起'
      }
      this.searchHideMore = !this.searchHideMore
    },
    backCount () {
      if (this.groupidxForCount - 1 >= 0) {
        this.groupidxForCount--
        this.resultCountHistory.pop()
        this.subTitleCount.pop()
        let result = this.resultCountHistory[this.resultCountHistory.length - 1]
        this.drawCountChart(null, result.data, false)
      }
    },
    goCount (params) {
      if (params.componentType === 'series') {
        if ((this.groupidxForCount + 1) < this.groupby.length) {
          this.subTitleCount.push(params.seriesName)
          let param = JSON.parse(JSON.stringify(this.resultCountHistory[this.groupidxForCount].param))
          let paramkey = this.groups[this.groupby[this.groupidxForCount]].reqParamKey
          param[paramkey] = params.seriesId
          this.groupidxForCount++
          param.groupby = this.groupby.slice(0, this.groupidxForCount + 1).join(',')
          this.search(param, [this.drawCountChart])
        }
      }
    },
    backAvg () {
      if (this.groupidxForAvg - 1 >= 0) {
        this.groupidxForAvg--
        this.resultAvgHistory.pop()
        this.subTitleAvg.pop()
        let result = this.resultAvgHistory[this.resultAvgHistory.length - 1]
        this.drawAvgChart(null, result.data, false)
      }
    },
    goAvg (params) {
      if (params.componentType === 'series') {
        if ((this.groupidxForAvg + 1) < this.groupby.length) {
          this.subTitleAvg.push(params.seriesName)
          let param = JSON.parse(JSON.stringify(this.resultAvgHistory[this.groupidxForAvg].param))
          let paramkey = this.groups[this.groupby[this.groupidxForAvg]].reqParamKey
          param[paramkey] = params.seriesId
          this.groupidxForAvg++
          param.groupby = this.groupby.slice(0, this.groupidxForAvg + 1).join(',')
          this.search(param, [this.drawAvgChart])
        }
      }
    },
    drawCountChart (param, data, store) {
      if (store) {
        this.resultCountHistory.push({param: param, data: data})
      }

      this.dateChartCount = this.$echarts.init(this.$refs.countChart)
      this.dateChartCount.clear()
      this.dateChartCount.on('click', this.goCount)
      mychart.optionCount.toolbox.feature.myTool.onclick = this.backCount
      mychart.optionCount.title.subtext = this.subTitleCount.join('->')
      let groupModel = this.groups[this.groupby[this.groupidxForCount]]
      mychart.prepareChartData(data, groupModel)
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
      mychart.prepareChartData(data, groupModel)
      this.dateChartAvg.clear()
      this.dateChartAvg.setOption(mychart.optionAvg)
    },

    drawEqpTypeChart (data) {
      this.showEqpTypeChart = true
      this.eqpTypeChartCount = this.$echarts.init(this.$refs.countChartByEqpType)
      this.eqpTypeChartAvg = this.$echarts.init(this.$refs.avgTimeChartByEqpType)
      this.eqpTypeChartCount.clear()
      this.eqpTypeChartAvg.clear()
      let chartData = mychart.prepareSubChartData(data, 'eqpTypeName')
      mychart.optionEqpTypeCount.xAxis[0].data = chartData.xAxisData
      mychart.optionEqpTypeCount.series = chartData.seariescount
      this.eqpTypeChartCount.setOption(mychart.optionEqpTypeCount)

      mychart.optionEqpTypeAvg.xAxis[0].data = chartData.xAxisData
      mychart.optionEqpTypeAvg.series = chartData.seariesavg
      this.eqpTypeChartAvg.setOption(mychart.optionEqpTypeAvg)
    },

    drawSupplierChart (data) {
      this.showSupplierChart = true
      this.supplierChartCount = this.$echarts.init(this.$refs.countChartBySupplier)
      this.supplierChartAvg = this.$echarts.init(this.$refs.avgTimeChartBySupplier)
      this.supplierChartCount.clear()
      this.supplierChartAvg.clear()
      let chartData = mychart.prepareSubChartData(data, 'supplierName')
      mychart.optionSupplierCount.xAxis[0].data = chartData.xAxisData
      mychart.optionSupplierCount.series = chartData.seariescount
      this.supplierChartCount.setOption(mychart.optionSupplierCount)

      mychart.optionSupplierAvg.xAxis[0].data = chartData.xAxisData
      mychart.optionSupplierAvg.series = chartData.seariesavg
      this.supplierChartAvg.setOption(mychart.optionSupplierAvg)
    },

    drawManufacturerChart (data) {
      this.showManufacturerChart = true
      this.manufacturerChartCount = this.$echarts.init(this.$refs.countChartByManufacturer)
      this.manufacturerChartAvg = this.$echarts.init(this.$refs.avgTimeChartByManufacturer)
      this.manufacturerChartCount.clear()
      this.manufacturerChartAvg.clear()
      let chartData = mychart.prepareSubChartData(data, 'manufacturerName')
      mychart.optionManufacturerCount.xAxis[0].data = chartData.xAxisData
      mychart.optionManufacturerCount.series = chartData.seariescount
      this.manufacturerChartCount.setOption(mychart.optionManufacturerCount)

      mychart.optionManufacturerAvg.xAxis[0].data = chartData.xAxisData
      mychart.optionManufacturerAvg.series = chartData.seariesavg
      this.manufacturerChartAvg.setOption(mychart.optionManufacturerAvg)
    },

    drawSubSystemChart (data) {
      this.showSubSystemChart = true
      this.subSystemChartCount = this.$echarts.init(this.$refs.countChartBySubSystem)
      this.subSystemChartAvg = this.$echarts.init(this.$refs.avgTimeChartBySubSystem)
      this.subSystemChartCount.clear()
      this.subSystemChartAvg.clear()
      let chartData = mychart.prepareSubChartData(data, 'subSystemName')
      mychart.optionSubSystemCount.xAxis[0].data = chartData.xAxisData
      mychart.optionSubSystemCount.series = chartData.seariescount
      this.subSystemChartCount.setOption(mychart.optionSubSystemCount)

      mychart.optionSubSystemAvg.xAxis[0].data = chartData.xAxisData
      mychart.optionSubSystemAvg.series = chartData.seariesavg
      this.subSystemChartAvg.setOption(mychart.optionSubSystemAvg)
    },
    drawLocationChart (data) {
      this.showLocationChart = true
      this.locationChartCount = this.$echarts.init(this.$refs.countChartByLocation)
      this.locationChartAvg = this.$echarts.init(this.$refs.avgTimeChartByLocation)
      this.locationChartCount.clear()
      this.locationChartAvg.clear()
      let chartData = mychart.prepareSubChartData(data.data, data.groupby)
      mychart.optionLocationCount.xAxis[0].data = chartData.xAxisData
      mychart.optionLocationCount.series = chartData.seariescount
      this.locationChartCount.setOption(mychart.optionLocationCount)

      mychart.optionLocationAvg.xAxis[0].data = chartData.xAxisData
      mychart.optionLocationAvg.series = chartData.seariesavg
      this.locationChartAvg.setOption(mychart.optionLocationAvg)
    },
    choseTunnel (tunnelID) {
      this.condition.splice(0, 4, '', '', '', '')
      this.condition.splice(5, 1, '')
      this.getUserList(tunnelID)
      // if (tunnelID === '') {
      //   this.parDisable = true
      //   this.eqpTypeDisable = true
      //   this.eqpDisable = true
      //   this.partitionList = []
      //   this.partition = {PartitionID: ''}
      //   this.eqpType = {eqpTypeID: ''}
      //   this.eqpTypeList = []
      //   this.eqp = {eqpID: ''}
      //   this.eqpList = []
      //   return
      // }
      if (tunnelID !== '') {
        this.condition[0] = ('管廊：' + this.tunnel.TunnelName)
      }

      this.parDisable = false
      this.eqpTypeDisable = true
      this.eqpDisable = true
      this.partitionList = []
      this.partition = {} // {PartitionID: ''}
      // this.eqpType = {eqpTypeID: ''}
      // this.eqpTypeList = []
      this.eqp = {} // {eqpID: ''}
      this.eqpList = []
      // 获取分区
      window.axios.post('/UtilityTunnel/GetTunnelIDPartition', {TunnelID: tunnelID}).then(res => {
        this.partitionList = res.data
      }).catch(err => console.log(err))
    },
    chosePartition (partitionID) {
      if (this.partition.PartitionID !== '') {
        this.condition[1] = ('分区：' + this.partition.PartionName)
        this.condition.splice(2, 2, '', '')
      } else {
        this.condition.splice(1, 3, '', '', '')
      }
      this.parDisable = false
      this.eqpTypeDisable = false
      this.eqpDisable = true
      this.eqpType = {} // {eqpTypeID: ''}
      this.eqpTypeList = []
      this.eqp = {} // {eqpID: ''}
      this.eqpList = []
    },
    choseEqpType (eqpTypeID) {
      if (this.eqpType.eqpTypeID !== '') {
        this.condition[2] = ('设备类型：' + this.eqpType.eqpTypeName)
        this.condition.splice(3, 1, '')
      } else {
        this.condition.splice(2, 2, '', '')
      }
      this.parDisable = false
      this.eqpTypeDisable = false
      this.eqpDisable = false
      this.eqp = {} // {eqpID: ''}
      this.eqpList = []
      // 获取设备
      window.axios.post('/UtilityTunnel/GetEquipmentByType',
        {tunnelID: this.tunnel.TunnelID, partitionID: this.partition.PartitionID, eqpTypeID: eqpTypeID}).then(res => {
        this.eqpList = res.data
      }).catch(err => console.log(err))
    },
    choseEqp () {
      if (this.eqp.eqpID !== '') {
        this.condition[3] = ('设备：' + this.eqp.eqpName)
      } else {
        this.condition.splice(3, 1, '')
      }
    },
    chosePeriod () {
      if (this.period.PeriodID !== '') {
        this.condition[4] = ('周期：' + this.period.PeriodName)
      } else {
        this.condition.splice(4, 1, '')
      }
    },
    choseUser () {
      if (this.toPerson.UserID !== '') {
        this.condition[5] = ('巡检人员：' + this.toPerson.UserName)
      } else {
        this.condition.splice(5, 1, '')
      }
    },
    getUserList (tunnelID) {
      // 获取巡检人员
      window.axios.post('/UserInfo/GetUserByTunnelID', {tunnelID: tunnelID}).then(res => {
        this.userList = res.data
        this.toPerson = {UserID: ''}
      }).catch(err => console.log(err))
    },
    initSelect () {
      // 子系统加载
      apiAuth.getSubCode(dictionary.subSystem).then(res => {
        this.subSystemList = res.data
      }).catch(err => console.log(err))

      // 设备类型加载
      apiEqp.getEqpTypeAll().then(res => {
        this.eqpTypeList = res.data
      }).catch(err => console.log(err))

      // 安装位置加载
      apiArea.SelectConfigAreaData().then(res => {
        this.areaList = res.data.dicAreaList
      }).catch(err => console.log(err))

      // 班组加载
      apiOrg.getOrgAll().then(res => {
        this.teamList = res.data
      }).catch(err => console.log(err))

      // 供应商/制造商加载
      apiEqp.getFirmAll().then(res => {
        this.supplierList = res.data.filter((item) => { return item.type === firmType.supplier })
        this.manufacturerList = res.data.filter((item) => { return item.type === firmType.manufacturer })
      }).catch(err => console.log(err))
    },

    init () {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = 1
      // this.searchResult()
    },
    // 改变排序
    changeOrder (sort) {
      // console.log(this.headOrder[sort])
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.PMTaskOrderID = 0
        this.headOrder.Status = 0
        this.headOrder.IsInvalid = 0
        this.headOrder.TunnelName = 0
        this.headOrder.PartionName = 0
        this.headOrder.EqpTypeName = 0
        this.headOrder.EqpName = 0
        this.headOrder.PeriodID = 0
        this.currentSort.order = 'asc'
        this.headOrder[sort] = 1
      } else if (this.headOrder[sort] === 2) { // 同一字段降序变升序
        this.currentSort.order = 'asc'
        this.headOrder[sort] = 1
      } else { // 同一字段升序变降序
        this.currentSort.order = 'desc'
        this.headOrder[sort] = 2
      }
      this.currentSort.sort = sort
      this.searchResult(this.currentPage)
    },

    search (param, callbacks) {
      this.loading = true
      api.reportAlarm(param).then(res => {
        this.loading = false
        if (res.code === ApiRESULT.Success) {
          for (let callback of callbacks) {
            callback(param, res.data, true)
          }
        }
      }).catch(err => console.log(err))
    },
    // 搜索
    searchResult () {
      // console.log(this.subSystem)
      var sTime = ''
      var eTime = ''
      if (this.time.text) {
        sTime = this.time.text[0]
        eTime = this.time.text[1] + ' 23:59:59'
      }
      var param = {
        SubSystemIDs: this.subSystem.join(','),
        EqpTypeIDs: this.eqpType.join(','),
        LocationPath: this.area.join(','),
        SupplierIDs: this.supplier.join(','),
        ManufacturerIDs: this.manufacturer.join(','),
        OrgPath: this.teamPath.text.join(','),
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
      this.search(param, [this.drawCountChart, this.drawAvgChart])

      if (param.EqpTypeIDs) {
        api.reportSubChartEqpType(param).then(res => {
          if (res.code === ApiRESULT.Success && res.data.length > 0) {
            this.drawEqpTypeChart(res.data)
          } else {
            this.showEqpTypeChart = false
          }
        }).catch(err => console.log(err))
      } else {
        this.showEqpTypeChart = false
      }

      if (param.SupplierIDs) {
        api.reportSubChartSupplier(param).then(res => {
          if (res.code === ApiRESULT.Success && res.data.length > 0) {
            this.drawSupplierChart(res.data)
          } else {
            this.showSupplierChart = false
          }
        }).catch(err => console.log(err))
      } else {
        this.showSupplierChart = false
      }

      if (param.ManufacturerIDs) {
        api.reportSubChartManufacturer(param).then(res => {
          if (res.code === ApiRESULT.Success && res.data.length > 0) {
            this.drawManufacturerChart(res.data)
          } else {
            this.showManufacturerChart = false
          }
        }).catch(err => console.log(err))
      } else {
        this.showManufacturerChart = false
      }

      if (param.SubSystemIDs) {
        api.reportSubChartSubSystem(param).then(res => {
          if (res.code === ApiRESULT.Success && res.data.length > 0) {
            this.drawSubSystemChart(res.data)
          } else {
            this.showSubSystemChart = false
          }
        }).catch(err => console.log(err))
      } else {
        this.showSubSystemChart = false
      }

      if (param.LocationPath) {
        api.reportSubChartLocation(param).then(res => {
          if (res.code === ApiRESULT.Success && res.data.data.length > 0) {
            this.drawLocationChart(res.data)
          } else {
            this.showLocationChart = false
          }
        }).catch(err => console.log(err))
      } else {
        this.showLocationChart = false
      }
      if (param.OrgPath) {
        api.reportSubChartOrg(param).then(res => {
          if (res.code === ApiRESULT.Success && res.data.data.length > 0) {
            // this.drawLocationChart(res.data)
          } else {
            // this.showLocationChart = false
          }
        }).catch(err => console.log(err))
      } else {
        // this.showLocationChart = false
      }
    },
    // 修改
    edit () {
      if (!this.checkedID.length) {
        this.$message({
          message: '请选择需要操作的任务单',
          duration: 2000,
          type: 'warning'
        })
      } else if (this.checkedID.length > 1) {
        this.$message({
          message: '选择的任务单不能超过1个',
          duration: 2000
        })
      } else {
        var curItem
        for (var i = 0; i < this.dataList.length; ++i) {
          if (this.dataList[i].PMTaskOrderID === this.checkedID[0]) {
            curItem = this.dataList[i]
            break
          }
        }
        if (curItem.Invalid === 1) {
          this.$message({
            message: '此任务单已作废',
            type: 'warning'
          })
          return
        }
        this.$router.push({name: 'InspectionManagementEditDetail', query: { TaskOrderID: this.checkedID[0] }})
      }
    },

    // 查看
    see () {
      if (!this.checkedID.length) {
        this.$message({
          message: '请选择需要操作的任务单',
          duration: 2000,
          type: 'warning'
        })
      } else if (this.checkedID.length > 1) {
        this.$message({
          message: '选择的任务单不能超过1个',
          duration: 2000
        })
      } else {
        this.$router.push({name: 'InspectionManagementSeeDetail', query: { TaskOrderID: this.checkedID[0], prePage: 'InspectionManagementList' }})
      }
    },

    del () {
      if (!this.checkedID.length) {
        this.$message({
          message: '请选择要删除的任务单',
          type: 'warning'
        })
      } else {
        for (var i = 0; i < this.checkedID.length; ++i) {
          for (var j = 0; j < this.dataList.length; ++j) {
            if (this.dataList[j].PMTaskOrderID === this.checkedID[i]) {
              let curItem = this.dataList[j]
              if (curItem.Status === 'Starting') {
                this.$message({
                  message: '不能删除未结束的任务单',
                  type: 'warning'
                })
                return
              }
              if (curItem.Invalid === 1) {
                this.$message({
                  message: '不能删除已作废的任务单',
                  type: 'warning'
                })
                return
              }
            }
          }
        }

        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条巡检任务?'
      }
    },
    // 弹框确认是否删除
    dialogEnter () {
      // 隐藏dialog
      this.dialogVisible.isShow = false
      window.axios.post('/PMTaskOrder/DelOrder', {
        SelectedIDs: this.checkedID.join(',')
      }).then(res => {
        if (res.data.result === 'OK') {
          // this.init()
          this.searchResult(this.currentPage)
          this.$message({
            message: '删除成功',
            type: 'success'
          })
        } else {
          this.$message({
            message: '删除失败',
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    // 全选
    checkAll () {
      this.bCheckAll ? this.dataList.map(val => this.checkedID.push(val.PMTaskOrderID)) : this.checkedID = []
    },

    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 上一页
    prevPage (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 下一页
    nextPage (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 班组下拉选中，过滤非班组
    cascader_change_copy (val) {
      var arr = this.teamPath.text
      let id = arr[arr.length - 1]
      this.teamGroupid = id
    }
  }
}
</script>
<style lang="scss" scoped>
.middle{
  position: relative;
  height: 12% !important;

  .lable{
    margin-right: 10px;
  }

  .middle-content-wrap{
    box-sizing: border-box;
    position: absolute;
    z-index: 9;
    top: 0;
    left: 0;
    width: 100%;

    .content{
      // min-height: 100%;
      height: 72px;
      background: #313035;
    }

    &.active{
      background: rgba(0,0,0,.7);
      .content{
        min-height: initial;
        padding-bottom: 10px;
        background: #313035;
        height: 137px;
      }
    }
  }

  // 查看更多按钮
  .more-btn{
    position: relative;
    height: 20px;
    text-align: center;
    cursor: pointer;

    .text{
      position: absolute;
      z-index: 1;
      top: 0;
      left: 50%;
      padding: 0 10px;
      background: #313035;
      transform: translateX(-50%);
    }

    &:after{
      content: "";
      position: absolute;
      top: 50%;
      left: 0;
      width: 100%;
      border-top: 1px dashed #676767;
    }

    .el-icon-caret-bottom{
      display: inline-block;
      color: $color-highlight-text;

      &.active{
        transform: rotateZ(180deg);
      }
    }
  }

  // 时间选择
  .top-input-group{
    display: flex;
    justify-content: space-between;
    padding: 15px 0;
  }

  /deep/ .el-date-editor{
    width: 305px;

    .el-input__icon{
      height: initial;
    }

    .el-range-separator{
      height: initial;
      color: $color-white;
    }

    .el-range-input{
      color: $color-white;
    }
  }

  // 隐藏部分
  .hide-more{
    display: flex;
    align-items: center;
    justify-content: space-between;
    flex-wrap: wrap;

    .select-wrap{
      display: flex;
      align-items: center;
      width: 30%;
      margin-top: 15px;

      &:nth-of-type(3n-1){
        justify-content: center;
      }

      &:nth-of-type(3n){
        justify-content: flex-end;
      }

      &.disable{
        @include opacity(0.5);
      }
    }
  }

  .show-result{
    display: flex;
    justify-content: space-between;
    min-height: 30px;
    margin-top: 10px;

    // 显示已选的查询条件
    .left{
      display: flex;
      width: 90%;

      .list{
        position: relative;
        width: 90px;
        height: 22px;
        margin-right: 20px;
        padding: 0 8px;
        background: rgba(255,255,255,.2);
        border-radius: $border-radius;
      }

      .text{
        display: block;
        width: 100%;
        height: 100%;
        margin: 0;
        @extend %ellipsis;
        background: none;
        border: 0;
        font-size: $font-size-small;
        line-height: 22px;
        text-align: left;
        font-weight: lighter;
        color: $color-white;
      }

      .el-icon-circle-close{
        position: absolute;
        right: -5px;
        top: -5px;
        color: #E04C4C;
        cursor: pointer;
      }
    }
  }
}

// 功能区
.operation{
  display: flex;
  align-items: center;
  height: percent(70, $content-height);
}

.content-header,
.list-con{
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: percent(50, $content-height);
  padding: 0 PXtoEm(24);

  .list{
    width: 10%;
    text-align: center;
    word-break: break-word;
  }

  .checkbox{
    width: 5%;
  }

  .time{
    width: 15%;
  }
}

.content-header{
  background: rgba(36,128,198,.5);
}

.scroll{
  /**
   * percent函数转换百分比
   * $content-height内容区域总高度
   * 页面标题栏高度：56
   * 查询条件高度：140
   * 操作按钮组：70
   * 表头高度：50
   */
  // height: percent($content-height - 56 - 140 - 70 - 50, $content-height);
  height: 82%;

  .list-con{
    height: initial;
    padding: {
      top: PXtoEm(15);
      bottom: PXtoEm(15);
    }
    &:nth-of-type(even){
      background: rgba(186,186,186,.5);
    }

    /deep/ .el-checkbox__label{
      display: none;
    }
  }
}
</style>
<style>
/* .el-select {
  display: inline-flex;
    position: relative;
    width: 79%;
} */
.el-select__tags {
  flex-wrap: nowrap;
  overflow: hidden;
  max-width: 250px !important;
}
.el-input__inner {
  height: 30px !important;
}
.el-date-editor {
  width:253px !important;
}
.select-wrap {
  width:unset !important;
}

</style>
