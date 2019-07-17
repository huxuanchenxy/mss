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
            <label for="name">用户名称</label>
            <div class="inp">
              <el-input v-model.trim="userName" placeholder="请输入用户名称"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="">角色</label>
            <div class="inp">
              <el-select v-model="role" clearable filterable placeholder="请选择">
                <el-option
                  v-for="item in roleList"
                  :key="item.key"
                  :label="item.role_name"
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
      <ul class="con-padding-horizontal btn-group">
        <li class="list">
          <x-button>
            <router-link :to="{ name: 'AddUser', params: { mark: 'add' } }">添加</router-link>
          </x-button>
        </li>
        <li class="list" @click="remove"><x-button>删除</x-button></li>
        <li class="list" @click="edit"><x-button>修改</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox" v-model="bCheckAll" @change="checkAll"></li>
        <li class="list number c-pointer" @click="changeOrder('id')">
          用户编号
          <i :class="[{ 'el-icon-d-caret': headOrder.id === 0 }, { 'el-icon-caret-top': headOrder.id === 1 }, { 'el-icon-caret-bottom': headOrder.id === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('acc_name')">
          登录账号
          <i :class="[{ 'el-icon-d-caret': headOrder.acc_name === 0 }, { 'el-icon-caret-top': headOrder.acc_name === 1 }, { 'el-icon-caret-bottom': headOrder.acc_name === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('user_name')">
          姓名
          <i :class="[{ 'el-icon-d-caret': headOrder.user_name === 0 }, { 'el-icon-caret-top': headOrder.user_name === 1 }, { 'el-icon-caret-bottom': headOrder.user_name === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('role_id')">
          角色
          <i :class="[{ 'el-icon-d-caret': headOrder.role_id === 0 }, { 'el-icon-caret-top': headOrder.role_id === 1 }, { 'el-icon-caret-bottom': headOrder.role_id === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('job_number')">
          工号
          <i :class="[{ 'el-icon-d-caret': headOrder.job_number === 0 }, { 'el-icon-caret-top': headOrder.job_number === 1 }, { 'el-icon-caret-bottom': headOrder.job_number === 2 }]"></i>
        </li>
        <li class="list number">职位</li>
        <li class="list number">手机</li>
        <li class="list number">邮件</li>
        <!--<li class="list number">身份证</li>
        <li class="list number">生日</li>
        <li class="list number">性别</li>-->
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
            <li class="list" v-for="(item) in UserList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="editUserID" :value="item.id" @change="emitEditID">
                </div>
                <div class="number">{{ item.id }}</div>
                <div class="number">{{ item.acc_name }}</div>
                <!--<div class="name">
                  <router-link :to="{ name: 'SeeActionList', params: { id: item.id } }">{{ item.user_name }}</router-link>
                </div>-->
                <div class="number">{{ item.user_name }}</div>
                <div class="number">{{ item.role_name }}</div>
                <div class="number">{{ item.job_number }}</div>
                <div class="number">{{ item.position }}</div>
                <div class="number">{{ item.mobile }}</div>
                <div class="number">{{ item.email }}</div>
                <!--<div class="number">{{ item.id_card }}</div>
                <div class="number">{{ item.birth }}</div>
                <div class="number">{{ item.sex }}</div>-->
                <div class="last-update-time color-white">{{ item.updated_time }}</div>
                <div class="last-maintainer">{{ item.updated_name }}</div>
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
    <el-dialog
      :visible.sync="dialogVisible.isShow"
      :modal-append-to-body="false"
      :show-close="false">
      {{ dialogVisible.text }}
      <template slot="footer" class="dialog-footer">
        <template v-if="dialogVisible.btn">
          <el-button @click="dialogVisible.isShow = false">否</el-button>
          <el-button @click="dialogEnter">是</el-button>
        </template>
        <el-button v-else @click="dialogVisible.isShow = false" :class="{ on: !dialogVisible.btn }">知道了</el-button>
      </template>
    </el-dialog>
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/authApi'
export default {
  name: 'SeeUserList',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 用户管理',
      userName: '',
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
        acc_name: 0,
        user_name: 0,
        role_id: 0,
        job_number: 0,
        updated_time: 0,
        updated_by: 0
      }
    }
  },
  created () {
    api.getActionByUser().then(res => {
      console.log(res)
    })
    this.$emit('title', '| 用户')
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
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = 1
      // this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.acc_name = 0
        this.headOrder.user_name = 0
        this.headOrder.role_id = 0
        this.headOrder.job_number = 0
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
      this.bCheckAll = false
      this.checkAll()
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
        searchName: this.userName,
        searchRole: this.role
      }
      api.getUser(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.updated_time = transformDate(item.updated_time)
        })
        this.UserList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },

    // 修改用户
    edit () {
      if (!this.editUserID.length) {
        this.$message({
          message: '请选择需要修改的用户',
          type: 'warning'
        })
      } else if (this.editUserID.length > 1) {
        this.$message({
          message: '修改的用户不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddUser',
          params: {
            id: this.editUserID[0],
            mark: 'edit'
          }
        })
      }
    },

    // 删除用户
    remove () {
      if (!this.editUserID.length) {
        this.$message({
          message: '请选择需要删除的用户',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条用户信息?'
      }
    },
    // 弹框确认是否删除
    dialogEnter () {
      api.delUser(this.editUserID.join(',')).then(res => {
        if (res.code === 0) {
          this.editUserID = []
          this.$message({
            message: '删除成功',
            type: 'success'
          })
          this.currentPage = 1
          this.searchResult(1)
        } else {
          this.$message({
            message: res.msg,
            type: 'error'
          })
        }
        // 隐藏dialog
        this.dialogVisible.isShow = false
      }).catch(err => console.log(err))
    },
    // 搜索功能
    searchRes () {
      this.$emit('title', '| 用户管理')
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
