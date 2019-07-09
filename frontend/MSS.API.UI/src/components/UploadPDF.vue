<template>
  <el-upload
    action="http://localhost:3851/api/v1/Upload"
    :multiple="true"
    :data="fileType"
    :headers="uploadHeaders"
    accept="application/pdf"
    :file-list="fileList"
    :show-file-list="true"
    list-type="text"
    :on-success="onSuccess"
    :before-remove="beforeRemove"
    :on-preview="preview">
    <span class="text">label</span>
    <x-button class="active upload-btn">点击上传</x-button>
    <!--<i class="iconfont icon-pdf"></i>-->
  </el-upload>
</template>
<script>
export default {
  name: 'UploadPDF',
  props: {
    label: String,
    fileType: Number,
    isShow: Boolean
  },
  data () {
    return {
      fileType: {type: fileType},
      uploadHeaders: {Authorization: ''},
      fileList: [],
      centerDialogVisible: false,
      previewUrl: ''
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
      console.log(this.fileList)
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
    }
  }
}
</script>
<style lang="scss" scoped>

</style>
