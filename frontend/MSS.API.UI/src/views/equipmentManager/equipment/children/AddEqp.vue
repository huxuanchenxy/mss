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
                      :label="item.sub_code_name"
                      :value="item.sub_code">
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
                <span class="text">管辖班组</span>
                <div class="inp">
                  <el-cascader class="cascader_width" clearable
                    :props="defaultParams"
                    change-on-select
                    @change="cascader_change"
                    :show-all-levels="true"
                    :options="teamList">
                  </el-cascader>
                </div>
              </div>
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
                <span class="text">供应商<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-select v-model="supplier.text" clearable filterable placeholder="请选择" @change="validateSelect(supplier)">
                    <el-option
                      v-for="item in supplierList"
                      :key="item.key"
                      :label="item.name"
                      :value="item.id">
                    </el-option>
                  </el-select>
                </div>
              </div>
              <p class="validate-tips">{{ supplier.tips }}</p>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">制造商<em class="validate-mark">*</em></span>
                <div class="inp">
                  <el-select v-model="manufacturer.text" clearable filterable placeholder="请选择" @change="validateSelect(manufacturer)">
                    <el-option
                      v-for="item in manufacturerList"
                      :key="item.key"
                      :label="item.name"
                      :value="item.id">
                    </el-option>
                  </el-select>
                </div>
              </div>
              <p class="validate-tips">{{ manufacturer.tips }}</p>
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
                <span class="text">额定电压</span>
                <div class="inp">
                  <el-input placeholder="请输入额定电压" v-model="ratedVoltage.text" @keyup.native="validateDouble(ratedVoltage)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ ratedVoltage.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">额定电流</span>
                <div class="inp">
                  <el-input placeholder="请输入额定电流" v-model="ratedCurrent.text" @keyup.native="validateDouble(ratedCurrent)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ ratedCurrent.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">额定电压</span>
                <div class="inp">
                  <el-input placeholder="请输入额定电压" v-model="ratedPower.text" @keyup.native="validateDouble(ratedPower)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ ratedPower.tips }}</p>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">安装位置</span>
                <div class="inp">
                  <el-cascader class="cascader_width" clearable
                    change-on-select
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
                <span class="text">上线日期</span>
                <div class="inp">
                  <el-date-picker
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
                <span class="text">使用期限</span>
                <div class="inp">
                  <el-input placeholder="请输入使用期限" v-model="life.text" @keyup.native="validateInputNull(life)"></el-input>
                </div>
              </div>
              <p class="validate-tips">{{ life.tips }}</p>
            </li>
            <li class="list">
              <div class="upload-wrap con-padding-horizontal">
                <span class="text">设备图纸：</span>
                <el-upload class="el-upload-my-picture-card"
                  ref="upload"
                  :action="``"
                  accept="application/pdf"
                  :file-list="fileList"
                  :show-file-list="true"
                  list-type="picture-card"
                  :auto-upload="false"
                  :http-request="upload"
                  :on-change="onChange"
                  :on-preview="preview">
                  <i class="iconfont icon-pdf"></i>
                </el-upload>
              </div>
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
    <el-dialog
      :visible.sync="centerDialogVisible"
      :modal-append-to-body="false"
      custom-class="show-list-wrap"
      center>
      <iframe :src="previewUrl" width="100%" height="100%" frameborder="0"></iframe>
    </el-dialog>
  </div>
