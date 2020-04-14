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
    <div class="box1">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
          <div class="input-group">
            <label for="name">年份</label>
            <div class="inp">
              <el-date-picker
                v-model="year"
                type="year"
                value-format="yyyy"
                placeholder="选择年">
              </el-date-picker>
            </div>
          </div>
          <div class="input-group">
            <label for="name">线路</label>
            <div class="inp">
              <el-select v-model="LineID" placeholder="请选择线路">
                 <el-option
                 v-for="item in MetroLineList"
                 :key="item.key"
                 :value="item.id"
                 :label="item.lineName">
                 </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="name">公司</label>
            <div class="inp">
              <el-cascader clearable
                change-on-select
                :props="defaultParams"
                @change="company_change"
                :show-all-levels="true"
                :options="companyList"
                v-model="companyPath">
              </el-cascader>
            </div>
          </div>
          <div class="input-group">
            <label for="name">部门</label>
            <div class="inp">
              <el-cascader clearable
                change-on-select
                :props="defaultParams"
                @change="dept_change"
                :show-all-levels="true"
                :options="companyList"
                v-model="deptPath">
              </el-cascader>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="search">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
    </div>
    <!-- 内容 -->
    <div class="height-full content-wrap">
      <ul class="content-header">
        <li class="list name">年份</li>
        <li class="list name">线路</li>
        <li class="list last-maintainer">公司</li>
        <li class="list name">部门</li>
        <li class="list name">状态</li>
        <li class="list last-update-time">生成时间</li>
        <li class="list name">生成人</li>
        <li class="list last-update-time">操作</li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="item in commontList" :key="item.key">
              <div class="list-content">
                <div class="name">{{ item.year }}</div>
                <div class="name">{{ item.lineName }}</div>
                <div class="last-maintainer">{{ item.companyName }}</div>
                <div class="name">{{ item.departmentName }}</div>
                <div class="name">{{ item.isCreatedMonth === 1 ? '已生成' : '未生成' }}</div>
                <div class="last-update-time">{{ item.createdTime }}</div>
                <div class="name">{{ item.createdName }}</div>
                <div class="last-update-time">
                  <li class="inline" @click="add(item, 1)" ><x-button :disabled="item.isCreatedMonth === 1 || btn.create">生成</x-button></li>
                  <li class="inline" @click="add(item, 2)" ><x-button :disabled="btn.update">修改</x-button></li>
                  <li class="inline" @click="add(item, 3)" ><x-button>查询</x-button></li>
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
import XButton from '@/components/button'
import { transformDate, getCascaderObj } from '@/common/js/utils.js'
import { btn } from '@/element/btn.js'
import api from '@/api/workflowApi'
import lineApi from '@/api/metroLineApi'
import apiOrg from '@/api/orgApi'
export default {
  name: 'CommonList',
  components: {
    XButton
  },
  data () {
    return {
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      title: ' | 月计划生成',
      year: '',
      btn: {
        create: false,
        update: false
      },
      commontList: [],
      total: 0,
      currentPage: 1,
      loading: false,
      companyList: [],
      companyPath: [],
      deptPath: [],
      company: '',
      dept: '',
      currentSort: {
        sort: 'id',
        order: 'asc'
      },
      headOrder: {
        stock_no: 0,
        trouble_no: 0,
        in_stock_no: 0,
        inspection_no: 0,
        repair_no: 0,
        lent_no: 0,
        amount: 0
      },
      MetroLineList: [],
      LineID: ''
    }
  },
  props: {
    random: Number
  },
  created () {
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    if (!user.is_super) {
      let actions = JSON.parse(window.sessionStorage.getItem('UserAction'))
      this.btn.save = !actions.some((item, index) => {
        return item.actionID === btn.monthDetail.create
      })
      this.btn.update = !actions.some((item, index) => {
        return item.actionID === btn.monthDetail.update
      })
    }
    lineApi.getAll().then(res => {
      this.MetroLineList = res.data
    }).catch(err => {
      console.log(err)
    })
    // 公司加载
    apiOrg.getOrgAll().then(res => {
      this.companyList = res.data
    }).catch(err => console.log(err))
    this.year = new Date().getFullYear() + ''
    // this.init()
  },
  activated () {
    this.init()
  },
  methods: {
    company_change (val) {
      if (val.length === 0) {
        this.company = ''
        this.companyPath = []
        return
      }
      let selectedTeam = val[val.length - 1]
      let obj = getCascaderObj(selectedTeam, this.companyList)
      if (obj.node_type === 1) {
        this.company = selectedTeam
      } else {
        this.$message({
          message: '您选择的不是公司',
          type: 'warning'
        })
      }
    },
    dept_change (val) {
      if (val.length === 0) {
        this.dept = ''
        this.deptPath = []
        return
      }
      let selectedTeam = val[val.length - 1]
      let obj = getCascaderObj(selectedTeam, this.companyList)
      if (obj.node_type === 2) {
        this.dept = selectedTeam
      } else {
        this.$message({
          message: '您选择的不是部门',
          type: 'error'
        })
      }
    },
    add (item, type) {
      this.$router.push({
        name: 'MonthList',
        params: {
          item: item,
          type: type,
          source: 'CommonList'
        }
      })
    },
    init () {
      this.currentPage = 1
      this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      // console.log(this.headOrder[sort])
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.stock_no = 0
        this.headOrder.trouble_no = 0
        this.headOrder.in_stock_no = 0
        this.headOrder.inspection_no = 0
        this.headOrder.repair_no = 0
        this.headOrder.lent_no = 0
        this.headOrder.amount = 0
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
    search () {
      this.searchResult(1)
    },
    // 搜索
    searchResult (page) {
      this.currentPage = page
      this.loading = true
      api.getCommonPage({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        Year: this.year,
        Line: this.LineID,
        Company: this.company,
        DepartmentName: this.dept
      }).then(res => {
        this.loading = false
        this.total = res.data.total
        res.data.rows.map(item => {
          if (item.createdTime !== null && item.createdTime !== '') {
            item.createdTime = transformDate(item.createdTime)
          }
        })
        this.commontList = res.data.rows
      }).catch(err => console.log(err))
    },
    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.currentPage = val
      this.searchResult(val)
    },

    // 上一页
    prevPage (val) {
      this.currentPage = val
      // this.searchResult(val)
    },

    // 下一页
    nextPage (val) {
      this.currentPage = val
      // this.searchResult(val)
    }
  }
}
</script>
<style lang="scss" scoped>
// 当前内容页面总高度
$height: $content-height - 18 + 50;
.wrap{
  height: percent($height, $content-height);
}
.box{
  height: percent(145, $height);
}

