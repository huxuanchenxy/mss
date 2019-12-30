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
            <el-radio v-model="position" label="1" style="padding-right:5px;"></el-radio>
            <label for="name">车站</label>
            <div class="inp">
              <el-select v-model="Station" :disabled="checkDisable(1)" placeholder="请选择">
                <el-option label="请选择" value=''></el-option>
                <el-option
                  v-for="item in StationList"
                  :key="item.key"
                  :label="item.areaName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <el-radio v-model="position" label="2" style="padding-right:5px;"></el-radio>
            <label for="">轨行区</label>
            <div class="inp">
              <el-select v-model="RailLineArea" :disabled="checkDisable(2)" placeholder="请选择">
                <el-option label="请选择" value=''></el-option>
                <el-option
                  v-for="item in RailLineAreaList"
                  :key="item.key"
                  :label="item.areaName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <el-radio v-model="position" label="3" style="padding-right:5px;"></el-radio>
            <label for="">保护区</label>
            <div class="inp">
              <el-select v-model="ProtectArea" :disabled="checkDisable(3)" placeholder="请选择">
                <el-option label="请选择" value=''></el-option>
                <el-option
                  v-for="item in ProtectAreaList"
                  :key="item.key"
                  :label="item.areaName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <el-radio v-model="position" label="4" style="padding-right:5px;"></el-radio>
            <label for="">车场</label>
            <div class="inp">
              <el-select v-model="ProductionArea" :disabled="checkDisable(4)" placeholder="请选择">
                <el-option label="请选择" value=''></el-option>
                <el-option
                  v-for="item in ProductionAreaList"
                  :key="item.key"
                  :label="item.areaName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <el-radio v-model="position" label="5" style="padding-right:5px;"></el-radio>
            <label for="">状态</label>
            <div class="inp">
              <el-select v-model="status" :disabled="checkDisable(5)" placeholder="请选择">
                <el-option label="请选择" value=''></el-option>
                <el-option label="故障" value='0'></el-option>
                <el-option label="报警" value='1'></el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <!--<el-radio v-model="position" label="6" style="padding-right:5px;"></el-radio>-->
            <label for="">图纸编号</label>
            <div class="inp">
              <el-input v-model.trim="eqpCode" placeholder="请输入设备图纸编号"></el-input>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <span style="padding-left:5px">
          全线设备总数：
          <font style="color:#fff;font-size:17px;">{{totalEqp}}</font>
        </span>
        <span style="padding-left:5px">
          , 有
          <font style="color:#be0909;font-size:17px;">{{troubleCount}}</font>
          台设备故障
        </span>
        <span style="padding-left:5px">
          , 有
          <font style="color:#f79707;font-size:17px;">{{otherCount}}</font>
          台设备报警
        </span>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list number c-pointer" @click="changeOrder('eqp_code')">
          图纸编号
          <i :class="[{ 'el-icon-d-caret': headOrder.eqp_code === 0 }, { 'el-icon-caret-top': headOrder.eqp_code === 1 }, { 'el-icon-caret-bottom': headOrder.eqp_code === 2 }]"></i>
        </li>
        <li class="list name c-pointer" @click="changeOrder('eqp_name')">
          名称
          <i :class="[{ 'el-icon-d-caret': headOrder.eqp_name === 0 }, { 'el-icon-caret-top': headOrder.eqp_name === 1 }, { 'el-icon-caret-bottom': headOrder.eqp_name === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('sub_system')">
          子系统
          <i :class="[{ 'el-icon-d-caret': headOrder.sub_system === 0 }, { 'el-icon-caret-top': headOrder.sub_system === 1 }, { 'el-icon-caret-bottom': headOrder.sub_system === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('eqp_type')">
          类型
          <i :class="[{ 'el-icon-d-caret': headOrder.eqp_type === 0 }, { 'el-icon-caret-top': headOrder.eqp_type === 1 }, { 'el-icon-caret-bottom': headOrder.eqp_type === 2 }]"></i>
        </li>
        <li class="list number c-pointer" @click="changeOrder('team')">
          管辖班组
          <i :class="[{ 'el-icon-d-caret': headOrder.team === 0 }, { 'el-icon-caret-top': headOrder.team === 1 }, { 'el-icon-caret-bottom': headOrder.team === 2 }]"></i>
        </li>
        <li class="list number">
          设备状态
        </li>
        <li class="list number">
          操作
        </li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in dataList" :key="item.key">
              <div class="list-content" :style="textColor(item.status)">
                <div class="number">{{ item.code }}</div>
                <div class="name">{{ item.name }}</div>
                <div class="name">{{ item.subSystemName }}</div>
                <div class="name">{{ item.tName }}</div>
                <div class="number">{{ item.teamName }}</div>
                <div class="number">
                  {{ statusShow(item.status) }}
                </div>
                <div class="number">
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
// import { transformDate } from '@/common/js/utils.js'
import Bus from '@/components/Bus'
import { ApiRESULT, EqpStatus } from '@/common/js/utils.js'
import XButton from '@/components/button'
import eqpApi from '@/api/eqpApi'
import apiEvent from '@/api/eventCenterApi'
import areaApi from '@/api/AreaApi'
export default {
  name: 'monitorCenter',
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
      title: ' | 设备监控',

      position: '1',
      StationList: [],
      RailLineAreaList: [],
      ProtectAreaList: [],
      ProductionAreaList: [],
      Station: '',
      RailLineArea: '',
      ProtectArea: '',
      ProductionArea: '',
      status: '',
      monitorCenter: '',
      dataList: [],
      alarmList: [], // 所有报警未确认的
      totalEqp: 0,
      troubleCount: 0, // 总故障数
      otherCount: 0, // 总报警数
      newMsg: null,

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
        eqp_code: 1,
        eqp_name: 0,
        sub_system: 0,
        eqp_type: 0,
        team: 0
      }
    }
  },
  watch: {
    newMsg () {
      if (this.newMsg.type === 0) {
        this.getAlarm()
      }
    },
    dataList () {
      this.updateEqpStatus()
    }
  },
  created () {
    this.getAllAreaList()
    this.init()
    this.getAlarm()
    this.getTotalEqp()
    Bus.$on('eqp_monitor', data => (this.newMsg = data))
  },
  activated () {
    this.search()
  },
  methods: {
    detail (id) {
      this.$router.push({
        name: 'DetailEqp',
        params: {
          id: id,
          sourceName: 'monitorCenter'
        }
      })
    },
    checkDisable (label) {
      if (+this.position === label) {
        return false
      } else {
        return true
      }
    },
    init () {
      this.bCheckAll = false
      this.checkAll()
      this.currentPage = 1
      // this.search()
    },
    getAllAreaList () {
      // 车站
      areaApi.GetSubWayStation(9).then(res => {
        this.StationList = res.data
      }).catch(err => console.log(err))
      // 正线轨行区
      areaApi.GetSubWayStation(10).then(res => {
        this.RailLineAreaList = res.data
      }).catch(err => console.log(err))
      // 保护区
      areaApi.GetSubWayStation(11).then(res => {
        this.ProtectAreaList = res.data
      }).catch(err => console.log(err))
      // 车场生产区
      areaApi.GetSubWayStation(12).then(res => {
        this.ProductionAreaList = res.data
      }).catch(err => console.log(err))
    },

    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.eqp_code = 0
        this.headOrder.eqp_name = 0
        this.headOrder.sub_system = 0
        this.headOrder.eqp_type = 0
        this.headOrder.team = 0
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
    textColor (status) {
      let style = {}
      switch (status) {
        case EqpStatus.fault:
          style = {background: '#be0909'}
          break
        case EqpStatus.warning:
          style = {background: '#f79707'}
          break
      }
      return style
    },
    statusShow (status) {
      let content = '正常'
      switch (status) {
        case EqpStatus.fault:
          content = '故障'
          break
        case EqpStatus.warning:
          content = '报警'
          break
      }
      return content
    },
    calcTrouble () {
      this.troubleCount = Object.values(this.alarmList).filter(item => item.isTrouble).length
      this.otherCount = Object.values(this.alarmList).length - this.troubleCount
    },
    updateEqpStatus () {
      for (let i = 0; i < this.dataList.length; ++i) {
        let item = this.dataList[i]
        let cur = this.alarmList[item.id]
        if (cur !== undefined) {
          if (cur.isTrouble) {
            item.status = EqpStatus.fault
          } else {
            item.status = EqpStatus.warning
          }
        } else {
          item.status = EqpStatus.normal
        }
      }
    },
    getAlarm () {
      apiEvent.getAlarm().then(res => {
        if (res.code === ApiRESULT.Success) {
          this.alarmList = this.groupBy(res.data, function (item) {
            return item.eqpID
          })
          this.calcTrouble()
          this.updateEqpStatus()
          if (this.position === '5') {
            this.searchByStatus()
          }
        }
      }).catch(err => console.log(err))
    },
    getTotalEqp () {
      eqpApi.getAllEqpCount().then(res => {
        if (res.code === ApiRESULT.Success) {
          this.totalEqp = res.data
        }
      }).catch(err => console.log(err))
    },
    groupBy (array, f) {
      const groups = {}
      array.forEach(function (o) {
        const group = f(o)
        if (group) {
          groups[group] = groups[group] || {isTrouble: false, pids: []}
          if (o.isTrouble) {
            groups[group].isTrouble = true
          }
          groups[group].pids.push(o)
        }
      })
      return groups
    },
    searchByStatus () {
      let ids = []
      if (this.status === '0') { // 故障
        for (let key in this.alarmList) {
          if (this.alarmList[key].isTrouble) {
            ids.push(key)
          }
        }
      } else if (this.status === '1') { // 报警
        for (let key in this.alarmList) {
          if (!this.alarmList[key].isTrouble) {
            ids.push(key)
          }
        }
      } else {
        ids = Object.keys(this.alarmList)
      }
      if (ids.length === 0) {
        // this.dataList = []
        // this.total = 0
        let parm = {
          order: this.currentSort.order,
          sort: this.currentSort.sort,
          rows: 10,
          page: this.currentPage
        }
        eqpApi.getEqp(parm).then(res => {
          this.loading = false
          this.dataList = res.data.rows
          console.log(this.dataList)
          this.total = res.data.total
        }).catch(err => console.log(err))
        return
      }
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: this.currentPage,
        ids: ids
      }
      this.loading = true
      apiEvent.getAlarmEqp(parm).then(res => {
        this.loading = false
        this.dataList = res.data.rows
        this.total = res.data.total
      }).catch(err => console.log(err))
    },
    search () {
      if (this.position === '5') {
        this.searchByStatus()
        return
      }
      this.loading = true
      let sub = ''
      switch (this.position) {
        case '1':
          sub = this.Station
          break
        case '2':
          sub = this.RailLineArea
          break
        case '3':
          sub = this.ProtectArea
          break
        case '4':
          sub = this.ProductionArea
          break
      }
      let location = this.position
      if (sub) {
        location = location + ',' + sub
      }
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: this.currentPage,
        LocationPath: location,
        SearchCode: this.eqpCode
      }
      eqpApi.getEqp(parm).then(res => {
        this.loading = false
        // res.data.rows.map((item, index) => {
        //   if (index === 1) {
        //     item.status = EqpStatus.normal
        //   } else if (index === 2 || index === 5) {
        //     item.status = EqpStatus.warning
        //   } else if (index === 3 || index === 8) {
        //     item.status = EqpStatus.fault
        //   }
        // })
        this.dataList = res.data.rows
        this.total = res.data.total
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
    width: 16%;
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
/deep/ .el-radio__label{
    display: none;
  }
</style>
