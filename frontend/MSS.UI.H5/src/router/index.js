import Vue from 'vue'
import Router from 'vue-router'

import home from "../components/home.vue"
import mine from "../components/mine.vue";
import trouble from "../components/trouble.vue"
import warning from "../components/warning.vue"
import my from "../components/my.vue"
import mysetting from "../components/mysetting.vue"
import troublelist from "../components/troublelist.vue"
import inputCheck from "../components/inputCheck.vue"
Vue.use(Router)

export default new Router({
  mode: 'hash',
  routes: [
    {
      path: '/',
      redirect: 'trouble'
    },
    {
      path: '/home',
      name: 'home',
      component: home
    },
    {
      path: '/trouble',
      name: 'trouble',
      component: trouble
    },
    {
      path: '/troublelist',
      name: 'troublelist',
      component: troublelist
    },
    {
      path: '/inputCheck',
      name: 'inputCheck',
      component: inputCheck
    },
    {
      path: '/warning',
      name: 'warning',
      component: warning
    },    
    {
      path: '/my',
      name: 'my',
      component: my
    }
        ,
    {
      path: '/mysetting',
      name: 'mysetting',
      component: mysetting
    }
    ,
    {
      path: '/login',
      name: 'Login',
      meta: { login: true },
      component: () => import(/* webpackChunkName: "login" */ '@/views/login/Login.vue')
    },
    {
      path: '/Password',
      name: 'Password',
      meta: { login: true },
      component: () => import(/* webpackChunkName: "login" */ '@/views/login/Password.vue')
    }
  //   {
  //     path: '/vuecommunitytest/personal',
  //     name: 'personal',
  //     component: personal
  //   },
  //   {
  //     path: '/vuecommunitytest/message',
  //     name: 'message',
  //     component: message
  //   }
  ]
})
