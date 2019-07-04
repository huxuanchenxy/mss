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
                }
              ]
            }
          ]
        }
      ]
  export default routes
  