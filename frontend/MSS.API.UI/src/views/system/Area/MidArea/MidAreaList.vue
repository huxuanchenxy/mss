<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }}
         {{ title }}
      </h2>
    </div>
    <div class="box">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
          <div class="input-group">
            <label for="name">站区名</label>
            <div class="inp">
              <el-input v-model.trim="AreaName" placeholder="请输入站区名称"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="">类型</label>
            <div class="inp">
              <el-select v-model="AreaType" clearable placeholder="请选择" @change="validateSelect()">
                 <el-option
                 v-for="item in AreaTypeList"
                 :key="item.key"
                 :value="item.id"
                 :label="item.areaName">
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
            <router-link :to="{ name: 'AddMidArea', params: { mark: 'add' } }">添加</router-link>
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
          序号
          <i :class="[{ 'el-icon-d-caret': headOrder.id === 0 }, { 'el-icon-caret-top': headOrder.id === 1 }, { 'el-icon-caret-bottom': headOrder.id === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('AreaName')">
          站区名
          <i :class="[{ 'el-icon-d-caret': headOrder.AreaName === 0 }, { 'el-icon-caret-top': headOrder.AreaName === 1 }, { 'el-icon-caret-bottom': headOrder.AreaName === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('ConfigType')">
          类型
          <i :class="[{ 'el-icon-d-caret': headOrder.ConfigType === 0 }, { 'el-icon-caret-top': headOrder.ConfigType === 1 }, { 'el-icon-caret-bottom': headOrder.ConfigType === 2 }]"></i>
        </li>
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
            <li class="list" v-for="(item,index) in ConfigBigAreaList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="editAreaIDList" :value="item.id" @change="emitEditID">
                </div>
                 <div class="number">{{index+1}}</div>
                 <div class="name">{{ item.areaName }}</div>
                <div class="number">{{ item.configTypeName }}</div>
                <div class="last-update-time color-white">{{ item.created_Time }}</div>
                <div class="last-maintainer">{{ '管理员' }}</div>
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
import api from '@/api/AreaApi'
export default {
  name: 'MidAreaList',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 站区配置',
      AreaName: '',
      AreaType: '',
      AreaTypeList: [],
      ConfigBigAreaList: [],
      editAreaIDList: [],
      UserInfo: {
        uid: '',
        UserName: '管理员'
      },
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
        AreaName: 0,
        AreaType: 0,
        Sort: 0,
        updated_time: 0,
        updated_by: 1
      }
    }
  },
  created () {
    this.$emit('title', '| 站区配置')
    this.init()

    // 站区配置类型列表
    api.SelectDicAreaData('1').then(res => {
      this.AreaTypeList = res.data
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
      this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.AreaName = 0
        this.headOrder.AreaType = 0
        this.headOrder.Sort = 0
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
        searchName: this.AreaName,
        searchType: this.AreaType
      }
      api.GetNameByUid('1').then(res => {
        this.UserInfo.UserName = res
      }).catch(err => console.log(err))
      api.GetBigAreaQueryPageByParm(parm).then(res => {
        this.loading = false
        res.data.map(item => {
          item.created_Time = transformDate(item.created_Time)
        })
        this.ConfigBigAreaList = res.data
        this.total = res.total
      }).catch(err => console.log(err))
    },

    // 修改站区
    edit () {
      if (!this.editAreaIDList.length) {
        this.$message({
          message: '请选择需要修改的站区',
          type: 'warning'
        })
      } else if (this.editAreaIDList.length > 1) {
        this.$message({
          message: '修改的站区不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddMidArea',
          params: {
            id: this.editAreaIDList[0],
            mark: 'edit'
          }
        })
      }
    },

    // 删除站区
    remove () {
      if (!this.editAreaIDList.length) {
        this.$message({
          message: '请选择需要删除的站区',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条站区信息?'
      }
    },
    // 弹框确认是否删除
    dialogEnter () {
      api.DelConfigBigAreaId(this.editAreaIDList.join(',')).then(res => {
        if (res.ret === 0) {
          this.editAreaIDList = []
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
      this.$emit('title', '| 站区别')
      this.loading = true
      this.init()
      this.searchResult(1)
    },

    // 获取修改站区id
    emitEditID () {
      this.$emit('editAreaIDList', this.editAreaIDList)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.ConfigBigAreaList.map(val => this.editAreaIDList.push(val.id)) : this.editAreaIDList = []
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
