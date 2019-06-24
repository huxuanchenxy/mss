<template>
    <el-menu
      :default-active="defaultPath"
      class="el-menu-vertical-demo"
      router :collapse="isCollapse" unique-opened>
        <template v-for="(item,index) in urls">
            <el-submenu :index="index+''" v-if="!item.isLeaf" :key="index">
                <template slot="title"><i :class="item.iconCls"></i>{{item.name}}</template>
                <el-menu-item v-for="(child, idx) in item.children" :index="child.path" :key="idx">{{child.name}}</el-menu-item>
            </el-submenu>
            <el-menu-item v-if="item.isLeaf" :index="item.path" :key="index"><i :class="item.iconCls"></i>{{item.name}}</el-menu-item>
        </template>
    </el-menu>
</template>
<script>
import api from '@/api/commonApi'
export default {
    data () {
        return {
            isCollapse: false,
            urls: [],
            defaultPath: '/productmgr/monitor'
        }
    },
    methods: {
        getMenu () {
            api.getMenu().then((res) => {
                this.urls = res.queryList
            })
        }
    },
    mounted () {
        this.getMenu()
        // let userAction = sessionStorage.getItem('userAction')
        // this.urls = JSON.parse(userAction)
        // if (this.$route.path === '/') {
        //     this.defaultPath = '/productmgr/monitor'
        // } else {
        //     this.defaultPath = this.$route.path
        // }
        // this.$router.push({path: this.defaultPath})
    }
}
</script>
<style>

</style>
