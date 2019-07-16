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
        <router-link :to="{name:'SeeUserList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">提醒方式<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-checkbox-group v-model="Reminder">
              <el-checkbox label="短信"></el-checkbox>
              <el-checkbox label="邮件"></el-checkbox>
            </el-checkbox-group>
            </div>
          </div>
          <p class="validate-tips">{{ accName.tips }}</p>
        </li>
      </ul>
      <ul class="input-group">
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">寿命提醒提前<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入寿命提醒提前" v-model.trim="userName.text" @keyup.native="validateInput(userName)"></el-input>
            </div>
            <span>(天)</span>
          </div>
          <p class="validate-tips">{{ userName.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">中修提醒提前<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入中修提醒提前" v-model.trim="userName.text" @keyup.native="validateInput(userName)"></el-input>
            </div>
            <span>(天)</span>
          </div>
          <p class="validate-tips">{{ userName.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">大修提醒提前<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入大修提醒提前" v-model.trim="userName.text" @keyup.native="validateInput(userName)"></el-input>
            </div>
            <span>(天)</span>
          </div>
          <p class="validate-tips">{{ userName.tips }}</p>
        </li>
      </ul>
    </div>
    <div class="btn-enter">
      <x-button class="close">
        <router-link :to="{ name: 'SeeUserList' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="enter">保存</x-button>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vInput, vEmail, vPhone, nullToEmpty } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/authApi'
export default {
  name: 'AddUser',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      // isShow: this.$route.params.mark,
      isShow: 'add',
      editUserID: this.$route.params.id,
      userID: '',
      accName: {
        text: '',
        tips: ''
      },
      userName: {
        text: '',
        tips: ''
      },
      jobNumber: {
        text: '',
        tips: ''
      },
      role: '',
      roleList: [],
      position: {
        text: '',
        tips: ''
      },
      mobile: {
        text: '',
        tips: ''
      },
      email: {
        text: '',
        tips: ''
      },
      Reminder: []
    }
  },
  created () {
    // 角色列表
    // api.getRoleAll().then(res => {
    //   this.roleList = res.data
    // }).catch(err => console.log(err))
    this.isShow = 'add'
    this.title = '| 寿命提醒配置'
  },
  mounted () {
    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 寿命提醒配置'
      // this.$emit('title', '| 添加用户')
      // this.btnText = '确认'
    } else if (this.isShow === 'edit') {
      this.loading = true
      this.title = '| 寿命提醒配置'
      // this.$emit('title', '| 修改用户')
      // this.btnText = '保存'
      // this.getUser()
    }
  },
  methods: {
    // 添加用户
    enter () {
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      let user = {
        acc_name: this.accName.text,
        user_name: this.userName.text,
        role_id: this.role,
        job_number: this.jobNumber.text,
        position: this.position.text,
        mobile: this.mobile.text,
        email: this.email.text
      }
      if (this.isShow === 'add') {
        // 添加用户
        api.addUser(user).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeUserList'
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
        user.id = this.userID
        // 修改用户
        api.updateUser(user).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeUserList'
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
    // 修改用户时获取用户资料
    getUser () {
      api.getUserByID(this.editUserID).then(res => {
        this.loading = false
        let _res = res.data
        this.userID = _res.id
        this.accName.text = _res.acc_name
        this.userName.text = _res.user_name
        this.role = _res.role_id === null ? '' : _res.role_id
        this.jobNumber.text = _res.job_number
        this.position.text = nullToEmpty(_res.position)
        this.mobile.text = nullToEmpty(_res.mobile)
        this.email.text = nullToEmpty(_res.email)
      }).catch(err => console.log(err))
    },
    // 验证
    validateInput (val) {
      validateInputCommon(val)
    },
    validateInputPhone (val) {
      if (val.text !== '' && !vPhone(val.text)) {
        val.tips = '此手机号非法'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateInputEMail (val) {
      if (val.text !== '' && !vEmail(val.text)) {
        val.tips = '此邮箱非法'
        return false
      } else {
        val.tips = ''
        return true
      }
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
      if (this.role.text === '') {
        this.role.tips = '此项必选'
        return false
      } else {
        this.role.tips = ''
        return true
      }
    },

    // validateNumber () {
    //   validateNumberCommon(this.groupOrder)
    // },

    validateAll () {
      if (!validateInputCommon(this.accName)) return false
      if (!validateInputCommon(this.userName)) return false
      if (!validateInputCommon(this.jobNumber)) return false
      if (!this.validateInputNull(this.position)) return false
      if (!this.validateInputEMail(this.email)) return false
      if (!this.validateInputPhone(this.mobile)) return false
      // if (!this.validateSelect()) return false
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
