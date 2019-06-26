<template>
  <div class="wrap">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <span class="left">
          <!-- <img :src="$router.navList[$route.matched[0].path].TitleIcon" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].GroupName }} -->
           {{ pageInfo.title ? pageInfo.title : '' }}
        </span>
      </h2>
      <x-button v-show="pageInfo.isShowBackBtn" class="active">
        <router-link class="color-white block" :to="{ name: pageInfo.url }">返回</router-link>
      </x-button>
    </div>
    <div class="content">
      <!-- <keep-alive include="OrgList">
        <router-view @pageInfo="getPageTitle"/>
      </keep-alive> -->
      <router-view :key="key" @pageInfo="getPageTitle"/>
    </div>
  </div>
</template>
<script>
import XButton from '@/components/button'
export default {
  name: 'Organization',
  components: {
    XButton
  },
  data () {
    return {
      pageInfo: {
        title: '',
        isShowBackBtn: false,
        url: ''
      }
    }
  },
  computed: {
    key () {
      return this.$route.name !== undefined ? this.$route.name + new Date() : this.$route + new Date()
    }
  },
  methods: {
    getPageTitle (val) {
      this.pageInfo = val
    }
  }
}
</script>
<style lang="scss" scoped>
.wrap{
  height: 100%;
  .header{
    display: flex;
    justify-content: space-between;
  }
  .content{
    height: percent($content-height - 56, $content-height);
  }
}
</style>
