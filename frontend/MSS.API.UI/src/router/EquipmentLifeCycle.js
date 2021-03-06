/**
 * 设备生命周期
 */
const routes = [ 
    {  
      path: '/EquipmentLifeCycle',
      meta: { validate: true },
      component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/EquipmentLifeCycle/EquipmentLifeCycle.vue'),
      children: [
        {
          path: '/',
          redirect: 'MaintainReg'
        },
        {
          path: 'MaintainReg',
          component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/EquipmentLifeCycle/MaintainReg/Index.vue'),
          children: [
            {
              path: '/',
              name: 'MaintainReg',
              redirect: 'list'
            }, {
              path: 'list:id?',
              name: 'DeviceMaintainList',
              component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/EquipmentLifeCycle/MaintainReg/children/DeviceMaintainList.vue')
            }, {
              path: 'addDeviceMaintain/:mark?/:id?',
              name: 'addDeviceMaintain',
              component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/EquipmentLifeCycle/MaintainReg/children/addDeviceMaintain.vue')
            }, {
              path: 'detail/:mark?/:id?',
              name: 'DetailDeviceMaintain',
              component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/EquipmentLifeCycle/MaintainReg/children/DetailDeviceMaintain.vue')
            }
          ]
        },
        {
          path: 'LifeTimeKeyMaintain',
          component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/EquipmentLifeCycle/LifeTimeKeyMaintain/Index.vue'),
          children: [
            {
              path: '/',
              name: 'LifeTimeKeyMaintain',
              redirect: 'list'
            }, {
              path: 'list',
              name: 'LifeTimeKeyMaintainlist',
              component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/EquipmentLifeCycle/LifeTimeKeyMaintain/children/LifeTimeKeyMaintainlist.vue')
            }
          ]
        },
        {
          path: 'history',
          component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/EquipmentLifeCycle/history/Index.vue'),
          children: [
            {
              path: '/',
              name: 'history',
              redirect: 'list'
            }, {
              path: 'list',
              name: 'SeeHistory',
              component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/EquipmentLifeCycle/history/children/SeeHistory.vue')
            }, {
              path: 'detail',
              name: 'DetailWorkingApplication',
              component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/EquipmentLifeCycle/history/children/DetailWorkingApplication.vue')
            }
          ]
        }
      ]
    }
  ]
export default routes
