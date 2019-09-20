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
    <!-- 接收列表 -->
    <ul v-show="curOperationType === operationType.receive" class="con-padding-horizontal input-group">
      <li class="list">
        <div class="inp-wrap">
          <span class="text">事务原因</span>
          <div class="inp">{{stockOperation.reasonName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">仓库</span>
          <div class="inp">{{stockOperation.warehouseName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">责任人</span>
          <div class="inp">{{stockOperation.pickerName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">合同</span>
          <div class="inp">{{stockOperation.agreement}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">预算部门</span>
          <div class="inp">{{stockOperation.budgetDeptName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">预算项目</span>
          <div class="inp">{{stockOperation.budgetItems}}</div>
        </div>
      </li>
      <li v-show="isShow.operation" class="list">
        <div class="inp-wrap">
          <span class="text">{{columnName.operation}}</span>
          <div class="inp">{{stockOperation.fromStockOperationName}}</div>
        </div>
      </li>
      <li class="list list-block">
        <div class="inp-wrap">
          <span class="text span-block">备注</span>
          <div class="inp word-break">{{stockOperation.remark}}</div>
        </div>
      </li>
    </ul>
    <!-- 发放列表 -->
    <ul v-show="curOperationType === operationType.delivery" class="con-padding-horizontal input-group">
      <li class="list">
        <div class="inp-wrap">
          <span class="text">事务原因</span>
          <div class="inp">{{stockOperation.reasonName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">仓库</span>
          <div class="inp">{{stockOperation.warehouseName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">过账时间</span>
          <div class="inp">{{stockOperation.createdTime}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">责任人</span>
          <div class="inp">{{stockOperation.pickerName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">责任人部门</span>
          <div class="inp">{{stockOperation.pickerDeptName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">经办人</span>
          <div class="inp">{{stockOperation.createdName}}</div>
        </div>
      </li>
      <li v-show="isShow.operation" class="list">
        <div class="inp-wrap">
          <span class="text">{{columnName.operation}}</span>
          <div class="inp">{{stockOperation.fromStockOperationName}}</div>
        </div>
      </li>
      <li v-show="isShow.workingOrder" class="list">
        <div class="inp-wrap">
          <span class="text">工单</span>
          <div class="inp">{{stockOperation.workingOrder}}</div>
        </div>
      </li>
      <li v-show="isShow.someOrder" class="list">
        <div class="inp-wrap">
          <span class="text">{{columnName.someOrder}}</span>
          <div class="inp">{{stockOperation.someOrder}}</div>
        </div>
      </li>
      <li v-show="isShow.operation" class="list"/>
      <li class="list list-block">
        <div class="inp-wrap">
          <span class="text span-block">备注</span>
          <div class="inp word-break">{{stockOperation.remark}}</div>
        </div>
      </li>
    </ul>
    <!-- 调整列表 -->
    <ul v-show="curOperationType === operationType.adjust" class="con-padding-horizontal input-group">
      <li class="list">
        <div class="inp-wrap">
          <span class="text">事务原因</span>
          <div class="inp">{{stockOperation.reasonName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">仓库</span>
          <div class="inp">{{stockOperation.warehouseName}}</div>
        </div>
      </li>
      <li class="list"></li>
      <li class="list"></li>
      <li class="list list-block">
        <div class="inp-wrap">
          <span class="text span-block">备注</span>
          <div class="inp word-break">{{stockOperation.remark}}</div>
        </div>
      </li>
    </ul>
    <!-- 调整列表 -->
    <ul v-show="curOperationType === operationType.move" class="con-padding-horizontal input-group">
      <li class="list">
        <div class="inp-wrap">
          <span class="text">事务原因</span>
          <div class="inp">{{stockOperation.reasonName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">移出仓库</span>
          <div class="inp">{{stockOperation.fromWarehouseName}}</div>
        </div>
      </li>
      <li class="list">
        <div class="inp-wrap">
          <span class="text">移入仓库</span>
          <div class="inp">{{stockOperation.warehouseName}}</div>
        </div>
      </li>
      <li class="list"></li>
      <li class="list"></li>
      <li class="list list-block">
        <div class="inp-wrap">
          <span class="text span-block">备注</span>
          <div class="inp word-break">{{stockOperation.remark}}</div>
        </div>
      </li>
    </ul>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list name">物资ID</li>
        <li class="list name">物资名称</li>
        <li class="list name">规格型号</li>
        <li class="list name">单位</li>
        <li class="list name">{{columnName.countNo}}</li>
        <li v-show="isShow.returnNo" class="list name">{{columnName.returnNo}}</li>
        <li class="list name">供应商</li>
        <li class="list name">保质期</li>
        <li class="list name">单价</li>
        <li class="list name">{{columnName.amount}}</li>
        <li class="list name">币种</li>
        <li v-show="isShow.exchangeRate" class="list name">汇率</li>
        <li v-show="isShow.exchangeRate" class="list name">本币总金额</li>
        <li class="list name">发票号</li>
        <li class="list name">备注</li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="item in stockOperation.detailList" :key="item.key">
              <div class="list-content">
                <div class="name">{{ item.Entity}}</div>
                <div class="name">{{ item.SparePartsName }}</div>
                <div class="name">{{ item.SparePartsModel }}</div>
                <div class="name">{{ item.SparePartsUnit }}</div>
                <div class="name">{{ item.CountNo }}</div>
                <div v-show="isShow.returnNo" class="name">{{ item.ReturnNo }}</div>
                <div class="name word-break">{{ item.SupplierName }}</div>
                <div class="name word-break">{{ item.LifeDate === null ? '' : item.LifeDate.slice(0,10) }}</div>
                <div class="name">{{ item.UnitPrice }}</div>
                <div class="name word-break">{{ (item.CountNo * item.UnitPrice).toFixed(2) }}</div>
                <div class="name word-break">{{ item.CurrencyName }}</div>
                <div v-show="isShow.exchangeRate" class="name word-break">{{ item.ExchangeRate }}</div>
                <div v-show="isShow.exchangeRate" class="name word-break">{{ item.TotalAmount }}</div>
                <div class="name word-break">{{ item.Invoice }}</div>
                <div class="name word-break">{{ item.Remark }}</div>
              </div>
            </li>
          </ul>
        </el-scrollbar>
      </div>
    </div>
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import { sparePartsOperationType, sparePartsOperationDetailType } from '@/common/js/dictionary.js'
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
      title: '',
      curOperationType: '',
      sourceName: '',
      operationType: {
        receive: sparePartsOperationType.receive,
        delivery: sparePartsOperationType.delivery,
        adjust: sparePartsOperationType.adjust,
        move: sparePartsOperationType.move
      },
      stockOperation: {
        id: '',
        operationID: '',
        type: '',
        reasonName: '',
        warehouseName: '',
        remark: '',
        fromWarehouseName: '',
        pickerName: '',
        pickerDeptName: '',
        supplierName: '',
        agreement: '',
        budgetDeptName: '',
        budgetItems: '',
        fromStockOperationName: '',
        createdName: '',
        createdTime: '',
        workingOrder: '',
        someOrder: '',
        detailList: []
      },
      isShow: {
        exchangeRate: true,
        operation: true,
        someOrder: true,
        workingOrder: true,
        returnNo: false
      },
      columnName: {
        countNo: '数量',
        amount: '金额',
        someOrder: '',
        operation: '',
        returnNo: ''
      }
    }
  },
  created () {
    this.sourceName = this.$route.params.sourceName
    this.stockOperation.id = this.$route.params.id
    this.getstockOperation()
  },
  methods: {
    back () {
      this.$router.push({
        name: this.sourceName
      })
    },
    getstockOperation () {
      this.loading = true
      api.getStockOperationByID(this.stockOperation.id).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.stockOperation.id = data.id
          this.stockOperation.operationID = data.operationID
          this.stockOperation.type = data.type
          this.stockOperation.reasonName = data.reasonName
          this.stockOperation.warehouseName = data.warehouseName
          this.stockOperation.fromWarehouseName = data.fromWarehouseName
          this.stockOperation.pickerName = data.pickerName
          this.stockOperation.pickerDeptName = data.pickerDeptName
          // this.stockOperation.supplierName = data.supplierName
          this.stockOperation.agreement = data.agreement
          this.stockOperation.budgetDeptName = data.budgetDeptName
          this.stockOperation.budgetItems = data.budgetItems
          this.stockOperation.fromStockOperationName = data.fromStockOperationName
          this.stockOperation.createdName = data.createdName
          this.stockOperation.createdTime = transformDate(data.createdTime)
          this.stockOperation.remark = data.remark
          this.stockOperation.workingOrder = data.workingOrder
          this.stockOperation.someOrder = data.someOrder
          this.stockOperation.detailList = JSON.parse(data.detailList)

          this.curOperationType = this.stockOperation.type
          switch (this.curOperationType) {
            case this.operationType.receive:
              this.title = '| 物资接收过账 | '
              break
            case this.operationType.delivery:
              this.title = '| 物资发送过账 | '
              break
            case this.operationType.adjust:
              this.title = '| 物资调整过账 | '
              break
            case this.operationType.move:
              this.title = '| 物资移库过账 | '
              break
          }
          switch (data.reason) {
            case sparePartsOperationDetailType.purchaseReceive:
            case sparePartsOperationDetailType.otherReceive:
              this.isShow.returnNo = false
              this.isShow.operation = false
              this.isShow.someOrder = false
              this.isShow.workingOrder = false
              this.isShow.exchangeRate = true
              this.columnName.countNo = '接收数量'
              // this.columnName.amount = '接收金额'
              break
            case sparePartsOperationDetailType.purchaseReturn:
              this.isShow.returnNo = false
              this.isShow.operation = true
              this.columnName.operation = '采购接收流水号'
              this.isShow.someOrder = false
              this.isShow.exchangeRate = false
              this.isShow.workingOrder = false
              this.columnName.countNo = '退货数量'
              // this.columnName.amount = '退货金额'
              break
            case sparePartsOperationDetailType.distribution:
              this.isShow.returnNo = true
              this.columnName.returnNo = '已退回数量'
              this.isShow.operation = false
              this.isShow.someOrder = false
              this.isShow.exchangeRate = false
              this.isShow.workingOrder = true
              this.columnName.countNo = '领用数量'
              // this.columnName.amount = '领用金额'
              break
            case sparePartsOperationDetailType.materialReturn:
            case sparePartsOperationDetailType.troubleReturn:
              this.isShow.returnNo = false
              this.isShow.operation = true
              this.columnName.operation = '物资领用流水号'
              this.isShow.someOrder = false
              this.isShow.exchangeRate = false
              this.isShow.workingOrder = true
              this.columnName.countNo = '退回数量'
              // this.columnName.amount = '退回金额'
              break
            case sparePartsOperationDetailType.troubleRepair:
              this.isShow.returnNo = true
              this.columnName.returnNo = '已归还数量'
              this.isShow.operation = false
              this.isShow.someOrder = true
              this.columnName.someOrder = '送修单'
              this.isShow.exchangeRate = false
              this.isShow.workingOrder = false
              this.columnName.countNo = '送修数量'
              // this.columnName.amount = '送修金额'
              break
            case sparePartsOperationDetailType.materialLend:
              this.isShow.returnNo = true
              this.columnName.returnNo = '已归还数量'
              this.isShow.operation = false
              this.isShow.someOrder = false
              this.isShow.exchangeRate = false
              this.isShow.workingOrder = false
              this.columnName.countNo = '借用数量'
              // this.columnName.amount = '借用金额'
              break
            case sparePartsOperationDetailType.lendReturn:
              this.isShow.returnNo = false
              this.isShow.operation = true
              this.columnName.operation = '物资借用流水号'
              this.isShow.someOrder = false
              this.isShow.exchangeRate = false
              this.isShow.workingOrder = false
              this.columnName.countNo = '归还数量'
              // this.columnName.amount = '归还金额'
              break
            case sparePartsOperationDetailType.inStockScrap:
            case sparePartsOperationDetailType.troubleScrap:
              this.isShow.returnNo = false
              this.isShow.operation = false
              this.isShow.someOrder = false
              this.isShow.exchangeRate = false
              this.isShow.workingOrder = false
              this.columnName.countNo = '报废数量'
              // this.columnName.amount = '报废金额'
              break
            case sparePartsOperationDetailType.repairReceive:
              this.isShow.returnNo = false
              this.isShow.operation = true
              this.columnName.operation = '故障件送修流水号'
              this.isShow.someOrder = true
              this.columnName.someOrder = '送修单'
              this.isShow.exchangeRate = false
              this.isShow.workingOrder = false
              this.columnName.countNo = '归还数量'
              // this.columnName.amount = '归还金额'
              break
            case sparePartsOperationDetailType.inspection:
              this.isShow.returnNo = true
              this.columnName.returnNo = '已归还数量'
              this.isShow.operation = false
              this.isShow.someOrder = true
              this.columnName.someOrder = '送检单'
              this.isShow.exchangeRate = false
              this.isShow.workingOrder = false
              this.columnName.countNo = '送检数量'
              // this.columnName.amount = '送检金额'
              break
            case sparePartsOperationDetailType.inspectionReturn:
              this.isShow.returnNo = false
              this.isShow.operation = true
              this.columnName.operation = '正常件送检流水号'
              this.isShow.someOrder = true
              this.columnName.someOrder = '送检单'
              this.isShow.exchangeRate = false
              this.isShow.workingOrder = false
              this.columnName.countNo = '归还数量'
              // this.columnName.amount = '归还金额'
              break
          }
          this.title = this.title + this.stockOperation.operationID
        }
        this.loading = false
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
    height: percent($con-height - 20, $con-height);
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
