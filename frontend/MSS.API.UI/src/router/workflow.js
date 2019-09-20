/**
 * 工作流
 */
const routes = [
  {
    path: '/workflow',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/workflow.vue'),
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
        }, 
        {
          path: 'ProcessTask',
          component: () => import( '@/views/workflow/process/index.vue'),
          children: [
            {
              path: '/',
              name: 'ProcessTask',
              redirect: 'list'
            }, 
            {
              path: 'list',
              name: 'SeeProcessTask',
              component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/SeeProcessTask.vue')
            }
          ]
        }, 
        {
          path: 'MyApply',
          component: () => import( '@/views/workflow/process/index.vue'),
          children: [
            {
              path: '/',
              name: 'MyApply',
              redirect: 'list'
            }, 
            {
              path: 'list',
              name: 'SeeProcessMyApply',
              component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/SeeProcessMyApply.vue')
            }
          ]
        }, 
        {
          path: 'ProcessHis',
          component: () => import( '@/views/workflow/process/index.vue'),
          children: [
            {
              path: '/',
              name: 'ProcessHis',
              redirect: 'list'
            }, 
            {
              path: 'list',
              name: 'SeeProcessHis',
              component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/SeeProcessHis.vue')
            }
          ]
        }
    ]
  }
]
export default routes
