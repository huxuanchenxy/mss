<template>
  <div
    class="wrap"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="box">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
          <div class="input-group">
            <label for="name">用户名称</label>
            <div class="inp">
              <el-input placeholder="请输入用户名称" v-model.trim="userName"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="">使用权限</label>
            <div class="inp">
              <el-cascader
                :options="actionInfo"
                v-model="authority">
              </el-cascader>
            </div>
          </div>
          <div class="input-group">
            <label for="">所属管廊</label>
            <div class="inp">
              <el-select v-model="tunnel" filterable placeholder="请选择">
                <el-option value="" label="所有"></el-option>
                <el-option
                  v-for="item in tunnelList"
                  :key="item.key"
                  :label="item.TunnelName"
                  :value="item.TunnelID">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">是否有效</label>
            <div class="inp">
              <el-select v-model="isDelete" placeholder="请选择">
                <el-option value="" label="所有"></el-option>
                <el-option value="0" label="有效"></el-option>
                <el-option value="1" label="无效"></el-option>
              </el-select>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="search">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="add"><x-button>添加</x-button></li>
        <li class="list" @click="remove"><x-button>删除</x-button></li>
        <li class="list" @click="edit"><x-button>修改</x-button></li>
        <li class="list" @click="deleteRecovery"><x-button>删除恢复</x-button></li>
        <li class="list" @click="setPassword"><x-button>密码重置</x-button></li>
        <li class="list" @click="shrinkAllSubContent"><x-button>{{ shrinkAll.text }}</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <div id="use-scroll" class="scroll">
        <el-scrollbar>
          <!--  :style="{ 'width': tableHeaderWidth }" -->
          <ul class="content-header">
            <li class="btn-wrap"></li>
            <li class="list">
              <input type="checkbox" v-model="bCheckAll" @change="checkAll">
            </li>
            <li class="list number c-pointer" @click="changeOrder('UserID')">
              用户编号
              <i :class="[{ 'el-icon-d-caret': headOrder.UserID === 0 }, { 'el-icon-caret-top': headOrder.UserID === 1 }, { 'el-icon-caret-bottom': headOrder.UserID === 2 }]"></i>
            </li>
            <li class="list name c-pointer" @click="changeOrder('UserName')">
              用户名称
              <i :class="[{ 'el-icon-d-caret': headOrder.UserName === 0 }, { 'el-icon-caret-top': headOrder.UserName === 1 }, { 'el-icon-caret-bottom': headOrder.UserName === 2 }]"></i>
            </li>
            <li class="list phone">手机号码</li>
            <li class="list email">电子邮箱</li>
            <li class="list last-update-time color-white c-pointer" @click="changeOrder('LmTime')">
              最后更新时间
              <i :class="[{ 'el-icon-d-caret': headOrder.LmTime === 0 }, { 'el-icon-caret-top': headOrder.LmTime === 1 }, { 'el-icon-caret-bottom': headOrder.LmTime === 2 }]"></i>
            </li>
            <li class="list last-maintainer c-pointer" @click="changeOrder('LmName')">
              最后更新人
              <i :class="[{ 'el-icon-d-caret': headOrder.LmName === 0 }, { 'el-icon-caret-top': headOrder.LmName === 1 }, { 'el-icon-caret-bottom': headOrder.LmName === 2 }]"></i>
            </li>
            <li class="list state c-pointer" @click="changeOrder('IsDelete')">
              状态
              <i :class="[{ 'el-icon-d-caret': headOrder.IsDelete === 0 }, { 'el-icon-caret-top': headOrder.IsDelete === 1 }, { 'el-icon-caret-bottom': headOrder.IsDelete === 2 }]"></i>
            </li>
            <li class="list branch c-pointer" @click="changeOrder('Department')">
              所属部门
              <i :class="[{ 'el-icon-d-caret': headOrder.Department === 0 }, { 'el-icon-caret-top': headOrder.Department === 1 }, { 'el-icon-caret-bottom': headOrder.Department === 2 }]"></i>
            </li>
            <li class="list gallery" :style="{ width: galleryWidth }">
              <p>所属管廊</p>
              <ul class="gallery-list-wrap">
                <li class="gallery-list" v-for="item in galleryList" :key="item.key">
                  <el-tooltip :content="item" placement="top"><div class="con">{{ item }}</div></el-tooltip>
                </li>
              </ul>
            </li>
          </ul>
          <div class="scroll-1">
            <el-scrollbar>
              <ul class="list-wrap">
                <li class="list" v-for="(item, index) in userList" :key="item.key">
                  <div class="list-content">
                    <div class="btn-wrap" title="展开" @click="shrinkSubContent(index, item.UserID, item.MyAction.length)">
                      <i class="iconfont icon-arrow-top-f" :class="{ active: item.isShowSubCon }"></i>
                    </div>
                    <div class="checkbox">
                      <input type="checkbox" v-model="editUserID" :value="item.UserID" @change="emitEditID">
                    </div>
                    <div class="number">{{ item.UserID }}</div>
                    <div class="name">{{ item.UserName }}</div>
                    <div class="phone">{{ item.Mobile === null ? '--' : item.Mobile }}</div>
                    <div class="email">{{ item.Email === null ? '--' : item.Email }}</div>
                    <div class="last-update-time color-white">{{ item.LmTime }}</div>
                    <div class="last-maintainer">{{ item.LmName }}</div>
                    <div class="state">{{ item.IsDelete === 0 ? '有效' : '无效' }}</div>
                    <div class="branch">{{ item.Department === null ? '--' : item.Department }}</div>
                    <div class="gallery" :style="{ width: galleryWidth }">
                      <ul class="gallery-list-wrap">
                        <li class="gallery-list" v-for="gallery in item.Tunnels" :key="gallery.key">
                          <i v-if="gallery" class="el-icon-star-on"></i>
                          <span v-else>--</span>
                        </li>
                      </ul>
                    </div>
                  </div>
                  <ul class="sub-content" :class="{ active: item.isShowSubCon }">
                    <li class="sub-con-list" v-for="sub in item.MyAction" :key="sub.key">
                      <div class="left-title">{{ sub.text }}</div>
                      <ul class="right-wrap">
                        <li class="list" v-for="three in sub.children" :key="three.key">{{ three.text }}</li>
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
        </el-scrollbar>
      </div>
    </div>
    <!-- dialog对话框 -->
    <el-dialog
      :visible.sync="dialogVisible.isShow"
      :modal-append-to-body="false"
      :close-on-click-modal="false"
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
export default {
  name: 'SeeUserList',
  components: {
    XButton
  },
  data () {
    return {
      tableHeaderWidth: '100%',
      userList: [],
      userName: '',
      authority: [],
      actionInfo: [],
      tunnel: '',
      tunnelList: [],
      getEditUserID: '',
      isDelete: '',
      editUserID: [],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      // 所属管廊宽度
      galleryWidth: '40%',
      galleryList: [],
      currentSort: {
        sort: 'UserID',
        order: 'asc'
      },
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      shrinkAll: {
        text: '全部展开',
        mark: true
      },
      headOrder: {
        UserID: 1,
        UserName: 0,
        LmTime: 0,
        LmName: 0,
        IsDelete: 0,
        Department: 0
      } // 默认UserID升序asc，箭头朝上，同时只可能按照一个字段排序，不排序的字段不出现箭头,0不排序、1升序、2降序，切换时默认升序
    }
  },
  created () {
    this.init()
    // 权限列表
    window.axios.get('/ActionInfo/GetActionInfoForElCascader').then(res => {
      this.actionInfo = res.data
      this.actionInfo.unshift({value: '', label: '所有'})
    }).catch(err => console.log(err))
    // 管廊列表
    window.axios.get('/UtilityTunnel/GetUtilityTunnel').then(res => (this.tunnelList = res.data)).catch(err => console.log(err))
  },
  watch: {
    galleryWidth () {
      let oScrollView = document.querySelector('#use-scroll .el-scrollbar__view')
      oScrollView.style.width = `${oScrollView.offsetWidth + parseFloat(this.galleryWidth)}px`
    }
  },
  methods: {
    init () {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = 1
      this.searchResult(1)
    },

    // 添加用户
    add () {
      this.$router.push({
        name: 'AddUser',
        query: { t: 'add' }
      })
    },

    // 修改用户
    edit () {
      if (!this.editUserID.length) {
        this.$message({
          message: '请选择需要修改的用户',
          duration: 2000,
          type: 'warning'
        })
      } else if (this.editUserID.length > 1) {
        this.$message({
          message: '修改的用户不能超过1个',
          duration: 2000,
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddUser',
          query: { t: 'edit', id: this.editUserID }
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
      } else if (!this.editUserID.length) { // 需要切换到界面模式，获取对象数据，根据对象数据中是否含有失效用户进行判断
        this.$message({
          message: '失效用户无需删除',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条用户信息?'
        this.dialogWhich = 1
      }
    },

    // 弹框确认是否删除
    dialogEnter () {
      if (this.dialogWhich === 1) {
        window.axios.post('/UserInfo/DeleteUserInfo', {
          ids: this.editUserID.join(',')
        }).then(res => {
          if (res.data === 'OK') {
            this.$message({
              message: '删除成功',
              type: 'success'
            })
            this.init()
          } else {
            this.$message({
              message: '删除失败',
              type: 'error'
            })
          }
          // 隐藏dialog
          this.dialogVisible.isShow = false
        }).catch(err => console.log(err))
      } else if (this.dialogWhich === 2) {
        window.axios.post('/UserInfo/ReDeleteUserInfo', {
          ids: this.editUserID.join(',')
        }).then(res => {
          if (res.data === 'OK') {
            this.$message({
              message: '恢复成功',
              type: 'success'
            })
            this.init()
          } else {
            this.$message({
              message: '恢复失败',
              type: 'error'
            })
          }
          // this.dialogVisible.isShow = true
          // this.dialogVisible.btn = false
          // this.dialogVisible.text = '您所选择的删除用户信息已恢复'
          // 隐藏dialog
          this.dialogVisible.isShow = false
        }).catch(err => console.log(err))
      } else if (this.dialogWhich === 3) {
        window.axios.post('/UserInfo/UserInfoSet', {
          ids: this.editUserID.join(',')
        }).then(res => {
          if (res.data === 'OK') {
            this.$message({
              message: '密码重置成功',
              type: 'success'
            })
            this.init()
          } else {
            this.$message({
              message: '密码重置失败',
              type: 'error'
            })
          }
          // 隐藏dialog
          this.dialogVisible.isShow = false
        }).catch(err => console.log(err))
      }
    },

    // 删除恢复
    deleteRecovery () {
      if (!this.editUserID.length) {
        this.$message({
          message: '请选择需要恢复的用户',
          type: 'warning'
        })
      } else if (!this.editUserID.length) { // 需要切换到界面模式，获取对象数据，根据对象数据中是否含有有效用户进行判断
        this.$message({
          message: '有效用户无需恢复',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定恢复该条用户信息?'
        this.dialogWhich = 2
      }
    },

    // 密码重置
    setPassword () {
      if (!this.editUserID.length) {
        this.$message({
          message: '请选择需要密码重置的用户',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定该用户密码重置?'
        this.dialogWhich = 3
      }
    },
    // 改变排序
    changeOrder (sort) {
      // console.log(this.headOrder[sort])
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.UserID = 0
        this.headOrder.UserName = 0
        this.headOrder.LmTime = 0
        this.headOrder.LmName = 0
        this.headOrder.IsDelete = 0
        this.headOrder.Department = 0
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

    // 按钮搜索
    search () {
      this.searchResult(1)
    },

    // 搜索
    searchResult (page) {
      this.$emit('title', '| 用户定义')
      this.currentPage = page
      this.loading = true
      window.axios.post('/UserInfo/GetUserInfo', {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        SearchName: this.userName,
        SearchRole: this.authority[1],
        searchIsDelete: this.isDelete,
        SearchTunnel: this.tunnel
      }).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.isShowSubCon = false
          item.LmTime = transformDate(item.LmTime)
        })
        this.total = res.data.total
        this.userList = res.data.rows
        this.galleryWidth = `${res.data.tunnels.length * 80}px`
        this.galleryList = res.data.tunnels
      }).catch(err => console.log(err))
    },

    // 收起展开权限列表
    shrinkSubContent (index, id, length) {
      this.userList[index].isShowSubCon ? this.userList[index].isShowSubCon = true : this.userList[index].isShowSubCon = false
      this.userList[index].isShowSubCon = !this.userList[index].isShowSubCon
    },

    // 全部展开、收起
    shrinkAllSubContent () {
      if (this.shrinkAll.mark) {
        this.shrinkAll.text = '全部收起'
        this.userList.map(item => {
          item.isShowSubCon = true
          item.height = `${item.MyAction.length * 41}px`
        })
      } else {
        this.shrinkAll.text = '全部展开'
        this.userList.map(item => {
          item.isShowSubCon = false
          item.height = '0px'
        })
      }
      this.shrinkAll.mark = !this.shrinkAll.mark
    },

    // 获取修改用户id
    emitEditID () {
      this.$emit('editUserID', this.editUserID)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.userList.map(val => this.editUserID.push(val.UserID)) : this.editUserID = []
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

  label{
    width: PXtoEm(32);
  }

  .inp{
    width: PXtoEm(160);
    margin-left: PXtoEm(10);
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
    overflow-y: hidden;
    height: 100%;

    /deep/ .el-scrollbar__view{
      overflow-y: hidden;
      width: 100%;
      height: 100%;

      .el-scrollbar__view{
        height: auto;
      }

      .el-scrollbar__bar{
        &.is-vertical{
          display: block;
          position: fixed;
          bottom: 0 !important;
          top: initial;
          height: 54.5%;
        }
      }
    }

    /deep/ .el-scrollbar__bar{
      &.is-horizontal{
        display: block !important;
      }

      &.is-vertical{
        display: none;
      }
    }
  }

  // 列表滚动区域
  .scroll-1{
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
      width: 100px;
      margin-left: 5%;
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
        display: flex;
        flex-wrap: wrap;
        width: 80%;
      }

      .list{
        width: 130px;
        margin-left: 10px;
      }
    }
  }

  .btn-wrap{
    width: 40px;
    font-size: 18px;
    text-align: left;

    .icon-arrow-top-f{
      display: inline-block;
      transition: .5s;
      cursor: pointer;

      &.active{
        transform: rotateZ(180deg);
      }
    }
  }

  .number,
  .name,
  .branch,
  .state,
  .phone,
  .email,
  .last-maintainer{
    width: 100px;
    word-break: break-all;
  }

  .last-update-time{
    width: 200px;
    color: $color-content-text;
  }
}
</style>
