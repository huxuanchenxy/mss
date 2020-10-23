<template>
  <div>
    <el-upload
      action="http://127.0.0.1:8081/api/v1/Upload"
      :disabled="readOnly"
      :multiple="true"
      :data="myFileType"
      :headers="uploadHeaders"
      :file-list="fileList"
      :show-file-list="true"
      list-type="text"
      :on-success="onSuccess"
      :before-upload="beforeUploadVideo"
      :on-preview="preview"
      :on-remove="onRemove">
      <span class="text">{{label}}</span>
      <x-button class="active upload-btn" v-show="!readOnly">视频上传</x-button>
      <!-- <video  width="250" height="250" v-if="previewUrl !='' && videoFlag == false" :src="previewUrl" class="avatar" controls="controls">您的浏览器不支持视频播放</video>
          <i v-else-if="previewUrl =='' && videoFlag == false" class="el-icon-plus avatar-uploader-icon"></i>
        <el-progress v-if="videoFlag == true" type="line" :percentage="videoUploadPercent" style="margin-top:30px;"></el-progress>  -->
    </el-upload>
    <el-dialog title :visible.sync="vediocenterDialogVisible" width="30%" :modal-append-to-body="false" @close="closeDialog">
         <video :src="previewUrl" controls autoplay class="avatar" :ref="dialogVideo"
         width="100%"></video>
      </el-dialog>
  </div>
</template>
<script>
import { FILE_SERVER_PATH } from '@/common/js/utils.js'
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
      videoFlag: false,
      videoUploadPercent: 0,
      myFileType: {},
      uploadHeaders: {Authorization: ''},
      fileList: [],
      previewPartUrl: [],
      previewUrl: '',
      vediocenterDialogVisible: false
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
    this.$refs.btn.onclik(function () {
      if (this.$refs.dialogVideo.paused) {
        this.$refs.dialogVideo.play()
      } else {
        this.$refs.dialogVideo.pause()
      }
    })
  },
  methods: {
    onSuccess (response, file, fileList) {
      this.returnFileIDs(fileList)
    },
    onRemove (file, fileList) {
      this.videoFlag = false
      this.previewUrl = ''
      if (!this.readOnly) {
        this.returnFileIDs(fileList)
      }
    },
    // 上传进度条处理
    uploadVideoProcess (event, file, fileList) {
      this.videoFlag = true
      this.videoUploadPercent = file.percentage.toFixed(0)
    },
    // 上传之前做格式检查
    beforeUploadVideo (file) {
      const isLt10M = file.size / 1024 / 1024 < 10
      if (['video/mp4', 'video/ogg', 'video/flv', 'video/avi', 'video/wmv', 'video/rmvb'].indexOf(file.type) === -1) {
        this.$message.error('请上传正确的视频格式')
        return false
      }
      if (!isLt10M) {
        this.$message.error('上传视频大小不能超过10MB哦!')
        return false
      }
    },
    preview (item) {
      if (item.status === 'success') {
        var aPos = item.url.indexOf('/')
        var bPos = item.url.substring(aPos + 1).indexOf('/')
        var val1 = item.url.substring(aPos + 1, aPos + bPos + 1)
        if (val1 === '') {
          this.previewUrl = item.url
        } else {
          this.previewUrl = FILE_SERVER_PATH + item.url
        }
        this.videoFlag = true
        this.centerDialogVisible = false
        this.vediocenterDialogVisible = true
      }
    },
    getFile () {
      api.getUploadFileByIDs(this.fileIDs).then(res => {
        this.fileList = res.data
      }).catch(err => console.log(err))
    },
    closeDialog () {
      this.previewUrl = '' // 清空数据 关闭视频播放
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
