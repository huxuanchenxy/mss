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
              <el-select v-model="spareParts" clearable filterable placeholder="请选择物资">
                <el-option
                  v-for="item in sparePartsList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="search">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="shrinkAllSubContent" ><x-button :disabled="isWarehouse">{{ shrinkAll.text }}</x-button></li>
        <li class="list" @click="detail" ><x-button>存货明细</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="height-full content-wrap">
      <ul class="content-header">
        <li class="btn-wrap">
          <i class="hide iconfont icon-arrow-top-f"></i>
        </li>
        <li class="list name">物资名称</li>
        <li class="list name c-pointer" @click="changeOrder('stock_no')">
          库存数量
          <i :class="[{ 'el-icon-d-caret': headOrder.stock_no === 0 }, { 'el-icon-caret-top': headOrder.stock_no === 1 }, { 'el-icon-caret-bottom': headOrder.stock_no === 2 }]"></i>
        </li>
        <li class="list name color-white c-pointer" @click="changeOrder('trouble_no')">
          故障件数量
          <i :class="[{ 'el-icon-d-caret': headOrder.trouble_no === 0 }, { 'el-icon-caret-top': headOrder.trouble_no === 1 }, { 'el-icon-caret-bottom': headOrder.trouble_no === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('in_stock_no')">
          存货数量
          <i :class="[{ 'el-icon-d-caret': headOrder.in_stock_no === 0 }, { 'el-icon-caret-top': headOrder.in_stock_no === 1 }, { 'el-icon-caret-bottom': headOrder.in_stock_no === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('inspection_no')">
          送检数量
          <i :class="[{ 'el-icon-d-caret': headOrder.inspection_no === 0 }, { 'el-icon-caret-top': headOrder.inspection_no === 1 }, { 'el-icon-caret-bottom': headOrder.inspection_no === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('repair_no')">
          送修数量
          <i :class="[{ 'el-icon-d-caret': headOrder.repair_no === 0 }, { 'el-icon-caret-top': headOrder.repair_no === 1 }, { 'el-icon-caret-bottom': headOrder.repair_no === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('amount')">
          总金额
          <i :class="[{ 'el-icon-d-caret': headOrder.amount === 0 }, { 'el-icon-caret-top': headOrder.amount === 1 }, { 'el-icon-caret-bottom': headOrder.amount === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item, index) in StockSumList" :key="item.key">
              <div class="list-content">
                <div class="btn-wrap">
                  <i v-show="!isWarehouse" title="展开" @click="shrinkSubContent(index, item.id, item.stocks.length)" class="iconfont icon-arrow-top-f" :class="{ active: item.isShowSubCon }"></i>
                  <input type="radio" v-model="editStockSum" :value="item.id">
                </div>
                <div class="name">{{ item.sparePartsName }}</div>
                <div class="name">{{ item.stockNo }}</div>
                <div class="name">{{ item.troubleNo }}</div>
                <div class="name">{{ item.inStockNo }}</div>
                <div class="name">{{ item.inspectionNo }}</div>
                <div class="name">{{ item.repairNo }}</div>
                <div class="name">{{ item.amount }}</div>
              </div>
              <ul class="sub-content" :class="{ active: item.isShowSubCon }">
                <!--<li class="sub-con-list" v-for="sub in item.action_trees" :key="sub.key">
                  <div class="left-title">{{ sub.text }}</div>
                  <ul class="right-wrap">
                    <li class="list" v-for="three in sub.children" :key="three.key">{{ three.text }}</li>
                  </ul>
                </li>-->
                <li class="sub-con-list">
                  <div class="list">
                    <div class="name">仓库名</div>
                    <div class="name">库存数量</div>
                    <div class="name">故障件数量</div>
                    <div class="name">存货数量</div>
                    <div class="name">送检数量</div>
                    <div class="name">送修数量</div>
                    <div class="name">总金额</div>
                  </div>
                </li>
                <li class="sub-con-list" v-for="sub in item.stocks" :key="sub.key">
                  <div class="list">
                    <div class="name">{{ sub.warehouseName }}</div>
                    <div class="name">{{ sub.stockNo }}</div>
                    <div class="name">{{ sub.troubleNo }}</div>
                    <div class="name">{{ sub.inStockNo }}</div>
                    <div class="name">{{ sub.inspectionNo }}</div>
                    <div class="name">{{ sub.repairNo }}</div>
                    <div class="name">{{ sub.amount }}</div>
                  </div>
                </li>
              </ul>
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
import XButton from '@/components/button'
import api from '@/api/wmsApi'
export default {
  name: 'SeeStockSumList',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 库存查询',
      isWarehouse: false,
      spareParts: '',
      sparePartsList: [],
      warehouseList: [],
      warehouse: '',
      StockSumList: [],
      editStockSum: [],
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'id',
        order: 'asc'
      },
      headOrder: {
        stock_no: 0,
        trouble_no: 0,
        in_stock_no: 0,
        inspection_no: 0,
        repair_no: 0,
        amount: 0
      },
      currentOpen: -1,
      shrinkAll: {
        text: '全部展开',
        mark: true
      }
    }
  },
  props: {
    random: Number
  },
  created () {
    this.init()
    // 物资加载
    api.getSparePartsAll().then(res => {
      this.sparePartsList = res.data
    }).catch(err => console.log(err))
    // 仓库加载
    api.getWarehouseAll().then(res => {
      this.warehouseList = res.data
    }).catch(err => console.log(err))
  },
  watch: {
    random () {
      this.shrinkAllSubContent()
    }
  },
  methods: {
    init () {
      this.currentPage = 1
      this.searchResult(1)
    },
    detail () {
      if (this.editStockSum === '') {
        this.$message({
          message: '请选择需要查看的物资',
          type: 'warning'
        })
      } else {
        let tmp = this.StockSumList.find(val => {
          return val.id === this.editStockSum
        })
        this.$router.push({
          name: 'StockDetail',
          params: {
            id: tmp.spareParts,
            warehouse: tmp.warehouse === undefined ? 0 : tmp.warehouse
            // sourceName: 'SeeStockAdjustList'
          }
        })
      }
    },
    // 改变排序
    changeOrder (sort) {
      // console.log(this.headOrder[sort])
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.stock_no = 0
        this.headOrder.trouble_no = 0
        this.headOrder.in_stock_no = 0
        this.headOrder.inspection_no = 0
        this.headOrder.repair_no = 0
        this.headOrder.amount = 0
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
    search () {
      this.searchResult(1)
    },
    // 搜索
    searchResult (page) {
      if (this.warehouse === '') this.isWarehouse = false
      else this.isWarehouse = true
      this.currentPage = page
      this.loading = true
      api.getStockSum({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        searchSpareParts: this.spareParts,
        SearchWarehouse: this.warehouse
      }).then(res => {
        this.loading = false
        res.data.rows.map(val => {
          val.isShowSubCon = false
        })
        this.total = res.data.total
        this.StockSumList = res.data.rows
      }).catch(err => console.log(err))
    },
    // 收起展开权限列表
    shrinkSubContent (index, id, length) {
      // this.StockSumList[index].isShowSubCon ? this.StockSumList[index].isShowSubCon = true : this.StockSumList[index].isShowSubCon = false
      // this.StockSumList[index].isShowSubCon = !this.StockSumList[index].isShowSubCon
      if (this.StockSumList[index].isShowSubCon) {
        this.currentOpen = -1
        this.StockSumList[index].isShowSubCon = false
      } else {
        if (this.currentOpen !== -1) {
          this.StockSumList[this.currentOpen].isShowSubCon = false
        }
        this.currentOpen = index
        this.StockSumList[index].isShowSubCon = true
      }
    },

    // 全部展开、收起
    shrinkAllSubContent () {
      if (this.shrinkAll.mark) {
        this.StockSumList.map(item => {
          item.isShowSubCon = true
          item.height = `${item.stocks.length * 41}px`
        })
        this.shrinkAll.text = '全部收起'
      } else {
        this.StockSumList.map(item => {
          item.isShowSubCon = false
          item.height = '0px'
        })
        this.shrinkAll.text = '全部展开'
      }
      this.shrinkAll.mark = !this.shrinkAll.mark
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
// 当前内容页面总高度
$height: $content-height - 18;
.wrap{
  height: percent($height, $content-height);
}
.box{
  height: percent(145, $height);
}

// 搜索组
.search-wrap{
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: percent(80, 145);
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
// 内容区
.content-wrap{
  height: percent($height - 145, $height);
  overflow: hidden;
  text-align: center;

  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: percent(50, $height - 145);
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);

    .gallery{
      p{
        margin-left: 10px;
      }
    }

    .gallery-list{
      height: 20px;
      font-size: $font-size-small;
    }
  }

  .scroll{
    height: percent($height - 50, $height);
  }

  // 所属管廊
  .gallery-list-wrap{
    display: flex;
    justify-content: space-between;
    margin-left: 10px;

    .gallery-list{
      width: 50px;
      .con{
        width: 100%;
        @extend %ellipsis;
      }
    }
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
      width: 50px;
      margin-left: 2%;
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
      }
    }

    .sub-con-list{
      display: flex;
      padding: 0 PXtoEm(24);
      border-top: 1px solid $color-main-background;
      background: rgba(0,0,0,.2);
      line-height: 40px;

      .right-wrap{
        display: -webkit-box;
        flex-wrap: wrap;
        // width: 80%;
      }

      .list{
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: PXtoEm(15) PXtoEm(24);
        margin-left: 10%;
      }
    }
  }

  .btn-wrap{
    width: 40px;
    font-size: 18px;
    text-align: left;

    .hide{
      visibility: hidden;
    }

    .icon-arrow-top-f{
      display: inline-block;
      transition: .5s;
      cursor: pointer;

      &.active{
        transform: rotateZ(180deg);
      }
    }
  }

  .number{
    width: 80px;
    word-break: break-word;
  }

  .name,
  .last-maintainer{
    width: 150px;
    word-break: break-word;
  }

  .name{
    a{
      color: #42abfd;
    }
  }
  .last-update-time{
    width: 200px;
    color: $color-content-text;
  }
}
</style>
