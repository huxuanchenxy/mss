<template>
  <div class="wrap height-full">
    <div class="content">
      <keep-alive include="SeePidList">
        <router-view @title="getTitle"/>
      </keep-alive>
    </div>
  </div>
</template>
<script>
import XButton from '@/components/button'
export default {
  name: 'pidtable',
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

  // 图片预览
  /deep/ .show-list-wrap{
    position: absolute;
    z-index: 9;
    top: 0;
    left: 0;
    overflow: hidden;
    width: 100%;
    height: 100%;
    background: #1c1c1c;
    transform: translate(0px);

    .el-carousel{
      height: 100%;
    }

    img{
      max-width: 100%;
      height: 100%;
    }

    .icon-close{
      position: absolute;
      z-index: 99;
      top: 3px;
      right: 3px;
      width: 27px;
      height: 27px;
      background: url(../../../common/images/icon-close.png) no-repeat 0 0/100% 100%;
      cursor: pointer;
    }

    .el-dialog__header{
      display: block;
      padding: 0;

      .el-dialog__headerbtn{
        top: -14px;
        right: -14px;
        width: 27px;
        height: 27px;
        background: url(../../../common/images/icon-close.png) no-repeat 0 0/100% 100%;
      }

      .el-dialog__close{
        display: none;
      }
    }

    .el-dialog__body{
      height: 100%;
    }
  }
}
</style>
