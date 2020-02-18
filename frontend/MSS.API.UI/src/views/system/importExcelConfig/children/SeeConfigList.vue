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
            <label for="name">导入文件名</label>
            <div class="inp">
              <el-input v-model.trim="fileName" placeholder="请输入导入文件名"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="">匹配内容</label>
            <div class="inp">
              <el-select v-model="matchedClass" placeholder="请选择">
                <el-option v-for="item in classList" :key="item.key" :value="item.id" :label="item.showName"></el-option>
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
          <x-button :disabled="btn.save">
            <router-link :to="{ name: 'AddImportExcelConfig', params: { mark: 'add' } }">添加</router-link>
          </x-button>
        </li>
        <li class="list" @click="remove" :disabled="btn.delete"><x-button>删除</x-button></li>
        <li class="list" @click="edit('edit')" :disabled="btn.update"><x-button>修改</x-button></li>
        <li class="list" @click="edit('detail')" ><x-button>查看明细</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox" v-model="bCheckAll" @change="checkAll"></li>
        <li class="list number">导入文件名</li>
        <li class="list number">匹配内容</li>
        <li class="list content">导入字段</li>
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
            <li class="list" v-for="(item) in importExcelConfigList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="editID" :value="item.id" @change="emitEditID">
                </div>
                <div class="number">{{ item.fileName }}</div>
                <div class="number">{{ item.classShow }}</div>
                <div class="content">{{ item.field }}</div>
                <div class="last-update-time color-white">{{ item.updatedTime }}</div>
                <div class="last-maintainer">{{ item.updatedName }}</div>
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
// import { dictionary } from '@/common/js/dictionary.js'
import { btn } from '@/element/btn.js'
import XButton from '@/components/button'
import api from '@/api/eqpApi'
export default {
  name: 'SeeImportExcelConfigList',
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
      title: ' | Excel导入配置',
      fileName: '',
      matchedClass: '',
      classList: [],
      importExcelConfigList: [],
      editID: [],
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
        updated_time: 0,
        updated_by: 0
      }
    }
  },
  created () {
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    if (!user.is_super) {
      let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
      this.btn.save = !actions.some((item, index) => {
        return item.actionID === btn.importExcelConfig.save
      })
      this.btn.delete = !actions.some((item, index) => {
        return item.actionID === btn.importExcelConfig.delete
      })
      this.btn.update = !actions.some((item, index) => {
        return item.actionID === btn.importExcelConfig.update
      })
    }
    // this.$emit('title', '| 配置别')
    this.init()

    // 导入数据库列表
    api.getImportExcelClass().then(res => {
      this.classList = res.data
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
        searchName: this.fileName,
        searchClass: this.matchedClass
      }
      api.getImportExcelConfig(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.updatedTime = transformDate(item.updatedTime)
        })
        this.importExcelConfigList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },

    // 修改配置
    edit (mark) {
      if (!this.editID.length) {
        this.$message({
          message: '请选择需要操作的配置',
          type: 'warning'
        })
      } else if (this.editID.length > 1) {
        this.$message({
          message: '操作的配置不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddImportExcelConfig',
          params: {
            id: this.editID[0],
            mark: mark
          }
        })
      }
    },

    // 删除配置
    remove () {
      if (!this.editID.length) {
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
      api.delImportExcelConfig(this.editID.join(',')).then(res => {
        if (res.code === 0) {
          this.editID = []
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
      // this.$emit('title', '| 配置')
      this.loading = true
      this.init()
      this.searchResult(1)
    },

    // 获取修改配置id
    emitEditID () {
      this.$emit('editID', this.editID)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.importExcelConfigList.map(val => this.editID.push(val.id)) : this.editID = []
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
    },

    // 下一页
    nextPage (val) {
      this.bCheckAll = false
      this.checkAll()
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
    width: 8%;
  }

  .name{
    a{
      color: #42abfd;
    }
  }

  .last-update-time{
    width: 15%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 10%;
  }

  .content{
    width: 45%;
  }
}
</style>
