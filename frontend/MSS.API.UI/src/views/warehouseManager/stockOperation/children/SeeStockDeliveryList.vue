<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
    </div>
    <div class="box">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
          <div class="input-group">
            <label for="name">事务原因</label>
            <div class="inp">
              <el-select v-model="reason" clearable filterable placeholder="请选择事务原因">
                <el-option
                  v-for="item in reasonList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="name">仓库</label>
            <div class="inp">
              <el-select v-model="warehouse" clearable filterable placeholder="请选择仓库">
                <el-option
                  v-for="item in warehouseList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="name">责任人</label>
            <div class="inp">
              <el-select v-model="picker" clearable filterable placeholder="请选择责任人">
                <el-option
                  v-for="item in pickerList"
                  :key="item.key"
                  :label="item.user_name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="name">过账日期</label>
            <div class="inp">
              <el-date-picker
                class="middle"
                v-model="searchDate"
                type="daterange"
                prefix-icon="el-icon-date"
                :unlink-panels="true"
                value-format="yyyy-MM-dd"
                range-separator="至"
                start-placeholder="开始日期"
                end-placeholder="结束日期">
              </el-date-picker>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="add"><x-button :disabled="btn.save">发放过账</x-button></li>
        <li class="list" @click="detail"><x-button>查看明细</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list number"></li>
        <li class="list name c-pointer" @click="changeOrder('operation_id')">
          发放流水号
          <i :class="[{ 'el-icon-d-caret': headOrder.operation_id === 0 }, { 'el-icon-caret-top': headOrder.operation_id === 1 }, { 'el-icon-caret-bottom': headOrder.operation_id === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('reason')">
          事务原因
          <i :class="[{ 'el-icon-d-caret': headOrder.reason === 0 }, { 'el-icon-caret-top': headOrder.reason === 1 }, { 'el-icon-caret-bottom': headOrder.reason === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('warehouse')">
          仓库
          <i :class="[{ 'el-icon-d-caret': headOrder.warehouse === 0 }, { 'el-icon-caret-top': headOrder.warehouse === 1 }, { 'el-icon-caret-bottom': headOrder.warehouse === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('picker')">
          责任人
          <i :class="[{ 'el-icon-d-caret': headOrder.picker === 0 }, { 'el-icon-caret-top': headOrder.picker === 1 }, { 'el-icon-caret-bottom': headOrder.picker === 2 }]"></i>
        </li>
        <li class="list name">责任人部门</li>
        <li class="list last-update-time c-pointer" @click="changeOrder('created_time')">
          过账时间
          <i :class="[{ 'el-icon-d-caret': headOrder.created_time === 0 }, { 'el-icon-caret-top': headOrder.created_time === 1 }, { 'el-icon-caret-bottom': headOrder.created_time === 2 }]"></i>
        </li>
        <li class="list last-update-time c-pointer" @click="changeOrder('created_by')">
          经办人
          <i :class="[{ 'el-icon-d-caret': headOrder.created_by === 0 }, { 'el-icon-caret-top': headOrder.created_by === 1 }, { 'el-icon-caret-bottom': headOrder.created_by === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="item in stockDeliveryList" :key="item.key">
              <div class="list-content">
                <div class="number">
                  <input type="radio" v-model="editStockDeliveryID" :value="item.id">
                </div>
                <div class="name">{{ item.operationID }}</div>
                <div class="name">{{ item.reasonName }}</div>
                <div class="name word-break">{{ item.warehouseName }}</div>
                <div class="name word-break">{{ item.pickerName }}</div>
                <div class="name word-break">{{ item.pickerDeptName }}</div>
                <div class="last-update-time color-white word-break">{{ item.createdTime }}</div>
                <div class="last-update-time word-break">{{ item.createdName }}</div>
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
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import { btn } from '@/element/btn.js'
import { sparePartsOperationType } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import api from '@/api/wmsApi'
import apiAuth from '@/api/authApi'
export default {
  name: 'SeeStockDeliveryList',
  components: {
    XButton
  },
  data () {
    return {
      btn: {
        save: false,
        delete: false,
        update: false
      },
      title: ' | 物资发放',
      reason: '',
      reasonList: [],
      warehouse: '',
      warehouseList: [],
      picker: '',
      pickerList: [],
      editStockDeliveryID: '',
      stockDeliveryList: [],
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'operation_id',
        order: 'desc'
      },
      headOrder: {
        operation_id: 2,
        reason: 0,
        warehouse: 0,
        picker: 0,
        created_time: 0,
        created_by: 0
      },
      searchDate: []
    }
  },
  created () {
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    if (!user.is_super) {
      let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
      this.btn.save = !actions.some((item, index) => {
        return item.actionID === btn.stockDelivery.save
      })
    }
    // 事务原因列表
    apiAuth.getSubCodeOrder(sparePartsOperationType.delivery).then(res => {
      this.reasonList = res.data
    }).catch(err => console.log(err))
    // 仓库加载
    api.getWarehouseAll().then(res => {
      this.warehouseList = res.data
    }).catch(err => console.log(err))
    // 责任人加载
    apiAuth.getUserAll().then(res => {
      this.pickerList = res.data
    }).catch(err => console.log(err))
    this.init()
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  methods: {
    init () {
      this.currentPage = 1
      // this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.operation_id = 0
        this.headOrder.reason = 0
        this.headOrder.warehouse = 0
        this.headOrder.picker = 0
        this.headOrder.created_time = 0
        this.headOrder.created_by = 0
        this.currentSort.order = 'asc'
        this.headOrder[sort] = 1
      } else if (this.headOrder[sort] === 2) { // 同一字段降序变升序
        this.currentSort.order = 'asc'
        this.headOrder[sort] = 1
      } else { // 同一字段升序变降序
        this.currentSort.order = 'desc'
        this.headOrder[sort] = 2
      }
      this.currentSort.sort = sort
      this.searchResult(this.currentPage)
    },
    // 搜索
    searchResult (page) {
      this.currentPage = page
      this.loading = true
      let st, et
      if (this.searchDate.length !== 0) {
        st = this.searchDate[0] + '00:00:00'
        et = this.searchDate[1] + '23:59:59'
      }
      api.getStockOperation({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        SearchType: sparePartsOperationType.delivery,
        SearchReason: this.reason,
        SearchWarehouse: this.warehouse,
        SearchPicker: this.picker,
        SearchStart: st,
        SearchEnd: et
      }).then(res => {
        this.loading = false
        if (res.data.total === 0) {
          this.stockDeliveryList = []
        } else {
          res.data.rows.map(item => {
            item.createdTime = transformDate(item.createdTime)
          })
          this.stockDeliveryList = res.data.rows
        }
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    add () {
      this.$router.push({name: 'AddStockDelivery', query: { type: 'Add' }})
    },
    detail () {
      if (this.editStockMoveID === '') {
        this.$message({
          message: '请选择需要查看的发放流水号',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'DetailStockOperation',
          params: {
            id: this.editStockDeliveryID,
            sourceName: 'SeeStockDeliveryList'
          }
        })
      }
    },
    // 搜索功能
    searchRes () {
      this.loading = true
      this.init()
      this.searchResult(1)
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
    padding: 0 PXtoEm(24);
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
    width: 1%;
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

// 图片
.pdf-btn{
  display: flex;
  justify-content: center;
  .box{
    position: relative;
    width: 60px;
    height: 30px;
    text-align: center;
    line-height: 30px;
    cursor: pointer;

    &:before{
      content: "\e6cc";
      font-size: 28px;
      font-family: "iconfont";
      color: #D8D8D8;
    }
  }
}

/deep/
.el-range-separator{
  color: #fff!important;
  padding-bottom: 10px;
}
/deep/
.el-range__icon{
  padding-bottom: 10px;
}
</style>
