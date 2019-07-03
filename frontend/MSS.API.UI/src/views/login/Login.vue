<template>
  <div class="wrap">
    <canvas class="canvas-bg" ref="canvas"></canvas>
    <!-- 头部 -->
    <div class="header-wrap con-padding-horizontal">
      <div class="logo">
        <img src="./images/logo.png" alt="">
      </div>
      <div class="contact">
        <img class="icon-phone" src="./images/icon-phone.svg" alt="">
        <img class="icon-wechat" src="./images/icon-wechat.svg" alt="">
      </div>
    </div>
    <!-- 主内容区域 -->
    <div class="content">
      <div class="text-wrap">
        <p class="con-sub-title">上海电气自动化设计研究所有限公司</p>
        <h2 class="con-title">上海轨道交通18号线维修支持系统</h2>
        <div class="en-title">Intelligent Pipeline Operation Management Platform</div>
      </div>
      <!-- 登录框 -->
      <div class="content-wrap">
        <div class="box">
          <div class="inp-wrap" :class="{ active: userName.active }">
            <span class="text">用户名</span>
            <el-input
              v-model="userName.str"
              @focus="currentActiveFoucs"
              @blur="currentActiveBlur">
              <i slot="suffix" v-if="userName.isShowIcon === 'success'" class="el-icon-circle-check"></i>
              <i slot="suffix" v-else-if="userName.isShowIcon === 'error'" class="el-icon-circle-close"></i>
            </el-input>
          </div>
          <div class="inp-wrap" :class="{ active: userPassword.active }">
            <span class="text">密码</span>
            <el-input
              type="password"
              v-model="userPassword.str"
              @focus="newActiveFocus"
              @blur="newActiveBlur"
              @keyup.native.enter="submit">
              <i slot="suffix" v-if="userPassword.isShowIcon === 'success'" class="el-icon-circle-check"></i>
              <i slot="suffix" v-else-if="userPassword.isShowIcon === 'error'" class="el-icon-circle-close"></i>
            </el-input>
          </div>
          <div class="is-remember">
            <el-checkbox label="记住密码"></el-checkbox>
          </div>
          <div class="btn-wrap">
            <el-button native-type="submit" v-if="isLoginIn" @click="submit">登录</el-button>
            <el-button native-type="submit" v-else><i class="el-icon-loading"></i> 登录中</el-button>
          </div>
        </div>
      </div>
    </div>
    <!-- 版权文字 -->
    <div class="copyright">©上海电气自动化设计研究所有限公司所有</div>
  </div>
</template>
<script>
import canvasDraw from './js/canvas'
import api from '@/api/loginApi'
export default {
  name: 'Login',
  data () {
    return {
      userName: {
        str: 'seari',
        active: false,
        isShowIcon: 'hide'
      },
      userPassword: {
        str: '123456',
        active: false,
        isShowIcon: 'hide'
      },
      isLoginIn: true
    }
  },
  methods: {
    currentActiveFoucs () {
      this.userName.active = true
    },
    currentActiveBlur () {
      if (this.userName.str === '') this.userName.active = false
    },
    newActiveFocus () {
      this.userPassword.active = true
    },
    newActiveBlur () {
      if (this.userPassword.str === '') this.userPassword.active = false
    },
    submit () {
      this.isLoginIn = false
      if (this.userName.str === '') {
        this.userName.isShowIcon = 'error'
        this.userPassword.isShowIcon = ''
        return
      }
      if (this.userPassword.str === '') {
        this.userName.isShowIcon = ''
        this.userPassword.isShowIcon = 'error'
        return
      }
      let param = {
        username: this.userName.str,
        password: this.userPassword.str
      }
      api.Login(param).then((res) => {
        if (res.code === 0) {
          this.userName.isShowIcon = 'success'
          this.userPassword.isShowIcon = 'success'
          window.sessionStorage.setItem('token', res.access_token)
          this.$router.push('/')
        } else {
          this.$message.error('登录失败')
        }
        this.isLoginIn = true
      })
    }
  },
  mounted () {
    canvasDraw(this.$refs.canvas)
  }
}
</script>
<style lang="scss" scoped>
.wrap{
  position: relative;
  width: 100%;
  height: 100%;
  background: url(./images/bg.jpg) no-repeat 0 0/100% 100%;

  // 头部
  .header-wrap{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: percent(70, $content-height);
    border-bottom: 1px solid rgba(255,255,255,.1);
    background: #0F0E19;

    .icon-wechat{
      margin-left: 20px;
    }
  }

  .canvas-bg{
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    // transform: translateX(-25%);
  }

  .content{
    box-sizing: border-box;
    position: relative;
    display: flex;
    flex-wrap: wrap;
    position: fixed;
    justify-content: space-between;
    align-items: center;
    width: 100%;
    height: percent($content-height - 70, $content-height);
    margin: 0 auto;
    padding: 0 110px
  }

  .con-sub-title{
    font-size: 34px;
  }

  .en-title{
    position: relative;
    width: 400px;
    font-size: 30px;

    &:after{
      content: "";
      position: absolute;
      top: -20px;
      left: 0;
      width: 20px;
      height: 3px;
      background: #00a0e9;
    }
  }

  .con-title{
    top: 40%;
    left: 50px;
    margin: 0 0 45px;
    font-size: 34px;
    // text-shadow: 0 0 40px #2480C6;
    font-weight: bold;
  }

  .content-wrap{
    box-sizing: border-box;
    width: 370px;
    height: 380px;
    padding: 60px 40px 40px;
    border-top: 1px solid transparent;
    background: url(./images/form-bg.png) no-repeat 0 0/100% 100%;
    color: #fff;

    .inp-wrap{
      position: relative;
      width: 100%;
      margin: 0 auto;

      &:first-of-type{
        margin-bottom: 35px;
      }

      &.active{
        .text{
          bottom: 35px;
        }

        /deep/ .el-input{
          border-color: #fff;
        }
      }

      .text{
        position: absolute;
        bottom: 4px;
        font-size: $font-size-large;
        color: #fff;
        transition: .5s;
      }

      /deep/ .el-input{
        border-bottom: 1px solid #D3DFEF;

        input{
          padding: 0;
          border: 0;
          color: #fff;
        }
      }

      // 图标
      /deep/ .el-input__suffix{
        display: flex;
        align-items: center;
      }

      /deep/ .el-icon-circle-close{
        color: $color-warning;
      }

      /deep/ .el-icon-circle-check{
        color: $color-highlight-text;
      }
    }

    .is-remember{
      margin-top: 20px;

      /deep/ .el-checkbox__label{
        padding-left: 5px;
        color: #fff;
      }
    }

    // 确认按钮
    .btn-wrap{
      margin: 40px auto 0;
      text-align: center;

      /deep/ .el-button{
        width: 100%;
        height: 50px;
        background: #fff !important;
        border-color: #fff !important;
        font-size: 18px;
        color: $color-highlight-text !important;
      }
    }
  }

  .copyright{
    position: fixed;
    bottom: 45px;
    left: 110px;
  }
}
</style>
