<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="{ titlepic }" alt="" class="icon">{{ title }}
      </h2>
    </div>
    <div class="box">
      <!-- 搜索框 -->
      <div class="con-padding-horizontal search-wrap">
        <div class="wrap">
          <div class="input-group">
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
        </div>
      </div>
      <ul class="con-padding-horizontal btn-group">
        <li class="list" @click="refreshpage"><x-button>刷新</x-button></li>
      </ul>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <!-- <ul class="content-header">
        <li class="list"><input type="checkbox" v-model="bCheckAll" style="visibility: hidden;"></li>
        <li class="list number c-pointer" @click="changeOrder('service_name')">
          服务名称
          <i :class="[{ 'el-icon-d-caret': headOrder.serviceName === 0 }, { 'el-icon-caret-top': headOrder.serviceName === 1 }, { 'el-icon-caret-bottom': headOrder.serviceName === 2 }]"></i>
       </li>
        <li class="list number c-pointer" @click="changeOrder('service_addr')">
          主机ip
          <i :class="[{ 'el-icon-d-caret': headOrder.serviceAddr === 0 }, { 'el-icon-caret-top': headOrder.serviceAddr === 1 }, { 'el-icon-caret-bottom': headOrder.serviceAddr === 2 }]"></i>
       </li>
       <li class="list number c-pointer" @click="changeOrder('service_port')">
          CPU
          <i :class="[{ 'el-icon-d-caret': headOrder.servicePort === 0 }, { 'el-icon-caret-top': headOrder.servicePort === 1 }, { 'el-icon-caret-bottom': headOrder.servicePort === 2 }]"></i>
       </li>
        <li class="list number c-pointer" @click="changeOrder('healthStatus')" >
          内存
          <i :class="[{ 'el-icon-d-caret': headOrder.healthStatus === 0 }, { 'el-icon-caret-top': headOrder.healthStatus === 1 }, { 'el-icon-caret-bottom': headOrder.healthStatus === 2 }]"></i>
       </li>
               <li class="list number c-pointer">
          网络
          <i :class="[{ 'el-icon-d-caret': headOrder.healthStatus === 0 }, { 'el-icon-caret-top': headOrder.healthStatus === 1 }, { 'el-icon-caret-bottom': headOrder.healthStatus === 2 }]"></i>
       </li>
        <li class="list number c-pointer" @click="changeOrder('service_pid')" >硬盘
          <i :class="[{ 'el-icon-d-caret': headOrder.servicePID === 0 }, { 'el-icon-caret-top': headOrder.servicePID === 1 }, { 'el-icon-caret-bottom': headOrder.servicePID === 2 }]"></i>
        </li>
      </ul> -->
      <div class="scroll">
        <!-- <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in UserList" :key="item.key">
              <div class="list-content">
                <div class="checkbox">
                  <input type="checkbox" v-model="lookOperlogID" :value="item.id" @change="emitEditID">
                </div>
                <div class="name">{{ item.prettyName }}</div>
                <div class="name">{{ item.ip }}</div>
                <div class="name">{{ item.cpuLoad }}%</div>
                <div class="name">{{ item.percentMemoryUsed }}%</div>
                <div class="name">{{ item.prettyTotalNetwork }}</div>
                <div class="name">{{ item.diskText }}%</div>
              </div>
            </li>
          </ul>
        </el-scrollbar> -->
      <div id="mynetwork"></div>
      </div>
    </div>

  </div>
