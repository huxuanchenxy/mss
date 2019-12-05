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
        redirect: 'ConstructionPlan/115'
      }, 
      {
        path: 'ConstructionPlan/115',
        component: () => import( '@/views/workflow/process/index.vue'),
        children: [
          {
            path: '/',
            name: 'SeeConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/SeeConstructionPlan.vue')
          }, 
          {
            path: 'add',
            name: 'AddConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/AddConstructionPlan.vue')
          }
        ]
      }, 
      {
        path: 'ConstructionPlan/153',
        component: () => import( '@/views/workflow/process/index.vue'),
        children: [
          {
            path: '/',
            name: 'SeeConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/SeeConstructionPlan.vue')
          }, 
          {
            path: 'add',
            name: 'AddConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/AddConstructionPlan.vue')
          }
        ]
      }, 
      {
        path: 'ConstructionPlan/154',
        component: () => import( '@/views/workflow/process/index.vue'),
        children: [
          {
            path: '/',
            name: 'SeeConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/SeeConstructionPlan.vue')
          }, 
          {
            path: 'add',
            name: 'AddConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/AddConstructionPlan.vue')
          }
        ]
      }, 
      {
        path: 'ConstructionPlan/155',
        component: () => import( '@/views/workflow/process/index.vue'),
        children: [
          {
            path: '/',
            name: 'SeeConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/SeeConstructionPlan.vue')
          }, 
          {
            path: 'add',
            name: 'AddConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/AddConstructionPlan.vue')
          }
        ]
      }, 
      {
        path: 'ConstructionPlan/156',
        component: () => import( '@/views/workflow/process/index.vue'),
        children: [
          {
            path: '/',
            name: 'SeeConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/SeeConstructionPlan.vue')
          }, 
          {
            path: 'add',
            name: 'AddConstructionPlan',
            component: () => import(/* webpackChunkName: "system" */ '@/views/workflow/process/children/AddConstructionPlan.vue')
          }
        ]
      }, 
      {
        path: 'ConstructionPlan/157',
        component: () => import( '@/views/workflow/process/index.vue'),
        children: [
          {
            path: '/',
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
