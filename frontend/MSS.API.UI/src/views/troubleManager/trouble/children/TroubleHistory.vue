<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <i @click="back"><x-button class="active">返回</x-button></i>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list last-update-time">操作时间</li>
        <li class="list name">操作步骤</li>
        <li class="list last-maintainer">操作描述/驳回原因</li>
        <li class="list name">操作人</li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in troubleHistoryList" :key="item.key">
              <div class="list-content">
                <div class="last-update-time color-white">{{ item.createdTime }}</div>
                <div class="name">{{ item.operationName }}</div>
                <div class="last-maintainer color-white" v-show="item.content.length > 40">
                {{item.content === null ? '' : item.content.slice(0, 20) + ' ......'}}
                <el-popover
                  popper-class="my-pop"
                  placement="bottom"
                  width="860">
                  <p class="update-content color-white">{{item.content}}</p>
                  <el-button class="btn1" slot="reference" >完整内容</el-button>
                </el-popover>
                </div>
                <div class="last-maintainer color-white" v-show="item.content.length <= 40">{{item.content}}</div>
                <div class="name">{{ item.createdByName }}</div>
              </div>
            </li>
          </ul>
        </el-scrollbar>
      </div>
    </div>
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import { troubleOperation } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
export default {
  name: 'TroubleHistory',
  components: {
    XButton
  },
  data () {
    return {
      title: ' | 故障操作历史-',
      updateOperation: '',
      troubleID: '',
      sourceName: '',
      troubleHistoryList: [],
      total: 0,
      currentPage: 1,
      loading: false,
      currentSort: {
        sort: 'created_time',
        order: 'asc'
      },
      headOrder: {
        created_time: 1
      }
    }
  },
  created () {
    this.troubleID = this.$route.params.id
    this.title = ' | 故障操作历史-' + this.$route.params.code
    this.sourceName = this.$route.params.sourceName
    this.updateOperation = troubleOperation.updateTrouble
    this.init()
  },
  methods: {
    init () {
      this.currentPage = 1
      this.searchResult(1)
    },
    back () {
      this.$router.push({
        name: this.sourceName
      })
    },
    // 搜索
    searchResult (page) {
      // this.currentPage = page
      // this.loading = true
      // let st, et
      // if (this.searchDate != null && this.searchDate.length !== 0) {
      //   st = this.searchDate[0] + ' 00:00:00'
      //   et = this.searchDate[1] + ' 23:59:59'
      // }
      // let parm = {
      //   order: this.currentSort.order,
      //   sort: this.currentSort.sort,
      //   rows: 10,
      //   page: page,
      //   TroubleReportDesc: this.desc,
      //   TroubleStatus: this.troubleStatus,
      //   StartTime: st,
      //   EndTime: et
      // }
      api.getTroubleHistoryByID(this.troubleID).then(res => {
        this.loading = false
        res.data.map(item => {
          item.createdTime = transformDate(item.createdTime)
        })
        this.troubleHistoryList = res.data
        // this.total = res.data.total
      }).catch(err => console.log(err))
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

  .number{
    width: 6%;
  }
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
    width: 25%;
  }

  .state{
    width: 5%;
  }
}
/deep/
.btn1{
  background: none;
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
</style>
<style>
.my-pop{
  background: #29282E;
  border-color: #29282E;
}
.update-content{
  word-break: break-all;
}
</style>
