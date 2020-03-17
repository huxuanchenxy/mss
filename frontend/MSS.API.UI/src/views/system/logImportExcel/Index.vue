<template>
  <div class="wrap"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
    </div>
    <!-- 搜索框 -->
    <div class="con-padding-horizontal search-wrap">
      <div class="wrap">
        <div class="input-group">
          <label for="name">导入文件名</label>
          <div class="inp">
            <el-input placeholder="请输入文件名" v-model="fileName"></el-input>
          </div>
        </div>
      </div>
      <div class="search-btn" @click="search(1)">
        <x-button><i class="iconfont icon-search"></i>查询</x-button>
      </div>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list number c-pointer" @click="changeOrder('ID')">
          日志ID
          <i :class="[{ 'el-icon-d-caret': headOrder.ID === 0 }, { 'el-icon-caret-top': headOrder.ID === 1 }, { 'el-icon-caret-bottom': headOrder.ID === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('file_name')">
          导入文件名
          <i :class="[{ 'el-icon-d-caret': headOrder.file_name === 0 }, { 'el-icon-caret-top': headOrder.file_name === 1 }, { 'el-icon-caret-bottom': headOrder.file_name === 2 }]"></i>
        </li>
        <li class="list module-name">导入字段</li>
        <li class="list name">记录数</li>
        <li class="list last-update-time color-white c-pointer" @click="changeOrder('Created_Time')">
          操作时间
          <i :class="[{ 'el-icon-d-caret': headOrder.Created_Time === 0 }, { 'el-icon-caret-top': headOrder.Created_Time === 1 }, { 'el-icon-caret-bottom': headOrder.Created_Time === 2 }]"></i>
        </li>
        <li class="list last-maintainer c-pointer" @click="changeOrder('Created_Name')">
          操作人员
          <i :class="[{ 'el-icon-d-caret': headOrder.Created_Name === 0 }, { 'el-icon-caret-top': headOrder.Created_Name === 1 }, { 'el-icon-caret-bottom': headOrder.Created_Name === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="item in logList" :key="item.key">
              <div class="list-content">
                <div class="number">{{ item.id }}</div>
                <div class="name">{{ item.fileName }}</div>
                <el-tooltip :content="item.field" :disabled="item.field.length < 12" placement="top">
                  <div class="module-name">{{ item.field }}</div>
                </el-tooltip>
                <div class="name">{{ item.recordNum }}</div>
                <div class="last-update-time color-white">{{ item.createdTime }}</div>
                <div class="last-maintainer">{{ item.createdName }}</div>
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
import XButton from '@/components/button'
import { transformDate } from '@/common/js/utils.js'
import api from '@/api/eqpApi'
export default {
  name: 'LogImportExcel',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 导入Excel日志管理',
      logList: [],
      fileName: '',
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'id',
        order: 'asc'
      },
      headOrder: {
        id: 1,
        file_name: 0,
        Created_Time: 0,
        Created_Name: 0
      } // 默认LogID升序asc，箭头朝上，同时只可能按照一个字段排序，不排序的字段不出现箭头,0不排序、1升序、2降序，切换时默认升序
    }
  },
  created () {
    this.search(1)
  },
  methods: {
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.file_name = 0
        this.headOrder.Created_Time = 0
        this.headOrder.Created_Name = 0
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
      this.search(this.currentPage)
    },
    search (page) {
      this.loading = true
      this.currentPage = page
      api.getImportExcelLog({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        FileName: this.fileName
      }).then(res => {
        this.loading = false
        res.data.rows.map(val => {
          val.createdTime = transformDate(val.createdTime)
        })
        this.logList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.currentPage = val
      this.search(val)
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
.wrap{
  height: 100%;

  // 搜索组
  .search-wrap{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: percent(80, $content-height);
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

  // 内容区
  .content-wrap{
    $con-height: $content-height - 56 - 80;
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

      .list{
        &:first-of-type{
          width: 5%;
        }
      }
    }

    .scroll{
      height: percent($con-height - 50, $con-height);
    }

    .list-wrap{
      padding-bottom: 10px;
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
    }

    .number{
      width: 5%;
    }

    .name,
    .last-maintainer{
      width: 20%;
    }

    .module-name{
      width: 20%;
      @extend %ellipsis;
    }

    .last-update-time{
      width: 35%;
      color: $color-content-text;
    }
  }
}
</style>
