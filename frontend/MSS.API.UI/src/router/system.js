/**
 * 系统管理
 */
const routes = [
  {
    path: '/system',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "system" */ '@/views/system/System.vue'),
    children: [
      {
        path: '/',
        redirect: 'user'
      }, {
        path: 'user',
        component: () => import(/* webpackChunkName: "system" */ '@/views/system/user/Index.vue'),
        children: [
          {
            path: '/',
            name: 'User',
            redirect: 'list'
          }, {
            path: 'list/:roleID?',
            name: 'SeeUserList',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/user/children/SeeUserList.vue')
          }, {
            path: 'add',
            name: 'AddUser',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/user/children/AddUser.vue')
          }
        ]
      }, {
        path: 'role',
        component: () => import(/* webpackChunkName: "system" */ '@/views/system/role/Index.vue'),
        children: [
          {
            path: '/',
            name: 'Role',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeeRoleList',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/role/children/SeeRoleList.vue')
          }, {
            path: 'add',
            name: 'AddRole',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/role/children/AddRole.vue')
          }
        ]
      }, {
        path: 'code',
        component: () => import(/* webpackChunkName: "system" */ '@/views/system/code/Index.vue'),
        children: [
          {
            path: '/',
            name: 'Code',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeeCodeList',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/code/children/SeeCodeList.vue')
          }, {
            path: 'add',
            name: 'AddCode',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/code/children/AddCode.vue')
          }
        ]
      }, {
        path: 'log',
        name: 'Log',
        component: () => import(/* webpackChunkName: "system" */ '@/views/system/log/Index.vue')
      }, {
        path: 'organization',
        component: () => import(/* webpackChunkName: "system" */ '@/views/system/organization/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'OrgList',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/organization/children/List.vue')
          }, {
            path: 'user/:id?',
            name: 'OrgUser',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/organization/children/User.vue')
          }, {
            path: 'node/:id?',
            name: 'OrgNode',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/organization/children/AddNode.vue')
          }
        ]
      }, {
        path: 'actionGroup',
        component: () => import(/* webpackChunkName: "system" */ '@/views/system/actionGroup/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'list'
          }, {
            path: 'list/:id?',
            name: 'SeeActionGroupList',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/actionGroup/children/SeeActionGroupList.vue')
          }, {
            path: 'addActionGroup/:mark?/:id?',
            name: 'AddActionGroup',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/actionGroup/children/AddActionGroup.vue')
          }
        ]
      }, {
        path: 'action',
        component: () => import(/* webpackChunkName: "system" */ '@/views/system/action/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'list'
          }, {
            path: 'list/:id?',
            name: 'SeeActionList',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/action/children/SeeActionList.vue')
          }, {
            path: 'addAction/:mark?/:id?',
            name: 'AddAction',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/action/children/AddAction.vue')
          }
        ]
      }, {
        path: 'warnsetting',
        component: () => import(/* webpackChunkName: "system" */ '@/views/system/warnsetting/Index.vue'),
        children: [
          {
            path: '/',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'WarnSettingList',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/warnsetting/children/List.vue')
          }, {
            path: 'setting/:id?',
            name: 'WarnSetting',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/warnsetting/children/AddWarnSetting.vue')
          }
        ]
      }, {
        path: 'Area',
        component: () => import(/* webpackChunkName: "system" */ '@/views/system/Area/index.vue'),
        children: [
          {
            path: '/',
            name: 'SmallArea',
            redirect: 'list'
          },
          {
            path: 'list',
            name: 'SmallAreaList',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/Area/SmallArea/SmallAreaList.vue')
          },
          {
            path: 'AddSmallArea/:mark?/:id?',
            name: 'AddSmallArea',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/Area/SmallArea/AddSmallArea.vue')
          }
        ]
      },{
        path: 'operlog',
        component: () => import(/* webpackChunkName: "system" */ '@/views/system/operlog/index.vue'), 
        children: [
          {
            path: '/',
            redirect: 'list'
          }, 
          {
            path: 'list',
            name: 'SeeOperlogList',
            component: () => import(/* webpackChunkName: "system" */ '@/views/system/operlog/children/SeeOperlogList.vue')
          }
      ]
    }
    ]
  }
]
export default routes
