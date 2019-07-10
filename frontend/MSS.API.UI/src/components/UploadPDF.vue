<template>
  <el-upload
    action="http://10.89.36.204:5801/eqpapi/Upload"
    :multiple="true"
    :data="myFileType"
    :headers="uploadHeaders"
    accept="application/pdf"
    :file-list="fileList"
    :show-file-list="true"
    list-type="text"
    :on-success="onSuccess"
    :before-remove="beforeRemove"
    :on-preview="preview">
    <span class="text">{{label}}</span>
    <x-button class="active upload-btn">点击上传</x-button>
    <!--<i class="iconfont icon-pdf"></i>-->
  </el-upload>
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
    fileType: Number
  },
  data () {
    return {
      myFileType: {},
      uploadHeaders: {Authorization: ''},
      fileList: [],
      centerDialogVisible: false,
      previewUrl: ''
    }
  },
  mounted () {
    debugger
    this.myFileType.type = this.fileType
    let token = window.sessionStorage.getItem('token')
    if (token) { // 判断是否存在token，如果存在的话，则每个http header都加上token
      this.uploadHeaders.Authorization = `Bearer ${token}`
    }
  },
  methods: {
    onSuccess (response, file, fileList) {
      this.fileList = fileList
      this.fileList.map(val => {
        if (val.status === 'success' && val.url.indexOf('blob:') !== -1) {
          val.id = val.response.data.id
        }
      })
    },
    beforeRemove (file, fileList) {
      api.deleteUploadFile(file.id).then(res => {
        this.fileList = fileList
        this.fileList.map(val => {
          if (val.status === 'success') {
            val.id = val.response.data.id
          }
        })
        return res.code === 0
      }).catch(err => console.log(err))
    },
    preview (item) {
      this.centerDialogVisible = true
      if (item.status === 'success') {
        if (item.url.indexOf('blob:') !== -1) {
          this.previewUrl = PDF_BLOB_VIEW_URL + item.url
        } else {
          this.previewUrl = PDF_UPLOADED_VIEW_URL + item.url
        }
      } else {
        this.previewUrl = PDF_BLOB_VIEW_URL + item.url
      }
      this.$emit('preview', this.previewUrl)
    }
  }
}
</script>
<style lang="scss" scoped>

</style>
