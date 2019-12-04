<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'MyProcessing' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">处理人<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="dealBy.text" clearable filterable placeholder="请选择责任人" >
                  <el-option
                    v-for="item in dealByList"
                    :key="item.key"
                    :label="item.userName"
                    :value="item.id">
                  </el-option>
                </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ dealBy.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">到达现场时间<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-date-picker
                  v-model="arrivedTime.text"
                  type="datetime"
                  value-format="yyyy-MM-dd HH:mm:ss"
                  :clearable="false"
                  class="datetime-width"
                  placeholder="请选择发生时间">
                </el-date-picker>
              </div>
            </div>
            <p class="validate-tips">{{ arrivedTime.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">处理完成时间<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-date-picker
                  v-model="finishedTime.text"
                  type="datetime"
                  value-format="yyyy-MM-dd HH:mm:ss"
                  :clearable="false"
                  class="datetime-width"
                  placeholder="请选择报修时间">
                </el-date-picker>
              </div>
            </div>
            <p class="validate-tips">{{ finishedTime.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">报修情况评价</span>
              <div class="inp">
                <el-input placeholder="请输入报修情况评价" v-model="repairDesc.text" @keyup.native="validateInputNull(repairDesc)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ repairDesc.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">引起原因<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input placeholder="请输入引起原因" v-model="reason.text" @keyup.native="validateInput(reason)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ reason.tips }}</p>
          </li>
          <li class="list"/>
        </ul>
        <div class="con-padding-horizontal cause">
            <span class="lable">处理经过<em class="validate-mark">*(500字以内)</em></span>
            <el-input type="textarea" :rows="4" v-model="process.text" placeholder="请输入报修故障描述" @keyup.native="validateInputRange(process)"></el-input>
        </div>
        <div class="validate-tips left">{{ process.tips }}</div>
        <div class="con-padding-horizontal cause">
            <span class="lable">备件更换情况</span>
            <el-input type="textarea" v-model="sparePartsReplace.text" placeholder="请输入报修故障描述" @keyup.native="validateInputNull(sparePartsReplace)"></el-input>
        </div>
        <div class="validate-tips left">{{ sparePartsReplace.tips }}</div>
        <!-- 按钮 -->
        <div class="btn-group">
          <x-button class="close">
            <router-link :to="{name: 'MyProcessing'}">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="save">保存</x-button>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { transformDate, nullToEmpty, validateInputCommon, vInput, getNowFormatDateTime } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/DeviceMaintainRegApi.js'
import apiOrg from '@/api/orgApi'
export default {
  name: 'DealTrouble',
  components: {
    XButton
  },
  data () {
    return {
      sourceName: '',
      troubleID: '',
      code: '',
      // 新增默认0，修改则为修改ID
      editID: 0,
      // 超级管理员实际不属于任何公司，为了调试方便，修改默认值
      topNode: 1,
      loading: false,
      title: '',
      arrivedTime: {text: '', tips: ''},
      finishedTime: {text: '', tips: ''},
      dealBy: {text: '', tips: ''},
      dealByList: [],
      process: {text: '', tips: ''},
      repairDesc: {text: '', tips: ''},
      reason: {text: '', tips: ''},
      sparePartsReplace: {text: '', tips: ''}
    }
  },
  created () {
    this.code = this.$route.params.code
    this.title = '| 故障处理结果-' + this.code
    this.troubleID = this.$route.params.troubleID
    this.sourceName = this.$route.params.sourceName
    let user = JSON.parse(window.sessionStorage.getItem('UserInfo'))
    apiOrg.getOrgTopNodeByUser(user.id).then(res => {
      if (res.data !== null) {
        this.topNode = res.data.id
      }
      api.getDealByID(this.troubleID, this.topNode).then(res => {
        if (res.data.length !== 0) {
          let _res = res.data[0]
          this.dealBy.text = _res.dealBy
          this.arrivedTime.text = transformDate(_res.arrivedTime)
          this.finishedTime.text = transformDate(_res.finishedTime)
          this.repairDesc.text = nullToEmpty(_res.repairEvaluation)
          this.reason.text = _res.repairReason
          this.process.text = _res.process
          this.sparePartsReplace.text = _res.sparePartsReplace
          this.editID = _res.id
        }
      }).catch(err => console.log(err))
      apiOrg.listUserAllByTopNode().then(res => {
        let users = res.data.find(val => {
          return val.id === this.topNode
        })
        this.dealByList = users.users
      }).catch(err => console.log(err))
    }).catch(err => console.log(err))
    this.init()
  },
  methods: {
    init () {
      this.arrivedTime.text = getNowFormatDateTime()
      this.finishedTime.text = this.arrivedTime.text
    },
    // 验证非法字符串，但允许为空
    validateInputNull (val) {
      if (!vInput(val.text)) {
        val.tips = '此项含有非法字符'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateInputRange () {
      if (this.process.text.length > 500) {
        this.process.text = '处理经过必须在500字以内'
        return false
      }
      return validateInputCommon(this.process)
    },
    validateInput (val) {
      return validateInputCommon(val)
    },
    validateInputAll () {
      if (this.arrivedTime.text === null) { this.msg('到达现场时间必选'); return false } else
      if (this.finishedTime.text === null) { this.msg('处理完成时间必选'); return false } else
      if (!this.validateInputNull(this.sparePartsReplace)) { this.msg('备件更换情况验证失败,请查看提示信息'); return false } else
      if (!this.validateInputNull(this.repairDesc)) { this.msg('报修情况评价验证失败,请查看提示信息'); return false } else
      if (!this.validateInput(this.reason)) { this.msg('报修情况评价验证失败,请查看提示信息'); return false } else
      if (this.dealBy.text === '') { this.msg('处理人必选'); return false } else
      if (!this.validateInputRange()) { this.msg('处理经过验证失败,请查看提示信息'); return false }
      // else if (this.repairCompany.text.length === 0) this.msg('接修单位必选')
      // else if (this.arrivedTime.text) this.msg('故障设备必填')
      return true
    },
    msg (str) {
      this.$message({
        message: str,
        type: 'error'
      })
    },
    save () {
      if (!this.validateInputAll()) return
      if ((new Date(this.finishedTime.text).getTime() - new Date(this.arrivedTime.text).getTime()) > 0) {
        this.$message({
          message: '处理完成时间必定大于到达现场时间',
          type: 'warning'
        })
        return
      }

      let dealTrouble = {
        ArrivedTime: this.arrivedTime.text,
        FinishedTime: this.finishedTime.text,
        OrgTop: this.topNode,
        DealBy: this.dealBy.text,
        Trouble: this.troubleID,
        RepairReason: this.reason.text,
        RepairEvaluation: this.repairDesc.text,
        Process: this.process.text,
        SparepartsReplace: this.sparePartsReplace.text,
        Code: this.code
      }
      if (this.editID > 0) dealTrouble.id = this.editID
      api.saveTroubleDeal(dealTrouble).then(res => {
        if (res.code === 0) {
          this.$router.push({name: 'MyProcessing'})
          this.$message({
            message: '处理成功',
            type: 'success'
          })
        } else {
          this.$message({
            message: res.msg === '' ? '处理失败' : res.msg,
            type: 'error'
          })
        }
      }).catch(err => console.log(err))
    }
  }
}
</script>
<style lang="scss" scoped>
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
  width: 50%;
}
.left{
  text-indent: 9.5%
}
.datetime-width{
  width: 93%!important;
}
/deep/
.el-tag--small{
  display: none;
}
// 卡片
.item {
  margin-bottom: 10px;
}

.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}
// .clearfix:after {
//   clear: both
// }

.box-card {
  width: 200px;
  background-color: #29282E;
  color: $color-content-text
}
</style>
