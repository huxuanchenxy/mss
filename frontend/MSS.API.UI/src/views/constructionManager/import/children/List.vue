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
            <label for="name">年份</label>
            <div class="inp">
              <el-date-picker
                v-model="importCommon.year"
                type="year"
                value-format="yyyy"
                placeholder="选择年">
              </el-date-picker>
            </div>
          </div>
          <div class="input-group">
            <label for="name">公司</label>
            <div class="inp">
              <el-cascader clearable
                placeholder="请选择公司"
                change-on-select
                :props="defaultParams"
                @change="cascader_change"
                :show-all-levels="true"
                :options="companyList"
                v-model="companyPath">
              </el-cascader>
            </div>
          </div>
          <div class="input-group">
            <label for="name"></label>
            <div class="inp">
              <el-radio v-model="isYearTmp" label="0">月表</el-radio>
              <el-radio v-model="isYearTmp" label="1">年表</el-radio>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="search">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="add"><x-button :disabled="btn.save">导入</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="height-full content-wrap">
      <ul class="content-header">
        <li class="list name">代码</li>
        <li class="list name">设备类型</li>
        <li class="list name">所在处所</li>
        <li class="list name">管辖班组</li>
        <li class="list name">单位</li>
        <li class="list name">数量</li>
        <li class="list numberUnit" v-show="isYear==='0'">检修频次(次/月)</li>
        <li class="list numberUnit" v-show="isYear==='1'">检修频次(次/年)</li>
        <li class="list minite">工时定额(分钟)</li>
        <li class="list day">1</li>
        <li class="list day">2</li>
        <li class="list day">3</li>
        <li class="list day">4</li>
        <li class="list day">5</li>
        <li class="list day">6</li>
        <li class="list day">7</li>
        <li class="list day">8</li>
        <li class="list day">9</li>
        <li class="list day">10</li>
        <li class="list day">11</li>
        <li class="list day">12</li>
        <li class="list day" v-show="isYear==='0'">13</li>
        <li class="list day" v-show="isYear==='0'">14</li>
        <li class="list day" v-show="isYear==='0'">15</li>
        <li class="list day" v-show="isYear==='0'">16</li>
        <li class="list day" v-show="isYear==='0'">17</li>
        <li class="list day" v-show="isYear==='0'">18</li>
        <li class="list day" v-show="isYear==='0'">19</li>
        <li class="list day" v-show="isYear==='0'">20</li>
        <li class="list day" v-show="isYear==='0'">21</li>
        <li class="list day" v-show="isYear==='0'">22</li>
        <li class="list day" v-show="isYear==='0'">23</li>
        <li class="list day" v-show="isYear==='0'">24</li>
        <li class="list day" v-show="isYear==='0'">25</li>
        <li class="list day" v-show="isYear==='0'">26</li>
        <li class="list day" v-show="isYear==='0'">27</li>
        <li class="list day" v-show="isYear==='0'">28</li>
        <li class="list day" v-show="isYear==='0'">29</li>
        <li class="list day" v-show="isYear==='0'">30</li>
        <li class="list day" v-show="isYear==='0'">31</li>

        <!--<li class="list name c-pointer" @click="changeOrder('stock_no')">
          库存数量
          <i :class="[{ 'el-icon-d-caret': headOrder.stock_no === 0 }, { 'el-icon-caret-top': headOrder.stock_no === 1 }, { 'el-icon-caret-bottom': headOrder.stock_no === 2 }]"></i>
        </li>-->
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="item in importList" :key="item.key">
              <div class="list-content">
                <div class="name">{{ item.code }}</div>
                <div class="name">{{ item.eqpTypeName }}</div>
                <div class="name">{{ item.locationName }}</div>
                <div class="name">{{ item.teamName }}</div>
                <div class="name">{{ item.unit }}</div>
                <div class="name">{{ item.quantity }}</div>
                <div class="numberUnit">{{ item.frequency }}</div>
                <div class="minite">{{ item.once }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.january===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.february===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.march===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.april===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.may===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.june===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.july===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.august===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.september===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.october===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.november===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='1'">{{ item.december===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.one===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.two===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.three===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.four===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.five===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.six===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.seven===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.eight===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.nine===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.ten===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.eleven===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.twelve===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.thirteen===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.fourteen===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.fifteen===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.sixteen===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.seventeen===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.eighteen===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.nineteen===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.twenty===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.twentyOne===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.twentyTwo===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.twentyThree===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.twentyFour===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.twentyFive===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.twentySix===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.twentySeven===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.twentyEight===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.twentyNine===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.thirty===1?'☆':'' }}</div>
                <div class="day" v-show="isYear==='0'">{{ item.thirtyOne===1?'☆':'' }}</div>
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
import { getCascaderObj } from '@/common/js/utils.js'
import { btn } from '@/element/btn.js'
import api from '@/api/workflowApi'
import apiOrg from '@/api/orgApi'
export default {
  name: 'ImportList',
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
      title: ' | 计划导入',
      importCommon: {
        year: '2019',
        company: ''
      },
      btn: {save: false},
      isYearTmp: '0',
      isYear: '0',
      companyList: [],
      companyPath: [],
      importList: [],
      total: 0,
      currentPage: 1,
      loading: false,
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
      }
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
        return item.actionID === btn.import.save
      })
    }
    // 公司加载
    apiOrg.getOrgAll().then(res => {
      this.companyList = res.data
    }).catch(err => console.log(err))
    // this.init()
  },
  activated () {
    if (this.importCommon.year !== '' && this.importCommon.company !== '') {
      this.searchResult(this.currentPage)
    }
  },
  methods: {
    add () {
      this.$router.push({
        name: 'ImportExl'
      })
    },
    cascader_change (val) {
      if (val.length === 0) {
        this.importCommon.company = ''
        this.companyPath = []
        return
      }
      let selectedTeam = val[val.length - 1]
      let obj = getCascaderObj(selectedTeam, this.companyList)
      if (obj.node_type === 1) {
        this.importCommon.company = selectedTeam
      } else {
        this.$message({
          message: '您选择的不是公司',
          type: 'error'
        })
      }
      // let el = document.querySelector('.pop-team')
      // el.style.display = 'none'
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
      if (this.importCommon.year === '' || this.importCommon.company === '') {
        this.$message({
          message: '由于数据量较大，请选择年份和公司',
          type: 'warning'
        })
        return
      }
      this.currentPage = page
      this.loading = true
      api.getImportPage({
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        SearchYear: this.importCommon.year,
        SearchCompany: this.importCommon.company,
        IsYear: this.isYearTmp === '1'
      }).then(res => {
        this.loading = false
        if (res.data === null) {
          this.total = 0
          this.importList = []
        } else {
          this.total = res.data.total
          this.importList = res.data.rows
          this.isYear = this.isYearTmp
        }
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
      this.searchResult(val)
    },

    // 下一页
    nextPage (val) {
      this.currentPage = val
      this.searchResult(val)
    }
  }
}
</script>
<style lang="scss" scoped>
// 当前内容页面总高度
$height: $content-height - 60;
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
  height: percent($height - 46, $height)!important;
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

  .numberUnit{
    width: 80px;
    word-break: break-word;
  }

  .minite{
    width: 70px;
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
    width: 150px;
    word-break: break-word;
  }

  // .name{
  //   a{
  //     color: #42abfd;
  //   }
  // }
  .last-update-time{
    width: 200px;
    color: $color-content-text;
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

/deep/
.el-year-table td .cell{
  color: $color-white!important;
}
/deep/
.el-date-picker__header-label{
  color: $color-white!important;
}

</style>
