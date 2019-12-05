// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
// import Axios from './service/http'
import ElementUI from 'element-ui'
import './common/scss/global.scss'
import '../theme/index.css'

import LightTimeline from 'vue-light-timeline'
// import Charts from '@/components/charts/index'

// 引入 ECharts 主模块
import echarts from 'echarts/lib/echarts'

// 具体可按需引入模块：https://github.com/apache/incubator-echarts/blob/master/index.js
// 引入柱状图
import 'echarts/lib/chart/bar'

// 引入折线图
import 'echarts/lib/chart/line'

// // 引入折线图
import 'echarts/lib/chart/pie'

// // 引入气泡图
import 'echarts/lib/chart/scatter'

import 'echarts/lib/chart/radar'

import 'echarts/lib/chart/gauge'
// // 引入提示框和标题组件
import 'echarts/lib/component/tooltip'
import 'echarts/lib/component/title'
import 'echarts/lib/component/dataZoom'
import 'echarts/lib/component/toolbox'
import 'echarts/lib/component/legend'

// 把components/charts目录下的组件都注册为注册全局组件
// Vue.use(Charts)

Vue.use(ElementUI)
Vue.use(LightTimeline)
Vue.config.productionTip = false
Vue.prototype.$echarts = echarts
// window.axios = Axios

/* eslint-disable no-new */
window.Vue = new Vue({
  el: '#app',
  router,
  components: { App },
  template: '<App/>'
})
