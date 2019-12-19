<template>
  <div
    class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active">
        <router-link :to="{name:'MaintenanceList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation scroll">
      <el-scrollbar>
        <el-collapse v-model="activeName" accordion v-for="(tab) in mModuleList" :key="tab.key">
          <el-collapse-item :title="tab.showName" :name="tab.showName" style="margin-top:10px;padding:0px;">
            <div v-show="tab.isShowEqp">
              <span style="width:400px;display:inline-block;margin-top:10px;">检查设施唯一编号</span>
              <el-input style="width:100px;margin-left:10px;margin-top:10px;" v-model.trim="tab.eqp" placeholder="请输入" :disabled="isShow"></el-input>
            </div>
            <div v-for="(item) in tab.items" :key="item.key">
              <span style="width:400px;display:inline-block;margin-top:10px;">{{item.itemName}}</span>
              <el-input style="width:100px;margin-left:10px;margin-top:10px;" v-model.trim="item.itemValue" placeholder="请输入" :disabled="isShow" v-show="item.itemType===0"></el-input>
              <el-checkbox style="margin-left:10px;margin-top:10px;"
               :disabled="isShow"
                v-model="item.itemValue"
                true-label="1"
                false-label="0"
                v-show="item.itemType===1">
              </el-checkbox>
            </div>
          </el-collapse-item>
        </el-collapse>
        <div class="btn-enter" v-show="!isShow">
          <x-button class="close">
            <router-link :to="{ name: 'MaintenanceList' }">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="enter(false)">保存(可修改)</x-button>
          <x-button class="active" @click.native="sure()">提交(不可修改)</x-button>
        </div>
      </el-scrollbar>
    </div>
    <!-- dialog对话框 -->
    <el-dialog
      :visible.sync="dialogVisible.isShow"
      :modal-append-to-body="false"
      :show-close="false">
      {{ dialogVisible.text }}
      <template slot="footer" class="dialog-footer">
        <template v-if="dialogVisible.btn">
          <el-button @click="dialogVisible.isShow = false">否</el-button>
          <el-button @click="enter(true)">是</el-button>
        </template>
        <el-button v-else @click="dialogVisible.isShow = false" :class="{ on: !dialogVisible.btn }">知道了</el-button>
      </template>
    </el-dialog>
  </div>
</template>
<script>
// import { dictionary } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
// import apiAuth from '@/api/authApi'
import api from '@/api/workflowApi'
export default {
  name: 'UpdateMaintenanceList',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      title: '',
      editID: '',
      isInit: '',
      // 查看不可编辑
      isShow: true,
      checkResult: {
        checkTrue: '1',
        checkFalse: '0'
      },
      mModuleList: [
        {
          id: 1,
          name: '机房',
          isShowEqp: false,
          items: [
            {
              eqp: '',
              item: 1,
              itemName: '空调正常工作（制冷状态）',
              itemType: 1,
              itemValue: ''
            }, {
              eqp: '',
              item: 2,
              itemName: '门窗、顶板情况',
              itemType: 1,
              itemValue: ''
            }, {
              eqp: '',
              item: 3,
              itemName: '温度',
              itemType: 0,
              itemValue: ''
            }
          ]
        }, {
          id: 2,
          name: '机房1',
          isShowEqp: false,
          items: [
            {
              eqp: '',
              item: 4,
              itemName: '空调正常工作（制冷状态）1',
              itemType: 1,
              itemValue: ''
            }, {
              eqp: '',
              item: 5,
              itemName: '门窗、顶板情况1',
              itemType: 1,
              itemValue: ''
            }, {
              eqp: '',
              item: 6,
              itemName: '温度1',
              itemType: 0,
              itemValue: ''
            }
          ]
        }
      ],
      workType: '',
      workTypeList: [],
      pmType: '',
      pmTypeList: [],
      activeName: '',
      dialogVisible: {
        isShow: false,
        text: '',
        // true 为两个按钮，false 为一个按钮
        btn: true
      }
    }
  },
  created () {
    this.title = ' | 检修单填写 - ' + this.$route.params.name
    this.editID = this.$route.params.id
    this.isInit = this.$route.params.isInit
    this.isShow = this.$route.params.isShow
    this.listMModuleByID()
  },
  methods: {
    sure () {
      this.dialogVisible.isShow = true
      this.dialogVisible.btn = true
      this.dialogVisible.text = '提交后将不可修改，是否确认?'
    },
    enter (type) {
      let ret = []
      this.mModuleList.map(val => {
        if (val.eqp !== null) {
          val.items.map(item => {
            item.eqp = val.eqp
          })
        }
        ret = ret.concat(val.items)
      })
      api.saveMMoudleItemValue({MaintenanceModuleItemValues: ret, IsFinished: type}).then(res => {
        if (res.code === 0) {
          this.$message({
            message: '操作成功',
            type: 'success'
          })
          if (type) {
            this.$router.push({
              name: 'MaintenanceList'
            })
            this.dialogVisible.isShow = false
          }
        } else {
          this.$message({
            message: res.msg,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    },
    // 获取检修单具体项
    listMModuleByID () {
      api.listMModuleByID(this.editID, this.isInit).then(res => {
        this.loading = false
        this.mModuleList = res.data
      }).catch(err => console.log(err))
    }
  }
}
</script>
<style lang="scss" scoped>
// 功能区
.operation{
  .input-group{
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list{
      width: 30%;
      margin-top: PXtoEm(25);

      span{
        width: 28%;
      }

      .inp-wrap{
        display: flex;
        align-items: center;
      }

      &:nth-of-type(3n+1){
        // justify-content: flex-start;
      }

      &:nth-of-type(3n){
        // justify-content: flex-end;
      }
    }
  }
}
.btn-enter{
  margin: 15px 0;
  text-align: center;

  button{
    border-color: $color-main-btn;
    background: $color-main-btn;
  }
}
/deep/
.el-collapse-item__content{
  padding: 0px!important;
  margin-bottom: 10px!important;
}
$con-height: $content-height+300;
.scroll{
  height: percent($con-height - 50, $con-height);
}
/deep/
.el-checkbox__input.is-disabled.is-checked .el-checkbox__inner{
  background-color: #1b7ec9!important;
}
</style>
