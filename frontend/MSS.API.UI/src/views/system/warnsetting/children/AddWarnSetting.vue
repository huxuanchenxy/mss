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
        <router-link :to="{name:'WarnSettingList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list" v-show="false">
          <div class="inp-wrap">
            <span class="text">预警设置ID</span>
            <div class="inp">
              <el-input v-model="ID" :disabled="isShow === 'edit'" ></el-input>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">设备类型</span>
            <div class="inp">
              <el-select v-model="equipmentTypeID" :disabled="isShow === 'edit'" @change="chooseEqpType" placeholder="请选择">
                <el-option
                  v-for="item in eqpTypeList"
                  :key="item.key"
                  :label="item.tName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">设备参数</span>
            <div class="inp">
              <el-select v-model="paramObj" :disabled="isShow === 'edit'" value-key="_paramID" placeholder="请选择">
                <el-option
                  v-for="item in paramList"
                  :key="item.key"
                  :label="item._paramName"
                  :value="item">
                </el-option>
              </el-select>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">是否启用</span>
            <div class="inp">
              <el-checkbox v-model="isActived"></el-checkbox>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">预阀值上限<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="预阀值上线" v-model.trim="limitUp.text" @keyup.native="validateInputDouble3(limitUp)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ limitUp.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">预阀值下限<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="预阀值下线" v-model.trim="limitDown.text" @keyup.native="validateInputDouble3(limitDown)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ limitDown.tips }}</p>
        </li>
        <li class="list"/>
      </ul>
      <ul class="input-group-ex">
        <li class="list" >
          <x-button class="active" @click.native="addSettingEx">添加环境条件</x-button>
        </li>
      </ul>
      <ul class="input-group-ex">
        <li class="list" v-for="(item, index) in settingEx"
                        :key="item.key">
          <div class="inp-wrap">
            <el-row>
              <el-col :span="2">条件{{index+1}}</el-col>
              <el-col :span="5">
                <el-select v-model="item.paramID" placeholder="环境条件">
                  <el-option
                    v-for="item in settingExTypeList"
                    :key="item.key"
                    :label="item.pidName"
                    :value="item.id">
                  </el-option>
                </el-select>
              </el-col>
              <el-col :span="5">
                <el-select v-model="item.paramLimitType" style="margin-left:10px" placeholder="运算符">
                  <el-option
                    v-for="item in limitTypeList"
                    :key="item.key"
                    :label="item.name"
                    :value="item.id">
                  </el-option>
                </el-select>
              </el-col>
              <el-col :span="3">
                <el-input placeholder="值" v-model="item.paramLimitValue"
                  style="margin-left:10px" @keyup.native="validateInputDouble3(limitDown)"></el-input>
              </el-col>
              <el-col :span="5">
                <x-button style="margin-left:30px" class="active" @click.native="DelThisSetting(index)">删除</x-button>
                <p class="validate-tips">{{ item.tips }}</p>
              </el-col>
            </el-row>
          </div>
        </li>
      </ul>
    </div>
    <div class="btn-enter">
      <x-button class="close">
        <router-link :to="{ name: 'WarnSettingList' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="save">保存</x-button>
    </div>
  </div>
