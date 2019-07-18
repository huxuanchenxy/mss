<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeEqpTypeList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">设备类型ID</span>
              <div class="inp disabled">
                <el-input v-model="eqpTypeID" :disabled="true"></el-input>
              </div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">设备类型名称<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input v-model="eqpTypeName.text"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ eqpTypeName.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">型号</span>
              <div class="inp">
                <el-input v-model="model.text"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ model.tips }}</p>
          </li>
          <li class="list">
            <upload-pdf ext="pdf" :fileType="fileType.EqpType_Working_Instruction" label="作业指导" :fileIDs="fileIDs.working" @getFileIDs="getWorkingFileIDs"></upload-pdf>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <div class="inp">
                <upload-pdf ext="pdf" :fileType="fileType.EqpType_Technical_Drawings" label="技术图纸" :fileIDs="fileIDs.drawings" @getFileIDs="getDrawingsFileIDs"></upload-pdf>
              </div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <div class="inp">
                <upload-pdf ext="pdf" :fileType="fileType.EqpType_Installation_Manual" label="安装手册" :fileIDs="fileIDs.install" @getFileIDs="getInstallFileIDs"></upload-pdf>
              </div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <div class="inp">
                <upload-pdf ext="pdf" :fileType="fileType.EqpType_User_Guide" label="使用手册" :fileIDs="fileIDs.user" @getFileIDs="getUserFileIDs"></upload-pdf>
              </div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <div class="inp">
                <upload-pdf ext="pdf" :fileType="fileType.EqpType_Regulations" label="维护规程" :fileIDs="fileIDs.regulations" @getFileIDs="getRegulationsFileIDs"></upload-pdf>
              </div>
            </div>
          </li>
          <li class="list"/>
        </ul>
        <div class="con-padding-horizontal cause">
          <span class="lable">设备类型描述：</span>
          <el-input type="textarea" v-model="eqpTypeDesc.text" placeholder="请输入设备类型描述"></el-input>
          <p class="validate-tips">{{ eqpTypeDesc.tips }}</p>
        </div>
        <!-- 按钮 -->
        <div class="btn-group">
          <x-button class="close">
            <router-link :to="{name: 'SeeEqpTypeList'}">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="save">保存</x-button>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vInput, nullToEmpty, FileType } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/eqpApi'
import MyUploadPDF from '@/components/UploadPDF'
export default {
  name: 'AddEqpType',
  components: {
    XButton,
    'upload-pdf': MyUploadPDF
  },
  data () {
    return {
      loading: false,
      title: '| 添加设备类型',
      eqpTypeID: '',
      eqpTypeName: {text: '', tips: ''},
      model: {text: '', tips: ''},
      eqpTypeDesc: {text: '', tips: ''},
      fileIDs: {
        working: '',
        drawings: '',
        install: '',
        user: '',
        regulations: ''
      },
      fileIDsEdit: [{
        Type: '',
        FileIDs: ''
      }, {
        Type: '',
        FileIDs: ''
      }, {
        Type: '',
        FileIDs: ''
      }, {
        Type: '',
        FileIDs: ''
      }, {
        Type: '',
        FileIDs: ''
      }],
      fileType: FileType,
      previewUrl: '',
      pageType: ''
    }
  },
  created () {
    // this.pageType = this.$route.query.type
    // this.title = this.pageType === 'Add' ? '| 添加设备类型' : '| 修改设备类型'
    this.init()
  },
  methods: {
    init () {
      if (this.$route.query.type !== 'Add') {
        this.title = '| 修改设备类型'
        this.loading = true
        this.getEqpType()
      }
    },
    getWorkingFileIDs (val) {
      this.fileIDsEdit[0].Type = FileType.EqpType_Working_Instruction
      this.fileIDsEdit[0].FileIDs = val
    },
    getDrawingsFileIDs (val) {
      this.fileIDsEdit[1].Type = FileType.EqpType_Technical_Drawings
      this.fileIDsEdit[1].FileIDs = val
    },
    getInstallFileIDs (val) {
      this.fileIDsEdit[2].Type = FileType.EqpType_Installation_Manual
      this.fileIDsEdit[2].FileIDs = val
    },
    getUserFileIDs (val) {
      this.fileIDsEdit[3].Type = FileType.EqpType_User_Guide
      this.fileIDsEdit[3].FileIDs = val
    },
    getRegulationsFileIDs (val) {
      this.fileIDsEdit[4].Type = FileType.EqpType_Regulations
      this.fileIDsEdit[4].FileIDs = val
    },
    getEqpType () {
      api.getEqpTypeByID(this.$route.query.id).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.eqpTypeID = data.id
          this.eqpTypeName.text = data.tName
          this.model.text = nullToEmpty(data.model)
          this.eqpTypeDesc.text = data.desc
          if (data.uploadFiles !== null) {
            let obj = JSON.parse(data.uploadFiles)
            let tmp = {
              working: [],
              drawings: [],
              install: [],
              user: [],
              regulations: []
            }
            obj.map(val => {
              if (val.type === FileType.EqpType_Working_Instruction) {
                tmp.working.push(val.file_id)
              } else if (val.type === FileType.EqpType_Technical_Drawings) {
                tmp.drawings.push(val.file_id)
              } else if (val.type === FileType.EqpType_Installation_Manual) {
                tmp.install.push(val.file_id)
              } else if (val.type === FileType.EqpType_User_Guide) {
                tmp.user.push(val.file_id)
              } else if (val.type === FileType.EqpType_Regulations) {
                tmp.regulations.push(val.file_id)
              }
            })
            this.fileIDs.working = tmp.working.join(',')
            this.fileIDs.drawings = tmp.drawings.join(',')
            this.fileIDs.install = tmp.install.join(',')
            this.fileIDs.user = tmp.user.join(',')
            this.fileIDs.regulations = tmp.regulations.join(',')
          }
        }
        this.loading = false
      }).catch(err => console.log(err))
    },
    validateInput () {
      if (!validateInputCommon(this.eqpTypeName)) {
        return false
      }
      if (!vInput(this.model.text)) {
        this.model.tips = '此项含有非法字符'
        return false
      } else {
        this.model.tips = ''
      }
      if (!vInput(this.eqpTypeDesc.text)) {
        this.eqpTypeDesc.tips = '此项含有非法字符'
        return false
      } else {
        this.eqpTypeDesc.tips = ''
      }
      return true
    },
    save () {
      if (!this.validateInput()) {
        return
      }
      for (let j = 0; j < this.fileIDsEdit.length; j++) {
        if (this.fileIDsEdit[j].FileIDs !== '') {
          let arr = this.fileIDsEdit[j].FileIDs.split(',')
          for (let i = 0; i < arr.length; i++) {
            if (arr[i] === '') {
              this.$message({
                message: '文件还未上传完成，请耐心等待',
                type: 'warning'
              })
              return
            }
          }
        }
      }
      let eqpType = {
        TName: this.eqpTypeName.text,
        Desc: this.eqpTypeDesc.text,
        Model: this.model.text,
        UploadFiles: JSON.stringify(this.fileIDsEdit)
      }
      if (this.$route.query.type === 'Add') {
        api.addEqpType(eqpType).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeEqpTypeList'})
            // this.$message({
            //   message: '保存成功',
            //   type: 'success'
            // })
          } else {
            this.$message({
              message: '保存失败',
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else {
        eqpType.ID = this.eqpTypeID
        api.updateEqpType(eqpType).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeEqpTypeList'})
            // this.$message({
            //   message: '保存成功',
            //   type: 'success'
            // })
          } else {
            this.$message({
              message: '保存失败',
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
</style>
