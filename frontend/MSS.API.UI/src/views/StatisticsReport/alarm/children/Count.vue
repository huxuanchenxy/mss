<template>
  <div class="height-full" v-loading="loading"
      element-loading-text="加载中"
      element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <title-module></title-module>
    </div>
    <!-- 搜索 -->
    <div class="middle">
      <div class="middle-content-wrap height-full" :class="{ active: searchHideMore }" :style="{ height: searchHideMoreHeight }">
        <div class="content con-padding-horizontal">
          <div class="top-input-group" ref="middleTopInput">
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
            <el-col :span="12" id="countChart" v-resize="onResize" v-loading="loading_count"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <!-- <div style="width:100%; height:300px;" ref="countChart" id="countChart" class="echart" v-resize="onResize"></div> -->
              <div style="width:100%; height:300px;" ref="countChart"  class="echart"></div>
              <div class="echartsubtitle">{{bottomDesForCount}}</div>
            </el-col>
            <el-col :span="12" id="avgTimeChart" v-resize="onResize" v-loading="loading_avg"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="avgTimeChart"  class="echart"></div>
              <div class="echartsubtitle">{{bottomDesForAvg}}</div>
              </el-col>
          </el-row>
          <el-row v-show="showEqpTypeChart">
            <el-col :span="12" id="countChartByEqpType" v-resize="onResize" v-loading="loading_countEqpType"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="countChartByEqpType"  class="echart" ></div>
            </el-col>
            <el-col :span="12" id="avgTimeChartByEqpType" v-resize="onResize" v-loading="loading_avgEqpType"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="avgTimeChartByEqpType"  class="echart" ></div>
            </el-col>
          </el-row>
          <el-row v-show="showSupplierChart">
            <el-col :span="12" id="countChartBySupplier" v-resize="onResize" v-loading="loading_countSupplier"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="countChartBySupplier"  class="echart" ></div>
            </el-col>
            <el-col :span="12" id="avgTimeChartBySupplier" v-resize="onResize" v-loading="loading_avgSupplier"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="avgTimeChartBySupplier"  class="echart" ></div>
            </el-col>
          </el-row>
          <el-row v-show="showManufacturerChart">
            <el-col :span="12" id="countChartByManufacturer" v-resize="onResize" v-loading="loading_countManufacturer"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="countChartByManufacturer"  class="echart" ></div>
            </el-col>
            <el-col :span="12" id="avgTimeChartByManufacturer" v-resize="onResize" v-loading="loading_avgManufacturer"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="avgTimeChartByManufacturer"  class="echart" ></div>
            </el-col>
          </el-row>
          <el-row v-show="showSubSystemChart">
            <el-col :span="12" id="countChartBySubSystem" v-resize="onResize" v-loading="loading_countSubSystem"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="countChartBySubSystem"  class="echart" ></div>
            </el-col>
            <el-col :span="12" id="avgTimeChartBySubSystem" v-resize="onResize" v-loading="loading_avgSubSystem"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="avgTimeChartBySubSystem"  class="echart" ></div>
            </el-col>
          </el-row>
          <el-row v-show="showLocationChart">
            <el-col :span="12" id="countChartByLocation" v-resize="onResize" v-loading="loading_countLocation"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="countChartByLocation" class="echart"></div>
            </el-col>
            <el-col :span="12" id="avgTimeChartByLocation" v-resize="onResize" v-loading="loading_avgLocation"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="avgTimeChartByLocation"  class="echart" ></div>
            </el-col>
          </el-row>
          <el-row v-show="showOrgChart">
            <el-col :span="12" id="countChartByOrg" v-resize="onResize" v-loading="loading_countOrg"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="countChartByOrg"  class="echart" ></div>
            </el-col>
            <el-col :span="12" id="avgTimeChartByOrg" v-resize="onResize" v-loading="loading_avgOrg"
                element-loading-text="加载中"
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.7)">
              <div style="width:100%; height:300px;" ref="avgTimeChartByOrg"  class="echart" ></div>
            </el-col>
          </el-row>
        </el-main>
      </el-container>
    </div>
  </div>
