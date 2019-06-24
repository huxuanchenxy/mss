<template>
  <div class="wrap">
    <div class="con-padding-horizontal header right">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].TitleIcon" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].GroupName }} {{ title }}
      </h2>
      <x-button class="active" v-show="title !== '| 用户定义'">
        <router-link :to="{name:'SeeUserList'}">返回</router-link>
      </x-button>
    </div>
    <router-view @title="getChildren"/>
  </div>
</template>
<script>
import XButton from '@/components/button'
export default {
  name: 'User',
  data () {
    return {
      title: '| 用户定义'
      // subTitle: ''
    }
  },
  components: {
    XButton
  },
  methods: {
    // 获取副标题
    getChildren (val) {
      this.title = val
    }
  }
}
</script>
<style lang="scss" scoped>
.right{
  display: flex;
  justify-content: space-between;
}
</style>
