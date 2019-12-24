<template>
  <div
    class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active">
        <router-link :to="{name:'EntityList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation scroll">
      <el-scrollbar>
      <el-collapse v-model="activeName" accordion @change="handleChange">
        <el-collapse-item title="模板选择" name="1" style="margin-top:10px;padding:0px;">
          <div class="box1">
            <!-- 搜索框 -->
            <div class="con-padding-horizontal search-wrap">
              <div class="wrap">
                <div class="input-group">
                  <label for="name">模板名称</label>
                  <div class="inp">
                    <el-input v-model.trim="pmModule.moduleName" placeholder="请输入模板名称"></el-input>
                  </div>
                </div>
                <div class="input-group">
                  <label for="name">上传文件名</label>
                  <div class="inp">
                    <el-input v-model.trim="pmModule.fileName" placeholder="请输入上传文件名"></el-input>
                  </div>
                </div>
                <div class="input-group">
                  <label for="name">设备设施</label>
                  <div class="inp">
                    <el-input v-model.trim="pmModule.deviceName" placeholder="请输入设备设施"></el-input>
                  </div>
                </div>
                <div class="input-group">
                  <label for="name">关键字</label>
                  <div class="inp">
                    <el-input v-model.trim="pmModule.keyWord" placeholder="请输入关键字"></el-input>
                  </div>
                </div>
              </div>
              <div class="search-btn" @click="searchResModule">
                <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
              </div>
            </div>
          </div>
          <!-- 内容 -->
          <div class="content-wrap">
            <ul class="content-header">
              <li style="margin-left:20px"/>
              <li class="list name c-pointer" @click="changeOrder('code')">
                模板编码
                <i :class="[{ 'el-icon-d-caret': pmModule.headOrder.code === 0 }, { 'el-icon-caret-top': pmModule.headOrder.code === 1 }, { 'el-icon-caret-bottom': pmModule.headOrder.code === 2 }]"></i>
              </li>
              <li class="list name c-pointer" @click="changeOrder('name')">
                模板名称
                <i :class="[{ 'el-icon-d-caret': pmModule.headOrder.name === 0 }, { 'el-icon-caret-top': pmModule.headOrder.name === 1 }, { 'el-icon-caret-bottom': pmModule.headOrder.name === 2 }]"></i>
              </li>
              <li class="list name c-pointer" @click="changeOrder('file_name')">
                上传文件名
                <i :class="[{ 'el-icon-d-caret': pmModule.headOrder.file_name === 0 }, { 'el-icon-caret-top': pmModule.headOrder.file_name === 1 }, { 'el-icon-caret-bottom': pmModule.headOrder.file_name === 2 }]"></i>
              </li>
              <li class="list name c-pointer" @click="changeOrder('device_name')">
                设备设施
                <i :class="[{ 'el-icon-d-caret': pmModule.headOrder.device_name === 0 }, { 'el-icon-caret-top': pmModule.headOrder.device_name === 1 }, { 'el-icon-caret-bottom': pmModule.headOrder.device_name === 2 }]"></i>
              </li>
              <li class="list name">上传部门</li>
              <li class="list name c-pointer" @click="changeOrder('death_year')">
                有效年限
                <i :class="[{ 'el-icon-d-caret': pmModule.headOrder.death_year === 0 }, { 'el-icon-caret-top': pmModule.headOrder.death_year === 1 }, { 'el-icon-caret-bottom': pmModule.headOrder.death_year === 2 }]"></i>
              </li>
              <li class="list name c-pointer" @click="changeOrder('level')">
                受控级别
                <i :class="[{ 'el-icon-d-caret': pmModule.headOrder.level === 0 }, { 'el-icon-caret-top': pmModule.headOrder.level === 1 }, { 'el-icon-caret-bottom': pmModule.headOrder.level === 2 }]"></i>
              </li>
              <li class="list last-update-time c-pointer" @click="changeOrder('created_time')">
                创建时间
                <i :class="[{ 'el-icon-d-caret': pmModule.headOrder.created_time === 0 }, { 'el-icon-caret-top': pmModule.headOrder.created_time === 1 }, { 'el-icon-caret-bottom': pmModule.headOrder.created_time === 2 }]"></i>
              </li>
              <li class="list name c-pointer" @click="changeOrder('created_by')">
                创建人
                <i :class="[{ 'el-icon-d-caret': pmModule.headOrder.created_by === 0 }, { 'el-icon-caret-top': pmModule.headOrder.created_by === 1 }, { 'el-icon-caret-bottom': pmModule.headOrder.created_by === 2 }]"></i>
              </li>
              <!--<li class="name">操作</li>-->
            </ul>
            <div class="scroll">
              <!--<el-scrollbar>-->
                <ul class="list-wrap">
                  <li class="list" v-for="(item) in mList" :key="item.key">
                    <div class="list-content">
                      <div >
                        <input type="radio" v-model="editModuleID" :value="item.id" @change="getModuleByID">
                      </div>
                      <div class="name word-break">{{ item.code }}</div>
                      <div class="name word-break">{{ item.name }}</div>
                      <div class="name word-break">{{ item.fileName }}</div>
                      <div class="name word-break">{{ item.deviceName }}</div>
                      <div class="name word-break">{{ item.department }}</div>
                      <div class="name word-break">{{ item.deathYear }}</div>
                      <div class="name word-break">{{ item.levelName }}</div>
                      <div class="last-update-time color-white">{{ item.createdTime }}</div>
                      <div class="name">{{ item.createdByName }}</div>
                      <!--<div class="name">
                        <li @click="detail(item.id)" ><x-button >查看明细</x-button></li>
                      </div>-->
                    </div>
                  </li>
                </ul>
              <!-- 分页 -->
                <el-pagination
                  :current-page.sync="pmModule.currentPageModule"
                  @current-change="handleCurrentChangeModule"
                  @prev-click="prevPageModule"
                  @next-click="nextPageModule"
                  layout="slot, jumper, prev, pager, next"
                  prev-text="上一页"
                  next-text="下一页"
                  :page-size="5"
                  :total="totalModule">
                  <span>总共 {{ totalModule }} 条记录</span>
                </el-pagination>
              <!--</el-scrollbar>-->
            </div>
          </div>
        </el-collapse-item>
        <el-collapse-item title="月生产计划选择" name="2" style="margin-top:10px;padding:0px;">
          <div class="box2">
            <!-- 搜索框 -->
            <div class="con-padding-horizontal search-wrap">
              <div class="wrap">
                <div class="input-group">
                  <label for="name">年份<em class="validate-mark">*</em></label>
                  <div class="inp">
                    <el-date-picker filterable
                      v-model="month.year"
                      style="width:200px!important"
                      type="year"
                      value-format="yyyy"
                      placeholder="请选择">
                    </el-date-picker>
                  </div>
                </div>
                <div class="input-group">
                  <label for="name">线路<em class="validate-mark">*</em></label>
                  <div class="inp">
                    <el-select v-model="month.LineID" placeholder="请选择" filterable>
                      <el-option
                      v-for="item in month.MetroLineList"
                      :key="item.key"
                      :value="item.id"
                      :label="item.lineName">
                      </el-option>
                    </el-select>
                  </div>
                </div>
                <div class="input-group">
                  <label for="name">公司<em class="validate-mark">*</em></label>
                  <div class="inp">
                    <el-cascader filterable
                      change-on-select
                      :props="month.defaultParams"
                      @change="company_change"
                      :show-all-levels="true"
                      :options="month.companyList"
                      v-model="month.companyPath">
                    </el-cascader>
                  </div>
                </div>
                <div class="input-group">
                  <label for="name"></label>
                  <div class="inp"></div>
                </div>
              </div>
              <div style="justify-content: space-between;display: flex;">
              <div class="wrap">
                <div class="input-group">
                  <label for="name">部门<em class="validate-mark">*</em></label>
                  <div class="inp">
                    <el-cascader filterable
                      change-on-select
                      :props="month.defaultParams"
                      @change="dept_change"
                      :show-all-levels="true"
                      :options="month.companyList"
                      v-model="month.deptPath">
                    </el-cascader>
                  </div>
                </div>
                <div class="input-group">
                  <label for="name">班组<em class="validate-mark">*</em></label>
                  <div class="inp">
                    <el-cascader filterable
                      change-on-select
                      :props="month.defaultParams"
                      @change="cascader_change"
                      :show-all-levels="true"
                      :options="month.teamList"
                      v-model="month.teamPath">
                    </el-cascader>
                  </div>
                </div>
                <div class="input-group">
                  <label for="name">地点<em class="validate-mark">*</em></label>
                  <div class="inp">
                    <el-cascader filterable
                      change-on-select
                      @change="position_change"
                      :props="month.areaParams"
                      :show-all-levels="true"
                      :options="month.areaList"
                      v-model="month.areaPath">
                    </el-cascader>
                  </div>
                </div>
                <div class="input-group" v-show="false">
                  <label for="name">计划日期</label>
                  <div class="inp">
                    <el-date-picker
                      v-model="month.planDate"
                      value-format="yyyy.MM.dd"
                      placeholder="请选择计划日期">
                    </el-date-picker>
                  </div>
                </div>
              </div>
              <div class="search-btn" @click="searchResult(1)">
                <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
              </div>
              </div>
            </div>
          </div>
          <div v-if="!isShowList" class="content-wrap">
            <el-tabs class="tab-height" v-model="month.activeName" @tab-click="searchResult(1)">
              <el-tab-pane class="pane-height pane-notification" :label="(index+1)+'月'" :name="index+''" v-for="(tab, index) in month.monthList" :key="tab.key">
                <!-- 内容 -->
                  <ul class="content-header">
                    <li class="list"></li>
                    <li class="list name">工作类型</li>
                    <li class="list name">设施设备</li>
                    <li class="list name">设备地点</li>
                    <li class="list name">管辖班组</li>
                    <li class="list name">维护类型</li>
                    <li class="list name">维护频次</li>
                    <li class="list name">数量</li>
                    <li class="list name">单位</li>
                    <li class="list last-update-time">计划日期</li>
                  </ul>
                  <div class="scroll">
                    <!--<el-scrollbar>-->
                      <ul class="list-wrap">
                        <li class="list" v-for="item in tab.rows" :key="item.key">
                          <div class="list-content">
                            <div class="checkbox">
                              <input type="checkbox" v-model="editMonth" :value="item" @change="monthDetailChange">
                            </div>
                            <div class="name">{{ item.workTypeName }}</div>
                            <div class="name">{{ item.eqpTypeName }}</div>
                            <div class="name">{{ item.locationName }}</div>
                            <div class="name">{{ item.teamName }}</div>
                            <div class="name">{{ item.pmTypeName }}</div>
                            <div class="name">{{ item.pmCycle }}</div>
                            <div class="name">{{ item.planQuantity }}</div>
                            <div class="name">{{ item.unit }}</div>
                            <div class="last-update-time">{{ item.planDate }}</div>
                          </div>
                        </li>
                      </ul>
                      <!-- 分页 -->
                      <el-pagination
                        :current-page.sync="month.currentPage"
                        @current-change="handleCurrentChange"
                        @prev-click="prevPage"
                        @next-click="nextPage"
                        layout="slot, jumper, prev, pager, next"
                        prev-text="上一页"
                        next-text="下一页"
                        :page-size="5"
                        :total="tab.total">
                        <span>总共 {{ tab.total }} 条记录</span>
                      </el-pagination>
                    <!--</el-scrollbar>-->
                  </div>
              </el-tab-pane>
            </el-tabs>
            <!-- 内容 -->
          </div>
          <div v-else class="content-wrap">
            <ul class="content-header">
              <li class="list name">月份</li>
              <li class="list name">工作类型</li>
              <li class="list name">设施设备</li>
              <li class="list name">设备地点</li>
              <li class="list name">管辖班组</li>
              <li class="list name">维护类型</li>
              <li class="list name">维护频次</li>
              <li class="list name">数量</li>
              <li class="list name">单位</li>
              <li class="list last-update-time">计划日期</li>
            </ul>
            <div class="scroll">
              <el-scrollbar>
                <ul class="list-wrap">
                  <li class="list" v-for="item in selectList" :key="item.key">
                    <div class="list-content">
                      <div class="name">{{ item.month }}</div>
                      <div class="name">{{ item.workTypeName }}</div>
                      <div class="name">{{ item.eqpTypeName }}</div>
                      <div class="name">{{ item.locationName }}</div>
                      <div class="name">{{ item.teamName }}</div>
                      <div class="name">{{ item.pmTypeName }}</div>
                      <div class="name">{{ item.pmCycle }}</div>
                      <div class="name">{{ item.planQuantity }}</div>
                      <div class="name">{{ item.unit }}</div>
                      <div class="last-update-time">{{ item.planDate }}</div>
                    </div>
                  </li>
                </ul>
              </el-scrollbar>
            </div>
          </div>
        </el-collapse-item>
        <el-collapse-item title="检修单填写" name="3" style="margin-top:10px;padding:0px;">
          <div class="con-padding-horizontal operation">
            <ul class="input-group">
              <li class="list">
                <div class="inp-wrap">
                  <span class="text">设备</span>
                  <div class="inp">
                    <el-select v-model="eqpID" placeholder="请选择" filterable>
                      <el-option
                      v-for="item in eqpList"
                      :key="item.key"
                      :value="item.id"
                      :label="item.name">
                      </el-option>
                    </el-select>
                  </div>
                </div>
                <p>注：若是针对设备的检修单，请务必选择，否则将不会记录进该设备的履历中</p>
              </li>
            </ul>
          </div>
          <HotTable :settings="settings" licenseKey="non-commercial-and-evaluation" style="margin-top: 20px"/>
        </el-collapse-item>
      </el-collapse>
      <div class="btn-enter">
        <x-button class="close">
          <router-link :to="{ name: 'EntityList' }">取消</router-link>
        </x-button>
        <x-button class="active" @click.native="enter(false)">保存(可修改)</x-button>
        <x-button class="active" @click.native="sure()">提交(不可修改)</x-button>
      </div>
      </el-scrollbar>
    </div>
    <!-- dialog对话框 -->
    <el-dialog
      :visible.sync="dialogVisible.isShow"
      :modal-append-to-body="false"
      :show-close="false">
      {{ dialogVisible.text }}
      <template slot="footer" class="dialog-footer">
        <template v-if="dialogVisible.btn">
          <el-button @click="dialogVisible.isShow = false">否</el-button>
          <el-button @click="enter(true)">是</el-button>
        </template>
        <el-button v-else @click="dialogVisible.isShow = false" :class="{ on: !dialogVisible.btn }">知道了</el-button>
      </template>
    </el-dialog>
  </div>
