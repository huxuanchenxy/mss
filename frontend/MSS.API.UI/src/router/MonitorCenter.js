/**
 * 设备台账
 */
const routes = [
  {
    path: '/monitorCenter',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/MonitorCenter/MonitorCenter.vue'),
    children: [
      {
        path: '/',
        redirect: 'eqpmonitor'
      }, {
        path: 'eqpmonitor',
        component: () => import(/* webpackChunkName: "system" */ '@/views/MonitorCenter/eqpmonitor/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'monitorCenter',
            component: () => import(/* webpackChunkName: "system" */ '@/views/MonitorCenter/eqpmonitor/children/List.vue')
          }
        ]
      }
    ]
  }
]
export default routes
