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
    readOnly: Boolean
  },
  data () {
    return {
      myFileType: {},
      uploadHeaders: {Authorization: ''},
      fileList: [],
      previewUrl: '',
      centerDialogVisible: false
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
    onSuccess (response, file, fileList) {
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
      if (!this.readOnly) {
        this.returnFileIDs(fileList)
      }
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
      this.$emit('getFileIDs', ids.join(','))
    }
  }
}
</script>
<style lang="scss" scoped>
.upload-btn{
  margin-left:50px!important;
}
</style>
