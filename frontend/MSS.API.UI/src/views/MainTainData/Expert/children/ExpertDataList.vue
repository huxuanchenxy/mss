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
            <label for="name">关键字</label>
            <div class="inp">
              <el-input v-model.trim="keyword"
                        placeholder="请输入关键字"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">主题</label>
            <div class="inp">
              <el-input v-model.trim="ExpertTitle"
                        placeholder="请输入主题名称"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="">设备类型</label>
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
              <el-select v-model="deviceType"
                         clearable
                         filterable
                         placeholder="请选择">
                <el-option v-for="item in deviceTypeList"
                           :key="item.key"
                           :label="item.tName"
                           :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">部门</label>
            <div class="inp">
              <!-- <el-select v-model="deptid" clearable placeholder="请选择">
                <option disabled value="" selected>请选择</option>
                 <el-option
                 v-for="item in deptList"
                 :key="item.key"
                 :value="item.id"
                 :label="item.deptName">
                 </el-option>
              </el-select> -->
              <el-cascader class="cascader_width"
                           clearable
                           :props="defaultParams"
                           change-on-select
                           @change="cascader_change_copy"
                           :show-all-levels="true"
                           :options="teamList"
                           v-model="teamPath.text">
              </el-cascader>
            </div>
          </div>
        </div>
        <div class="search-btn"
             @click="searchRes">
          <x-button><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list">
          <x-button>
            <router-link :to="{ name: 'AddExpertData', params: { mark: 'add' } }">添加</router-link>
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
        <li class="list"
            @click="detail">
          <x-button>查看明细</x-button>
        </li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list"><input type="checkbox"
                 v-model="bCheckAll"
                 @change="checkAll"></li>
        <li class="list indexkey c-pointer"
            @click="changeOrder('id')">
          序号
          <i :class="[{ 'el-icon-d-caret': headOrder.id === 0 }, { 'el-icon-caret-top': headOrder.id === 1 }, { 'el-icon-caret-bottom': headOrder.id === 2 }]"></i>
        </li>
        <li class="list name c-pointer"
            @click="changeOrder('keyword')">
          关键字
          <i :class="[{ 'el-icon-d-caret': headOrder.keyword === 0 }, { 'el-icon-caret-top': headOrder.keyword === 1 }, { 'el-icon-caret-bottom': headOrder.keyword === 2 }]"></i>
        </li>
        <li class="list title c-pointer"
            @click="changeOrder('title')">
          主题
          <i :class="[{ 'el-icon-d-caret': headOrder.title === 0 }, { 'el-icon-caret-top': headOrder.title === 1 }, { 'el-icon-caret-bottom': headOrder.title === 2 }]"></i>
        </li>
        <!-- <li class="list number c-pointer" @click="changeOrder('content')">
          内容
          <i :class="[{ 'el-icon-d-caret': headOrder.content === 0 }, { 'el-icon-caret-top': headOrder.content === 1 }, { 'el-icon-caret-bottom': headOrder.content === 2 }]"></i>
        </li> -->
        <li class="list number c-pointer"
            @click="changeOrder('device_type')">
          设备类型
          <i :class="[{ 'el-icon-d-caret': headOrder.device_type === 0 }, { 'el-icon-caret-top': headOrder.device_type === 1 }, { 'el-icon-caret-bottom': headOrder.device_type === 2 }]"></i>
        </li>
        <li class="list number c-pointer"
            @click="changeOrder('deptid')">
          录入部门
          <i :class="[{ 'el-icon-d-caret': headOrder.deptid === 0 }, { 'el-icon-caret-top': headOrder.deptid === 1 }, { 'el-icon-caret-bottom': headOrder.deptid === 2 }]"></i>
        </li>
        <!-- <li class="list number c-pointer" @click="changeOrder('video_file')">
          视频
          <i :class="[{ 'el-icon-d-caret': headOrder.video_file === 0 }, { 'el-icon-caret-top': headOrder.video_file === 1 }, { 'el-icon-caret-bottom': headOrder.video_file === 2 }]"></i>
        </li> -->
        <li class="list upload-cascader c-pointer"
            @click="changeOrder('attch_file')">
          附件
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
                v-for="(item,index) in ExpertdataList"
                :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox"
                         v-model="editExpertIDList"
                         :value="item.id"
                         @change="emitEditID">
                </div>
                <div class="indexkey">{{index+1}}</div>
                <div class="number">{{ item.keyword }}</div>
                <div class="title">{{ item.title }}</div>
                <div class="number">{{ item.deviceTypeName }}</div>
                <div class="number">{{ item.deptname }}</div>
                <div class="upload-cascader">
                  <el-cascader clearable
                               @change="preview"
                               :show-all-levels="false"
                               :options="item.uploadFileArr">
                  </el-cascader>
                </div>
                <div class="last-update-time color-white">{{ item.updatedTime }}</div>
                <div class="last-maintainer">{{ item.updated_name }}</div>
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
    <el-dialog :visible.sync="centerDialogVisible"
               :modal-append-to-body="false"
               custom-class="show-list-wrap"
               center>
      <iframe v-if="previewUrl !='' && videoFlag == false"
              :src="previewUrl"
              width="100%"
              height="100%"
              frameborder="0"></iframe>
    </el-dialog>
    <el-dialog title
               :visible.sync="vediocenterDialogVisible"
               width="30%"
               :modal-append-to-body="false"
               @close="closeDialog">
      <video :src="previewUrl"
             controls
             autoplay
             class="video"
             :ref="dialogVideo"
             width="100%"></video>
    </el-dialog>
  </div>
