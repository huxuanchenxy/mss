<template>
  <login v-if="isLoadingCompleted === '1'"></login>
  <div v-else-if="isLoadingCompleted === '2'" id="app">
    <app-header></app-header>
    <app-sidebar></app-sidebar>
    <div id="main">
      <router-view/>
    </div>
    <app-footer></app-footer>
  </div>
</template>

<script>
import AppHeader from '@/components/header/AppHeader'
import AppSidebar from '@/components/sidebar/AppSidebar'
import AppFooter from '@/components/footer/AppFooter'
import Login from '@/views/login/Login'
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
    Login
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
  position: fixed;
  top: 15%;
  left: 50%;
  width: percent(960, 1440);
  height: percent(631, 780);
  border-radius: $border-radius;
  transform: translateX(percent(-960 / 2, 960));

  &.shrink{
    width: percent(1100, 1440);
    transform: translateX(percent(-960 / 2, 1100));
  }

  > .wrap{
    height: 100%;
    background: $color-main-background;

    // 标题
    .header{
      display: flex;
      align-items: center;
      position: relative;
      height: percent(25, $content-height);

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
      height: 78.45455% !important
    }
</style>
