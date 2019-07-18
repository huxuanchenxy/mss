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
        <router-link :to="{name:'SeeMaintainConfig'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <el-form ref="form" :rules="rules" label-position="left" :model="form"
        class="custom-form" label-width="80px">
        <el-row >
          <el-col>
            <el-form-item label="提醒方式" label-width="150px">
              <el-checkbox-group v-model="form.reminder">
                <el-checkbox label="短信" :disabled="this.ReadOnly"></el-checkbox>
                <el-checkbox label="邮件" :disabled="this.ReadOnly"></el-checkbox>
              </el-checkbox-group>
            </el-form-item>
          </el-col>
        </el-row>
        <div>
          <el-row :gutter="32">
            <el-col :span="8">
              <el-form-item label="寿命提醒提前天数" label-width="150px" prop="beforeDead">
                <el-input v-model="form.beforeDead" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="中修提醒提前天数" label-width="150px" prop="beforeMaintainMiddle">
                <el-input v-model="form.beforeMaintainMiddle" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="大修提醒提前天数" label-width="150px" prop="beforeMaintainBig">
                <el-input v-model.number="form.beforeMaintainBig" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row>
            <el-col>
              <el-form-item label="提醒内容格式模板" label-width="150px">
                <el-input type="textarea" v-model="form.textTemplate" :disabled="this.ReadOnly" :rows="7"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row >
            <el-col>
              <el-form-item label="是否启用规则" label-width="150px">
                <el-radio-group v-model="form.published">
                <el-radio :label="1" :disabled="this.ReadOnly">启用</el-radio>
                <el-radio :label="0" :disabled="this.ReadOnly">禁用</el-radio>
              </el-radio-group>
              </el-form-item>
            </el-col>
          </el-row>
        </div>
        <el-row v-if="this.$route.params.mark!=='look'">
          <el-col align="center">
            <el-form-item>
              <x-button class="close">
                <router-link :to="{ name: 'SeeMaintainConfig' }">取消</router-link>
              </x-button>
              <x-button class="active" @click.native="submitForm">保存</x-button>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vInput, vPhone, validateNumberCommon, ApiRESULT } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/eqpConfigApi'
