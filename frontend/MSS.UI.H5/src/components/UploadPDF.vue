<template>
  <div>
    <!-- <el-upload
      action="http://10.89.36.154:5801/eqpapi/Upload"
      :disabled="isDisabled"
      :headers="uploadHeaders"
      :multiple="true"
      :data="myData"
      :accept="currentExt"
      :file-list="currentFileList"
      :show-file-list="false"
      list-type="text"
      :before-upload="beforeUpload"
      :on-success="onSuccess">
      <span>{{label}}</span>
      <el-select v-show="!readOnly" ref="mySelect" v-model="myData.type" clearable filterable placeholder="请选择上传附件类型" class="upload-btn" @change="onChange">
        <el-option
          v-for="item in fileTypeList"
          :key="item.key"
          :label="item.name"
          :value="item.business_type">
        </el-option>
      </el-select>
      <el-button size="small" type="primary" class="btn" v-show="!readOnly" :loading="loading">点击上传</el-button>
    </el-upload> -->
    <div v-for="(item) in fileList" :key="item.key">
      <el-upload
        action="http://10.89.36.154:5801/eqpapi/Upload"
        :disabled="isDisabled"
        :file-list="item.list"
        :on-remove="onRemove"
        :on-preview="preview">
        <div class="type-title">{{item.typeName}}</div>
      </el-upload>
    </div>
    <el-dialog
      :visible.sync="centerDialogVisible"
      :modal-append-to-body="false"
      custom-class="show-list-wrap"
      center>
      <iframe :src="previewUrl" width="100%" height="100%" frameborder="0" v-show="!isVedio"></iframe>
      <video :src="previewUrl"
        v-show="isVedio"
        controls
        autoplay
        height="99%"></video>
    </el-dialog>
  </div>
