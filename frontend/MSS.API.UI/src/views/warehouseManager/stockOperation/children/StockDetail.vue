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
    <ul class="con-padding-horizontal input-group">
      <li class="list">
        <div class="inp-wrap">
          <span class="text">物资名称</span>
          <div class="inp">{{spareParts.name}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">物资类型</span>
          <div class="inp">{{spareParts.typeName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">计量单位</span>
          <div class="inp">{{spareParts.unit}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">规格型号</span>
          <div class="inp">{{spareParts.model}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">适用设备类型</span>
          <div class="inp">{{spareParts.eqpTypeName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">计划价格(RMB)</span>
          <div class="inp">{{spareParts.planPrice}}</div>
        </div>
      </li>
      <li class="list list-block">
        <div class="inp-wrap">
          <span class="text span-block">英语描述</span>
          <div class="inp">{{spareParts.englishDes}}</div>
        </div>
      </li>
      <li class="list list-block">
        <div class="inp-wrap">
          <span class="text span-block">备注</span>
          <div class="inp word-break">{{spareParts.remark}}</div>
        </div>
      </li>
    </ul>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list name">接收日期</li>
        <li class="list name">仓库</li>
        <li class="list name">接收数量</li>
        <li class="list name">库存数量</li>
        <li class="list name">故障件数量</li>
        <li class="list name">存货数量</li>
        <li class="list name">送检数量</li>
        <li class="list name">送修数量</li>
        <li class="list name">接收币种</li>
        <li class="list name">接收单价</li>
        <li class="list name">存货金额</li>
        <li class="list name">供应商</li>
        <li class="list name">保质期</li>
        <li class="list name">接收备注</li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="item in stockDetailList" :key="item.key">
              <div class="list-content">
                <div class="name">{{ item.acceptDate.slice(0,10)}}</div>
                <div class="name">{{ item.warehouseName }}</div>
                <div class="name">{{ item.acceptNo }}</div>
                <div class="name">{{ item.stockNo }}</div>
                <div class="name">{{ item.troubleNo }}</div>
                <div class="name">{{ item.inStockNo }}</div>
                <div class="name">{{ item.inspectionNo }}</div>
                <div class="name">{{ item.repairNo }}</div>
                <div class="name word-break">{{ item.currencyName }}</div>
                <div class="name word-break">{{ item.acceptUnitPrice.toFixed(2) }}</div>
                <div class="name word-break">{{ (item.acceptUnitPrice * item.inStockNo).toFixed(2) }}</div>
                <div class="name word-break">{{ item.supplierName }}</div>
                <div class="name word-break">{{ item.lifeDate === null || item.lifeDate === '' ? '' : item.lifeDate.slice(0,10) }}</div>
                <div class="name word-break">{{ item.remark }}</div>
              </div>
            </li>
          </ul>
        </el-scrollbar>
      </div>
    </div>
  </div>
</template>
<script>
// import { transformDate } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/wmsApi'
export default {
  name: 'DetailStockOperation',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      title: ' | 存货明细',
      spareParts: {
        id: this.$route.params.id,
        name: '',
        typeName: '',
        unit: '',
        model: '',
        eqpTypeName: '',
        planPrice: 0,
        englishDes: '',
        remark: ''
      },
      warehouse: '',
      stockDetailList: []
    }
  },
  created () {
    // this.sourceName = this.$route.params.sourceName
    this.spareParts.id = this.$route.params.id
    this.warehouse = this.$route.params.warehouse
    this.getStockDetail()
    this.getSpareParts()
  },
  methods: {
    back () {
      this.$router.push({
        name: 'SeeStockSumList'
      })
    },
    getSpareParts () {
      api.getSparePartsByID(this.spareParts.id).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.spareParts.id = data.id
          this.spareParts.name = data.name
          this.spareParts.typeName = data.typeName
          this.spareParts.model = data.model
          this.spareParts.unit = data.unit
          this.spareParts.eqpTypeName = data.eqpTypeName
          this.spareParts.planPrice = data.planPrice
          this.spareParts.englishDes = data.englishDes
          this.spareParts.remark = data.remark
        }
      }).catch(err => console.log(err))
    },
    getStockDetail () {
      api.getStockDetail(this.spareParts.id, this.warehouse).then(res => {
        if (res.code === 0) {
          this.stockDetailList = res.data
        } else this.loading = false
      }).catch(err => console.log(err))
    }
  }
}
</script>
<style lang="scss" scoped>
$con-height: $content-height - 145 - 56;
// 内容区
.content-wrap{
  overflow: hidden;
  height: percent($con-height, $content-height);
  text-align: center;
  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: percent(20, $con-height)!important;
    padding: 0 PXtoEm(24);
    margin-top: PXtoEm(12);
    background: rgba(36,128,198,.5);

    .last-update-time{
      color: $color-white;
    }
  }

  .scroll{
    height: percent($con-height - 50, $con-height);
  }

  .list-wrap{
    .list{
      &:nth-of-type(even){
        .list-content{
          background: rgba(186,186,186,.5);
        }
      }
    }

    .list-content{
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: PXtoEm(15) PXtoEm(24);
    }

    .left-title{
      margin-right: 10px;
      font-weight: bold;
    }

    // 隐藏内容
    .sub-content{
      overflow: hidden;
      height: 0;
      font-size: $font-size-small;
      text-align: left;
      color: $color-content-text;

      &.active{
        overflow: inherit;
        height: auto;
        transition: .7s .2s;
      }
    }

    .sub-con-list{
      display: flex;
      padding: PXtoEm(15) PXtoEm(24);
      border-top: 1px solid $color-main-background;
      background: rgba(0,0,0,.2);

      .right-wrap{
        display: flex;
        flex-wrap: wrap;
      }

      .list{
        margin-right: 10px;
      }
    }
  }

  .number{
    width: 6%;
  }

  .name,
  .btn-wrap{
    width: 8%;
  }

  .last-update-time{
    width: 15%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 10%;
  }

  .upload-cascader{
    width: 13%;
  }

  .url{
    width: 15%;
  }

  .menuOrder{
    width: 10%;
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
.btn-commit-group{
  padding: 20px 0;
  text-align: center;
}

.btn-group{
  display: flex;
  align-items: center;
  height: percent(65, 145);

  .list{
    margin-right: PXtoEm(10);
  }

  .btn{
    &:hover{
      border-color: $color-main-btn;
      background: $color-main-btn;
    }
  }
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
  width: -webkit-fill-available;
}
.left{
  text-indent: 9.5%
}
</style>
