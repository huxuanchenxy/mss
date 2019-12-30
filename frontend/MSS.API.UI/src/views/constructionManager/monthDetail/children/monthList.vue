<template>
  <div
    class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active">
        <router-link :to="{name: 'CommonList'}">返回</router-link>
      </x-button>
    </div>
    <div class="box1">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
          <div class="input-group">
            <label for="name">班组</label>
            <div class="inp">
              <el-cascader clearable
                change-on-select
                :props="defaultParams"
                @change="cascader_change"
                :show-all-levels="true"
                :options="teamList"
                v-model="teamPath">
              </el-cascader>
            </div>
          </div>
          <div class="input-group">
            <label for="name">地点</label>
            <div class="inp">
              <el-cascader clearable
                change-on-select
                @change="position_change"
                :props="areaParams"
                :show-all-levels="true"
                :options="areaList"
                v-model="areaPath">
              </el-cascader>
            </div>
          </div>
          <div class="input-group">
            <label for="name">计划日期</label>
            <div class="inp">
              <el-date-picker
                v-model="planDate"
                value-format="yyyy.MM.dd"
                placeholder="请选择计划日期">
              </el-date-picker>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchResult(1)">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
    </div>
    <div  class="content-wrap">
      <el-tabs class="tab-height" v-model="activeName" @tab-click="searchResult(1)">
        <el-tab-pane class="pane-height pane-notification" :label="(index+1)+'月'" :name="index+''" v-for="(tab, index) in monthList" :key="tab.key">
          <!-- 内容 -->
            <ul class="content-header">
              <li class="list"></li>
              <li class="list name">工作类型</li>
              <li class="list name">设施设备</li>
              <li class="list name">设备地点</li>
              <li class="list name">管辖班组</li>
              <li class="list name">维护类型</li>
              <li class="list name">维护频次</li>
              <li class="list name">数量</li>
              <li class="list name">单位</li>
              <li class="list last-update-time">计划日期</li>
              <li class="list name">操作</li>
            </ul>
            <div class="scroll">
              <el-scrollbar>
                <ul class="list-wrap">
                  <li class="list" v-for="item in tab.rows" :key="item.key">
                    <div class="list-content">
                      <div class="checkbox">
                        <input type="checkbox" v-model="editObj" :value="item">
                      </div>
                      <div class="name">{{ item.workTypeName }}</div>
                      <div class="name">{{ item.eqpTypeName }}</div>
                      <div class="name">{{ item.locationName }}</div>
                      <div class="name">{{ item.teamName }}</div>
                      <div class="name">{{ item.pmTypeName }}</div>
                      <div class="name">{{ item.pmCycle }}</div>
                      <div class="name">{{ item.planQuantity }}</div>
                      <div class="name">{{ item.unit }}</div>
                      <div class="last-update-time">{{ item.planDate }}</div>
                      <div class="name">
                        <li @click="edit(item)" ><x-button :disabled="type==3">修改</x-button></li>
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
                  :total="currentTotal">
                  <span>总共 {{ tab.total }} 条记录</span>
                </el-pagination>
              </el-scrollbar>
            </div>
        </el-tab-pane>
      </el-tabs>
    </div>
  </div>
