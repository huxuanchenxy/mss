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
      <i @click="back"><x-button class="active">返回</x-button></i>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <div class="con-padding-horizontal operation">
          <ul class="input-group">
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备图纸编码</span>
                <div class="inp">{{eqp.eqpCode}}</div>
              </div>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">设备名称</span>
                <div class="inp">{{eqp.eqpName}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">子系统</span>
                <div class="inp">{{eqp.subSystem}}</div>
              </div>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">设备类型</span>
                <div class="inp">{{eqp.eqpType}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备资产编码</span>
                <div class="inp">{{eqp.assetNo}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备规格型号</span>
                <div class="inp">{{eqp.model}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">管辖班组</span>
                <div class="inp">{{eqp.team}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">条码</span>
                <div class="inp">{{eqp.barCode}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">描述</span>
                <div class="inp word-break">{{eqp.desc}}</div>
              </div>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">供应商</span>
                <div class="inp">{{eqp.supplier}}</div>
              </div>
            </li>
            <li class="list" >
              <div class="inp-wrap">
                <span class="text">制造商</span>
                <div class="inp">{{eqp.manufacturer}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">设备序列号</span>
                <div class="inp">{{eqp.serialNo}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">额定电压(V)</span>
                <div class="inp">{{eqp.ratedVoltage}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">额定电流(A)</span>
                <div class="inp">{{eqp.ratedCurrent}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">额定功率(KW)</span>
                <div class="inp">{{eqp.ratedPower}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">安装位置</span>
                <div class="inp">{{eqp.area}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">上线日期</span>
                <div class="inp">{{eqp.time}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">使用期限</span>
                <div class="inp">{{eqp.life}}</div>
              </div>
            </li>
            <li class="upload-list">
              <div>
                <upload-pdf :canDown="true" :fileIDs="eqp.fileIDs" :readOnly="true" :systemResource="systemResource"></upload-pdf>
              </div>
            </li>
          </ul>
        </div>
        <div class="con-padding-horizontal header"/>
        <div class="con-padding-horizontal operation">
          <ul class="input-group">
            <li class="list">
              <div class="inp-wrap">
                <span class="text">中修频率</span>
                <div class="inp">{{eqp.mediumRepair}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">大修频率</span>
                <div class="inp">{{eqp.largeRepair}}</div>
              </div>
            </li>
            <li class="list">
              <div class="inp-wrap">
                <span class="text">再次上线日期</span>
                <div class="inp">{{eqp.timeAgain}}</div>
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
import { FileType, transformDateNoTime } from '@/common/js/utils.js'
import { systemResource } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import MyUploadPDF from '@/components/UploadPDF'
import api from '@/api/eqpApi'
export default {
  name: 'DetailEqp',
  components: {
    XButton,
    'upload-pdf': MyUploadPDF
  },
  data () {
    return {
      systemResource: systemResource.eqp,
      fileIDs: '',
      fileType: FileType.Eqp_Drawings,
      title: '| 设备明细',
      sourceName: this.$route.params.sourceName,
      eqpSelected: this.$route.params.eqpSelected,
      eqpType: this.$route.params.eqpType,
      loading: false,
      eqp: {
        id: this.$route.params.id,
        eqpCode: '',
        eqpName: '',
        subSystem: '',
        eqpType: '',
        team: '',
        assetNo: '',
        model: '',
        barCode: '',
        desc: '',
        supplier: '',
        manufacturer: '',
        serialNo: '',
        ratedVoltage: '',
        ratedCurrent: '',
        ratedPower: '',
        area: '',
        time: '',
        life: '',
        mediumRepair: '',
        largeRepair: '',
        timeAgain: ''
      }
    }
  },
  created () {
    this.getEqp()
  },
  methods: {
    back () {
      if (this.sourceName === 'SeeHistory') {
        this.$router.push({
          name: 'SeeHistory',
          params: {
            eqpSelected: this.eqpSelected,
            eqpType: this.eqpType
          }
        })
      } else {
        this.$router.push({
          name: this.sourceName
        })
      }
    },
    // 修改设备时获取设备资料
    getEqp () {
      api.getEqpDetailByID(this.eqp.id).then(res => {
        this.loading = false
        let _res = res.data
        this.eqpType = _res.type
        this.eqp.eqpCode = _res.code
        this.eqp.eqpName = _res.name
        this.eqp.eqpType = _res.tName
        this.eqp.subSystem = _res.subSystemName
        this.eqp.team = _res.teamName
        this.eqp.assetNo = _res.assetNo
        this.eqp.model = _res.model
        this.eqp.barCode = _res.barCode
        this.eqp.desc = _res.desc
        this.eqp.supplier = _res.supplierName
        this.eqp.manufacturer = _res.manufacturerName
        this.eqp.serialNo = _res.serialNo
        this.eqp.ratedVoltage = _res.ratedVoltage
        this.eqp.ratedCurrent = _res.ratedCurrent
        this.eqp.ratedPower = _res.ratedPower
        this.eqp.area = _res.locationName
        this.eqp.time = transformDateNoTime(_res.online)
        this.eqp.life = _res.life
        this.eqp.mediumRepair = _res.mediumRepair
        this.eqp.largeRepair = _res.largeRepair
        this.eqp.timeAgain = transformDateNoTime(_res.onlineAgain)
        this.eqp.fileIDs = _res.fileIDs
      }).catch(err => console.log(err))
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

      &:nth-of-type(3n+1){
        // justify-content: flex-start;
      }

      &:nth-of-type(3n){
        // justify-content: flex-end;
      }
    }
    .upload-list{
      margin-top: PXtoEm(25);
      margin-bottom: PXtoEm(25);
      width: 50%;
    }
    .list-block{
      width: 100%;
      .span-block{
        width: 8.5%;
      }
      .whole-line{
        width: 86.5%;
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
