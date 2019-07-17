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
    <div class="box" style="height:68px;">
<ul class="con-padding-horizontal btn-group" style="padding-top: 20px;">
        <li class="list">
          <x-button>
            <router-link :to="{ name: 'addMaintainConfig', params: { mark: 'add' } }">添加</router-link>
          </x-button>
        </li>
        <li class="list" @click="remove"><x-button>删除</x-button></li>
        <li class="list" @click="edit"><x-button>修改</x-button></li>
        <li class="list" @click="look"><x-button>查看</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox" v-model="bCheckAll" style="visibility: hidden;"></li>
        <li class="list number c-pointer" @click="changeOrder('reminder')">
          提醒方式
          <i :class="[{ 'el-icon-d-caret': headOrder.reminder === 0 }, { 'el-icon-caret-top': headOrder.reminder === 1 }, { 'el-icon-caret-bottom': headOrder.reminder === 2 }]"></i>
       </li>
        <li class="list number c-pointer" @click="changeOrder('beforeDead')">
          寿命提前提醒天数
          <i :class="[{ 'el-icon-d-caret': headOrder.beforeDead === 0 }, { 'el-icon-caret-top': headOrder.beforeDead === 1 }, { 'el-icon-caret-bottom': headOrder.beforeDead === 2 }]"></i>
       </li>
       <li class="list number c-pointer" @click="changeOrder('beforeMaintainMiddle')">
          中修提前提醒天数
          <i :class="[{ 'el-icon-d-caret': headOrder.beforeMaintainMiddle === 0 }, { 'el-icon-caret-top': headOrder.beforeMaintainMiddle === 1 }, { 'el-icon-caret-bottom': headOrder.beforeMaintainMiddle === 2 }]"></i>
       </li>
        <li class="list number c-pointer" @click="changeOrder('beforeMaintainBig')" >
          大修提前提醒天数
          <i :class="[{ 'el-icon-d-caret': headOrder.beforeMaintainBig === 0 }, { 'el-icon-caret-top': headOrder.beforeMaintainBig === 1 }, { 'el-icon-caret-bottom': headOrder.beforeMaintainBig === 2 }]"></i>
       </li>
        <li class="list number c-pointer" @click="changeOrder('textTemplate')" >提醒模板
          <i :class="[{ 'el-icon-d-caret': headOrder.textTemplate === 0 }, { 'el-icon-caret-top': headOrder.textTemplate === 1 }, { 'el-icon-caret-bottom': headOrder.textTemplate === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('published')" >是否启用规则
<i :class="[{ 'el-icon-d-caret': headOrder.published === 0 }, { 'el-icon-caret-top': headOrder.published === 1 }, { 'el-icon-caret-bottom': headOrder.published === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('updatedTime')">最后更新时间
          <i :class="[{ 'el-icon-d-caret': headOrder.updatedTime === 0 }, { 'el-icon-caret-top': headOrder.updatedTime === 1 }, { 'el-icon-caret-bottom': headOrder.updatedTime === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('updatedName')">最后更新人
          <i :class="[{ 'el-icon-d-caret': headOrder.updatedName === 0 }, { 'el-icon-caret-top': headOrder.updatedName === 1 }, { 'el-icon-caret-bottom': headOrder.updatedName === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in ObjList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="lookOperlogID" :value="item.id" @change="emitEditID">
                </div>
                <div class="name">{{ item.reminder }}</div>
                <div class="name">{{ item.beforeDead }}</div>
                <!--<div class="name">
                  <router-link :to="{ name: 'SeeActionList', params: { id: item.id } }">{{ item.user_name }}</router-link>
                </div>-->
                <div class="name">{{ item.beforeMaintainMiddle }}</div>
                <div class="name">{{ item.beforeMaintainBig }}</div>
                <div class="name">{{ item.textTemplate }}</div>
                <div class="name">{{ item.published }}</div>
                <div class="name">{{ item.updatedTime }}</div>
                <div class="name">{{ item.updatedName }}</div>
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
import api from '@/api/operlogApi'
export default {
  name: 'SeeMaintainConfig',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 设备配置列表',
      time: '',
      userName: '',
      actionName: '',
      methodName: '',
      startTime: '',
      endTime: '',
      role: '',
      roleList: [],
      ObjList: [],
      lookOperlogID: [],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'id',
        order: 'desc'
      },
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      headOrder: {
        id: 0,
        reminder: 0,
        beforeDead: 0,
        beforeMaintainMiddle: 0,
        beforeMaintainBig: 0,
        textTemplate: 0,
        published: 0,
        updatedTime: 0,
        updatedName: 0
      }
    }
  },
  created () {
    this.$emit('title', '| 日志查看')
    this.init()
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  methods: {
    init () {
      this.searchResult(1)
    },
    transformreminder (obj) {
      var r
      if (obj === 1) {
        r = '短信'
      } else if (obj === 2) {
        r = '邮件'
      } else if (obj === 3) {
        r = '短信,邮件'
      }
      return r
    },
    transformpublish (obj) {
      var r
      if (obj === true) {
        r = '启用'
      } else if (obj === false) {
        r = '禁用'
      }
      return r
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.acc_name = 0
        this.headOrder.user_name = 0
        this.headOrder.controller_name = 0
        this.headOrder.method_name = 0
        this.headOrder.ip = 0
        this.headOrder.mac_add = 0
        this.headOrder.created_time = 0
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
        page: page
      }
      api.getEquipmentConfig(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.updatedTime = transformDate(item.updatedTime)
          item.reminder = this.transformreminder(item.reminder)
          item.published = this.transformpublish(item.published)
        })
        this.ObjList = res.data.rows
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
      this.$emit('lookOperlogID', this.lookOperlogID)
    },
    // 全选
    // checkAll () {
    //   this.bCheckAll ? this.ObjList.map(val => this.editUserID.push(val.id)) : this.editUserID = []
    //   this.emitEditID()
    // },
    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.bCheckAll = false
      // this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 上一页
    prevPage (val) {
      this.bCheckAll = false
      // this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 下一页
    nextPage (val) {
      this.bCheckAll = false
      // this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },
    look () {
      if (!this.lookOperlogID.length) {
        this.$message({
          message: '请选择需要查看的配置',
          type: 'warning'
        })
      } else if (this.lookOperlogID.length > 1) {
        this.$message({
          message: '查看的配置不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'addMaintainConfig',
          params: {
            id: this.lookOperlogID[0],
            mark: 'look'
          }
        })
      }
    },
    // 修改
    edit () {
      if (!this.lookOperlogID.length) {
        this.$message({
          message: '请选择需要修改的配置',
          type: 'warning'
        })
      } else if (this.lookOperlogID.length > 1) {
        this.$message({
          message: '修改的配置不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'addMaintainConfig',
          params: {
            id: this.lookOperlogID[0],
            mark: 'edit'
          }
        })
      }
    },
    remove () {
      if (!this.lookOperlogID.length) {
        this.$message({
          message: '请选择需要删除的配置',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条配置信息?'
      }
    },
    // 弹框确认是否删除
    dialogEnter () {
      api.delEquipmentConfig(this.lookOperlogID.join(',')).then(res => {
        if (res.code === 0) {
          this.lookOperlogID = []
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
<style>
  .box1{
    height: 30px;
  }
</style>
