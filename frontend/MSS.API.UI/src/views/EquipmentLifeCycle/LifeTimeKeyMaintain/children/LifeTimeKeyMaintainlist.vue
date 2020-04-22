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
            <label for="">设备类别</label>
            <div class="inp">
             <el-select v-model="deviceType" clearable filterable placeholder="请选择" @change="validatedeviceTypeSelect(deviceType)">
                <option disabled value="" selected>请选择</option>
                <el-option
                  v-for="item in deviceTypeList"
                  :key="item.key"
                  :label="item.tName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">设备名称</label>
            <div class="inp">
              <!-- <el-select v-model="deviceName" clearable placeholder="请选择">
                <option disabled value="" selected>请选择</option>
                 <el-option
                 v-for="item in devicelist"
                 :key="item.key"
                 :value="item.id"
                 :label="item.deviceName">
                 </el-option>
              </el-select> -->
               <el-cascader clearable filterable
                             :props="defaultParams1"
                             change-on-select
                             :show-all-levels="true"
                             :options="devicelist"
                             v-model="deviceName">
                </el-cascader>
            </div>
          </div>
        </div>
         <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list number c-pointer"
            @click="changeOrder('eqp_code')">
          设备编码
          <i :class="[{ 'el-icon-d-caret': headOrder.eqp_code === 0 }, { 'el-icon-caret-top': headOrder.eqp_code === 1 }, { 'el-icon-caret-bottom': headOrder.eqp_code === 2 }]"></i>
        </li>
        <li class="list number c-pointer"
            @click="changeOrder('eqp_name')">
          设备名称
          <i :class="[{ 'el-icon-d-caret': headOrder.eqp_name === 0 }, { 'el-icon-caret-top': headOrder.eqp_name === 1 }, { 'el-icon-caret-bottom': headOrder.eqp_name === 2 }]"></i>
        </li>
        <li class="list number c-pointer"
            @click="changeOrder('team')">
          管辖班组
          <i :class="[{ 'el-icon-d-caret': headOrder.team === 0 }, { 'el-icon-caret-top': headOrder.team === 1 }, { 'el-icon-caret-bottom': headOrder.team === 2 }]"></i>
        </li>
        <li class="list number c-pointer"
            @click="changeOrder('online_date')">
          上线时间
          <i :class="[{ 'el-icon-d-caret': headOrder.online_date === 0 }, { 'el-icon-caret-top': headOrder.online_date === 1 }, { 'el-icon-caret-bottom': headOrder.online_date === 2 }]"></i>
        </li>
        <li class="list number c-pointer"
            @click="changeOrder('life')">
          使用年限
          <i :class="[{ 'el-icon-d-caret': headOrder.life === 0 }, { 'el-icon-caret-top': headOrder.life === 1 }, { 'el-icon-caret-bottom': headOrder.life === 2 }]"></i>
        </li>
        <li class="list number c-pointer"
            @click="changeOrder('next_medium_repair_date')">
          下次中修日期
          <i :class="[{ 'el-icon-d-caret': headOrder.next_medium_repair_date === 0 }, { 'el-icon-caret-top': headOrder.next_medium_repair_date === 1 }, { 'el-icon-caret-bottom': headOrder.next_medium_repair_date === 2 }]"></i>
        </li>
        <li class="list number c-pointer"
            @click="changeOrder('next_large_repair_date')">
          下次大修日期
          <i :class="[{ 'el-icon-d-caret': headOrder.next_large_repair_date === 0 }, { 'el-icon-caret-top': headOrder.next_large_repair_date === 1 }, { 'el-icon-caret-bottom': headOrder.next_large_repair_date === 2 }]"></i>
        </li>
        <!-- <li class="list last-update-time c-pointer"
            @click="changeOrder('updated_time')">
          最后更新时间
          <i :class="[{ 'el-icon-d-caret': headOrder.updatedTime === 0 }, { 'el-icon-caret-top': headOrder.updatedTime === 1 }, { 'el-icon-caret-bottom': headOrder.updatedTime === 2 }]"></i>
        </li>
        <li class="list last-maintainer c-pointer"
            @click="changeOrder('Updated_By')">
          最后更新人
          <i :class="[{ 'el-icon-d-caret': headOrder.updatedBy === 0 }, { 'el-icon-caret-top': headOrder.updatedBy === 1 }, { 'el-icon-caret-bottom': headOrder.updatedBy === 2 }]"></i>
        </li> -->
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list"
                v-for="(item) in LifeTimeKeyMaintainlist"
                :key="item.key">
              <div class="list-content">
                <div class="number">{{ item.code }}</div>
                <div class="number">{{ item.name }}</div>
                <div class="number">{{ item.teamName }}</div>
                <div class="number">{{ item.online }}</div>
                <div class="number">{{ item.life }}</div>
                <div class="number">{{ item.nextMediumRepairDate }}</div>
                <div class="name">{{ item.nextLargeRepairDate }}</div>
                <!-- <div class="last-update-time color-white">{{ item.updatedTime }}</div>
                <div class="last-maintainer">{{ item.updated_name }}</div> -->
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
  </div>
