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
        <router-link :to="{name:'SeeEqpList'}">返回</router-link>
      </x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <div class="con-padding-horizontal operation">
          <ul class="input-group">
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备图纸编码<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-input placeholder="请输入设备图纸编码" v-model="eqpCode.text" @keyup.native="validateInput(eqpCode)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ eqpCode.tips }}</p>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">设备名称<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-input placeholder="请输入设备名称" v-model="eqpName.text" @keyup.native="validateInput(eqpName)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ eqpName.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">子系统<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-select v-model="subSystem.text" clearable filterable placeholder="请选择" @change="validateSelect(subSystem)">
                    <el-option
                      v-for="item in subSystemList"
                      :key="item.key"
                      :label="item.name"
                      :value="item.id">
                    </el-option>
                  </el-select>
                </div>
              </div>
              <p class="validate-tips">{{ subSystem.tips }}</p>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">设备类型<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-select v-model="eqpType.text" clearable filterable placeholder="请选择" @change="validateSelect(eqpType)">
                    <el-option
                      v-for="item in eqpTypeList"
                      :key="item.key"
                      :label="item.tName"
                      :value="item.id">
                    </el-option>
                  </el-select>
                </div>
              </div>
              <p class="validate-tips">{{ eqpType.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备资产编码</span>
                <div class="inp">
                  <el-input placeholder="请输入设备资产编码" v-model="assetNo.text" @keyup.native="validateInputNull(assetNo)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ assetNo.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备规格型号</span>
                <div class="inp">
                  <el-input placeholder="请输入设备规格型号" v-model="model.text" @keyup.native="validateInputNull(model)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ model.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">管辖班组<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-cascader class="cascader_width" clearable ref='team'
                    expand-trigger="hover"
                    change-on-select
                    popper-class="pop-team"
                    :props="defaultParams"
                    @change="cascader_change"
                    :show-all-levels="true"
                    :options="teamList"
                    v-model="teamPath.text">
                  </el-cascader>
                </div>
              </div>
              <p class="validate-tips">{{ teamPath.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">条码</span>
                <div class="inp">
                  <el-input placeholder="请输入条码" v-model="barCode.text" @keyup.native="validateInputNull(barCode)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ barCode.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">描述</span>
                <div class="inp">
                  <el-input placeholder="请输入描述" v-model="desc.text" @keyup.native="validateInputNull(desc)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ desc.tips }}</p>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">供应商</span>
                <div class="inp">
                  <el-select v-model="supplier" clearable filterable placeholder="请选择">
                    <el-option
                      v-for="item in supplierList"
                      :key="item.key"
                      :label="item.name"
                      :value="item.id">
                    </el-option>
                  </el-select>
                </div>
              </div>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">制造商</span>
                <div class="inp">
                  <el-select v-model="manufacturer" clearable filterable placeholder="请选择">
                    <el-option
                      v-for="item in manufacturerList"
                      :key="item.key"
                      :label="item.name"
                      :value="item.id">
                    </el-option>
                  </el-select>
                </div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备序列号</span>
                <div class="inp">
                  <el-input placeholder="请输入设备序列号" v-model="serialNo.text" @keyup.native="validateInputNull(serialNo)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ serialNo.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">额定电压(v)</span>
                <div class="inp">
                  <el-input placeholder="请输入额定电压" v-model="ratedVoltage.text" @keyup.native="validateDouble(ratedVoltage)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ ratedVoltage.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">额定电流(A)</span>
                <div class="inp">
                  <el-input placeholder="请输入额定电流" v-model="ratedCurrent.text" @keyup.native="validateDouble(ratedCurrent)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ ratedCurrent.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">额定功率(KW)</span>
                <div class="inp">
                  <el-input placeholder="请输入额定功率" v-model="ratedPower.text" @keyup.native="validateDouble(ratedPower)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ ratedPower.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">安装位置<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-cascader class="cascader_width" clearable
                    change-on-select
                    @change="position_change"
                    :props="areaParams"
                    :show-all-levels="true"
                    :options="areaList"
                    v-model="area.text">
                  </el-cascader>
                </div>
              </div>
              <p class="validate-tips">{{ area.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">上线日期<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-date-picker class="el-date-width"
                    v-model="time.text"
                    type="date"
                    value-format="yyyy-MM-dd"
                    placeholder="请选择日期">
                  </el-date-picker>
                </div>
              </div>
              <p class="validate-tips">{{ time.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">使用年限<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-input placeholder="请输入使用期限" v-model="life.text" @keyup.native="validateNumber(life)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ life.tips }}</p>
            </li>
            <div class="upload-list">
              <upload-pdf :fileIDs="fileIDs" :systemResource="systemResource" @getFileIDs="getFileIDs"></upload-pdf>
            </div>
          </ul>
        </div>
        <div class="con-padding-horizontal header"/>
        <div class="con-padding-horizontal operation">
          <ul class="input-group">
            <li class="list">
              <div class="inp-wrap">
                <span class="text">中修频率(日)<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-input placeholder="请输入中修频率" v-model="mediumRepair.text" @keyup.native="validateNumber(mediumRepair)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ mediumRepair.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">大修频率(日)<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-input placeholder="请输入大修频率" v-model="largeRepair.text" @keyup.native="validateNumber(largeRepair)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ largeRepair.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">再次上线日期</span>
                <div class="inp">
                  <el-date-picker class="el-date-width"
                    v-model="timeAgain.text"
                    type="date"
                    value-format="yyyy-MM-dd"
                    placeholder="请选择日期">
                  </el-date-picker>
                </div>
              </div>
              <p class="validate-tips">{{ timeAgain.tips }}</p>
            </li>
          </ul>
        </div>
        <div class="btn-enter">
          <x-button class="close">
            <router-link :to="{ name: 'SeeEqpList' }">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="enter">保存</x-button>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
// import { validateInputCommon, validateNumberCommon, vInput, vdouble3, PDF_BLOB_VIEW_URL, PDF_UPLOADED_VIEW_URL, nullToEmpty, FileType } from '@/common/js/utils.js'
import { validateInputCommon, validateNumberCommon, vInput, vdouble3, nullToEmpty, strToIntArr, getCascaderObj } from '@/common/js/utils.js'
import { dictionary, firmType, systemResource } from '@/common/js/dictionary.js'
import { isUploadFinished } from '@/common/js/UpDownloadFileHelper.js'
import XButton from '@/components/button'
import MyUploadPDF from '@/components/UploadPDF'
import apiAuth from '@/api/authApi'
import api from '@/api/eqpApi'
import apiOrg from '@/api/orgApi'
import apiArea from '@/api/AreaApi.js'
export default {
  name: 'AddEqp',
  components: {
    XButton,
    'upload-pdf': MyUploadPDF
  },
  data () {
    return {
      systemResource: systemResource.eqp,
      isEditFile: false,
      fileIDs: '',
      fileIDsEdit: [],
      // fileType: FileType.Eqp_Drawings,
      areaParams: {
        label: 'areaName',
        value: 'id',
        children: 'children'
      },
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
        // disabled: 'node_type'
        // 'node_type' !== 3
      },
      title: '| 添加设备',
      loading: false,
      isShow: this.$route.params.mark,
      editEqpID: this.$route.params.id,
      eqpCode: {
        text: '',
        tips: ''
      },
      eqpName: {
        text: '',
        tips: ''
      },
      subSystem: {
        text: '',
        tips: ''
      },
      subSystemList: [],
      eqpType: {
        text: '',
        tips: ''
      },
      eqpTypeList: [],
      team: '',
      teamPath: {
        text: [],
        tips: ''
      },
      teamList: [],
      assetNo: {
        text: '',
        tips: ''
      },
      model: {
        text: '',
        tips: ''
      },
      barCode: {
        text: '',
        tips: ''
      },
      desc: {
        text: '',
        tips: ''
      },
      supplier: '',
      supplierList: [],
      manufacturer: '',
      manufacturerList: [],
      serialNo: {
        text: '',
        tips: ''
      },
      ratedVoltage: {
        text: '',
        tips: ''
      },
      ratedCurrent: {
        text: '',
        tips: ''
      },
      ratedPower: {
        text: '',
        tips: ''
      },
      area: {
        text: [],
        tips: ''
      },
      areaList: [],
      time: {
        text: '',
        tips: ''
      },
      life: {
        text: '',
        tips: ''
      },
      mediumRepair: {
        text: 0,
        tips: ''
      },
      largeRepair: {
        text: 0,
        tips: ''
      },
      timeAgain: {
        text: '',
        tips: ''
      }
    }
  },
  created () {
    // 子系统加载
    apiAuth.getSubCode(dictionary.subSystem).then(res => {
      this.subSystemList = res.data
    }).catch(err => console.log(err))

    // 设备类型加载
    api.getEqpTypeAll().then(res => {
      this.eqpTypeList = res.data
    }).catch(err => console.log(err))

    // 班组加载
    apiOrg.getOrgAll().then(res => {
      this.teamList = res.data
    }).catch(err => console.log(err))

    // 供应商/制造商加载
    api.getFirmAll().then(res => {
      this.supplierList = res.data.filter((item) => { return item.type === firmType.supplier })
      this.manufacturerList = res.data.filter((item) => { return item.type === firmType.manufacturer })
    }).catch(err => console.log(err))

    // 安装位置加载
    apiArea.SelectConfigAreaData().then(res => {
      this.areaList = res.data.dicAreaList
    }).catch(err => console.log(err))

    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 添加设备'
    } else if (this.isShow === 'edit') {
      this.loading = true
      this.title = '| 修改设备'
      this.getEqp()
    }
  },
  methods: {
    // visibleChange (parm) {
    //   console.log(parm)
    // },
    position_change () {
      if (this.area.text.length < 3) {
        this.area.tips = '设备必须安装在站点和区域上'
        this.area.text = []
        return false
      } else {
        this.area.tips = ''
        return true
      }
    },
    getFileIDs (ids) {
      this.fileIDsEdit = ids
      if (this.isShow === 'edit') {
        this.isEditFile = true
      }
    },
    // 班组下拉选中，过滤非班组
    cascader_change (val) {
      // console.log(val)
      let selectedTeam = val[val.length - 1]
      let obj = getCascaderObj(selectedTeam, this.teamList)
      if (obj.node_type === 3) {
        this.team = selectedTeam
        this.teamPath.tips = ''
      } else {
        this.teamPath.tips = '您选择的不是班组'
      }
      // let el = document.querySelector('.pop-team')
      // el.style.display = 'none'
    },
    // 添加设备
    enter () {
      if (!this.validateAll()) {
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
      let eqp = {
        Code: this.eqpCode.text,
        Name: this.eqpName.text,
        Type: this.eqpType.text,
        AssetNo: this.assetNo.text,
        Model: this.model.text,
        SubSystem: this.subSystem.text,
        Team: this.team,
        TeamPath: this.teamPath.text.join(','),
        TopOrg: this.teamPath.text[0],
        Line: this.area.text[0],
        BarCode: this.barCode.text,
        Desc: this.desc.text,
        Supplier: this.supplier,
        Manufacturer: this.manufacturer,
        SerialNo: this.serialNo.text,
        Online: this.time.text,
        Life: this.life.text,
        MediumRepair: this.mediumRepair.text,
        LargeRepair: this.largeRepair.text,
        OnlineAgain: this.timeAgain.text,
        FileIDs: this.fileIDsEdit.length === 0 && !this.isEditFile ? '' : JSON.stringify(this.fileIDsEdit)
      }
      // if (this.ratedVoltage.text !== '') {
      //   eqp.RatedVoltage = this.ratedVoltage.text
      // }
      // if (this.ratedCurrent.text !== '') {
      //   eqp.RatedCurrent = this.ratedCurrent.text
      // }
      // if (this.ratedPower.text !== '') {
      //   eqp.RatedPower = this.ratedPower.text
      // }
      // 新增了路线 为了和原来保持 再-1
      let l = this.area.text.length - 1
      if (l > -1) {
        eqp.Location = this.area.text[l]
        eqp.LocationBy = l - 1
        eqp.LocationPath = this.area.text.join(',')
      }
      if (this.isShow === 'add') {
        // 添加设备
        api.addEqp(eqp).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeEqpList'
                })
              }
            })
          } else {
            this.$message({
              message: res.msg === '' ? '添加失败' : res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else if (this.isShow === 'edit') {
        eqp.ID = this.editEqpID
        // 修改设备
        api.updateEqp(eqp).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeEqpList'
                })
              }
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
    // 修改设备时获取设备资料
    getEqp () {
      api.getEqpByID(this.editEqpID).then(res => {
        this.loading = false
        let _res = res.data
        this.eqpCode.text = _res.code
        this.eqpName.text = _res.name
        this.eqpType.text = _res.type
        this.subSystem.text = _res.subSystem
        this.team = _res.team
        this.supplier = _res.supplier
        this.manufacturer = _res.manufacturer
        this.teamPath.text = strToIntArr(_res.teamPath)
        this.assetNo.text = nullToEmpty(_res.assetNo)
        this.model.text = nullToEmpty(_res.model)
        this.barCode.text = nullToEmpty(_res.barCode)
        this.desc.text = nullToEmpty(_res.desc)
        this.serialNo.text = nullToEmpty(_res.serialNo)
        this.ratedVoltage.text = _res.ratedVoltage === null ? '' : _res.ratedVoltage
        this.ratedCurrent.text = _res.ratedCurrent === null ? '' : _res.ratedCurrent
        this.ratedPower.text = _res.ratedPower === null ? '' : _res.ratedPower
        this.area.text = strToIntArr(_res.locationPath)
        this.time.text = _res.online
        this.life.text = nullToEmpty(_res.life)
        this.mediumRepair.text = _res.mediumRepair
        this.largeRepair.text = _res.largeRepair
        this.timeAgain.text = nullToEmpty(_res.onlineAgain)
        this.fileIDs = _res.fileIDs
      }).catch(err => console.log(err))
    },
    // 验证
    validateInput (val) {
      return validateInputCommon(val)
    },

    // 验证3位小数
    validateDouble (val) {
      val.tips = ''
      if (val.text !== '') {
        if (!vdouble3(val.text)) {
          val.tips = '此项必须为最多三位小数的浮点数'
          return false
        } else {
          return true
        }
      }
      return true
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
    validateSelect (val) {
      if (val.text === '') {
        val.tips = '此项必选'
        return false
      } else {
        val.tips = ''
        return true
      }
    },

    validateNumberNull (val) {
      return validateNumberCommon(val)
    },
    validateNumber (val) {
      val.tips = ''
      if (val.text === '') {
        val.tips = '此项必填'
        return false
      } else {
        return validateNumberCommon(val)
      }
    },

    validateAll () {
      if (!validateInputCommon(this.eqpCode)) return false
      if (!validateInputCommon(this.eqpName)) return false
      if (!this.validateSelect(this.subSystem)) return false
      if (!this.validateSelect(this.eqpType)) return false
      if (!this.validateInputNull(this.assetNo)) return false
      if (!this.validateInputNull(this.model)) return false
      if (!this.validateInputNull(this.barCode)) return false
      if (!this.validateInputNull(this.desc)) return false
      if (!this.validateInputNull(this.serialNo)) return false
      if (!this.validateDouble(this.ratedVoltage)) return false
      if (!this.validateDouble(this.ratedCurrent)) return false
      if (!this.validateDouble(this.ratedPower)) return false
      if (this.teamPath.text.length === 0) {
        this.teamPath.tips = '此项必选'
        return false
      } else if (this.team === '') {
        this.teamPath.tips = '您选的并非是班组，请选择班组'
        return false
      }
      if (!this.position_change()) return false
      if (this.time.text === '') {
        this.time.tips = '此项必选'
        return false
      }
      if (!validateNumberCommon(this.life)) return false
      if (!this.validateNumber(this.mediumRepair)) return false
      if (!this.validateNumber(this.largeRepair)) return false
      return true
    }
  },
  mounted () {
    // console.log(this.$refs.team.id)
    // let el = document.querySelector('.pop-display')
    // console.log(el)
    // el.style.display = 'block'
  }

}
</script>
<style lang="scss" scoped>
.pop-display{
  display: block!important;
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

      .inp-wrap-upload{
        // display: flex;
        align-items: center;
      }

      &:nth-of-type(3n+1){
        // justify-content: flex-start;
      }

      &:nth-of-type(3n){
        // justify-content: flex-end;
      }
    }
    .upload-list{
      margin-top: PXtoEm(25);
      margin-bottom: PXtoEm(25);
      width: 50%;
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
.cascader_width{
  width: 100%!important;
}
// .el-date-editor.el-input, .el-date-editor.el-input__inner{
//   width: 80%!important;
// }
.el-date-width{
  width: 93%!important;
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
</style>
