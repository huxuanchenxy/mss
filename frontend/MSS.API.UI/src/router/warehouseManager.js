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
          }
        ]
      }
    ]
  }
]
export default routes
