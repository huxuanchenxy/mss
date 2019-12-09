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
            <label for="name">检修单名称</label>
            <div class="inp">
              <el-input v-model.trim="pmName" placeholder="请输入检修单别名称"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="">检修单状态</label>
            <div class="inp">
              <el-select v-model="pmStatus" clearable filterable placeholder="请选择">
                <el-option
                  v-for="item in pmStatusList"
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
        <li class="list number c-pointer" @click="changeOrder('id')">
          检修单编号
          <i :class="[{ 'el-icon-d-caret': headOrder.id === 0 }, { 'el-icon-caret-top': headOrder.id === 1 }, { 'el-icon-caret-bottom': headOrder.id === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('title')">
          检修单抬头
          <i :class="[{ 'el-icon-d-caret': headOrder.title === 0 }, { 'el-icon-caret-top': headOrder.title === 1 }, { 'el-icon-caret-bottom': headOrder.title === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('team')">
          检修班组
          <i :class="[{ 'el-icon-d-caret': headOrder.team === 0 }, { 'el-icon-caret-top': headOrder.team === 1 }, { 'el-icon-caret-bottom': headOrder.team === 2 }]"></i>
        </li>
        <li class="list number">检修地点</li>
        <li class="list number c-pointer" @click="changeOrder('status')">
          检修单状态
          <i :class="[{ 'el-icon-d-caret': headOrder.status === 0 }, { 'el-icon-caret-top': headOrder.status === 1 }, { 'el-icon-caret-bottom': headOrder.status === 2 }]"></i>
        </li>
        <li class="list last-update-time c-pointer" @click="changeOrder('updated_time')">
          最后更新时间
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_time === 0 }, { 'el-icon-caret-top': headOrder.updated_time === 1 }, { 'el-icon-caret-bottom': headOrder.updated_time === 2 }]"></i>
        </li>
        <li class="list last-maintainer c-pointer" @click="changeOrder('updated_by')">
          最后更新人
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_by === 0 }, { 'el-icon-caret-top': headOrder.updated_by === 1 }, { 'el-icon-caret-bottom': headOrder.updated_by === 2 }]"></i>
        </li>
        <li class="list last-maintainer">操作</li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in mList" :key="item.key">
              <div class="list-content">
                <div class="number">{{ item.id }}</div>
                <div class="number">{{ item.title }}</div>
                <div class="number">{{ item.teamName }}</div>
                <div class="number">{{ item.locationName }}</div>
                <div class="number">{{ item.statusName }}</div>
                <div class="last-update-time color-white">{{ item.updatedTime }}</div>
                <div class="last-maintainer">{{ item.updatedByName }}</div>
                <div class="last-maintainer">
                  <li class="inline" @click="edit(item,false)" ><x-button :disabled="item.status===179">填写</x-button></li>
                  <li class="inline" @click="edit(item,true)" ><x-button >查看</x-button></li>
                </div>
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
import { dictionary, pmStatus } from '@/common/js/dictionary.js'
// import { btn } from '@/element/btn.js'
import XButton from '@/components/button'
import apiAuth from '@/api/authApi'
import api from '@/api/workflowApi'
export default {
  name: 'MaintenanceList',
  components: {
    XButton
  },
  data () {
    return {
      btn: {
        save: false,
        update: false
      },
      title: ' | 检修单',
      pmName: '',
      pmStatus: '',
      pmStatusList: [],
      mList: [],
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'id',
        order: 'asc'
      },
      headOrder: {
        id: 1,
        title: 0,
        status: 0,
        team: 0,
        updated_time: 0,
        updated_by: 0
      }
    }
  },
  created () {
    // let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    // if (!user.is_super) {
    //   let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
    //   this.btn.save = !actions.some((item, index) => {
    //     return item.actionID === btn.actionGroup.save
    //   })
    //   this.btn.delete = !actions.some((item, index) => {
    //     return item.actionID === btn.actionGroup.delete
    //   })
    //   this.btn.update = !actions.some((item, index) => {
    //     return item.actionID === btn.actionGroup.update
    //   })
    // }
    this.init()

    // 检修单状态列表
    apiAuth.getSubCode(dictionary.pmStatus).then(res => {
      this.pmStatusList = res.data
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
        this.headOrder.id = 0
        this.headOrder.title = 0
        this.headOrder.status = 0
        this.headOrder.team = 0
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
        name: this.pmName,
        status: this.pmStatus
      }
      api.getMList(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.updatedTime = transformDate(item.updatedTime)
        })
        this.mList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },

    // 修改检修单
    edit (item, isShow) {
      this.$router.push({
        name: 'UpdateMaintenanceList',
        params: {
          id: item.id,
          name: item.title,
          isInit: item.status === pmStatus.init,
          isShow: isShow
        }
      })
    },
    // 搜索功能
    searchRes () {
      this.$emit('title', '| 检修单')
      this.loading = true
      this.init()
      this.searchResult(1)
    },

    // 获取修改检修单id
    emitEditID () {
      this.$emit('editActionGroupID', this.editActionGroupID)
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
/deep/ .box1{
    // height: percent(145, $content-height);
    height: 60px;
    // 搜索组
    .search-wrap{
      display: flex;
      justify-content: space-between;
      align-items: center;
      height: 100%;
      // height: percent(80, 145);
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
  .inline{
    display: inline;
  }
</style>
