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
            name: 'list',
            redirect: 'troubleList'
          }, {
            path: 'troubleList',
            name: 'SeeTroubleList',
            component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/children/SeeTroubleList.vue')
          }, {
            path: 'addTrouble',
            name: 'AddTrouble',
            component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/children/AddTrouble.vue')
          }, {
            path: 'myRepair',
            name: 'MyRepair',
            component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/children/MyRepair.vue')
          }, {
            path: 'myCheck',
            name: 'MyCheck',
            component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/children/MyCheck.vue')
          }, {
            path: 'troubleHistory',
            name: 'TroubleHistory',
            component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/children/TroubleHistory.vue')
          }, {
            path: 'detailTroubleReport',
            name: 'DetailTroubleReport',
            component: () => import(/* webpackChunkName: "EquipmentLifeCycle" */ '@/views/troubleManager/trouble//children/DetailTroubleReport.vue')
          }, {
            path: 'myProcessing',
            name: 'MyProcessing',
            component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/children/MyProcessing.vue')
          }, {
            path: 'assignEqp',
            name: 'AssignEqp',
            component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/children/AssignEqp.vue')
          }, {
            path: 'dealTrouble',
            name: 'DealTrouble',
            component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/children/DealTrouble.vue')
          }, {
            path: 'myPreCheck',
            name: 'MyPreCheck',
            component: () => import(/* webpackChunkName: "troubleManager" */ '@/views/troubleManager/trouble/children/MyPreCheck.vue')
          }
        ]
      }
    ]
  }
]
export default routes
