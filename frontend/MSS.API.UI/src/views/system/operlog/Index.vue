<template>
  <div class="wrap height-full">
    <div class="con-padding-horizontal header right">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active" v-show="title !== '| 角色管理'">
        <router-link :to="{name:'SeeRoleList'}">返回</router-link>
      </x-button>
    </div>
    <router-view @title="getSubTitle"/>
  </div>
</template>
<script>
import SeeRoleList from './children/SeeRoleList'
import AddRole from './children/AddRole'
import XButton from '@/components/button'
export default {
  name: 'Role',
  data () {
    return {
      title: '| 角色管理'
      // subTitle: ''
    }
  },
  components: {
    SeeRoleList,
    AddRole,
    XButton
  },
  created () {
  },
  methods: {
    getSubTitle (val) {
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
