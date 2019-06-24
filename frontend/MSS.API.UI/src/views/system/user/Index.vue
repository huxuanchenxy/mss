<template>
  <div class="wrap height-full">
    <div class="content">
      <keep-alive include="SeeUserList">
        <router-view @title="getTitle"/>
      </keep-alive>
    </div>
  </div>
</template>
<script>
import XButton from '@/components/button'
export default {
  name: 'User',
  components: {
    XButton
  },
  data () {
    return {
      title: ''
    }
  },
  methods: {
    getTitle (val) {
      this.title = val
    }
  }
}
</script>
<style lang="scss" scoped>
.wrap{
  /deep/ .header{
    display: flex;
    justify-content: space-between;
  }
  .content{
    height: 100%;
  }

  // 子组件下边的按钮加搜索
  /deep/ .box{
    height: percent(145, $content-height);

    // 搜索组
    .search-wrap{
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

    .btn-group{
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
}
</style>
