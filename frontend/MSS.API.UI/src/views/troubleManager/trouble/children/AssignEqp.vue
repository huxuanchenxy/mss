<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <i @click="back"><x-button class="active">返回</x-button></i>
    </div>
    <!-- 内容 -->
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list last-update-time">设备名称</li>
        <li class="list last-update-time">分配给部门/班组</li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item, index) in troubleEqpList" :key="item.key">
              <div class="list-content">
                <div class="last-update-time color-white">{{ item.eqpName }}</div>
                <div class="last-update-time color-white">
                  <el-cascader class="cascader_width" :ref="'assignedTo' + index"
                    change-on-select
                    :props="defaultParams"
                    :show-all-levels="true"
                    :options="orgList"
                    v-model="item.orgPath">
                  </el-cascader>
                </div>
              </div>
            </li>
          </ul>
          <!-- 按钮 -->
          <div class="btn-group">
            <x-button class="close"  @click.native="back">取消</x-button>
            <x-button class="active" @click.native="save">保存</x-button>
          </div>
        </el-scrollbar>
      </div>
    </div>
  </div>
</template>
<script>
import { strToIntArr, troubleMenu } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
import apiOrg from '@/api/orgApi'
export default {
  name: 'AssignEqp',
  components: {
    XButton
  },
  data () {
    return {
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      title: ' | 调度分配',
      sourceName: '',
      troubleID: '',
      repairCompany: -2,
      troubleView: 0,
      troubleEqpList: [],
      orgList: [],
      loading: false
    }
  },
  created () {
    this.sourceName = this.$route.params.sourceName
    if (this.sourceName === 'MyRepair') {
      this.troubleView = troubleMenu.myRepair
    } else if (this.sourceName === 'MyProcessing') {
      this.troubleView = troubleMenu.myProcessing
    }
    this.troubleID = this.$route.params.id
    this.title = ' | 调度分配-' + this.$route.params.code
    this.repairCompany = this.$route.params.repairCompany
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    if (user.is_super && this.repairCompany === 0) {
      this.repairCompany = 8
    }
    // 班组加载
    apiOrg.getOrgAll().then(res => {
      let obj = res.data.find(item => {
        return item.id === this.repairCompany
      })
      this.orgList = obj.children
      this.getEqp()
    }).catch(err => console.log(err))
  },
  methods: {
    save () {
      let arr = []
      for (let i = 0; i < this.troubleEqpList.length; i++) {
        let len = this.$refs['assignedTo' + i][0].currentLabels.length
        let val = this.troubleEqpList[i]
        let tmp = {}
        let orgNodeName = this.$refs['assignedTo' + i][0].currentLabels[len - 1]
        if (orgNodeName === undefined) {
          this.$message({
            message: '分配给部门/班组必选',
            type: 'warning'
          })
          return
        } else {
          tmp = {
            id: val.id,
            trouble: val.trouble,
            eqpName: val.eqpName,
            org: val.org,
            eqp: val.eqp,
            orgNode: val.orgPath[val.orgPath.length - 1],
            orgNodeName: orgNodeName,
            orgPath: val.orgPath.join(',')
          }
          arr.push(tmp)
        }
      }
      api.assignEqp(JSON.stringify(arr)).then(res => {
        if (res.code === 0) {
          this.$router.push({name: 'MyRepair'})
          this.$message({
            message: '分配成功',
            type: 'success'
          })
        } else {
          this.$message({
            message: res.msg === '' ? '分配失败' : res.msg,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    back () {
      this.$router.push({
        name: this.sourceName
      })
    },
    // 搜索
    getEqp () {
      api.getTroubleEqpByID(this.troubleID, this.repairCompany, this.troubleView).then(res => {
        this.loading = false
        this.troubleEqpList = res.data
        this.troubleEqpList.map(item => {
          if (item.orgPath === null) {
            item.orgPath = strToIntArr(item.orgPathTmp)
            item.orgPath.splice(0, 1)
          } else {
            item.orgPath = strToIntArr(item.orgPath)
          }
        })
        // console.log(this.troubleEqpList)
      }).catch(err => console.log(err))
    }
  }
}
</script>
<style lang="scss" scoped>
$con-height: $content-height - 145 - 56;
// 内容区
.content-wrap{
  overflow: hidden;
  height: percent($con-height, $content-height);
  text-align: center;
  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: percent(50, $con-height);
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);

    .last-update-time{
      color: $color-white;
    }
  }

  .scroll{
    height: percent($con-height - 50, $con-height);
  }

  .list-wrap{
    .list{
      &:nth-of-type(even){
        .list-content{
          background: rgba(186,186,186,.5);
        }
      }
    }

    .list-content{
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: PXtoEm(15) PXtoEm(24);

      div{
        word-break: break-all;
      }
    }

    .left-title{
      margin-right: 10px;
      font-weight: bold;
    }

    // 隐藏内容
    .sub-content{
      overflow: hidden;
      height: 0;
      font-size: $font-size-small;
      text-align: left;
      color: $color-content-text;

      &.active{
        overflow: inherit;
        height: auto;
        transition: .7s .2s;
      }
    }

    .sub-con-list{
      display: flex;
      padding: PXtoEm(15) PXtoEm(24);
      border-top: 1px solid $color-main-background;
      background: rgba(0,0,0,.2);

      .right-wrap{
        display: flex;
        flex-wrap: wrap;
      }

      .list{
        margin-right: 10px;
      }
    }
  }

  .number{
    width: 6%;
  }
  .name,
  .btn-wrap{
    width: 10%;
  }

  .name{
    a{
      color: #42abfd;
    }
  }

  .last-update-time{
    width: 50%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 25%;
  }

  .state{
    width: 5%;
  }
}
/deep/
.btn1{
  background: none;
  border: 1px solid $color-main-btn-border;
  border-radius: $border-radius;
  color: $color-white;
  cursor: pointer;
  &.active,
  &:hover{
    border-color: $color-main-btn!important;
    background: $color-main-btn!important;
  }
}
.cascader_width{
  width: 300px;
}
</style>
<style>
.my-pop{
  background: #29282E;
  border-color: #29282E;
}
.update-content{
  word-break: break-all;
}
</style>
