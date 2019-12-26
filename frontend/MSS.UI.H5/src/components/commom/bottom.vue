<template>
    <mu-paper class="bottomBar">
        <mu-bottom-nav :value="bottomNav" @change="handleChange">
            <mu-bottom-nav-item to="/home" value="维护资料" title="维护资料" icon="book"/>
            <mu-bottom-nav-item to="/troublelist" value="故障接修" title="故障接修" icon="pan_tool"/>
            <mu-bottom-nav-item to="/warning" value="消息" title="消息" icon="warning"/>
            <mu-bottom-nav-item to="/my" value="我的" title="我的" icon="account_circle"/>
        </mu-bottom-nav>
    </mu-paper>
</template>
<script>
    export default{
        data(){
            return {
                bottomNav: '维护资料'
            }
        },
        created:function(){
            this.facthData();
        },
        watch: {
            '$route': 'facthData'
        },
        methods:{
            handleChange(val){
                console.log(val)
                this.bottomNav=val;
            },
            facthData() {
                // let accesstoken = localStorage.getItem("accesstoken");
                //  accesstoken =true;
                // if (accesstoken) {
                //     // console.log('yes')
                //     this.person_path = '/home/personal'
                // } else {
                //     // console.log('no')
                //     this.person_path = '/home/login'
                // }
                if (this.$route.path === '/home') {
                    this.bottomNav = '维护资料'
                }
                if (this.$route.path === '/troublelist') {
                    this.bottomNav = '故障接修'
                }
                if (this.$route.path === '/warning') {
                    this.bottomNav = '消息'
                }
                if (this.$route.path === '/my') {
                    this.bottomNav = '我的'
                }
            }
        }
    }
</script>
<style>
    .bottomBar{
        position:fixed;
        /* height:5rem; */
        width:100%;
        bottom:0;
        left:0;
        background:red;
    }
</style>