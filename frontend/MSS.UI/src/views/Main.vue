<template>
    <el-container class="container">
        <el-header style="height:8vh; padding:0px"><app-header></app-header></el-header>
        <el-container style="height:87vh">
            <el-aside class="leftmenu"><app-menu></app-menu></el-aside>
            <el-main>
                <el-container style="height:100%">
                    <el-header height="40px">
                        <el-breadcrumb separator-class="el-icon-arrow-right">
                            <el-breadcrumb-item v-for="(item, index) in route_list"
                                    :key="index">
                                {{ item.meta.navName }}
                            </el-breadcrumb-item>
                        </el-breadcrumb>
                    </el-header>
                    <el-main class="main"><router-view></router-view></el-main>
                </el-container>
            </el-main>
        </el-container>
        <el-footer style="height:5vh; padding:0px"><app-footer></app-footer></el-footer>
    </el-container>
</template>

<script>
import AppHeader from '@/components/header/AppHeader'
import AppFooter from '@/components/footer/AppFooter'
import AppMenu from '@/components/menu/Menu'
export default {
    name: 'App',
    data () {
        return {
            loading: null
        }
    },
    components: {
        AppHeader,
        AppFooter,
        AppMenu
    },
    computed: {
        route_list () {
            return this.$route.matched.filter(route => route.meta.navName)
        }
    }
}
</script>
<style>
.container{
    height:100vh;
}
.el-header, .el-footer {
    padding: 0px;
}
.main {
    padding: 0px;
    margin: 0px;
    overflow: hidden;
}
.leftmenu {
    border-right: 1px solid #E6E6E6;
}
</style>
