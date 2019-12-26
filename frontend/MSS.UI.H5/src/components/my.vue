<template>
    <div class="wrapper">
        <isheader class="header"></isheader>
        <div>
            <mu-list class="mylist">
                <mu-list-item @click="JumpTo(1)">
                    <mu-icon value="fingerprint" class="muicon"></mu-icon>
                    <span class="mucontent">修改密码</span>
                    <mu-icon value="keyboard_arrow_right" class="muiconright"></mu-icon>
                </mu-list-item>
                <mu-divider class="mudivider"></mu-divider>
                <mu-list-item @click="JumpTo(2)">
                    <mu-icon value="settings" class="muicon"></mu-icon>
                    <span class="mucontent">设置</span>
                    <mu-icon value="keyboard_arrow_right" class="muiconright"></mu-icon>
                </mu-list-item>
                <mu-divider class="mudivider"></mu-divider>
            </mu-list>
        </div>
        <BottomNavigation></BottomNavigation>
    </div>
</template>
<script>
    import isheader from './commom/header2.vue'
    import BottomNavigation from './commom/bottom.vue'
    import axios from 'axios'
    import loginapi from '@/api/loginApi'
    export default {
        name: 'warning',
        components: {
            isheader,
            BottomNavigation
        },
        data() {
            return {
                msg: 'Welcome to Your 上海18号线智能运维系统 App',
                accName:'',
                userName:'',
            }
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
                    if (res.code === ApiRESULT.Success) {
                    this.userName = res.data.user_name
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
                    this.$router.push({
                        name: 'mysetting'
                    })
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
    .mylist{
        top:60px;
    }
    .mylist .mucontent{
        position: relative;
        left: 10px;
        bottom: 5px;
        color: #fff;
    }
    .mylist .muicon{
        color: #fff;
    }
    .mylist .mudivider{
        background-color: #fff;
    }
    .mylist .muiconright{
        right: 0;
        position: absolute;
        color:#fff
    }
</style>