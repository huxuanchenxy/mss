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
      }, 
      {
        path: 'ProcessConsul',
        component: () => import( '@/views/system/operlog/index.vue'),
        children: [
          {
            path: '/',
            name: 'ProcessConsul',
            redirect: 'list'
          }, 
          {
            path: 'list',
            name: 'SeeProcessConsul',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/operlog/children/SeeProcessConsul.vue')
          }, 
          {
            path: 'addMaintainConfig/:mark?/:id?',
            name: 'addMaintainConfig',
            component: () => import('@/views/system/operlog/children/addMaintainConfig.vue')
          }
        ]
      },
      {
        path: 'MonitorServer',
        component: () => import( '@/views/system/operlog/index.vue'),
        children: [
          {
            path: '/',
            name: 'MonitorServer',
            redirect: 'list'
          }, 
          {
            path: 'list',
            name: 'SeeMonitorServer',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/operlog/children/SeeMonitorServer.vue')
          }
        ]
      }
    ]
  }
]
export default routes
