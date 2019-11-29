<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeStockMoveList' }">返回</router-link></x-button>
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
          <li class="list">
            <div class="inp-wrap">
              <span class="text">责任人<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="picker.text" clearable filterable placeholder="请选择责任人"  @change="validateSelect(picker)">
                  <el-option
                    v-for="item in pickerList"
                    :key="item.key"
                    :label="item.user_name"
                    :value="item.id">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ picker.tips }}</p>
          </li>
          <li class="list" v-show="isShow.toWarehouse">
            <div class="inp-wrap">
              <span class="text">移入仓库<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="toWarehouse.text" clearable filterable placeholder="请选择仓库" @change="toWarehouseChange">
                <el-option
                  v-for="item in warehouseList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ toWarehouse.tips }}</p>
          </li>
          <li class="list" v-show="!isShow.toWarehouse"/>
          <li class="list list-block">
            <div class="inp-wrap">
              <span class="text span-block">备注</span>
              <el-input type="textarea" :rows="1" class="whole-line" placeholder="请输入备注" v-model="remark.text" @keyup.native="validateInputNull(remark)"></el-input>
            </div>
            <p class="validate-tips">{{ remark.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text" v-show="isShow.toWarehouse">移出仓库<em class="validate-mark">*</em></span>
              <span class="text" v-show="!isShow.toWarehouse">仓库<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="warehouse.text" clearable filterable placeholder="请选择仓库" :disabled="isDisabledWarehouse" @change="warehouseChange">
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
          <li class="list">
            <div class="inp-wrap">
              <span class="text">物资<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select ref="spareParts" v-model="spareParts.text" clearable filterable placeholder="请先选择仓库" @change="validateSelect(spareParts)">
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
          <li class="list">
            <div class="inp-wrap">
              <span class="text"></span>
              <div class="inp">
                <x-button class="active" @click.native="getStockDetail">获取库存清单</x-button>
              </div>
            </div>
          </li>
        </ul>
        <div>
          <el-tabs class="tab-height" v-model="activeName">
            <el-tab-pane class="pane-height pane-notification" label="库存清单" name="stock">
              <!-- 内容 -->
              <div class="content-wrap">
                <ul class="content-header">
                  <li class="list number">物资ID</li>
                  <li class="list name">物资名称</li>
                  <li class="list name">规格型号</li>
                  <li class="list name">保质期</li>
                  <li class="list name">供应商</li>
                  <li class="list name">移出库位</li>
                  <li class="list name">库存数量</li>
                  <li class="list name">故障件数量</li>
                  <li class="list name">存货数量</li>
                  <!--<li class="list name">送检数量</li>
                  <li class="list name">送修数量</li>
                  <li class="list name">借用数量</li>-->
                  <li class="list menuOrder">转移数量</li>
                  <li class="list menuOrder">转移物资ID(批量)</li>
                  <li class="list menuOrder">移入库位</li>
                  <li class="list menuOrder">备注</li>
                </ul>
                <div class="scroll">
                  <el-scrollbar>
                    <ul class="list-wrap">
                      <li class="list" v-for="(item, index) in detailList" :key="item.key">
                        <div class="list-content">
                          <div class="number">{{ item.entity }}</div>
                          <div class="name">{{ item.sparePartsName }}</div>
                          <div class="name">{{ item.model }}</div>
                          <div class="name">{{ item.lifeDate === null ? '' : item.lifeDate.slice(0,10) }}</div>
                          <div class="name">{{ item.supplierName }}</div>
                          <div class="name">{{ item.storageLocationName }}</div>
                          <div class="name">{{ item.stockNo }}</div>
                          <div class="name">{{ item.troubleNo }}</div>
                          <div class="name">{{ item.inStockNo }}</div>
                          <!--<div class="name">{{ item.inspectionNo }}</div>
                          <div class="name">{{ item.repairNo }}</div>
                          <div class="name">{{ item.lentNo }}</div>-->
                          <div class="menuOrder">
                            <el-input class="center" v-model="item.editNo" @keyup.native="validateAllEditNo(item, index)"></el-input>
                          </div>
                          <div class="menuOrder">
                            <el-input class="center" :disabled="item.inStockNo === 1 || item.stockNo + '' === item.editNo" v-model="item.newEntity"></el-input>
                          </div>
                          <div class="menuOrder">
                            <el-select v-model="item.storageLocation" placeholder="请选择库位" filterable>
                              <el-option
                                v-for="item in storageLocationList"
                                :key="item.key"
                                :label="item.name"
                                :value="item.id">
                              </el-option>
                            </el-select>
                          </div>
                          <div class="menuOrder word-break">
                            <el-input class="center" v-model="item.editRemark"></el-input>
                          </div>
                        </div>
                      </li>
                    </ul>
                  </el-scrollbar>
                </div>
              </div>
              <!-- 按钮 -->
              <div class="btn-commit-group">
                <x-button class="active" @click.native="insert">确认</x-button>
              </div>
            </el-tab-pane>
            <el-tab-pane class="pane-height pane-notification" label="移库清单" name="send">
              <!-- 内容 -->
              <div class="content-wrap">
                <ul class="content-header">
                  <li class="list name">物资ID</li>
                  <li class="list name">物资名称</li>
                  <li class="list name">{{editNoCompareName}}</li>
                  <li class="list name">移出库位</li>
                  <li class="list name">转移数量</li>
                  <li class="list name">转移物资ID(批量)</li>
                  <li class="list name">移入库位</li>
                  <li class="list url">备注</li>
                  <li class="list name">操作</li>
                </ul>
                <div class="scroll">
                  <el-scrollbar>
                    <ul class="list-wrap">
                      <li class="list" v-for="(item, index)  in distributionList" :key="item.key">
                        <div class="list-content">
                          <div class="name">{{ item.entity }}</div>
                          <div class="name">{{ item.sparePartsName }}</div>
                          <div class="name">{{ item.inStockNo }}</div>
                          <div class="name">{{ item.storageLocationName }}</div>
                          <div v-show="!item.isEdit" class="name word-break">{{ item.countNo }}</div>
                          <div v-show="item.isEdit" class="name word-break">
                            <el-input class="center" v-model="item.countNo" @keyup.native="validateEditNo(item.inStockNo, item.countNo, index)"></el-input>
                          </div>
                          <div v-show="!item.isEdit" class="name word-break">{{ item.newEntity }}</div>
                          <div v-show="item.isEdit" class="name word-break">
                            <el-input class="center" :disabled="item.inStockNo === 1 || item.stockNo + '' === item.countNo" v-model="item.newEntity"></el-input>
                          </div>
                          <div class="name word-break">
                            <el-select v-model="item.storageLocation" placeholder="请选择库位" filterable :disabled="!item.isEdit">
                              <el-option
                                v-for="item in storageLocationList"
                                :key="item.key"
                                :label="item.name"
                                :value="item.id">
                              </el-option>
                            </el-select>
                          </div>
                          <div v-show="!item.isEdit" class="url word-break">{{ item.remark }}</div>
                          <div v-show="item.isEdit" class="url word-break">
                            <el-input class="center" v-model="item.remark"></el-input>
                          </div>
                          <div class="name word-break">
                            <x-button v-show="!item.isEdit" class="active" @click.native="edit(index)">编辑</x-button>
                            <x-button v-show="item.isEdit" class="active" @click.native="saveTmp(index)">保存</x-button>
                            <x-button class="active" @click.native="del(index)">删除</x-button>
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
                  <router-link :to="{name: 'SeeStockMoveList'}">取消</router-link>
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
  name: 'AddStockMove',
  components: {
    XButton
  },
  data () {
    return {
      activeName: 'stock',
      loading: false,
      title: '| 物资移库过账',
      reasonList: [],
      reason: {text: '', tips: ''},
      warehouseList: [],
      warehouse: {text: '', tips: ''},
      toWarehouse: {text: '', tips: ''},
      spareParts: {text: '', tips: ''},
      sparePartsList: '',
      pickerList: [],
      picker: {text: '', tips: ''},
      remark: {text: '', tips: ''},
      detailList: [],
      distributionList: [],
      storageLocationList: [],
      editNoCompareName: '存货数量',
      isDisabledWarehouse: false,
      isShow: {
        toWarehouse: false
      }
    }
  },
  created () {
    // 事务原因列表
    apiAuth.getSubCodeOrder(sparePartsOperationType.move).then(res => {
      this.reasonList = res.data
    }).catch(err => console.log(err))
    // 仓库加载
    api.getWarehouseAll().then(res => {
      this.warehouseList = res.data
    }).catch(err => console.log(err))
    // // 物资加载
    // api.getSparePartsAll().then(res => {
    //   this.sparePartsList = res.data
    // }).catch(err => console.log(err))
    // 领料人加载
    apiAuth.getUserAll().then(res => {
      this.pickerList = res.data
    }).catch(err => console.log(err))
  },
  methods: {
    reasonChange () {
      this.reason.tips = ''
      if (this.reason.text === sparePartsOperationDetailType.moveTo || this.reason.text === sparePartsOperationDetailType.moveLocation) {
        this.editNoCompareName = '存货数量'
      } else {
        this.editNoCompareName = '故障件数量'
      }
      if (this.reason.text === sparePartsOperationDetailType.troubleMoveLocation || this.reason.text === sparePartsOperationDetailType.moveLocation) {
        this.isShow.toWarehouse = false
      } else {
        this.isShow.toWarehouse = true
      }
      this.warehouse.text = ''
      this.toWarehouse.text = ''
      this.spareParts.text = ''
      this.sparePartsList = []
      this.storageLocationList = []
      this.detailList = []
      this.distributionList = []
      this.isDisabledWarehouse = false
      this.activeName = 'stock'
    },
    warehouseChange () {
      this.spareParts.text = ''
      if (!this.validateSelect(this.warehouse)) return
      // 根据仓库找物资
      api.getSparePartsByWH(this.warehouse.text).then(res => {
        this.sparePartsList = res.data
      }).catch(err => console.log(err))
      if (this.reason.text === sparePartsOperationDetailType.troubleMoveLocation || this.reason.text === sparePartsOperationDetailType.moveLocation) {
        console.log(this.warehouse.text)
        this.getStorageLocationByWarehouse(this.warehouse.text)
      }
    },
    toWarehouseChange () {
      this.getStorageLocationByWarehouse(this.toWarehouse.text)
    },
    getStorageLocationByWarehouse (warehouse) {
      // 根据仓库找库位
      api.getStorageLocationByWarehouse(warehouse).then(res => {
        this.storageLocationList = res.data
      }).catch(err => console.log(err))
    },
    getStockDetail () {
      if (!this.validateSelect(this.reason) || !this.validateSelect(this.spareParts) || !this.validateSelect(this.warehouse)) {
        return
      }
      if (this.isShow.fromStorageLocation && !this.validateSelect(this.toWarehouse)) return
      // 库存明细
      api.getStockDetail(this.spareParts.text, this.warehouse.text, this.reason.text, 0).then(res => {
        this.detailList = res.data
        if (this.detailList.length === 0) {
          this.$message({
            message: '没有找到相关可转移数据',
            type: 'warning'
          })
        } else {
          this.detailList.map(val => {
            val.fromStorageLocation = val.storageLocation
            val.storageLocation = ''
          })
        }
        this.activeName = 'stock'
      }).catch(err => console.log(err))
    },
    validateEditNo (old, now, index) {
      if (vNumber(now)) {
        if (now > old) {
          this.$message({
            message: '转移数量不可大于' + this.editNoCompareName,
            type: 'warning'
          })
          this.distributionList[index].countNo = 0
        } else if (now === old + '') this.distributionList[index].newEntity = ''
      } else {
        this.$message({
          message: '请输入数字',
          type: 'warning'
        })
        this.distributionList[index].countNo = 0
      }
    },
    validateAllEditNo (item, index) {
      if (vNumber(item.editNo)) {
        if ((this.editNoCompareName === '存货数量' && item.stockNo < item.editNo) ||
        (this.editNoCompareName === '故障件数量' && item.troubleNo < item.editNo)) {
          this.$message({
            message: '转移数量不可大于' + this.editNoCompareName,
            type: 'warning'
          })
          this.detailList[index].editNo = 0
        } else if (item.stockNo + '' === item.editNo) this.detailList[index].newEntity = ''
      } else {
        this.$message({
          message: '请输入数字',
          type: 'warning'
        })
        this.detailList[index].editNo = 0
      }
    },
    insert () {
      let isInsert = false
      for (let i = 0; i < this.detailList.length; i++) {
        if (this.detailList[i].editNo + '' !== '0') {
          let distribution = {
            entity: this.detailList[i].entity,
            spareParts: this.detailList[i].spareParts,
            sparePartsName: this.detailList[i].sparePartsName,
            fromStorageLocation: this.detailList[i].fromStorageLocation,
            storageLocationName: this.detailList[i].storageLocationName,
            stockNo: this.detailList[i].stockNo,
            inStockNo: this.detailList[i].inStockNo,
            countNo: this.detailList[i].editNo,
            newEntity: this.detailList[i].newEntity === null ? '' : this.detailList[i].newEntity,
            storageLocation: this.detailList[i].storageLocation,
            remark: this.detailList[i].editRemark,
            // stockDetail: this.detailList[i].id,
            // acceptUnitPrice: this.detailList[i].acceptUnitPrice,
            exchangeRate: this.detailList[i].exchangeRate,
            isEdit: false
          }
          if (this.detailList[i].stockNo !== 1 && (this.detailList[i].newEntity === null || this.detailList[i].newEntity === '')) {
            this.$message({
              message: this.detailList[i].entity + '批次必须录入转移物资ID',
              type: 'error'
            })
            return
          }
          if (!vInput(this.detailList[i].newEntity)) {
            this.$message({
              message: this.detailList[i].entity + '的新物资ID含有非法字符',
              type: 'error'
            })
            return
          }
          if (this.detailList[i].storageLocation === '') {
            this.$message({
              message: '请选择库位',
              type: 'error'
            })
            return
          }
          let has = this.distributionList.some(item => {
            return this.detailList[i].entity === item.entity
          })
          if (has) {
            this.$message({
              message: '移库清单中已存在物资ID:' + this.detailList[i].entity,
              type: 'warning'
            })
            return
          }
          distribution.stockDetail = this.detailList[i].id
          if (this.reason.text === sparePartsOperationDetailType.torubleMoveTo) distribution.inStockNo = this.detailList[i].troubleNo
          distribution.unitPrice = this.detailList[i].acceptUnitPrice
          isInsert = true
          this.distributionList.push(distribution)
        }
      }
      if (isInsert) {
        this.isDisabledWarehouse = true
        this.$message({
          message: '所选物资已进入移库,确认所有后，请切换标签页执行',
          type: 'success'
        })
      }
      // this.activeName = 'send'
    },
    edit (index) {
      this.distributionList[index].isEdit = true
    },
    saveTmp (index) {
      this.distributionList[index].isEdit = false
    },
    del (index) {
      this.distributionList.splice(index, 1)
      if (this.distributionList.length === 0) {
        this.isDisabledWarehouse = false
      }
    },
    validateInput (val) {
      return validateInputCommon(val)
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
    validateInputAll () {
      let ret = true
      ret = this.validateSelect(this.picker)
      if (this.isShow.fromStorageLocation && !this.validateSelect(this.toWarehouse)) return false
      return ret
    },
    save () {
      if (this.distributionList.length === 0) {
        this.$message({
          message: '请从库存清单中选择转移物资',
          type: 'warning'
        })
        return
      }
      if (!this.validateInputAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      if (this.warehouse.text === this.toWarehouse.text) {
        this.$message({
          message: '移入仓库和移出仓库不能相同',
          type: 'error'
        })
        return
      }
      let arr = this.distributionList.concat([])
      arr.sort()
      for (var i = 0; i < arr.length - 1; i++) {
        if (arr[i].newEntity !== '' && arr[i].newEntity === arr[i + 1].newEntity) {
          this.$message({
            message: '转移物资ID' + arr[i].newEntity + '重复',
            type: 'warning'
          })
          return
        }
      }
      for (let i = 0; i < this.distributionList.length; i++) {
        if (this.distributionList[i].countNo + '' === '0') {
          this.$message({
            message: '转移数量不可为0',
            type: 'error'
          })
          return
        }
        if (this.distributionList[i].stockNo !== 1 && this.distributionList[i].newEntity === '') {
          this.$message({
            message: this.distributionList[i].entity + '批次必须录入转移物资ID',
            type: 'warning'
          })
          return
        }
        if (!vInput(this.distributionList[i].newEntity)) {
          this.$message({
            message: this.distributionList[i].entity + '的新物资ID含有非法字符',
            type: 'warning'
          })
          return
        }
        this.distributionList[i].unitPrice = this.distributionList[i].unitPrice
        this.distributionList[i].amount = this.distributionList[i].countNo * this.distributionList[i].unitPrice * this.distributionList[i].exchangeRate
        this.distributionList[i].orderNo = i + 1
      }
      let stockMove = {
        Type: sparePartsOperationType.move,
        Reason: this.reason.text,
        Warehouse: this.warehouse.text,
        ToWarehouse: this.toWarehouse.text,
        Remark: this.remark.text,
        Picker: this.picker.text,
        DetailList: JSON.stringify(this.distributionList)
      }
      api.addStockOperation(stockMove).then(res => {
        if (res.code === 0) {
          this.$router.push({name: 'SeeStockMoveList'})
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
