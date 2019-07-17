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
              <el-checkbox-group v-model="reminder" @change="reminderclick">
              <el-checkbox label="短信"></el-checkbox>
              <el-checkbox label="邮件"></el-checkbox>
            </el-checkbox-group>
            </div>
          </div>
        </li>
      </ul>
      <ul class="input-group">
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">寿命提醒提前<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入寿命提醒提前" v-model.trim="beforeDead.text" @keyup.native="validateInput(beforeDead)"></el-input>
            </div>
            <span>(天)</span>
          </div>
          <p class="validate-tips">{{ beforeDead.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">中修提醒提前<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入中修提醒提前" v-model.trim="beforeMaintainMiddle.text" @keyup.native="validateInput(beforeMaintainMiddle)"></el-input>
            </div>
            <span>(天)</span>
          </div>
          <p class="validate-tips">{{ beforeMaintainMiddle.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">大修提醒提前<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入大修提醒提前" v-model.trim="beforeMaintainBig.text" @keyup.native="validateInput(beforeMaintainBig)"></el-input>
            </div>
            <span>(天)</span>
          </div>
          <p class="validate-tips">{{ beforeMaintainBig.tips }}</p>
        </li>
      </ul>
      <ul class="input-group">
        <li class="list1">
          <div class="inp-wrap">
            <span class="text">提醒内容格式模板</span>
            <div class="inp" style="width:86.5%;">
              <el-input type="textarea" v-model="textTemplate"  :rows="7" ></el-input>
            </div>
          </div>
        </li>
      </ul>
      <ul class="input-group">
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">是否启用规则<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-radio-group v-model="published">
                <el-radio :label="1">启用</el-radio>
                <el-radio :label="0">禁用</el-radio>
              </el-radio-group>
            </div>
          </div>
        </li>
      </ul>
    </div>
    <div class="btn-enter">
      <x-button class="close">
        <router-link :to="{ name: 'SeeUserList' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="enter">设定</x-button>
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
      beforeDead: {
        text: '',
        tips: ''
      },
      beforeMaintainMiddle: {
        text: '',
        tips: ''
      },
      beforeMaintainBig: {
        text: '',
        tips: ''
      },
      reminder: [],
      published: 0,
      textTemplate: ''
    }
  },
  created () {
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
      alert(this.reminder)
      var _reminder
      if (this.reminder.length === 0) {
        this.$message({
          message: '验证失败，请选择提醒方式',
          type: 'error'
        })
        return
      } else if (this.reminder.length === 2) {
        _reminder = 3
      } else if (this.reminder[0] === '短信') {
        _reminder = 1
      } else if (this.reminder[0] === '邮件') {
        _reminder = 2
      }
      alert('aaa' + _reminder)
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      let obj = {
        Reminder: _reminder
      }
      if (this.isShow === 'add') {
        // 添加用户
        api.addUser(obj).then(res => {
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
        // user.id = this.userID
        // 修改用户
        api.updateUser(obj).then(res => {
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
    },
    reminderclick () {
      // alert(333)
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
<style>
  .el-textarea .el-textarea__inner{
    resize: none;
  }
</style>
