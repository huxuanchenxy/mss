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
        </ul>
        <div class="con-padding-horizontal cause">
          <span class="lable">设备类型描述：</span>
          <el-input type="textarea" v-model="eqpTypeDesc.text" placeholder="请输入设备类型描述"></el-input>
          <p class="validate-tips">{{ eqpTypeDesc.tips }}</p>
        </div>
        <!-- 上传图片列表 -->
        <div class="upload-wrap con-padding-horizontal">
          <span class="lable">作业指导书：</span>
          <el-upload
            ref="uploadWorking"
            :action="``"
            accept="application/pdf"
            :file-list="fileList.working"
            :show-file-list="true"
            list-type="picture-card"
            :auto-upload="false"
            :http-request="uploadWorking"
            :on-change="onChangeWorking"
            :on-preview="preview">
            <i class="iconfont icon-pdf"></i>
          </el-upload>
        </div>
        <div class="upload-wrap con-padding-horizontal">
          <span class="lable">技术图纸：</span>
          <el-upload
            ref="uploadDrawings"
            :action="``"
            accept="application/pdf"
            :file-list="fileList.drawings"
            :show-file-list="true"
            list-type="picture-card"
            :auto-upload="false"
            :http-request="uploadDrawings"
            :on-change="onChangeDrawings"
            :on-preview="preview">
            <i class="iconfont icon-pdf"></i>
          </el-upload>
        </div>
        <div class="upload-wrap con-padding-horizontal">
          <span class="lable">安装手册：</span>
          <el-upload
            ref="uploadInstall"
            :action="``"
            accept="application/pdf"
            :file-list="fileList.install"
            :show-file-list="true"
            list-type="picture-card"
            :auto-upload="false"
            :http-request="uploadInstall"
            :on-change="onChangeInstall"
            :on-preview="preview">
            <i class="iconfont icon-pdf"></i>
          </el-upload>
        </div>
        <div class="upload-wrap con-padding-horizontal">
          <span class="lable">使用手册：</span>
          <el-upload
            ref="uploadUser"
            :action="``"
            accept="application/pdf"
            :file-list="fileList.user"
            :show-file-list="true"
            list-type="picture-card"
            :auto-upload="false"
            :http-request="uploadUser"
            :on-change="onChangeUser"
            :on-preview="preview">
            <i class="iconfont icon-pdf"></i>
          </el-upload>
        </div>
        <div class="upload-wrap con-padding-horizontal">
          <span class="lable">维护规程：</span>
          <el-upload
            ref="uploadRegulations"
            :action="``"
            accept="application/pdf"
            :file-list="fileList.regulations"
            :show-file-list="true"
            list-type="picture-card"
            :auto-upload="false"
            :http-request="uploadRegulations"
            :on-change="onChangeRegulations"
            :on-preview="preview">
            <i class="iconfont icon-pdf"></i>
          </el-upload>
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
import { validateInputCommon, vInput, PDF_IMAGE, PDF_BLOB_VIEW_URL, PDF_UPLOADED_VIEW_URL } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/eqpApi'
export default {
  name: 'AddEqpType',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      title: '| 添加设备类型',
      eqpTypeID: '',
      eqpTypeName: {text: '', tips: ''},
      model: {text: '', tips: ''},
      eqpTypeDesc: {text: '', tips: ''},
      centerDialogVisible: false,
      fileList: {
        working: [],
        drawings: [],
        install: [],
        user: [],
        regulations: []
      },
      needUpload: {
        working: [],
        drawings: [],
        install: [],
        user: [],
        regulations: []
      },
      previewUrl: '',
      pageType: ''
    }
  },
  created () {
    // this.pageType = this.$route.query.type
    this.title = this.pageType === 'Add' ? '| 添加设备类型' : '| 修改设备类型'
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
    getEqpType () {
      api.getEqpTypeByID(this.$route.query.id).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.eqpTypeID = data.id
          this.eqpTypeName.text = data.tName
          this.model.text = data.model
          this.eqpTypeDesc.text = data.desc
          if (data.pWorking !== null) {
            this.fileList.working.push({
              url: PDF_IMAGE,
              pdfUrl: data.pWorking,
              status: 'success'
            })
          }
          if (data.pDrawings !== null) {
            this.fileList.drawings.push({
              url: PDF_IMAGE,
              pdfUrl: data.pDrawings,
              status: 'success'
            })
          }
          if (data.pInstall !== null) {
            this.fileList.install.push({
              url: PDF_IMAGE,
              pdfUrl: data.pInstall,
              status: 'success'
            })
          }
          if (data.pUser !== null) {
            this.fileList.user.push({
              url: PDF_IMAGE,
              pdfUrl: data.pUser,
              status: 'success'
            })
          }
          if (data.pRegulations !== null) {
            this.fileList.regulations.push({
              url: PDF_IMAGE,
              pdfUrl: data.pRegulations,
              status: 'success'
            })
          }
        } else {
          this.$message({
            message: res.msg,
            type: 'error'
          })
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
      let fd = new FormData()
      if (this.$refs.uploadWorking.uploadFiles.length > 0) {
        if (this.$refs.uploadWorking.uploadFiles[0].status !== 'success') {
          this.$refs.uploadWorking.submit()
          fd.append('file', this.needUpload.working[0])
          fd.append('PWorking', this.needUpload.working[0].name)
        }
      }
      if (this.$refs.uploadDrawings.uploadFiles.length > 0) {
        if (this.$refs.uploadDrawings.uploadFiles[0].status !== 'success') {
          this.$refs.uploadDrawings.submit()
          fd.append('file', this.needUpload.drawings[0])
          fd.append('PDrawings', this.needUpload.drawings[0].name)
        }
      }
      if (this.$refs.uploadInstall.uploadFiles.length > 0) {
        if (this.$refs.uploadInstall.uploadFiles[0].status !== 'success') {
          this.$refs.uploadInstall.submit()
          fd.append('file', this.needUpload.install[0])
          fd.append('PInstall', this.needUpload.install[0].name)
        }
      }
      if (this.$refs.uploadUser.uploadFiles.length > 0) {
        if (this.$refs.uploadUser.uploadFiles[0].status !== 'success') {
          this.$refs.uploadUser.submit()
          fd.append('file', this.needUpload.user[0])
          fd.append('PUser', this.needUpload.user[0].name)
        }
      }
      if (this.$refs.uploadRegulations.uploadFiles.length > 0) {
        if (this.$refs.uploadRegulations.uploadFiles[0].status !== 'success') {
          this.$refs.uploadRegulations.submit()
          fd.append('file', this.needUpload.regulations[0])
          fd.append('PRegulations', this.needUpload.regulations[0])
        }
      }
      fd.append('TName', this.eqpTypeName.text)
      fd.append('Desc', this.eqpTypeDesc.text)
      fd.append('Model', this.model.text)
      // for (let i = 0; i < this.needUpload.length; i++) {
      //   fd.append('file', this.needUpload[i])
      // }
      let config = {
        headers: {
          'Content-Type': 'multipart/form-data'
        },
        transformRequest: function (data, headers) {
          return data
        }
      }
      if (this.$route.query.type === 'Add') {
        api.addEqpType(fd, config).then(res => {
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
        fd.append('ID', this.eqpTypeID)
        api.updateEqpType(fd, config).then(res => {
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
    },
    uploadWorking (file) {
      this.needUpload.working.push(file.file)
    },
    uploadDrawings (file) {
      this.needUpload.drawings.push(file.file)
    },
    uploadInstall (file) {
      this.needUpload.install.push(file.file)
    },
    uploadUser (file) {
      this.needUpload.user.push(file.file)
    },
    uploadRegulations (file) {
      this.needUpload.regulations.push(file.file)
    },
    onChangeWorking (file, fileList) {
      file.pdfUrl = file.url
      file.url = PDF_IMAGE
      this.fileList.working = []
      this.fileList.working.push(file)
    },
    onChangeDrawings (file, fileList) {
      file.pdfUrl = file.url
      file.url = PDF_IMAGE
      this.fileList.drawings = []
      this.fileList.drawings.push(file)
    },
    onChangeInstall (file, fileList) {
      file.pdfUrl = file.url
      file.url = PDF_IMAGE
      this.fileList.install = []
      this.fileList.install.push(file)
    },
    onChangeUser (file, fileList) {
      file.pdfUrl = file.url
      file.url = PDF_IMAGE
      this.fileList.user = []
      this.fileList.user.push(file)
    },
    onChangeRegulations (file, fileList) {
      file.pdfUrl = file.url
      file.url = PDF_IMAGE
      this.fileList.regulations = []
      this.fileList.regulations.push(file)
    },
    preview (item) {
      this.centerDialogVisible = true
      // var urlbase = process.env.NODE_ENV === 'production' ? '' : '/api'
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
    // 关闭图片预览窗口
    closeDialog () {
      this.centerDialogVisible = false
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
