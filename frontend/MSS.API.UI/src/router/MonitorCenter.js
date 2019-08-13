/**
 * 设备台账
 */
const routes = [
  {
    path: '/monitorCenter',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/EquipmentManager.vue'),
    children: [
      {
        path: '/',
        redirect: 'eqpmonitor'
      }, {
        path: 'eqpmonitor',
        component: () => import(/* webpackChunkName: "system" */ '@/views/equipmentManager/eqpmonitor/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'EqpMonitor',
            component: () => import(/* webpackChunkName: "system" */ '@/views/equipmentManager/eqpmonitor/children/List.vue')
          }
        ]
      }
    ]
  }
]
export default routes
