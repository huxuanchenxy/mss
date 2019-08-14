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
      }
    ]
  }
]
export default routes
