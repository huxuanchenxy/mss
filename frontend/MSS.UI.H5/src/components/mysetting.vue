<template>
    <div class="wrapper">
        <isheader class="header"></isheader>
                    <mu-list class="mylist">
                <mu-list-item>
                    <mu-icon value="attach_file" class="muicon"></mu-icon>
                    <span class="mucontent">登录名:</span>
                    <span class="muvalue">{{acc_name}}</span>
                </mu-list-item>
                <mu-divider class="mudivider"></mu-divider>
                <mu-list-item>
                    <mu-icon value="attach_file" class="muicon"></mu-icon>
                    <span class="mucontent">用户名:</span>
                    <span class="muvalue">{{user_name}}</span>
                </mu-list-item>
                <mu-divider class="mudivider"></mu-divider>
                <mu-list-item>
                    <mu-icon value="attach_file" class="muicon"></mu-icon>
                    <span class="mucontent">手机号:</span>
                    <span class="muvalue">{{mobile}}</span>
                </mu-list-item>
                <mu-divider class="mudivider"></mu-divider>
            </mu-list>
        <div class="logout">
        <el-button type="danger" @click="Logout" class="logoutbutton" >退出登录</el-button>
        </div>
    </div>
</template>
<script>
    import isheader from './commom/header3.vue'
    import BottomNavigation from './commom/bottom.vue'
    import axios from 'axios'
    import loginapi from '@/api/loginApi'
    export default {
        name: 'mysetting',
        components: {
            isheader,
            BottomNavigation
        },
        data() {
            return {
                msg: 'Welcome to Your 上海18号线智能运维系统 App',
                acc_name:'',
                user_name:'',
                mobile:'',
            }
        },
        mounted(){
            this.getUserInfo()
        },
        methods:{
            Logout () {
            window.sessionStorage.removeItem('token')
                this.$router.push({
                    name: 'Login'
                })
            },    
            getUserInfo () {
                loginapi.getUserInfo().then((res) => {
                    // console.log(res)
                    if (res.code === 0) {
                    this.user_name = res.data.user_name
                    this.acc_name = res.data.acc_name
                    this.mobile = res.data.mobile
                    window.sessionStorage.setItem('UserInfo', JSON.stringify(res.data))
                    }
                })
            },
            JumpTo (p) {
                if(p === 1){
                    this.$router.push({
                        name: 'Password'
                    })
                }else if (p === 2){
                    
                }
                
            }
        }
    }
</script>
<style>
    .wrapper {
        /* display: flex;
        flex-direction: column;
        height: 100vh; */
        position: fixed;
        width: 100%;
        height: 100%;
    }
    .tab {
        flex: 1;
        margin: 9rem 0 4rem 0;
    }
    .header {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        z-index: 1;
    }
    .logout{
        bottom: 14%;
        position: absolute;
        width: 100%;
        text-align: center;
    }
    .logout button{
        width:200px;
    }
    .muvalue{
        right: 55px;
    position: absolute;
    color: #fff;
    top:34%;
    }
</style>