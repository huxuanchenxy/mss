<template>
  <div class="wrap">
    <div class="con-padding-horizontal header">
      <h2 class="title">
          <img class="icon" src="./images/icon-password.svg" alt=""> 修改密码
        </h2>
    </div>
    <div class="content-wrap">
      <h2 class="con-title">修改密码</h2>
      <div class="inp-wrap" :class="{ active: currentPassword.active }">
        <span class="text">当前密码</span>
        <el-input type="password" v-model="currentPassword.str" @focus="currentActiveFoucs" @blur="currentActiveBlur">
          <i slot="suffix" v-if="currentPassword.isShowIcon === 'success'" class="el-icon-circle-check"></i>
          <i slot="suffix" v-else-if="currentPassword.isShowIcon === 'error'" class="el-icon-circle-close"></i>
        </el-input>
      </div>
      <div class="inp-wrap" :class="{ active: newPassword.active }">
        <span class="text">新密码</span>
        <el-input type="password" v-model="newPassword.str" @focus="newActiveFocus" @blur="newActiveBlur">
          <i slot="suffix" v-if="newPassword.isShowIcon === 'success'" class="el-icon-circle-check"></i>
          <i slot="suffix" v-else-if="newPassword.isShowIcon === 'error'" class="el-icon-circle-close"></i>
        </el-input>
      </div>
      <div></div>
      <div class="inp-wrap" :class="{ active: newCheckPassword.active }">
        <span class="text">确认新密码</span>
        <el-input type="password" v-model="newCheckPassword.str" @focus="newActiveFocus1" @blur="newActiveBlur1">
          <i slot="suffix" v-if="newCheckPassword.isShowIcon === 'success'" class="el-icon-circle-check"></i>
          <i slot="suffix" v-else-if="newCheckPassword.isShowIcon === 'error'" class="el-icon-circle-close"></i>
        </el-input>
      </div>

      <div class="btn-wrap">
        <el-button native-type="submit" @click="check">确认</el-button>
        <el-button native-type="submit">
          <router-link class="color-white" :to="{ name: 'Index' }">取消</router-link>
        </el-button>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: 'Password',
  data () {
    return {
      currentPassword: {
        str: '',
        active: false,
        isShowIcon: 'hide'
      },
      newPassword: {
        str: '',
        active: false,
        isShowIcon: 'hide'
      },
      newCheckPassword: {
        str: '',
        active: false,
        isShowIcon: 'hide'
      }
    }
  },
  methods: {
    check () {
      if (this.currentPassword.str === '') {
        this.currentPassword.isShowIcon = 'error'
        this.$message({
          message: '请输入新密码',
          type: 'warning'
        })
      } else if (this.newPassword.str === '') {
        this.currentPassword.isShowIcon = 'success'
        this.newPassword.isShowIcon = 'error'
        this.$message({
          message: '请输入新密码',
          type: 'warning'
        })
      } else if (this.newCheckPassword.str === '') {
        this.currentPassword.isShowIcon = 'success'
        this.newPassword.isShowIcon = 'success'
        this.newCheckPassword.isShowIcon = 'error'
        this.$message({
          message: '请输入确认新密码',
          type: 'warning'
        })
      } else if (this.newPassword.str === this.newCheckPassword.str) {
        this.newPassword.isShowIcon = 'success'
        this.newCheckPassword.isShowIcon = 'success'
        window.axios.post('/Home/ModifyPwd', {
          OldPwd: this.currentPassword.str,
          NewPwd: this.newPassword.str
        }).then(res => {
          if (res.data === 'OK') {
            this.currentPassword.isShowIcon = 'success'
            this.$message({
              message: '修改密码成功',
              type: 'success'
            })
          } else if (res.data === 'PwdError') {
            this.currentPassword.isShowIcon = 'error'
            this.$message({
              message: '原密码输入错误',
              type: 'error'
            })
          } else {
            this.currentPassword.isShowIcon = 'success'
            this.$message({
              message: res.data,
              type: '修改密码失败'
            })
          }
        }).catch(err => console.log(err))
      } else {
        this.newPassword.isShowIcon = 'success'
        this.currentPassword.isShowIcon = 'success'
        this.newCheckPassword.isShowIcon = 'error'
        this.$message({
          message: '新密码不一致',
          type: 'warning'
        })
      }
    },
    currentActiveFoucs () {
      this.currentPassword.active = true
    },
    currentActiveBlur () {
      if (this.currentPassword.str === '') this.currentPassword.active = false
    },
    newActiveFocus () {
      this.newPassword.active = true
    },
    newActiveBlur () {
      if (this.newPassword.str === '') this.newPassword.active = false
    },
    newActiveFocus1 () {
      this.newCheckPassword.active = true
    },
    newActiveBlur1 () {
      if (this.newCheckPassword.str === '') this.newCheckPassword.active = false
    }
  }
}
</script>
<style lang="scss" scoped>
.wrap {
  height: 100%;
  // 标题
  .header {
    display: flex;
    align-items: center;
    position: relative;
    height: percent(56, $content-height);

    &:after {
      content: "";
      position: absolute;
      bottom: -7px;
      left: 0;
      width: 100%;
      height: 11px;
      background: url("../../common/images/line.png") no-repeat 0 0/100% 100%;
    }

    .title {
      font-size: PXtoEm(18);
    }

    .icon {
      vertical-align: middle;
    }
  }

  .content-wrap {
    $height: $content-height - 56;
    height: percent($height, $content-height);
    border-top: 1px solid transparent;

    .con-title {
      margin: 60px 0 45px;
      font-size: 22px;
      text-align: center;
    }

    .inp-wrap {
      position: relative;
      width: 315px;
      margin: 0 auto;

      &:first-of-type {
        margin-bottom: 35px;
      }
      &:nth-of-type(2) {
        margin-bottom: 35px;
      }

      &.active {
        .text {
          bottom: 35px;
        }

        /deep/ .el-input {
          border-color: $color-highlight-text;
        }
      }

      .text {
        position: absolute;
        bottom: 4px;
        font-size: $font-size-large;
        color: $color-content-text;
        transition: 0.5s;
      }

      /deep/ .el-input {
        border-bottom: 1px solid $color-sub-text;

        input {
          padding: 0;
          border: 0;
        }
      }

      // 图标
      /deep/ .el-input__suffix {
        display: flex;
        align-items: center;
      }

      /deep/ .el-icon-circle-close {
        color: $color-warning;
      }

      /deep/ .el-icon-circle-check {
        color: $color-highlight-text;
      }
    }

    .btn-wrap {
      margin: 40px auto;
      text-align: center;

      /deep/ .el-button {
        width: 150px;
        height: 50px;
        background: #2480C6 !important;
        border-color: #2480C6 !important;
        font-size: 18px;
        font-weight: lighter;
        color: #fff;
      }
    }
  }
}
</style>
