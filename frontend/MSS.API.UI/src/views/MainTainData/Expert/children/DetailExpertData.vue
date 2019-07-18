<template>
  <div
    class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active">
        <router-link :to="{name:'ExpertDataList'}">返回</router-link>
      </x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <div class="con-padding-horizontal operation">
          <ul class="input-group">
            <li class="list">
              <div class="inp-wrap">
                <span class="text">关键字</span>
                <div class="inp">{{ExpertData.keyword}}</div>
              </div>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">标题</span>
                <div class="inp">{{ExpertData.Experttitle}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备类型</span>
                <div class="inp">{{ExpertData.deviceType}}</div>
              </div>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">上传部门</span>
                <div class="inp">{{ExpertData.deptName}}</div>
              </div>
            </li>
          </ul>
         <ul class="input-group">
        <li class="list1">
          <div class="inp-wrap">
            <span class="text">内容详情</span>
            <div class="inp" style="width:86.5%">
              <el-input type="textarea" v-model="ExpertData.content"  :rows="7"></el-input>
            </div>
          </div>
        </li>
      </ul>
        <!-- 上传图片列表 -->
          <br/>
        <div class="upload-wrap con-padding-horizontal">
          <upload-vedio :fileType="filevedioType" label="视频上传" :fileIDs="filevedioIDs" @getFileIDs="getvedioFileID" :readOnly="true"></upload-vedio>
        </div>
        <br/>
        <div class="upload-wrap con-padding-horizontal">
          <upload-pdf :fileType="fileattachType" label="附件上传" :fileIDs="fileattachIDs" @getFileIDs="getattachFileID" :readOnly="true"></upload-pdf>
        </div>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { FileType } from '@/common/js/utils.js'
import api from '@/api/ExpertApi'
import XButton from '@/components/button'
import UploadPDF from '@/components/UploadPDF'
import UploadVedio from '@/components/UploadVideo'
export default {
  name: 'AddExpertData',
  components: {
    XButton,
    'upload-pdf': UploadPDF,
    'upload-vedio': UploadVedio
  },
  data () {
    return {
      fileattachType: FileType.ExpertData_attach,
      filevedioType: FileType.ExpertData_vedio,
      title: '| 专家资料详情',
      loading: false,
      filevedioIDs: '',
      filevedioIDsEdit: '',
      fileattachIDs: '',
      fileattachIDsEdit: '',
      ExpertData: {
        id: this.$route.params.id,
        keyword: '',
        deviceType: '',
        Experttitle: '',
        deptName: '',
        content: ''
      }
    }
  },
  created () {
    this.getExpertData()
  },
  methods: {
    // 修改设备时获取设备资料
    getExpertData () {
      let id = this.ExpertData.id
      api.GetExpertDataById(id).then(res => {
        this.loading = false
        let _res = res.data
        this.ExpertData.keyword = _res.keyword
        this.ExpertData.Experttitle = _res.title
        this.ExpertData.content = _res.content
        this.ExpertData.deviceType = _res.deviceTypeName
        this.ExpertData.deptName = _res.deptname
        this.filevedioIDs = _res.video_file
        this.filevedioIDsEdit = _res.video_file
        this.fileattachIDs = _res.attch_file
        this.fileattachIDsEdit = _res.attch_file
      }).catch(err => console.log(err))
    },
    getvedioFileID (val) {
      this.filevedioIDsEdit = val
    },
    getattachFileID (val) {
      this.fileattachIDsEdit = val
    }
  }
}
</script>
<style lang="scss" scoped>
// 功能区
.operation{
  .input-group{
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list{
      width: 30%;
      margin-top: PXtoEm(25);

      span{
        width: 28%;
      }

      .inp-wrap{
        display: flex;
        align-items: center;
      }

      .inp-wrap-upload{
        // display: flex;
        align-items: center;
      }
    .upload-list{
      margin-bottom: PXtoEm(25);
    }
    }

    .list1{
      width: 100%;
      margin-top: PXtoEm(25);

      span{
        width: 8.3%;
      }
      .inp-wrap{
        display: flex;
        align-items: center;
      }
    }
  }
}
.btn-enter{
  margin: 15px 0;
  text-align: center;

  button{
    border-color: $color-main-btn;
    background: $color-main-btn;
  }
}
.cascader_width{
  width: 100%!important;
}
.el-date-width{
  width: 93%!important;
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
        background: url(../../../../common/images/icon-close.png) no-repeat 0 0/100% 100%;
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
