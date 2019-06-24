<template>
  <div class="wrap"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].TitleIcon" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].GroupName }} {{ title ? `| ${title}` : '' }}
      </h2>
    </div>
    <!-- 搜索框 -->
    <div class="con-padding-horizontal search-wrap">
      <div class="wrap">
        <div class="input-group">
          <label for="name">操作描述</label>
          <div class="inp">
            <el-input placeholder="请输入操作描述" v-model="logDes"></el-input>
          </div>
        </div>
        <div class="input-group">
          <label for="">菜单模块</label>
          <div class="inp">
            <el-cascader
              :options="logArr"
              v-model="logRes">
            </el-cascader>
          </div>
        </div>
        <div class="input-group">
          <label for="">所属管廊</label>
          <div class="inp">
            <el-select v-model="tunnel" filterable placeholder="请选择">
              <el-option
                v-for="item in tunnelList"
                :key="item.key"
                :label="item.TunnelName"
                :value="item.TunnelID">
              </el-option>
            </el-select>
          </div>
        </div>
      </div>
      <div class="search-btn" @click="search(1)">
        <x-button><i class="iconfont icon-search"></i> 查询</x-button>
      </div>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list number c-pointer" @click="changeOrder('LogID')">
          日志ID
          <i :class="[{ 'el-icon-d-caret': headOrder.LogID === 0 }, { 'el-icon-caret-top': headOrder.LogID === 1 }, { 'el-icon-caret-bottom': headOrder.LogID === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('ModuleName')">
          菜单模块
          <i :class="[{ 'el-icon-d-caret': headOrder.ModuleName === 0 }, { 'el-icon-caret-top': headOrder.ModuleName === 1 }, { 'el-icon-caret-bottom': headOrder.ModuleName === 2 }]"></i>
        </li>
        <li class="list module-name">操作描述</li>
        <li class="list last-update-time color-white c-pointer" @click="changeOrder('CreateTime')">
          操作时间
          <i :class="[{ 'el-icon-d-caret': headOrder.CreateTime === 0 }, { 'el-icon-caret-top': headOrder.CreateTime === 1 }, { 'el-icon-caret-bottom': headOrder.CreateTime === 2 }]"></i>
        </li>
        <li class="list last-maintainer c-pointer" @click="changeOrder('CreateUserName')">
          操作人员
          <i :class="[{ 'el-icon-d-caret': headOrder.CreateUserName === 0 }, { 'el-icon-caret-top': headOrder.CreateUserName === 1 }, { 'el-icon-caret-bottom': headOrder.CreateUserName === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="item in logList" :key="item.key">
              <div class="list-content">
                <div class="number">{{ item.LogID }}</div>
                <div class="name">{{ item.ModuleName }}</div>
                <el-tooltip :content="item.Description" :disabled="item.Description.length < 12" placement="top">
                  <div class="module-name">{{ item.Description }}</div>
                </el-tooltip>
                <div class="last-update-time color-white">{{ item.CreateTime }}</div>
                <div class="last-maintainer">{{ item.CreateUserName }}</div>
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
export default {
  name: 'Log',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 日志管理',
      logList: [],
      logDes: '',
      logRes: [],
      logArr: [],
      tunnel: '',
      tunnelList: [],
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'LogID',
        order: 'asc'
      },
      headOrder: {
        LogID: 1,
        ModuleName: 0,
        CreateTime: 0,
        CreateUserName: 0
      } // 默认LogID升序asc，箭头朝上，同时只可能按照一个字段排序，不排序的字段不出现箭头,0不排序、1升序、2降序，切换时默认升序
    }
  },
  created () {
    // 权限列表
    window.axios.get('/ActionInfo/GetActionInfoForElCascader').then(res => {
      this.logArr = res.data
      this.logArr.unshift({value: '', label: '所有'})
    }).catch(err => console.log(err))
    // 管廊列表
    window.axios.get('/UtilityTunnel/GetTunnelByUserID').then(res => {
      this.tunnelList = res.data
      this.tunnel = res.data[0].TunnelID
      this.search(1)
    }).catch(err => console.log(err))
  },
  methods: {
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.LogID = 0
        this.headOrder.ModuleName = 0
        this.headOrder.CreateTime = 0
        this.headOrder.CreateUserName = 0
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
      this.search()
    },
    search (page) {
      this.loading = true
      this.currentPage = page
      this.title = '日志管理'
      window.axios.post('/Log/GetLog', {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        SearchDes: this.logDes,
        SearchModule: this.logRes[this.logRes.length - 1],
        SearchTunnel: this.tunnel
      }).then(res => {
        this.loading = false
        res.data.rows.map(val => {
          val.CreateTime = transformDate(val.CreateTime)
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
      this.search(val)
    },

    // 下一页
    nextPage (val) {
      console.log(this.currentSort)
      this.currentPage = val
      this.searchResult(val)
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
