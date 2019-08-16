<template>
  <div class="header">
    <ul class="list-wrap">
      <li class="list">
        <img src="./images/icon-company.svg" class="vertical-middle icon" width="35" height="40" alt="">
        <p class="inline-block vertical-middle">
          <span class="block title">{{comName}}</span>
          <span class="sub-title">当前共运营<span class="mark-text"> {{ number }} </span>条地铁</span>
        </p>
        <p class="inline-block vertical-middle date">
          <span class="block text-left">{{ week }}</span>
          <span class="block sub-title">{{ date }}</span>
        </p>
      </li>
      <li class="list">
        <h2 class="title"><span class="text">上海轨道交通18号线智能运维系统</span></h2>
      </li>
      <li class="list clearfix">
        <div class="fl">
          <img src="./images/icon-home.svg" class="vertical-middle icon" width="35" height="40" alt="">
          <p class="inline-block vertical-middle">
            <span class="block title">系统首页</span>
            <router-link class="sub-title" :to="{ name: 'Index' }">查看 &gt;</router-link>
          </p>
        </div>
        <div class="fl">
          <img src="./images/icon-user.svg" class="vertical-middle icon" width="35" height="40" alt="">
          <p class="inline-block vertical-middle">
            <span class="block title">{{userName}}</span>
            <router-link class="sub-title" :to="{ name: 'Password' }">修改密码</router-link> |
            <router-link class="sub-title" :to="{ name: 'Login' }" @click="window.sessionStorage.removeItem('UserID')">退出</router-link>
          </p>
        </div>
      </li>
    </ul>
  </div>
</template>
<script>
import { getDate, ApiRESULT } from '@/common/js/utils.js'
import api from '@/api/loginApi'
export default {
  name: 'myHeader',
  data () {
    return {
      week: getDate(),
      date: `${getDate('date').year}-${getDate('date').month}-${getDate('date').date}`,
      number: 1,
      comName: '',
      userName: ''
    }
  },
  methods: {
    getUserInfo () {
      api.getUserInfo().then((res) => {
        if (res.code === ApiRESULT.Success) {
          this.userName = res.data.user_name
          window.sessionStorage.setItem('UserInfo', JSON.stringify(res.data))
        }
      })
    },
    getUserAction () {
      api.getUserAction().then((res) => {
        if (res.code === ApiRESULT.Success) {
          window.sessionStorage.setItem('UserAction', JSON.stringify(res.data))
        }
      })
    }
  },
  created () {
    this.getUserInfo()
    this.getUserAction()
  }
}
</script>
<style lang="scss" scoped>
.list-wrap{
  $width: 1440;
  display: flex;
  justify-content: space-between;
  width: 100%;
  height: 75px;
  margin: 0 auto;

  .sub-title{
    font-size: $font-size-small;
    color: $color-sub-text;

    &:hover{
      color: $color-highlight-text;
    }
  }

  // 标记文字
  .mark-text{
    color: #19D671;
  }

  .list{
    display: inline-block;

    &:first-of-type{
      width: percent(426, $width);
      height: 55px;
      margin-top: 20px;
      padding-left: percent(70, $width);

      .title{
        display: block;
        width: 140px;
        margin-right: 30px;
        @extend %ellipsis;
      }

      .date{
        position: relative;
        padding-left: 30px;

        &:after{
          content: "";
          position: absolute;
          top: 50%;
          left: 0;
          width: 1px;
          height: 30px;
          background: #57565B;
          transform: translateY(-50%);
        }
      }

      .sub-title{
        color: $color-sub-text;
      }
    }

    // 中间标题
    &:nth-of-type(2){
      box-sizing: border-box;
      width: 444px;
      height: 120px;
      background: url(./images/title.png) no-repeat 0 -17px/100% 100%;
      text-align: center;

      .title{
        position: relative;
        height: 100%;
        font-size: PXtoEm(24);
        font-weight: bold;
        text-align: center;
        text-shadow: 0 0 15px $color-highlight-text;

        // 左右两个装饰背景
        &:before,
        &:after{
          content: "";
          position: absolute;
          top: 35px;
          z-index: -1;
          width: 100%;
          height: 25px;
        }

        &:before{
          left: -100%;
          background: url(./images/line-left.png) no-repeat 0 0/100% 100%;
        }

        &:after{
          right: -100%;
          background: url(./images/line-right.png) no-repeat 0 0/100% 100%;
        }
      }

      .text{
        display: block;
        padding-top: 20px;
      }
    }

    // 用户信息
    &:last-of-type{
      display: flex;
      justify-content: flex-end;
      width: percent(426, $width);
      height: 55px;
      margin-top: 20px;
      padding-right: percent(70, $width);

      div{
        &:last-of-type{
          margin-left: 50px;
        }
      }
    }
  }
}
@media screen and (min-width: 1600px){
  .list-wrap{
    .icon{
      margin-right: 15px;
    }

    .list{
      &:last-of-type{
        div{
          &:last-of-type{
            margin-left: 80px;
          }
        }
      }
    }
  }
}
</style>
