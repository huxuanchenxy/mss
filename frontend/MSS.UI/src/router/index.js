import Vue from 'vue'
import Router from 'vue-router'

import system from './SystemSetting.js'

Vue.use(Router)

let main = {
    path: '/',
    name: 'main',
    component: () => import(/* webpackChunkName: "main" */ '@/views/Main.vue'),
    children: [
    ]
}

// 系统管理
main.children.push(system)

let routes = [
    {
        path: '/login',
        component: () => import(/* webpackChunkName: "main" */ '@/views/Login.vue'),
        name: 'login'
    },
    {
        path: '/404',
        component: () => import(/* webpackChunkName: "main" */ '@/views/404.vue'),
        name: ''
    },
    main
]

export default new Router({
    routes
})
