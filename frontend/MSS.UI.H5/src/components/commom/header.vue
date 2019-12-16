<template>
    <div class="hello">
        <mu-appbar class="title" title="上海18号线智能运维系统" >
            <!--// <mu-icon-button slot="left">
            //     <img class="logo" :src="src"
            // </mu-icon-button>-->
            <mu-icon-button icon="menu" slot="left" @click="toggle()"/>
            <mu-icon-button icon="more_vert" slot="right" @click="toggle1()"/>
            <!-- <mu-icon-menu slot="right" icon="more_vert" :value="theme" @change="changeTheme">
                <!-- <mu-menu-item title="LIGHT" value="light" />
                <mu-menu-item title="CARBON" value="carbon" />
                <mu-menu-item title="TEAL" value="teal" /> 
                <mu-menu-item title="CARBON" value="logout" />
            </mu-icon-menu> -->
        </mu-appbar>
        <mu-drawer :open="open" :docked="docked" @close="toggle()" style="width:40%;">
            <mu-list @itemClick="docked ? '' : toggle()">
                <mu-list-item title="技术资料" @click="JumpTo(1)"/>
                <mu-list-item title="我的工单" @click="JumpTo(2)"/>
                <mu-list-item title="我的故障报告" @click="JumpTo(3)"/>
                <mu-list-item title="账户设定" @click="JumpTo(4)"/>
                <mu-list-item title="关于" @click="JumpTo(5)"/> 
                <mu-list-item v-if="docked" @click.native="open = false" title="收回"/>
            </mu-list>
        </mu-drawer>
        <mu-drawer :open="open1" :docked="docked1" @close="toggle1()" style="width:18%;" :right="right1">
            <mu-list @itemClick="docked1 ? '' : toggle1()">
                <mu-list-item title="退出" @click="Logout()"/>
                <mu-list-item v-if="docked1" @click.native="open1 = false" title="收回"/>
            </mu-list>
        </mu-drawer>
    </div>
</template>
<script>
    import axios from 'axios'
    import light from '!raw-loader!muse-ui/dist/theme-default.css'
    import carbon from '!raw-loader!muse-ui/dist/theme-carbon.css'
    import teal from '!raw-loader!muse-ui/dist/theme-teal.css'
    export default {
        name: 'hello',
        data() {
            return {
                theme: 'carbon',
                src:"http://www.vue-js.com/public/images/vue.png",
                themes: {
                    carbon
                    // light,
                    // teal
                },
                open: false,
                docked: true,
                open1: false,
                docked1: true,
                right1:true
            }
        },
        created () {
            this.changeTheme('carbon')
        },
        methods: {
            changeTheme(theme) {
                this.theme = theme;
                // console.log(this.theme)
                const styleEl = this.getThemeStyle()
                styleEl.innerHTML = this.themes[theme] || ''
            },
            getThemeStyle() {
                const themeId = 'muse-theme'
                let styleEl = document.getElementById(themeId)
                // console.log(styleEl)
                if (styleEl) return styleEl
                styleEl = document.createElement('style')
                styleEl.id = themeId
                document.body.appendChild(styleEl)
                // console.log(styleEl)
                return styleEl
            },
            toggle (flag) {
                this.open = !this.open
                this.docked = !flag
                // console.log(event)
            },
            toggle1 (flag1) {
                this.open1 = !this.open1
                this.docked1 = !flag1
                // console.log(event)
            },
            JumpTo (v) {
                // console.log('v:' +  v)
                if(v === 4){
                    this.$router.push({
                        name: 'Password'
                    })
                }
                if(v === 1){
                    this.$router.push({
                        name: 'home'
                    })
                }

            },
            Logout(){
                window.sessionStorage.removeItem('token')
                this.$router.push({
                    name: 'Login'
                })
            }
        }
    }
</script>
<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    .title {
        text-align: center;
        height: 4rem;
    }
    .mu-icon-button {
        padding: 0.4rem;
    }
</style>
