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
                <span class="text">故障编号</span>
                <div class="inp">{{troubleReport.id}}</div>
              </div>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">故障类别</span>
                <div class="inp">{{troubleReport.typeName}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">故障时间</span>
                <div class="inp">{{troubleReport.happeningTime}}</div>
              </div>
            </li>
            <li class="list list-block">
              <div class="inp-wrap">
                <span class="text span-block">故障描述</span>
                <div class="inp">{{troubleReport.desc}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备图纸编号</span>
                <div class="inp">{{troubleReport.eqpCode}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备名称</span>
                <div class="inp">{{troubleReport.eqpName}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">报告人</span>
                <div class="inp">{{troubleReport.reportedByName}}</div>
              </div>
            </li>
            <li class="list list-block" >
              <div class="inp-wrap">
                <span class="text span-block">故障诊断</span>
                <div class="inp">{{troubleReport.solution}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">开单人</span>
                <div class="inp">{{troubleReport.createdByName}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">开单时间</span>
                <div class="inp">{{troubleReport.createdTime}}</div>
              </div>
            </li>
            <li class="list"/>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">接收人</span>
                <div class="inp">{{troubleReport.acceptedByName}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">接收时间</span>
                <div class="inp">{{troubleReport.acceptedTime}}</div>
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
  name: 'DetailTroubleReport',
  components: {
    XButton
  },
  data () {
    return {
      title: '| 故障报告单明细',
      loading: false,
      troubleReport: {
        id: this.$route.params.id,
        typeName: '',
        desc: '',
        eqpName: '',
        eqpCode: '',
        happeningTime: '',
        statusName: '',
        solution: '',
        reportedByName: '',
        createdByName: '',
        createdTime: '',
        acceptedByName: '',
        acceptedTime: ''
      }
    }
  },
  created () {
    this.getTroubleReport()
  },
  methods: {
    getTroubleReport () {
      api.getTroubleReportByID(this.troubleReport.id).then(res => {
        this.loading = false
        let _res = res.data
        this.troubleReport.typeName = _res.typeName
        this.troubleReport.desc = _res.desc
        this.troubleReport.eqpName = _res.eqpName
        this.troubleReport.eqpCode = _res.eqpCode
        this.troubleReport.happeningTime = transformDate(_res.happeningTime)
        this.troubleReport.statusName = _res.statusName
        this.troubleReport.solution = _res.solution
        this.troubleReport.reportedByName = _res.reportedByName
        this.troubleReport.createdByName = _res.createdByName
        this.troubleReport.createdTime = transformDate(_res.createdTime)
        this.troubleReport.acceptedTime = transformDate(_res.acceptedTime)
        this.troubleReport.acceptedByName = _res.acceptedByName
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
