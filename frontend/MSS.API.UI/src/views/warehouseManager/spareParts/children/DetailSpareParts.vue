<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeSparePartsList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">物资名称</span>
              <div class="inp">{{spareParts.name}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">物资类型</span>
              <div class="inp">{{spareParts.typeName}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">计量单位</span>
              <div class="inp">{{spareParts.unit}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">规格型号</span>
              <div class="inp">{{spareParts.model}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">适用设备类型</span>
              <div class="inp">{{spareParts.eqpTypeName}}</div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">计划价格(RMB)</span>
              <div class="inp">{{spareParts.planPrice}}</div>
            </div>
          </li>
          <li class="list list-block">
            <div class="inp-wrap">
              <span class="text span-block">英语描述</span>
              <div class="inp">{{spareParts.englishDes}}</div>
            </div>
          </li>
          <li class="list list-block">
            <div class="inp-wrap">
              <span class="text span-block">备注</span>
              <div class="inp word-break">{{spareParts.remark}}</div>
            </div>
          </li>
        </ul>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import api from '@/api/wmsApi'
import XButton from '@/components/button'
export default {
  name: 'DetailSpareParts',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      title: '| 物资详情',
      spareParts: {
        id: this.$route.params.id,
        name: '',
        typeName: '',
        unit: '',
        model: '',
        eqpTypeName: '',
        planPrice: 0,
        englishDes: '',
        remark: ''
      }
    }
  },
  created () {
    this.getSpareParts()
  },
  methods: {
    getSpareParts () {
      api.getSparePartsByID(this.spareParts.id).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.spareParts.id = data.id
          this.spareParts.name = data.name
          this.spareParts.typeName = data.typeName
          this.spareParts.model = data.model
          this.spareParts.unit = data.unit
          this.spareParts.eqpTypeName = data.eqpTypeName
          this.spareParts.planPrice = data.planPrice
          this.spareParts.englishDes = data.englishDes
          this.spareParts.remark = data.remark
        }
        this.loading = false
      }).catch(err => console.log(err))
    }
  }
}
</script>
<style lang="scss" scoped>
// 显示大图容器
.el-dialog__wrapper{
  width: 100%;
  height: 100%;

  .el-carousel{
    height: 100%;
  }
}

.header{
  display: flex;
  justify-content: space-between;
}

// 顶部信息
.middle{
  position: relative;
  margin-bottom: 10px;
  padding-bottom: 20px;

  .text-right{
    text-align: right !important;
  }

  [class*="list-wrap"]{
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list{
      margin-top: 20px;
      padding: 0 2%;
      border-right: 1px solid #C9CACD;
      text-align: center;

      &:first-of-type{
        padding-left: 0;
        text-align: left;
      }

      &:last-of-type{
        padding-right: 0;
        border: 0;
        text-align: right;
      }
    }
  }

  .list-wrap{
    .list{
      width: 16%;

      span{
        display: inline-block;
        width: 100%;
        @extend %ellipsis;
      }
    }
  }

  .sub-list-wrap{
    .list{
      margin-right: 40px;
      padding: 0;
      border: 0;
      text-align: left;
    }

    .text{
      color: $color-content-text;
    }

    &:after{
      content: "";
      flex: auto;
    }
  }
}

.scroll{
  /**
   * percent函数转换百分比
   * $content-height内容区域总高度
   * 页面标题栏高度：56
   */
  height: percent($content-height - 56, $content-height);
  .upload-wrap{
    display: flex;
    align-items: center;
  }
  /deep/ .el-collapse-item{
    .img-list{
      margin: 20px 10px 0 0;
      cursor: pointer;
    }
  }
}

.input-group{
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;

  .list{
    width: 30%;
    margin-top: PXtoEm(20);

    .inp-wrap{
      display: flex;
      align-items: center;
    }

    .text{
      width: 28%;
    }

    &:nth-of-type(3n+1){
      justify-content: flex-start;
    }

    &:nth-of-type(3n){
      justify-content: flex-end;
    }
  }

  .list-block{
    width: 100%;
    .span-block{
      width: 8.5%;
    }
    .whole-line{
        width: 86.5%;
      }
  }
}
.cause{
  display: flex;
  margin-top: 20px;
  align-items: center;

  .el-textarea{
    flex: 1;
    width: auto;
  }
}

// 列表
.list-bottom-wrap{
  margin-top: 10px;

  .list{
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    // @extend .con-padding-horizontal;
    background: rgba(186,186,186,.1);
    text-align: center;

    &:nth-of-type(odd){
      background: rgba(128,128,128,.1);
    }

    &.list-header{
      height: 50px;
      background: $color-content-table-header;
    }

    &:not(.list-header){
      padding: {
        top: 20px;
        bottom: 20px;
      }
    }
  }

  .list-con{
    display: flex;
    align-items: center;
    justify-content: center;
    width: 20%;

    &:first-of-type{
      justify-content: flex-start;
    }
  }

  .textarea-wrap{
    width: 100%;
    margin-top: 10px;
  }
}

// 提交底部按钮
.btn-group{
  padding: 20px 0;
  text-align: center;
}

.lable{
  width: 100px;
}

.disabled{
  background: #8c939d;
}

.upload-list{
  margin-top: PXtoEm(25);
  margin-bottom: PXtoEm(25);
  width: -webkit-fill-available;
}
.left{
  text-indent: 9.5%
}
</style>
