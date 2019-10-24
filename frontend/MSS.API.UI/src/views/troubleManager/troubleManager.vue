<template>
  <router-view/>
</template>
<script>
export default {
  name: 'troubleManager',
  data () {
    return {

    }
  }
}
</script>
<style lang="scss" scoped>
// 上传图片
  /deep/ .upload-wrap{
    margin: 20px 0;

    .el-upload{
      position: relative;
      overflow: hidden;
      width: 80px;
      height: 80px;
      border: 0;
      background: #36363C;
      border-radius: 3px;
      font-size: 22px;
      text-align: center;
      line-height: 80px;
      cursor: pointer;
    }

    .el-upload-list__item{
      width: 80px;
      height: 80px;
      margin-bottom: 0;
    }

    .el-icon-close-tip{
      display: none !important;
    }
  }

  // 显示大图容器
  /deep/ .el-dialog__wrapper{
    width: 100%;
    height: 100%;

    .el-carousel{
      height: 100%;
    }
  }

  // 图片显示列表没有上传
  /deep/ .picture-list-wrap{
    display: flex;
    align-items: center;
    // margin-top: 30px;
    flex-wrap:wrap;
    .list{
      position: relative;
      margin-right: 10px;

      img{
        vertical-align: top;
        border-radius: 6px;
      }

      &:hover{
        .operation{
          @include opacity(1);
        }
      }
    }

    // 查看和下载按钮
    .operation{
      display: flex;
      align-items: center;
      justify-content: center;
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      @include opacity(0);
      background: rgba(0, 0, 0, 0.5);

      i{
        font-size: 18px;
        cursor: pointer;
      }

      .el-icon-download{
        margin-left: 15px;
        font-size: 20px;
      }

      .el-icon-delete{
        margin-left: 15px;
      }
    }
  }
</style>