</template>
<script>
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
import eqpApi from '@/api/eqpApi.js'
export default {
  name: 'LifeTimeKeyMaintainlist',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 寿命与重点维保',
      maintain_type: '',
      deviceName: '',
      devicelist: null,
      deviceType: '',
      deviceTypeList: [],
      MaintainList: [{ label: '中修', value: 40 }, { label: '大修', value: 41 }],
      LifeTimeKeyMaintainlist: [],
      editLifeTimeKeyMaintainIDList: [],
      bCheckAll: false,
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      defaultParams1: {
        label: 'name',
        value: 'id',
        children: 'children'
      },
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
        eqp_code: 0,
        eqp_name: 0,
        team: 0,
        online_date: 0,
        life: 0,
        nextMediumRepairDate: 0,
        nextLargeRepairDate: 0
      }
    }
  },
  created () {
    this.init()
    eqpApi.getEqpTypeAll().then(res => {
      this.deviceTypeList = res.data
    }).catch(err => console.log(err))
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  methods: {
    init () {
      this.bCheckAll = false
      // this.checkAll()
      this.currentPage = 1
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.eqp_code = 0
        this.headOrder.eqp_name = 0
        this.headOrder.team = 0
        this.headOrder.online_date = 0
        this.headOrder.life = 0
        this.headOrder.nextMediumRepairDate = 0
        this.headOrder.nextLargeRepairDate = 0
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
      this.DeviceMaintainRegList = []
      this.currentPage = page
      this.loading = true
      let parm = {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        rows: 10,
        page: page,
        device_type: this.deviceType,
        device_id: this.deviceName[this.deviceName.length - 1]
      }
      api.GetLifeTimeKeyListByPage(parm).then(res => {
        console.log(res.data)
        this.loading = false
        if (res.code === 0) {
          res.data.rows.map(item => {
            item.online = item.online.slice(0, 10)
            item.nextMediumRepairDate = item.nextMediumRepairDate.slice(0, 10)
            item.nextLargeRepairDate = item.nextLargeRepairDate.slice(0, 10)
          })
          this.LifeTimeKeyMaintainlist = res.data.rows
          this.total = res.data.total
        }
      }).catch(err => console.log(err))
    },
    // 搜索功能
    searchRes () {
      this.$emit('title', '| 寿命与重点维保')
      this.loading = true
      this.init()
      this.searchResult(1)
    },
    // 全选
    // checkAll () {
    //   this.bCheckAll ? this.DeviceMaintainRegList.map(val => this.editDeviceMaintainRegIDList.push(val.id)) : this.editDeviceMaintainRegIDList = []
    //   this.emitEditID()
    // },
    validatedeviceTypeSelect (val) {
      if (val === '') {
        this.devicelist = []
        this.deviceType.text = ''
      }
      api.GetEqpByTypeAndLine(val).then(res => {
        if (res.data.length > 0) {
          this.devicelist = res.data
          this.deviceType.id = ''
        } else {
          this.devicelist = null
          this.deviceType.id = ''
        }
      }).catch(err => {
        console.log(err)
      })
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
    width: 10%;
    color: $color-content-text;
  }
  .maintain-date{
    width: 8%;
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
