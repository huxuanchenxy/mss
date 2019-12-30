// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import MuseUI from 'muse-ui'
import { Swipe, SwipeItem } from 'vue-swipe'
import 'muse-ui/dist/muse-ui.css'
import 'muse-ui/dist/theme-teal.css'
import scroll from 'vue-infinite-scroll'
import BaiduMap from 'vue-baidu-map'
import silder from "./components/commom/slide.vue"
import ElementUI from 'element-ui'
import './common/scss/global.scss'
import '../theme/index.css'
import VueEmoji from "vue-emoji"

// excel在线编辑插件handsontable
import 'handsontable/dist/handsontable.full.css'

// Vue.use(silder)
//引用拖拽插件
import VueDND from 'awe-dnd'
Vue.use(ElementUI)
Vue.use(VueEmoji)
Vue.use(VueDND)
Vue.use(MuseUI)
Vue.use(scroll)
var infiniteScroll =  require('vue-infinite-scroll')
Vue.use(infiniteScroll)
Vue.use(BaiduMap, {
  // ak 是在百度地图开发者平台申请的密钥 详见 http://lbsyun.baidu.com/apiconsole/key */
  ak: 'ZZunCHGz66gcNItI8ILT6j2AI6EjTBhK'
})
Vue.config.productionTip = false
import MintUI from 'mint-ui'
import 'mint-ui/lib/style.css'

Vue.use(MintUI)
/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  template: '<App/>',
  components: { App }
})
