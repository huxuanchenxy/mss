/**
 * 设备台账
 */
const routes = [
  {
    path: '/statistics',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/StatisticsReport/StatisticsReport.vue'),
    children: [
      {
        path: '/',
        redirect: 'alarmReport'
      }, {
        path: 'alarmReport',
        component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/alarm/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'alarmCount'
          }, {
            path: 'alarmCount',
            name: 'AlarmCount',
            component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/alarm/children/Count.vue')
          }
        ]
      }, {
        path: 'troubleReport',
        component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/trouble/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'trouble'
          }, {
            path: 'trouble',
            name: 'Trouble',
            component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/trouble/children/Trouble.vue')
          }
        ]
      }
    ]
  }
]
export default routes
