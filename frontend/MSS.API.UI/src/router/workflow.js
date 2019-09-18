/**
 * 工作流
 */
const routes = [
  {
    path: '/workflow',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "system" */ '@/views/system/System.vue'),
    children: [
      {
        path: '/',
        redirect: 'Design'
      }, 
        {
          path: 'Design',
          component: () => import( '@/views/system/operlog/index.vue'),
          children: [
            {
              path: '/',
              name: 'Design',
              redirect: 'list'
            }, 
            {
              path: 'list',
              name: 'SeeWorkFlowDesign',
              component: () => import(/* webpackChunkName: "system" */ '@/views/system/operlog/children/SeeWorkFlowDesign.vue')
            }
          ]
        }
    ]
  }
]
export default routes
