<template>
  <div>
    <div v-for="(item) in fileList" :key="item.key">
      <el-upload
        action="http://10.89.36.154:5801/eqpapi/Upload"
        :disabled="isDisabled"
        :file-list="item.list"
        :on-preview="preview">
        <div class="type-title">{{item.typeName}}</div>
      </el-upload>
    </div>
        <div class="pdf-box" v-if="showPDF">
      <svg t="1576214238668" @click="closepdf" class="closeicon" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="1885"><path d="M511.998977 961.610044c-248.306272 0-449.607998-201.307865-449.607998-449.614138S263.692704 62.389956 511.998977 62.389956c248.364601 0 449.610044 201.299679 449.610044 449.606974S760.363577 961.610044 511.998977 961.610044L511.998977 961.610044zM718.186989 380.921639c8.457626-8.462742 8.457626-22.202675 0-30.658254l-45.927005-45.871747c-8.459672-8.459672-22.138206-8.459672-30.599925 0L511.603981 434.44874 381.546879 304.391638c-8.459672-8.459672-22.1423-8.459672-30.599925 0l-45.927005 45.871747c-8.457626 8.455579-8.457626 22.195511 0 30.658254l130.057101 130.053008L305.019948 641.031748c-8.457626 8.455579-8.457626 22.140253 0 30.599925L350.946954 717.555609c8.457626 8.404414 22.140253 8.404414 30.599925 0l130.057101-130.057101L641.661082 717.555609c8.461719 8.404414 22.140253 8.404414 30.599925 0l45.927005-45.922912c8.457626-8.459672 8.457626-22.144346 0-30.599925L588.129888 510.97567 718.186989 380.921639 718.186989 380.921639z" p-id="1886"></path></svg>
        <pdf
        v-for="i in numPages"
        :key="i"
        :src="pdfSrc"
        :page="i">
        </pdf>
    </div>
  </div>
  
</template>
<script>
// import { PDF_BLOB_VIEW_URL, PDF_UPLOADED_VIEW_URL, FILE_SERVER_PATH } from '@/common/js/utils.js'
import api from '@/api/eqpApi'
import apiAuth from '@/api/authApi'
import XButton from '@/components/button'
import { downloadFile } from '@/common/js/UpDownloadFileHelper.js'
import { systemResource } from '@/common/js/dictionary.js'
import pdf from 'vue-pdf'
import CMapReaderFactory from 'vue-pdf/src/CMapReaderFactory.js'
export default {
  name: 'UploadPDF',
  metaInfo: {
      meta: [
      { name: 'viewport', content: 'width=device-width,initial-scale=1,minimum-scale=1,maximum-scale=2,user-scalable=yes' }
      ]
  },
  components: {
    XButton,
    pdf
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
      showPDF: false,
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
      previewUrl: '/File/25/29/7d948fb7-aad5-42ed-aa3e-1648bec4902a.pdf',
      centerDialogVisible: true,
      ext: {},
      currentExt: '',
      isVedio: false,
      numPages: undefined,
      // pdfSrc: '/File/25/29/411ea423-ada4-4ae9-85ff-5b374ee48de3.PDF', // pdf文件地址
      pdfSrc: '/File/25/29/a9c4aa8b-c566-4ad9-8316-19f4c1c31afe.pdf', // pdf文件地址
      // pdfSrc: '/File/25/29/baidu.pdf', // pdf文件地址
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
    // this.previewUrl = pdf.createLoadingTask(this.previewUrl)
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
    this.pdfSrc = pdf.createLoadingTask({ url: this.pdfSrc, CMapReaderFactory })
    this.pdfSrc.then(pdf => {
    this.numPages = pdf.numPages
    })
  },
  methods: {
    closepdf () {
      this.showPDF = false
      this.pdfSrc = undefined
    },
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
      // console.log(item)
      // let arr = item.name.split('.')
      // let extTmp = arr[arr.length - 1]
      // if (arr[arr.length - 1] === 'pdf') {
      //   if (item.status === 'success') {
      //     if (item.url.indexOf('blob:') !== -1) {
      //       this.previewUrl = PDF_BLOB_VIEW_URL + item.url
      //     } else {
      //       this.previewUrl = PDF_UPLOADED_VIEW_URL + item.url
      //     }
      //   } else {
      //     this.previewUrl = PDF_BLOB_VIEW_URL + item.url
      //   }
      //   this.isVedio = false
      //   // this.centerDialogVisible = true
      //   this.centerDialogVisible = false
      // } else if (extTmp === 'mp4' || extTmp === 'avi' || extTmp === 'flv' || extTmp === 'rmvb' || extTmp === 'ogg') {
      //   this.previewUrl = FILE_SERVER_PATH + item.url
      //   this.isVedio = true
      //   this.centerDialogVisible = true
      // } else if (this.canDown) {
      //   downloadFile(item.id, item.name)
      // }
      window.open(item.url, '_blank')
      // let _this = this
      // this.pdfSrc = item.url
      // this.showPDF = true
      // let lodingTask = pdf.createLoadingTask({ url: this.pdfSrc, CMapReaderFactory })
      // this.pdfSrc = lodingTask
      // console.log('this.pdfsrc:' + JSON.stringify(this.pdfSrc))
      // this.pdfSrc.then(pdf => {
      //   this.numPages = pdf.numPages

      // }).catch(function(err){
      //   console.log(err)
      //   _this.showPDF = false
      // })
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
.closeicon{
    width: 20px;
    position: fixed;
    z-index: 10;
    right: 0;
    top: 10rem;
}
.pdf-box{
    position: absolute;
    top: 0px;
    // bottom: 1px;
    z-index: 3000;
    -webkit-overflow-scrolling: touch;
}
  .el-icon-close{
    display:none !important;
  }
</style>
<style>

  .el-icon-upload-success{
    display:none !important;
  }
</style>