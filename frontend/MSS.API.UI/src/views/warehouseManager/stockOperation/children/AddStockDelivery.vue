<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeStockDeliveryList' }">返回</router-link></x-button>
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
          <li v-show="isShow.picker" class="list">
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
          <li v-show="isShow.workingOrder === 1 && !isShow.stockDetail" class="list">
            <div class="inp-wrap">
              <span class="text">故障单<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="workingOrder.text" clearable filterable placeholder="请选择故障单"  @change="validateSelect(workingOrder)">
                  <el-option
                    v-for="item in workingOrderList"
                    :key="item.key"
                    :label="item.code"
                    :value="item.id">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ workingOrder.tips }}</p>
          </li>
          <li v-show="isShow.workingOrder === 2 && !isShow.stockDetail" class="list">
            <div class="inp-wrap">
              <span class="text">{{someOrderName}}<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input :placeholder="'请输入' + someOrderName" v-model="someOrder.text" @keyup.native="validateInput(someOrder)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ someOrder.tips }}</p>
          </li>
          <li v-show="!(!isShow.stockDetail && (isShow.workingOrder === 2 || isShow.workingOrder === 1))" class="list"/>
          <li class="list list-block">
            <div class="inp-wrap">
              <span class="text span-block">备注</span>
              <el-input type="textarea" :rows="1" class="whole-line" placeholder="请输入备注" v-model="remark.text" @keyup.native="validateInputNull(remark)"></el-input>
            </div>
            <p class="validate-tips">{{ remark.tips }}</p>
          </li>
          <li v-show="!isShow.stockDetail" class="list">
            <div class="inp-wrap">
              <span class="text">仓库<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="warehouse.text" filterable placeholder="请选择仓库" @change="warehouseChange">
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
          <li v-show="!isShow.stockDetail" class="list">
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
          <li v-show="isShow.scrap" class="list">
            <div class="inp-wrap">
              <span class="text">物资ID<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select clearable filterable placeholder="请输入物资ID" v-model="entity" @change="getStockDetailByID">
                  <el-option
                    v-for="item in entityList"
                    :key="item.key"
                    :label="item.entity"
                    :value="item.id">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips"></p>
          </li>
          <li v-show="!isShow.stockDetail" class="list">
            <div class="inp-wrap">
              <span class="text"></span>
              <div class="inp">
                <x-button class="active" @click.native="getStockDetail">获取库存清单</x-button>
              </div>
            </div>
          </li>
          <li v-show="isShow.stockDetail && !isShow.scrap" class="list">
            <div class="inp-wrap">
              <span class="text">{{fromStockOperationName}}流水号<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select clearable filterable :placeholder="'请输入' + fromStockOperationName + '流水号'" v-model="stockOperation" @change="getStockOperationDetail">
                  <el-option
                    v-for="item in stockOperationList"
                    :key="item.key"
                    :label="item.operation_id"
                    :value="item.id">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips"></p>
          </li>
          <li v-show="isShow.stockDetail && !isShow.scrap" class="list">
            <div class="inp-wrap">
              <span class="text">仓库</span>
              <div class="inp">{{stockOperationShow.warehouse}}</div>
            </div>
          </li>
          <li v-show="isShow.workingOrder !== 3 && isShow.stockDetail" class="list">
            <div class="inp-wrap">
              <span class="text">{{someOrderName}}</span>
              <div class="inp">{{stockOperationShow.someOrder}}</div>
            </div>
          </li>
          <li v-show="!(isShow.workingOrder !== 3 && isShow.stockDetail)" class="list"/>
        </ul>
        <div>
          <el-tabs class="tab-height" v-model="activeName">
            <el-tab-pane class="pane-height pane-notification" :label="tabLabel.stock" name="stock">
              <!-- 内容 -->
              <div class="content-wrap">
                <ul class="content-header">
                  <li class="list number">物资ID</li>
                  <li class="list name">物资名称</li>
                  <li class="list name">规格型号</li>
                  <li class="list name">保质期</li>
                  <li class="list name">供应商</li>
                  <li v-show="isShow.scrap" class="list name">仓库</li>
                  <li class="list name">库位</li>
                  <li v-show="isShow.stockDetail && !isShow.scrap" class="list name">{{fromStockOperationName}}数量</li>
                  <li v-show="isShow.stockDetail && !isShow.scrap" class="list name">已{{editNoName}}</li>
                  <li v-show="!isShow.stockDetail" class="list name">库存数量</li>
                  <li v-show="!isShow.stockDetail || isShow.scrap" class="list name">故障件数量</li>
                  <li v-show="!isShow.stockDetail || isShow.scrap" class="list name">存货数量</li>
                  <li v-show="!isShow.stockDetail" class="list name">已送检数量</li>
                  <li v-show="!isShow.stockDetail" class="list name">已送修数量</li>
                  <li v-show="!isShow.stockDetail" class="list name">已借用数量</li>
                  <li class="list menuOrder" v-show="isShowNewEntity">新物资ID</li>
                  <li class="list menuOrder">{{editNoName}}</li>
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
                          <div v-show="isShow.scrap" class="name">{{ item.warehouseName}}</div>
                          <div v-show="!(isShow.stockDetail && !isShow.scrap && item.inStockNo===0)" class="name">{{ item.storageLocationName}}</div>
                          <div class="name" v-show="isShow.stockDetail && !isShow.scrap && item.inStockNo===0">
                            <el-select v-model="item.storageLocation" placeholder="请选择库位" filterable>
                              <el-option
                                v-for="item in storageLocationList"
                                :key="item.key"
                                :label="item.name"
                                :value="item.id">
                              </el-option>
                            </el-select>
                          </div>
                          <div v-show="isShow.stockDetail && !isShow.scrap" class="name">{{ item.countNo }}</div>
                          <div v-show="isShow.stockDetail && !isShow.scrap" class="name">{{ item.returnNo }}</div>
                          <div v-show="!isShow.stockDetail" class="name">{{ item.stockNo }}</div>
                          <div v-show="!isShow.stockDetail || isShow.scrap" class="name">{{ item.troubleNo }}</div>
                          <div v-show="!isShow.stockDetail || isShow.scrap" class="name">{{ item.inStockNo }}</div>
                          <div v-show="!isShow.stockDetail" class="name">{{ item.inspectionNo }}</div>
                          <div v-show="!isShow.stockDetail" class="name">{{ item.repairNo }}</div>
                          <div v-show="!isShow.stockDetail" class="name">{{ item.lentNo }}</div>
                          <div class="menuOrder" v-show="isShow.stockDetail && !isShow.scrap && item.inStockNo===0 && item.isBacth===1">
                            <el-input class="center" v-model="item.newEntity" @keyup.native="validateEntity(item.newEntity, detailList, index)"></el-input>
                          </div>
                          <div class="menuOrder">
                            <el-input class="center" v-model="item.editNo" @keyup.native="validateAllEditNo(item, index)"></el-input>
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
            <el-tab-pane class="pane-height pane-notification" :label="tabLabel.send" name="send">
              <!-- 内容 -->
              <div class="content-wrap">
                <ul class="content-header">
                  <li class="list name">物资ID</li>
                  <li class="list name">物资名称</li>
                  <li class="list name">库位</li>
                  <li class="list name">{{editNoCompareName}}</li>
                  <li class="list name">{{editNoName}}</li>
                  <li class="list name" v-show="isShowNewEntity">新物资ID</li>
                  <li class="list url">备注</li>
                  <li class="list last-maintainer">操作</li>
                </ul>
                <div class="scroll">
                  <el-scrollbar>
                    <ul class="list-wrap">
                      <li class="list" v-for="(item, index)  in distributionList" :key="item.key">
                        <div class="list-content">
                          <div class="name">{{ item.entity }}</div>
                          <div class="name">{{ item.sparePartsName }}</div>
                          <div class="name" v-show="!(isShow.stockDetail && !isShow.scrap && item.inStockNoOld===0)">{{ item.storageLocationName }}</div>
                          <div class="name" v-show="isShow.stockDetail && !isShow.scrap && item.inStockNoOld===0">
                            <el-select v-model="item.storageLocation" placeholder="请选择库位" filterable :disabled="!item.isEdit">
                              <el-option
                                v-for="item in storageLocationList"
                                :key="item.key"
                                :label="item.name"
                                :value="item.id">
                              </el-option>
                            </el-select>
                          </div>
                          <div class="name">{{ item.inStockNo }}</div>
                          <div v-show="!item.isEdit" class="name word-break">{{ item.countNo }}</div>
                          <div v-show="item.isEdit" class="name word-break">
                            <el-input class="center" v-model="item.countNo" @keyup.native="validateEditNo(item.inStockNo, item.countNo, index, distributionList)"></el-input>
                          </div>
                          <div class="name word-break" v-show="isShow.stockDetail && !isShow.scrap && item.inStockNoOld===0 && item.isBacth===1 && !item.isEdit">
                            {{item.newEntity}}
                          </div>
                          <div class="name word-break" v-show="isShow.stockDetail && !isShow.scrap && item.inStockNoOld===0 && item.isBacth===1 && item.isEdit">
                            <el-input class="center" v-model="item.newEntity" @keyup.native="validateEntity(item.newEntity, distributionList, index)"></el-input>
                          </div>
                          <div v-show="!item.isEdit" class="url word-break">{{ item.remark }}</div>
                          <div v-show="item.isEdit" class="url word-break">
                            <el-input class="center" v-model="item.remark"></el-input>
                          </div>
                          <div class="last-maintainer word-break">
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
                  <router-link :to="{name: 'SeeStockDeliveryList'}">取消</router-link>
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
import { sparePartsOperationType, sparePartsOperationDetailType, troubleStatus } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import api from '@/api/wmsApi'
import apiAuth from '@/api/authApi'
import apiMain from '@/api/DeviceMaintainRegApi.js'
export default {
  name: 'AddStockDelivery',
  components: {
    XButton
  },
  data () {
    return {
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      activeName: 'stock',
      loading: false,
      title: '| 物资发放过账',
      isShowNewEntity: false,
      bCheckAll: false,
      reasonList: [],
      reason: {text: '', tips: ''},
      warehouseList: [],
      warehouse: {text: '', tips: ''},
      spareParts: {text: '', tips: ''},
      sparePartsList: '',
      pickerList: [],
      picker: {text: '', tips: ''},
      workingOrderList: [],
      workingOrder: {text: '', tips: ''},
      remark: {text: '', tips: ''},
      entity: '',
      entityList: [],
      stockOperation: '',
      stockOperationList: '',
      stockOperationShow: {
        warehouse: '',
        someOrder: ''
      },
      someOrder: {text: '', tips: ''},
      storageLocationList: [],
      detailList: [],
      distributionList: [],
      editID: [],
      editNoName: '发放数量',
      editNoCompareName: '存货数量',
      fromStockOperationName: '领料',
      someOrderName: '',
      tabLabel: {
        stock: '库存清单',
        send: '发放清单'
      },
      isShow: {
        picker: true,
        // 1:显示故障单；2:显示someOrder；3:都不显示
        workingOrder: 1,
        stockDetail: false,
        scrap: false
      },
      isDisabledWarehouse: false,
      warehouseTmp: ''
    }
  },
  created () {
    // 事务原因列表
    apiAuth.getSubCodeOrder(sparePartsOperationType.delivery).then(res => {
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
    // 故障单加载
    apiMain.getTroubleReportByStatus(troubleStatus.processing).then(res => {
      this.workingOrderList = res.data
    }).catch(err => console.log(err))
  },
  methods: {
    reasonChange () {
      this.reason.tips = ''
      if (this.reason.text === sparePartsOperationDetailType.inStockScrap || this.reason.text === sparePartsOperationDetailType.troubleScrap) {
        this.isShow.scrap = true
        // 物资ID加载
        api.getStockDetailByReason(this.reason.text).then(res => {
          this.entityList = res.data
        }).catch(err => console.log(err))
      } else this.isShow.scrap = false
      switch (this.reason.text) {
        case sparePartsOperationDetailType.distribution:
          this.isShow.picker = true
          this.isShow.workingOrder = 1
          this.isShow.stockDetail = false
          this.editNoName = '领料数量'
          this.editNoCompareName = '存货数量'
          this.tabLabel.stock = '库存清单'
          this.tabLabel.send = '领料清单'
          break
        case sparePartsOperationDetailType.materialReturn:
        case sparePartsOperationDetailType.troubleReturn:
          this.getStockOperationSelect(sparePartsOperationDetailType.distribution)
          this.fromStockOperationName = '领料'
          this.isShow.picker = true
          this.isShow.workingOrder = 1
          this.someOrderName = '故障单'
          this.isShow.stockDetail = true
          this.editNoName = '退回数量'
          this.editNoCompareName = '可退回数量'
          this.tabLabel.stock = '流水明细'
          this.tabLabel.send = '退回清单'
          break
        case sparePartsOperationDetailType.troubleRepair:
          this.isShow.picker = true
          this.isShow.workingOrder = 2
          this.someOrderName = '送修单'
          this.isShow.stockDetail = false
          this.editNoName = '送修数量'
          this.editNoCompareName = '故障件数量'
          this.tabLabel.stock = '库存清单'
          this.tabLabel.send = '送修清单'
          break
        case sparePartsOperationDetailType.materialLend:
          this.isShow.picker = true
          this.isShow.workingOrder = 3
          this.isShow.stockDetail = false
          this.editNoName = '借用数量'
          this.editNoCompareName = '存货数量'
          this.tabLabel.stock = '库存清单'
          this.tabLabel.send = '借用清单'
          break
        case sparePartsOperationDetailType.lendReturn:
          this.getStockOperationSelect(sparePartsOperationDetailType.materialLend)
          this.fromStockOperationName = '借用'
          this.isShow.picker = true
          this.isShow.workingOrder = 3
          this.isShow.stockDetail = true
          this.editNoName = '归还数量'
          this.editNoCompareName = '未归还数量'
          this.tabLabel.stock = '流水明细'
          this.tabLabel.send = '归还清单'
          break
        case sparePartsOperationDetailType.inStockScrap:
          this.isShow.picker = false
          this.isShow.workingOrder = 3
          this.isShow.stockDetail = true
          this.editNoName = '报废数量'
          this.editNoCompareName = '存货数量'
          this.tabLabel.stock = '物资明细'
          this.tabLabel.send = '报废清单'
          break
        case sparePartsOperationDetailType.troubleScrap:
          this.isShow.picker = false
          this.isShow.workingOrder = 3
          this.isShow.stockDetail = true
          this.editNoName = '报废数量'
          this.editNoCompareName = '故障件数量'
          this.tabLabel.stock = '物资明细'
          this.tabLabel.send = '报废清单'
          break
        case sparePartsOperationDetailType.repairReceive:
          this.getStockOperationSelect(sparePartsOperationDetailType.troubleRepair)
          this.fromStockOperationName = '送修'
          this.isShow.picker = true
          this.isShow.workingOrder = 2
          this.someOrderName = '送修单'
          this.isShow.stockDetail = true
          this.editNoName = '归还数量'
          this.editNoCompareName = '未归还数量'
          this.tabLabel.stock = '流水明细'
          this.tabLabel.send = '归还清单'
          break
        case sparePartsOperationDetailType.inspection:
          this.isShow.picker = true
          this.isShow.workingOrder = 2
          this.someOrderName = '送检单'
          this.isShow.stockDetail = false
          this.editNoName = '送检数量'
          this.editNoCompareName = '存货数量'
          this.tabLabel.stock = '库存清单'
          this.tabLabel.send = '送检清单'
          break
        case sparePartsOperationDetailType.inspectionReturn:
          this.getStockOperationSelect(sparePartsOperationDetailType.inspection)
          this.fromStockOperationName = '送检'
          this.isShow.picker = true
          this.isShow.workingOrder = 2
          this.someOrderName = '送检单'
          this.isShow.stockDetail = true
          this.editNoName = '归还数量'
          this.editNoCompareName = '未归还数量'
          this.tabLabel.stock = '流水明细'
          this.tabLabel.send = '归还清单'
          break
      }
      this.detailList = []
      this.distributionList = []
      this.warehouseTmp = ''
      this.isDisabledWarehouse = false
      this.activeName = 'stock'
      this.stockOperation = ''
      this.warehouse.text = ''
      this.spareParts.text = ''
    },
    getStockOperationSelect (reason) {
      // 流水号下拉
      api.getStockOperationByReason(reason).then(res => {
        this.stockOperationList = res.data
      }).catch(err => console.log(err))
    },
    warehouseChange () {
      this.spareParts.text = ''
      if (!this.validateSelect(this.warehouse)) return
      // 根据仓库找物资
      api.getSparePartsByWH(this.warehouse.text).then(res => {
        this.sparePartsList = res.data
      }).catch(err => console.log(err))
    },
    getStockDetail () {
      if (!this.validateSelect(this.reason) || !this.validateSelect(this.spareParts) || !this.validateSelect(this.warehouse)) {
        return
      }
      // 库存明细 获取库存清单按钮
      api.getStockDetail(this.spareParts.text, this.warehouse.text, this.reason.text, 0).then(res => {
        this.detailList = res.data
        if (this.detailList.length === 0) {
          this.$message({
            message: '批量的物资不允许进行此事务原因的操作',
            type: 'warning'
          })
        }
        this.activeName = 'stock'
      }).catch(err => console.log(err))
    },
    getStockDetailByID () {
      // if (!this.validateSelect(this.reason) || !this.validateSelect(this.stockDetail)) {
      //   return
      // }
      // 存货明细 报废时获取的数据
      api.getStockDetailByID(this.entity).then(res => {
        if (res.data !== null) {
          this.detailList = []
          this.detailList.push(res.data)
          this.activeName = 'stock'
        } else {
          this.$message({
            message: '没有找到对应的物资明细',
            type: 'warning'
          })
        }
      }).catch(err => console.log(err))
    },
    getStockOperationDetail () {
      // 流水号明细 归还时获取的数据
      api.getStockOperationDetailByIDForEdit(this.stockOperation).then(res => {
        if (res.data === null) {
          this.detailList = []
          this.$message({
            message: '此流水操作的物资都已归还',
            type: 'success'
          })
          return
        }
        this.detailList = res.data.detailList
        this.isShowNewEntity = this.detailList.some(item => {
          return item.inStockNo === 0 && item.isBacth === 1
        })
        this.stockOperationShow.warehouse = res.data.warehouseName
        this.warehouseTmp = res.data.warehouse
        api.getStorageLocationByWarehouse(this.warehouseTmp).then(res => {
          this.storageLocationList = res.data
        }).catch(err => console.log(err))
        if (this.isShow.workingOrder === 2) {
          this.stockOperationShow.someOrder = this.detailList[0].someOrder
        } else if (this.isShow.workingOrder === 1) {
          this.stockOperationShow.someOrder = this.detailList[0].workingOrder
        }
      }).catch(err => console.log(err))
    },
    validateEditNo (old, now, index) {
      if (vNumber(now)) {
        // if (!this.isShow.stockDetail && now > old) {
        //   this.$message({
        //     message: this.editNoName + '不可大于' + this.editNoCompareName,
        //     type: 'warning'
        //   })
        //   this.distributionList[index].countNo = 0
        // } else if (this.isShow.stockDetail && now > old) {
        //   if (this.reason.text === sparePartsOperationDetailType.lendReturn ||
        //   this.reason.text === sparePartsOperationDetailType.repairReceive ||
        //   this.reason.text === sparePartsOperationDetailType.inspectionReturn ||
        //   this.reason.text === sparePartsOperationDetailType.troubleScrap ||
        //   this.reason.text === sparePartsOperationDetailType.inStockScrap) {
        //     this.$message({
        //       message: this.editNoName + '不可大于' + this.editNoCompareName,
        //       type: 'warning'
        //     })
        //     this.distributionList[index].countNo = 0
        //   }
        // }
        if (now > old) {
          this.$message({
            message: this.editNoName + '不可大于' + this.editNoCompareName,
            type: 'warning'
          })
          this.distributionList[index].countNo = 0
        }
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
        if (!this.isShow.stockDetail) {
          if ((this.editNoCompareName === '存货数量' && item.stockNo < item.editNo) ||
          (this.editNoCompareName === '故障件数量' && item.troubleNo < item.editNo)) {
            this.$message({
              message: this.editNoName + '不可大于' + this.editNoCompareName,
              type: 'warning'
            })
            this.detailList[index].editNo = 0
          }
        } else {
          let isError = false
          let msg = ''
          switch (this.reason.text) {
            case sparePartsOperationDetailType.materialReturn:
            case sparePartsOperationDetailType.troubleReturn:
            case sparePartsOperationDetailType.inspectionReturn:
            case sparePartsOperationDetailType.lendReturn:
            case sparePartsOperationDetailType.repairReceive:
              if (item.countNo - item.returnNo < item.editNo) {
                isError = true
                msg = '总和'
              }
              break
            case sparePartsOperationDetailType.troubleScrap:
              if (item.troubleNo < item.editNo) isError = true
              break
            case sparePartsOperationDetailType.inStockScrap:
              if (item.stockNo < item.editNo) isError = true
              break
          }
          if (isError) {
            this.$message({
              message: this.editNoName + msg + '不可大于' + this.editNoCompareName,
              type: 'warning'
            })
            this.detailList[index].editNo = 0
          }
        }
      } else {
        this.$message({
          message: '请输入数字',
          type: 'warning'
        })
        this.detailList[index].editNo = 0
      }
    },
    validateEntity (entity, list, index) {
      let arr = [].concat(list)
      arr.splice(index, 1)
      let ret = arr.some(item => {
        return item.entity === entity
      })
      if (ret) {
        this.$message({
          message: '物资ID重复',
          type: 'warning'
        })
      }
    },
    insert () {
      if (!this.isShow.stockDetail) {
        if (this.isShow.workingOrder === 1) {
          if (!this.validateSelect(this.workingOrder)) {
            this.$message({
              message: '验证失败，请查看提示信息',
              type: 'warning'
            })
            return
          }
        } else if (this.isShow.workingOrder === 2) {
          if (!this.validateInput(this.someOrder)) {
            this.$message({
              message: '验证失败，请查看提示信息',
              type: 'warning'
            })
            return
          }
        }
      }
      let isInsert = false
      let first = true
      for (let i = 0; i < this.detailList.length; i++) {
        if (this.detailList[i].editNo + '' !== '0') {
          if (this.isShow.scrap) {
            if (this.warehouseTmp === '') {
              this.warehouseTmp = this.detailList[i].warehouse
            } else if (this.warehouseTmp !== this.detailList[i].warehouse) {
              this.$message({
                message: '退入库中的物资来源必须是（当前）同一个仓库',
                type: 'warning'
              })
              return
            }
          }
          let distribution = {
            entity: this.detailList[i].entity,
            newEntity: this.detailList[i].newEntity,
            spareParts: this.detailList[i].spareParts,
            sparePartsName: this.detailList[i].sparePartsName,
            // 目前只是为了判断是否可以改变库位，批量改变库位时需要定义物资ID（存货为0才行）
            inStockNoOld: this.detailList[i].inStockNo,
            isBacth: this.detailList[i].isBacth,
            countNo: this.detailList[i].editNo,
            remark: this.detailList[i].editRemark,
            // stockDetail: this.detailList[i].id,
            // acceptUnitPrice: this.detailList[i].acceptUnitPrice,
            exchangeRate: this.detailList[i].exchangeRate,
            storageLocation: this.detailList[i].storageLocation,
            storageLocationName: this.detailList[i].storageLocationName,
            fromStorageLocation: this.detailList[i].fromStorageLocation === undefined ? this.detailList[i].storageLocation : this.detailList[i].fromStorageLocation,
            isEdit: false
          }
          if (this.isShow.workingOrder === 1) distribution.workingOrder = this.workingOrder.text
          else if (this.isShow.workingOrder === 2) {
            distribution.someOrder = this.someOrder.text
          }
          switch (this.reason.text) {
            case sparePartsOperationDetailType.distribution:
            case sparePartsOperationDetailType.materialLend:
            case sparePartsOperationDetailType.troubleRepair:
            case sparePartsOperationDetailType.inspection:
            case sparePartsOperationDetailType.troubleScrap:
            case sparePartsOperationDetailType.inStockScrap:
              let has = this.distributionList.some(item => {
                return this.detailList[i].entity === item.entity
              })
              if (has) {
                this.$message({
                  message: this.tabLabel.send + '中已存在物资ID:' + this.detailList[i].entity,
                  type: 'warning'
                })
                return
              }
              distribution.stockDetail = this.detailList[i].id
              if (this.reason.text === sparePartsOperationDetailType.troubleRepair || this.reason.text === sparePartsOperationDetailType.troubleScrap) distribution.inStockNo = this.detailList[i].troubleNo
              else distribution.inStockNo = this.detailList[i].inStockNo
              distribution.unitPrice = this.detailList[i].acceptUnitPrice
              isInsert = true
              break
            default:
              distribution.FromStockOperationDetail = this.detailList[i].id
              distribution.inStockNo = this.detailList[i].countNo - this.detailList[i].returnNo
              distribution.unitPrice = this.detailList[i].unitPrice
              distribution.stockDetail = this.detailList[i].stockDetail
              this.activeName = 'send'
              if (first) {
                this.distributionList = []
                first = false
              }
              break
          }
          this.distributionList.push(distribution)
        }
      }
      if (isInsert) {
        this.isDisabledWarehouse = true
        this.$message({
          message: '所选物资已进入' + this.tabLabel.send + ',确认所有后，请切换标签页执行',
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
        this.warehouseTmp = ''
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
      if (val.text === '' || val.text.length === 0) {
        val.tips = '此项必选'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateInputAll () {
      let ret = true
      if (this.isShow.picker) {
        ret = this.validateSelect(this.picker)
      }
      return ret
    },
    save () {
      if (this.distributionList.length === 0) {
        this.$message({
          message: '请从库存清单中选择发放物资',
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
      // this.distributionList.map((val, index) => {
      //   val.orderNo = index + 1
      // })
      for (let i = 0; i < this.distributionList.length; i++) {
        let item = this.distributionList[i]
        if (this.isShow.stockDetail && !this.isShow.scrap && item.inStockNo === 0 && item.isBacth === 1) {
          if (item.entity === '') {
            this.$message({
              message: '第' + i + '条记录的物资ID必填',
              type: 'error'
            })
            return
          }
          if (!vInput(item.entity)) {
            this.$message({
              message: '第' + i + '条记录的物资ID中有非法字符串',
              type: 'error'
            })
            return
          }
        }
        if (!vInput(item.remark)) {
          this.$message({
            message: '第' + i + '条记录的备注中有非法字符串',
            type: 'error'
          })
          return
        }
        if (item.countNo + '' === '0') {
          this.$message({
            message: this.editNoName + '不可为0',
            type: 'error'
          })
          return
        }
        item.unitPrice = item.unitPrice
        item.amount = item.countNo * item.unitPrice * item.exchangeRate
        item.orderNo = i + 1
      }
      let stockDelivery = {
        Type: sparePartsOperationType.delivery,
        Reason: this.reason.text,
        Warehouse: this.warehouse.text,
        // StorageLocation: this.warehouse.text[1],
        Remark: this.remark.text,
        Picker: this.picker.text,
        DetailList: JSON.stringify(this.distributionList)
      }
      if (this.isShow.stockDetail || this.isShow.scrap) {
        stockDelivery.Warehouse = this.warehouseTmp
        stockDelivery.FromStockOperation = this.stockOperation
      }
      api.addStockOperation(stockDelivery).then(res => {
        if (res.code === 0) {
          this.$router.push({name: 'SeeStockDeliveryList'})
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
