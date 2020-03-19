/**
 * 设备台账
 */
const routes = [
  {
    path: '/equipmentManager',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/EquipmentManager.vue'),
    children: [
      {
        path: '/',
        redirect: 'equipment'
      }, {
        path: 'eqpType',
        component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/eqpType/Index.vue'),
        children: [
          {
            path: '/',
            name: 'eqpType',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeeEqpTypeList',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/equipmentManager/eqpType/children/SeeEqpTypeList.vue')
          }, {
            path: 'add',
            name: 'AddEqpType',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/equipmentManager/eqpType/children/AddEqpType.vue')
          }
        ]
      },{
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
        path: 'equipment',
        component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/equipment/Index.vue'),
        children: [
          {
            path: '/',
            name: 'equipment',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeeEqpList',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/equipment/children/SeeEqpList.vue')
          }, {
            path: 'add',
            name: 'AddEqp',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/equipment/children/AddEqp.vue')
          }, {
            path: 'detail',
            name: 'DetailEqp',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/equipment/children/DetailEqp.vue')
          }
        ]
      }, {
        path: 'eqpRepair',
        component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/firm/Index.vue'),
        children: [
          {
            path: '/',
            name: 'eqpRepair',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeeEqpRepairList',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/eqpRepair/children/SeeEqpRepairList.vue')
          }, {
            path: 'add',
            name: 'AddEqpRepair',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/eqpRepair/children/AddEqpRepair.vue')
          }
        ]
      }, {
        path: 'firm',
        component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/firm/Index.vue'),
        children: [
          {
            path: '/',
            name: 'firm',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeeFirmList',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/firm/children/SeeFirmList.vue')
          }, {
            path: 'add',
            name: 'AddFirm',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/firm/children/AddFirm.vue')
          }
        ]
      }, {
        path: 'pidtable',
        component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/pidtable/Index.vue'),
        children: [
          {
            path: '/',
            name: 'pidtable',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeePidList',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/pidtable/children/SeePidList.vue')
          }, {
            path: 'add',
            name: 'AddPid',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/pidtable/children/AddPid.vue')
          }, {
            path: 'detail',
            name: 'DetailPid',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/pidtable/children/DetailPid.vue')
          }
        ]
      }, {
        path: 'pidcount',
        component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/pidcount/Index.vue'),
        children: [
          {
            path: '/',
            name: 'pidcount',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeePidCountList',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/pidcount/children/SeePidCountList.vue')
          }, {
            path: 'add',
            name: 'AddPidCount',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/pidcount/children/AddPidCount.vue')
          }, {
            path: 'detail',
            name: 'DetailPidCount',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/pidcount/children/DetailPidCount.vue')
          }, {
            path: 'adddetail',
            name: 'AddPidCountDetail',
            component: () => import(/* webpackChunkName: "equipmentManager" */ '@/views/equipmentManager/pidcount/children/AddPidCountDetail.vue')
          }
        ]
      }
    ]
  }
]
export default routes
