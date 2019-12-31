<template>
    <div class="wrapper">
        <isheader class="header"></isheader>
        <tabs class="tab"></tabs>
        <BottomNavigation></BottomNavigation>
    </div>
</template>
<script>
    import isheader from './commom/header2.vue'
    import BottomNavigation from './commom/bottom.vue'
    import tabs from './commom/tabs.vue'
    import axios from 'axios'
    export default {
        name: 'home',
        components: {
            isheader,
            tabs,
            BottomNavigation
        },
        data() {
            return {
                msg: 'Welcome to Your 上海18号线智能运维系统 App'
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
</style>