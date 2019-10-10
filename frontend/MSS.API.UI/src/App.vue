<template>
  <login v-if="isLoadingCompleted === '1'"></login>
  <div v-else-if="isLoadingCompleted === '2'" id="app">
    <app-header></app-header>
    <app-sidebar></app-sidebar>
    <div id="main" class="shrink">
      <router-view/>
    </div>
    <app-footer></app-footer>
  </div>
  <index v-else-if="isLoadingCompleted === '3'"></index>
</template>

<script>
import AppHeader from '@/components/header/AppHeader'
import AppSidebar from '@/components/sidebar/AppSidebar'
import AppFooter from '@/components/footer/AppFooter'
import Login from '@/views/login/Login'
import Index from '@/views/Index'
export default {
  name: 'App',
  data () {
    return {
      loading: null
    }
  },
  components: {
    AppHeader,
    AppSidebar,
    AppFooter,
    Login,
    Index
  },
  computed: {
    isLoadingCompleted () {
      const loading = this.$loading({
        text: '加载中...',
        spinner: 'el-icon-loading'
      })
      if (this.$route.name === null) {
        return '0'
      } else if (this.$route.name === 'Login') {
        loading.close()
        return '1'
      } else if (this.$route.name === 'Index') {
        loading.close()
        return '3'
      } else {
        loading.close()
        return '2'
      }
    }
  }
  // 测试用放用户相关信息,登录界面完成后可删除
  // created () {
  //   window.sessionStorage.setItem('UserName', '童伟光')
  //   window.sessionStorage.setItem('UserID', 'twg')
  // }
}
</script>

<style lang="scss">
#app{
  width: 100%;
  height: 100%;
}
#main{
  position: relative;
  top: 1%;
  // left: 47%;
  width: 93%;
  // height: percent(630, 780);
  height: 80%;
  border-radius: $border-radius;
  // transform: translateX(percent(-960 / 2, 960));
  bottom:6%;
  padding-left: 2%;
  &.shrink{
    // left: 44%;
    width: 93%;
    // transform: translateX(percent(-960 / 2, 1100));
  }

  > .wrap{
    height: 100%;
    background: $color-main-background;

    // 标题
    .header{
      display: flex;
      align-items: center;
      position: relative;
      height: percent(30, $content-height);

      &:after{
        content: "";
        position: absolute;
        bottom: -7px;
        left: 0;
        width: 100%;
        height: 11px;
        background: url(./common/images/line.png) no-repeat 0 0/100% 100%;
      }

      .title{
        font-size: PXtoEm(18);
      }

      .icon{
        width: 22px;
        height: 22px;
        vertical-align: bottom;
      }

      .btn{
        margin-left: PXtoEm(20);
      }
    }
  }
}
</style>
<style>
  .box{
      height: 16.36364%  !important
    }
    .content-wrap{
      height: 81.45455% !important
    }
    .content-header{
      height: 10.45455% !important
    }
</style>