</template>
<script>
import XButton from '@/components/button'
import { dictionary, firmType } from '@/common/js/dictionary.js'
import { getNowFormatDate, ApiRESULT } from '@/common/js/utils.js'
import api from '@/api/statisticsApi'
import mychart from './chart'
import apiAuth from '@/api/authApi'
import apiEqp from '@/api/eqpApi'
import apiArea from '@/api/AreaApi.js'
import resize from 'vue-resize-directive'
import apiOrg from '@/api/orgApi'
import TitleModule from '@/components/TitleModule'
export default {
  name: 'InspectionManagementList',
  components: {
    XButton,
    'title-module': TitleModule
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
      orgChartCount: null,
      orgChartAvg: null,

      showEqpTypeChart: false,
      showSupplierChart: false,
      showManufacturerChart: false,
      showSubSystemChart: false,
      showLocationChart: false,
      showOrgChart: false,

      loading_count: false,
      loading_avg: false,
      loading_countEqpType: false,
      loading_avgEqpType: false,
      loading_countSupplier: false,
      loading_avgSupplier: false,
      loading_countManufacturer: false,
      loading_avgManufacturer: false,
      loading_countSubSystem: false,
      loading_avgSubSystem: false,
      loading_countLocation: false,
      loading_avgLocation: false,
      loading_countOrg: false,
      loading_avgOrg: false,

      teamPath: {
        text: [],
        tips: ''
      },

      eqpTypeList: [],
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

      searchHideMore: false,
      searchHideMoreHeight: '100%',
      shrinkText: '更多',

      bottomDesForCount: '',
      bottomDesForAvg: '',

      loading: false
    }
  },
  created () {
    this.initSelect()
    // this.init()
    var nowDate = getNowFormatDate()
    var d = new Date()
    d.setDate(d.getDate() - 70)
    var startDate = getNowFormatDate(d)
    this.time.text = [startDate, nowDate]
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
      if (this.orgChartCount && el.id === 'countChartByOrg') {
        this.orgChartCount.resize()
      }
      if (this.orgChartAvg && el.id === 'avgTimeChartByOrg') {
        this.orgChartAvg.resize()
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
    magicTypeChanged (param, param1) {
      if (param.currentType === 'tiled' && param.newOption.series.length > 8) {

      }
    },
    drawCountChart (param, data, store) {
      if (store) {
        this.resultCountHistory.push({param: param, data: data})
      }

      this.dateChartCount = this.$echarts.init(this.$refs.countChart)
      this.dateChartCount.clear()
      this.dateChartCount.on('click', this.goCount)
      this.dateChartCount.on('magicTypeChanged', this.magicTypeChanged)
      mychart.optionCount.toolbox.feature.myTool.onclick = this.backCount
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

    drawEqpTypeChart (data) {
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
    drawOrgChart (data) {
      this.orgChartCount = this.$echarts.init(this.$refs.countChartByOrg)
      this.orgChartAvg = this.$echarts.init(this.$refs.avgTimeChartByOrg)
      this.orgChartCount.clear()
      this.orgChartAvg.clear()
      let chartData = mychart.prepareSubChartData(data.data, data.groupby)
      mychart.optionOrgCount.xAxis[0].data = chartData.xAxisData
      mychart.optionOrgCount.series = chartData.seariescount
      this.orgChartCount.setOption(mychart.optionOrgCount)

      mychart.optionOrgAvg.xAxis[0].data = chartData.xAxisData
      mychart.optionOrgAvg.series = chartData.seariesavg
      this.orgChartAvg.setOption(mychart.optionOrgAvg)
    },
    initSelect () {
      // 子系统加载
      apiAuth.getSubCode(dictionary.subSystem).then(res => {
        this.subSystemList = res.data
        this.searchResult()
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
      api.reportAlarm(param).then(res => {
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
    getSubSystemSelected () {
      let legend = []
      if (this.subSystem.length > 0) {
        for (let i = 0; i < this.subSystem.length; ++i) {
          let name = ''
          for (let j = 0; j < this.subSystemList.length; j++) {
            if (this.subSystem[i] === this.subSystemList[j].id) {
              name = this.subSystemList[j].name
              break
            }
          }
          if (name) {
            legend.push(name)
          }
        }
      } else {
        // for (let i = 0; i < this.subSystemList.length; i++) {
        //   legend.push(this.subSystemList[i].name)
        // }
      }
      this.groups.sub_system_id.legend = legend
    },
    getEqpTypeSelected () {
      let legend = []
      if (this.eqpType.length > 0) {
        for (let i = 0; i < this.eqpType.length; ++i) {
          let name = ''
          for (let j = 0; j < this.eqpTypeList.length; j++) {
            if (this.eqpType[i] === this.eqpTypeList[j].id) {
              name = this.eqpTypeList[j].tName
              break
            }
          }
          if (name) {
            legend.push(name)
          }
        }
      } else {
        for (let i = 0; i < this.eqpTypeList.length; i++) {
          legend.push(this.eqpTypeList[i].tName)
        }
      }
      this.groups.eqp_type_id.legend = legend
    },
    getManufacturerSelected () {
      let legend = []
      if (this.manufacturer.length > 0) {
        for (let i = 0; i < this.manufacturer.length; ++i) {
          let name = ''
          for (let j = 0; j < this.manufacturerList.length; j++) {
            if (this.manufacturer[i] === this.manufacturerList[j].id) {
              name = this.manufacturerList[j].name
              break
            }
          }
          if (name) {
            legend.push(name)
          }
        }
      } else {
        // for (let i = 0; i < this.manufacturerList.length; i++) {
        //   legend.push(this.manufacturerList[i].name)
        // }
      }
      this.groups.manufacturer_id.legend = legend
    },
    // 搜索
    searchResult () {
      // console.log(this.subSystem)
      // this.searchHideMoreHeight = '100%'
      this.shrinkText = '更多'
      this.searchHideMoreHeight = '100%'
      this.searchHideMore = false
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
      // 获取当前子系统类别
      this.getSubSystemSelected()
      // 获取当前设备类型
      this.getEqpTypeSelected()
      // 获取当前制造商
      this.getManufacturerSelected()
      this.search(param, [this.drawCountChart, this.drawAvgChart])

      if (param.EqpTypeIDs) {
        this.showEqpTypeChart = true
        this.loading_countEqpType = true
        this.loading_avgEqpType = true
        api.reportSubChartEqpType(param).then(res => {
          this.loading_countEqpType = false
          this.loading_avgEqpType = false
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
        this.showSupplierChart = true
        this.loading_countSupplier = true
        this.loading_avgSupplier = true
        api.reportSubChartSupplier(param).then(res => {
          this.loading_countSupplier = false
          this.loading_avgSupplier = false
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
        this.showManufacturerChart = true
        this.loading_countManufacturer = true
        this.loading_avgManufacturer = true
        api.reportSubChartManufacturer(param).then(res => {
          this.loading_countManufacturer = false
          this.loading_avgManufacturer = false
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
        this.showSubSystemChart = true
        this.loading_countSubSystem = true
        this.loading_avgSubSystem = true
        api.reportSubChartSubSystem(param).then(res => {
          this.loading_countSubSystem = false
          this.loading_avgSubSystem = false
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
        this.showLocationChart = true
        this.loading_countLocation = true
        this.loading_avgLocation = true
        api.reportSubChartLocation(param).then(res => {
          this.loading_countLocation = false
          this.loading_avgLocation = false
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
        this.showOrgChart = true
        this.loading_countOrg = true
        this.loading_avgOrg = true
        api.reportSubChartOrg(param).then(res => {
          this.loading_countOrg = false
          this.loading_avgOrg = false
          if (res.code === ApiRESULT.Success && res.data.data.length > 0) {
            this.drawOrgChart(res.data)
          } else {
            this.showOrgChart = false
          }
        }).catch(err => console.log(err))
      } else {
        this.showOrgChart = false
      }
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
    z-index: 1;
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
#avgTimeChart {
  display: none;
}
#avgTimeChartByEqpType {
  display: none;
}
#avgTimeChartBySupplier {
  display: none;
}
#avgTimeChartByManufacturer {
  display: none;
}
#avgTimeChartBySubSystem {
  display: none;
}
#avgTimeChartByLocation {
  display: none;
}
#avgTimeChartByOrg {
  display: none;
}
.el-col-12 {
  width:100%;
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

.tableechart {
    /* cellspacing:0 ; */
    border-collapse: collapse; /* IE7 and lower */
    border-spacing: 0;
    width: 100%;
    /* color:black; */
    /* background-color: brown; */
    /* background: rgba(49, 48, 53, 0.5); */
}

div.echart > div:last-child {
  background-color: rgba(49, 48, 53, 1) !important;
}
div.echart > div:last-child > div:last-child > div:first-child {
  display:none;
}
div.echart h4 {
  display:none;
}
.echartsubtitle {
    position: relative;
    top: 0px;
    padding-left: 9%;
    width: 80%;
    text-align: center;
}
</style>
