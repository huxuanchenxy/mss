/**
 * 施工管理
 */
const routes = [
  {
    path: '/constructionManager',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "constructionManager" */ '@/views/constructionManager/constructionManager.vue'),
    children: [
      {
        path: '/',
        redirect: 'import'
      }, {
        path: 'import',
        component: () => import(/* webpackChunkName: "constructionManager" */ '@/views/constructionManager/import/Index.vue'),
        children: [
          {
            path: '/',
            name: 'import',
            redirect: 'importList'
          }, {
            path: 'importList',
            name: 'ImportList',
            component: () => import(/* webpackChunkName: "constructionManager" */ '@/views/constructionManager/import/children/List.vue')
          }, {
            path: 'importExl',
            name: 'ImportExl',
            component: () => import(/* webpackChunkName: "constructionManager" */ '@/views/constructionManager/import/children/import.vue')
          }
        ]
      }, {
        path: 'monthDetail',
        component: () => import(/* webpackChunkName: "constructionManager" */ '@/views/constructionManager/monthDetail/Index.vue'),
        children: [
          {
            path: '/',
            name: 'monthDetail',
            redirect: 'commonList'
          }, {
            path: 'commonList',
            name: 'CommonList',
            component: () => import(/* webpackChunkName: "constructionManager" */ '@/views/constructionManager/monthDetail/children/List.vue')
          }, {
            path: 'monthList',
            name: 'MonthList',
            component: () => import(/* webpackChunkName: "constructionManager" */ '@/views/constructionManager/monthDetail/children/monthList.vue')
          }, {
            path: 'update',
            name: 'UpdateMonthPlan',
            component: () => import(/* webpackChunkName: "constructionManager" */ '@/views/constructionManager/monthDetail/children/update.vue')
          }
        ]
      }
    ]
  }
]
export default routes