</template>
<script>
import { transformDate, getCascaderObj } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/workflowApi'
import apiOrg from '@/api/orgApi'
import apiEqp from '@/api/eqpApi'
import apiArea from '@/api/AreaApi.js'
import lineApi from '@/api/metroLineApi'
import { HotTable } from '@handsontable/vue'
// import Handsontable from 'handsontable'
export default {
  name: 'CreateEntity',
  components: {
    XButton,
    HotTable
  },
  data () {
    return {
      // settings: {
      //   data: Handsontable.helper.createSpreadsheetData(500, 50)
      // },
      settings: {},
      isEdit: false,
      editID: '',
      activeName: '1',
      dialogVisible: {
        isShow: false,
        text: '',
        btn: true
      },
      editModuleID: '',
      eqpID: '',
      eqpList: [],
      mList: [],
      totalModule: 0,
      pmModule: {
        moduleName: '',
        fileName: '',
        deviceName: '',
        keyWord: '',
        currentPage: 1,
        loading: false,
        currentSort: {
          sort: 'code',
          order: 'asc'
        },
        headOrder: {
          code: 1,
          name: 0,
          file_name: 0,
          device_name: 0,
          death_year: 0,
          level: 0,
          created_time: 0,
          created_by: 0
        }
      },
      isShowList: false,
      selectList: [],
      editMonth: [],
      month: {
        isChanged: false,
        year: '',
        LineID: '',
        MetroLineList: [],
        companyList: [],
        companyPath: [],
        deptPath: [],
        areaParams: {
          label: 'areaName',
          value: 'id',
          children: 'children'
        },
        defaultParams: {
          label: 'label',
          value: 'id',
          children: 'children'
        },
        activeName: '0',
        monthList: [
          [{
            workTypeName: 1,
            eqpTypeName: 2,
            locationName: 3,
            teamName: 4,
            pmTypeName: 5,
            pmFrequency: 6,
            planQuantity: 7,
            unit: 8,
            planDate: 9
          }],
          [{
            workTypeName: 10,
            eqpTypeName: 2,
            locationName: 3,
            teamName: 4,
            pmTypeName: 5,
            pmFrequency: 6,
            planQuantity: 7,
            unit: 8,
            planDate: 9
          }]
        ],
        currentPage: 1,
        team: '',
        teamPath: [],
        teamList: [],
        area: '',
        areaPath: [],
        areaList: [],
        planDate: ''
      },
      loading: false,
      title: ' | 创建检修单'
    }
  },
  created () {
    this.month.activeName = new Date().getMonth() + ''
    this.month.year = new Date().getFullYear() + ''
    lineApi.getAll().then(res => {
      this.month.MetroLineList = res.data
    }).catch(err => {
      console.log(err)
    })
    // 班组加载
    apiOrg.getOrgAll().then(res => {
      this.month.teamList = res.data
      this.month.companyList = res.data
    }).catch(err => console.log(err))
    // 地点加载
    apiArea.SelectConfigAreaData().then(res => {
      this.month.areaList = res.data.dicAreaList
    }).catch(err => console.log(err))
    this.month.monthList = []
    for (let i = 0; i < 12; i++) {
      this.month.monthList[i] = []
    }
    this.month.isChanged = false
    if (this.$route.params.type === 'edit') {
      this.month.year = ''
      this.isEdit = true
      this.editID = this.$route.params.id
      this.title = ' | 修改检修单'
      // this.loading = true
      api.getEntityByID(this.editID, true).then(res => {
        let _data = res.data
        // this.loading = false
        this.totalModule = 1
        this.mList = []
        _data.module.createdTime = transformDate(_data.module.createdTime)
        this.mList.push(_data.module)
        this.editModuleID = _data.module.id

        this.selectList = _data.cpmd
        this.loadEqpList(_data.cpmd[0].team)
        this.eqpID = _data.eqp === null ? '' : _data.eqp
        this.editMonth = _data.cpmd
        this.isShowList = true

        this.settings = {
          data: _data.showData,
          tableClassName: ['htMiddle', 'htCenter'],
          width: '100%',
          height: 400,
          mergeCells: _data.showMerge
        }
        this.activeName = '3'
      }).catch(err => console.log(err))
    } else {
      this.isEdit = false
      this.isShowList = false
      this.title = ' | 创建检修单'
      this.activeName = '1'
      this.searchResultModule(1)
    }
  },
  methods: {
    monthDetailChange () {
      this.month.isChanged = true
    },
    sure () {
      this.dialogVisible.isShow = true
      this.dialogVisible.btn = true
      this.dialogVisible.text = '提交后将不可修改，是否确认?'
    },
    enter (isFinished) {
      if (this.editModuleID === '') {
        this.$message({
          message: '请选择模板',
          type: 'warning'
        })
        return
      }
      if (this.editMonth.length === 0) {
        this.$message({
          message: '请选择月计划',
          type: 'warning'
        })
        return
      }
      let items = this.editMonth
      let ids = []
      items.map(val => {
        ids.push(val.id)
      })
      let pmEntity = {
        title: this.settings.data[0][0] + '(' + items[0].pmTypeName + ')',
        team: items[0].team,
        planDate: items[0].planDate,
        location: items[0].location,
        locationBy: items[0].locationBy,
        module: this.editModuleID,
        contents: this.settings.data,
        eqp: this.eqpID,
        isFinished: isFinished,
        PMMonthDetails: ids
      }
      if (this.isEdit) {
        pmEntity.id = this.editID
        pmEntity.isPlanChanged = this.month.isChanged
        api.updatePMEntity(pmEntity).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '操作成功',
              type: 'success'
            })
            if (isFinished) {
              this.$router.push({
                name: 'EntityList'
              })
              this.dialogVisible.isShow = false
            }
          } else {
            this.$message({
              message: res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else {
        api.savePMEntity(pmEntity).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '操作成功',
              type: 'success'
            })
            if (isFinished) {
              this.$router.push({
                name: 'EntityList'
              })
              this.dialogVisible.isShow = false
            }
          } else {
            this.$message({
              message: res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      }
    },
    handleChange () {
      if ((this.activeName === '3' || this.activeName === '') && this.editModuleID === '') {
        this.activeName = '1'
      }
    },
    getModuleByID () {
      api.getModuleByID(this.editModuleID).then(res => {
        this.settings = {
          // readOnly: true,
          data: res.data.data,
          tableClassName: ['table01', 'htCenter'],
          width: '100%',
          height: 400,
          mergeCells: res.data.mergeCells
        }
      }).catch(err => console.log(err))
    },
    position_change (val) {
      if (val.length === 0) {
        this.month.area = ''
        this.month.areaPath = []
        return
      }
      if (this.month.areaPath.length < 3) {
        this.month.area = ''
        this.month.areaPath = []
        this.$message({
          message: '地点在3级和4级位置',
          type: 'warning'
        })
      } else {
        this.month.area = val[val.length - 1]
      }
    },
    company_change (val) {
      if (val.length === 0) {
        this.month.companyPath = []
        return
      }
      let selectedTeam = val[val.length - 1]
      let obj = getCascaderObj(selectedTeam, this.month.companyList)
      if (obj.node_type !== 1) {
        this.month.companyPath = []
        this.$message({
          message: '您选择的不是公司',
          type: 'warning'
        })
      }
    },
    dept_change (val) {
      if (val.length === 0) {
        this.month.deptPath = []
        return
      }
      let selectedTeam = val[val.length - 1]
      let obj = getCascaderObj(selectedTeam, this.month.companyList)
      if (obj.node_type !== 2) {
        this.month.deptPath = []
        this.$message({
          message: '您选择的不是部门',
          type: 'error'
        })
      }
    },
    // 班组下拉选中，过滤非班组
    cascader_change (val) {
      if (val.length === 0) {
        this.month.teamPath = []
        this.month.team = ''
        return
      }
      let selectedTeam = val[val.length - 1]
      let obj = getCascaderObj(selectedTeam, this.month.teamList)
      if (obj.node_type === 3) {
        this.month.team = selectedTeam
        this.loadEqpList(selectedTeam)
      } else {
        this.month.team = ''
        this.month.teamPath = []
        this.$message({
          message: '您选择的不是班组',
          type: 'warning'
        })
      }
    },
    // 根据班组加载设备
    loadEqpList (team) {
      apiEqp.getEqpByTeam(team).then(res => {
        this.eqpList = res.data
      }).catch(err => console.log(err))
    },
    searchResult (page) {
      if (this.month.year === '' || this.month.LineID === '' || this.month.companyPath.length === 0 || this.month.deptPath.length === 0 ||
      this.month.teamPath.length === 0 || this.month.areaPath.length === 0) {
        this.$message({
          message: '所有选项必选',
          type: 'warning'
        })
        return
      }
      this.isShowList = false
      this.month.currentPage = page
      // this.loading = true
      let month = parseInt(this.month.activeName)
      let selectMonth
      if (this.month.planDate !== null && this.month.planDate !== '') {
        selectMonth = parseInt(this.month.planDate.split('.')[1])
        if (selectMonth !== month + 1) {
          month = selectMonth - 1
          this.month.activeName = month + ''
        }
      }
      api.getMonthPlanPage({
        // order: this.currentSort.order,
        // sort: this.currentSort.sort,
        rows: 5,
        page: page,
        Year: this.month.year,
        Line: this.month.LineID,
        Company: this.month.companyPath[this.month.companyPath.length - 1],
        Department: this.month.deptPath[this.month.deptPath.length - 1],
        Month: month + 1,
        Team: this.month.team,
        Location: this.month.area,
        LocationBy: this.month.areaPath.length === 0 ? '' : this.month.areaPath.length - 2,
        PlanDate: this.month.planDate
      }).then(res => {
        this.loading = false
        let tmp = res.data
        if (tmp === null) tmp = []
        this.$set(this.month.monthList, month, tmp)
        // this.monthList[month] = res.data
      }).catch(err => console.log(err))
    },
    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.month.currentPage = val
      this.searchResult(val)
    },

    // 上一页
    prevPage (val) {
      this.month.currentPage = val
      // this.searchResult(val)
    },

    // 下一页
    nextPage (val) {
      this.month.currentPage = val
      // this.searchResult(val)
    },
    searchResultModule (page) {
      this.pmModule.currentPageModule = page
      this.loading = true
      let parm = {
        order: this.pmModule.currentSort.order,
        sort: this.pmModule.currentSort.sort,
        rows: 5,
        page: page,
        moduleName: this.pmModule.moduleName,
        fileName: this.pmModule.fileName,
        deviceName: this.pmModule.deviceName,
        keyWord: this.pmModule.keyWord
      }
      api.getModuleList(parm).then(res => {
        this.loading = false
        if (res.data.total > 0) {
          res.data.rows.map(item => {
            item.createdTime = transformDate(item.createdTime)
          })
          this.mList = res.data.rows
          this.totalModule = res.data.total
        } else {
          this.mList = []
          this.total = 0
        }
      }).catch(err => console.log(err))
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.code = 0
        this.headOrder.name = 0
        this.headOrder.file_name = 0
        this.headOrder.death_year = 0
        this.headOrder.device_name = 0
        this.headOrder.level = 0
        this.headOrder.updated_by = 0
        this.headOrder.updated_time = 0
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
      this.bCheckAll = false
      this.checkAll()
      this.searchResult(this.currentPage)
    },
    // 搜索功能
    searchResModule () {
      this.loading = true
      this.searchResultModule(1)
    },
    // 序号、指定页翻页
    handleCurrentChangeModule (val) {
      this.pmModule.currentPageModule = val
      this.searchResultModule(val)
    },
    // 上一页
    prevPageModule (val) {
      this.pmModule.currentPageModule = val
    },

    // 下一页
    nextPageModule (val) {
      this.pmModule.currentPageModule = val
    },
    detail (id) {
      this.$router.push({
        name: 'DetailMaintenanceList',
        params: {
          id: id,
          sourceName: 'CreateEntity'
        }
      })
    }
  }
}
</script>
<style lang="scss" scoped>
$con-height: $content-height + 146;
.box1{
    // height: percent(145, $content-height);
    height: 60px;
    // 搜索组
    .search-wrap{
      display: flex;
      justify-content: space-between;
      align-items: center;
      height: 100%;
      // height: percent(80, 145);
      background: rgba(128, 128, 128, 0.1);
      color: $color-white;

      .wrap{
        display: flex;
      }

      .input-group{
        display: inherit;
        align-items: center;
        margin-right: PXtoEm(24);
      }

      .inp{
        width: PXtoEm(160);
        margin-left: PXtoEm(14);
      }

      .btn{
        border: 0;
        background: $color-main-btn;
      }
    }
  }
  // 两行查询条件
