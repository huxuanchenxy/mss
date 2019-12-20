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
            <label for="name">检修单名称</label>
            <div class="inp">
              <el-input v-model.trim="entityTitle" placeholder="请输入检修单名称"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">检修单状态</label>
            <div class="inp">
              <el-select v-model="entityStatus" clearable filterable placeholder="请选择" >
                <el-option
                  v-for="item in entityStatusList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="name">最后保存/提交日期</label>
            <div class="inp">
              <el-date-picker
                v-model="searchDate"
                type="daterange"
                prefix-icon="el-icon-date"
                :unlink-panels="true"
                value-format="yyyy-MM-dd"
                range-separator="至"
                start-placeholder="开始日期"
                end-placeholder="结束日期">
              </el-date-picker>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="add"><x-button :disabled="btn.save">创建</x-button></li>
        <li class="list" @click="detail('edit')"><x-button :disabled="btn.update">修改</x-button></li>
        <li class="list" @click="del"><x-button :disabled="btn.delete">删除</x-button></li>
        <li class="list" @click="detail('reject')"><x-button :disabled="btn.reject">打回</x-button></li>
        <li class="list" @click="detail"><x-button >查看明细</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list number" />
        <li class="list name c-pointer" @click="changeOrder('title')">
          检修单名称
          <i :class="[{ 'el-icon-d-caret': headOrder.title === 0 }, { 'el-icon-caret-top': headOrder.title === 1 }, { 'el-icon-caret-bottom': headOrder.title === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('team')">
          班组
          <i :class="[{ 'el-icon-d-caret': headOrder.team === 0 }, { 'el-icon-caret-top': headOrder.team === 1 }, { 'el-icon-caret-bottom': headOrder.team === 2 }]"></i>
        </li>
        <li class="list name">地点</li>
        <li class="list name c-pointer" @click="changeOrder('status')">
          状态
          <i :class="[{ 'el-icon-d-caret': headOrder.status === 0 }, { 'el-icon-caret-top': headOrder.status === 1 }, { 'el-icon-caret-bottom': headOrder.status === 2 }]"></i>
        </li>
        <li class="list last-update-time c-pointer" @click="changeOrder('updated_time')">
          最后保存/提交时间
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_time === 0 }, { 'el-icon-caret-top': headOrder.updated_time === 1 }, { 'el-icon-caret-bottom': headOrder.updated_time === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('updated_by')">
          最后保存/提交人
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_by === 0 }, { 'el-icon-caret-top': headOrder.updated_by === 1 }, { 'el-icon-caret-bottom': headOrder.updated_by === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in mList" :key="item.key">
              <div class="list-content">
                <div class="number">
                  <input type="checkbox" v-model="editEntity" :value="item">
                </div>
                <div class="name word-break">{{ item.title }}</div>
                <div class="name word-break">{{ item.teamName }}</div>
                <div class="name word-break">{{ item.locationName }}</div>
                <div class="name word-break">{{ item.statusName }}</div>
                <div class="last-update-time color-white">{{ item.updatedTime }}</div>
                <div class="name">{{ item.updatedByName }}</div>
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
          <el-button @click="enter">是</el-button>
        </template>
        <el-button v-else @click="dialogVisible.isShow = false" :class="{ on: !dialogVisible.btn }">知道了</el-button>
      </template>
    </el-dialog>
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import { pmStatus, dictionary } from '@/common/js/dictionary.js'
// import { btn } from '@/element/btn.js'
import XButton from '@/components/button'
import api from '@/api/workflowApi'
import apiAuth from '@/api/authApi'
export default {
  name: 'EntityList',
  components: {
    XButton
  },
  data () {
    return {
      btn: {
        save: false,
        update: false,
        delete: false,
        reject: false
      },
      title: ' | 检修单',
      dialogVisible: {
        isShow: false,
        text: '',
        btn: true
      },
      editEntity: [],
      entityTitle: '',
      entityStatus: '',
      entityStatusList: [],
      searchDate: [],
      mList: [],
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'id',
        order: 'asc'
      },
      headOrder: {
        name: 0,
        team: 0,
        status: 0,
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
    // this.init()
    // 检修单状态加载
    apiAuth.getSubCode(dictionary.pmStatus).then(res => {
      this.entityStatusList = res.data
    }).catch(err => console.log(err))
  },
  activated () {
    this.searchResult(1)
  },
  methods: {
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.title = 0
        this.headOrder.team = 0
        this.headOrder.status = 0
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
      this.editEntity = []
      this.loading = true
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        title: this.entityTitle,
        status: this.entityStatus
      }
      if (this.searchDate.length !== 0) {
        parm.start = this.searchDate[0]
        parm.end = this.searchDate[1] + ' 23:59:59'
      }
      api.getEntityList(parm).then(res => {
        this.loading = false
        if (res.data.total > 0) {
          res.data.rows.map(item => {
            item.updatedTime = transformDate(item.updatedTime)
          })
          this.mList = res.data.rows
          this.total = res.data.total
        } else {
          this.mList = []
          this.total = 0
        }
      }).catch(err => console.log(err))
    },
    add () {
      this.$router.push({
        name: 'CreateEntity'
      })
    },
    detail (type) {
      if (this.editEntity.length > 1) {
        this.$message({
          message: '所要操作的检修单不能超过1个',
          type: 'warning'
        })
        return
      }
      if (type === 'edit') {
        if (this.editEntity[0].status === pmStatus.finished) {
          this.$message({
            message: '已提交检修单不可修改',
            type: 'warning'
          })
          return
        }
        this.$router.push({
          name: 'CreateEntity',
          params: {
            id: this.editEntity[0].id,
            type: 'edit'
          }
        })
      } else if (type === 'reject') {
        // 状态改回编辑
        api.updatePMEntityStatus(this.editEntity[0].id, pmStatus.editing).then(res => {
          if (res.code === 0) {
            this.searchResult(1)
            this.$message({
              message: '操作成功',
              type: 'success'
            })
          } else {
            this.$message({
              message: res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else {
        this.$router.push({
          name: 'DetailEntity',
          params: {
            id: this.editEntity[0].id,
            sourceName: 'EntityList'
          }
        })
      }
    },
    del () {
      for (let i = 0; i < this.editEntity.length; i++) {
        if (this.editEntity[i].status === pmStatus.finished) {
          this.$message({
            message: '已提交检修单不可删除',
            type: 'warning'
          })
          return
        }
      }
      this.dialogVisible.isShow = true
      this.dialogVisible.btn = true
      this.dialogVisible.text = '是否确认删除此检修单?'
    },
    enter () {
      let ids = []
      this.editEntity.map(val => {
        ids.push(val.id)
      })
      api.delEntity(ids.join(',')).then(res => {
        if (res.code === 0) {
          this.$message({
            message: '删除成功',
            type: 'success'
          })
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

  .number{
    width: 6%;
  }
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
