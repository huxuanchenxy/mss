<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeStockAdjustList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">事务原因<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="reason.text" filterable placeholder="请选择事务原因" @change="reasonChange">
                <el-option
                  v-for="item in reasonList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ reason.tips }}</p>
          </li>
          <li class="list list-block">
            <div class="inp-wrap">
              <span class="text span-block">备注</span>
              <el-input type="textarea" :rows="1" class="whole-line" placeholder="请输入备注" v-model="remark.text" @keyup.native="validateInputNull(remark)"></el-input>
            </div>
            <p class="validate-tips">{{ remark.tips }}</p>
          </li>
          <li v-show="!isLoss" class="list">
            <div class="inp-wrap">
              <span class="text">物资ID<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select filterable placeholder="请输入物资ID" v-model="entity.text" ref="stockDetailID">
                  <el-option
                    v-for="item in entityList"
                    :key="item.key"
                    :label="item.entity"
                    :value="item.id">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ entity.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">仓库<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="warehouse.text" clearable filterable placeholder="请选择仓库" @change="warehouseChange">
                <el-option
                  v-for="item in warehouseList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ warehouse.tips }}</p>
          </li>
          <li v-show="!isLoss" class="list">
            <div class="inp-wrap">
              <span class="text">盘盈数量<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input v-model="profitNo.text" placeholder="请输入盘盈数量" @keyup.native="validateNumber(profitNo)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ profitNo.tips }}</p>
          </li>
          <li v-show="isLoss" class="list">
            <div class="inp-wrap">
              <span class="text">物资</span>
              <div class="inp">
                <el-select ref="spareParts" v-model="spareParts.text" clearable filterable placeholder="请先选择仓库">
                  <el-option
                    v-for="item in sparePartsList"
                    :key="item.key"
                    :label="item.spname"
                    :value="item.spare_parts">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ spareParts.tips }}</p>
          </li>
          <li v-show="isLoss" class="list">
            <div class="inp-wrap">
              <span class="text"></span>
              <div class="inp">
                <x-button class="active" @click.native="searchResult(1)">获取库存清单</x-button>
              </div>
            </div>
          </li>
        </ul>
        <div v-show="!isLoss" class="btn-commit-group">
          <x-button class="close">
            <router-link :to="{name: 'SeeStockAdjustList'}">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="saveProfit">执行</x-button>
        </div>
        <div v-show="isLoss">
          <el-tabs class="tab-height" v-model="activeName">
            <el-tab-pane class="pane-height pane-notification" label="库存清单" name="stock">
              <!-- 内容 -->
              <div class="content-wrap">
                <ul class="content-header">
                  <li class="list name">物资名称</li>
                  <!--<li class="list name">规格型号</li>
                  <li class="list name">保质期</li>
                  <li class="list name">供应商</li>-->
                  <li class="list name">库存数量</li>
                  <li class="list name">存货数量</li>
                  <li class="list name">操作</li>
                </ul>
                <div class="scroll">
                  <el-scrollbar>
                    <ul class="list-wrap">
                      <li class="list" v-for="item in detailList" :key="item.key">
                        <div class="list-content">
                          <div class="name">{{ item.sparePartsName }}</div>
                          <!--<div class="name">{{ item.model }}</div>
                          <div class="name">{{ item.lifeDate === null ? '' : item.lifeDate.slice(0,10) }}</div>
                          <div class="name">{{ item.supplierName }}</div>-->
                          <div class="name">{{ item.stockNo }}</div>
                          <div class="name">{{ item.inStockNo }}</div>
                          <div class="name">
                            <x-button class="active" @click.native="inStockDetail(item)">亏损确认</x-button>
                          </div>
                        </div>
                      </li>
                    </ul>
                    <!-- 分页 -->
                    <el-pagination
                      :current-page.sync="currentPage"
                      @current-change="handleCurrentChange"
                      @prev-click="prevPage"
                      @next-click="nextPage"
                      layout="slot, jumper, prev, pager, next"
                      prev-text="上一页"
                      next-text="下一页"
                      :total="total">
                      <span>总共 {{ total }} 条记录</span>
                    </el-pagination>
                  </el-scrollbar>
                </div>
              </div>
            </el-tab-pane>
            <el-tab-pane class="pane-height pane-notification" label="存货明细" name="send">
              <!-- 内容 -->
              <div class="content-wrap">
                <ul class="content-header">
                  <li class="list name">物资ID</li>
                  <li class="list name">物资名称</li>
                  <li class="list name">物资状态</li>
                  <li class="list name">存货数量</li>
                  <li class="list name">盈亏数量</li>
                  <li class="list url">备注</li>
                </ul>
                <div class="scroll">
                  <el-scrollbar>
                    <ul class="list-wrap">
                      <li class="list" v-for="(item, index)  in distributionList" :key="item.key">
                        <div class="list-content">
                          <div class="name">{{ item.entity }}</div>
                          <div class="name">{{ item.sparePartsName }}</div>
                          <div class="name">{{ item.statusName === '' ? '无' : item.statusName}}</div>
                          <div class="name">{{ item.inStockNo }}</div>
                          <div class="name word-break">
                            <el-input class="center" v-model="item.editNo" @keyup.native="validateEditNo(item.inStockNo, item.editNo, index)"></el-input>
                          </div>
                          <div class="url word-break">
                            <el-input class="center" v-model="item.remark"></el-input>
                          </div>
                        </div>
                      </li>
                    </ul>
                  </el-scrollbar>
                </div>
              </div>
              <!-- 按钮 -->
              <div class="btn-commit-group">
                <x-button class="close">
                  <router-link :to="{name: 'SeeStockAdjustList'}">取消</router-link>
                </x-button>
                <x-button class="active" @click.native="save">执行</x-button>
              </div>
            </el-tab-pane>
          </el-tabs>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { vInput, vNumber, validateInputCommon } from '@/common/js/utils.js'
