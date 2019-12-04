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
        path: 'alarmReport1',
        component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/alarm/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'alarmCount1'
          }, {
            path: 'alarmCount1',
            name: 'AlarmCount1',
            component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/alarm/children/Count1.vue')
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
      }, {
        path: 'troubleReport1',
        component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/trouble/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'trouble1'
          }, {
            path: 'trouble1',
            name: 'Trouble1',
            component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/trouble/children/Trouble1.vue')
          }
        ]
      }, {
        path: 'planChart',
        component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/PlanChart/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'planChart'
          }, {
            path: 'planChart',
            name: 'PlanChart',
            component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/PlanChart/children/PlanChart.vue')
          }
        ]
      }, {
        path: 'stockChart',
        component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/StockChart/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'stockChart'
          }, {
            path: 'stockChart',
            name: 'StockOper',
            component: () => import(/* webpackChunkName: "system" */ '@/views/StatisticsReport/StockChart/children/StockOper.vue')
          }
        ]
      }
    ]
  }
]
export default routes
