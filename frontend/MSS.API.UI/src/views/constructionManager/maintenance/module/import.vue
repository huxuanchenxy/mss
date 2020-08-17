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
        <router-link :to="{name:'MaintenanceList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">模板编号<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入模板编号" v-model="mCode.text" @keyup.native="validateInput(mCode, true)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ mCode.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">模板名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入模板名称" v-model="mName.text" @keyup.native="validateInput(mName, true)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ mName.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">保存部门</span>
            <div class="inp">
              <el-input placeholder="请输入保存部门" v-model="department.text" @keyup.native="validateInputNull(department, true)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ department.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">保存年限</span>
            <div class="inp">
              <el-input placeholder="请输入保存年限" v-model="deathYear.text" @keyup.native="validateNumber(deathYear, true)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ deathYear.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">受限级别</span>
            <div class="inp">
              <el-select v-model="pmModule.level" clearable filterable placeholder="请选择" >
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
        <li class="list">
          <div class="inp-wrap">
            <span class="text">专业类别</span>
            <div class="inp">
              <el-select v-model="pmModule.major" clearable filterable placeholder="请选择">
                <el-option
                  v-for="item in majorList"
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
            <span class="text">地点</span>
            <div class="inp">
              <el-cascader clearable filterable class="cascader_width"
                change-on-select
                @change="position_change"
                :props="areaParams"
                :show-all-levels="true"
                :options="locationList"
                v-model="location.text">
              </el-cascader>
            </div>
          </div>
          <p class="validate-tips">{{ location.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">设备设施<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入设备设施" v-model="deviceName.text" @keyup.native="validateInput(deviceName, true)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ deviceName.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">关键字</span>
            <div class="inp">
              <el-input placeholder="请输入关键字" v-model="keyWord.text" @keyup.native="validateInputNull(keyWord, true)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ keyWord.tips }}</p>
        </li>
      </ul>
    </div>
    <div class="btn-enter">
      <!--:headers="uploadHeaders"-->
      <!--http://10.89.34.154:5801/workflowApi/Maintenance/Import-->
      <!--http://localhost:3851/api/v1/Maintenance/Import-->
      <el-upload
        action="http://10.89.34.154:5801/workflowApi/Maintenance/Import"
        :headers="uploadHeaders"
        :multiple="false"
        :data="pmModule"
        accept=".xls,.xlsx"
        :show-file-list="false"
        :before-upload="beforeUpload"
        :on-success="onSuccess">
        <el-button size="small" type="primary" class="import-btn" :loading="loading">导入</el-button>
      </el-upload>
    </div>
    <HotTable :settings="settings" licenseKey="non-commercial-and-evaluation" v-show="isShowHotTable" ref="hotTableComponent"/>
  </div>
</template>
<script>
import { validateInputCommon, validateNumberCommon, vInput } from '@/common/js/utils.js'
import { dictionary } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import apiAuth from '@/api/authApi'
import apiArea from '@/api/AreaApi.js'
import { HotTable } from '@handsontable/vue'
import Handsontable from 'handsontable'
export default {
  name: 'ImportPM',
  components: {
    XButton,
    HotTable
  },
  data () {
    return {
      settings: {
        data: Handsontable.helper.createSpreadsheetData(500, 50)
      },
      areaParams: {
        label: 'areaName',
        value: 'id',
        children: 'children'
      },
      isShowHotTable: false,
      loading: false,
      title: ' | 导入模板',
      uploadHeaders: {Authorization: ''},
      pmModule: {
        major: '',
        level: ''
      },
      mCode: {text: '', tips: ''},
      mName: {text: '', tips: ''},
      majorList: [],
      location: {text: [], tips: ''},
      locationList: [],
      deviceName: {text: '', tips: ''},
      keyWord: {text: '', tips: ''},
      department: {text: '', tips: ''},
      deathYear: {text: '', tips: ''},
      levelList: [],
      lineList: []
    }
  },
  created () {
    // 专业类别加载
    apiAuth.getSubCode(dictionary.pmMajor).then(res => {
      this.majorList = res.data
    }).catch(err => console.log(err))
    // 受控级别加载
    apiAuth.getSubCode(dictionary.pmLevel).then(res => {
      this.levelList = res.data
    }).catch(err => console.log(err))
    // 安装位置加载
    apiArea.SelectConfigAreaData().then(res => {
      this.locationList = res.data.dicAreaList
    }).catch(err => console.log(err))
  },
  mounted () {
    let token = window.sessionStorage.getItem('token')
    if (token) { // 判断是否存在token，如果存在的话，则每个http header都加上token
      this.uploadHeaders.Authorization = `Bearer ${token}`
    }
  },
  methods: {
    position_change (val) {
      let locationLength = val.length
      if (locationLength > 0) {
        if (locationLength < 3) {
          this.location.tips = '请选择站点或区域'
          this.location.text = []
          return false
        } else {
          this.location.tips = ''
          this.pmModule.line = val[0]
          this.pmModule.location = val[locationLength - 1]
          this.pmModule.locationBy = locationLength - 2
          this.pmModule.locationPath = val.join(',')
          return true
        }
      }
    },
    // 验证
    validateInput (val, isSet) {
      if (isSet) {
        switch (val) {
          case this.mCode:
            this.pmModule.code = val.text
            break
          case this.mName:
            this.pmModule.name = val.text
            break
          case this.department:
            this.pmModule.department = val.text
            break
          case this.deviceName:
            this.pmModule.deviceName = val.text
            break
        }
      }
      return validateInputCommon(val)
    },
    // 验证非法字符串，但允许为空
    validateInputNull (val, isSet) {
      if (isSet) {
        if (isSet) {
          switch (val) {
            case this.department:
              this.pmModule.department = val.text
              break
            case this.keyWord:
              this.pmModule.keyWord = val.text
              break
          }
        }
      }
      if (!vInput(val.text)) {
        val.tips = '此项含有非法字符'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    // validateSelect (val, isSet) {
    //   if (isSet) {
    //     this.pmModule.level = val.text
    //   }
    //   if (val.text === '') {
    //     val.tips = '此项必选'
    //     return false
    //   } else {
    //     val.tips = ''
    //     return true
    //   }
    // },
    validateNumber (val, isSet) {
      if (isSet) {
        this.pmModule.deathYear = val.text
      }
      return validateNumberCommon(val)
      // val.tips = ''
      // if (val.text === '') {
      //   val.tips = '此项必填'
      //   return false
      // } else {
      //   return validateNumberCommon(val)
      // }
    },
    validateAll () {
      if (!this.validateInput(this.mCode, false) || !this.validateInput(this.mName, false) || !this.validateInputNull(this.department, false) ||
      !this.validateNumber(this.deathYear, false) || !this.validateInput(this.deviceName, false) || !this.validateInputNull(this.keyWord, false)) return false
      else return true
    },
    beforeUpload (file) {
      // if (file.size > 52428800) {
      //   this.$message({
      //     message: '单个文件不允许超过50M',
      //     type: 'warning'
      //   })
      //   return false
      // }
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return false
      }
      let tmp = file.name.split('.')
      let myExt = tmp[tmp.length - 1]
      if (myExt !== 'xls' && myExt !== 'xlsx') {
        this.$message({
          message: '不支持非excel的文件类型',
          type: 'warning'
        })
        return false
      }
      this.pmModule = {
        code: this.mCode.text,
        name: this.mName.text,
        major: this.major,
        line: this.line,
        deviceName: this.deviceName.text,
        keyWord: this.keyWord.text,
        department: this.department.text,
        deathYear: this.deathYear.text,
        level: this.level
      }
      let locationLength = this.location.text.length
      if (locationLength !== 0) {
        this.pmModule.location = this.location.text[locationLength - 1]
        this.pmModule.locationBy = locationLength - 1
        this.pmModule.locationPath = this.location.text.join(',')
      }
      // this.loading = true
    },
    onSuccess (response, file, fileList) {
      if (response.code === 0) {
        let data = response.data
        this.settings = {
          readOnly: true,
          data: data.data,
          tableClassName: ['table01', 'htCenter'],
          width: '100%',
          height: 320,
          mergeCells: data.mergeCells,
          cells: function (row, col) {
            var cellProperties = {}
            if (col === 0) {
              cellProperties.width = 50
            }
            return cellProperties
          }
        }
        this.isShowHotTable = true
        this.$message({
          message: '导入成功',
          type: 'success'
        })
      } else {
        this.$message({
          message: response.msg,
          type: 'error'
        })
      }
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
.cascader-width{
  width: 220px!important;
}
.cascader_width{
  width: 100%!important;
}

/deep/
  .handsontableInput{
    color: #fff;
    background-color: #212028
  }
  /deep/
  .handsontable table.htCore{
    text-align: center
  }
  // /deep/
  // .handsontable .htDimmed{
  //   color: #FFF;
  //   background-color: #212028
  // }
  /deep/
  .handsontable td{
    color: #FFF;
    background-color: #212028
  }
  /deep/
  .htContextMenu{
    background-color: #212028
  }
  /deep/
  .htContextMenu table tbody tr td.htDisabled{
    background-color: #212028
  }
</style>
