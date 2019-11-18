<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeTroubleList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">发生时间<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-date-picker
                  v-model="happeningTime.text"
                  type="datetime"
                  value-format="yyyy-MM-dd HH:mm:ss"
                  :clearable="false"
                  class="datetime-width"
                  placeholder="请选择发生时间">
                </el-date-picker>
              </div>
            </div>
            <p class="validate-tips">{{ happeningTime.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">报修时间<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-date-picker
                  v-model="reportedTime.text"
                  type="datetime"
                  value-format="yyyy-MM-dd HH:mm:ss"
                  :clearable="false"
                  class="datetime-width"
                  placeholder="请选择报修时间">
                </el-date-picker>
              </div>
            </div>
            <p class="validate-tips">{{ reportedTime.tips }}</p>
          </li>
          <li class="list"/>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">起始站点/区域<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-cascader filterable
                  change-on-select
                  @change="position_change"
                  :props="areaParams"
                  :show-all-levels="true"
                  :options="areaList"
                  v-model="areaStart.text">
                </el-cascader>
              </div>
            </div>
            <p class="validate-tips">{{ areaStart.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">结束站点</span>
              <div class="inp">
                <el-select v-model="areaEnd.text" clearable filterable placeholder="请选择结束站点" @change="station_change">
                  <el-option
                    v-for="item in areaEndList"
                    :key="item.key"
                    :label="item.areaName"
                    :value="item.id">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ areaEnd.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">抢修令号</span>
              <div class="inp">
                <el-input v-model="urgentOrder.text" placeholder="请输入抢修令号" @keyup.native="validateInputNull(urgentOrder)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ urgentOrder.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">接修单位<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="repairCompany.text" clearable filterable placeholder="请选择接修单位" @change="repair_change" value-key="id">
                  <el-option
                    v-for="item in repairCompanyList"
                    :key="item.id"
                    :label="item.name"
                    :value="item">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ repairCompany.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">故障设备<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="eqp.text" clearable filterable placeholder="请选择接修单位" multiple collapse-tags @change="eqp_change" value-key="id">
                  <el-option
                    v-for="item in eqpList"
                    :key="item.id"
                    :label="item.name"
                    :value="item">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ eqp.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">故障等级</span>
              <div class="inp">
                <el-select v-model="level" clearable filterable placeholder="请选择故障等级">
                  <el-option
                    v-for="item in levelList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.id">
                  </el-option>
                </el-select>
              </div>
            </div>
          </li>
          <li class="list" v-for="company in cardList" :key="company.id">
            <div class="inp-wrap">
              <span class="text"></span>
              <div class="inp">
                <el-card class="box-card">
                  <div slot="header" class="clearfix">
                    <span>{{company.companyName}}</span>
                  </div>
                  <div v-for="o in company.eqpList" :key="o.id" class="item">
                    {{o.name }}
                  </div>
                </el-card>
              </div>
            </div>
            <p class="validate-tips">{{ reportBy.tips }}</p>
          </li>
          <li class="list" v-show="cardList.length % 3 === 2 || cardList.length % 3 === 1"/>
          <li class="list" v-show="cardList.length % 3 === 1"/>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">报修单位<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-cascader class="cascader_width" clearable
                  @change="report_change"
                  change-on-select
                  :props="defaultParams"
                  :show-all-levels="true"
                  :options="reportCompanyList"
                  v-model="reportCompanyPath.text">
                </el-cascader>
              </div>
            </div>
            <p class="validate-tips">{{ reportCompanyPath.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">报修人<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="reportBy.text" clearable filterable placeholder="请选择报修人">
                  <el-option
                    v-for="item in reportByList"
                    :key="item.key"
                    :label="item.userName"
                    :value="item.userID">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ reportBy.tips }}</p>
          </li>
          <li class="list"/>
        </ul>
        <div class="con-padding-horizontal cause">
            <span class="lable">报修故障描述<em class="validate-mark">*(500字以内)</em></span>
            <el-input type="textarea" :rows="4" v-model="repairDesc.text" placeholder="请输入报修故障描述" @keyup.native="validateInput(repairDesc)"></el-input>
        </div>
        <div class="validate-tips left">{{ repairDesc.tips }}</div>
        <div class="con-padding-horizontal upload-list">
          <upload-pdf :systemResource="systemResource" :fileIDs="fileIDs" @getFileIDs="getFileIDs"></upload-pdf>
        </div>
        <!-- 按钮 -->
        <div class="btn-group">
          <x-button class="close">
            <router-link :to="{name: 'SeeTroubleList'}">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="save">保存</x-button>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vInput } from '@/common/js/utils.js'
import { systemResource, orgType, dictionary } from '@/common/js/dictionary.js'
import { isUploadFinished } from '@/common/js/UpDownloadFileHelper.js'
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
import apiArea from '@/api/AreaApi.js'
import apiOrg from '@/api/orgApi'
import apiEqp from '@/api/eqpApi'
import apiAuth from '@/api/authApi'
import MyUploadPDF from '@/components/UploadPDF'
export default {
  name: 'AddTrouble',
  components: {
    XButton,
    'upload-pdf': MyUploadPDF
  },
  data () {
    return {
      systemResource: systemResource.troubleReport,
      loading: false,
      title: '| 添加报修故障',
      happeningTime: {text: '', tips: ''},
      reportedTime: {text: '', tips: ''},
      areaParams: {
        label: 'areaName',
        value: 'id',
        children: 'children'
      },
      areaStart: {text: [], tips: ''},
      areaList: [],
      areaEnd: {text: '', tips: ''},
      areaEndList: [],
      urgentOrder: {text: '', tips: ''},
      repairCompany: {text: '', tips: ''},
      repairCompanyList: [],
      eqp: {text: [], tips: ''},
      eqpList: [],
      level: '',
      levelList: [],
      cardList: [],
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      reportCompanyPath: {text: [], tips: ''},
      reportCompanyList: [],
      reportBy: {text: '', tips: ''},
      reportByList: [],
      repairDesc: {text: '', tips: ''},
      fileIDs: '',
      fileIDsEdit: [],
      editID: ''
    }
  },
  created () {
    apiArea.SelectConfigAreaData().then(res => {
      this.areaList = res.data.dicAreaList
      this.init()
    }).catch(err => console.log(err))
    apiOrg.ListNodeByNodeType(orgType.company).then(res => {
      this.repairCompanyList = res.data
    }).catch(err => console.log(err))
    // apiAuth.getUserAll().then(res => {
    //   this.reportByList = res.data
    // }).catch(err => console.log(err))
    // 故障等级列表
    apiAuth.getSubCode(dictionary.troubleLevel).then(res => {
      this.levelList = res.data
    }).catch(err => console.log(err))
    // 报修单位加载
    apiOrg.getOrgAll().then(res => {
      this.reportCompanyList = res.data
    }).catch(err => console.log(err))
    // this.init()
  },
  methods: {
    report_change (val) {
      apiOrg.ListUserByNode(val[val.length - 1]).then(res => {
        this.reportByList = res.data
      }).catch(err => console.log(err))
    },
    eqp_change (val) {
      let eqpList = {}
      let index
      for (let i = 0; i < this.cardList.length; i++) {
        if (this.cardList[i].id === this.repairCompany.text.id) {
          eqpList = this.cardList[i]
          index = i
          break
        }
      }
      // let eqpList = this.cardList.find(item => {
      //   return item.id === this.repairCompany.text.id
      // })
      if (val.length !== 0) eqpList.eqpList = this.eqp.text
      else this.cardList.splice(index, 1)
      if (this.cardList.length === 0) this.initCardList()
    },
    repair_change (val) {
      this.eqp.text = []
      if (val !== '') {
        if (this.areaStart.text !== '' && this.areaStart.text.length !== 0) {
          this.repairCompany.tips = ''
          let l = this.areaStart.text.length
          let line = this.areaStart.text[0]
          let location = this.areaStart.text[l - 1]
          let locationBy = l - 2 > 0 ? (l - 2) : 0
          if (this.areaEnd.text !== '') {
            location = 0
            locationBy = 0
          }
          if (this.cardList[0].id === '') {
            this.cardList = []
            this.addNewRepairCompany(val)
          } else {
            let eqpList = this.cardList.find(item => {
              return item.id === val.id
            })
            if (eqpList === undefined) this.addNewRepairCompany(val)
          }
          apiEqp.getEqpByTopOrg(val.id, line, location, locationBy).then(res => {
            this.eqpList = res.data
          }).catch(err => console.log(err))
        } else {
          this.repairCompany.tips = '请选择起点站点/区域'
          this.repairCompany.text = ''
        }
      } else this.initCardList()
    },
    addNewRepairCompany (val) {
      this.cardList.push({
        id: val.id,
        companyName: val.name,
        eqpList: [{eqp: '', topOrg: val.id, name: '还未选择故障设备'}]
      })
    },
    initCardList () {
      this.cardList = [{
        id: '',
        companyName: '已选中的接修单位名称',
        eqpList: [{
          eqp: undefined,
          org: '',
          name: '已选中的故障设备名称列表'
        }]
      }]
    },
    station_change (val) {
      if (val !== '') {
        if (this.areaStart.text.length !== 3 || this.areaStart.text[1] !== 9) {
          this.areaEnd.text = ''
          this.areaEnd.tips = '起始为站点时才能选择结束站点'
        } else if (this.areaStart.text[2] === this.areaEnd.text) {
          this.areaEnd.text = ''
          this.areaEnd.tips = '起始站点和结束站点不能相同'
        } else this.areaEnd.tips = ''
      } else this.areaEnd.tips = ''
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
        return true
      }
    },
    init () {
      this.happeningTime.text = new Date()
      this.reportedTime.text = this.happeningTime.text
      this.initCardList()
      if (this.$route.params.type !== 'Add') {
        this.editID = this.$route.params.id
        this.loading = true
        this.getTroubleReport()
      }
    },
    strToIntArr (str) {
      let arr = str.split(',')
      let ret = []
      for (let i = 0; i < arr.length; i++) {
        ret.push(parseInt(arr[i]))
      }
      return ret
    },
    getTroubleReport () {
      api.getTroubleReportByID(this.editID).then(res => {
        this.loading = false
        let _res = res.data
        this.title = '| 修改报修故障-' + _res.code
        this.happeningTime.text = _res.happeningTime
        this.reportedTime.text = _res.reportedTime
        this.areaStart.text = this.strToIntArr(_res.startLocationPath)
        this.areaEnd.text = _res.endLocation === null ? '' : _res.endLocation
        this.urgentOrder.text = _res.urgentRepairOrder === null ? '' : _res.urgentRepairOrder
        this.cardList = JSON.parse(_res.repairCompany)
        this.level = _res.level === null ? '' : _res.level
        this.reportCompanyPath.text = this.strToIntArr(_res.reportedCompanyPath)
        this.report_change(this.reportCompanyPath.text)
        this.position_change()
        this.reportBy.text = _res.reportedBy
        this.repairDesc.text = _res.desc
        this.fileIDs = _res.uploadFiles
      }).catch(err => console.log(err))
    },
    getFileIDs (ids) {
      this.fileIDsEdit = ids
    },
    // 验证非法字符串，但允许为空
    validateInputNull (val) {
      if (!vInput(val.text)) {
        val.tips = '此项含有非法字符'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateInput () {
      if (this.repairDesc.length > 500) {
        this.repairDesc.text = '报修故障描述必须在500字以内'
        return false
      }
      return validateInputCommon(this.repairDesc)
    },
    validateInputAll () {
      console.log(this.cardList)
      if (this.happeningTime.text === null) { this.msg('发生时间必选'); return false } else
      if (this.reportedTime.text === null) { this.msg('报修时间必选'); return false } else
      if (this.areaStart.text.length === 0) { this.msg('起点站区/区域必选'); return false } else
      if (!this.validateInputNull(this.urgentOrder)) { this.msg('抢修号令验证失败,请查看提示信息'); return false } else
      if (this.reportCompanyPath.text.length === 0) { this.msg('报修单位必选'); return false } else
      if (this.reportBy.text === '') { this.msg('报修人必选'); return false } else
      if (!this.validateInput()) { this.msg('报修故障描述验证失败,请查看提示信息'); return false }
      // else if (this.repairCompany.text.length === 0) this.msg('接修单位必选')
      // else if (this.happeningTime.text) this.msg('故障设备必填')
      return true
    },
    msg (str) {
      this.$message({
        message: str,
        type: 'error'
      })
    },
    save () {
      if (!this.validateInputAll()) return
      if ((new Date(this.reportedTime.text).getTime() - new Date(this.happeningTime.text).getTime()) / (3600 * 1000) > 2) {
        this.$message({
          message: '故障必须在发生后两小时内登记',
          type: 'warning'
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
      let eqpIDs = []
      if (this.$route.params.type === 'Add') {
        this.cardList.map(val => {
          val.eqpList.map(item => {
            if (item.id !== undefined) {
              eqpIDs.push({
                eqp: item.id,
                org: item.topOrg
              })
            }
          })
        })
      } else {
        this.cardList.map(val => {
          val.eqpList.map(item => {
            if (item.id !== undefined) {
              eqpIDs.push({
                trouble: this.editID,
                eqp: item.id,
                org: item.topOrg
              })
            }
          })
        })
      }
      if (eqpIDs.length === 0) {
        this.msg('设备故障必选')
        return
      }
      let troubleReport = {
        HappeningTime: this.happeningTime.text,
        ReportedTime: this.reportedTime.text,
        Line: this.areaStart.text[0],
        StartLocation: this.areaStart.text[this.areaStart.text.length - 1],
        StartLocationBy: this.areaStart.text.length - 2,
        StartLocationPath: this.areaStart.text.join(','),
        EndLocation: this.areaEnd.text === '' ? null : this.areaEnd.text,
        EndLocationBy: this.areaEnd.text === '' ? null : 1,
        UrgentRepairOrder: this.urgentOrder.text,
        Eqps: JSON.stringify(eqpIDs),
        Level: this.level,
        ReportedCompany: this.reportCompanyPath.text[this.reportCompanyPath.text.length - 1],
        ReportedCompanyPath: this.reportCompanyPath.text.join(','),
        ReportedBy: this.reportBy.text,
        Desc: this.repairDesc.text,
        UploadFiles: this.fileIDsEdit.length === 0 ? '' : JSON.stringify(this.fileIDsEdit)
      }
      if (this.$route.params.type === 'Add') {
        api.SaveTroubleReport(troubleReport).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeTroubleList'})
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
        troubleReport.ID = this.editID
        api.UpdateTroubleReport(troubleReport).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeTroubleList'})
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
  height: percent($content-height - 56, $content-height);
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

  .list{
    width: 30%;
    margin-top: PXtoEm(20);

    .inp-wrap{
      display: flex;
      align-items: center;
    }

    .text{
      width: 28%;
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
  padding: 20px 0;
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
.datetime-width{
  width: 93%;
}
/deep/
.el-tag--small{
  display: none;
}
// 卡片
.item {
  margin-bottom: 10px;
}

.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}
// .clearfix:after {
//   clear: both
// }

.box-card {
  width: 200px;
  background-color: #29282E;
  color: $color-content-text
}
</style>