</template>
<script>
// test
import { vdouble3, ApiRESULT } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/warnSettingApi'
export default {
  name: 'AddUser',
  components: {
    XButton
  },
  data () {
    return {
      title: '',
      loading: false,
      isShow: 'Add',
      settingExTypeList: [],
      limitTypeList: [{
        id: 1,
        name: '>'
      }, {
        id: 2,
        name: '='
      }, {
        id: 3,
        name: '<'
      }],
      paramList: [],
      equipmentTypeID: '',
      paramObj: {},
      ID: null,
      isActived: true,
      limitUp: {
        text: '',
        tips: ''
      },
      limitDown: {
        text: '',
        tips: ''
      },
      settingEx: []
    }
  },
  created () {
    if (this.isShow === 'add') {
      this.title = '| 添加预警设定'
    } else if (this.isShow === 'edit') {
      this.title = '| 修改预警设定'
      this.getWarnningSetting()
    }
    this.getSettingExType()
    this.getAllEqpType()
  },
  activated () {
    this.isShow = this.$route.params.mark
    if (this.isShow === 'edit') {
      this.title = '| 修改预警设定'
      this.ID = this.$route.params.id
      this.getWarnningSetting()
    } else {
      this.title = '| 添加预警设定'
    }
  },
  methods: {
    getSettingExType () {
      api.getSettingExType().then(res => {
        this.settingExTypeList = res.data
      }).catch(err => console.log(err))
    },
    getAllEqpType () {
      api.getAllEqpType().then(res => {
        this.eqpTypeList = res.data
        this.equipmentTypeID = res.data[0].id
        this.chooseEqpType()
      }).catch(err => console.log(err))
    },
    chooseEqpType () {
      let id = this.equipmentTypeID
      api.getParamByEqpType(id).then(res => {
        this.paramList = res.data
        this.paramObj = res.data[0]
      }).catch(err => console.log(err))
    },
    validateInputDouble3 (val) {
      if (val.text === '' || !vdouble3(val.text)) {
        val.tips = '此处比需填数字且最多三位小数'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    addSettingEx () {
      this.settingEx.push({
        paramID: '',
        paramLimitType: '',
        paramLimitValue: null
      })
    },
    DelThisSetting (idx) {
      this.settingEx.splice(idx, 1)
    },
    validateAll () {
      if (!this.validateInputDouble3(this.limitUp)) return false
      if (!this.validateInputDouble3(this.limitDown)) return false
      if (parseFloat(this.limitDown.text) > parseFloat(this.limitUp.text)) {
        this.$message.error('下限值必须小于上限值')
        return false
      }
      // if (!this.validateSelect()) return false
      // 验证扩展条件
      for (let i = 0; i < this.settingEx.length; ++i) {
        let item = this.settingEx[i]
        if (item.paramID === '' || item.paramLimitType === '' ||
            !vdouble3(item.paramLimitValue)) {
          this.$message({
            message: '条件' + (i + 1) + '输入不合格',
            type: 'error'
          })
          return false
        }
      }
      return true
    },
    save () {
      if (this.validateAll()) {
        var param = {
          EquipmentTypeID: this.equipmentTypeID,
          ParamID: this.paramObj._paramID,
          ParamName: this.paramObj._paramName,
          ParamUnit: this.paramObj._paramUnit,
          ParamLimitUpper: this.limitUp.text,
          ParamLimitLower: this.limitDown.text,
          IsActived: this.isActived,
          settingEx: this.settingEx
        }
        if (this.isShow === 'add') {
          api.saveWarnningSetting(param).then((res) => {
            if (res.code === ApiRESULT.Success) {
              this.$message.success('设定成功')
              this.$router.push({
                name: 'WarnSettingList'
              })
            } else if (res.code === ApiRESULT.DataIsExist) {
              this.$message.error('参数已设定重复')
            } else {
              this.$message.error('设定失败')
            }
          })
        } else if (this.isShow === 'edit') {
          param.ID = this.ID
          api.updateWarnningSetting(this.ID, param).then((res) => {
            if (res.code === ApiRESULT.Success) {
              this.$message.success('修改成功')
              this.$router.push({
                name: 'WarnSettingList'
              })
            } else {
              this.$message.error('修改失败')
            }
          })
        }
      }
    },

    // 修改用户时获取用户资料
    getWarnningSetting () {
      this.loading = true
      api.getWarnSettingByID(this.ID).then(res => {
        this.loading = false
        if (res.code === ApiRESULT.Success) {
          this.equipmentTypeID = res.data.equipmentTypeID
          this.paramObj = {_paramID: res.data.paramID}
          this.isActived = res.data.isActived
          this.limitUp.text = res.data.paramLimitUpper
          this.limitDown.text = res.data.paramLimitLower
          this.settingEx = res.data.settingEx
        }
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
  .input-group-ex{
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list{
      width: 100%;
      margin-top: PXtoEm(25);

      span{
        width: PXtoEm(100);
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
</style>