</template>
<script>
import { validateInputCommon, validateNumberCommon, vInput, vdouble3, PDF_IMAGE, PDF_BLOB_VIEW_URL, PDF_UPLOADED_VIEW_URL } from '@/common/js/utils.js'
import XButton from '@/components/button'
import apiAuth from '@/api/authApi'
import api from '@/api/eqpApi'
import apiOrg from '@/api/orgApi'
import apiArea from '@/api/AreaApi.js'
export default {
  name: 'AddEqp',
  components: {
    XButton
  },
  data () {
    return {
      fileList: [],
      needUpload: [],
      centerDialogVisible: false,
      previewUrl: '',
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
      team: '',
      teamList: [],
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
      supplier: {
        text: '',
        tips: ''
      },
      supplierList: [],
      manufacturer: {
        text: '',
        tips: ''
      },
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
      }
    }
  },
  created () {
    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 添加设备'
    } else if (this.isShow === 'edit') {
      this.loading = true
      this.title = '| 修改设备'
      // this.getActionGroup()
    }
    // 子系统加载
    apiAuth.getSubCode('sub_system').then(res => {
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
      this.supplierList = res.data.filter((item) => { return item.type === 0 })
      this.manufacturerList = res.data.filter((item) => { return item.type === 1 })
    }).catch(err => console.log(err))

    // 安装位置加载
    apiArea.SelectConfigAreaData().then(res => {
      this.areaList = res.data.dicAreaList
    }).catch(err => console.log(err))
  },
  methods: {
    upload (file) {
      this.needUpload.push(file.file)
    },
    onChange (file, fileList) {
      file.pdfUrl = file.url
      file.url = PDF_IMAGE
      this.fileList = []
      this.fileList.push(file)
    },
    preview (item) {
      this.centerDialogVisible = true
      if (item.status === 'success') {
        if (item.pdfUrl.indexOf('blob:') !== -1) {
          this.previewUrl = PDF_BLOB_VIEW_URL + item.pdfUrl
        } else {
          this.previewUrl = PDF_UPLOADED_VIEW_URL + item.pdfUrl
        }
      } else {
        this.previewUrl = PDF_BLOB_VIEW_URL + item.pdfUrl
      }
    },
    cascader_change (val) {
      let selectedTeam = val[val.length - 1]
      let obj = this.getCascaderObj(selectedTeam, this.teamList)
      if (obj.node_type === 3) {
        this.team = val[val.length - 1]
      }
    },
    getCascaderObj (val, opt) {
      for (let i = 0; i < opt.length; ++i) {
        let item = opt[i]
        if (val === item.id) {
          return item
        } else {
          if (item.children) {
            let ret = this.getCascaderObj(val, item.children)
            if (ret) {
              return ret
            }
          }
        }
      }
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
      let actionGroup = {
        group_name: this.eqpName.text,
        group_type: this.groupType.text,
        group_order: this.groupOrder.text,
        request_url: this.groupUrl.text,
        icon: this.groupIcon.text,
        active_icon: this.titleIcon.text
      }
      if (this.isShow === 'add') {
        // 添加设备
        api.AddEqp(actionGroup).then(res => {
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
              message: res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else if (this.isShow === 'edit') {
        actionGroup.id = this.eqpCode
        // 修改设备
        api.updateActionGroup(actionGroup).then(res => {
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
              message: res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      }
    },
    // 修改设备时获取设备资料
    getActionGroup () {
      api.getActionGroupByID(this.editEqpID).then(res => {
        this.loading = false
        let _res = res.data
        this.eqpCode = _res.id
        this.eqpName.text = _res.group_name
        this.groupType.text = _res.group_type.toString()
        this.groupOrder.text = _res.group_order.toString()
        this.groupUrl.text = _res.request_url
        this.groupIcon.text = _res.icon
        this.titleIcon.text = _res.active_icon
      }).catch(err => console.log(err))
    },
    // 验证
    validateInput (val) {
      validateInputCommon(val)
    },

    // 验证3位小数
    validateDouble (val) {
      if (!vdouble3(val.text)) {
        val.tips = '此项必须为最多三位小数的浮点数'
        return false
      } else {
        val.tips = ''
        return true
      }
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

    validateNumber () {
      validateNumberCommon(this.groupOrder)
    },

    validateAll () {
      if (!validateInputCommon(this.eqpCode)) return false
      if (!validateInputCommon(this.eqpName)) return false
      if (!this.validateSelect(this.subSystem)) return false
      if (!this.validateSelect(this.eqpType)) return false
      if (!this.validateInputNull(this.assetNo)) return false
      if (!this.validateInputNull(this.model)) return false
      if (!this.validateSelect()) return false
      return true
    }
  }
}
</script>
<style lang="scss" scoped>
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
.cascader_width{
  width: 100%!important;
}
.el-date-editor.el-input, .el-date-editor.el-input__inner{
  width: 93%;
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
