/**
 * 故障管理
 */
const routes = [
  {
    path: '/troubleManager',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/troubleManager.vue'),
    children: [
      {
        path: '/',
        redirect: 'list'
      }, {
        path: 'list',
        component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/Index.vue'),
        children: [
          {
            path: '/',
            name: 'TroubleList',
            redirect: 'troubleList'
          }, {
            path: 'troubleList',
            name: 'SeeTroubleList',
            component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/children/SeeTroubleList.vue')
          }, {
            path: 'addTrouble',
            name: 'AddTrouble',
            component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/children/AddTrouble.vue')
          }
        ]
      }
    ]
  }
]
export default routes
