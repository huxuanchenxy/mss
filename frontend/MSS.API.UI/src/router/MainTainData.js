/**
 * 系统管理
 */
const routes = [ 
        {  
          path: '/MainTainData',
          meta: { validate: true },
          component: () => import(/* webpackChunkName: "MaintainData" */ '@/views/MainTainData/MainTainData.vue'),
          children: [
            {
              path: '/',
              redirect: 'Expert'
            },
            {
              path: 'Expert',
              component: () => import(/* webpackChunkName: "MainTainData" */ '@/views/MainTainData/Expert/Index.vue'),
              children: [
                {
                  path: '/',
                  name: 'Expert',
                  redirect: 'list'
                }, {
                  path: 'list:id?',
                  name: 'ExpertDataList',
                  component: () => import(/* webpackChunkName: "MainTainData" */ '@/views/MainTainData/Expert/children/ExpertDataList.vue')
                }, {
                  path: 'AddExpertData/:mark?/:id?',
                  name: 'AddExpertData',
                  component: () => import(/* webpackChunkName: "MainTainData" */ '@/views/MainTainData/Expert/children/AddExpertData.vue')
                }, {
                  path: 'detail/:mark?/:id?',
                  name: 'DetailExpertData',
                  component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/MainTainData/Expert/children/DetailExpertData.vue')
                }
              ]
            },
            {
              path: 'EmergencyPlan',
              component: () => import(/* webpackChunkName: "MainTainData" */ '@/views/MainTainData/EmergencyPlan/Index.vue'),
              children: [
                {
                  path: '/',
                  name: 'EmergencyPlan',
                  redirect: 'list'
                }, {
                  path: 'list',
                  name: 'SeeEmergencyPlanList',
                  component: () => import(/* webpackChunkName: "MainTainData" */ '@/views/MainTainData/EmergencyPlan/children/SeeEmergencyPlanList.vue')
                }, {
                  path: 'Add',
                  name: 'AddEmergency',
                  component: () => import(/* webpackChunkName: "MainTainData" */ '@/views/MainTainData/EmergencyPlan/children/AddEmergency.vue')
                }
              ]
            },
            {
              path: 'regulation',
              component: () => import(/* webpackChunkName: "MainTainData" */ '@/views/MainTainData/regulation/Index.vue'),
              children: [
                {
                  path: '/',
                  name: 'regulation',
                  redirect: 'list'
                }, {
                  path: 'list',
                  name: 'SeeRegulationList',
                  component: () => import(/* webpackChunkName: "MainTainData" */ '@/views/MainTainData/regulation/children/SeeRegulationList.vue')
                }, {
                  path: 'Add',
                  name: 'AddRegulation',
                  component: () => import(/* webpackChunkName: "MainTainData" */ '@/views/MainTainData/regulation/children/AddRegulation.vue')
                }
              ]
            },
            {
              path: 'TechnicalData',
              component: () => import(/* webpackChunkName: "MainTainData" */ '@/views/MainTainData/TechnicalData/Index.vue'),
              children: [
                {
                  path: '/',
                  name: 'TechnicalData',
                  redirect: 'Add'
                }, {
                  path: 'Add',
                  name: 'AddTechnicalData',
                  component: () => import(/* webpackChunkName: "MainTainData" */ '@/views/MainTainData/TechnicalData/children/AddTechnicalData.vue')
                }
              ]
            }
          ]
        }
      ]
  export default routes
  