</template>
<script>
import XButton from '@/components/button'
import api from '@/api/monitorserver'
import vis from 'vis-network/dist/vis-network.min.js'
import 'vis-network/dist/vis-network.min.css'
import pcimg from './pc.png'
export default {
  name: 'SeeMonitorServer',
  components: {
    XButton
  },
  data () {
    return {
      title: '',
      titlepic: '',
      time: '',
      serviceName: '',
      startTime: '',
      endTime: '',
      role: '',
      roleList: [],
      UserList: [],
      lookOperlogID: [],
      bCheckAll: false,
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'service_name',
        order: 'asc'
      },
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      headOrder: {
        id: 0,
        healthStatus: 0,
        serviceName: 0,
        action_name: 0,
        serviceAddr: 0,
        servicePort: 0,
        servicePID: 0,
        mac_add: 0,
        updatedTime: 0
      }
    }
  },
  created () {
    // this.$emit('title', '| 服务监控')
    this.init()
  },
  activated () {
    this.searchResult(this.currentPage)
  },
  mounted () {
    // this.ShowVisNetWork()
    this.InitTitle()
  },
  methods: {
    init () {
      // this.bCheckAll = false  :disabled='item.healthStatus'
      // this.checkAll()
      this.currentPage = 1
      this.searchResult(1)
    },
    // 改变排序
    changeOrder (sort) {
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.id = 0
        this.headOrder.healthStatus = 0
        this.headOrder.servicePort = 0
        this.headOrder.serviceName = 0
        this.headOrder.serviceAddr = 0
        this.headOrder.servicePID = 0
        this.headOrder.updatedTime = 0
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
      // this.bCheckAll = false
      // this.checkAll()
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
        serviceName: this.serviceName
      }
      api.getPage2(parm).then(res => {
        this.loading = false
        // res.data.rows.map(item => {
        //   // item.updatedTime = transformDate(item.updatedTime)
        //   // item.healthDo = item.healthStatus === 1
        //   item.PrettyMemoryUsed = 1
        // })
        // console.log(res)
        let data = res.data.rows
        this.ShowVisNetWork(data)
        // this.UserList = res.data
        // this.total = res.data.total
      }).catch(err => console.log(err))
    },
    // 搜索功能
    searchRes () {
      this.$emit('title', '| 服务监控')
      this.loading = true
      this.init()
      this.searchResult(1)
    },
    // 获取修改用户id
    emitEditID () {
      this.$emit('lookOperlogID', this.lookOperlogID)
    },
    // 全选
    // checkAll () {
    //   this.bCheckAll ? this.UserList.map(val => this.editUserID.push(val.id)) : this.editUserID = []
    //   this.emitEditID()
    // },
    // 序号、指定页翻页
    handleCurrentChange (val) {
      this.bCheckAll = false
      // this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 上一页
    prevPage (val) {
      this.bCheckAll = false
      // this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },

    // 下一页
    nextPage (val) {
      this.bCheckAll = false
      // this.checkAll()
      this.currentPage = val
      this.searchResult(val)
    },
    refreshpage () {
      // this.$router.go(0)
      this.currentPage = 1
      this.loading = true
      this.searchResult(1)
    },
    ShowVisNetWork (res) {
      // create an array with nodes
      let n2 = res.filter(c => c.ip === '127.0.0.1')[0]
      let n3 = res.filter(c => c.ip === '127.0.0.1')[0]
      let n4 = res.filter(c => c.ip === '127.0.0.1')[0]
      let n5 = res.filter(c => c.ip === '127.0.0.1')[0]
      let retarr = []
      retarr.push({ id: 1, label: 'MBN', fixed: true, x: 0, y: 300, physics: false })
      retarr.push({ id: 2, label: 'Web服务器(' + n2.ip + ')\n(cpu:' + n2.cpuLoad + '%)(mem:' + n2.percentMemoryUsed + '%)(network:' + n2.prettyTotalNetwork + ')(disk:' + n2.diskUsed + '%)', shape: 'image', image: pcimg, fixed: true, x: 0, y: 200, physics: false, font: {color: '#fff'} })
      retarr.push({ id: 3, label: '网关(' + n3.ip + ')\n(cpu:' + n3.cpuLoad + '%)(mem:' + n3.percentMemoryUsed + '%)(network:' + n3.prettyTotalNetwork + ')(disk:' + n3.diskUsed + '%)', shape: 'image', image: pcimg, fixed: true, x: 0, y: 100, physics: false, font: {color: '#fff'} })
      retarr.push({ id: 4, label: '后台服务(' + n4.ip + ')\n(cpu:' + n4.cpuLoad + '%)(mem:' + n4.percentMemoryUsed + '%)(network:' + n4.prettyTotalNetwork + ')(disk:' + n4.diskUsed + '%)', shape: 'image', image: pcimg, fixed: true, x: -300, y: 0, physics: false, font: {color: '#fff'} })
      retarr.push({ id: 5, label: '后台服务(' + n5.ip + ')\n(cpu:' + n5.cpuLoad + '%)(mem:' + n5.percentMemoryUsed + '%)(network:' + n5.prettyTotalNetwork + ')(disk:' + n5.diskUsed + '%)', shape: 'image', image: pcimg, fixed: true, x: 300, y: 0, physics: false, font: {color: '#fff'} })
      // var nodes = new vis.DataSet([
      //   { id: 1, label: 'MBN', fixed: true, x: 0, y: 300, physics: false },
      //   { id: 2, label: 'Web服务器(127.0.0.1)', shape: 'image', image: pcimg, fixed: true, x: 0, y: 200, physics: false, font: {color: '#fff'} },
      //   { id: 3, label: '网关(127.0.0.1)', shape: 'image', image: pcimg, fixed: true, x: 0, y: 100, physics: false, font: {color: '#fff'} },
      //   { id: 4, label: '后台服务(127.0.0.1)', shape: 'image', image: pcimg, fixed: true, x: 0, y: 0, physics: false, font: {color: '#fff'} }
      //   // { id: 5, label: 'MBN' }
      // ])
      var nodes = new vis.DataSet(retarr)

      // create an array with edges
      var edges = new vis.DataSet([
        // { from: 1, to: 3 },
        { from: 1, to: 2 },
        { from: 2, to: 3 },
        { from: 3, to: 4 },
        { from: 3, to: 5 }
        // { from: 3, to: 3 }
      ])

      // create a network
      var container = document.getElementById('mynetwork')
      var data = {
        nodes: nodes,
        edges: edges
      }
      var options = {
        // clickToUse: false,
        interaction: {
          dragView: false,
          zoomView: false
        }
      }
      var network = new vis.Network(container, data, options)// eslint-disable-line no-unused-vars
    },
    InitTitle () {
      let navlist = this.$router.navList
      if (navlist !== undefined) {
        let r = this.$router.navList[this.$route.matched[0].path]
        let rc = r.children
        this.titlepic = this.$router.navList[this.$route.matched[0].path].iconClsActive
        this.title = this.$router.navList[this.$route.matched[0].path].name + ' | ' + rc.filter(item => item.path === this.$route.path)[0].name
        window.sessionStorage.setItem('title', this.title)
        window.sessionStorage.setItem('titlepic', this.titlepic)
      } else {
        this.title = window.sessionStorage.getItem('title')
        this.titlepic = window.sessionStorage.getItem('titlepic')
      }
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
    width: 13%;
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
<style>
#mynetwork {
    width: 97%;
    height: 400px;
    /* border: 1px solid lightgray; */
    position: absolute;
    /* top: 50%; */
    /* left: 15%; */
}
.vis-network { outline: none; }
</style>
