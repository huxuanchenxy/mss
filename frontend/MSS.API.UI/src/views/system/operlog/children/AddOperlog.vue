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
        <router-link :to="{name:'SeeOperlogList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">模块名称</span>
            <div class="inp">
              <el-input v-model="controller_name" :disabled="isShow === 'look'" ></el-input>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">操作名称</span>
            <div class="inp">
              <el-input v-model="method_name" :disabled="isShow === 'look'" ></el-input>
            </div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">用户姓名</span>
            <div class="inp">
              <el-input v-model="user_name" :disabled="isShow === 'look'" ></el-input>
            </div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">用户账号</span>
            <div class="inp">
              <el-input v-model="acc_name" :disabled="isShow === 'look'" ></el-input>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">ip</span>
            <div class="inp">
              <el-input v-model="ip" :disabled="isShow === 'look'" ></el-input>
            </div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">操作时间</span>
            <div class="inp">
              <el-input v-model="created_time" :disabled="isShow === 'look'" ></el-input>
            </div>
          </div>
        </li>
      </ul>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list1">
          <div class="inp-wrap">
            <span class="text">请求详情</span>
            <div class="inp" style="width:86.5%">
              <el-input type="textarea" v-model="request_description" :disabled="isShow === 'look'" ></el-input>
            </div>
          </div>
        </li>
      </ul>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list1">
          <div class="inp-wrap">
            <span class="text">响应详情</span>
            <div class="inp" style="width:86.5%;">
              <el-input type="textarea" v-model="response_description" :disabled="isShow === 'look'" :rows="7" ></el-input>
            </div>
          </div>
        </li>
      </ul>
    </div>
    <!-- <div class="btn-enter">
      <x-button class="close">
        <router-link :to="{ name: 'SeeActionGroupList' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="enter">保存</x-button>
    </div> -->
  </div>
</template>
<script>
import { validateInputCommon, validateNumberCommon, vInput, transformDate } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/operlogApi'
export default {
  name: 'AddOperlog',
  components: {
    XButton
  },
  data () {
    return {
      loading: true,
      isShow: this.$route.params.mark,
      editOperlogID: this.$route.params.id,
      acc_name: '',
      action_name: '',
      controller_name: '',
      created_time: '',
      ip: '',
      method_name: '',
      request_description: '',
      response_description: '',
      user_name: ''
    }
  },
  created () {
    if (this.isShow === 'look') {
      this.loading = true
      this.title = '| 查看日志详情'
      this.getOperlogByID()
      // this.$emit('title', '| 添加权限组')
      // this.btnText = '确认'
    } else if (this.isShow === 'edit') {
      // this.loading = true
    }
  },
  methods: {
    // 添加权限组
    enter () {
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      let actionGroup = {
        group_name: this.groupName.text,
        group_type: this.groupType.text,
        group_order: this.groupOrder.text,
        request_url: this.groupUrl.text,
        icon: this.groupIcon.text,
        active_icon: this.titleIcon.text
      }
      if (this.isShow === 'add') {
        // 添加权限组
        api.addActionGroup(actionGroup).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeActionGroupList'
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
      } else if (this.isShow === 'edit') {
        actionGroup.id = this.actionGroupID
        // 修改权限组
        api.updateActionGroup(actionGroup).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeActionGroupList'
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
    getOperlogByID () {
      api.getOperationLogByID(this.editOperlogID).then(res => {
        this.loading = false
        let _res = res.data
        // console.log(_res)
        this.acc_name = _res.acc_name
        this.action_name = _res.action_name
        this.controller_name = _res.controller_name
        this.controller_name = _res.controller_name
        this.created_time = transformDate(_res.created_time)
        this.ip = _res.ip
        this.method_name = _res.method_name
        this.request_description = _res.request_description
        this.response_description = _res.response_description
        this.user_name = _res.user_name
      }).catch(err => console.log(err))
    },
    // 验证
    validateInput (val) {
      validateInputCommon(val)
    },

    // 验证非法字符串，但允许为空
    validateInputNull (val) {
      if (!vInput(val.text)) {
        val.tips = '此项含有非法字符'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateSelect () {
      if (this.groupType.text === '') {
        this.groupType.tips = '此项必选'
        return false
      } else {
        this.groupType.tips = ''
        return true
      }
    },

    validateNumber () {
      validateNumberCommon(this.groupOrder)
    },

    validateAll () {
      if (!validateInputCommon(this.groupName)) return false
      if (!validateInputCommon(this.groupUrl)) return false
      if (!validateNumberCommon(this.groupOrder)) return false
      if (!this.validateInputNull(this.groupIcon)) return false
      if (!this.validateInputNull(this.titleIcon)) return false
      if (!this.validateSelect()) return false
      return true
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

      .list1{
      width: 100%;
      margin-top: PXtoEm(25);

      span{
        width: 8.3%;
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

#responsearea{
    min-height: 93px;
}
</style>
