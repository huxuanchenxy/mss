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
            <label for="">设备类型</label>
            <div class="inp">
              <el-select v-model="eqpType" filterable placeholder="请选择" @change="eqpTypeChange">
                <el-option
                  v-for="item in eqpTypeList"
                  :key="item.key"
                  :label="item.tName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
          <div class="input-group">
            <label for="">设备</label>
            <div class="inp">
              <el-cascader class="cascader_width"
                expand-trigger="hover"
                :props="defaultParams"
                :show-all-levels="false"
                :options="eqpList"
                v-model="eqpSelected">
              </el-cascader>
            </div>
          </div>
        </div>
        <div class="search-btn">
          <i @click="searchRes"><x-button ><i class="iconfont icon-search"></i> 查询</x-button></i>
          <i @click="eqpDetail"><x-button ><i class="iconfont el-icon-more"></i> 设备明细</x-button></i>
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
                    <div class="secondContent" v-show="item.detailType === 1" @click="detail(data.id, 1)">{{"施工申请单"+data.id}}</div>
                    <div class="secondContent" v-show="item.detailType === 2" @click="detail(data.id, 2)">{{"故障报告单"+data.id}}</div>
                    <div class="secondContent" v-show="item.detailType === 0" @click="preview(data)">{{data.name}}</div>
                  </div>
                </el-collapse-item>
              </el-collapse>
              <div class='myContentOnly' v-show="item.children === null || item.children.length === 0">{{item.content}}</div>
            </template>
          </light-timeline>
        </el-scrollbar>
      </div>
    </div>
    <el-dialog
      :visible.sync="centerDialogVisible"
      :modal-append-to-body="false"
      custom-class="show-list-wrap"
      center>
      <iframe :src="previewUrl" width="100%" height="100%" frameborder="0"></iframe>
    </el-dialog>
  </div>
</template>
<script>
import XButton from '@/components/button'
import { PDF_UPLOADED_VIEW_URL } from '@/common/js/utils.js'
import { isPreview } from '@/common/js/UpDownloadFileHelper.js'
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
      defaultParams: {
        label: 'name',
        value: 'id',
        children: 'children'
      },
      eqpTypeName: '',
      items: [],
      centerDialogVisible: false,
      previewUrl: '',
      eqpType: '',
      eqpTypeList: [],
      eqp: '',
      eqpSelected: [],
      eqpList: []
    }
  },
  created () {
    // this.loading = true
    // 设备类型加载
    apiEqp.getEqpTypeAll().then(res => {
      this.eqpTypeList = res.data
      this.eqpType = this.eqpTypeList[0].id
      // 设备加载
      api.GetDeviceListByTypeId(this.eqpType).then(res => {
        this.eqpList = res.data
        // this.eqpSelected.push(this.eqpList[0].id)
        // this.eqp = this.getDefaultEqp(this.eqpList[0])
        // this.searchRes()
      }).catch(err => console.log(err))
    }).catch(err => console.log(err))
  },
  activated () {
    debugger
    if ('eqpSelected' in this.$route.params) {
      this.eqpSelected = this.$route.params.eqpSelected
      this.eqpType = this.$route.params.eqpType
      this.searchRes()
    }
  },
  methods: {
    eqpTypeChange () {
      api.GetDeviceListByTypeId(this.eqpType).then(res => {
        this.eqpList = res.data
      }).catch(err => console.log(err))
    },
    getDefaultEqp (list) {
      let tmp = list.children[0]
      this.eqpSelected.push(tmp.id)
      if (tmp.children === null) return tmp.id
      else return this.getDefaultEqp(tmp)
    },
    searchRes () {
      this.eqp = this.eqpSelected[this.eqpSelected.length - 1]
      api.ListByEqp(this.eqp).then(res => {
        apiEqp.getUploadFileByEqp(this.eqp).then(ret => {
          let tmp = [{
            tag: '技术资料',
            stage: 1,
            content: '无',
            children: null
          }]
          if (ret.data !== null && res.data !== null) this.items = ret.data.concat(res.data)
          else if (ret.data !== null && res.data === null) this.items = ret.data
          else if (ret.data === null && res.data !== null) this.items = tmp.concat(res.data)
          else this.items = tmp
          this.loading = false
        }).catch(err => console.log(err))
      }).catch(err => console.log(err))
    },
    detail (id, detailType) {
      switch (detailType) {
        case 1:
          this.$router.push({
            name: 'DetailWorkingApplication',
            params: {
              id: id
            }
          })
          break
        case 2:
          this.$router.push({
            name: 'DetailTroubleReport',
            params: {
              id: id
            }
          })
          break
      }
    },
    preview (item) {
      if (isPreview(item.id, item.name)) {
        this.centerDialogVisible = true
        this.previewUrl = PDF_UPLOADED_VIEW_URL + item.url
      }
    },
    eqpDetail () {
      if (this.eqpSelected.length === 0) {
        this.$message({
          message: '请选择需要查看的设备',
          type: 'warning'
        })
      } else {
        this.eqp = this.eqpSelected[this.eqpSelected.length - 1]
        this.$router.push({
          name: 'DetailEqp',
          params: {
            id: this.eqp,
            eqpSelected: this.eqpSelected,
            sourceName: 'SeeHistory'
          }
        })
      }
    }
  }
}
</script>
<style lang="scss" scoped>
$con-height: $content-height - 145 - 56 + 64;
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
    margin-top: 11px;
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
