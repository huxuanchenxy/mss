import Vue from 'vue'
import Router from 'vue-router'

Vue.use(Router)

let routes = [
  {
    path: '/',
    name: 'Index',
    component: () => import(/* webpackChunkName: "index" */ '@/views/Index.vue')
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
    path: '*',
    name: 'NotFound',
    component: () => import('@/views/404/Index.vue')
  }
]

// 系统管理
import system from './system'
routes = routes.concat(system)

// 设备台账
import equipmentManager from './equipmentManager'
routes = routes.concat(equipmentManager)

// // 基础数据
// import dataBasic from './dataBasic'
// routes = routes.concat(dataBasic)

// // 模板管理
// import template from './template'
// routes = routes.concat(template)

// // 警报管理
// import alarm from './alarm'
// routes = routes.concat(alarm)

// // 备品备件管理
// import spares from './spares'
// routes = routes.concat(spares)

// // 抢修任务单管理
// import emergency from './emergencymaintain'
// routes = routes.concat(emergency)

// // 维修巡检管理
// import inspectionManagement from './inspectionManagement'
// routes = routes.concat(inspectionManagement)

// // 管廊维修管理
// import maintenanceOfCorridor from './maintenanceOfCorridor'
// routes = routes.concat(maintenanceOfCorridor)

// // 监控
// import monitor from './monitor'
// routes = routes.concat(monitor)

// // 合同管理
// import Manufacturer from './Manufacturer'
// routes = routes.concat(Manufacturer)

// // 应急预案管理
// import contingencyPlan from './contingencyPlan'
// routes = routes.concat(contingencyPlan)

// // 专项检查
// import specialInspection from './specialInspection'
// routes = routes.concat(specialInspection)

// // 报表
// import charts from './charts'
// routes = routes.concat(charts)

const router = new Router({
  routes
})

// 全局路由拦截
router.beforeEach((to, from, next) => {
  if (from.matched[0] === undefined && to.name !== 'Login' && window.sessionStorage.getItem('token') === null) {
    next('/login')
  } else {
    next()
  }
})

export default router