// import eventBus from '@/components/Bus'
export default {
  name: 'addMaintainConfig',
  components: {
    XButton
  },
  data () {
    return {
      form: {
        ID: '0',
        mark: this.$route.params.mark,
        editID: this.$route.params.id,
        reminder: [],
        published: '',
        beforeDead: '',
        beforeMaintainMiddle: '',
        beforeMaintainBig: '',
        textTemplate: ''
      },
      rules: {
        beforeDead: [{ required: true, validator: this.valNumber, trigger: 'blur' }],
        beforeMaintainMiddle: [{ required: true, validator: this.valNumber, trigger: 'blur' }],
        beforeMaintainBig: [{ required: true, validator: this.valNumber, trigger: 'blur' }],
        PhoneNum: [
          { validator: this.validatePhone, trigger: 'blur' }
        ]
      },
      title: '',
      loading: true,
      Users: [],
      ReadOnly: false
    }
  },
  created () {
    this.loading = false
    if (this.$route.query.id !== 0) {
      this.form.ID = this.$route.query.id
    }
    if (this.$route.params.mark === 'look') {
      this.ReadOnly = true
      this.getobj()
    }
    if (this.$route.params.mark === 'edit') {
      this.getobj()
    }
    let subTitle = this.ReadOnly ? ' 设备寿命配置查看' : ' 设备寿命配置'
    this.title = '| ' + subTitle
    this.$emit('pageInfo', {
      title: ' | 寿命配置 | ' + subTitle,
      isShowBackBtn: true,
      url: 'SeeMaintainConfig'
    })
  },
  methods: {
    validateNumber () {
      validateNumberCommon(this.menuOrder)
    },
    // 验证
    validateInput (val) {
      validateInputCommon(val)
    },
    validateAll () {
      if (!validateInputCommon(this.Name)) return false
      if (this.PhoneNum.text) {
        if (!vPhone(this.PhoneNum.text)) {
          this.PhoneNum.tips = '电话格式不正确'
          return false
        }
      }

      return true
    },
    validateName (rule, value, callback) {
      value = value.replace(/\s*/g, '')
      if (value === '') {
        callback(new Error('该字段不能空'))
      } else if (!vInput(value)) {
        callback(new Error('此项含有非法字符'))
      } else {
        callback()
      }
    },
    valNumber (rule, value, callback) {
      if (value === '') {
        callback(new Error('该字段不能空'))
      } else if (!Number.isInteger(Number(value))) {
        callback(new Error('请输入整形数字'))
      } else {
        callback()
      }
    },
    validatePhone (rule, value, callback) {
      if (value) {
        if (!vPhone(value)) {
          callback(new Error('电话号码验证错误'))
        } else {
          callback()
        }
      } else {
        callback()
      }
    },
    submitForm () {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          var _reminder
          if (this.form.reminder.length === 0) {
            this.$message({
              message: '验证失败，请选择提醒方式',
              type: 'error'
            })
            return
          } else if (this.form.reminder.length === 2) {
            _reminder = 3
          } else if (this.form.reminder[0] === '短信') {
            _reminder = 1
          } else if (this.form.reminder[0] === '邮件') {
            _reminder = 2
          }
          var obj = {
            ID: this.$route.params.id,
            reminder: _reminder,
            beforeDead: this.form.beforeDead,
            beforeMaintainMiddle: this.form.beforeMaintainMiddle,
            beforeMaintainBig: this.form.beforeMaintainBig,
            published: this.form.published,
            eqpid: -1,
            textTemplate: this.form.textTemplate
          }
          // alert(obj.ID)
          // alert(this.form.mark)
          // eventBus.$emit('submit', obj)
          if (this.form.mark === 'add') {
            api.addEquipmentConfig(obj).then((res) => {
              if (res.code === ApiRESULT.Success) {
              // this.$refs.tree.append({
              //   id: res.data.id,
              //   label: res.data.name,
              //   node_type: res.data.nodeType
              // }, res.data.parentID)
                this.$message.success('保存成功')
                this.$router.push({
                  name: 'SeeMaintainConfig'
                })
              } else {
                this.$message.error('保存失败')
              }
            })
          } else {
            api.updateEquipmentConfig(obj).then((res) => {
              if (res.code === ApiRESULT.Success) {
                this.$message.success('更新成功')
                this.$router.push({
                  name: 'SeeMaintainConfig'
                })
              } else {
                this.$message.error('更新失败')
              }
            })
          }
        }
      })
    },
    // 修改用户时获取用户资料
    getobj () {
      var id = this.$route.params.id
      api.getEquipmentConfigByID(id).then(res => {
        this.loading = false
        let _res = res.data
        // this.userID = _res.id
        var r = _res.reminder
        if (r === 1) {
          this.form.reminder = ['短信']
        } else if (r === 2) {
          this.form.reminder = ['邮件']
        } else if (r === 3) {
          this.form.reminder = ['短信', '邮件']
        }
        var p = _res.published
        this.form.published = p
        this.form.beforeDead = _res.beforeDead
        this.form.beforeMaintainMiddle = _res.beforeMaintainMiddle
        this.form.beforeMaintainBig = _res.beforeMaintainBig
        this.form.textTemplate = _res.textTemplate
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
.btn-group{
  margin: 15px 0;
  text-align: center;

  button{
    border-color: $color-main-btn;
    background: $color-main-btn;
  }
}
// 表单验证
.custom-form /deep/ .el-form-item.is-success .el-input__inner{
  border-color: #fff
}
.custom-form /deep/ .el-form-item.is-error .el-input__inner{
  border-color: #ffffff
}
.custom-form /deep/ .el-form-item__error{
  color: red;
}
.custom-form /deep/ .el-form-item__label{
  color: #fff
}
</style>