</template>
<script>
import { PDF_BLOB_VIEW_URL, PDF_UPLOADED_VIEW_URL, FILE_SERVER_PATH } from '@/common/js/utils.js'
import api from '@/api/eqpApi'
import apiAuth from '@/api/authApi'
import XButton from '@/components/button'
import { downloadFile } from '@/common/js/UpDownloadFileHelper.js'
import { systemResource } from '@/common/js/dictionary.js'
export default {
  name: 'UploadPDF',
  components: {
    XButton
  },
  props: {
    // label: String,
    systemResource: Number,
    // fileType: Number,
    fileIDs: String,
    readOnly: Boolean,
    canDown: Boolean,
    unSelectedEntity: Number
    // ext: String
  },
  data () {
    return {
      isDisabled: this.readOnly,
      loading: false,
      label: '',
      myData: {
        type: '',
        systemResource: ''
      },
      myFileIDs: [],
      currentFileList: [],
      fileTypeList: [],
      uploadHeaders: {Authorization: ''},
      fileList: [],
      retFileList: [],
      previewUrl: '',
      centerDialogVisible: false,
      ext: {},
      currentExt: '',
      isVedio: false
    }
  },
  created () {
    apiAuth.getBusinessType(this.systemResource).then(res => {
      this.fileTypeList = res.data
      this.fileTypeList.map(val => {
        this.ext[val.business_type] = val.ext === null ? '' : val.ext.toLowerCase()
      })
    }).catch(err => console.log(err))
    this.label = this.readOnly ? '已上传文件列表：' : '附件类型'
  },
  watch: {
    fileIDs () {
      this.currentFileList = []
      this.fileList = []
      this.retFileList = []
      if (this.fileIDs !== '' && this.fileIDs !== null) {
        this.myFileIDs = []
        this.myFileIDs = JSON.parse(this.fileIDs)
        this.getFile()
      }
    }
  },
  mounted () {
    this.myData.systemResource = this.systemResource
    let token = window.sessionStorage.getItem('token')
    if (token) { // 判断是否存在token，如果存在的话，则每个http header都加上token
      this.uploadHeaders.Authorization = `Bearer ${token}`
    }
  },
  methods: {
    onChange () {
      if (this.fileList.length !== 0 && !this.uploadIsFinished(this.fileList[this.fileList.length - 1].list)) {
        this.$message({
          message: '此类型的文件还未上传完成，不可改变文件类型再次上传，请耐心等待',
          type: 'warning'
        })
      } else {
        let ret = this.fileList.some(val => {
          if (+val.type === +this.myData.type) {
            this.currentFileList = val.list
            // this.myFileIDs = this.fileList.concat()
            return true
          }
          return false
        })
        if (!ret) this.currentFileList = []
      }
      if (this.myData.type !== '') {
        this.currentExt = this.ext[this.myData.type]
      }
    },
    uploadIsFinished (fileList) {
      if (fileList.length !== 0) {
        let list = fileList
        if (list !== '' && list.length !== 0) {
          for (let i = 0; i < list.length; i++) {
            if (list[i].status !== 'success') {
              return false
            }
          }
        }
      }
      return true
    },
    beforeUpload (file) {
      if (file.size > 52428800) {
        this.$message({
          message: '单个文件不允许超过50M',
          type: 'warning'
        })
        return false
      }
      if (this.unSelectedEntity > 0) {
        let msg = '请选择' + this.intToStr(this.unSelectedEntity)
        this.$message({
          message: msg,
          type: 'warning'
        })
        return false
      }
      if (this.myData.type === '') {
        this.$message({
          message: '请选择所要上传的文件类型',
          type: 'warning'
        })
        return false
      }
      let ext = this.currentExt
      if (ext !== '') {
        let tmp = file.name.split('.')
        let myExt = tmp[tmp.length - 1]
        let arr = ext.split(',')
        for (let i = 0; i < arr.length; i++) {
          if (('.' + myExt.toLowerCase()) === arr[i]) {
            this.loading = true
            this.isDisabled = true
            return true
          }
        }
        this.$message({
          message: '此类型不支持扩展名为' + myExt + '的文件上传',
          type: 'warning'
        })
        return false
      }
      this.loading = true
      this.isDisabled = true
    },
    intToStr (i) {
      switch (i) {
        case systemResource.eqpType: return '设备类型'
        case systemResource.eqp: return '设备'
      }
    },
    onSuccess (response, file, fileList) {
      this.returnFileIDs(fileList, this.myData.type, true)
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
      if (fileList.length === 0) {
        this.fileList.some((item, index) => {
          if (+item.type === +file.type) {
            this.fileList.splice(index, 1)
            return true
          }
          return false
        })
        this.retFileList.some((item, index) => {
          if (+item.type === +file.type) {
            this.retFileList.splice(index, 1)
            return true
          }
          return false
        })
        this.myData.type = '' + file.type
      }
      this.currentFileList = fileList
      this.returnFileIDs(fileList, file.type)
    },
    preview (item) {
      let arr = item.name.split('.')
      let extTmp = arr[arr.length - 1]
      if (arr[arr.length - 1] === 'pdf') {
        if (item.status === 'success') {
          if (item.url.indexOf('blob:') !== -1) {
            this.previewUrl = PDF_BLOB_VIEW_URL + item.url
          } else {
            this.previewUrl = PDF_UPLOADED_VIEW_URL + item.url
          }
        } else {
          this.previewUrl = PDF_BLOB_VIEW_URL + item.url
        }
        this.isVedio = false
        this.centerDialogVisible = true
      } else if (extTmp === 'mp4' || extTmp === 'avi' || extTmp === 'flv' || extTmp === 'rmvb' || extTmp === 'ogg') {
        this.previewUrl = FILE_SERVER_PATH + item.url
        this.isVedio = true
        this.centerDialogVisible = true
      } else if (this.canDown) {
        downloadFile(item.id, item.name)
      }
    },
    getFile () {
      let allFileIds = []
      this.myFileIDs.map(val => {
        val.list.map(item => {
          allFileIds.push(item.id)
        })
      })
      api.getUploadFileByIDs(allFileIds.join(',')).then(res => {
        this.fileList = res.data
        this.fileList.map(val => {
          let ids = []
          val.list.map(item => {
            ids.push(item.id)
          })
          this.retFileList.push({
            type: val.type,
            ids: ids.join(',')
          })
        })
        // this.fileList = [{
        //   typeName: 'test1',
        //   list: [{
        //     id: 1,
        //     name: 'test11',
        //     url: 'path11',
        //     status: 'success'
        //   }, {
        //     id: 2,
        //     name: 'test12',
        //     url: 'path12',
        //     status: 'success'
        //   }]
        // }, {
        //   typeName: 'test2',
        //   list: [{
        //     id: 3,
        //     name: 'test21',
        //     url: 'path21',
        //     status: 'success'
        //   }, {
        //     id: 4,
        //     name: 'test22',
        //     url: 'path22',
        //     status: 'success'
        //   }]
        // }, {
        //   typeName: 'test3',
        //   list: [{
        //     id: 3,
        //     name: 'test31',
        //     url: 'path31',
        //     status: 'success'
        //   }, {
        //     id: 4,
        //     name: 'test32',
        //     url: 'path32',
        //     status: 'success'
        //   }]
        // }]
      }).catch(err => console.log(err))
    },
    returnFileIDs (fileList, type, isAdd) {
      let ids = []
      let list = fileList
      let retif = false
      //  && this.uploadIsFinished(fileList)
      if (list.length !== 0 && this.uploadIsFinished(fileList)) {
        list.map(val => {
          if (val.status === 'success' && val.url.indexOf('blob:') !== -1) {
            val.type = type
            val.id = val.response.data.id
          }
          ids.push(val.id)
        })
        retif = this.fileList.some((item, index) => {
          if (+item.type === +type) {
            if (isAdd) {
              this.myFileIDs.some((me, index1) => {
                if (+me.type === +type) {
                  if (me.list.length > 0) {
                    // me.list.map(val => {
                    //   ids.push(val.id)
                    // })
                    item.list = list.concat(item.list)
                    if (this.myData.type === type) {
                      this.currentFileList = item.list
                    } else {
                      this.myData.type = '' + type
                      this.currentFileList = list
                    }
                  }
                  return true
                }
                return false
              })
            }
            item.list = list
            this.myData.type = '' + type
            this.currentFileList = list
            return true
          }
          return false
        })
        if (!retif) {
          this.fileList.push({
            type: type,
            typeName: this.$refs['mySelect'].query,
            list: list
          })
        }
        retif = this.retFileList.some((item, index) => {
          if (+item.type === +type) {
            item.ids = ids.join(',')
            return true
          }
          return false
        })
        if (!retif) {
          this.retFileList.push({
            type: type,
            ids: ids.join(',')
          })
        }
      }
      this.loading = false
      this.isDisabled = false
      this.$emit('getFileIDs', this.retFileList)
    }
  }
}
</script>
<style lang="scss" scoped>
.upload-btn{
  margin-left:44px!important;
}

.type-title{
  text-align:left;
  margin-left:10px;
  margin-top:10px;
}

.btn{
  font-size: 14px;
  width: 85px;
  border-color: $color-main-btn!important;
  background: $color-main-btn!important;
  &:hover{
    border-color: $color-main-btn!important;
    background: $color-main-btn!important;
  }
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
