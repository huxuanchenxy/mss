<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeSparePartsList' }">返回</router-link></x-button>
    </div>
        <!-- 列表 -->
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
            </div>
            <div class="search-btn" @click="searchRes">
              <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
            </div>
          </div>
        </div>
        <!-- 内容 -->
        <div class="content-wrap">
          <ul class="content-header">
            <li class="list url c-pointer" @click="changeOrder('warehouse')">
              仓库
              <i :class="[{ 'el-icon-d-caret': headOrder.warehouse === 0 }, { 'el-icon-caret-top': headOrder.warehouse === 1 }, { 'el-icon-caret-bottom': headOrder.warehouse === 2 }]"></i>
            </li>
            <li class="list name c-pointer" @click="changeOrder('safe_storage')">
              安全库存
              <i :class="[{ 'el-icon-d-caret': headOrder.safe_storage === 0 }, { 'el-icon-caret-top': headOrder.safe_storage === 1 }, { 'el-icon-caret-bottom': headOrder.safe_storage === 2 }]"></i>
            </li>
            <li class="list last-update-time c-pointer" @click="changeOrder('updated_time')">
              最后更新时间
              <i :class="[{ 'el-icon-d-caret': headOrder.updated_time === 0 }, { 'el-icon-caret-top': headOrder.updated_time === 1 }, { 'el-icon-caret-bottom': headOrder.updated_time === 2 }]"></i>
            </li>
            <li class="list name c-pointer" @click="changeOrder('updated_by')">
              最后更新人
              <i :class="[{ 'el-icon-d-caret': headOrder.updated_by === 0 }, { 'el-icon-caret-top': headOrder.updated_by === 1 }, { 'el-icon-caret-bottom': headOrder.updated_by === 2 }]"></i>
            </li>
          </ul>
          <div class="scroll">
            <el-scrollbar>
              <ul class="list-wrap">
                <li class="list" v-for="item in warehouseAlarmList" :key="item.key">
                  <div class="list-content">
                    <div class="url">{{ item.warehouseName }}</div>
                    <div class="name word-break">{{ item.safeStorage }}</div>
                    <div class="last-update-time color-white word-break">{{ item.updatedTime }}</div>
                    <div class="name word-break">{{ item.updatedName }}</div>
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
import api from '@/api/wmsApi'
import XButton from '@/components/button'
export default {
  name: 'DetailSpareParts',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      title: '| 物资详情',
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
      warehouseList: [],
      warehouseAlarmList: [],
      total: 0,
      currentPage: 1,
      currentSort: {
        sort: 'ID',
        order: 'asc'
      },
      headOrder: {
        warehouse: 1,
        safe_storage: 0,
        updated_time: 0,
        updated_by: 0
      }
    }
  },
  created () {
    // 仓库加载
    api.getWarehouseAll().then(res => {
      this.warehouseList = res.data
    }).catch(err => console.log(err))
    this.getSpareParts()
  },
  methods: {
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
          this.searchResult(1)
        } else this.loading = false
      }).catch(err => console.log(err))
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.warehouse = 0
        this.headOrder.safe_storage = 0
        this.headOrder.updated_by = 0
        this.headOrder.updated_time = 0
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
      api.getWarehouseAlarm({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        SearchSpareParts: this.spareParts.id,
        SearchType: this.warehouse
      }).then(res => {
        this.loading = false
        if (res.data.total === 0) {
          this.warehouseAlarmList = []
        } else {
          res.data.rows.map(item => {
            item.updatedTime = transformDate(item.updatedTime)
          })
          this.warehouseAlarmList = res.data.rows
        }
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    // 搜索功能
    searchRes () {
      this.loading = true
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
    height: percent($con-height - 50 - 70, $con-height);
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

.box{
  margin-top: PXtoEm(20);
  margin-bottom: PXtoEm(20);
  height: percent(56, $content-height)!important;
  .search-wrap{
    height: 100%!important;
  }
}
.header{
  display: flex;
  justify-content: space-between;
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
.btn-group{
  padding: 20px 0;
  text-align: center;
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