</template>
<script>
import { getCascaderObj } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/workflowApi'
import apiOrg from '@/api/orgApi'
import apiArea from '@/api/AreaApi.js'
export default {
  name: 'MonthList',
  components: {
    XButton
  },
  data () {
    return {
      btn: {
        save: false
      },
      // 当前标签页的总数，直接用tab.total且页数不同时，翻页无效（比如其他标签页2页，9月份3页，第三页始终只能跳到第二页）
      currentTotal: 1,
      editObj: [],
      title: '',
      areaParams: {
        label: 'areaName',
        value: 'id',
        children: 'children'
      },
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      loading: false,
      pmTitle: '',
      updateTitle: '',
      importCommon: {},
      type: '',
      activeName: '0',
      monthList: [
        [{
          workTypeName: 1,
          eqpTypeName: 2,
          locationName: 3,
          teamName: 4,
          pmTypeName: 5,
          pmFrequency: 6,
          planQuantity: 7,
          unit: 8,
          planDate: 9
        }],
        [{
          workTypeName: 10,
          eqpTypeName: 2,
          locationName: 3,
          teamName: 4,
          pmTypeName: 5,
          pmFrequency: 6,
          planQuantity: 7,
          unit: 8,
          planDate: 9
        }]
      ],
      currentPage: 1,
      team: '',
      teamPath: [],
      teamList: [],
      area: '',
      areaPath: [],
      areaList: [],
      planDate: ''
    }
  },
  created () {
    this.importCommon = this.$route.params.item
    this.type = this.$route.params.type
    this.title = ' | 月计划列表 | ' + this.importCommon.year + '年-' + this.importCommon.lineName + '-' + this.importCommon.companyName + '-' + this.importCommon.departmentName
    this.updateTitle = ' | 月计划修改 | ' + this.importCommon.year + '年-' + this.importCommon.lineName + '-' + this.importCommon.companyName + '-' + this.importCommon.departmentName
    this.activeName = new Date().getMonth() + ''
    // 班组加载
    apiOrg.getOrgAll().then(res => {
      this.teamList = res.data
    }).catch(err => console.log(err))
    // 地点加载
    apiArea.SelectConfigAreaData().then(res => {
      this.areaList = res.data.dicAreaList
    }).catch(err => console.log(err))
  },
  activated () {
    if (this.$route.params.type !== undefined) {
      this.type = this.$route.params.type
      if (this.type === 1) {
        api.createMonthPlan(this.importCommon.id).then(res => {
          this.monthList = res.data
          this.currentTotal = this.monthList[0].total
          this.type = 2
        }).catch(err => console.log(err))
      } else {
        this.monthList = []
        for (let i = 0; i < 12; i++) {
          this.monthList[i] = []
        }
      }
    }
    this.searchResult(1)
  },
  methods: {
    // add () {
    //   if (this.editObj.length === 0) {
    //     this.$message({
    //       message: '请选择所要创建检修单的计划',
    //       type: 'warning'
    //     })
    //     return
    //   }
    //   let arr = []
    //   let before = {
    //     team: this.editObj[0].team,
    //     location: this.editObj[0].location,
    //     locationBy: this.editObj[0].locationBy,
    //     planDate: this.editObj[0].planDate
    //   }
    //   for (let i = 0; i < this.editObj.length; i++) {
    //     let item = this.editObj[i]
    //     if (item.team === before.team && item.location === before.location && item.locationBy === before.locationBy && (item.planDate === before.planDate || item.planDate.indexOf('-') !== -1 || before.planDate.indexOf('-') !== -1)) {
    //       arr.push({
    //         detail: item.id,
    //         count: item.planQuantity,
    //         planCode: item.eqpType,
    //         pmType: item.pmType
    //       })
    //     } else {
    //       this.$message({
    //         message: '所选计划的班组、地点、时间必须保持一致',
    //         type: 'warning'
    //       })
    //     }
    //   }
    //   let mList = {
    //     title: this.pmTitle,
    //     team: before.team,
    //     location: before.location,
    //     locationBy: before.locationBy,
    //     planDate: before.planDate,
    //     details: arr
    //   }
    //   api.saveMList(mList).then(res => {
    //     if (res.code === 0) {
    //       this.$message({
    //         message: '创建成功',
    //         type: 'success'
    //         // onClose: () => {
    //         //   this.$router.push({
    //         //     name: 'MaintenanceList'
    //         //   })
    //         // }
    //       })
    //       this.popVisiable = false
    //     } else {
    //       this.$message({
    //         message: res.msg === '' ? '创建失败' : res.msg,
    //         type: 'error'
    //       })
    //     }
    //   }).catch(err => console.log(err))
    // },
    position_change (val) {
      if (val.length === 0) {
        this.area = ''
        this.areaPath = []
        return
      }
      if (this.areaPath.length < 3) {
        this.areaPath = []
        this.$message({
          message: '地点在3级和4级位置',
          type: 'warning'
        })
      } else {
        this.area = val[val.length - 1]
      }
    },
    // 班组下拉选中，过滤非班组
    cascader_change (val) {
      if (val.length === 0) {
        this.teamPath = []
        this.team = ''
        return
      }
      let selectedTeam = val[val.length - 1]
      let obj = getCascaderObj(selectedTeam, this.teamList)
      if (obj.node_type === 3) {
        this.team = selectedTeam
      } else {
        this.teamPath = []
        this.$message({
          message: '您选择的不是班组',
          type: 'warning'
        })
      }
    },
    edit (item) {
      this.$router.push({
        name: 'UpdateMonthPlan',
        params: {
          id: item.id,
          title: this.updateTitle,
          locationName: item.locationName
        }
      })
    },
    searchResult (page) {
      this.currentPage = page
      // this.loading = true
      let month = parseInt(this.activeName)
      let selectMonth
      if (this.planDate !== null && this.planDate !== '') {
        selectMonth = parseInt(this.planDate.split('.')[1])
        if (selectMonth !== month + 1) {
          month = selectMonth - 1
          this.activeName = month + ''
        }
      }
      api.getMonthPlanPage({
        // order: this.currentSort.order,
        // sort: this.currentSort.sort,
        rows: 10,
        page: page,
        Query: this.importCommon.id,
        Month: month + 1,
        Team: this.team,
        Location: this.area,
        LocationBy: this.areaPath.length === 0 ? '' : this.areaPath.length - 2,
        PlanDate: this.planDate
      }).then(res => {
        this.loading = false
        let tmp = res.data
        this.currentTotal = tmp.total
        if (tmp === null) tmp = []
        this.$set(this.monthList, month, tmp)
        // this.monthList[month] = res.data
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
$con-height: $content-height - 106;
// 内容区
.content-wrap{
  overflow: hidden;
  height: percent($con-height, $content-height)!important;
  // height: 90%!important;
  text-align: center;
  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 30px;
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);

    .last-update-time{
      color: $color-white;
    }
  }
  .tab-height{
    height: percent($con-height, $con-height);
  }
  /deep/ .el-tabs__header{
    margin-left: 10px!important;
    height: percent(30, $con-height)
  }
  /deep/ .el-tabs__content{
    height: percent($con-height - 50, $con-height)
  }
  .pane-height{
    height: 100%
  }
  .pane-notification{
    .content-header{
      height: percent(25, $con-height)
    }
  }
  .scroll{
    height: percent($con-height - 50, $con-height)
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
    width: 30px;
  }

  .name,
  .btn-wrap{
    width: 8%;
  }

  .last-update-time{
    width: 15%;
    color: $color-white;
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
// 功能区
.operation{
  .input-group{
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list{
      width: 30%;
      margin-top: PXtoEm(25);

      span{
        width: 28%;
      }

      .inp-wrap{
        display: flex;
        align-items: center;
      }

      &:nth-of-type(3n+1){
        // justify-content: flex-start;
      }

      &:nth-of-type(3n){
        // justify-content: flex-end;
      }
    }
  }
}
.btn-enter{
  margin: 15px 0;
  text-align: center;

  button{
    border-color: $color-main-btn;
    background: $color-main-btn;
  }
}
.import-btn{
  margin-top: 50px;
  border-color: $color-main-btn!important;
  background: $color-main-btn!important;
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

/deep/
.center .el-input__inner{
  text-align: center;
  width: 90%;
}
/deep/
.btn1{
  background: none;
  width: 260px;
  border: 1px solid $color-main-btn-border;
  border-radius: $border-radius;
  color: $color-white;
  cursor: pointer;
  &.active,
  &:hover{
    border-color: $color-main-btn!important;
    background: $color-main-btn!important;
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
.el-button--text:hover{
  color: #4998d4;
  border-color: transparent;
  background-color: transparent;
  border: none;
}
</style>
<style>
.my-pop{
  background: #29282E!important;
  border-color: #29282E!important;
}
</style>
