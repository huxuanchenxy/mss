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
            <label for="name">设备类型</label>
            <div class="inp">
              <el-select v-model="eqpTypeID" @change="chooseEqpType" placeholder="请选择">
                <el-option label="所有设备类型" value=''></el-option>
                <el-option
                  v-for="item in eqpTypeList"
                  :key="item.key"
                  :label="item.tName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">参数名</label>
            <div class="inp">
              <el-select v-model="paramID" placeholder="请选择">
                <el-option
                  v-for="item in paramList"
                  :key="item.key"
                  :label="item._paramName"
                  :value="item._paramID">
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
            <router-link :to="{ name: 'WarnSetting', params: { mark: 'add' } }">添加</router-link>
          </x-button>
        </li>
        <li class="list" @click="remove"><x-button>删除</x-button></li>
        <li class="list" @click="edit"><x-button>修改</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><el-checkbox v-model="bCheckAll" @change="checkAll"></el-checkbox></li>
        <li class="list number c-pointer" @click="changeOrder('id')">
          序号
          <i :class="[{ 'el-icon-d-caret': headOrder.id === 0 }, { 'el-icon-caret-top': headOrder.id === 1 }, { 'el-icon-caret-bottom': headOrder.id === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('equipment_type_id')">
          设备类型
          <i :class="[{ 'el-icon-d-caret': headOrder.equipment_type_id === 0 }, { 'el-icon-caret-top': headOrder.equipment_type_id === 1 }, { 'el-icon-caret-bottom': headOrder.equipment_type_id === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('param_name')">
          参数名
          <i :class="[{ 'el-icon-d-caret': headOrder.param_name === 0 }, { 'el-icon-caret-top': headOrder.param_name === 1 }, { 'el-icon-caret-bottom': headOrder.param_name === 2 }]"></i>
        </li>
        <li class="list name c-pointer">
          最小阀值
        </li>
        <li class="list name c-pointer">
          最大阀值
        </li>
        <li class="list name c-pointer">
          扩展条件
        </li>
        <li class="list number c-pointer">
          是否启用
        </li>
        <li class="list last-maintainer c-pointer" @click="changeOrder('updated_by')">
          最后更新人
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_by === 0 }, { 'el-icon-caret-top': headOrder.updated_by === 1 }, { 'el-icon-caret-bottom': headOrder.updated_by === 2 }]"></i>
        </li>
        <li class="list last-update-time c-pointer" @click="changeOrder('updated_time')">
          最后更新时间
          <i :class="[{ 'el-icon-d-caret': headOrder.updated_time === 0 }, { 'el-icon-caret-top': headOrder.updated_time === 1 }, { 'el-icon-caret-bottom': headOrder.updated_time === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in dataList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <el-checkbox v-model="checkedID" :label="item.id"></el-checkbox>
                </div>
                <div class="number">{{ item.id }}</div>
                <div class="number">{{ item.equipmentTypeName }}</div>
                <div class="number">{{ item.paramName }}</div>
                <div class="number">{{ item.paramLimitLower }}</div>
                <div class="number">{{ item.paramLimitUpper }}</div>
                <div class="number">{{ item.settingExShow }}</div>
                <div class="number">{{ item.isActivedShow }}</div>
                <div class="last-maintainer">{{ item.userName }}</div>
                <div class="last-update-time color-white">{{ item.updatedTime }}</div>
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
import api from '@/api/warnSettingApi'
export default {
  name: 'SeeUserList',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 预警设置',
      eqpTypeList: [],
      paramList: [],
      eqpTypeID: '',
      paramID: '',
      dataList: [],
      checkedID: [],

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
        equipment_type_id: 0,
        param_name: 0,
        updated_time: 0,
        updated_by: 0
      }
    }
  },
  created () {
    this.getAllEqpType()
    this.init()
  },
  activated () {
    this.search()
  },
  methods: {
    init () {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = 1
      this.search()
      // this.searchResult(1)
    },
    getAllEqpType () {
      api.getAllEqpType().then(res => {
        this.eqpTypeList = res.data
      }).catch(err => console.log(err))
    },
    chooseEqpType () {
      let id = this.eqpTypeID
      this.paramList = []
      this.paramID = ''
      if (id !== '') {
        api.getParamByEqpType(id).then(res => {
          this.paramList = res.data
        }).catch(err => console.log(err))
      }
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.equipment_type_id = 0
        this.headOrder.param_name = 0
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
      this.search()
    },
    search () {
      this.loading = true
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: this.currentPage,
        EquipmentTypeID: this.eqpTypeID,
        ParamID: this.paramID
      }
      api.getWarnSetting(parm).then(res => {
        this.loading = false
        res.data.rows.map(item => {
          item.updatedTime = transformDate(item.updatedTime)
          item.isActivedShow = item.isActived ? '是' : '否'
          item.settingExShow = ''
          item.settingEx.forEach(el => {
            let op = ''
            if (el.paramLimitType === 1) {
              op = '>'
            } else if (el.paramLimitType === 2) {
              op = '='
            } else {
              op = '<'
            }
            item.settingExShow += (el.pidName + op + el.paramLimitValue + ' ')
          })
        })
        this.dataList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },

    // 修改用户
    edit () {
      if (!this.checkedID.length) {
        this.$message({
          message: '请选择一行',
          type: 'warning'
        })
      } else if (this.checkedID.length > 1) {
        this.$message({
          message: '选择不能超过一行',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'WarnSetting',
          params: {
            id: this.checkedID[0],
            mark: 'edit'
          }
        })
      }
    },

    // 删除用户
    remove () {
      if (!this.checkedID.length) {
        this.$message({
          message: '请选择需要删除的行',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该行信息?'
      }
    },
    // 弹框确认是否删除
    dialogEnter () {
      api.deleteWarnningSetting(this.checkedID.join(',')).then(res => {
        if (res.code === 0) {
          this.checkedID = []
          this.$message({
            message: '删除成功',
            type: 'success'
          })
          this.currentPage = 1
          this.search()
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
      this.init()
      this.search()
    },
    // 全选
    checkAll () {
      this.bCheckAll ? this.dataList.map(val => this.checkedID.push(val.id)) : this.checkedID = []
    },

    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
      this.search()
    },

    // 上一页
    prevPage (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
      this.search()
    },

    // 下一页
    nextPage (val) {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = val
      this.search()
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
  /deep/ .el-checkbox__label{
    display: none;
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
