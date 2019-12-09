import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

let routes = [
  {
    path: '/',
    name: 'Login',
    meta: { login: true },
    // component: () => import(/* webpackChunkName: "index" */ '@/views/Index.vue')
    component: () => import(/* webpackChunkName: "login" */ '@/views/login/Login.vue')
  }, {
    path: '/login',
    name: 'Login',
    meta: { login: true },
    component: () => import(/* webpackChunkName: "login" */ '@/views/login/Login.vue')
  }, {
    path: '/password',
    name: 'Password',
    component: () => import(/* webpackChunkName: "login" */ '@/views/login/Password.vue')
  }, {
    path: '/Index',
    name: 'Index',
    component: () => import(/* webpackChunkName: "index" */ '@/views/Index.vue')
  }, {
    path: '*',
    name: 'NotFound',
    component: () => import('@/views/404/Index.vue')
  }
]

// 专家库
import system from './system'
import MainTainData from './MainTainData'
import EquipmentLifeCycle from './EquipmentLifeCycle'
import EventCenter from './EventCenter'
import MonitorCenter from './MonitorCenter'
import WarehouseManager from './warehouseManager'
import Statistics from './StatisticsReport'
import ConstructionManager from './constructionManager'
import ConstructionManager1 from './constructionManager1'
import TroubleManager from './troubleManager'

routes = routes.concat(system)
routes = routes.concat(MainTainData)
routes = routes.concat(EquipmentLifeCycle)
// 设备台账
import equipmentManager from './equipmentManager'
routes = routes.concat(equipmentManager)

//事件中心
routes = routes.concat(EventCenter)
// 监控中心
routes = routes.concat(MonitorCenter)
// 仓库管理
routes = routes.concat(WarehouseManager)
// 统计报表
routes = routes.concat(Statistics)
// 施工管理
routes = routes.concat(ConstructionManager)
routes = routes.concat(ConstructionManager1)
// 故障管理
routes = routes.concat(TroubleManager)
// 网络监控
import monitor from './monitor'
routes = routes.concat(monitor)

// 工作流
import workflow from './workflow'
routes = routes.concat(workflow)

const router = new Router({
  routes
})

// 全局路由拦截
// router.beforeEach((to, from, next) => {
//   if (from.matched[0] === undefined && to.name !== 'Login' && window.sessionStorage.getItem('token') === null) {
//     next('/login')
//   } else {
//     next()
//   }
// })

 

export default router
