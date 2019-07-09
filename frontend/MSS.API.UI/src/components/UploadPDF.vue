<template>
  <el-upload
    ref="upload"
    :action="``"
    :multiple="true"
    accept="application/pdf"
    :file-list="fileList"
    :show-file-list="true"
    list-type="text"
    :auto-upload="false"
    :http-request="upload"
    :on-change="onChange"
    :on-preview="preview">
    <span class="text">设备图纸</span>
    <x-button class="active upload-btn">点击上传</x-button>
  </el-upload>
</template>
<script>
export default {
  name: 'UploadPDF',
  props: {
    fileType: String,
    isShow: Boolean
  },
  data () {
    return {

    }
  }
}
</script>
<style lang="scss" scoped>

</style>
