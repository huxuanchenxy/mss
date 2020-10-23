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
            <label for="name">设备类型名称</label>
            <div class="inp">
              <el-input v-model.trim="eqpTypeName" placeholder="请输入设备类型名称"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">设备类型描述</label>
            <div class="inp">
              <el-input v-model.trim="description" placeholder="请输入设备类型描述"></el-input>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="add"><x-button :disabled="btn.save">添加</x-button></li>
        <li class="list" @click="remove"><x-button :disabled="btn.delete">删除</x-button></li>
        <li class="list" @click="edit"><x-button :disabled="btn.update">修改</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox" v-model="bCheckAll" @change="checkAll"></li>
        <li class="list number c-pointer" @click="changeOrder('id')">
          编号
          <i :class="[{ 'el-icon-d-caret': headOrder.id === 0 }, { 'el-icon-caret-top': headOrder.id === 1 }, { 'el-icon-caret-bottom': headOrder.id === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('type_name')">
          名称
          <i :class="[{ 'el-icon-d-caret': headOrder.type_name === 0 }, { 'el-icon-caret-top': headOrder.type_name === 1 }, { 'el-icon-caret-bottom': headOrder.type_name === 2 }]"></i>
        </li>
        <li class="list name">型号</li>
        <li class="list url">描述</li>
        <li class="list upload-cascader">上传文件</li>
        <li class="list last-update-time c-pointer" @click="changeOrder('updated_time')">
          最后更新时间
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_time === 0 }, { 'el-icon-caret-top': headOrder.updated_time === 1 }, { 'el-icon-caret-bottom': headOrder.updated_time === 2 }]"></i>
        </li>
        <li class="list last-update-time c-pointer" @click="changeOrder('updated_by')">
          最后更新人
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_by === 0 }, { 'el-icon-caret-top': headOrder.updated_by === 1 }, { 'el-icon-caret-bottom': headOrder.updated_by === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="item in eqpTypeList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="editEqpTypeID" :value="item.id" @change="emitEditID">
                </div>
                <div class="number">{{ item.id }}</div>
                <div class="name word-break">
                  <router-link :to="{ name: 'SeeEqpList', params: { id: item.id } }">{{ item.tName }}</router-link>
                </div>
                <div class="name word-break">{{ item.model }}</div>
                <div class="url word-break">{{ item.desc }}</div>
                <div class="upload-cascader">
                  <el-cascader clearable
                    v-show="item.uploadFileArr.length != 0"
                    @change="preview"
                    :show-all-levels="false"
                    :options="item.uploadFileArr">
                  </el-cascader>
                </div>
                <div class="last-update-time color-white word-break">{{ item.updatedTime }}</div>
                <div class="last-update-time word-break">{{ item.updatedName }}</div>
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
    <el-dialog
      :visible.sync="centerDialogVisible"
      :modal-append-to-body="false"
      custom-class="show-list-wrap"
      center>
      <iframe :src="previewUrl" width="100%" height="100%" frameborder="0"></iframe>
    </el-dialog>
  </div>
</template>
<script>
import { transformDate, PDF_UPLOADED_VIEW_URL } from '@/common/js/utils.js'
import { isPreview } from '@/common/js/UpDownloadFileHelper.js'
import { btn } from '@/element/btn.js'
import XButton from '@/components/button'
import api from '@/api/eqpApi'
export default {
  name: 'SeeEqpTypeList',
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
      title: ' | 设备类型定义',
      eqpTypeName: '',
      description: '',
      eqpTypeList: [],
      editEqpTypeID: [],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'ID',
        order: 'asc'
      },
      dialogVisible: {
        isShow: false,
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      headOrder: {
        id: 1,
        type_name: 0,
        updated_time: 0,
        updated_by: 0
      },
      centerDialogVisible: false,
      uploadFile: {},
      previewUrl: ''
    }
  },
  created () {
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    if (!user.is_super) {
      let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
      this.btn.save = !actions.some((item, index) => {
        return item.actionID === btn.eqpType.save
      })
      this.btn.delete = !actions.some((item, index) => {
        return item.actionID === btn.eqpType.delete
      })
      this.btn.update = !actions.some((item, index) => {
        return item.actionID === btn.eqpType.update
      })
    }
    this.init()
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
    preview (val) {
      let id = val[val.length - 1]
      if (isPreview(id, this.uploadFile[id].label)) {
        api.fileIsExist(id).then(res => {
          if (res.data) {
            this.previewUrl = PDF_UPLOADED_VIEW_URL + this.uploadFile[id].url
            this.centerDialogVisible = true
          } else {
            this.$message({
              message: '未找到所要查看的文件',
              type: 'warning'
            })
          }
        }).catch(err => console.log(err))
      }
      // 'http://127.0.0.1:8090' + '/Compoment/pdfViewer/web/viewer.html?file=/' + item
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.type_name = 0
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
      api.getEqpType({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        searchName: this.eqpTypeName,
        searchDesc: this.description
      }).then(res => {
        this.loading = false
        if (res.data.total === 0) {
          this.eqpTypeList = []
        } else {
          res.data.rows.map(item => {
            item.updatedTime = transformDate(item.updatedTime)
            if (item.uploadFiles !== null) {
              item.uploadFileArr = JSON.parse(item.uploadFiles)
              item.uploadFileArr.map(val => {
                val.children.map(item => {
                  this.uploadFile[item.value] = item
                })
              })
            } else {
              item.uploadFileArr = []
            }
          })
          this.eqpTypeList = res.data.rows
        }
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    add () {
      // 判断权限，符合则允许跳转
      this.$router.push({name: 'AddEqpType', query: { type: 'Add' }})
    },
    // 修改设备类型
    edit () {
      if (!this.editEqpTypeID.length) {
        this.$message({
          message: '请选择修改操作的设备类型',
          type: 'warning'
        })
      } else if (this.editEqpTypeID.length > 1) {
        this.$message({
          message: '修改的设备类型不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddEqpType',
          query: {
            id: this.editEqpTypeID[0],
            type: 'edit'
          }
        })
      }
    },

    // 删除设备类型
    remove () {
      if (!this.editEqpTypeID.length) {
        this.$message({
          message: '请选择需删除作的设备类型',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条设备类型信息?'
      }
    },

    // 搜索功能
    searchRes () {
      this.$emit('title', '| 设备类型定义')
      this.loading = true
      this.init()
      this.searchResult(1)
    },

    // 弹框确认是否删除
    dialogEnter () {
      api.delEqpType(this.editEqpTypeID.join(',')).then(res => {
        if (res.code === 0) {
          this.editEqpTypeID = []
          this.$message({
            message: '删除成功',
            type: 'success'
          })
          this.currentPage = 1
          this.searchResult(1)
        } else {
          this.$message({
            message: '删除失败，' + res.msg,
            type: 'error'
          })
        }
        // 隐藏dialog
        this.dialogVisible.isShow = false
      }).catch(err => console.log(err))
    },

    // 获取修改设备类型id
    emitEditID () {
      this.$emit('editEqpTypeID', this.editEqpTypeID)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.eqpTypeList.map(val => this.editEqpTypeID.push(val.id)) : this.editEqpTypeID = []
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
    width: 8%;
  }

  .last-update-time{
    width: 10%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 10%;
  }

  .upload-cascader{
    width: 13%;
  }

  .url{
    width: 10%;
  }

  .menuOrder{
    width: 10%;
  }
}

// 图片
.pdf-btn{
  display: flex;
  justify-content: center;
  .box{
    position: relative;
    width: 60px;
    height: 30px;
    text-align: center;
    line-height: 30px;
    cursor: pointer;

    &:before{
      content: "\e6cc";
      font-size: 28px;
      font-family: "iconfont";
      color: #D8D8D8;
    }
  }
}
</style>
