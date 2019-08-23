/**
 * 设备台账
 */
const routes = [
  {
    path: '/warehouseManager',
    meta: { validate: true },
    component: () => import(/* webpackChunkName: "warehouseManager" */ '@/views/warehouseManager/warehouseManager.vue'),
    children: [
      {
        path: '/',
        redirect: 'warehouse'
      }, {
        path: 'warehouse',
        component: () => import(/* webpackChunkName: "warehouseManager" */ '@/views/warehouseManager/warehouse/Index.vue'),
        children: [
          {
            path: '/',
            name: 'warehouse',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeeWarehouseList',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/warehouse/children/SeeWarehouseList.vue')
          }, {
            path: 'add',
            name: 'AddWarehouse',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/warehouse/children/AddWarehouse.vue')
          }
        ]
      }, {
        path: 'spareParts',
        component: () => import(/* webpackChunkName: "warehouseManager" */ '@/views/warehouseManager/spareParts/Index.vue'),
        children: [
          {
            path: '/',
            name: 'spareParts',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeeSparePartsList',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/spareParts/children/SeeSparePartsList.vue')
          }, {
            path: 'add',
            name: 'AddSpareParts',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/spareParts/children/AddSpareParts.vue')
          }, {
            path: 'detail',
            name: 'DetailSpareParts',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/spareParts/children/DetailSpareParts.vue')
          }, {
            path: 'setAlarm',
            name: 'SetWarehouseAlarm',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/spareParts/children/SetWarehouseAlarm.vue')
          }, {
            path: 'addAlarm',
            name: 'AddWarehouseAlarm',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/spareParts/children/AddWarehouseAlarm.vue')
          }
        ]
      }, {
        path: 'stockOperation',
        component: () => import(/* webpackChunkName: "warehouseManager" */ '@/views/warehouseManager/stockOperation/Index.vue'),
        children: [
          {
            path: '/',
            name: 'stockOperation',
            redirect: 'list'
          }, {
            path: 'list',
            name: 'SeeStockReceiveList',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/stockOperation/children/SeeStockReceiveList.vue')
          }, {
            path: 'add',
            name: 'AddStockReceive',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/stockOperation/children/AddStockReceive.vue')
          }, {
            path: 'detail',
            name: 'DetailStockOperation',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/stockOperation/children/DetailStockOperation.vue')
          }, {
            path: 'listDelivery',
            name: 'SeeStockDeliveryList',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/stockOperation/children/SeeStockDeliveryList.vue')
          }, {
            path: 'addDelivery',
            name: 'AddStockDelivery',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/stockOperation/children/AddStockDelivery.vue')
          }, {
            path: 'listAdjust',
            name: 'SeeStockAdjustList',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/stockOperation/children/SeeStockAdjustList.vue')
          }, {
            path: 'addAdjust',
            name: 'AddStockAdjust',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/stockOperation/children/AddStockAdjust.vue')
          }, {
            path: 'listMove',
            name: 'SeeStockMoveList',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/stockOperation/children/SeeStockMoveList.vue')
          }, {
            path: 'addMove',
            name: 'AddStockMove',
            component: () => import(/* webpackChunkName: "equipment" */ '@/views/warehouseManager/stockOperation/children/AddStockMove.vue')
          }
        ]
      }
    ]
  }
]
export default routes
