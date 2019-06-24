<template>
    <el-form :model="login" :rules="rules" ref="ruleForm" label-position="left" label-width="0px" class="demo-ruleForm login-container">
      <h3 class="title">系统登录</h3>
      <el-form-item prop="account">
        <el-input type="text" v-model="login.account" auto-complete="off" placeholder="账号"></el-input>
      </el-form-item>
      <el-form-item prop="password">
        <el-input type="password" v-model="login.password" auto-complete="off" placeholder="密码"></el-input>
      </el-form-item>
      <el-checkbox v-model="checked" checked class="remember">记住密码</el-checkbox>
      <el-form-item style="width:100%;text-align:center">
        <el-button type="primary" @click.native.prevent="handleSubmit" :loading="logining">登录</el-button>
        <el-button type="primary" @click.native.prevent="handleReset">重置</el-button>
      </el-form-item>
    </el-form>
</template>

<script>
import api from '@/api/commonApi'
import {ERR_SHOW} from '@/common/js/utils.js'
// import NProgress from 'nprogress'
export default {
    data () {
        return {
            logining: false,
            login: {
                account: 'seari',
                password: '1'
            },
            rules: {
                account: [
                    { required: true, message: '请输入账号', trigger: 'blur' }
                    // { validator: validaePass }
                ],
                password: [
                    { required: true, message: '请输入密码', trigger: 'blur' }
                    // { validator: validaePass2 }
                ]
            },
            checked: true
        }
    },
    methods: {
        handleReset () {
            this.$refs.ruleForm.resetFields()
            this.logining = false
            this.login = {
                account: '',
                password: ''
            }
        },
        handleSubmit (ev) {
            this.$refs.ruleForm.validate((valid) => {
                if (valid) {
                    // _this.$router.replace('/table');
                    this.logining = true
                    // NProgress.start();
                    var loginParams = { UserID: this.login.account, Password: this.login.password }
                    api.Login(loginParams).then(data => {
                        this.logining = false
                        // NProgress.done();
                        if (data.isSuccess) {
                            sessionStorage.setItem('user', JSON.stringify(data.queryObject))
                            sessionStorage.setItem('userAction', JSON.stringify(data.queryList))
                            this.$router.push({ path: '/' })
                        } else {
                            this.$message.error(ERR_SHOW[data.errType])
                        }
                    })
                } else {
                    return false
                }
            })
        }
    }
}

</script>

<style lang="scss" scoped>
    .login-container {
        /*box-shadow: 0 0px 8px 0 rgba(0, 0, 0, 0.06), 0 1px 0px 0 rgba(0, 0, 0, 0.02);*/
        -webkit-border-radius: 5px;
        border-radius: 5px;
        -moz-border-radius: 5px;
        background-clip: padding-box;
        margin: 180px auto;
        width: 350px;
        padding: 35px 35px 15px 35px;
        background: #fff;
        border: 1px solid #eaeaea;
        box-shadow: 0 0 25px #cac6c6;
        .title {
          margin: 0px auto 40px auto;
          text-align: center;
          color: #505458;
        }
        .remember {
          margin: 0px 0px 35px 0px;
        }
    }
</style>
