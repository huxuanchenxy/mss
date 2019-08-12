<template>
  <div class="footer">
    <div class="footer-nav-wrap">
      <div class="box">
        <router-link v-for="(item, key, index) in navList" :key="item.key" :to="{ path: item.path }" @click.native="moveNav(index)">
          <el-tooltip class="item" effect="dark" :content="item.name" placement="top">
            <img :src="item.iconCls" width="24" height="24" alt="">
          </el-tooltip>
        </router-link>
      </div>
    </div>
    <div class="copyright">{{ copyright }}</div>
  </div>
</template>
<script>
import Bus from '@/components/Bus'
export default {
  name: 'myFooter',
  data () {
    return {
      copyright: '©上海电气自动化设计研究所有限公司所有',
      navList: []
    }
  },
  methods: {
    // 设置导航显示对应位置
    moveNav (i) {
      document.querySelector('.nav-wrap .el-scrollbar__wrap').scrollTop = document.querySelector('#nav-move-wrap .list').offsetHeight * i
    }
  },
  mounted () {
    Bus.$on('navList', data => (this.navList = data))
  }
}
</script>
<style lang="scss" scoped>
.footer{
  position: fixed;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 20px;
}

.footer-nav-wrap{
  display: none;
  flex-wrap: wrap;
  position: absolute;
  bottom: 35px;
  left: 50%;
  min-width: 300px;
  height: 45px;
  transform: translateX(-50%);

  .box{
    display: flex;
    justify-content: center;
    margin-top: -10px;
    padding: 0 40px;
  }

  a{
    display: flex;
    justify-content: center;
    align-items: center;
    position: relative;
    width: 60px;
    height: 60px;
    background: url(./images/nav-bg.svg) no-repeat center;
    transition: .5s;

    &:hover{
      transform: scale(1.3);
    }

    &:last-of-type{
      margin-right: 0;
    }

    &.router-link-active{
      background: url(./images/nav-bg-1.svg) no-repeat;
    }
  }

  &:after{
    content: "";
    position: absolute;
    bottom: -14px;
    left: 0;
    z-index: -1;
    width: 100%;
    height: 45px;
    background: url(./images/bg.png) no-repeat 0 0/100% 100%;
  }
}

.copyright{
  position: absolute;
  bottom: 5px;
  left: 50%;
  font-size: $font-size-small;
  text-align: center;
  color: $color-footer-text;
  transform: translateX(-50%);
}
</style>
