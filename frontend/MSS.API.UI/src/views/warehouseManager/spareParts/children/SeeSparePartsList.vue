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
            <label for="name">物资名称</label>
            <div class="inp">
              <el-input v-model.trim="sparePartsName" placeholder="请输入物资名称"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">物资类型</label>
            <div class="inp">
              <el-select v-model="type" clearable filterable placeholder="请选择物资类型">
                <el-option
                  v-for="item in typeList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">适用设备类型</label>
            <div class="inp">
              <el-select v-model="eqpType" clearable filterable placeholder="请选择设备类型">
                <el-option
                  v-for="item in eqpTypeList"
                  :key="item.key"
                  :label="item.tName"
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
        <li class="list" @click="add"><x-button :disabled="btn.save">添加</x-button></li>
        <li class="list" @click="remove"><x-button :disabled="btn.delete">删除</x-button></li>
        <li class="list" @click="edit"><x-button :disabled="btn.update">修改</x-button></li>
        <li class="list" @click="setAlarm" :disabled="btn.set"><x-button>仓库预警设定</x-button></li>
        <li class="list" @click="detail"><x-button>查看明细</x-button></li>
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
        <li class="list url c-pointer" @click="changeOrder('name')">
          物资名称
          <i :class="[{ 'el-icon-d-caret': headOrder.name === 0 }, { 'el-icon-caret-top': headOrder.name === 1 }, { 'el-icon-caret-bottom': headOrder.name === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('type')">
          物资类型
          <i :class="[{ 'el-icon-d-caret': headOrder.type === 0 }, { 'el-icon-caret-top': headOrder.type === 1 }, { 'el-icon-caret-bottom': headOrder.type === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('eqp_type')">
          适用类型
          <i :class="[{ 'el-icon-d-caret': headOrder.eqp_type === 0 }, { 'el-icon-caret-top': headOrder.eqp_type === 1 }, { 'el-icon-caret-bottom': headOrder.eqp_type === 2 }]"></i>
        </li>
        <li class="list name">计量单位</li>
        <li class="list last-update-time c-pointer" @click="changeOrder('updated_time')">
          最后更新时间
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_time === 0 }, { 'el-icon-caret-top': headOrder.updated_time === 1 }, { 'el-icon-caret-bottom': headOrder.updated_time === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('updated_by')">
          最后更新人
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_by === 0 }, { 'el-icon-caret-top': headOrder.updated_by === 1 }, { 'el-icon-caret-bottom': headOrder.updated_by === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="item in sparePartsList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="editSparePartsID" :value="item.id" @change="emitEditID">
                </div>
                <div class="number">{{ item.id }}</div>
                <div class="url">{{ item.name }}</div>
                <div class="name word-break">{{ item.typeName }}</div>
                <div class="name word-break">{{ item.eqpTypeName }}</div>
                <div class="name word-break">{{ item.unit }}</div>
                <div class="last-update-time color-white word-break">{{ item.updatedTime }}</div>
                <div class="name word-break">{{ item.updatedName }}</div>
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
import { btn } from '@/element/btn.js'
import { dictionary } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import apiAuth from '@/api/authApi'
import apiEqp from '@/api/eqpApi'
import api from '@/api/wmsApi'
export default {
  name: 'SeeSparePartsList',
  components: {
    XButton
  },
  data () {
    return {
      btn: {
        save: false,
        delete: false,
        update: false,
        set: false
      },
      title: ' | 物资定义',
      sparePartsName: '',
      type: '',
      typeList: [],
      eqpType: '',
      eqpTypeList: [],
      sparePartsList: [],
      editSparePartsID: [],
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
        name: 0,
        type: 0,
        eqp_type: 0,
        updated_time: 0,
        updated_by: 0
      },
      centerDialogVisible: false
    }
  },
  created () {
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    if (!user.is_super) {
      let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
      this.btn.save = !actions.some((item, index) => {
        return item.actionID === btn.spareParts.save
      })
      this.btn.delete = !actions.some((item, index) => {
        return item.actionID === btn.spareParts.delete
      })
      this.btn.update = !actions.some((item, index) => {
        return item.actionID === btn.spareParts.update
      })
      this.btn.set = !actions.some((item, index) => {
        return item.actionID === btn.spareParts.set
      })
    }
    apiAuth.getSubCode(dictionary.sparePartsType).then(res => {
      this.typeList = res.data
    }).catch(err => console.log(err))
    // 设备类型加载
    apiEqp.getEqpTypeAll().then(res => {
      this.eqpTypeList = res.data
    }).catch(err => console.log(err))
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
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.name = 0
        this.headOrder.type = 0
        this.headOrder.eqp_type = 0
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
      api.getSpareParts({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        SearchName: this.sparePartsName,
        SearchType: this.type,
        SearchEqpType: this.eqpType
      }).then(res => {
        this.loading = false
        if (res.data.total === 0) {
          this.sparePartsList = []
        } else {
          res.data.rows.map(item => {
            item.updatedTime = transformDate(item.updatedTime)
          })
          this.sparePartsList = res.data.rows
        }
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    add () {
      // 判断权限，符合则允许跳转
      this.$router.push({name: 'AddSpareParts', query: { type: 'Add' }})
    },
    // 修改物资
    edit () {
      if (!this.editSparePartsID.length) {
        this.$message({
          message: '请选择修改操作的物资',
          type: 'warning'
        })
      } else if (this.editSparePartsID.length > 1) {
        this.$message({
          message: '修改的物资不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddSpareParts',
          query: {
            id: this.editSparePartsID[0],
            type: 'edit'
          }
        })
      }
    },

    // 删除物资
    remove () {
      if (!this.editSparePartsID.length) {
        this.$message({
          message: '请选择需删除作的物资',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条物资信息?'
      }
    },

    // 查看物资明细
    detail () {
      if (!this.editSparePartsID.length) {
        this.$message({
          message: '请选择需要查看的物资',
          type: 'warning'
        })
      } else if (this.editSparePartsID.length > 1) {
        this.$message({
          message: '查看的物资不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'DetailSpareParts',
          params: {
            id: this.editSparePartsID[0]
            // sourceName: 'SeeEqpList'
          }
        })
      }
    },
    // 仓库预警设定
    setAlarm () {
      if (!this.editSparePartsID.length) {
        this.$message({
          message: '请选择需要设定的物资',
          type: 'warning'
        })
      } else if (this.editSparePartsID.length > 1) {
        this.$message({
          message: '设定的物资不能超过1个',
          type: 'warning'
        })
      } else {
        let tmp = this.sparePartsList.find((item, index) => {
          return item.id === this.editSparePartsID[0]
        })
        this.$router.push({
          name: 'SetWarehouseAlarm',
          params: {
            id: tmp.id,
            name: tmp.name,
            unit: tmp.unit,
            sourceName: 'SeeSparePartsList'
          }
        })
      }
    },
    // 搜索功能
    searchRes () {
      this.$emit('title', '| 物资定义')
      this.loading = true
      this.init()
      this.searchResult(1)
    },

    // 弹框确认是否删除
    dialogEnter () {
      api.delSpareParts(this.editSparePartsID.join(',')).then(res => {
        if (res.code === 0) {
          this.editSparePartsID = []
          this.$message({
            message: '删除成功',
            type: 'success'
          })
          this.currentPage = 1
          this.searchResult(1)
        } else {
          this.$message({
            message: res.msg === '' ? '删除失败' : res.msg,
            type: 'error'
          })
        }
        // 隐藏dialog
        this.dialogVisible.isShow = false
      }).catch(err => console.log(err))
    },

    // 获取修改物资id
    emitEditID () {
      this.$emit('editSparePartsID', this.editSparePartsID)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.sparePartsList.map(val => this.editSparePartsID.push(val.id)) : this.editSparePartsID = []
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
    width: 15%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 10%;
  }

  .upload-cascader{
    width: 13%;
  }

  .url{
    width: 15%;
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
