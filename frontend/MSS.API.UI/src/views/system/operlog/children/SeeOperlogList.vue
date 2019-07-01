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
            <label for="name">用户姓名</label>
            <div class="inp">
              <el-input v-model.trim="userName" placeholder="请输入用户姓名"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">操作名称</label>
            <div class="inp">
              <el-input v-model.trim="actionName" placeholder="请输入操作名称"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">操作时间</label>
            <div class="inp" style="width:165px;">
              <el-input v-model.trim="startTime" placeholder="请输入开始时间"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">至</label>
            <div class="inp" style="width:165px;">
              <el-input v-model.trim="endTime" placeholder="请输入结束时间" ></el-input>
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
          序号
          <i :class="[{ 'el-icon-d-caret': headOrder.id === 0 }, { 'el-icon-caret-top': headOrder.id === 1 }, { 'el-icon-caret-bottom': headOrder.id === 2 }]"></i>
        </li>
        <li class="list name">
          模块名称
       </li>
        <li class="list name">
          操作名称
       </li>
       <li class="list name">
          用户姓名
       </li>
        <li class="list name" >
          登录账号
       </li>
        <li class="list name">ip地址</li>
        <li class="list name">mac地址</li>
        <li class="list name">操作时间</li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in UserList" :key="item.key">
              <div class="list-content">
                <div class="number">{{ item.id }}</div>
                <div class="name">{{ item.controller_name }}</div>
                <div class="name">{{ item.action_name }}</div>
                <!--<div class="name">
                  <router-link :to="{ name: 'SeeActionList', params: { id: item.id } }">{{ item.user_name }}</router-link>
                </div>-->
                <div class="name">{{ item.user_name }}</div>
                <div class="name">{{ item.acc_name }}</div>
                <div class="name">{{ item.ip }}</div>
                <div class="name">{{ item.mac_add }}</div>
                <div class="name">{{ item.created_time }}</div>
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
import api from '@/api/operlogApi'
export default {
  name: 'SeeOperlogList',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 日志查看',
      userName: '',
      actionName: '',
      startTime: '',
      endTime: '',
      role: '',
      roleList: [],
      UserList: [],
      editUserID: [],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'id',
        order: 'asc'
      },
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      headOrder: {
        id: 1,
        acc_name: 0
      }
    }
  },
  created () {
    this.$emit('title', '| 日志查看')
    if (this.$route.params.roleID !== '' && this.$route.params.roleID !== null) {
      this.role = this.$route.params.roleID
    }
    this.init()

    // 角色列表
    api.getRoleAll().then(res => {
      this.roleList = res.data
    }).catch(err => console.log(err))
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  methods: {
    init () {
      // this.bCheckAll = false
      // this.checkAll()
      this.currentPage = 1
      this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        // this.headOrder.acc_name = 0
        // this.headOrder.user_name = 0
        // this.headOrder.role_id = 0
        // this.headOrder.job_number = 0
        // this.headOrder.updated_by = 0
        // this.headOrder.updated_time = 0
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
      // this.bCheckAll = false
      // this.checkAll()
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
        actionName: this.actionName,
        userName: this.userName,
        startTime: this.startTime,
        endTime: this.endTime
      }
      api.getOperationLog(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.created_time = transformDate(item.created_time)
        })
        this.UserList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    // 搜索功能
    searchRes () {
      this.$emit('title', '| 操作日志')
      this.loading = true
      this.init()
      this.searchResult(1)
    },
    // 获取修改用户id
    emitEditID () {
      this.$emit('editUserID', this.editUserID)
    },
    // 全选
    checkAll () {
      this.bCheckAll ? this.UserList.map(val => this.editUserID.push(val.id)) : this.editUserID = []
      this.emitEditID()
    },
    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 上一页
    prevPage (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 下一页
    nextPage (val) {
      this.bCheckAll = false
      this.checkAll()
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
    width: 13%;
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
