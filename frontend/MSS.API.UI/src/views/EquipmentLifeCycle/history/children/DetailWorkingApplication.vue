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
        <router-link :to="{name:'SeeHistory'}">返回</router-link>
      </x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <div class="con-padding-horizontal operation">
          <ul class="input-group">
            <li class="list">
              <div class="inp-wrap">
                <span class="text">日常/重大</span>
                <div class="inp">{{workingApplication.type}}</div>
              </div>
            </li>
            <li class="list"/>
            <li class="list"/>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">申请单位</span>
                <div class="inp">{{workingApplication.appPartName}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">申报人</span>
                <div class="inp">{{workingApplication.applicantName}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">联系电话</span>
                <div class="inp">{{workingApplication.applicantName}}</div>
              </div>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">工作单位</span>
                <div class="inp">{{workingApplication.workingPartName}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">负责人</span>
                <div class="inp">{{workingApplication.managerName}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">联系电话</span>
                <div class="inp">{{workingApplication.managerMobile}}</div>
              </div>
            </li>
            <li class="list list-block">
              <div class="inp-wrap">
                <span class="text span-block">工作地点</span>
                <div class="inp">{{workingApplication.workingLocation}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">登记车站</span>
                <div class="inp">{{workingApplication.regStationName}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备图纸编号</span>
                <div class="inp">{{workingApplication.eqpCode}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备名称</span>
                <div class="inp">{{workingApplication.eqpName}}</div>
              </div>
            </li>
            <li class="list list-block" >
              <div class="inp-wrap">
                <span class="text span-block">起止时间</span>
                <div class="inp">{{workingApplication.startTime}}&#12288;&#12288;至&#12288;&#12288;{{workingApplication.endTime}}</div>
              </div>
            </li>
            <li class="list list-block" >
              <div class="inp-wrap">
                <span class="text span-block">施工内容</span>
                <div class="inp">{{workingApplication.content}}</div>
              </div>
            </li>
            <li class="list list-block">
              <div class="inp-wrap">
                <span class="text span-block">施工详细说明</span>
                <div class="inp">{{workingApplication.detail}}</div>
              </div>
            </li>
            <li class="list list-block">
              <div class="inp-wrap">
                <span class="text span-block">配合要求</span>
                <div class="inp">{{workingApplication.coordinationRequirements}}</div>
              </div>
            </li>
            <li class="list list-block">
              <div class="inp-wrap">
                <span class="text span-block">配合意见</span>
                <div class="inp">{{workingApplication.coordinationSuggestions}}</div>
              </div>
            </li>
            <li class="list list-block">
              <div class="inp-wrap">
                <span class="text span-block">影响区段</span>
                <div class="inp">{{workingApplication.influencingSection}}</div>
              </div>
            </li>
            <li class="list list-block">
              <div class="inp-wrap">
                <span class="text span-block">影响说明</span>
                <div class="inp">{{workingApplication.influencingDescription}}</div>
              </div>
            </li>
            <li class="list list-block">
              <div class="inp-wrap">
                <span class="text span-block">安全措施</span>
                <div class="inp">{{workingApplication.securityMeasures}}</div>
              </div>
            </li>
            <li class="list list-block">
              <div class="inp-wrap">
                <span class="text span-block">批复意见</span>
                <div class="inp">{{workingApplication.responseSuggestions}}</div>
              </div>
            </li>
            <li class="list list-last">
              <div class="inp-wrap">
                <span class="text">批复人</span>
                <div class="inp">{{workingApplication.responseByName}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">批复时间</span>
                <div class="inp">{{workingApplication.responseTime}}</div>
              </div>
            </li>
            <li class="list"/>
          </ul>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
export default {
  name: 'DetailWorkingApplication',
  components: {
    XButton
  },
  data () {
    return {
      title: '| 施工申请单明细',
      loading: false,
      workingApplication: {
        id: this.$route.params.id,
        type: '',
        appPartName: '',
        applicantName: '',
        workingPartName: '',
        managerName: '',
        managerMobile: '',
        workingLocation: '',
        regStationName: '',
        startTime: '',
        endTime: '',
        eqpCode: '',
        eqpName: '',
        content: '',
        detail: '',
        coordinationRequirements: '',
        coordinationSuggestions: '',
        influencingSection: '',
        influencingDescription: '',
        securityMeasures: '',
        responseSuggestions: '',
        responseByName: '',
        responseTime: ''
      }
    }
  },
  created () {
    this.getWorkingApplication()
  },
  methods: {
    getWorkingApplication () {
      api.getWorkingApplicationByID(this.workingApplication.id).then(res => {
        this.loading = false
        let _res = res.data
        this.workingApplication.type = _res.type
        this.workingApplication.appPartName = _res.appPartName
        this.workingApplication.applicantName = _res.applicantName
        this.workingApplication.workingPartName = _res.workingPartName
        this.workingApplication.managerName = _res.managerName
        this.workingApplication.managerMobile = _res.managerMobile
        this.workingApplication.workingLocation = _res.workingLocation
        this.workingApplication.regStationName = _res.regStationName
        this.workingApplication.startTime = transformDate(_res.startTime)
        this.workingApplication.endTime = transformDate(_res.endTime)
        this.workingApplication.eqpCode = _res.eqpCode
        this.workingApplication.eqpName = _res.eqpName
        this.workingApplication.content = _res.content
        this.workingApplication.detail = _res.detail
        this.workingApplication.coordinationRequirements = _res.coordinationRequirements
        this.workingApplication.coordinationSuggestions = _res.coordinationSuggestions
        this.workingApplication.influencingSection = _res.influencingSection
        this.workingApplication.influencingDescription = _res.influencingDescription
        this.workingApplication.securityMeasures = _res.securityMeasures
        this.workingApplication.responseSuggestions = _res.responseSuggestions
        this.workingApplication.responseTime = transformDate(_res.responseTime)
        this.workingApplication.responseByName = _res.responseByName
      }).catch(err => console.log(err))
    }
  }
}
</script>
<style lang="scss" scoped>
.header{
  display: flex;
  justify-content: space-between;
}
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
        width: 27%;
      }

      .inp-wrap{
        display: flex;
        align-items: center;
        .inp{
          max-width: 90%;
          word-wrap: break-word;
        }
      }

      .inp-wrap-upload{
        // display: flex;
        align-items: center;
      }

      &:nth-of-type(3n+1){
        // justify-content: flex-start;
      }

      &:nth-of-type(3n){
        // justify-content: flex-end;
      }
    }
    .list-last{
      margin-bottom: PXtoEm(25);
    }
    .list-block{
      width: 100%;
      .span-block{
        width: 8%;
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
// .el-date-editor.el-input, .el-date-editor.el-input__inner{
//   width: 80%!important;
// }
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