</template>
<script>
import { transformDate, FILE_SERVER_PATH, PDF_UPLOADED_VIEW_URL } from '@/common/js/utils.js'
import { isPreview } from '@/common/js/UpDownloadFileHelper.js'
import XButton from '@/components/button'
import eqpApi from '@/api/eqpApi.js'
import apiOrg from '@/api/orgApi'
import api from '@/api/ExpertApi'
export default {
  name: 'ExpertDataList',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 专家库',
      ExpertTitle: '',
      ExpertID: '',
      deptid: '',
      deptList: [], // 部门
      deviceType: '',
      deviceTypeList: [],
      ExpertdataList: [],
      editExpertIDList: [],
      nodeType: '',
      uploadFile: {},
      team: '',
      teamPath: {
        text: [],
        tips: ''
      },
      teamList: [],
      assetNo: {
        text: '',
        tips: ''
      },
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      bCheckAll: false,
      videoFlag: false,
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
        keyword: 1,
        title: 0,
        content: 0,
        device_type: 0,
        deptid: 0,
        video_file: 0,
        attch_file: 0,
        Sort: 0,
        updatedTime: 0,
        updatedBy: 0
      },
      centerDialogVisible: false,
      vediocenterDialogVisible: false,
      previewPartUrl: [],
      previewUrl: ''
    }
  },
  created () {
    this.$emit('title', '| 专家库配置')
    this.init()
    // 设备类型列表
    eqpApi.getEqpTypeAll().then(res => {
      this.deviceTypeList = res.data
    }).catch(err => console.log(err))
    // 部门列表
    apiOrg.getOrgAll().then(res => {
      this.teamList = res.data
    }).catch(err => console.log(err))
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  mounted () {
    this.$refs.btn.onclik(function () {
      if (this.$refs.dialogVideo.paused) {
        this.$refs.dialogVideo.play()
      } else {
        this.$refs.dialogVideo.pause()
      }
    })
  },
  methods: {
    init () {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = 1
      // this.searchResult(1)
    },
    preview (val) {
      if (val.length < 1) {
        return
      }
      let id = val[val.length - 1]
      if (isPreview(id, this.uploadFile[id].label)) {
        let arr = this.uploadFile[id].label.split('.')
        if (arr[arr.length - 1] === 'pdf') {
          this.centerDialogVisible = true
          this.previewUrl = PDF_UPLOADED_VIEW_URL + this.uploadFile[id].url
          this.videoFlag = false
          this.centerDialogVisible = true
          this.vediocenterDialogVisible = false
        } else {
          this.previewUrl = FILE_SERVER_PATH + this.uploadFile[id].url
          this.videoFlag = false
          this.centerDialogVisible = true
          this.vediocenterDialogVisible = false
        }
      }
    },
    // 班组下拉选中，过滤非班组
    cascader_change_copy (val) {
      var arr = this.teamPath.text
      let id = arr[arr.length - 1]
      this.deptid = id
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
      this.ExpertdataList = []
      this.currentPage = page
      this.loading = true
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        keyword: this.keyword,
        title: this.ExpertTitle,
        deptid: this.deptid,
        deviceType: this.deviceType
      }
      api.GetListByPage(parm).then(res => {
        this.loading = false
        if (res.code === 0) {
          res.data.list.map(item => {
            item.updatedTime = transformDate(item.updatedTime)
            if (item.uploadFiles !== null && item.uploadFiles !== '[]') {
              item.uploadFileArr = JSON.parse(item.uploadFiles)
              item.uploadFileArr.map(val => {
                val.children.map(item => {
                  this.uploadFile[item.value] = item
                })
              })
            } else {
              item.uploadFileArr = null // []
            }
            this.ExpertdataList = res.data.list
          })
          this.total = res.data.total
        }
      }).catch(err => console.log(err))
    },
    // 修改站区
    edit () {
      if (!this.editExpertIDList.length) {
        this.$message({
          message: '请选择需要修改的专家库资料',
          type: 'warning'
        })
      } else if (this.editExpertIDList.length > 1) {
        this.$message({
          message: '修改的专家库资料不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'AddExpertData',
          params: {
            id: this.editExpertIDList[0],
            mark: 'edit'
          }
        })
      }
    },
    // 查看设备明细
    detail () {
      if (!this.editExpertIDList.length) {
        this.$message({
          message: '请选择需要查看的专家资料',
          type: 'warning'
        })
      } else if (this.editExpertIDList.length > 1) {
        this.$message({
          message: '查看的专家资料不能超过1个',
          type: 'warning'
        })
      } else {
        this.$router.push({
          name: 'DetailExpertData',
          params: {
            id: this.editExpertIDList[0]
          }
        })
      }
    },
    // 删除站区
    remove () {
      if (!this.editExpertIDList.length) {
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
    // 弹框确认是否删除
    dialogEnter () {
      api.DeleteList(this.editExpertIDList.join(',')).then(res => {
        if (res.code === 0) {
          this.editExpertIDList = []
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
      this.$emit('title', '| 专家库')
      this.loading = true
      this.init()
      this.searchResult(1)
    },

    // 获取修改站区id
    emitEditID () {
      this.$emit('editExpertIDList', this.editExpertIDList)
    },

    // 全选
    checkAll () {
      this.bCheckAll ? this.ExpertdataList.map(val => this.editExpertIDList.push(val.id)) : this.editExpertIDList = []
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
.content-wrap {
  overflow: hidden;
  height: percent($con-height, $content-height);
  text-align: center;
  .content-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: percent(50, $con-height);
    padding: 0 PXtoEm(24);
    background: rgba(36, 128, 198, 0.5);

    .last-update-time {
      color: $color-white;
    }
  }

  .scroll {
    height: percent($con-height - 50, $con-height);
  }

  .list-wrap {
    .list {
      &:nth-of-type(even) {
        .list-content {
          background: rgba(186, 186, 186, 0.5);
        }
      }
    }

    .list-content {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: PXtoEm(15) PXtoEm(24);

      div {
        word-break: break-all;
      }
    }

    .left-title {
      margin-right: 10px;
      font-weight: bold;
    }

    // 隐藏内容
    .sub-content {
      overflow: hidden;
      height: 0;
      font-size: $font-size-small;
      text-align: left;
      color: $color-content-text;

      &.active {
        overflow: inherit;
        height: auto;
        transition: 0.7s 0.2s;
      }
    }

    .sub-con-list {
      display: flex;
      padding: PXtoEm(15) PXtoEm(24);
      border-top: 1px solid $color-main-background;
      background: rgba(0, 0, 0, 0.2);

      .right-wrap {
        display: flex;
        flex-wrap: wrap;
      }

      .list {
        margin-right: 10px;
      }
    }
  }

  .number,
  .name,
  .btn-wrap {
    width: 10%;
  }
  .indexkey {
    width: 5%;
  }
  .title {
    width: 15%;
  }
  .name {
    a {
      color: #42abfd;
    }
  }
  .upload-cascader {
    width: 15%;
  }
  .last-update-time {
    width: 18%;
    color: $color-content-text;
  }

  .last-maintainer {
    width: 10%;
  }

  .state {
    width: 5%;
  }
}
</style>
