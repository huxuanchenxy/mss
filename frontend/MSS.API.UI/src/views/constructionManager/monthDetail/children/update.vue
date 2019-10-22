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
        <router-link :to="{name:'MonthList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">设备名称</span>
            <div class="inp">{{monthDetail.eqpTypeName}}</div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">设备地点</span>
            <div class="inp">{{monthDetail.locationName}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">管辖班组</span>
            <div class="inp">{{monthDetail.teamName}}</div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">维护频次</span>
            <div class="inp">{{monthDetail.pmFrequency}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">数量</span>
            <div class="inp">{{monthDetail.planQuantity}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">单位</span>
            <div class="inp">{{monthDetail.unit}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">计划时间</span>
            <div class="inp">{{monthDetail.planDate}}</div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">工作类型</span>
            <div class="inp">
              <el-select v-model="workType">
                <el-option
                  v-for="item in workTypeList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">维护类型</span>
            <div class="inp">
              <el-select v-model="pmType">
                <el-option
                  v-for="item in pmTypeList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
        </li>
      </ul>
    </div>
    <div class="btn-enter">
      <x-button class="close">
        <router-link :to="{ name: 'MonthList' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="enter">修改</x-button>
    </div>
  </div>
</template>
<script>
import { dictionary } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import apiAuth from '@/api/authApi'
import api from '@/api/workflowApi'
export default {
  name: 'UpdateMonthPlan',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      title: '',
      editID: '',
      workType: '',
      workTypeList: [],
      pmType: '',
      pmTypeList: [],
      monthDetail: {
        eqpTypeName: '',
        locationName: '',
        teamName: '',
        pmFrequency: '',
        planQuantity: '',
        unit: '',
        planDate: '',
        workType: '',
        pmType: ''
      }
    }
  },
  created () {
    this.title = this.$route.params.title
    this.editID = this.$route.params.id
    this.monthDetail.locationName = this.$route.params.locationName
    // 工作类型
    apiAuth.getSubCode(dictionary.workType).then(res => {
      this.workTypeList = res.data
      // 维护类型
      apiAuth.getSubCode(dictionary.pmType).then(res => {
        this.pmTypeList = res.data
        this.getMonthDetail()
      }).catch(err => console.log(err))
    }).catch(err => console.log(err))
  },
  methods: {
    enter () {
      let detail = {
        id: this.editID,
        workType: this.workType,
        pmType: this.pmType
      }
      api.updateMonthPlan(detail).then(res => {
        if (res.code === 0) {
          this.$message({
            message: '修改成功',
            type: 'success',
            onClose: () => {
              this.$router.push({
                name: 'MonthList'
              })
            }
          })
        } else {
          this.$message({
            message: res.msg,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    // 修改权限组时获取权限组资料
    getMonthDetail () {
      api.getMonthPlanDetailByID(this.editID).then(res => {
        this.loading = false
        let _res = res.data
        this.monthDetail.eqpTypeName = _res.eqpTypeName
        // this.monthDetail.locationName = _res.locationName
        this.monthDetail.teamName = _res.teamName
        this.monthDetail.pmFrequency = _res.pmFrequency
        this.monthDetail.planQuantity = _res.planQuantity
        this.monthDetail.unit = _res.unit
        this.monthDetail.planDate = _res.planDate
        this.workType = _res.workType
        this.pmType = _res.pmType
        console.log(this.monthDetail)
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
</style>
