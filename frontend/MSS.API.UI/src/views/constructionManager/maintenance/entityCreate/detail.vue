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
      <i @click="back"><x-button class="active">返回</x-button></i>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">检修单名称</span>
            <div class="inp">{{entity.title}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">班组</span>
            <div class="inp">{{entity.teamName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">地点</span>
            <div class="inp">{{entity.locationName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">状态</span>
            <div class="inp">{{entity.statusName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">最后保存/提交人</span>
            <div class="inp">{{entity.updatedByName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">最后保存/提交时间</span>
            <div class="inp">{{entity.updatedTime}}</div>
          </div>
        </li>
      </ul>
    </div>
    <HotTable :settings="settings" licenseKey="non-commercial-and-evaluation" ref="hotTableComponent" v-show="isShow" style="margin-top:30px"/>
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import api from '@/api/workflowApi'
import XButton from '@/components/button'
import { HotTable } from '@handsontable/vue'
// import Handsontable from 'handsontable'
export default {
  name: 'DetailEntity',
  components: {
    XButton,
    HotTable
  },
  data () {
    return {
      settings: {},
      loading: false,
      title: ' | 检修单明细',
      entity: {},
      eqpLifeHistory: {
        eqpSelected: [],
        eqpType: ''
      },
      isShow: false,
      sourceName: ''
    }
  },
  created () {
    this.getDetail(this.$route.params.id)
    this.sourceName = this.$route.params.sourceName
    if (this.sourceName === 'SeeHistory') {
      this.eqpLifeHistory.eqpSelected = this.$route.params.eqpSelected
      this.eqpLifeHistory.eqpType = this.$route.params.eqpType
    }
  },
  methods: {
    back () {
      if (this.sourceName === 'SeeHistory') {
        this.$router.push({
          name: this.sourceName,
          params: {
            eqpSelected: this.eqpLifeHistory.eqpSelected,
            eqpType: this.eqpLifeHistory.eqpType
          }
        })
      } else {
        this.$router.push({
          name: this.sourceName
        })
      }
    },
    getDetail (id) {
      this.loading = true
      api.getEntityByID(id, false).then(res => {
        this.settings = {
          readOnly: true,
          data: res.data.showData,
          tableClassName: ['htMiddle', 'htCenter'],
          width: '100%',
          height: 500,
          mergeCells: res.data.showMerge,
          cells: function (row, col) {
            var cellProperties = {}
            if (col === 0) {
              cellProperties.width = 50
            }
            return cellProperties
          }
        }
        let _data = res.data.entity
        this.entity = {
          title: _data.title,
          teamName: _data.teamName,
          locationName: _data.locationName,
          statusName: _data.statusName,
          updatedTime: transformDate(_data.updatedTime),
          updatedByName: _data.updatedByName
        }
        this.loading = false
        this.isShow = true
      }).catch(err => console.log(err))
    }
  }
}
</script>
<style lang="scss" scoped>
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
.cascader-width{
  width: 220px!important;
}
.cascader_width{
  width: 100%!important;
}
</style>
