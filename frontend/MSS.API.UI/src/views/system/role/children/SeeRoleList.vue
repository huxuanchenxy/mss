<template>
  <div class="wrap"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="box">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
          <div class="input-group">
            <label for="name">角色名称</label>
            <div class="inp">
              <el-input placeholder="请输入角色名称" v-model.trim="roleName"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="">使用权限</label>
            <div class="inp">
              <el-cascader clearable
                change-on-select
                :props="defaultParams"
                :show-all-levels="true"
                :options="actionInfo"
                v-model="authority">
              </el-cascader>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="search">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list">
          <x-button :disabled="btn.save">
            <router-link :to="{ name: 'AddRole', query: { t: 'add' } }">添加</router-link>
          </x-button>
        </li>
        <li class="list" @click="remove"><x-button :disabled="btn.delete">删除</x-button></li>
        <li class="list" @click="edit"><x-button :disabled="btn.update">修改</x-button></li>
        <li class="list" @click="shrinkAllSubContent" ><x-button>{{ shrinkAll.text }}</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="height-full content-wrap">
      <ul class="content-header">
        <li class="btn-wrap">
          <i class="hide iconfont icon-arrow-top-f"></i>
          <input type="checkbox" v-model="bCheckAll" @change="checkAll">
        </li>
        <li class="list number c-pointer" @click="changeOrder('id')">
          角色编号
          <i :class="[{ 'el-icon-d-caret': headOrder.id === 0 }, { 'el-icon-caret-top': headOrder.id === 1 }, { 'el-icon-caret-bottom': headOrder.id === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('role_name')">
          角色名称
          <i :class="[{ 'el-icon-d-caret': headOrder.role_name === 0 }, { 'el-icon-caret-top': headOrder.role_name === 1 }, { 'el-icon-caret-bottom': headOrder.role_name === 2 }]"></i>
        </li>
        <li class="list last-update-time color-white c-pointer" @click="changeOrder('updated_time')">
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
            <li class="list" v-for="(item, index) in RoleList" :key="item.key">
              <div class="list-content">
                <div class="btn-wrap">
                  <i title="展开" @click="shrinkSubContent(index, item.id, item.action_trees.length)" class="iconfont icon-arrow-top-f" :class="{ active: item.isShowSubCon }"></i>
                  <input type="checkbox" v-model="editRoleID" :value="item.id" @change="emitEditID">
                </div>
                <div class="number">{{ item.id }}</div>
                <div class="name"><router-link :to="{ name: 'SeeUserList', params: { roleID: item.id } }">{{ item.role_name }}</router-link></div>
                <div class="last-update-time color-white">{{ item.updated_time }}</div>
                <div class="last-maintainer">{{ item.updated_name }}</div>
              </div>
              <ul class="sub-content" :class="{ active: item.isShowSubCon }">
                <!--<li class="sub-con-list" v-for="sub in item.action_trees" :key="sub.key">
                  <div class="left-title">{{ sub.text }}</div>
                  <ul class="right-wrap">
                    <li class="list" v-for="three in sub.children" :key="three.key">{{ three.text }}</li>
                  </ul>
                </li>-->
                <li class="sub-con-list" v-for="sub in item.action_trees" :key="sub.key">
                  <div class="left-title">{{ sub.text }}</div>
                  <ul>
                    <li v-for="three in sub.children" :key="three.key">
                      <ul class="right-wrap">
                        <li class="list">{{ three.text }}</li>
                        <li class="list" v-for="four in three.children" :key="four.key">{{ four.text }}</li>
                      </ul>
                    </li>
                  </ul>
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
import XButton from '@/components/button'
import { btn } from '@/element/btn.js'
import { transformDate, keyRole } from '@/common/js/utils.js'
import api from '@/api/authApi'
export default {
  name: 'Role',
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
      defaultParams: {
        label: 'text',
        value: 'id',
        children: 'children'
      },
      roleName: '',
      actionInfo: [],
      authority: [],
      RoleList: [],
      getEditRoleID: '',
      editRoleID: [],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      currentSort: {
        sort: 'id',
        order: 'asc'
      },
      headOrder: {
        id: 1,
        role_name: 0,
        updated_time: 0,
        updated_by: 0
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
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    if (!user.is_super) {
      let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
      this.btn.save = !actions.some((item, index) => {
        return item.actionID === btn.role.save
      })
      this.btn.delete = !actions.some((item, index) => {
        return item.actionID === btn.role.delete
      })
      this.btn.update = !actions.some((item, index) => {
        return item.actionID === btn.role.update
      })
    }
    this.init()
    // this.$emit('title', '| 角色定义')
    // 权限列表
    api.getActionTree().then(res => {
      this.actionInfo = res.data
    }).catch(err => console.log(err))
  },
  watch: {
    random () {
      this.shrinkAllSubContent()
    }
  },
  methods: {
    init () {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = 1
      this.searchResult(1)
    },
    edit () {
      if (!this.editRoleID.length) {
        this.$message({
          message: '请选择需要修改的角色',
          type: 'warning'
        })
      } else if (this.editRoleID.length > 1) {
        this.$message({
          message: '修改的角色不能超过1个',
          type: 'warning'
        })
      } else {
        // this.title = '修改角色'
        // this.showChildren = 'edit'
        this.$router.push({
          name: 'AddRole',
          query: { t: 'edit', id: this.editRoleID }
        })
      }
    },
    // 删除角色
    remove () {
      for (let i = 0; i < this.editRoleID.length; i++) {
        if (keyRole.dispatcher === this.editRoleID[i]) {
          this.$message({
            message: '编号' + keyRole.dispatcher + '是关键角色，不可删除',
            type: 'suwarningccess'
          })
          return
        }
      }
      if (!this.editRoleID.length) {
        this.$message({
          message: '请选择需要删除的角色',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条角色信息?'
      }
    },

    // 弹框确认是否删除
    dialogEnter () {
      api.delRole(this.editRoleID.join(',')).then(res => {
        if (res.code === 0) {
          this.editRoleID = []
          this.$message({
            message: '删除成功',
            type: 'success'
          })
          this.init()
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
    // 改变排序
    changeOrder (sort) {
      // console.log(this.headOrder[sort])
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.role_name = 0
        this.headOrder.updated_time = 0
        this.headOrder.updated_by = 0
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
    search () {
      this.searchResult(1)
    },
    // 搜索
    searchResult (page) {
      this.$emit('title', '| 角色管理')
      this.currentPage = page
      this.loading = true
      let act = null
      let actionGroup = null
      let l = this.authority.length
      if (l === 1) {
        actionGroup = this.authority[0]
      } else if (l > 1) {
        act = this.authority[l - 1]
      }
      api.getRole({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        searchName: this.roleName,
        searchAction: act,
        searchActionGroup: actionGroup
      }).then(res => {
        this.loading = false
        res.data.rows.map(val => {
          val.isShowSubCon = false
          val.updated_time = transformDate(val.updated_time)
        })
        this.total = res.data.total
        this.RoleList = res.data.rows
      }).catch(err => console.log(err))
    },
    // 收起展开权限列表
    shrinkSubContent (index, id, length) {
      // this.RoleList[index].isShowSubCon ? this.RoleList[index].isShowSubCon = true : this.RoleList[index].isShowSubCon = false
      // this.RoleList[index].isShowSubCon = !this.RoleList[index].isShowSubCon
      if (this.RoleList[index].isShowSubCon) {
        this.currentOpen = -1
        this.RoleList[index].isShowSubCon = false
      } else {
        if (this.currentOpen !== -1) {
          this.RoleList[this.currentOpen].isShowSubCon = false
        }
        this.currentOpen = index
        this.RoleList[index].isShowSubCon = true
      }
    },

    // 全部展开、收起
    shrinkAllSubContent () {
      if (this.shrinkAll.mark) {
        this.RoleList.map(item => {
          item.isShowSubCon = true
          item.height = `${item.action_trees.length * 41}px`
        })
        this.shrinkAll.text = '全部收起'
      } else {
        this.RoleList.map(item => {
          item.isShowSubCon = false
          item.height = '0px'
        })
        this.shrinkAll.text = '全部展开'
      }
      this.shrinkAll.mark = !this.shrinkAll.mark
    },
    // 获取修改角色id
    emitEditID () {
      this.$emit('editRoleID', this.editRoleID)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.RoleList.map(val => this.editRoleID.push(val.id)) : this.editRoleID = []
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
    },

    // 下一页
    nextPage (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
    }
  }
}
</script>
<style lang="scss" scoped>
// 当前内容页面总高度
$height: $content-height - 56;
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
        width: 100px;
        margin-left: 50px;
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
