<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <i @click="$router.back(-1)"><x-button class="active">返回</x-button></i>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
              <span class="text">计划名称:</span>
              <div class="inp-wrap">
                <el-select v-model="lineid.text"  collapse-tags  clearable filterable placeholder="请选择" :disabled="isDisabled">
                  <el-option
                    v-for="item in lineList"
                    :key="item.id"
                    :label="item.lineName"
                    :value="item.id">
                  </el-option>
                </el-select><p class="validate-tips">{{ lineid.tips }}</p>
                <el-select v-model="areaid.text"  collapse-tags  clearable filterable placeholder="请选择" :disabled="isDisabled">
                  <el-option
                    v-for="item in areaList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.key">
                  </el-option>
                </el-select><p class="validate-tips">{{ areaid.tips }}</p>
                <el-select v-model="areaTypeName.text"  collapse-tags  clearable filterable placeholder="请选择" :disabled="isDisabled">
                  <el-option
                    v-for="item in areaTypeNameList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.key">
                  </el-option>
                </el-select><p class="validate-tips">{{ areaTypeName.tips }}</p>
                <span style="width: 200px;">施工检修申请单</span>
              </div>
          </li>
          <li class="list">
              <span class="text">作业类别:</span>
              <div class="inp-wrap">
                <el-radio-group v-model="planType.text"  :disabled="isDisabled">
                  <el-radio :label=1>单点</el-radio>
                  <el-radio :label=2>多点</el-radio>
                </el-radio-group><p class="validate-tips">{{ planType.tips }}</p>
              </div>
          </li>
        </ul>
        <ul class="con-padding-horizontal input-group">
          <li class="list">
              <span class="text">重要程度:</span>
              <div class="inp-wrap">
                <el-select v-model="importantLevel.text"  collapse-tags  clearable filterable placeholder="请选择" :disabled="isDisabled">
                  <el-option
                    v-for="item in importantLevelList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.key">
                  </el-option>
                </el-select><p class="validate-tips">{{ importantLevel.tips }}</p>
                <span>日常/重大</span>
                <el-select v-model="planLevel.text"  collapse-tags  clearable filterable placeholder="请选择" :disabled="isDisabled">
                  <el-option
                    v-for="item in planLevelList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.key">
                  </el-option>
                </el-select><p class="validate-tips">{{ planLevel.tips }}</p>
              </div>
          </li>
          <li class="list">
              <span class="text">计划编号:</span>
              <div class="inp-wrap">
                <el-input style="width:200px;" :disabled="isDisabled"
                  placeholder="输入计划编号"
                  v-model="planNumber.text" clearable
                  @keyup.native="validateInputNull(planNumber)">
                </el-input><p class="validate-tips">{{ planNumber.tips }}</p>
              </div>
          </li>
        </ul>
        <ul class="con-padding-horizontal input-group">
          <li class="list">
              <span class="text">申请单位:</span>
              <div class="inp-wrap">
                <el-select v-model="applyCompanyOrgId.text"  collapse-tags  clearable filterable placeholder="请选择" :disabled="isDisabled">
                  <el-option
                    v-for="item in applyCompanyOrgIdList"
                    :key="item.id"
                    :label="item.label"
                    :value="item.id">
                  </el-option>
                </el-select><p class="validate-tips">{{ applyCompanyOrgId.tips }}</p>
              </div>
          </li>
          <li class="list">
              <span class="text">施工单位:</span>
              <div class="inp-wrap">
                <el-select v-model="constructionCompanyOrgId.text"  collapse-tags  clearable filterable placeholder="请选择" :disabled="isDisabled">
                  <el-option
                    v-for="item in constructionCompanyOrgIdList"
                    :key="item.id"
                    :label="item.label"
                    :value="item.id">
                  </el-option>
                </el-select><p class="validate-tips">{{ constructionCompanyOrgId.tips }}</p>
              </div>
          </li>
        </ul>
        <ul class="con-padding-horizontal input-group">
          <li class="list">
              <span class="text">起止时间:</span>
              <div class="inp-wrap">
                <el-date-picker :disabled="isDisabled"
                  class="eldatepicker"
                  v-model="time.text"
                  type="datetimerange"
                  prefix-icon="el-icon-date"
                  :unlink-panels="true"
                  value-format="yyyy-MM-dd HH:mm"
                  range-separator="至"
                  start-placeholder="计划开始日期"
                  end-placeholder="计划结束日期">
                </el-date-picker><p class="validate-tips">{{ time.tips }}</p>
              </div>
          </li>
          <li class="list">
              <span class="text">登记车站:</span>
              <div class="inp-wrap">
                <el-select v-model="registerStationId.text"  collapse-tags  clearable filterable placeholder="请选择" :disabled="isDisabled">
                  <el-option
                    v-for="item in registerStationIdList"
                    :key="item.title"
                    :label="item.title"
                    :value="item.title">
                  </el-option><p class="validate-tips">{{ registerStationId.tips }}</p>
                </el-select>
              </div>
          </li>
        </ul>
        <ul class="con-padding-horizontal input-group">
          <li class="list">
              <span class="text">设施设备号:</span>
              <div class="inp-wrap">
                <el-input style="width:200px;" :disabled="isDisabled"
                  placeholder="输入设施设备号"
                  v-model="deviceNum.text" clearable>
                </el-input><p class="validate-tips">{{ deviceNum.tips }}</p>
              </div>
          </li>
          <li class="list">
              <span class="text">故障令号:</span>
              <div class="inp-wrap">
                <el-input style="width:200px;" :disabled="isDisabled"
                  placeholder="输入故障令号"
                  v-model="troubleNum.text" clearable>
                </el-input><p class="validate-tips">{{ troubleNum.tips }}</p>
              </div>
          </li>
        </ul>
        <div class="con-padding-horizontal cause">
            <span class="lable">工作地点:</span>
            <el-input type="textarea" v-model="operationAddress.text" placeholder="请输入工作地点" @keyup.native="validateInputNull(operationAddress)" :disabled="isDisabled"></el-input>
            <p class="validate-tips">{{ operationAddress.tips }}</p>
        </div>
        <div class="con-padding-horizontal cause">
            <span class="lable">施工内容:</span>
            <el-input type="textarea" v-model="constructionContent.text" placeholder="请输入施工内容" @keyup.native="validateInputNull(constructionContent)" :disabled="isDisabled"></el-input>
            <p class="validate-tips">{{ constructionContent.tips }}</p>
        </div>
        <div class="con-padding-horizontal cause">
            <span class="lable">施工详细说明:</span>
            <el-input type="textarea" v-model="constructionDetail.text" placeholder="请输入施工内容" @keyup.native="validateInputNull(constructionDetail)" :disabled="isDisabled"></el-input>
            <p class="validate-tips">{{ constructionDetail.tips }}</p>
        </div>
        <div class="con-padding-horizontal cause">
            <span class="lable">配合要求:</span>
            <el-input type="textarea" v-model="coordinationRequest.text" placeholder="请输入配合要求" @keyup.native="validateInputNull(coordinationRequest)" :disabled="isDisabled"></el-input>
            <p class="validate-tips">{{ coordinationRequest.tips }}</p>
        </div>
        <div class="con-padding-horizontal cause">
            <span class="lable">配合意见:</span>
            <el-input type="textarea" v-model="coordinationAudit.text" placeholder="请输入配合要求" @keyup.native="validateInputNull(coordinationAudit)" :disabled="isDisabled"></el-input>
            <p class="validate-tips">{{ coordinationAudit.tips }}</p>
        </div>
        <div class="con-padding-horizontal cause">
            <span class="lable">停送电范围:</span>
            <el-input type="textarea" v-model="electricRange.text" placeholder="请输入停送电范围" @keyup.native="validateInputNull(electricRange)" :disabled="isDisabled"></el-input>
            <p class="validate-tips">{{ electricRange.tips }}</p>
        </div>
        <ul class="con-padding-horizontal input-group">
          <li class="list">
              <span class="text">接触网/轨停电:</span>
              <div class="inp-wrap">
                <el-select v-model="stopElectric.text"  collapse-tags  clearable filterable placeholder="请选择" :disabled="isDisabled">
                  <el-option
                    v-for="item in stopElectricList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.key">
                  </el-option><p class="validate-tips">{{ stopElectric.tips }}</p>
                </el-select>
                <span class="text">牵引动力:</span>
                <el-select v-model="tractionPower.text"  collapse-tags  clearable filterable placeholder="请选择" :disabled="isDisabled">
                  <el-option
                    v-for="item in tractionPowerList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.key">
                  </el-option>
                </el-select><p class="validate-tips">{{ tractionPower.tips }}</p>
              </div>
          </li>
          <li class="list">
              <span class="text">使用梯车:</span>
              <div class="inp-wrap">
                <el-select v-model="useLaddercar.text"  collapse-tags  clearable filterable placeholder="请选择" :disabled="isDisabled">
                  <el-option
                    v-for="item in useLaddercarList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.key">
                  </el-option>
                </el-select><p class="validate-tips">{{ useLaddercar.tips }}</p>
                <span class="lable">牵引动力列数:</span>
                <el-input style="width:150px;" :disabled="isDisabled"
                  placeholder="输入牵引动力列数"
                  v-model="tractionTrainNum.text" clearable>
                </el-input><p class="validate-tips">{{ tractionTrainNum.tips }}</p>
              </div>
          </li>
        </ul>
        <div class="con-padding-horizontal cause">
            <span class="lable">施工影响区段:</span>
            <el-input type="textarea" v-model="effectArea.text" placeholder="请输入施工影响区段" @keyup.native="validateInputNull(effectArea)" :disabled="isDisabled"></el-input>
            <p class="validate-tips">{{ effectArea.tips }}</p>
        </div>
        <div class="con-padding-horizontal cause">
            <span class="lable">施工影响说明:</span>
            <el-input type="textarea" v-model="effectExplain.text" placeholder="请输入施工影响说明" @keyup.native="validateInputNull(effectExplain)" :disabled="isDisabled"></el-input>
            <p class="validate-tips">{{ effectExplain.tips }}</p>
        </div>
        <div class="con-padding-horizontal cause">
            <span class="lable">安全措施:</span>
            <el-input type="textarea" v-model="safeMeasure.text" placeholder="请输入安全措施" @keyup.native="validateInputNull(safeMeasure)" :disabled="isDisabled"></el-input>
            <p class="validate-tips">{{ safeMeasure.tips }}</p>
        </div>
        <div class="upload-list">
          <upload-pdf :systemResource="systemResource" :fileIDs="fileIDs" @getFileIDs="getFileIDs" :readOnly="isDisabled"></upload-pdf>
        </div>
        <button @click="changeShow"></button>
        <area-dialog :name="show" @changeShowChild = "changeShow"></area-dialog>
        <!-- 按钮 -->
        <div class="btn-group" v-show="!isDisabled">
          <x-button class="close" @click.native="back">取消</x-button>
          <x-button class="active" @click.native="save">保存</x-button>
        </div>
        <!-- <div class="btn-group">
          <workflow-module :AppInstanceID="ID"></workflow-module>
        </div> -->
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vInput, nullToEmpty } from '@/common/js/utils.js'
import { systemResource } from '@/common/js/dictionary.js'
import { isUploadFinished } from '@/common/js/UpDownloadFileHelper.js'
import XButton from '@/components/button'
import constructionapi from '@/api/workflowApi'
import MyUploadPDF from '@/components/UploadPDF'
import lineapi from '@/api/metroLineApi'
import orgapi from '@/api/orgApi'
import AreaDialog from '@/components/AreaDialog'
import WorkflowModule from '@/components/WorkflowModule'
import ConstrucionPlanEnum from './ConstrucionPlanEnum'
export default {
  name: 'AddConstructionPlan',
  components: {
    XButton,
    'upload-pdf': MyUploadPDF,
    'area-dialog': AreaDialog,
    'workflow-module': WorkflowModule
  },
  watch: {
    '$route.path': function () {
      console.log('changeto:' + this.$route.path)
      this.InitTitle()
    }
  },
  data () {
    return {
      msg: 'Welcome to Your Vue.js App',
      sourceName: '',
      isDisabled: false,
      show: false,
      systemResource: systemResource.construction,
      loading: false,
      title: '| 添加施工计划',
      ID: this.$route.query.id,
      eqpTypeName: {text: '', tips: ''},
      model: {text: '', tips: ''},
      eqpTypeDesc: {text: '', tips: ''},
      operationAddress: {text: '', tips: ''},
      constructionContent: {text: '', tips: ''},
      constructionDetail: {text: '', tips: ''},
      coordinationRequest: {text: '', tips: ''},
      coordinationAudit: {text: '', tips: ''},
      electricRange: {text: '', tips: ''},
      effectArea: {text: '', tips: ''},
      effectExplain: {text: '', tips: ''},
      safeMeasure: {text: '', tips: ''},
      isEditFile: false,
      fileIDs: '',
      fileIDsEdit: [],
      planType: {text: '', tips: ''},
      areaid: {text: '', tips: ''},
      areaList: [],
      areaTypeName: {text: '', tips: ''},
      areaTypeNameList: [],
      importantLevel: {text: '', tips: ''},
      importantLevelList: [],
      planLevel: {text: '', tips: ''},
      planLevelList: [],
      applyCompanyOrgId: {text: '', tips: ''},
      applyCompanyOrgIdList: [],
      constructionCompanyOrgId: {text: '', tips: ''},
      constructionCompanyOrgIdList: [],
      lineList: [],
      lineid: {text: '', tips: ''},
      time: {text: '', tips: ''},
      planNumber: {text: '', tips: ''},
      areaTypename: {text: '', tips: ''},
      planName: {text: '', tips: ''},
      registerStationId: {text: '', tips: ''},
      registerStationIdList: [],
      deviceNum: {text: '', tips: ''},
      troubleNum: {text: '', tips: ''},
      troubleID: '',
      stopElectric: {text: '', tips: ''},
      stopElectricList: [],
      tractionPower: {text: '', tips: ''},
      tractionPowerList: [],
      useLaddercar: {text: '', tips: ''},
      useLaddercarList: [],
      tractionTrainNum: {text: '', tips: ''},
      conPlanType: 0,
      sourceId: 0
    }
  },
  created () {
    this.InitPage()
  },
  mounted () {
    this.InitTitle()
  },
  methods: {
    back () {
      // this.$router.push({
      //   name: this.sourceName
      // })
      this.$router.back(-1)
    },
    InitTitle () {
      let sourceId = this.$route.query.sourceId
      // console.log('sourceId:' + sourceId)
      let r = this.$router.navList[this.$route.matched[0].path]
      // console.log('add1:' + this.$route.matched[1].path)
      // console.log('add2:' + this.$route.path)
      let rc = r.children
      let tmptitle = ' | ' + rc.filter(item => item.id === sourceId)[0].name
      if (this.$route.query.type !== 'Add') {
        this.loading = true
        this.getObjByID()
        if (this.$route.query.type === 'Edit') {
          this.title = '| 修改 ' + tmptitle
        } else {
          this.title = '| 明细 ' + tmptitle
          this.isDisabled = true
        }
      } else if (this.$route.query.code !== undefined) {
        this.troubleNum.text = this.$route.query.code
        this.troubleID = this.$route.query.troubleID
      }
      if (this.$route.query.type === 'Add') {
        this.title = '| 添加 ' + tmptitle
      }
      this.sourceName = this.$route.query.sourceName
      this.conPlanType = ConstrucionPlanEnum.conPlanTypeMenu[sourceId]
      this.sourceId = sourceId
    },
    changeShow () {
      this.show = !this.show
    },
    getFileIDs (ids) {
      this.fileIDsEdit = ids
      if (this.$route.query.type !== 'Add') {
        this.isEditFile = true
      }
    },
    getObjByID () {
      constructionapi.getConstructionPlanByID(this.ID).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.ID = data.id
          this.lineid.text = data.lineId
          this.areaid.text = data.areaId
          this.areaTypeName.text = data.areaTypename
          this.planNumber.text = nullToEmpty(data.planNumber)
          this.planType.text = data.planType
          this.importantLevel.text = data.importantLevel
          this.planLevel.text = data.planLevel
          this.applyCompanyOrgId.text = data.applyCompanyOrgId
          this.importantLevel.text = data.importantLevel
          this.constructionCompanyOrgId.text = data.constructionCompanyOrgId
          this.time.text = [data.startTime, data.endTime]
          this.registerStationId.text = data.registerStationId
          this.deviceNum.text = data.deviceNum
          this.troubleNum.text = data.troubleNum
          this.operationAddress.text = data.operationAddress
          this.constructionContent.text = data.constructionContent
          this.constructionDetail.text = data.constructionDetail
          this.coordinationRequest.text = data.coordinationRequest
          this.coordinationAudit.text = data.coordinationAudit
          this.electricRange.text = data.electricRange
          this.stopElectric.text = data.stopElectric
          this.tractionPower.text = data.tractionPower
          this.tractionTrainNum.text = data.tractionTrainNum
          this.useLaddercar.text = data.useLaddercar
          this.effectArea.text = data.effectArea
          this.effectExplain.text = data.effectExplain
          this.safeMeasure.text = data.safeMeasure
          this.fileIDs = data.fileIDs
          this.conPlanType = data.conPlanType
        }
        this.loading = false
      }).catch(err => console.log(err))
    },
    validateInputNull (val) {
      if (!vInput(val.text)) {
        val.tips = '此项含有非法字符'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateSelect (val) {
      if (val.text === '') {
        val.tips = '此项必选'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateInput () {
      if (!this.validateSelect(this.lineid)) return false
      if (!this.validateSelect(this.areaid)) return false
      if (!this.validateSelect(this.areaTypeName)) return false
      if (!this.validateSelect(this.planType)) return false
      if (!this.validateSelect(this.importantLevel)) return false
      if (!this.validateSelect(this.planLevel)) return false
      if (!validateInputCommon(this.planNumber)) return false
      if (!this.validateSelect(this.applyCompanyOrgId)) return false
      if (!this.validateSelect(this.constructionCompanyOrgId)) return false
      if (!this.validateSelect(this.time)) return false
      if (!this.validateSelect(this.registerStationId)) return false
      if (!this.validateSelect(this.deviceNum)) return false
      if (!this.validateSelect(this.operationAddress)) return false
      if (!this.validateSelect(this.constructionContent)) return false
      if (!this.validateSelect(this.constructionDetail)) return false
      if (!this.validateSelect(this.coordinationRequest)) return false
      if (!this.validateSelect(this.coordinationAudit)) return false
      if (!this.validateSelect(this.electricRange)) return false
      if (!this.validateSelect(this.stopElectric)) return false
      if (!this.validateSelect(this.tractionPower)) return false
      if (!this.validateSelect(this.useLaddercar)) return false
      if (!this.validateSelect(this.effectArea)) return false
      if (!this.validateSelect(this.effectExplain)) return false
      if (!this.validateSelect(this.safeMeasure)) return false
      return true
    },
    validateInputAll () {
      if (!this.validateInput()) {
        return false
      }
      return true
    },
    save () {
      if (!this.validateInputAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      if (this.fileIDsEdit.length !== 0 && !isUploadFinished(this.fileIDsEdit)) {
        this.$message({
          message: '文件正在上传中，请耐心等待',
          type: 'warning'
        })
        return
      }
      var lineidobj = {}
      lineidobj = this.lineList.find(item => {
        return item.id === this.lineid.text
      })
      var areaidobj = {}
      areaidobj = this.areaList.find(item => {
        return item.key === this.areaid.text
      })
      // console.log(obj.chname)
      this.planName = lineidobj.lineName + areaidobj.name + this.areaTypeName.text + '施工检修申请单'
      var sTime = ''
      var eTime = ''
      if (this.time.text) {
        sTime = this.time.text[0]
        eTime = this.time.text[1]
      }
      let dataObj = {
        LineId: this.lineid.text,
        AreaId: this.areaid.text,
        AreaTypename: this.areaTypeName.text,
        PlanNumber: this.planNumber.text,
        PlanName: this.planName,
        PlanType: this.planType.text,
        PlanLevel: this.planLevel.text,
        ApplyCompanyOrgId: this.applyCompanyOrgId.text,
        ImportantLevel: this.importantLevel.text,
        constructionCompanyOrgId: this.constructionCompanyOrgId.text,
        StartTime: sTime,
        EndTime: eTime,
        registerStationId: this.registerStationId.text,
        deviceNum: this.deviceNum.text,
        troubleNum: this.troubleNum.text,
        operationAddress: this.operationAddress.text,
        constructionContent: this.constructionContent.text,
        constructionDetail: this.constructionDetail.text,
        coordinationRequest: this.coordinationRequest.text,
        coordinationAudit: this.coordinationAudit.text,
        electricRange: this.electricRange.text,
        stopElectric: this.stopElectric.text,
        tractionPower: this.tractionPower.text,
        tractionTrainNum: this.tractionTrainNum.text,
        useLaddercar: this.useLaddercar.text,
        effectArea: this.effectArea.text,
        effectExplain: this.effectExplain.text,
        safeMeasure: this.safeMeasure.text,
        FileIDs: this.fileIDsEdit.length === 0 && !this.isEditFile ? '' : JSON.stringify(this.fileIDsEdit),
        ConPlanType: this.conPlanType
      }
      if (this.$route.query.type === 'Add') {
        constructionapi.addConstructionPlan(dataObj).then(res => {
          if (res.code === 0) {
            // this.$router.push({name: 'SeeConstructionPlan'})
            this.$router.push({path: '/constructionManager1/ConstructionPlan/' + this.sourceId})
            this.$message({
              message: '添加成功',
              type: 'success'
            })
          } else {
            this.$message({
              message: res.msg === '' ? '添加失败' : res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else {
        dataObj.ID = this.ID
        constructionapi.updateConstructionPlan(dataObj).then(res => {
          if (res.code === 0) {
            this.$router.push({path: '/constructionManager1/ConstructionPlan/' + this.sourceId})
            this.$message({
              message: '修改成功',
              type: 'success'
            })
          } else {
            this.$message({
              message: res.msg === '' ? '修改失败' : res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      }
    },
    InitPage () {
      this.InitLine()
      this.InitArea()
      this.InitImportantLevel()
      this.InitCompany()
      this.InitStation()
    },
    InitLine () {
      let parm = {
        order: 'asc',
        sort: 'id',
        rows: 20,
        page: 1
      }
      lineapi.getByPage(parm).then(res => {
        this.lineList = res.data.rows
      }).catch(err => console.log(err))
    },
    InitArea () {
      this.areaList = [{key: 1, name: '轨行区'}, {key: 2, name: '车场'}, {key: 3, name: '车站'}, {key: 4, name: '保护区'}]
      this.areaTypeNameList = [{key: '正线', name: '正线'}, {key: '不下线', name: '不下线'}]
    },
    InitImportantLevel () {
      this.importantLevelList = [{key: 1, name: '普通计划'}, {key: 2, name: '核心计划'}, {key: 3, name: '临时计划'}, {key: 4, name: '抢修计划'}]
      this.planLevelList = [{key: 1, name: '日常'}, {key: 2, name: '重大'}]
      this.stopElectricList = [{key: 1, name: '是'}, {key: 2, name: '否'}]
      this.tractionPowerList = [{key: 1, name: '电客'}, {key: 2, name: '轨道'}, {key: 3, name: '无'}]
      this.useLaddercarList = [{key: 1, name: '是'}, {key: 2, name: '否'}]
    },
    InitCompany () {
      orgapi.getOrgAll().then(res => {
        this.applyCompanyOrgIdList = res.data.filter((item) => { return item.node_type === 1 })
        this.constructionCompanyOrgIdList = res.data.filter((item) => { return item.node_type === 1 })
      }).catch(err => console.log(err))
    },
    InitStation () {
      lineapi.getAllStation().then(res => {
        this.registerStationIdList = res.data
      }).catch(err => console.log(err))
    }
  }
}
</script>
<style lang="scss" scoped>
// 显示大图容器
.el-dialog__wrapper{
  width: 100%;
  height: 100%;

  .el-carousel{
    height: 100%;
  }
}

.header{
  display: flex;
  justify-content: space-between;
}

// 顶部信息
.middle{
  position: relative;
  margin-bottom: 10px;
  padding-bottom: 20px;

  .text-right{
    text-align: right !important;
  }

  [class*="list-wrap"]{
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list{
      margin-top: 20px;
      padding: 0 2%;
      border-right: 1px solid #C9CACD;
      text-align: center;

      &:first-of-type{
        padding-left: 0;
        text-align: left;
      }

      &:last-of-type{
        padding-right: 0;
        border: 0;
        text-align: right;
      }
    }
  }

  .list-wrap{
    .list{
      width: 16%;

      span{
        display: inline-block;
        width: 100%;
        @extend %ellipsis;
      }
    }
  }

  .sub-list-wrap{
    .list{
      margin-right: 40px;
      padding: 0;
      border: 0;
      text-align: left;
    }

    .text{
      color: $color-content-text;
    }

    &:after{
      content: "";
      flex: auto;
    }
  }
}

.scroll{
  /**
   * percent函数转换百分比
   * $content-height内容区域总高度
   * 页面标题栏高度：56
   */
  height: 93.8%;
  .upload-wrap{
    display: flex;
    align-items: center;
  }
  /deep/ .el-collapse-item{
    .img-list{
      margin: 20px 10px 0 0;
      cursor: pointer;
    }
  }
}

.input-group{
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;
  height: 50px;

  .list{
    width: 49%;
    margin-top: PXtoEm(20);

    .inp-wrap{
      display: inline-flex;
      align-items: center;
      height: 28px;
      width: 500px;
    }

    .text{
      width: 16%;
      display: inline-block;
    }

    &:nth-of-type(3n+1){
      justify-content: flex-start;
    }

    &:nth-of-type(3n){
      justify-content: flex-end;
    }
  }
}
.cause{
  display: flex;
  margin-top: 20px;
  align-items: center;

  .el-textarea{
    flex: 1;
    width: auto;
  }
}

// 列表
.list-bottom-wrap{
  margin-top: 10px;

  .list{
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    // @extend .con-padding-horizontal;
    background: rgba(186,186,186,.1);
    text-align: center;

    &:nth-of-type(odd){
      background: rgba(128,128,128,.1);
    }

    &.list-header{
      height: 50px;
      background: $color-content-table-header;
    }

    &:not(.list-header){
      padding: {
        top: 20px;
        bottom: 20px;
      }
    }
  }

  .list-con{
    display: flex;
    align-items: center;
    justify-content: center;
    width: 20%;

    &:first-of-type{
      justify-content: flex-start;
    }
  }

  .textarea-wrap{
    width: 100%;
    margin-top: 10px;
  }
}

// 提交底部按钮
.btn-group{
  // padding: 20px 0;
  text-align: center;
}

.lable{
  width: 100px;
}

.disabled{
  background: #8c939d;
}

.upload-list{
  margin-top: PXtoEm(25);
  margin-bottom: PXtoEm(25);
  width: 50%;
}
.left{
  text-indent: 9.5%
}
.eldatepicker{
  width:466px !important;
}
</style>
