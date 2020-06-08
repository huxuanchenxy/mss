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
            <label for="name">模板名称</label>
            <div class="inp">
              <el-input v-model.trim="moduleName" placeholder="请输入模板名称"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">上传文件名</label>
            <div class="inp">
              <el-input v-model.trim="fileName" placeholder="请输入上传文件名"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">设备设施</label>
            <div class="inp">
              <el-input v-model.trim="deviceName" placeholder="请输入设备设施"></el-input>
            </div>
          </div>
          <div class="input-group">
            <label for="name">关键字</label>
            <div class="inp">
              <el-input v-model.trim="keyWord" placeholder="请输入关键字"></el-input>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="add"><x-button :disabled="btn.save">导入</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list number c-pointer" @click="changeOrder('code')">
          模板编码
          <i :class="[{ 'el-icon-d-caret': headOrder.code === 0 }, { 'el-icon-caret-top': headOrder.code === 1 }, { 'el-icon-caret-bottom': headOrder.code === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('name')">
          模板名称
          <i :class="[{ 'el-icon-d-caret': headOrder.name === 0 }, { 'el-icon-caret-top': headOrder.name === 1 }, { 'el-icon-caret-bottom': headOrder.name === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('file_name')">
          上传文件名
          <i :class="[{ 'el-icon-d-caret': headOrder.file_name === 0 }, { 'el-icon-caret-top': headOrder.file_name === 1 }, { 'el-icon-caret-bottom': headOrder.file_name === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('device_name')">
          设备设施
          <i :class="[{ 'el-icon-d-caret': headOrder.device_name === 0 }, { 'el-icon-caret-top': headOrder.device_name === 1 }, { 'el-icon-caret-bottom': headOrder.device_name === 2 }]"></i>
        </li>
        <li class="list number">关键字</li>
        <li class="list number">上传部门</li>
        <li class="list number c-pointer" @click="changeOrder('death_year')">
          有效年限
          <i :class="[{ 'el-icon-d-caret': headOrder.death_year === 0 }, { 'el-icon-caret-top': headOrder.death_year === 1 }, { 'el-icon-caret-bottom': headOrder.death_year === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('level')">
          受控级别
          <i :class="[{ 'el-icon-d-caret': headOrder.level === 0 }, { 'el-icon-caret-top': headOrder.level === 1 }, { 'el-icon-caret-bottom': headOrder.level === 2 }]"></i>
        </li>
        <li class="list last-update-time c-pointer" @click="changeOrder('created_time')">
          创建时间
          <i :class="[{ 'el-icon-d-caret': headOrder.created_time === 0 }, { 'el-icon-caret-top': headOrder.created_time === 1 }, { 'el-icon-caret-bottom': headOrder.created_time === 2 }]"></i>
        </li>
        <li class="list last-maintainer c-pointer" @click="changeOrder('created_by')">
          创建人
          <i :class="[{ 'el-icon-d-caret': headOrder.created_by === 0 }, { 'el-icon-caret-top': headOrder.created_by === 1 }, { 'el-icon-caret-bottom': headOrder.created_by === 2 }]"></i>
        </li>
        <li class="name">操作</li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in mList" :key="item.key">
              <div class="list-content">
                <div class="name word-break">{{ item.code }}</div>
                <div class="name word-break">{{ item.name }}</div>
                <div class="name word-break">{{ item.fileName }}</div>
                <div class="name word-break">{{ item.deviceName }}</div>
                <div class="name word-break">{{ item.keyWord }}</div>
                <div class="name word-break">{{ item.department }}</div>
                <div class="name word-break">{{ item.deathYear }}</div>
                <div class="name word-break">{{ item.levelName }}</div>
                <div class="last-update-time color-white">{{ item.createdTime }}</div>
                <div class="name">{{ item.createdByName }}</div>
                <div class="name">
                  <li @click="detail(item.id)" ><x-button >查看明细</x-button></li>
                </div>
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
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import { btn } from '@/element/btn.js'
import XButton from '@/components/button'
import api from '@/api/workflowApi'
export default {
  name: 'MaintenanceList',
  components: {
    XButton
  },
  data () {
    return {
      btn: {
        import: false
      },
      title: ' | 检修单模板',
      moduleName: '',
      fileName: '',
      deviceName: '',
      keyWord: '',
      mList: [],
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'code',
        order: 'asc'
      },
      headOrder: {
        code: 1,
        name: 0,
        file_name: 0,
        device_name: 0,
        death_year: 0,
        level: 0,
        created_time: 0,
        created_by: 0
      }
    }
  },
  created () {
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    if (!user.is_super) {
      let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
      this.btn.import = !actions.some((item, index) => {
        return item.actionID === btn.pmModule.import
      })
    }
    this.init()
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  methods: {
    init () {
      this.currentPage = 1
      // this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.code = 0
        this.headOrder.name = 0
        this.headOrder.file_name = 0
        this.headOrder.device_name = 0
        this.headOrder.death_year = 0
        this.headOrder.level = 0
        this.headOrder.created_by = 0
        this.headOrder.created_time = 0
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
      this.loading = true
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        moduleName: this.moduleName,
        fileName: this.fileName,
        deviceName: this.deviceName,
        keyWord: this.keyWord
      }
      api.getModuleList(parm).then(res => {
        this.loading = false
        if (res.data.total > 0) {
          res.data.rows.map(item => {
            item.createdTime = transformDate(item.createdTime)
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
        name: 'ImportPM'
      })
    },
    detail (id) {
      this.$router.push({
        name: 'DetailMaintenanceList',
        params: {
          id: id,
          sourceName: 'MaintenanceList'
        }
      })
    },
    // 搜索功能
    searchRes () {
      this.loading = true
      this.init()
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