// 搜索组
.search-wrap{
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: percent(80, 145);
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
    width: PXtoEm(160)!important;
    margin-left: PXtoEm(14)!important;
    .el-date-editor{
      width: PXtoEm(160)!important;
    }
  }

  .btn{
    border: 0;
    background: $color-main-btn;
  }
}

.btn-group{
  display: flex;
  align-items: center;
  height: percent(65, 145);

  .list{
    margin-right: PXtoEm(10);
  }

  .btn{
    &:hover{
      border-color: $color-main-btn;
      background: $color-main-btn;
    }
  }
}
// 内容区
.content-wrap{
  height: percent($height - 145, $height);
  overflow: hidden;
  text-align: center;

  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: percent(50, $height - 145);
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);

    .gallery{
      p{
        margin-left: 10px;
      }
    }

    .gallery-list{
      height: 20px;
      font-size: $font-size-small;
    }
  }

  .scroll{
    height: percent($height - 50, $height);
  }

  // 所属管廊
  .gallery-list-wrap{
    display: flex;
    justify-content: space-between;
    margin-left: 10px;

    .gallery-list{
      width: 50px;
      .con{
        width: 100%;
        @extend %ellipsis;
      }
    }
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
      width: 50px;
      margin-left: 2%;
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
      }
    }

    .sub-con-list{
      display: flex;
      padding: 0 PXtoEm(24);
      border-top: 1px solid $color-main-background;
      background: rgba(0,0,0,.2);
      line-height: 40px;

      .right-wrap{
        display: -webkit-box;
        flex-wrap: wrap;
        // width: 80%;
      }

      .list{
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: PXtoEm(15) PXtoEm(24);
        margin-left: 10%;
      }
    }
  }

  .btn-wrap{
    width: 40px;
    font-size: 18px;
    text-align: left;

    .hide{
      visibility: hidden;
    }

    .icon-arrow-top-f{
      display: inline-block;
      transition: .5s;
      cursor: pointer;

      &.active{
        transform: rotateZ(180deg);
      }
    }
  }

  .number{
    width: 80px;
    word-break: break-word;
  }

  .name{
    width: 5%;
    word-break: break-word;
  }
  .day{
    width: 1%;
  }
  .last-maintainer{
    width: 250px;
    word-break: break-word;
  }

  // .name{
  //   a{
  //     color: #42abfd;
  //   }
  // }
  .last-update-time{
    width: 200px;
    color: $color-white;
  }
  .alarm{
    color: red;
  }
}

.import-btn{
  border-color: #979797!important;
  background: none!important;
  &:hover{
    border-color: $color-main-btn!important;
    background: $color-main-btn!important;
  }
}

.inline{
  display: inline;
}

/deep/
.el-year-table td .cell{
  color: $color-white!important;
}
/deep/
.el-date-picker__header-label{
  color: $color-white!important;
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
</style>
