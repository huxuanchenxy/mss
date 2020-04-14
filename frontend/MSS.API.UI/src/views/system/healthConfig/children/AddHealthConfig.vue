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
        <router-link :to="{name:'SeeHealthConfig'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation" v-show="isTrouble">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">健康度配置类型</span>
            <div class="inp">{{healthConfig.typeName}}</div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">设备类型</span>
            <div class="inp">
              <el-select v-model="eqpType" clearable filterable placeholder="请选择" @change="getHealthConfig">
                <el-option
                  v-for="item in eqpTypeList"
                  :key="item.key"
                  :label="item.tName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
        </li>
        <li class="list" />
        <li class="list">
          <div class="inp-wrap">
            <span class="text">故障等级</span>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">配置值</span>
          </div>
        </li>
        <li class="list" />
      </ul>
      <ul class="input-group" v-for="(item) in troubleLevelList" :key="item.key">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">{{item.troubleLevelName}}</span>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">
              <el-input placeholder="请输入数值" v-model="item.val"></el-input>
            </span>
          </div>
        </li>
        <li class="list" />
      </ul>
      <div style="margin-top:30px;">{{'备注：' + remark}}</div>
    </div>
    <div class="con-padding-horizontal operation" v-show="!isTrouble">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">健康度配置类型</span>
            <div class="inp">{{healthConfig.typeName}}</div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text" v-if="healthType === constType.eqpReplace ">配置值<em class="validate-mark">*</em></span>
            <span class="text" v-else>配置值(%)<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入数值" v-model="healthConfig.val"></el-input>
            </div>
          </div>
        </li>
        <li class="list" />
        <li style="margin-top:30px">{{'备注：' + remark}}</li>
      </ul>
    </div>
    <div class="btn-enter">
      <x-button class="close">
        <router-link :to="{ name: 'SeeHealthConfig' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="del">删除</x-button>
      <x-button class="active" @click.native="enter">修改</x-button>
    </div>
    <el-dialog
      :visible.sync="dialogVisible.isShow"
      :modal-append-to-body="false"
      :show-close="false">
      {{ dialogVisible.text }}
      <template slot="footer" class="dialog-footer">
        <template v-if="dialogVisible.btn">
          <el-button @click="dialogVisible.isShow = false">否</el-button>
          <el-button @click="dialogEnter">是</el-button>
        </template>
        <el-button v-else @click="dialogVisible.isShow = false" :class="{ on: !dialogVisible.btn }">知道了</el-button>
      </template>
    </el-dialog>
  </div>
</template>
<script>
import { vdouble2 } from '@/common/js/utils.js'
import { healthType } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
import apiEqp from '@/api/eqpApi'
export default {
  name: 'AddHealthConfig',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      eqpType: this.$route.params.mark,
      healthType: this.$route.params.id,
      healthID: 0,
      eqpTypeList: [],
      isTrouble: false,
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      },
      remark: '',
      healthConfig: {
        typeName: '',
        eqpType: '',
        val: 0
      },
      troubleLevelList: [],
      constType: ''
    }
  },
  created () {
    this.loading = true
    this.title = '| 修改健康度配置'
    this.constType = healthType
    if (this.healthType === healthType.trouble) {
      this.eqpType = this.eqpType === 0 ? 1 : this.eqpType
      // 设备类型加载
      apiEqp.getEqpTypeAll().then(res => {
        this.eqpTypeList = res.data
      }).catch(err => console.log(err))
      this.isTrouble = true
    }
    this.getHealthConfig()
  },
  methods: {
    isDouble (val) {
      if (!vdouble2(val)) {
        this.$message({
          message: '请填写最多两位小数的数字',
          type: 'error'
        })
        return false
      }
      return true
    },
    // 添加权限组
    enter () {
      if (this.healthType !== healthType.trouble) {
        if (this.isDouble(this.healthConfig.val)) {
          let hconfig = {
            id: this.healthID,
            val: this.healthConfig.val,
            type: this.healthType
          }
          api.updateHealthConfig(hconfig).then(res => {
            if (res.code === 0) {
              this.$message({
                message: '修改成功',
                type: 'success',
                onClose: () => {
                  this.$router.push({
                    name: 'SeeHealthConfig'
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
        }
      } else {
        let list = []
        this.troubleLevelList.map(item => {
          list.push({
            type: this.healthType,
            eqpType: this.eqpType,
            troubleLevel: item.troubleLevel,
            val: item.val
          })
        })
        api.addHealthConfig(list).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeHealthConfig'
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
      }
    },
    // 修改权限组时获取权限组资料
    getHealthConfig () {
      let parm = this.healthType
      if (this.eqpType !== 0) {
        parm += ',' + this.eqpType
        api.getHealthConfigByTroubleLevel(parm).then(res => {
          this.loading = false
          this.healthConfig.typeName = res.data.typeName
          this.remark = res.data.des
          this.troubleLevelList = []
          res.data.data.map(item => {
            let myVal = 0
            let myID = 0
            if (item.val !== null) {
              myVal = item.val
              myID = item.id
            }
            this.troubleLevelList.push({
              id: myID,
              val: myVal,
              troubleLevel: item.troubleLevel,
              troubleLevelName: item.troubleLevelName
            })
          })
        }).catch(err => console.log(err))
      } else {
        api.getHealthConfigByType(parm).then(res => {
          this.loading = false
          let _res = res.data
          this.healthConfig.typeName = _res.typeName
          this.remark = _res.des
          if (_res.val === null) {
            this.healthConfig.val = 0
          } else {
            this.healthConfig.val = _res.val
            this.healthID = _res.id
          }
        }).catch(err => console.log(err))
      }
    },
    del () {
      this.dialogVisible.isShow = true
      this.dialogVisible.btn = true
      this.dialogVisible.text = '确定删除该配置?'
    },
    dialogEnter () {
      let parm = this.healthType
      if (this.$route.params.mark !== 0) parm += ',' + this.$route.params.mark
      api.delHealthConfig(parm).then(res => {
        if (res.code === 0) {
          this.$message({
            message: '删除成功',
            type: 'success',
            onClose: () => {
              this.$router.push({
                name: 'SeeHealthConfig'
              })
            }
          })
        } else {
          this.$message({
            message: res.msg,
            type: 'error'
          })
        }
        // 隐藏dialog
        this.dialogVisible.isShow = false
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

      // &:nth-of-type(3n+1){
      //   // justify-content: flex-start;
      // }

      // &:nth-of-type(3n){
      //   // justify-content: flex-end;
      // }
    }
  }
}
.btn-enter{
  margin: 25px 0;
  text-align: center;

  button{
    border-color: $color-main-btn;
    background: $color-main-btn;
  }
}
</style>
