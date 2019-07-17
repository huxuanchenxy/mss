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
        <router-link :to="{name:'SeeActionList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list" v-show="false">
          <div class="inp-wrap">
            <span class="text">权限ID</span>
            <div class="inp">
              <el-input v-model="actionID" :disabled="isShow === 'edit'"></el-input>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">权限名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model="actionName.text" placeholder="请输入权限名称" @keyup.native="validateInput(actionName)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ actionName.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">权限URL<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model="actionUrl.text" placeholder="请输入权限URL" @keyup.native="validateInput(actionUrl)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ actionUrl.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">权限组</span>
            <div class="inp">
              <el-select v-model="actionGroup" clearable placeholder="请选择">
                <el-option v-for="item in actionGroupList" :key="item.key" :value="item.id" :label="item.group_name"></el-option>
              </el-select>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">所属菜单</span>
            <div class="inp">
              <el-select v-model="parentMenu" clearable placeholder="请选择">
                <el-option v-for="item in parentMenuList" :key="item.key" :value="item.id" :label="item.action_name"></el-option>
              </el-select>
            </div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">权限顺序</span>
            <div class="inp">
              <el-input v-model="menuOrder.text" placeholder="请输入权限顺序" @keyup.native="validateNumber()"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ menuOrder.tips }}</p>
        </li>
        <li class="list"></li>
      </ul>
    </div>
    <div class="btn-group">
      <x-button class="close">
        <router-link :to="{ name: 'SeeActionList' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="enter">保存</x-button>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, validateNumberCommon } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/authApi'
export default {
  name: 'AddActionGroup',
  components: {
    XButton
  },
  data () {
    return {
      title: '',
      loading: true,
      isShow: this.$route.params.mark,
      editActionID: this.$route.params.id,
      btnText: '',
      actionID: '',
      actionName: {
        text: '',
        tips: ''
      },
      actionUrl: {
        text: '',
        tips: ''
      },
      actionGroup: '',
      actionGroupList: [],
      menuOrder: {
        text: '',
        tips: ''
      },
      parentMenu: '',
      parentMenuList: []
    }
  },
  created () {
    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 添加权限'
      // this.$emit('title', '| 添加权限')
      // this.btnText = '确认'
    } else if (this.isShow === 'edit') {
      this.loading = true
      this.title = '| 修改权限'
      // this.$emit('title', '| 修改权限')
      // this.btnText = '保存'
      this.getAction()
    }
    // 权限组列表
    api.getActionGroupAll().then(res => {
      this.actionGroupList = res.data
    }).catch(err => console.log(err))
    // 菜单组列表
    api.getActionMenu().then(res => {
      this.parentMenuList = res.data
    }).catch(err => console.log(err))
  },
  methods: {
    // 添加权限
    enter () {
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      let action = {
        action_name: this.actionName.text,
        group_id: this.actionGroup,
        action_order: this.menuOrder.text,
        request_url: this.actionUrl.text,
        parent_menu: this.parentMenu
      }
      if (this.isShow === 'add') {
        // 添加权限
        api.addAction(action).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '添加成功',
              type: 'success'
              // onClose: () => {
              //   this.$router.push({
              //     name: 'SeeActionList'
              //   })
              // }
            })
          } else {
            this.$message({
              message: res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else if (this.isShow === 'edit') {
        action.id = this.actionID
        // 修改权限
        api.updateAction(action).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeActionList'
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
    // 修改权限时获取权限资料
    getAction () {
      api.getActionByID(this.editActionID).then(res => {
        this.loading = false
        let _res = res.data
        this.actionID = _res.id
        this.actionName.text = _res.action_name
        this.actionGroup = _res.group_id
        this.menuOrder.text = _res.action_order.toString()
        this.actionUrl.text = _res.request_url
        this.parentMenu = _res.parent_menu
      }).catch(err => console.log(err))
    },
    // 验证
    validateInput (val) {
      validateInputCommon(val)
    },

    validateSelect (item) {
      if (item.text === '') {
        item.tips = '此项必选'
        return false
      } else {
        item.tips = ''
        return true
      }
    },

    validateNumber () {
      validateNumberCommon(this.menuOrder)
    },

    validateAll () {
      if (!validateInputCommon(this.actionName)) return false
      if (!validateInputCommon(this.actionUrl)) return false
      if (!validateNumberCommon(this.menuOrder)) return false
      return true
    }
  }
}
</script>
<style lang="scss" scoped>
.header{
  display: flex;
  justify-content: space-between;
}
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
.btn-group{
  margin: 15px 0;
  text-align: center;

  button{
    border-color: $color-main-btn;
    background: $color-main-btn;
  }
}
</style>
