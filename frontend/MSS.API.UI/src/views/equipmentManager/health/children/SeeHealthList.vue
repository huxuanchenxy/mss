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
            <label for="">健康度配置类型</label>
            <div class="inp">
              <el-select v-model="healthType" clearable filterable placeholder="请选择">
                <el-option
                  v-for="item in healthTypeList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">设备</label>
            <div class="inp">
              <el-cascader
                filterable
                clearable
                :props="defaultParams"
                :show-all-levels="false"
                :options="eqpList"
                v-model="eqp">
              </el-cascader>
            </div>
          </div>
        </div>
        <div class="search-btn">
        <div @click="searchRes" style="display:inline-block;">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
        </div>
      </div>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list number c-pointer" @click="changeOrder('eqp')">
          设备
          <i :class="[{ 'el-icon-d-caret': headOrder.eqp === 0 }, { 'el-icon-caret-top': headOrder.eqp === 1 }, { 'el-icon-caret-bottom': headOrder.eqp === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('type')">
          配置类型
          <i :class="[{ 'el-icon-d-caret': headOrder.type === 0 }, { 'el-icon-caret-top': headOrder.type === 1 }, { 'el-icon-caret-bottom': headOrder.type === 2 }]"></i>
        </li>
        <li class="list number">健康度</li>
        <li class="list last-update-time c-pointer" @click="changeOrder('updated_time')">
          最后更新时间
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_time === 0 }, { 'el-icon-caret-top': headOrder.updated_time === 1 }, { 'el-icon-caret-bottom': headOrder.updated_time === 2 }]"></i>
        </li>
        <li class="list last-maintainer c-pointer" @click="changeOrder('updated_by')">
          最后更新人
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_by === 0 }, { 'el-icon-caret-top': headOrder.updated_by === 1 }, { 'el-icon-caret-bottom': headOrder.updated_by === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in healthConfigList" :key="item.key">
              <div class="list-content">
                <div class="number">{{ item.eqpName }}</div>
                <div class="number">{{ item.typeName }}</div>
                <div class="number">{{ item.val }}</div>
                <div class="last-update-time color-white">{{ item.updatedTime }}</div>
                <div class="last-maintainer">{{ item.updatedByName }}</div>
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
    <!-- dialog对话框 -->
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import { dictionary } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import apiAuth from '@/api/authApi'
import api from '@/api/DeviceMaintainRegApi.js'
export default {
  name: 'SeeHealth',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 健康度',
      defaultParams: {
        label: 'name',
        value: 'id',
        children: 'children'
      },
      eqp: [],
      eqpList: [],
      healthType: '',
      healthTypeList: [],
      healthConfigList: [],
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'id',
        order: 'asc'
      },
      headOrder: {
        eqp: 0,
        type: 1,
        updated_time: 0,
        updated_by: 0
      }
    }
  },
  created () {
    // let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    // if (!user.is_super) {
    //   let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
    //   this.btn.update = !actions.some((item, index) => {
    //     return item.actionID === btn.actionGroup.update
    //   })
    // }
    // 设备加载
    api.GetEqpByTypeAndLine(0).then(res => {
      this.eqpList = res.data
    }).catch(err => console.log(err))
    this.init()

    // 健康度配置类型列表
    apiAuth.getSubCode(dictionary.healthType).then(res => {
      this.healthTypeList = res.data
    }).catch(err => console.log(err))
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
        this.headOrder.type = 0
        this.headOrder.eqp = 0
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
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        Eqp: this.eqp[this.eqp.length - 1],
        Type: this.healthType
      }
      api.getHealth(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.updatedTime = transformDate(item.updatedTime)
        })
        this.healthConfigList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },

    // 搜索功能
    searchRes () {
      // this.$emit('title', '| 权限组别')
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

      div{
        word-break: break-all;
      }
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

  .number,
  .name,
  .btn-wrap{
    width: 10%;
  }

  .name{
    a{
      color: #42abfd;
    }
  }

  .last-update-time{
    width: 18%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 10%;
  }

  .state{
    width: 5%;
  }
}
</style>
