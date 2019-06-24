<template>
  <div class="wrap">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img class="icon" src="../common/images/icon-home.svg" alt=""> 首页
      </h2>
      <div class="inp-wrap">
        <el-select v-model="tunnel" value-key="TunnelID" filterable placeholder="请选择">
          <el-option
            v-for="item in tunnelList"
            :key="item.key"
            :label="item.TunnelName"
            :value="item">
          </el-option>
        </el-select>
      </div>
    </div>
    <div class="con-padding-horizontal content">
      <div class="left">
        <div class="title">三维漫游图像</div>
        <div class="picture-wrap">
          <three-d></three-d>
        </div>
      </div>
      <div class="right">
        <div class="charts-wrap">
          <charts-task-complete :tunelID="{id: 1, name: '管廊有分区修改'}" type='1'></charts-task-complete>
        </div>
        <div class="charts-wrap">
          <charts-ht-rt :tunelID="{TunnelCode: '01', TunnelName: '管廊有分区修改'}" isFirst='1' ></charts-ht-rt>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: 'Index',
  data () {
    return {
      name: '',
      cycle: {id: 1, name: '管廊有分区修改'},
      tunnelList: [],
      tunnel: ''
    }
  },
  created () {
    // 获取管廊
    window.axios.post('/UtilityTunnel/GetTunnelByUserID').then(res => {
      this.tunnelList = res.data
    }).catch(err => console.log(err))
  }
}
</script>
<style lang="scss" scoped>
.header{
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: relative;
  height: percent(56, $content-height);

  &:after{
    content: "";
    position: absolute;
    bottom: -7px;
    left: 0;
    width: 100%;
    height: 11px;
    background: url(../common/images/line.png) no-repeat 0 0/100% 100%;
  }

  .title{
    font-size: PXtoEm(18);
  }

  .icon{
    vertical-align: middle;
  }
}

.content{
  display: flex;
  justify-content: space-between;
  height: percent($content-height - 100, $content-height);
  padding: {
    top: 20px;
    bottom: 20px;
  }

  .left{
    width: percent(565, $content-width - 48);
    height: 100%;
    background: #28272E;
    border-radius: $border-radius;

    .title{
      display: flex;
      align-items: center;
      height: percent(40, 565);
      padding-left: 15px;
    }

    .picture-wrap{
      height: percent(565 - 40, 565);
      border-radius: 0 0 $border-radius $border-radius;
    }
  }

  .right{
    display: flex;
    flex-wrap: wrap;
    width: percent(325, $content-width - 48);

    .charts-wrap{
      box-sizing: border-box;
      width: 100%;
      height: percent(215, $content-height - 100);
      padding: 10px;
      background: #28272E;
      border-radius: $border-radius;

      &:last-of-type{
        align-self: flex-end;
      }
    }
  }
}
</style>
