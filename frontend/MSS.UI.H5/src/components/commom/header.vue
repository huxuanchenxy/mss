<template>
  <div class="header1hello">
    <mu-appbar class="title" title="上海18号线智能运维系统">
      <!--// <mu-icon-button slot="left">
            //     <img class="logo" :src="src"
            // </mu-icon-button>-->
      <mu-icon-button icon="menu" slot="left" @click="toggle()" />
      <mu-icon-button
        icon="add"
        slot="right"
        @click="toggle1()"
        class="iconright"
      />
      <!-- <mu-icon-menu slot="right" icon="more_vert" :value="theme" @change="changeTheme">
                <!-- <mu-menu-item title="LIGHT" value="light" />
                <mu-menu-item title="CARBON" value="carbon" />
                <mu-menu-item title="TEAL" value="teal" />
                <mu-menu-item title="CARBON" value="logout" />
            </mu-icon-menu> -->
    </mu-appbar>
    <mu-drawer :open="open" :docked="docked" @close="toggle()" class="navleft">
      <mu-list @itemClick="docked ? '' : toggle()">
        <mu-list-item title="技术资料" @click="JumpTo(1)"
          ><mu-icon value="business_center" class="navlefticon"></mu-icon
        ></mu-list-item>
        <mu-list-item title="我的工单" @click="JumpTo(2)"
          ><mu-icon value="mail" class="navlefticon"></mu-icon
        ></mu-list-item>
        <mu-list-item title="我的故障报告" @click="JumpTo(3)"
          ><mu-icon value="report" class="navlefticon"></mu-icon
        ></mu-list-item>
        <mu-list-item title="修改密码" @click="JumpTo(4)"
          ><mu-icon value="person" class="navlefticon"></mu-icon
        ></mu-list-item>
        <mu-list-item @click.native="open = false" title="收回"
          ><mu-icon value="chevron_left" class="navlefticon"></mu-icon
        ></mu-list-item>
      </mu-list>
    </mu-drawer>
    <mu-drawer
      :open="open1"
      :docked="docked1"
      @close="toggle1()"
      class="navright"
      :right="right1"
    >
      <mu-list @itemClick="docked1 ? '' : toggle1()">
        <mu-list-item title="添加"
          ><mu-icon value="add_circle_outline" class="navrighticon"></mu-icon
        ></mu-list-item>
        <mu-list-item v-if="docked1" @click.native="open1 = false" title="收回"
          ><mu-icon value="chevron_right" class="navrighticon"></mu-icon
        ></mu-list-item>
      </mu-list>
    </mu-drawer>
  </div>
</template>
<script>
import axios from "axios";
import light from "!raw-loader!muse-ui/dist/theme-default.css";
import carbon from "!raw-loader!muse-ui/dist/theme-carbon.css";
import teal from "!raw-loader!muse-ui/dist/theme-teal.css";
import Bus from "../commom/Bus";
export default {
  name: "hello",
  data() {
    return {
      theme: "carbon",
      src: "http://www.vue-js.com/public/images/vue.png",
      themes: {
        carbon
        // light,
        // teal
      },
      open: false,
      docked: false,
      open1: false,
      docked1: true,
      right1: true,
      addattr: "",
      position: "left",
      hoverShow: false
    };
  },
  created() {
    this.changeTheme("carbon");
    Bus.$on("addattr", val => {
      console.log("headercraed:" + val);
      this.addattr = val;
    });
  },
  methods: {
    closeHover() {
      this.hoverShow = false;
    },
    changeTheme(theme) {
      this.theme = theme;
      // console.log(this.theme)
      const styleEl = this.getThemeStyle();
      styleEl.innerHTML = this.themes[theme] || "";
    },
    getThemeStyle() {
      const themeId = "muse-theme";
      let styleEl = document.getElementById(themeId);
      // console.log(styleEl)
      if (styleEl) return styleEl;
      styleEl = document.createElement("style");
      styleEl.id = themeId;
      document.body.appendChild(styleEl);
      // console.log(styleEl)
      return styleEl;
    },
    toggle(flag) {
      this.open = !this.open;
      this.docked = false;
      // this.open1 = false
      // this.docked1 = false
      // console.log(event)
    },
    toggle1(flag1) {
      // this.open1 = !this.open1
      // // this.docked1 = !flag1
      // this.open = false
      // this.docked = false
      Bus.$on("addattr", val => {
        console.log("val:" + val);
        this.addattr = val;
      });
      console.log("addattr:" + this.addattr);

      this.$router.push({
        name: "trouble"
      });
    },
    JumpTo(v) {
      // console.log('v:' +  v)
      if (v === 4) {
        this.$router.push({
          name: "Password"
        });
      }
      if (v === 1) {
        this.$router.push({
          name: "home"
        });
      }
    },
    Logout() {
      window.sessionStorage.removeItem("token");
      this.$router.push({
        name: "Login"
      });
    }
  }
};
</script>
<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
.title {
  text-align: center;
  height: 58px;
}
.mu-icon-button {
  padding: 0.4rem;
}
.navlefticon {
  position: absolute;
  top: 10px;
  left: 5px;
}
.navrighticon {
  position: absolute;
  top: 11px;
  left: 2px;
}
.navleftcontent {
  left: 18px;
}
.navleft {
  width: 38%;
}
.navright {
  width: 18%;
}
.header1hello .left {
  position: absolute;
  left: 0;
}
.header1hello .iconright {
  position: absolute;
  right: 0;
}
</style>
<style>
.navleft .mu-item-title-row {
  left: 18px;
}
.navright .mu-item-title-row {
  left: 12px;
}
.mu-overlay {
  position: fixed;
  left: unset !important;
  right: 0;
  top: 0;
  bottom: 0;
  background-color: #000;
  opacity: 0.4;
  z-index: 6;
  width: 62%;
}

.header1hello .mu-appbar-title {
  position: absolute;
  width: 250px;
  height: 50px;
  margin: 0 auto;
  left: 0;
  right: 0;
  top: 0;
  bottom: 0;
}
</style>
