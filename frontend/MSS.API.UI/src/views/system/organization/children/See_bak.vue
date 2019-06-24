<template>
  <div class="wrap height-full">
    <div class="con-padding-horizontal search-wrap">
      <div class="left">部门名称：{{ $route.params.label }}</div>
    </div>
    <ul class="content-header con-padding-horizontal">
      <li class="list number c-pointer" @click="changeOrder('UserID')">
        人员编号
        <i :class="[{ 'el-icon-d-caret': headOrder.UserID === 0 }, { 'el-icon-caret-top': headOrder.UserID === 1 }, { 'el-icon-caret-bottom': headOrder.UserID === 2 }]"></i>
      </li>
      <li class="list name c-pointer" @click="changeOrder('UserName')">
        人员名称
        <i :class="[{ 'el-icon-d-caret': headOrder.UserName === 0 }, { 'el-icon-caret-top': headOrder.UserName === 1 }, { 'el-icon-caret-bottom': headOrder.UserName === 2 }]"></i>
      </li>
      <li class="list gallery" :style="{ width: galleryWidth, flex: 1 }">
        <p>所属管廊</p>
        <ul class="gallery-list-wrap">
          <li class="gallery-list" v-for="item in galleryList" :key="item.key">
            <el-tooltip :content="item" placement="top"><div class="con">{{ item }}</div></el-tooltip>
          </li>
        </ul>
      </li>
    </ul>
    <div class="scroll">
      <el-scrollbar>
        <ul class="list-wrap">
          <li class="list-con con-padding-horizontal" v-for="item in Users" :key="item.key">
            <div class="list">{{ item.UserID }}</div>
            <div class="list">{{ item.UserName }}</div>
            <div class="list gallery" :style="{ width: galleryWidth, flex: 1 }">
              <ul class="gallery-list-wrap">
                <li class="gallery-list" v-for="gallery in item.Tunnels" :key="gallery.key">
                  <i v-if="gallery" class="el-icon-star-on"></i>
                  <span v-else>--</span>
                </li>
              </ul>
            </div>
          </li>
        </ul>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import XButton from '@/components/button'
export default {
  name: 'OrgSee',
  nodeID: '',
  components: {
    XButton
  },
  data () {
    return {
      searchName: '',
      Users: [],
      gallery: [],
      galleryList: [],
      // 所属管廊宽度
      galleryWidth: '60%',
      currentSort: {
        sort: 'UserID',
        order: 'asc'
      },
      headOrder: {
        UserID: 1,
        UserName: 0
      } // 默认UserID升序asc，箭头朝上，同时只可能按照一个字段排序，不排序的字段不出现箭头,0不排序、1升序、2降序，切换时默认升序
    }
  },
  methods: {
    // 改变排序
    changeOrder (sort) {
      // console.log(this.headOrder[sort])
      if (this.headOrder[sort] === 0) { // 不同字段切换时默认升序
        this.headOrder.UserID = 0
        this.headOrder.UserName = 0
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
      this.doSearch()
    },
    doSearch () {
      // 获取组织用户
      window.axios.post('/Organization/GetOrgUsers', {
        order: this.currentSort.order,
        sort: this.currentSort.sort,
        OrgTreeID: this.nodeID
      }).then(res => {
        this.Users = res.data.rows
        this.galleryWidth = `${res.data.tunnels.length * 80}px`
        this.galleryList = res.data.tunnels
      }).catch(err => console.log(err))
    }
  },
  created () {
    // this.$emit('title', '组织架构列表 | 添加')
    this.$emit('pageInfo', {
      title: ' | 组织架构 | 人员配置',
      isShowBackBtn: true,
      url: 'OrgList'
    })
    this.nodeID = this.$route.params.id
    this.doSearch()
    // 测试数据
    // window.axios.post('/UtilityTunnel/GetUtilityTunnel').then(res => (this.galleryList = res.data)).catch(err => console.log(err))
  },
  watch: {
    galleryWidth () {
      let oScrollView = document.querySelector('#use-scroll .el-scrollbar__view')
      let nTotalWidth = oScrollView.offsetWidth + parseFloat(this.galleryWidth)
      nTotalWidth < this.$refs.scrollBar.$el.offsetWidth ? oScrollView.style.width = '100%' : oScrollView.style.width = `${nTotalWidth}px`
    }
  }
}
</script>
<style lang="scss" scoped>
$height: $content-height - 56;
// 搜索组
.search-wrap{
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: percent(80, $height);
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
.content-header,
.list-con{
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: percent(50, $height);
  padding: 0 PXtoEm(24);

  .list{
    width: 10%;
    text-align: center;
    word-break: break-word;
  }
  .white-color{
    color: $color-white;
  }
  .checkbox{
    width: 5%;
  }

  .time{
    width: 15%;
  }
}

.content-header{
  background: rgba(36,128,198,.5);
}

.scroll{
  /**
   * percent函数转换百分比
   * $content-height内容区域总高度
   * 页面标题栏高度：56
   * 查询条件高度：140
   * 操作按钮组：70
   * 表头高度：50
   */
  height: percent($height - 50, $content-height);

  .list-con{
    height: initial;
    padding: {
      top: PXtoEm(15);
      bottom: PXtoEm(15);
    }
    &:nth-of-type(even){
      background: rgba(186,186,186,.5);
    }

    /deep/ .el-checkbox__label{
      display: none;
    }
  }
}

// 所属管廊
.gallery-list-wrap{
  display: flex;
  justify-content: space-between;
  margin-left: 10px;

  .gallery-list{
    width: 80px;
    .con{
      width: 100%;
      @extend %ellipsis;
    }
  }
}
</style>
