<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeStockReceiveList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul v-show="!isAdd" class="con-padding-horizontal input-group">
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
          <li class="list" v-show="isShowPurchaseReturn">
            <div class="inp-wrap">
              <span class="text">采购接收流水号<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="stockOperation" filterable placeholder="请选择采购接收流水号" @change="operationChange">
                <el-option
                  v-for="item in stockOperationList"
                  :key="item.key"
                  :label="item.operation_id"
                  :value="item.id">
                </el-option>
              </el-select>
              </div>
            </div>
          </li>
          <li class="list" v-show="!isShowPurchaseReturn"/>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">仓库<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select :disabled="isShowPurchaseReturn" v-model="warehouse.text" filterable placeholder="请选择仓库" @change="validateSelect(warehouse)">
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
              <span class="text">合同号</span>
              <div class="inp">
                <el-input placeholder="请输入合同号" :disabled="isShowPurchaseReturn" v-model="agreement.text" @keyup.native="validateInputNull(agreement)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ agreement.tips }}</p>
          </li>
          <li class="list"/>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">预算部门</span>
              <div class="inp">
                <el-cascader class="cascader_width" clearable ref='dept'
                  :disabled="isShowPurchaseReturn"
                  expand-trigger="hover"
                  change-on-select
                  :props="defaultParams"
                  @change="cascader_change"
                  :show-all-levels="true"
                  :options="deptList"
                  v-model="deptPath.text">
                </el-cascader>
              </div>
            </div>
            <p class="validate-tips">{{ deptPath.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">预算项目</span>
              <div class="inp">
                <el-input placeholder="请输入预算项目" :disabled="isShowPurchaseReturn" v-model="budgetItems.text" @keyup.native="validateInputNull(budgetItems)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ budgetItems.tips }}</p>
          </li>
          <li class="list"/>
          <li class="list list-block">
            <div class="inp-wrap">
              <span class="text span-block">备注</span>
              <el-input type="textarea" :rows="1" class="whole-line" placeholder="请输入备注" :disabled="isShowPurchaseReturn" v-model="remark.text" @keyup.native="validateInputNull(remark)"></el-input>
            </div>
            <p class="validate-tips">{{ remark.tips }}</p>
          </li>
        </ul>
        <ul v-show="isAdd" class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">物资<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select ref="spareParts" v-model="spareParts.text" clearable filterable placeholder="请选择物资" @change="validateSelect(spareParts)">
                <el-option
                  v-for="item in sparePartsList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ spareParts.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">物资ID<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input placeholder="请输入物资ID" v-model="entity.text" @keyup.native="validateInput(entity)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ entity.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">数量<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input placeholder="请输入接收数量" v-model="countNo.text" @keyup.native="validateNumber(countNo)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ countNo.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">单价<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input placeholder="请输入单价" v-model="unitPrice.text" @keyup.native="validateDouble2(unitPrice)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ unitPrice.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">币种<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select ref="currency" v-model="currency" placeholder="请选择币种">
                  <el-option
                    v-for="item in currencyList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.id">
                  </el-option>
                </el-select>
              </div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">汇率<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input placeholder="请输入汇率" v-model="exchangeRate.text" @keyup.native="validateDouble4(exchangeRate)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ exchangeRate.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">发票号</span>
              <div class="inp">
                <el-input placeholder="请输入发票号" v-model="invoice.text" @keyup.native="validateInputNull(invoice)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ invoice.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">供应商</span>
              <div class="inp">
                <el-select v-model="supplier.text" clearable filterable placeholder="请选择供应商" ref="supplier">
                <el-option
                  v-for="item in supplierList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ supplier.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">保质期</span>
              <div class="inp">
                <el-date-picker class="el-date-width"
                  v-model="lifeDate"
                  type="date"
                  value-format="yyyy-MM-dd"
                  placeholder="请选择保质期">
                </el-date-picker>
              </div>
            </div>
            <p class="validate-tips"></p>
          </li>
          <li class="list list-block">
            <div class="inp-wrap">
              <span class="text span-block">备注</span>
              <el-input type="textarea" :rows="1" class="whole-line" placeholder="请输入备注" v-model="remarkAdd.text" @keyup.native="validateInputNull(remarkAdd)"></el-input>
            </div>
            <p class="validate-tips">{{ remarkAdd.tips }}</p>
          </li>
        </ul>
        <ul v-show="!isAdd" class="con-padding-horizontal btn-group">
          <li class="list" @click="add"><x-button :disabled="isShowPurchaseReturn">添加物资</x-button></li>
          <li class="list" @click="remove"><x-button :disabled="isShowPurchaseReturn">删除物资</x-button></li>
          <li class="list" @click="edit"><x-button :disabled="isShowPurchaseReturn">修改物资</x-button></li>
        </ul>
        <div v-show="isAdd" class="btn-commit-group">
          <x-button class="close" @click.native="cancel">返回</x-button>
          <x-button class="active" @click.native="insert">确认</x-button>
        </div>
        <!-- 采购接收/其他接收内容 -->
        <div class="content-wrap" v-show="!isShowPurchaseReturn">
          <ul class="content-header">
            <li class="list"><input :disabled="isAdd" type="checkbox" v-model="bCheckAll" @change="checkAll"></li>
            <li class="list number">序号</li>
            <li class="list name">物资ID</li>
            <li class="list name">物资名称</li>
            <li class="list name">数量</li>
            <li class="list name">单价</li>
            <li class="list name">金额</li>
            <li class="list name">币种</li>
            <li class="list name">汇率</li>
            <li class="list name">本币总金额</li>
            <li class="list name">发票号</li>
            <li class="list name">供应商</li>
            <li class="list name">保质期</li>
            <li class="list name">备注</li>
          </ul>
          <div class="scroll">
            <el-scrollbar>
              <ul class="list-wrap">
                <li class="list" v-for="item in detailList" :key="item.key">
                  <div class="list-content">
                    <div class="checkbox">
                      <input :disabled="isAdd" type="checkbox" v-model="editID" :value="item.orderNo" @change="checkChange">
                    </div>
                    <div class="number">{{ item.orderNo}}</div>
                    <div class="name">{{ item.entity }}</div>
                    <div class="name">{{ item.sparePartsName }}</div>
                    <div class="name word-break">{{ item.countNo }}</div>
                    <div class="name word-break">{{ item.unitPrice }}</div>
                    <div class="name word-break">{{ item.amount }}</div>
                    <div class="name word-break">{{ item.currencyName }}</div>
                    <div class="name word-break">{{ item.exchangeRate }}</div>
                    <div class="name word-break">{{ item.totalAmount }}</div>
                    <div class="name word-break">{{ item.invoice }}</div>
                    <div class="name word-break">{{ item.supplierName }}</div>
                    <div class="name word-break">{{ item.lifeDate }}</div>
                    <div class="name word-break">{{ item.remark }}</div>
                  </div>
                </li>
              </ul>
            </el-scrollbar>
          </div>
        </div>
        <!-- 采购退货内容 -->
        <div class="content-wrap" v-show="isShowPurchaseReturn">
          <ul class="content-header">
            <li class="list name">物资ID</li>
            <li class="list name">接收物资</li>
            <li class="list name">所在仓库</li>
            <li class="list name">供应商</li>
            <li class="list name">接收数量</li>
            <li class="list name">存货数量</li>
            <li class="list name">接收币种</li>
            <li class="list name">接收时汇率</li>
            <li class="list name">接收单价</li>
            <li class="list name">存货金额</li>
            <li class="list name">退货数量</li>
            <li class="list name">退货备注</li>
          </ul>
          <div class="scroll">
            <el-scrollbar>
              <ul class="list-wrap">
                <li class="list" v-for="(item, index) in detailList" :key="item.key">
                  <div class="list-content">
                    <div class="name">{{ item.entity}}</div>
                    <div class="name">{{ item.sparePartsName}}</div>
                    <div class="name">{{ item.warehouseName}}</div>
                    <div class="name">{{ item.supplierName}}</div>
                    <div class="name">{{ item.acceptNo }}</div>
                    <div class="name">{{ item.inStockNo }}</div>
                    <div class="name">{{ item.currencyName }}</div>
                    <div class="name">{{ item.exchangeRate }}</div>
                    <div class="name">{{ item.acceptUnitPrice.toFixed(2) }}</div>
                    <div class="name">{{ (item.acceptUnitPrice * item.inStockNo).toFixed(2) }}</div>
                    <div class="name">
                      <el-input class="center" v-model="item.editNo" @keyup.native="validateEditNo(item.inStockNo, item.editNo, index)"></el-input>
                    </div>
                    <div class="name word-break">
                      <el-input class="center" v-model="item.remark"></el-input>
                    </div>
                  </div>
                </li>
              </ul>
            </el-scrollbar>
          </div>
        </div>
        <!-- 按钮 -->
        <div v-show="!isAdd" class="btn-commit-group">
          <x-button class="close">
            <router-link :to="{name: 'SeeStockReceiveList'}">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="save">{{btnText}}</x-button>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { vInput, validateNumberCommon, vdouble4, vdouble2, vNumber, strToIntArr, validateInputCommon } from '@/common/js/utils.js'
import { sparePartsOperationType, firmType, dictionary, sparePartsOperationDetailType } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import apiOrg from '@/api/orgApi'
import api from '@/api/wmsApi'
import apiAuth from '@/api/authApi'
import apiEqp from '@/api/eqpApi'
export default {
  name: 'AddStockReceive',
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
      isShowPurchase: false,
      btnText: '接收',
      isShowPurchaseReturn: false,
      isAdd: false,
      loading: false,
      title: '| 物资接收过账',
      stockOperation: '',
      stockOperationList: [],
      bCheckAll: false,
      reasonList: [],
      reason: {text: '', tips: ''},
      pickerList: [],
      picker: {text: '', tips: ''},
      warehouseList: [],
      warehouse: {text: '', tips: ''},
      agreement: {text: '', tips: ''},
      deptList: [],
      deptPath: {text: [], tips: ''},
      budgetItems: {text: '', tips: ''},
      remark: {text: '', tips: ''},
      detailList: [],
      editID: [],

      // 新增数据
      currencyList: [],
      spareParts: {text: '', tips: ''},
      sparePartsList: '',
      entity: {text: '', tips: ''},
      countNo: {text: '', tips: ''},
      unitPrice: {text: '', tips: ''},
      currency: {text: '', tips: ''},
      exchangeRate: {text: 1, tips: ''},
      invoice: {text: '', tips: ''},
      lifeDate: '',
      supplierList: [],
      supplier: {text: '', tips: ''},
      remarkAdd: {text: '', tips: ''}
    }
  },
  created () {
    // 部门加载
    apiOrg.getOrgAll().then(res => {
      this.deptList = res.data
    }).catch(err => console.log(err))
    // 事务原因列表
    apiAuth.getSubCodeOrder(sparePartsOperationType.receive).then(res => {
      this.reasonList = res.data
    }).catch(err => console.log(err))
    // 币种
    apiAuth.getSubCode(dictionary.currency).then(res => {
      this.currencyList = res.data
      this.currency = this.currencyList[0].id
    }).catch(err => console.log(err))
    // 仓库加载
    api.getWarehouseAll().then(res => {
      this.warehouseList = res.data
    }).catch(err => console.log(err))
    // 物资加载
    api.getSparePartsAll().then(res => {
      this.sparePartsList = res.data
    }).catch(err => console.log(err))
    // 供应商加载
    apiEqp.getFirmByType(firmType.supplier).then(res => {
      this.supplierList = res.data
    }).catch(err => console.log(err))
    // 责任人加载
    apiAuth.getUserAll().then(res => {
      this.pickerList = res.data
    }).catch(err => console.log(err))
  },
  methods: {
    reasonChange () {
      this.reason.tips = ''
      if (this.reason.text === sparePartsOperationDetailType.purchaseReturn) {
        this.getStockOperationSelect()
        this.isShowPurchaseReturn = true
        this.btnText = '退货'
      } else {
        this.isShowPurchaseReturn = false
        this.btnText = '接收'
      }
      if (this.reason.text === sparePartsOperationDetailType.purchaseReceive) {
        this.isShowPurchase = true
      } else {
        this.isShowPurchase = false
      }
      this.detailList = []
      this.stockOperation = ''
    },
    getStockOperationSelect () {
      // 采购接收流水号下拉
      api.getStockOperationByReason(sparePartsOperationDetailType.purchaseReceive).then(res => {
        this.stockOperationList = res.data
      }).catch(err => console.log(err))
    },
    getstockOperation () {
      api.getStockOperationByID(this.stockOperation).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.warehouse.text = data.warehouse
          this.agreement.text = data.agreement
          this.deptPath.text = strToIntArr(data.budgetDeptPath)
          this.budgetItems.text = data.budgetItems
          this.remark.text = data.remark
        }
      }).catch(err => console.log(err))
    },
    operationChange () {
      if (this.stockOperation !== '') {
        this.getstockOperation()
        this.getStockOperationDetail()
      }
    },
    getStockOperationDetail () {
      // 采购接收流水号明细
      api.getStockOperationDetailByID(this.stockOperation).then(res => {
        this.detailList = res.data
      }).catch(err => console.log(err))
    },
    validateEditNo (old, now, index) {
      if (vNumber(now)) {
        if (now > old) {
          this.$message({
            message: '退货数量不可大于存货数量',
            type: 'warning'
          })
          this.detailList[index].editNo = 0
        }
      } else {
        this.$message({
          message: '请输入数字',
          type: 'warning'
        })
        this.detailList[index].editNo = 0
      }
    },
    insert () {
      if (!this.validateSelect(this.spareParts) || !this.validateNumber(this.countNo) || !this.validateDouble2(this.unitPrice) ||
        !this.validateDouble4(this.exchangeRate) || !this.validateInputNull(this.invoice) || !this.validateInputNull(this.remarkAdd)) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      let arr = this.detailList.concat([])
      if (this.title === '| 物资接收过账 | 修改物资明细') {
        let index = arr.findIndex(val => {
          return this.editID[0] === val.orderNo
        })
        arr.splice(index, 1)
      }
      let spID = ''
      let isRepeat = arr.some(val => {
        spID = val.entity
        return val.entity === this.entity.text
      })
      if (isRepeat) {
        this.$message({
          message: '物资ID：' + spID + ' 已存在，不可重复添加',
          type: 'warning'
        })
        return
      }
      let tmp = (this.countNo.text * this.unitPrice.text * 1).toFixed(2)
      let detail = {
        entity: this.entity.text,
        spareParts: this.spareParts.text,
        sparePartsName: this.$refs.spareParts.selected.label,
        countNo: this.countNo.text,
        unitPrice: (this.unitPrice.text * 1).toFixed(2),
        amount: tmp,
        currency: this.currency,
        currencyName: this.$refs.currency.selected.label,
        exchangeRate: this.exchangeRate.text,
        totalAmount: (this.exchangeRate.text * tmp).toFixed(2),
        invoice: this.invoice.text,
        lifeDate: this.lifeDate,
        supplier: this.supplier.text,
        supplierName: this.$refs.supplier.selected.label,
        remark: this.remarkAdd.text,
        // 以下两个字段在这里无用，只是为了不让采购退货中的toFixed(2)函数报错
        acceptUnitPrice: 1,
        inStockNo: 1
      }
      if (this.title === '| 物资接收过账 | 添加物资明细') {
        detail.orderNo = this.detailList.length + 1
        this.detailList.push(detail)
      } else {
        detail.orderNo = this.editID[0]
        this.detailList.splice(this.editID[0] - 1, 1, detail)
        this.isAdd = false
        this.title = '| 物资接收过账'
      }
    },
    cancel () {
      this.isAdd = false
      this.title = '| 物资接收过账'
    },
    add () {
      this.isAdd = true
      this.title = '| 物资接收过账 | 添加物资明细'
    },
    remove () {
      this.editID.reverse().map(val => {
        this.detailList.splice(val - 1, 1)
      })
      this.detailList.map((val, index) => {
        val.orderNo = index + 1
      })
      this.editID = []
    },
    edit () {
      if (!this.editID.length) {
        this.$message({
          message: '请选择修改的物资',
          type: 'warning'
        })
      } else if (this.editID.length > 1) {
        this.$message({
          message: '修改的物资不能超过1个',
          type: 'warning'
        })
      } else {
        this.isAdd = true
        this.title = '| 物资接收过账 | 修改物资明细'
        let editObj = this.detailList.find(val => {
          return val.orderNo === this.editID[0]
        })
        this.entity.text = editObj.entity
        this.spareParts.text = editObj.spareParts
        this.countNo.text = editObj.countNo
        this.unitPrice.text = editObj.unitPrice
        this.currency = editObj.currency
        this.exchangeRate.text = editObj.exchangeRate
        this.invoice.text = editObj.invoice
        this.remarkAdd.text = editObj.remark
      }
    },
    checkChange () {
      if (this.editID.length === 0) {
        this.bCheckAll = false
      }
    },
    // 全选
    checkAll () {
      this.bCheckAll ? this.detailList.map(val => this.editID.push(val.orderNo)) : this.editID = []
      // this.emitEditID()
    },
    cascader_change (val) {
      if (val.length === 0) {
        this.deptPath.tips = ''
        this.dept = ''
        return
      }
      let selectedDept = val[val.length - 1]
      let obj = this.getCascaderObj(selectedDept, this.deptList)
      if (obj.node_type === 2) {
        this.dept = selectedDept
        this.deptPath.tips = ''
      } else {
        this.deptPath.tips = '您选择的不是部门'
      }
    },
    getCascaderObj (val, opt) {
      for (let i = 0; i < opt.length; ++i) {
        let item = opt[i]
        if (val === item.id) {
          return item
        } else {
          if (item.children) {
            let ret = this.getCascaderObj(val, item.children)
            if (ret) {
              return ret
            }
          }
        }
      }
    },
    // 验证2位小数
    validateDouble2 (val) {
      val.tips = ''
      if (val.text.trim() !== '') {
        if (!vdouble2(val.text)) {
          val.tips = '此项必须为最多两位小数的浮点数'
          return false
        } else {
          return true
        }
      } else {
        val.tips = '此项必填'
        return false
      }
    },
    // 验证4位小数
    validateDouble4 (val) {
      val.tips = ''
      if (val.text !== '') {
        if (!vdouble4(val.text)) {
          val.tips = '此项必须为最多四位小数的浮点数'
          return false
        } else {
          return true
        }
      } else {
        val.tips = '此项必填'
        return false
      }
    },
    validateNumber (val) {
      if (val.text.trim() === '') {
        val.tips = '此项必填'
        return false
      }
      return validateNumberCommon(val)
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
      if (!this.validateSelect(this.reason) || !this.validateSelect(this.warehouse) ||
        !this.validateInputNull(this.agreement) || !this.validateInputNull(this.budgetItems) || !this.validateInputNull(this.remark)) {
        return false
      }
      return true
    },
    save () {
      if (this.detailList.length === 0) {
        let msg = this.isShowPurchaseReturn ? '请选择退货物资及其数量' : '请添加物资明细'
        this.$message({
          message: msg,
          type: 'warning'
        })
        return
      }
      if (!this.isShowPurchaseReturn) {
        if (!this.validateInputAll() || this.deptPath.tips !== '') {
          this.$message({
            message: '验证失败，请查看提示信息',
            type: 'error'
          })
          return
        }
      }
      let stockReceive = {
        Type: sparePartsOperationType.receive,
        Reason: this.reason.text,
        Picker: this.picker.text,
        Warehouse: this.warehouse.text,
        Remark: this.remark.text,
        Agreement: this.agreement.text,
        BudgetDept: this.deptPath.text[this.deptPath.text.length - 1],
        BudgetDeptPath: this.deptPath.text.join(','),
        BudgetItems: this.budgetItems.text,
        DetailList: JSON.stringify(this.detailList)
      }
      if (this.isShowPurchaseReturn) {
        stockReceive.FromStockOperation = this.stockOperation
        let tmp = []
        let i = 0
        this.detailList.map(val => {
          if ((val.editNo + '') !== '0') {
            i += 1
            let obj = {
              orderNo: i,
              entity: val.entity,
              stockDetail: val.id,
              spareParts: val.spareParts,
              countNo: val.editNo,
              unitPrice: val.acceptUnitPrice,
              amount: (val.acceptUnitPrice * val.editNo * val.exchangeRate).toFixed(2),
              remark: val.remark,
              fromStockOperationDetail: val.fromStockOperationDetail
            }
            tmp.push(obj)
          }
        })
        if (tmp.length === 0) {
          this.$message({
            message: '至少一种物资退货数量非0',
            type: 'error'
          })
          return
        }
        stockReceive.DetailList = JSON.stringify(tmp)
      }
      if (this.$route.query.type === 'Add') {
        api.addStockOperation(stockReceive).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeStockReceiveList'})
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
.el-date-width{
  width: 93%!important;
}
/deep/
.center .el-input__inner{
  text-align: center;
}
</style>
