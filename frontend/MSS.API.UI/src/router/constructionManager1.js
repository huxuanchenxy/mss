/**
 * 施工管理
 */
const routes = [
  {
    path: '/constructionManager1',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "constructionManager" */ '@/views/workflow/workflow.vue'),
    children: [
      {
        path: '/',
        redirect: 'ConstructionPlan'
      }, 
      {
        path: 'ConstructionPlan',
        component: () => import( '@/views/workflow/process/index.vue'),
        children: [
          {
            path: '/',
            name: 'ConstructionPlan',
            redirect: 'list'
          }, 
          {
            path: 'list',
            name: 'SeeConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/SeeConstructionPlan.vue')
          }, 
          {
            path: 'add',
            name: 'AddConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/AddConstructionPlan.vue')
          }
        ]
      }
    ]
  }
]
export default routes