.box2{
    // height: percent(145, $content-height);
    height: 90px;
    // 搜索组
    .search-wrap{
      // display: flex;
      justify-content: space-between;
      align-items: center;
      height: 100%;
      // height: percent(80, 145);
      background: rgba(128, 128, 128, 0.1);
      color: $color-white;

      .wrap{
        display: flex;
      }

      .input-group{
        margin-top: 10px;
        display: inherit;
        align-items: center;
        margin-right: PXtoEm(24);
      }

      .inp{
        width: PXtoEm(160);
        margin-left: PXtoEm(14);
      }

      .btn{
        margin-top: 10px;
        border: 0;
        background: $color-main-btn;
      }
    }
  }
// 内容区
.content-wrap{
  overflow: hidden;
  // height: percent($con-height, $content-height);
  height: 400px!important;
  text-align: center;
  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 50px!important;
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);

    .last-update-time{
      color: $color-white;
    }
  }

  .list-wrap{
    .list{
      &:nth-of-type(even){
        .list-content{
          background: rgba(186,186,186,.5);
        }
      }
    }

    .list-content{
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: PXtoEm(15) PXtoEm(24);

      div{
        word-break: break-all;
      }
    }

    .left-title{
      margin-right: 10px;
      font-weight: bold;
    }

    // 隐藏内容
    .sub-content{
      overflow: hidden;
      height: 0;
      font-size: $font-size-small;
      text-align: left;
      color: $color-content-text;

      &.active{
        overflow: inherit;
        height: auto;
        transition: .7s .2s;
      }
    }

    .sub-con-list{
      display: flex;
      padding: PXtoEm(15) PXtoEm(24);
      border-top: 1px solid $color-main-background;
      background: rgba(0,0,0,.2);

      .right-wrap{
        display: flex;
        flex-wrap: wrap;
      }

      .list{
        margin-right: 10px;
      }
    }
  }

  .number{
    width: 4%;
  }
  .name,
  .btn-wrap{
    width: 10%;
  }

  .name{
    a{
      color: #42abfd;
    }
  }

  .last-update-time{
    width: 18%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 10%;
  }

  .state{
    width: 5%;
  }
}

.scroll{
  height: percent($con-height - 50, $con-height);
  // height: 50%
}
// 功能区
.operation{
  .input-group{
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list{
      width: 30%;
      margin-top: PXtoEm(25);

      span{
        width: 28%;
      }

      .inp-wrap{
        display: flex;
        align-items: center;
      }

      &:nth-of-type(3n+1){
        // justify-content: flex-start;
      }

      &:nth-of-type(3n){
        // justify-content: flex-end;
      }
    }
  }
}
.btn-enter{
  margin: 15px 0;
  text-align: center;

  button{
    border-color: $color-main-btn;
    background: $color-main-btn;
  }
}
.import-btn{
  margin-top: 50px;
  border-color: $color-main-btn!important;
  background: $color-main-btn!important;
  &:hover{
    border-color: $color-main-btn!important;
    background: $color-main-btn!important;
  }
}

/deep/
.el-year-table td .cell{
  color: $color-white!important;
}
/deep/
.el-date-picker__header-label{
  color: $color-white!important;
}
/deep/
.el-collapse .el-collapse-item__content{
  padding-top: 10px
}
</style>
