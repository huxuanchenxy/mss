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
            <label for="name">代码名称</label>
            <div class="inp">
              <el-input placeholder="请输入代码名称" v-model.trim="codeName"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">子代码名称</label>
            <div class="inp">
              <el-input placeholder="请输入子代码名称" v-model="subCodeName"></el-input>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="add"><x-button>添加</x-button></li>
        <li class="list" @click="remove"><x-button>删除</x-button></li>
        <li class="list" @click="edit"><x-button>修改</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox" v-model="bCheckAll" @change="checkAll"></li>
        <li class="list number c-pointer" @click="changeOrder('id')">
          代码编号
          <i :class="[{ 'el-icon-d-caret': headOrder.id === 0 }, { 'el-icon-caret-top': headOrder.id === 1 }, { 'el-icon-caret-bottom': headOrder.id === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('code_name')">
          代码名称
          <i :class="[{ 'el-icon-d-caret': headOrder.code_name === 0 }, { 'el-icon-caret-top': headOrder.code_name === 1 }, { 'el-icon-caret-bottom': headOrder.code_name === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('code')">
          代码值
          <i :class="[{ 'el-icon-d-caret': headOrder.code === 0 }, { 'el-icon-caret-top': headOrder.code === 1 }, { 'el-icon-caret-bottom': headOrder.code === 2 }]"></i>
        </li>
        <li class="list name">子代码名称</li>
        <li class="list name">子代码值</li>
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
            <li class="list" v-for="(item) in codeList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="editCodeID" :value="item.id" @change="emitEditID">
                </div>
                <div class="number">{{ item.id }}</div>
                <div class="name">{{ item.code_name }}</div>
                <div class="name">{{ item.code }}</div>
                <div class="name">{{ item.sub_code_name }}</div>
                <div class="name">{{ item.sub_code }}</div>
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
  name: 'SeeCodeList',
  components: {
    XButton
  },
  data () {
    return {
      codeList: [],
      codeName: '',
      subCodeName: '',
      geteditCodeID: '',
      editCodeID: [],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'id',
        order: 'asc'
      },
      headOrder: {
        id: 1,
        code_name: 0,
        code: 0,
        updated_time: 0,
        updated_by: 0
      },
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      }
    }
  },
  created () {
    this.init()
  },
  methods: {
    init () {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = 1
      this.searchResult(1)
    },

    // 添加代码
    add () {
      this.$emit('title', '| 添加代码')
      this.$router.push({
        name: 'AddCode',
        query: {
          t: 'add'
        }
      })
    },

    // 修改代码
    edit () {
      if (!this.editCodeID.length) {
        this.$message({
          message: '请选择修改的代码',
          type: 'warning'
        })
      } else if (this.editCodeID.length > 1) {
        this.$message({
          message: '修改的代码不能超过1个'
        })
      } else {
        this.$emit('title', '| 修改代码')
        this.showChildren = 'edit'
        this.$router.push({
          name: 'AddCode',
          query: { t: 'edit', id: this.editCodeID[0] }
        })
      }
    },

    // 删除代码
    remove () {
      if (!this.editCodeID.length) {
        this.$message({
          message: '请选择需要删除的代码',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条代码信息?'
      }
    },

    // 搜索功能
    searchRes () {
      this.init()
    },

    // 弹框确认是否删除
    dialogEnter () {
      api.delDictionary(this.editCodeID.join(',')).then(res => {
        if (res.code === 0) {
          this.$message({
            message: '删除成功',
            type: 'success'
          })
          this.currentPage = 1
          this.searchResult(1)
        } else {
          this.$message({
            message: '删除失败',
            type: 'error'
          })
        }
        // 隐藏dialog
        this.dialogVisible.isShow = false
      }).catch(err => console.log(err))
    },

    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.code_name = 0
        this.headOrder.code_value = 0
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
      this.$emit('title', '| 代码管理')
      this.currentPage = page
      this.loading = true
      api.getDictionary({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        searchName: this.codeName,
        searchSubName: this.subCodeName
      }).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.updated_time = transformDate(item.updated_time)
        })
        this.codeList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },

    // 获取修改代码id
    emitEditID () {
      this.$emit('editCodeID', this.editCodeID)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.codeList.map(val => this.editCodeID.push(val.ID)) : this.editCodeID = []
      // this.emitEditID()
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
.wrap{
  height: percent($content-height - 56, $content-height);
}
// 内容区
.content-wrap{
  $con-height: $content-height - (145 + 56);
  overflow: hidden;
  height: percent($con-height, $content-height - 56);
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
    }

    .left-title{
      margin-right: 10px;
      font-weight: bold;
    }
  }

  .number,
  .name,
  .btn-wrap{
    width: 10%;
  }

  .last-update-time{
    width: 20%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 15%;
  }
}
</style>
