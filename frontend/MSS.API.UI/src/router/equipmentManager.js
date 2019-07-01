/**
 * 设备台账
 */
const routes = [
  {
    path: '/equipmentManager',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/EquipmentManager.vue'),
    children: [
      {
        path: '/',
        redirect: 'eqpType'
      }, {
        path: 'eqpType',
        component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/eqpType/Index.vue'),
        children: [
          {
            path: '/',
            name: 'eqpType',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeeEqpTypeList',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/equipmentManager/eqpType/children/SeeEqpTypeList.vue')
          }, {
            path: 'add',
            name: 'AddEqpType',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/equipmentManager/eqpType/children/AddEqpType.vue')
          }
        ]
      }
      // , {
      //   path: 'equipment',
      //   component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/equipment/Index.vue'),
      //   children: [
      //     {
      //       path: '/',
      //       name: 'equipment',
      //       redirect: 'list'
      //     }, {
      //       path: 'list',
      //       name: 'SeeEqpList',
      //       component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/equipment/children/SeeEqpList.vue')
      //     }, {
      //       path: 'add',
      //       name: 'AddEqp',
      //       component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/equipment/children/AddEqp.vue')
      //     }
      //   ]
      // }
    ]
  }
]
export default routes
