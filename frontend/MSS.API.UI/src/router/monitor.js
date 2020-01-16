/**
 * 网络监控
 */
const routes = [
  {
    path: '/monitor',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "system" */ '@/views/system/System.vue'),
    children: [
      {
        path: '/',
        redirect: 'ProcessConsul'
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
        }
    ]
  }
]
export default routes
