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
    <div class="box1">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
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
            <label for="name">物资</label>
            <div class="inp">
              <el-select v-model="spareParts" clearable filterable placeholder="请先选择物资">
                <el-option
                  v-for="item in sparePartsList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="name">预警日期</label>
            <div class="inp">
              <el-date-picker
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
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list name c-pointer" @click="changeOrder('warehouse')">
          仓库
          <i :class="[{ 'el-icon-d-caret': headOrder.warehouse === 0 }, { 'el-icon-caret-top': headOrder.warehouse === 1 }, { 'el-icon-caret-bottom': headOrder.warehouse === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('spare_parts')">
          物资
          <i :class="[{ 'el-icon-d-caret': headOrder.spare_parts === 0 }, { 'el-icon-caret-top': headOrder.spare_parts === 1 }, { 'el-icon-caret-bottom': headOrder.spare_parts === 2 }]"></i>
        </li>
        <li class="list name">库存数量</li>
        <li class="list name">预警阈值</li>
        <li class="list last-update-time c-pointer" @click="changeOrder('created_time')">
          创建时间
          <i :class="[{ 'el-icon-d-caret': headOrder.created_time === 0 }, { 'el-icon-caret-top': headOrder.created_time === 1 }, { 'el-icon-caret-bottom': headOrder.created_time === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="item in warehouseAlarmHistoryList" :key="item.key">
              <div class="list-content">
                <div class="name">{{ item.warehouseName }}</div>
                <div class="name word-break">{{ item.sparePartsName }}</div>
                <div class="name word-break">{{ item.stockNo }}</div>
                <div class="name word-break">{{ item.safeStorage }}</div>
                <div class="last-update-time color-white word-break">{{ item.createdTime }}</div>
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
import XButton from '@/components/button'
import api from '@/api/wmsApi'
export default {
  name: 'List',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 预警历史',
      warehouse: '',
      warehouseList: [],
      spareParts: '',
      sparePartsList: [],
      warehouseAlarmHistoryList: [],
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'ID',
        order: 'asc'
      },
      headOrder: {
        warehouse: 0,
        spare_parts: 0,
        created_time: 0
      },
      searchDate: []
    }
  },
  created () {
    // 仓库加载
    api.getWarehouseAll().then(res => {
      this.warehouseList = res.data
    }).catch(err => console.log(err))
    // 物资加载
    api.getSparePartsAll().then(res => {
      this.sparePartsList = res.data
    }).catch(err => console.log(err))
    this.init()
  },
  methods: {
    init () {
      this.currentPage = 1
      this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.warehouse = 0
        this.headOrder.spare_parts = 0
        this.headOrder.created_time = 0
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
      if (this.searchDate != null && this.searchDate.length !== 0) {
        st = this.searchDate[0] + ' 00:00:00'
        et = this.searchDate[1] + ' 23:59:59'
      }
      api.getWarehouseAlarmHistory({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        SearchType: this.warehouse,
        SearchSpareParts: this.spareParts,
        SearchStart: st,
        SearchEnd: et
      }).then(res => {
        this.loading = false
        if (res.data.total === 0) {
          this.warehouseAlarmHistoryList = []
        } else {
          res.data.rows.map(item => {
            item.createdTime = transformDate(item.createdTime)
          })
          this.warehouseAlarmHistoryList = res.data.rows
        }
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    // 搜索功能
    searchRes () {
      this.$emit('title', '| 预警历史')
      this.loading = true
      this.init()
      // this.searchResult(1)
    },
    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.currentPage = val
      this.searchResult(val)
    },

    // 上一页
    prevPage (val) {
      this.currentPage = val
    },

    // 下一页
    nextPage (val) {
      this.currentPage = val
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

/deep/ .box1{
  height: percent(45, $content-height);

  // 搜索组
  .search-wrap{
    display: flex;
    justify-content: space-between;
    align-items: center;
    // height: percent(80, 145);
    height: 100%!important;
    background: rgba(128, 128, 128, 0.1);
    color: $color-white;

    .wrap{
      display: flex;
    }

    .input-group{
      display: inherit;
      align-items: center;
      margin-right: PXtoEm(24);
    }

    .inp{
      width: PXtoEm(160);
      margin-left: PXtoEm(14);
    }

    .btn{
      border: 0;
      background: $color-main-btn;
    }
  }
}

/deep/
.el-range-separator{
  color: #fff!important;
  padding-bottom: 10px!important;
}
/deep/
.el-range-input{
  color: #fff!important;
}
/deep/
.el-range__icon{
  padding-bottom: 10px!important;
}
/deep/
.el-range__close-icon{
  padding-bottom: 10px!important;
}
</style>
