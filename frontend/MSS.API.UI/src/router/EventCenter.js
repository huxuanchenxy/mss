/**
 * 设备生命周期
 */
const routes = [ 
    {  
      path: '/eventcenter',
      meta: { validate: true },
      component: () => import(/* webpackChunkName: "eventcenter" */ '@/views/EventCenter/EventCenter.vue'),
      children: [
        {
          path: '/',
          redirect: 'monitor'
        },
        {
          path: 'monitor',
          component: () => import(/* webpackChunkName: "eventcenter" */ '@/views/EventCenter/Monitor/Index.vue'),
          children: [
            {
              path: '/',
              name: 'Monitor',
              redirect: 'alarm'
            }, {
              path: 'alarm/:type?',
              name: 'alarm',
              component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/EventCenter/Monitor/children/Alarm.vue')
            }
          ]
        },
        {
          path: 'history',
          component: () => import(/* webpackChunkName: "eventcenter" */ '@/views/EventCenter/History/Index.vue'),
          children: [
            {
              path: '/',
              name: 'EventHistory',
              redirect: 'alarmHistory'
            }, {
              path: 'alarmHistory/:type?',
              name: 'alarmHistory',
              component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/EventCenter/History/children/AlarmHistory.vue')
            }
          ]
        }
      ]
    }
  ]
export default routes
