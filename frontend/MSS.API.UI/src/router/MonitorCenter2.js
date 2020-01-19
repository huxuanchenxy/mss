const routes = [
  {
    path: '/monitorCenter',
    component: () => import('@/views/MonitorCenter/MonitorCenter.vue'),
    children: [
      {
        path: '/monitorCenter/monitorServer',
        component: () => import('@/views/MonitorCenter/MonitorServer/SeeMonitorServer.vue')
      },
      {
        path: '/monitorCenter/ProcessConsul',
        component: () => import('@/views/MonitorCenter/MonitorServer/SeeProcessConsul.vue')
      }
    ]
  }
]
export default routes
