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
          <div class="input-group" v-show="false">
            <label for="name">故障编号</label>
            <div class="inp">
              <el-select v-model="trouble" clearable filterable placeholder="请选择故障编号">
                <el-option
                  v-for="item in troubleList"
                  :key="item.key"
                  :label="item.code"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">设备</label>
            <div class="inp">
              <el-cascader class="cascader_width"
                filterable clearable
                :props="defaultParams"
                :show-all-levels="false"
                :options="eqpList"
                v-model="eqpSelected">
              </el-cascader>
            </div>
          </div>
          <div class="input-group">
            <label for="">维修类型</label>
            <div class="inp">
              <el-select v-model="pmType" filterable placeholder="请选择维修类型">
                <el-option label="中修" value="40"/>
                <el-option label="大修" value="41"/>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">更换类型</label>
            <div class="inp">
              <el-select v-model="replaceType" filterable placeholder="请选择更换类型">
                <el-option label="无更换" value="0"/>
                <el-option label="部分更换" value="157"/>
                <el-option label="整件更换" value="205"/>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="name">过程描述</label>
            <div class="inp">
              <el-input v-model.trim="desc" placeholder="请输入过程描述"></el-input>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="add"><x-button :disabled="btn.save">添加</x-button></li>
        <!-- 删除和履历健康度都有关联，暂时取消此功能；若需要，则需要修改后台代码，权限表中插入168 -->
        <li class="list" @click="remove" v-show="false"><x-button :disabled="btn.delete">删除</x-button></li>
        <li class="list" @click="edit"><x-button :disabled="btn.update">修改</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox" v-model="bCheckAll" @change="checkAll"></li>
        <li class="list name c-pointer" @click="changeOrder('eqp')">
          设备编码
          <i :class="[{ 'el-icon-d-caret': headOrder.eqp === 0 }, { 'el-icon-caret-top': headOrder.eqp === 1 }, { 'el-icon-caret-bottom': headOrder.eqp === 2 }]"></i>
        </li>
        <li class="list name">设备名称</li>
        <li class="list name c-pointer" @click="changeOrder('pm_type')">
          维护类型
          <i :class="[{ 'el-icon-d-caret': headOrder.pm_type === 0 }, { 'el-icon-caret-top': headOrder.pm_type === 1 }, { 'el-icon-caret-bottom': headOrder.pm_type === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('replace_type')">
          更换类型
          <i :class="[{ 'el-icon-d-caret': headOrder.replace_type === 0 }, { 'el-icon-caret-top': headOrder.replace_type === 1 }, { 'el-icon-caret-bottom': headOrder.replace_type === 2 }]"></i>
        </li>
        <li class="list desc">过程描述</li>
        <li class="list name c-pointer" @click="changeOrder('pm_date')">
          维修日期
          <i :class="[{ 'el-icon-d-caret': headOrder.pm_date === 0 }, { 'el-icon-caret-top': headOrder.pm_date === 1 }, { 'el-icon-caret-bottom': headOrder.pm_date === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('charge')">
          维修负责人
          <i :class="[{ 'el-icon-d-caret': headOrder.charge === 0 }, { 'el-icon-caret-top': headOrder.charge === 1 }, { 'el-icon-caret-bottom': headOrder.charge === 2 }]"></i>
        </li>
        <li class="list upload-cascader">附件</li>
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
            <li class="list" v-for="item in eqpRepairList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="editEqpRepairID" :value="item.id" @change="emitEditID">
                </div>
                <div class="name">{{ item.eqpCode }}</div>
                <div class="name">{{ item.eqpName }}</div>
                <div class="name word-break">{{ item.pmTypeName }}</div>
                <div class="name word-break">{{ item.replaceTypeName }}</div>
                <div class="desc color-white word-break">{{ item.desc }}</div>
                <div class="name color-white word-break">{{ item.pmDate }}</div>
                <div class="name color-white word-break">{{ item.chargeName }}</div>
                <div class="upload-cascader">
                  <el-cascader clearable
                    v-show="item.uploadFileArr.length != 0"
                    @change="preview"
                    :show-all-levels="false"
                    :options="item.uploadFileArr">
                  </el-cascader>
                </div>
                <div class="last-update-time color-white word-break">{{ item.updatedTime }}</div>
                <div class="name color-white word-break">{{ item.updatedName }}</div>
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
// import { dictionary } from '@/common/js/dictionary.js'
import { isPreview } from '@/common/js/UpDownloadFileHelper.js'
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
export default {
  name: 'SeeEqpRepairList',
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
        label: 'name',
        value: 'id',
        children: 'children'
      },
      title: ' | 维修过程记录',
      trouble: '',
      troubleList: [],
      replaceType: '',
      pmType: '',
      eqpSelected: [],
      eqpList: [],
      desc: '',
      editEqpRepairID: [],
      eqpRepairList: [],
      uploadFile: [],
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
        eqp: 0,
        pm_type: 0,
        replace_type: 0,
        pm_date: 0,
        charge: 0,
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
        return item.actionID === btn.eqpRepairHistory.save
      })
      this.btn.delete = !actions.some((item, index) => {
        return item.actionID === btn.eqpRepairHistory.delete
      })
      this.btn.update = !actions.some((item, index) => {
        return item.actionID === btn.eqpRepairHistory.update
      })
    }
    // api.getAllTroubleReport().then(res => {
    //   this.troubleList = res.data
    // }).catch(err => console.log(err))
    // 设备加载
    api.GetEqpByTypeAndLine(0).then(res => {
      this.eqpList = res.data
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
      this.searchResult(1)
    },
    preview (val) {
      let id = val[val.length - 1]
      if (isPreview(id, this.uploadFile[id].label)) {
        // let arr = this.uploadFile[id].label.split('.')
        // if (arr[arr.length - 1] === 'pdf') {
        //   this.centerDialogVisible = true
        //   this.previewUrl = PDF_UPLOADED_VIEW_URL + this.uploadFile[id].url
        //   this.isVedio = false
        // } else {
        //   this.previewUrl = FILE_SERVER_PATH + this.uploadFile[id].url
        //   this.isVedio = true
        //   this.centerDialogVisible = true
        // }
      }
      // 'http://10.89.36.103:8090' + '/Compoment/pdfViewer/web/viewer.html?file=/' + item
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.eqp = 0
        this.headOrder.pm_type = 0
        this.headOrder.pm_date = 0
        this.headOrder.charge = 0
        this.headOrder.replace_type = 0
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
      let eqp
      let len = this.eqpSelected.length
      if (len !== 0) {
        if (len < 3) {
          this.$message({
            message: '验证失败，请查看提示信息',
            type: 'error'
          })
          this.eqpSelected.tips = '此项必选设备'
          return
        } else {
          eqp = this.eqpSelected[len - 1]
        }
      }
      this.currentPage = page
      this.loading = true
      api.getEqpRepair({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        Desc: this.desc,
        PMType: this.pmType,
        ReplaceType: this.replaceType,
        Eqp: eqp
      }).then(res => {
        this.loading = false
        if (res.data.total === 0) {
          this.eqpRepairList = []
        } else {
          res.data.rows.map(item => {
            item.updatedTime = transformDate(item.updatedTime)
            item.pmDate = item.pmDate.slice(0, 10)
            item.replaceTypeName = item.replaceType === 0 ? '无变更' : item.replaceTypeName
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
          this.eqpRepairList = res.data.rows
        }
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    add () {
      // 判断权限，符合则允许跳转
      this.$router.push({name: 'AddEqpRepair', query: { type: 'Add' }})
    },
    // 修改维修过程记录
    edit () {
      if (!this.editEqpRepairID.length) {
        this.$message({
          message: '请选择修改操作的维修过程记录',
          type: 'warning'
        })
      } else if (this.editEqpRepairID.length > 1) {
        this.$message({
          message: '修改的维修过程记录不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddEqpRepair',
          query: {
            id: this.editEqpRepairID[0],
            type: 'edit'
          }
        })
      }
    },

    // 删除维修过程记录
    remove () {
      if (!this.editEqpRepairID.length) {
        this.$message({
          message: '请选择需删除作的维修过程记录',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条维修过程记录信息?'
      }
    },

    // 搜索功能
    searchRes () {
      this.$emit('title', '| 维修过程记录定义')
      this.loading = true
      this.init()
      // this.searchResult(1)
    },

    // 弹框确认是否删除
    dialogEnter () {
      api.delEqpRepair(this.editEqpRepairID.join(',')).then(res => {
        if (res.code === 0) {
          this.editEqpRepairID = []
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

    // 获取修改维修过程记录id
    emitEditID () {
      this.$emit('editEqpRepairID', this.editEqpRepairID)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.eqpRepairList.map(val => this.editEqpRepairID.push(val.id)) : this.editEqpRepairID = []
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
    width: 12%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 10%;
  }

  .upload-cascader{
    width: 13%;
  }

  .desc{
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
