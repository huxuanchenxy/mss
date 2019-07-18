<template>
  <div class="wrap height-full"
       v-loading="loading"
       element-loading-text="加载中"
       element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive"
             alt=""
             class="icon"> {{ $router.navList[$route.matched[0].path].name }}
        {{ title }}
      </h2>
    </div>
    <div class="box">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
          <div class="input-group">
            <label for="">设备名称</label>
            <div class="inp">
              <!-- <el-select v-model="deviceType" clearable placeholder="请选择">
                <option disabled value="" selected>请选择</option>
                 <el-option
                 v-for="item in deviceTypeList"
                 :key="item.key"
                 :value="item.id"
                 :label="item.deviceTypeName">
                 </el-option>
              </el-select> -->
              <el-cascader clearable
                           :show-all-levels="true"
                           :options="deviceTypeList"
                           v-model="deviceType">
              </el-cascader>
            </div>
          </div>
          <!-- <div class="input-group">
            <label for="">设备名称</label>
            <div class="inp">
              <el-select v-model="deviceName" clearable placeholder="请选择">
                <option disabled value="" selected>请选择</option>
                 <el-option
                 v-for="item in devicelist"
                 :key="item.key"
                 :value="item.id"
                 :label="item.deviceName">
                 </el-option>
              </el-select>
            </div>
          </div> -->
          <div class="input-group">
            <label for="">负责班组</label>
            <div class="inp">
              <el-select v-model="teamGroupid"
                         clearable
                         placeholder="请选择">
                <option disabled
                        value=""
                        selected>请选择</option>
                <el-option v-for="item in TeamGroupList"
                           :key="item.key"
                           :value="item.id"
                           :label="item.teamGroupName">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">维护负责人</label>
            <div class="inp">
              <el-select v-model="directorid"
                         clearable
                         placeholder="请选择">
                <option disabled
                        value=""
                        selected>请选择</option>
                <el-option v-for="item in directorList"
                           :key="item.key"
                           :value="item.id"
                           :label="item.directorName">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">维护日期</label>
            <div class="inp">
              <el-date-picker v-model="maintain_date"
                              type="date"
                              prefix-icon="el-icon-date"
                              :unlink-panels="true"
                              placeholder="选择日期"
                              value-format="yyyy-MM-dd">
              </el-date-picker>
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
            <router-link :to="{ name: 'addDeviceMaintain', params: { mark: 'add' } }">添加</router-link>
          </x-button>
        </li>
        <li class="list"
            @click="remove">
          <x-button>删除</x-button>
        </li>
        <li class="list"
            @click="edit">
          <x-button>修改</x-button>
        </li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox"
                 v-model="bCheckAll"
                 @change="checkAll"></li>
        <li class="list number c-pointer"
            @click="changeOrder('id')">
          序号
          <i :class="[{ 'el-icon-d-caret': headOrder.id === 0 }, { 'el-icon-caret-top': headOrder.id === 1 }, { 'el-icon-caret-bottom': headOrder.id === 2 }]"></i>
        </li>
        <!-- <li class="list name c-pointer"
            @click="changeOrder('device_type_id')">
          设备类别
          <i :class="[{ 'el-icon-d-caret': headOrder.device_type_id === 0 }, { 'el-icon-caret-top': headOrder.device_type_id === 1 }, { 'el-icon-caret-bottom': headOrder.device_type_id === 2 }]"></i>
        </li> -->
        <li class="list number c-pointer"
            @click="changeOrder('device_name')">
          设备名称
          <i :class="[{ 'el-icon-d-caret': headOrder.device_id === 0 }, { 'el-icon-caret-top': headOrder.device_id === 1 }, { 'el-icon-caret-bottom': headOrder.device_id === 2 }]"></i>
        </li>
        <li class="list number c-pointer"
            @click="changeOrder('team_group_id')">
          维护班组
          <i :class="[{ 'el-icon-d-caret': headOrder.team_group_id === 0 }, { 'el-icon-caret-top': headOrder.team_group_id === 1 }, { 'el-icon-caret-bottom': headOrder.team_group_id === 2 }]"></i>
        </li>
        <li class="list number c-pointer"
            @click="changeOrder('director_id')">
          维护负责人
          <i :class="[{ 'el-icon-d-caret': headOrder.director_id === 0 }, { 'el-icon-caret-top': headOrder.director_id === 1 }, { 'el-icon-caret-bottom': headOrder.director_id === 2 }]"></i>
        </li>
        <li class="list number c-pointer"
            @click="changeOrder('maintain_date')">
          维护日期
          <i :class="[{ 'el-icon-d-caret': headOrder.maintain_date === 0 }, { 'el-icon-caret-top': headOrder.maintain_date === 1 }, { 'el-icon-caret-bottom': headOrder.maintain_date === 2 }]"></i>
        </li>
        <!-- <li class="list number c-pointer"
            @click="changeOrder('detail_desc')">
          过程记录
          <i :class="[{ 'el-icon-d-caret': headOrder.detail_desc === 0 }, { 'el-icon-caret-top': headOrder.detail_desc === 1 }, { 'el-icon-caret-bottom': headOrder.detail_desc === 2 }]"></i>
        </li> -->
        <li class="list number c-pointer"
            @click="changeOrder('attch_file')">
          附件上传
          <i :class="[{ 'el-icon-d-caret': headOrder.attch_file === 0 }, { 'el-icon-caret-top': headOrder.attch_file === 1 }, { 'el-icon-caret-bottom': headOrder.attch_file === 2 }]"></i>
        </li>
        <li class="list last-update-time c-pointer"
            @click="changeOrder('updated_time')">
          最后更新时间
          <i :class="[{ 'el-icon-d-caret': headOrder.updatedTime === 0 }, { 'el-icon-caret-top': headOrder.updatedTime === 1 }, { 'el-icon-caret-bottom': headOrder.updatedTime === 2 }]"></i>
        </li>
        <li class="list last-maintainer c-pointer"
            @click="changeOrder('Updated_By')">
          最后更新人
          <i :class="[{ 'el-icon-d-caret': headOrder.updatedBy === 0 }, { 'el-icon-caret-top': headOrder.updatedBy === 1 }, { 'el-icon-caret-bottom': headOrder.updatedBy === 2 }]"></i>
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list"
                v-for="(item,index) in DeviceMaintainRegList"
                :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox"
                         v-model="editDeviceMaintainRegIDList"
                         :value="item.id"
                         @change="emitEditID">
                </div>
                <div class="number">{{index+1}}</div>
                <!-- <div class="number">{{ item.device_type_name }}</div> -->
                <div class="name">{{ item.device_name }}</div>
                <div class="number">{{ item.team_group_name }}</div>
                <div class="number">{{ item.director_name }}</div>
                <div class="number">{{ item.maintain_date }}</div>
                <!-- <div class="number">{{ item.detail_desc }}</div> -->
                <!-- <div class="number">{{ item.arr }}</div> -->
                <div class="upload-cascader">
                  <el-cascader clearable
                             @change="preview"
                             :show-all-levels="false"
                             :options="item.arr">
                </el-cascader>
                </div>
                <div class="last-update-time color-white">{{ item.updatedTime }}</div>
                <div class="last-maintainer">{{ '管理员' }}</div>
              </div>
            </li>
          </ul>
          <!-- 分页 -->
          <el-pagination :current-page.sync="currentPage"
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
    <el-dialog :visible.sync="dialogVisible.isShow"
               :modal-append-to-body="false"
               :show-close="false">
      {{ dialogVisible.text }}
      <template slot="footer"
                class="dialog-footer">
        <template v-if="dialogVisible.btn">
          <el-button @click="dialogVisible.isShow = false">否</el-button>
          <el-button @click="dialogEnter">是</el-button>
        </template>
        <el-button v-else
                   @click="dialogVisible.isShow = false"
                   :class="{ on: !dialogVisible.btn }">知道了</el-button>
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
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
import eqpApi from '@/api/eqpApi.js'
export default {
  name: 'DeviceMaintainList',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 设备维修',
      maintain_date: '',
      deviceName: '',
      devicelist: [],
      DeviceMaintainRegID: '',
      teamGroupid: '',
      TeamGroupList: [],
      directorid: '',
      directorList: [],
      deviceType: '',
      deviceTypeList: [],
      DeviceMaintainRegList: [],
      editDeviceMaintainRegIDList: [],
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
        device_type_id: 0,
        device_name: 0,
        team_group_id: 0,
        director_id: 0,
        maintain_date: 0,
        detail_desc: 0,
        Sort: 0,
        updatedTime: 0,
        updatedBy: 0
      },
      centerDialogVisible: false,
      previewPartUrl: [],
      previewUrl: ''
    }
  },
  created () {
    // this.$emit('title', '| 设备维修')
    this.init()
    // 设备配置类型列表
    api.GetEquipmentTypeList().then(res => {
      res.data.map((e, i) => {
        if (e.children != null && e.children.length > 0) {
          this.deviceTypeList.push({'value': e.id, 'label': e.deviceTypeName, 'children': []
          })
          e.children.map((item) => {
            this.deviceTypeList[i].children.push({ 'value': item.id, 'label': item.deviceName })
          })
        } else {
          this.deviceTypeList.push({ 'value': e.id, 'label': e.deviceTypeName
          })
        }
      })
    }).catch(err => console.log(err))
    // 设备配置类型列表
    api.GetDirectorList().then(res => {
      this.directorList = res.data
    }).catch(err => console.log(err))
    // 部门列表
    api.GetTeamGroupList().then(res => {
      this.TeamGroupList = res.data
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
    preview (val) {
      this.centerDialogVisible = true
      this.previewUrl = PDF_UPLOADED_VIEW_URL + val[val.length - 1]
      // 'http://10.89.36.103:8090' + '/Compoment/pdfViewer/web/viewer.html?file=/' + item
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
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
      this.DeviceMaintainRegList = []
      this.currentPage = page
      this.loading = true
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        deviceType: '', // this.deviceType.text,
        deviceId: '', // this.deviceName,
        TeamGroup: this.teamGroupid,
        Director: this.directorid,
        maintain_date: this.maintain_date
      }
      if (this.deviceType !== '') {
        switch (this.deviceType.length) {
          case 0:
            parm.deviceType = ''
            parm.deviceId = ''
            break
          case 1:
            parm.deviceType = this.deviceType[0]
            break
          case 2:
            parm.deviceType = this.deviceType[0]
            parm.deviceId = this.deviceType[1]
            break
        }
      }
      api.GetListByPage(parm).then(res8 => {
        this.loading = false
        if (res8.code === 0) {
          res8.data.list.map(item => {
            item.updatedTime = transformDate(item.updatedTime)
            // this.InvokeOutApI(item)
            if (item.attch_file !== null && item.attch_file !== '') {
              this.InvokeOutApI(item)
            } else {
              this.DeviceMaintainRegList.push(item)
            }
          })
          this.total = res8.data.total
        }
      }).catch(err => console.log(err))
    },
    InvokeOutApI (item) {
      // this.DeviceMaintainRegList = []
      eqpApi.getUploadFileByIDs(item.attch_file).then(res => {
        if (res.code === 0) {
          let arr = []
          arr.push(this.convertDrdlist(res.data, item.file_type))
          item.arr = arr // this.data3
          this.DeviceMaintainRegList.push(item)
        }
      })
    },
    convertDrdlist (val, val1) {
      let list = {
        value: '',
        label: '',
        children: []
      }
      switch (val1) {
        case '7': // 视频
          list.label = '视频资料'
          list.value = 7
          list.children = this.convertName(val)
          break
        case '6': // 附件
          list.label = '附件资料'
          list.value = 6
          list.children = this.convertName(val)
          break
      }
      return list
    },
    convertName (val) {
      let children = []
      val.map((item) => {
        children.push({ 'value': item.url, 'label': item.name })
      })
      return children
    },
    // 修改站区
    edit () {
      if (!this.editDeviceMaintainRegIDList.length) {
        this.$message({
          message: '请选择需要修改的设备维护记录',
          type: 'warning'
        })
      } else if (this.editDeviceMaintainRegIDList.length > 1) {
        this.$message({
          message: '修改的设备维护记录不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'addDeviceMaintain',
          params: {
            id: this.editDeviceMaintainRegIDList[0],
            mark: 'edit'
          }
        })
      }
    },

    // 删除站区
    remove () {
      if (!this.editDeviceMaintainRegIDList.length) {
        this.$message({
          message: '请选择需要删除的资料',
          type: 'warning'
        })
      } else {
        this.dialogVisible.isShow = true
        this.dialogVisible.btn = true
        this.dialogVisible.text = '确定删除该条资料信息?'
      }
    },
    // 搜索功能
    searchRes () {
      this.$emit('title', '| 设备维修')
      this.loading = true
      this.init()
      this.searchResult(1)
    },
    // 弹框确认是否删除
    dialogEnter () {
      api.DeleteList(this.editDeviceMaintainRegIDList.join(',')).then(res => {
        if (res.code === 0) {
          this.editDeviceMaintainRegIDList = []
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
    // 获取修改站区id
    emitEditID () {
      this.$emit('editDeviceMaintainRegIDList', this.editDeviceMaintainRegIDList)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.DeviceMaintainRegList.map(val => this.editDeviceMaintainRegIDList.push(val.id)) : this.editDeviceMaintainRegIDList = []
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
    width: 4%;
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