import { sparePartsOperationType, sparePartsOperationDetailType } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import api from '@/api/wmsApi'
import apiAuth from '@/api/authApi'
export default {
  name: 'AddStockAdjust',
  components: {
    XButton
  },
  data () {
    return {
      activeName: 'stock',
      loading: false,
      title: '| 物资盘盈盘亏',
      bCheckAll: false,
      reasonList: [],
      reason: {text: '', tips: ''},
      warehouseList: [],
      warehouse: {text: '', tips: ''},
      spareParts: {text: '', tips: ''},
      sparePartsList: '',
      remark: {text: '', tips: ''},
      profitNo: {text: '', tips: ''},
      entity: {text: '', tips: ''},
      entityList: [],
      currentPage: 1,
      total: 0,
      detailList: [],
      distributionList: [],
      isLoss: true
    }
  },
  created () {
    // 事务原因列表
    apiAuth.getSubCodeOrder(sparePartsOperationType.adjust).then(res => {
      this.reasonList = res.data
    }).catch(err => console.log(err))
    // 仓库加载
    api.getWarehouseAll().then(res => {
      this.warehouseList = res.data
    }).catch(err => console.log(err))
    // 物资ID加载
    api.getStockDetailAll().then(res => {
      this.entityList = res.data
    }).catch(err => console.log(err))
    // // 物资加载
    // api.getSparePartsAll().then(res => {
    //   this.sparePartsList = res.data
    // }).catch(err => console.log(err))
  },
  methods: {
    reasonChange () {
      this.isLoss = this.reason.text === sparePartsOperationDetailType.inventoryLoss
    },
    inStockDetail (item) {
      api.getStockDetail(item.spareParts, this.warehouse.text).then(res => {
        if (res.code === 0) {
          this.distributionList = res.data
          this.distributionList.map(val => {
            val.remark = ''
          })
          this.activeName = 'send'
        } else this.loading = false
      }).catch(err => console.log(err))
    },
    warehouseChange () {
      this.reason.tips = ''
      if (this.reason.text === '') {
        this.$message({
          message: '请选择事务原因',
          type: 'warning'
        })
        this.warehouse.text = ''
        return
      }
      if (this.reason.text === sparePartsOperationDetailType.inventoryLoss) {
        this.spareParts.text = ''
        if (!this.validateSelect(this.warehouse)) return
        // 根据仓库找物资
        api.getSparePartsByWH(this.warehouse.text).then(res => {
          this.sparePartsList = res.data
        }).catch(err => console.log(err))
      }
    },
    validateEditNo (old, now, index) {
      if (vNumber(now)) {
        if (now > old && this.reason.text === sparePartsOperationDetailType.inventoryLoss) {
          this.$message({
            message: '盘亏数量不可大于库存数量',
            type: 'warning'
          })
          this.distributionList[index].countNo = 1
        }
      } else {
        this.$message({
          message: '请输入数字',
          type: 'warning'
        })
        this.distributionList[index].countNo = 1
      }
    },
    validateInput (val) {
      return validateInputCommon(val)
    },
    validateNumber (val) {
      if (vNumber(val.text)) {
        val.tips = ''
        return true
      } else {
        val.tips = '此项必填数字'
        return false
      }
    },
    validateInputNull (val) {
      if (!vInput(val.text)) {
        val.tips = '此项含有非法字符'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateSelect (val) {
      if (val.text === '') {
        val.tips = '此项必选'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    save () {
      if (this.distributionList.length === 0) {
        this.$message({
          message: '请从库存清单中选择盘点物资',
          type: 'warning'
        })
        return
      }
      let arr = []
      this.distributionList.map((val, index) => {
        if (val.editNo + '' !== '0') {
          val.unitPrice = val.acceptUnitPrice
          val.amount = val.editNo * val.unitPrice * val.exchangeRate
          val.orderNo = index + 1
          val.stockDetail = val.id
          val.countNo = val.editNo
          arr.push(val)
        }
      })
      let stockAdjust = {
        Type: sparePartsOperationType.adjust,
        Reason: this.reason.text,
        Warehouse: this.warehouse.text,
        Remark: this.remark.text,
        DetailList: JSON.stringify(arr)
      }
      api.addStockOperation(stockAdjust).then(res => {
        if (res.code === 0) {
          this.$router.push({name: 'SeeStockAdjustList'})
          this.$message({
            message: '执行成功',
            type: 'success'
          })
        } else {
          this.$message({
            message: res.msg === '' ? '执行失败' : res.msg,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    saveProfit () {
      if (!this.validateSelect(this.entity) || !this.validateSelect(this.warehouse) || !this.validateNumber(this.profitNo)) return
      let arr = {
        id: this.entity.text,
        entity: this.$refs.stockDetailID.selected.label,
        countNo: this.profitNo.text,
        warehouse: this.warehouse.text
      }
      let stockAdjust = {
        Type: sparePartsOperationType.adjust,
        Reason: this.reason.text,
        Warehouse: this.warehouse.text,
        Remark: this.remark.text,
        DetailList: JSON.stringify(arr)
      }
      api.addStockOperation(stockAdjust).then(res => {
        if (res.code === 0) {
          this.$router.push({name: 'SeeStockAdjustList'})
          this.$message({
            message: '执行成功',
            type: 'success'
          })
        } else {
          this.$message({
            message: res.msg === '' ? '执行失败' : res.msg,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    // 搜索
    searchResult (page) {
      if (!this.validateSelect(this.reason) || !this.validateSelect(this.warehouse)) return
      this.activeName = 'stock'
      this.currentPage = page
      this.loading = true
      api.getStockSum({
        order: 'asc',
        rows: 10,
        page: page,
        searchSpareParts: this.spareParts.text,
        SearchWarehouse: this.warehouse.text
      }).then(res => {
        this.loading = false
        this.total = res.data.total
        this.detailList = res.data.rows
      }).catch(err => console.log(err))
    },
    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.currentPage = val
      this.searchResult(val)
    },

    // 上一页
    prevPage (val) {
      this.currentPage = val
      this.searchResult(val)
    },

    // 下一页
    nextPage (val) {
      this.currentPage = val
      this.searchResult(val)
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
    height: percent(50, $con-height);
    padding: PXtoEm(12) PXtoEm(24);
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
.tab-height{
  height: percent($con-height, $con-height);
}
/deep/ .el-tabs__header{
  margin-left: 10px!important;
  height: percent(50, $con-height)
}
/deep/ .el-tabs__content{
  overflow: hidden;
  height: percent($con-height - 50, $con-height)
}
/deep/
.center .el-input__inner{
  text-align: center;
  width: 90%;
}
</style>
