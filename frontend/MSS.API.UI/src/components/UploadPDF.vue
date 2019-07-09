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
  }
}
</script>
<style lang="scss" scoped>

</style>
