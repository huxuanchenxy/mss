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
        <router-link :to="{name:'ImportList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">年份<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-date-picker
                v-model="yearCommon.year"
                type="year"
                value-format="yyyy"
                placeholder="选择年">
              </el-date-picker>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">公司</span>
            <div class="inp">
              <el-cascader clearable
                class="cascader-width"
                change-on-select
                :props="defaultParams"
                @change="cascader_change"
                :show-all-levels="true"
                :options="companyList"
                v-model="companyPath">
              </el-cascader>
            </div>
          </div>
        </li>
        <li class="list" />
      </ul>
    </div>
    <div class="btn-enter">
      <!--:headers="uploadHeaders"-->
      <!--http://127.0.0.1:5801/workflowApi/ConstructionPlanImport-->
      <!--http://localhost:3851/api/v1/ConstructionPlanImport-->
      <el-upload
        action="http://127.0.0.1:5801/workflowApi/ConstructionPlanImport"
        :headers="uploadHeaders"
        :multiple="false"
        :data="yearCommon"
        accept=".xls,.xlsx"
        :show-file-list="false"
        :before-upload="beforeUpload"
        :on-success="onSuccess">
        <el-button size="small" type="primary" class="import-btn" :loading="loading">导入计划</el-button>
      </el-upload>
    </div>
  </div>
</template>
<script>
import { getCascaderObj } from '@/common/js/utils.js'
import XButton from '@/components/button'
// import api from '@/api/authApi'
import apiOrg from '@/api/orgApi'
export default {
  name: 'Import',
  components: {
    XButton
  },
  data () {
    return {
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      loading: false,
      title: ' | 导入',
      uploadHeaders: {Authorization: ''},
      yearCommon: {
        year: '2019',
        company: '申通'
      },
      companyList: [],
      companyPath: []
    }
  },
  created () {
    // 公司加载
    apiOrg.getOrgAll().then(res => {
      this.companyList = res.data
    }).catch(err => console.log(err))
  },
  mounted () {
    let token = window.sessionStorage.getItem('token')
    if (token) { // 判断是否存在token，如果存在的话，则每个http header都加上token
      this.uploadHeaders.Authorization = `Bearer ${token}`
    }
  },
  methods: {
    cascader_change (val) {
      let selectedTeam = val[val.length - 1]
      let obj = getCascaderObj(selectedTeam, this.companyList)
      if (obj.node_type === 1) {
        this.yearCommon.company = selectedTeam
      } else {
        this.$message({
          message: '您选择的不是公司',
          type: 'error'
        })
      }
    },
    beforeUpload (file) {
      if (file.size > 52428800) {
        this.$message({
          message: '单个文件不允许超过50M',
          type: 'warning'
        })
        return false
      }
      if (this.yearCommon.year === '' || this.yearCommon.company === '') {
        this.$message({
          message: '请选择年份和公司',
          type: 'error'
        })
        return false
      }
      let tmp = file.name.split('.')
      let myExt = tmp[tmp.length - 1]
      if (myExt !== 'xls' && myExt !== 'xlsx') {
        this.$message({
          message: '不支持非excel的文件类型',
          type: 'warning'
        })
        return false
      }
      // this.loading = true
    },
    onSuccess (response, file, fileList) {
      if (response.code === 0) {
        this.$message({
          message: '导入成功',
          type: 'success'
        })
      } else {
        this.$message({
          message: response.msg,
          type: 'error'
        })
      }
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
</style>
