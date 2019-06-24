<template>
  <div class="wrap height-full">
    <div class="con-padding-horizontal header right">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active" v-show="title !== '| 代码管理'">
        <router-link :to="{name:'SeeCodeList'}">返回</router-link>
      </x-button>
    </div>
    <router-view @title="getSubTitle"/>
  </div>
</template>
<script>
import XButton from '@/components/button'
export default {
  name: 'division',
  data () {
    return {
      title: '| 代码管理'
    }
  },
  components: {
    XButton
  },
  methods: {
    getSubTitle (val) {
      this.title = val
    }
  }
}
</script>
<style lang="scss" scoped>
.wrap{
  height: 100%;
  /deep/ .box{
    height: percent(145, $content-height);
  }

  // 搜索组
  /deep/ .search-wrap{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: percent(80, 145);
    background: rgba(128, 128, 128, 0.1);
    color: $color-white;

    .wrap{
      display: flex;
    }

    .input-group{
      display: inherit;
      align-items: center;
      margin-right: PXtoEm(24);
    }

    .inp{
      width: PXtoEm(160);
      margin-left: PXtoEm(14);
    }

    .btn{
      border: 0;
      background: $color-main-btn;
    }
  }

  /deep/ .btn-group{
    display: flex;
    align-items: center;
    height: percent(65, 145);

    .list{
      margin-right: PXtoEm(10);
    }

    .btn{
      &:hover{
        border-color: $color-main-btn;
        background: $color-main-btn;
      }
    }
  }
}

.right{
  display: flex;
  justify-content: space-between;
}
</style>
