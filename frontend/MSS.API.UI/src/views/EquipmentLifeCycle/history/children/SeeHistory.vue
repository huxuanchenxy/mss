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
            <label for="name">设备类型名称</label>
            <div class="inp">
              <el-input v-model.trim="eqpTypeName" placeholder="请输入设备类型名称"></el-input>
            </div>
          </div>
        </div>
        <div class="search-btn" @click="searchRes">
          <x-button ><i class="iconfont icon-search"></i> 查询</x-button>
        </div>
      </div>
    </div>
    <!-- 内容 -->
    <!--<div class="content-wrap">
      <light-timeline :items='items'></light-timeline>
    </div>-->
    <div class="content-wrap">
      <div class="scroll">
        <el-scrollbar>
          <light-timeline :items='items' class="light-timeline">
            <template slot='tag' slot-scope='{ item }'>
              <div class='myTag'>{{item.tag}}</div>
            </template>
            <template slot='symbol' slot-scope='{ item }'>
              <div class='mySymbol' v-show="item.stage === 2"></div>
              <div class='mySymbol first' v-show="item.stage === 1"></div>
              <div class='mySymbol last' v-show="item.stage === 3"></div>
              <div v-show="item.stage === 0"></div>
            </template>
            <template slot='content' slot-scope='{ item }'>
              <el-collapse class='myContent' v-show="item.children">
                <el-collapse-item :title="item.content">
                  <div v-for="data in item.children" :key="data.key">
                    <div class="secondContent" v-show="!item.isFile" @click="detail(data.id)">{{"施工单"+data.id}}</div>
                    <div class="secondContent" v-show="item.isFile" @click="preview(data)">{{data.name}}</div>
                  </div>
                </el-collapse-item>
              </el-collapse>
              <div class='myContentOnly' v-show="!item.children">{{item.content}}</div>
            </template>
          </light-timeline>
        </el-scrollbar>
      </div>
    </div>
  </div>
</template>
<script>
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
import apiEqp from '@/api/eqpApi'
export default {
  name: 'SeeHistory',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      title: ' | 设备履历',
      eqpTypeName: '',
      items: []
    }
  },
  created () {
    this.searchRes()
  },
  methods: {
    searchRes () {
      api.ListByEqp(1).then(res => {
        apiEqp.getUploadFileByEqp(1).then(ret => {
          console.log(ret.data)
          this.items = ret.data.concat(res.data)
          console.log(this.items)
        }).catch(err => console.log(err))
      }).catch(err => console.log(err))
    },
    detail (id) {
      console.log(id)
    },
    preview (data) {
      console.log(data)
    }
  }
}
</script>
<style lang="scss" scoped>
$con-height: $content-height - 145 - 56 + 80;
// 内容区
.content-wrap{
  overflow: hidden;
  height: percent($con-height, $content-height);
  // text-align: center;
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
    height: percent($con-height, $con-height);
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
}

.light-timeline{
  margin-left: 20%;
  font-size: 16px;
  color: #FFFFFF;
  .myTag{
    margin-top: 10px;
    color: #FFFFFF;
    font-size: 16px;
    width: 150px;
  }
  .mySymbol{
    margin-top: 13px;
    margin-left: 47px;
    width: 16px;
    height: 16px;
    background-color: blue;
    border-radius: 50%;
    border: 2px solid
  }
  .myContent{
    margin-left: 50px;
    width: 400px;
    .secondContent{
      font-size: 16px;
      cursor: pointer;
      text-decoration:underline;
    }
  }
  .myContentOnly{
    margin-left: 74px;
  }
}
.line-container::after{
  margin-left: 57px;
  z-index: -99;
}

/deep/
.line-container .item-symbol{
  // position: absolute;
  // z-index: -1;
  // left: -1rem;
  // margin-top: 4px;
  // margin-left: 49px;
  width: 0px;
  height: 0px;
  // background-color: blue;
  // border-radius: 50%;
  // border: 2px solid;
}
.first{
  background-color: green!important;
}
.last{
  background-color: red!important;
}
/deep/
.el-collapse .el-collapse-item__content{
  padding-bottom: 0px;
}
</style>
