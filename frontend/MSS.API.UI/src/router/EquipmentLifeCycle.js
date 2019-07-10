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
            }
          ]
        }
      ]
    }
  ]
export default routes
