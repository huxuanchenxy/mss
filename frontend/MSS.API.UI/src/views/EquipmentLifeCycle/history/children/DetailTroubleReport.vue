<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <i @click="back"><x-button class="active">返回</x-button></i>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">发生时间</span>
              <div class="inp">{{troubleReport.happeningTime}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">报修时间</span>
              <div class="inp">{{troubleReport.reportedTime}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">状态</span>
              <div class="inp">{{troubleReport.statusName}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">起始站点/区域</span>
              <div class="inp">{{troubleReport.startLocationName}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">结束站点</span>
              <div class="inp">{{troubleReport.endLocationName}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">抢修令号</span>
              <div class="inp">{{troubleReport.urgentOrder}}</div>
            </div>
          </li>
          <li class="list" v-for="(company, index) in cardList" :key="company.id">
            <div class="inp-wrap">
              <span class="text" v-show="index===0">故障设备</span>
              <span class="text" v-show="index!==0"></span>
              <div class="inp">
                <el-card class="box-card">
                  <div slot="header" class="clearfix">
                    <span>{{company.companyName}}</span>
                  </div>
                  <div v-for="o in company.eqpList" :key="o.id" class="item">
                    {{o.name }}
                  </div>
                </el-card>
              </div>
            </div>
          </li>
          <li class="list" v-show="cardList.length % 3 === 2 || cardList.length % 3 === 1"/>
          <li class="list" v-show="cardList.length % 3 === 1"/>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">故障等级</span>
              <div class="inp">{{troubleReport.levelName}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">报修单位</span>
              <div class="inp">{{troubleReport.reportedCompanyName}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">报修人</span>
              <div class="inp">{{troubleReport.reportedByName}}</div>
            </div>
          </li>
          <li class="list"/>
        </ul>
        <div class="con-padding-horizontal cause">
            <span class="lable">报修故障描述</span>
            {{troubleReport.desc}}
        </div>
        <div class="con-padding-horizontal upload-list">
          <upload-pdf :systemResource="systemResource" :fileIDs="troubleReport.fileIDs" :readOnly="true"></upload-pdf>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import { systemResource } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
import MyUploadPDF from '@/components/UploadPDF'
export default {
  name: 'DetailTroubleReport',
  components: {
    XButton,
    'upload-pdf': MyUploadPDF
  },
  data () {
    return {
      systemResource: systemResource.troubleReport,
      loading: false,
      title: '| 故障报告单明细',
      cardList: [],
      troubleReport: {
        id: this.$route.params.id
      },
      fileIDs: ''
    }
  },
  created () {
    this.sourceName = this.$route.params.sourceName
    this.getTroubleReport()
  },
  methods: {
    back () {
      this.$router.push({
        name: this.sourceName
      })
    },
    getTroubleReport () {
      api.getTroubleReportByID(this.troubleReport.id).then(res => {
        this.loading = false
        let _res = res.data
        console.log(_res)
        this.title = '| 故障报修明细-' + _res.code
        this.troubleReport.desc = _res.desc
        this.troubleReport.happeningTime = transformDate(_res.happeningTime)
        this.troubleReport.reportedTime = transformDate(_res.reportedTime)
        this.troubleReport.startLocationName = _res.startLocationName
        this.troubleReport.endLocationName = _res.endLocationName === null ? '无' : _res.endLocationName
        this.troubleReport.urgentOrder = _res.urgentRepairOrder === null ? '无' : _res.urgentRepairOrder
        this.troubleReport.statusName = _res.statusName
        this.troubleReport.reportedByName = _res.reportedByName
        this.troubleReport.createdByName = _res.createdByName
        this.troubleReport.createdTime = transformDate(_res.createdTime)
        this.troubleReport.reportedCompanyName = _res.reportedCompanyName
        this.cardList = JSON.parse(_res.repairCompany)
        this.troubleReport.levelName = _res.levelName === null ? '无' : _res.levelName
        this.troubleReport.fileIDs = _res.uploadFiles
      }).catch(err => console.log(err))
    }
  }
}
</script>
<style lang="scss" scoped>
// 显示大图容器
.el-dialog__wrapper{
  width: 100%;
  height: 100%;

  .el-carousel{
    height: 100%;
  }
}

.header{
  display: flex;
  justify-content: space-between;
}

// 顶部信息
.middle{
  position: relative;
  margin-bottom: 10px;
  padding-bottom: 20px;

  .text-right{
    text-align: right !important;
  }

  [class*="list-wrap"]{
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list{
      margin-top: 20px;
      padding: 0 2%;
      border-right: 1px solid #C9CACD;
      text-align: center;

      &:first-of-type{
        padding-left: 0;
        text-align: left;
      }

      &:last-of-type{
        padding-right: 0;
        border: 0;
        text-align: right;
      }
    }
  }

  .list-wrap{
    .list{
      width: 16%;

      span{
        display: inline-block;
        width: 100%;
        @extend %ellipsis;
      }
    }
  }

  .sub-list-wrap{
    .list{
      margin-right: 40px;
      padding: 0;
      border: 0;
      text-align: left;
    }

    .text{
      color: $color-content-text;
    }

    &:after{
      content: "";
      flex: auto;
    }
  }
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

.input-group{
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;

  .list{
    width: 30%;
    margin-top: PXtoEm(20);

    .inp-wrap{
      display: flex;
      align-items: center;
    }

    .text{
      width: 28%;
    }

    &:nth-of-type(3n+1){
      justify-content: flex-start;
    }

    &:nth-of-type(3n){
      justify-content: flex-end;
    }
  }
}
.cause{
  display: flex;
  margin-top: 20px;
  align-items: center;

  .el-textarea{
    flex: 1;
    width: auto;
  }
}

// 列表
.list-bottom-wrap{
  margin-top: 10px;

  .list{
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    // @extend .con-padding-horizontal;
    background: rgba(186,186,186,.1);
    text-align: center;

    &:nth-of-type(odd){
      background: rgba(128,128,128,.1);
    }

    &.list-header{
      height: 50px;
      background: $color-content-table-header;
    }

    &:not(.list-header){
      padding: {
        top: 20px;
        bottom: 20px;
      }
    }
  }

  .list-con{
    display: flex;
    align-items: center;
    justify-content: center;
    width: 20%;

    &:first-of-type{
      justify-content: flex-start;
    }
  }

  .textarea-wrap{
    width: 100%;
    margin-top: 10px;
  }
}

// 提交底部按钮
.btn-group{
  padding: 20px 0;
  text-align: center;
}

.lable{
  width: 100px;
}

.disabled{
  background: #8c939d;
}

.upload-list{
  margin-top: PXtoEm(25);
  margin-bottom: PXtoEm(25);
  width: 50%;
}
.left{
  text-indent: 9.5%
}
.datetime-width{
  width: 93%;
}
/deep/
.el-tag--small{
  display: none;
}
// 卡片
.item {
  margin-bottom: 10px;
}

.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}
// .clearfix:after {
//   clear: both
// }

.box-card {
  width: 200px;
  background-color: #29282E;
  color: $color-content-text
}
</style>
