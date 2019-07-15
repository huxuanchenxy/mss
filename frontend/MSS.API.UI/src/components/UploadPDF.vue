<template>
  <div>
    <el-upload
      action="http://10.89.36.204:5801/eqpapi/Upload"
      :disabled="readOnly"
      :multiple="true"
      :data="myFileType"
      :headers="uploadHeaders"
      accept="application/pdf"
      :file-list="fileList"
      :show-file-list="true"
      list-type="text"
      :before-upload="beforeUpload"
      :on-success="onSuccess"
      :on-remove="onRemove"
      :on-preview="preview">
      <span class="text">{{label}}</span>
      <x-button class="active upload-btn" v-show="!readOnly">点击上传</x-button>
    </el-upload>
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
import { PDF_BLOB_VIEW_URL, PDF_UPLOADED_VIEW_URL } from '@/common/js/utils.js'
import api from '@/api/eqpApi'
import XButton from '@/components/button'
export default {
  name: 'UploadPDF',
  components: {
    XButton
  },
  props: {
    label: String,
    fileType: Number,
    fileIDs: String,
    readOnly: Boolean,
    ext: String
  },
  data () {
    return {
      myFileType: {},
      uploadHeaders: {Authorization: ''},
      fileList: [],
      previewUrl: '',
      centerDialogVisible: false,
      isDeleted: false,
      isAdd: false
    }
  },
  watch: {
    fileIDs () {
      if (this.fileIDs !== '') {
        this.getFile()
      }
    }
  },
  mounted () {
    this.myFileType.type = this.fileType
    let token = window.sessionStorage.getItem('token')
    if (token) { // 判断是否存在token，如果存在的话，则每个http header都加上token
      this.uploadHeaders.Authorization = `Bearer ${token}`
    }
  },
  methods: {
    beforeUpload (file) {
      if (this.ext !== undefined && this.ext !== '') {
        let tmp = file.name.split('.')
        let myExt = tmp[tmp.length - 1]
        let arr = this.ext.split(',')
        for (let i = 0; i < arr.length; i++) {
          if (myExt === arr[i]) return true
        }
        this.$message({
          message: '不支持扩展名为' + myExt + '的文件上传',
          type: 'warning'
        })
        return false
      }
    },
    onSuccess (response, file, fileList) {
      this.isAdd = true
      this.returnFileIDs(fileList)
    },
    onRemove (file, fileList) {
      // api.deleteUploadFile(file.id).then(res => {
      //   this.fileList = fileList
      //   this.fileList.map(val => {
      //     if (val.status === 'success') {
      //       val.id = val.response.data.id
      //     }
      //   })
      //   return res.code === 0
      // }).catch(err => console.log(err))
      this.isDeleted = true
      this.returnFileIDs(fileList)
    },
    preview (item) {
      if (item.status === 'success') {
        if (item.url.indexOf('blob:') !== -1) {
          this.previewUrl = PDF_BLOB_VIEW_URL + item.url
        } else {
          this.previewUrl = PDF_UPLOADED_VIEW_URL + item.url
        }
      } else {
        this.previewUrl = PDF_BLOB_VIEW_URL + item.url
      }
      this.centerDialogVisible = true
    },
    getFile () {
      api.getUploadFileByIDs(this.fileIDs).then(res => {
        this.fileList = res.data
      }).catch(err => console.log(err))
    },
    returnFileIDs (fileList) {
      let ids = []
      this.fileList = fileList
      this.fileList.map(val => {
        if (val.status === 'success' && val.url.indexOf('blob:') !== -1) {
          val.id = val.response.data.id
        }
        ids.push(val.id)
      })
      // 有新增但没有删除过的时候 必须把原来上传的关系加上 这样 后台才能先把原有的关系删除 再加上修改的 就不会遗漏了
      if (this.isAdd && !this.isDeleted && this.fileIDs !== '') {
        ids.push(this.fileIDs)
      }
      this.$emit('getFileIDs', ids.join(','))
    }
  }
}
</script>
<style lang="scss" scoped>
.upload-btn{
  margin-left:50px!important;
}

// 图片预览
  /deep/ .show-list-wrap{
    width: 100% !important;
    height: 100%;

    .el-dialog__header{
      display: block;
      padding: 0;

      .el-dialog__headerbtn{
        top: 0!important;
        right: 0!important;
        z-index: 999;
        width: 27px;
        height: 27px;
        background: url(../common/images/icon-close.png) no-repeat 0 0/100% 100%;
      }

      .el-dialog__close{
        display: none;
      }
    }

    .el-dialog__body{
      width: 100%;
      height: 100%;
      padding: 0 !important;

      img{
        max-width: 100%;
        max-height: 100%;
      }
    }

    .el-carousel__item.is-animating{
      display: flex;
      justify-content: center;
      align-items: center;
    }

    // 左右箭头
    .el-carousel__arrow i{
      font-size: 20px;
    }
  }
</style>
