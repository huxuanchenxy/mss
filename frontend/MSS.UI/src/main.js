// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import ElementUI from 'element-ui'
import 'element-ui/lib/theme-chalk/index.css'
import Vuex from 'vuex'
import {TaskNode, TaskNodeStore} from 'vue-task-node'
import 'vue-task-node/dist/css/vnode.css'
// // 引入 ECharts 主模块
// import echarts from 'echarts/lib/echarts'
// // 引入柱状图
// import 'echarts/lib/chart/custom'
// import 'echarts/lib/chart/bar'
// import 'echarts/lib/component/tooltip'
// import 'echarts/lib/component/title'

// import VueDraggable from 'vue-draggable'
import Mock from './mock/index'
// if (process.env.NODE_ENV === 'development') {
Mock.bootstrap()
// }
Vue.use(TaskNode)
Vue.use(ElementUI)
Vue.use(Vuex)
// Vue.use(VueDraggable)
Vue.config.productionTip = false
// Vue.prototype.$echarts = echarts

const store = new Vuex.Store({
    modules: {
        TaskNodeStore
    }
})

// router.beforeEach((to, from, next) => {
//     if (to.path === '/login') {
//         sessionStorage.removeItem('user')
//         sessionStorage.removeItem('userAction')
//     }
//     let user = JSON.parse(sessionStorage.getItem('user'))
//     if (!user && to.path !== '/login') {
//         next({ path: '/login' })
//     } else {
//         next()
//     }
// })

/* eslint-disable no-new */
new Vue({
    el: '#app',
    store,
    router,
    render: h => h(App)
})
