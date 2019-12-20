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
            <span class="text">模板编号</span>
            <div class="inp">{{pmModule.code}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">模板名称</span>
            <div class="inp">{{pmModule.name}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">保存部门</span>
            <div class="inp">{{pmModule.department}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">保存年限</span>
            <div class="inp">{{pmModule.deathYear}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">受限级别</span>
            <div class="inp">{{pmModule.levelName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">专业类别</span>
            <div class="inp">{{pmModule.majorName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">线路</span>
            <div class="inp">{{pmModule.lineName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">地点</span>
            <div class="inp">{{pmModule.locationName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">设备设施</span>
            <div class="inp">{{pmModule.deviceName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">关键字</span>
            <div class="inp">{{pmModule.keyWord}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">填写人</span>
            <div class="inp">{{pmModule.createdByName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">填写时间</span>
            <div class="inp">{{pmModule.createdTime}}</div>
          </div>
        </li>
      </ul>
    </div>
    <div style="width:100%">
    <HotTable :settings="settings" licenseKey="non-commercial-and-evaluation" ref="hotTableComponent" v-show="isShow" style="margin-top:30px;"/>
    </div>
  </div>
</template>
<script>
import { transformDate } from '@/common/js/utils.js'
import api from '@/api/workflowApi'
import XButton from '@/components/button'
import { HotTable } from '@handsontable/vue'
import Handsontable from 'handsontable'
export default {
  name: 'DetailMaintenanceList',
  components: {
    XButton,
    HotTable
  },
  data () {
    return {
      settings: {
        data: Handsontable.helper.createSpreadsheetData(500, 50)
      },
      loading: false,
      title: ' | 模板明细',
      pmModule: {},
      isShow: false,
      sourceName: ''
    }
  },
  created () {
    this.getDetail(this.$route.params.id)
    this.sourceName = this.$route.params.sourceName
  },
  methods: {
    back () {
      this.$router.push({
        name: this.sourceName
      })
    },
    getDetail (id) {
      this.loading = true
      api.getModuleByID(id).then(res => {
        this.settings = {
          readOnly: true,
          data: res.data.data,
          tableClassName: ['htMiddle', 'htCenter'],
          width: '100%',
          height: 500,
          mergeCells: res.data.mergeCells,
          cells: function (row, col) {
            var cellProperties = {}
            if (col === 0) {
              cellProperties.width = 50
            }
            return cellProperties
          }
        }
        let _data = res.data.obj
        this.pmModule = {
          code: _data.code,
          name: _data.name,
          majorName: _data.majorName,
          lineName: _data.lineName,
          deviceName: _data.deviceName,
          keyWord: _data.keyWord,
          department: _data.department,
          deathYear: _data.deathYear,
          levelName: _data.levelName,
          locationName: _data.locationName,
          createdTime: transformDate(_data.createdTime),
          createdByName: _data.createdByName
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
