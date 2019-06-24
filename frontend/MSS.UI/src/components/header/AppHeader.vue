<template>
    <div style="height:100%;min-height:55px;background:#20a0ff">
        <el-row>
            <el-col :span="4" :offset="1">
                <img src="./images/icon-company.svg" class="vertical-middle" width="35" height="40" alt="">
                <p class="grid-content inline-block vertical-middle">
                    <span class="block "></span>
                </p>
            </el-col>
            <el-col :span="15"><p class="grid-content title" style="font-size:20px">SEARI智能制造MES开发平台v2.0</p></el-col>
            <el-col :span="4">
                <img src="./images/icon-user.svg" class="vertical-middle" width="35" height="40" alt="">
                <p class="grid-content inline-block vertical-middle">
                    <span class="block">{{pwd.user}}</span>
                    <!--<router-link class="sub-title" :to="{ name: 'password' }">修改密码</router-link> |-->
                    <span class="sub-title likeRouterLink" @click="showDialog" >修改密码</span> |
                    <router-link class="sub-title" :to="{ name: 'login' }" @click="clearSession">退出</router-link>
                </p>
            </el-col>
        </el-row>
        <el-dialog  title="密码修改"  :visible.sync="dialogVisible">
            <el-form :model="pwd" :rules="rules" ref="ruleForm" label-position="left" label-width="0px" >
                <el-form-item prop="user">
                    <el-input type="text" v-model="pwd.user" auto-complete="off" disabled></el-input>
                </el-form-item>
                <el-form-item prop="oldPwd">
                    <el-input type="password" v-model="pwd.oldPwd" auto-complete="off" placeholder="旧密码"></el-input>
                </el-form-item>
                <el-form-item prop="newPwd">
                    <el-input type="password" v-model="pwd.newPwd" auto-complete="off" placeholder="新密码"></el-input>
                </el-form-item>
                <el-form-item prop="newPwdSure">
                    <el-input type="password" v-model="pwd.newPwdSure" auto-complete="off" placeholder="新密码确认"></el-input>
                </el-form-item>
                <el-form-item style="width:100%;text-align:center">
                    <el-button type="primary" @click.native.prevent="handleSubmit" :loading="pwding">确认</el-button>
                    <el-button type="primary" @click.native.prevent="handleReset">取消</el-button>
                </el-form-item>
            </el-form>
        </el-dialog>
    </div>
</template>
<script>
import api from '@/api/commonApi'
import {ERR_SHOW} from '@/common/js/utils.js'
export default {
    data () {
        var validatePwdSure = (rule, value, callback) => {
            if (value === '') {
                callback(new Error('请再次输入新密码'))
            } else if (value !== this.pwd.newPwd) {
                callback(new Error('两次输入密码不一致!'))
            } else {
                callback()
            }
        }
        return {
            dialogVisible: false,
            pwding: false,
            pwd: {
                user: '',
                oldPwd: '',
                newPwd: '',
                newPwdSure: ''
            },
            rules: {
                oldPwd: [
                    { required: true, message: '请输入旧密码', trigger: 'blur' }
                ],
                newPwd: [
                    { required: true, message: '请输入新密码', trigger: 'blur' }
                ],
                newPwdSure: [
                    { validator: validatePwdSure, trigger: 'blur' }
                ]
            },
            checked: true
        }
    },
    created () {
        // this.pwd.user = JSON.parse(sessionStorage.getItem('user')).UserName
    },
    methods: {
        handleReset () {
            this.$refs.ruleForm.resetFields()
            this.dialogVisible = false
            this.pwding = false
            this.pwd.oldPwd = ''
            this.pwd.newPwd = ''
            this.pwd.newPwdSure = ''
        },
        handleSubmit (ev) {
            this.$refs.ruleForm.validate((valid) => {
                if (valid) {
                    this.pwding = true
                    var pwdParams = { OldPwd: this.pwd.oldPwd, NewPwd: this.pwd.newPwd }
                    api.ModifyPwd(pwdParams).then(data => {
                        this.pwding = false
                        if (data.isSuccess) {
                            this.$message.success('密码修改成功')
                            this.dialogVisible = false
                        } else {
                            this.$message.error(ERR_SHOW[data.errType])
                        }
                    })
                } else {
                    return false
                }
            })
        },
        clearSession () {
            window.sessionStorage.removeItem('user')
            window.sessionStorage.removeItem('userAction')
        },
        showDialog () {
            if (this.$refs['ruleForm'] !== undefined) {
                this.$refs['ruleForm'].resetFields()
            }
            this.dialogVisible = true
        }
    }
}
</script>
<style>
.grid-content {
    border-radius: 4px;
    min-height: 40px;
    height: 40px;
}
.vertical-middle {
    vertical-align: middle;
}
.inline-block {
    display: inline-block;
}
.block {
    display: block;
}
.title{
    color: #fff;
    position: relative;
    height: 100%;
    font-weight: bold;
    text-align: center;
    line-height: 40px;
    text-shadow: 0 0 15px;
}
.sub-title{
    color: #cdcecf;
}
// 文本对齐
.text-left {
    text-align: left;
}
.date{
    position: relative;
    padding-left: 30px;

    &:after{
        content: "";
        position: absolute;
        top: 50%;
        left: 0;
        width: 1px;
        height: 30px;
        background: #57565B;
        transform: translateY(-50%);
    }
}
.likeRouterLink{
    cursor: pointer;
    text-decoration: underline;
}
</style>
