/**
 * 系统设置
 */
const nav = {
    path: '/system',
    name: 'system',
    meta: { navName: '系统管理' },
    component: () => import(/* webpackChunkName: "system" */ '@/views/SystemSetting/SystemSetting.vue'),
    children: [{
        path: 'user',
        name: 'user',
        meta: { navName: '人员管理' },
        component: () => import(/* webpackChunkName: "system" */ '@/views/SystemSetting/UserManager/User.vue')
    }, {
        path: 'org',
        name: 'org',
        meta: { navName: '组织机构' },
        component: () => import(/* webpackChunkName: "system" */ '@/views/SystemSetting/OrgManager/Org.vue'),
        children: [{
            path: 'company',
            name: 'company',
            meta: { navName: '公司' },
            component: () => import(/* webpackChunkName: "system" */ '@/views/SystemSetting/OrgManager/children/Company.vue')
        }, {
            path: 'section',
            name: 'section',
            meta: { navName: '部门' },
            component: () => import(/* webpackChunkName: "system" */ '@/views/SystemSetting/OrgManager/children/Section.vue')
        }, {
            path: 'team',
            name: 'team',
            meta: { navName: '班组' },
            component: () => import(/* webpackChunkName: "system" */ '@/views/SystemSetting/OrgManager/children/Team.vue')
        }, {
            path: 'see',
            name: 'see',
            meta: { navName: '查看' },
            component: () => import(/* webpackChunkName: "system" */ '@/views/SystemSetting/OrgManager/children/See.vue')
        }]
    }]
}

export default nav
