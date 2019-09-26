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
        <router-link :to="{name:'MetroLineList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <el-form ref="form" :rules="rules" label-position="left" :model="form"
        class="custom-form" label-width="80px">
        <el-row :gutter="40" style="padding-top:20px;">
          <el-col :span="8">
            <el-form-item label="线路名称" prop="LineName">
              <el-input v-model="form.LineName" ></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row style="padding-top:20px;">
          <el-col>
            <el-form-item label="描述" prop="Description">
              <el-input type="textarea" v-model="form.Description"></el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row style="padding-top:20px;">
          <el-col align="center">
            <el-form-item>
              <x-button class="close">
                <router-link :to="{ name: 'MetroLineList' }">取消</router-link>
              </x-button>
              <x-button class="active" @click.native="save">保存</x-button>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
  </div>
</template>
<script>
// test
import { vInput, ApiRESULT } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/metroLineApi'
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
      form: {
        ID: '0',
        LineName: '',
        Description: ''
      },
      rules: {
        LineName: [
          { required: true, validator: this.validateName, message: '该字段不能为空,或含有非法字符', trigger: 'blur' }
        ]
      }
    }
  },
  created () {
  },
  activated () {
    this.isShow = this.$route.params.mark
    if (this.isShow === 'edit') {
      this.title = '| 修改线路'
      this.ID = this.$route.params.id
      this.getMetroLine()
    } else {
      this.title = '| 添加线路'
      this.reset()
    }
  },
  methods: {
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
    save () {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          let param = this.form
          if (this.isShow === 'add') {
            api.save(param).then((res) => {
              if (res.code === ApiRESULT.Success) {
                this.$message.success('添加成功')
                this.$router.push({
                  name: 'MetroLineList'
                })
              } else if (res.code === ApiRESULT.DataIsExist) {
                this.$message.error('名称重复')
              } else {
                this.$message.error('添加失败')
              }
            })
          } else if (this.isShow === 'edit') {
            api.update(param.ID, param).then((res) => {
              if (res.code === ApiRESULT.Success) {
                this.$message.success('修改成功')
                this.$router.push({
                  name: 'MetroLineList'
                })
              } else if (res.code === ApiRESULT.DataIsExist) {
                this.$message.error('名称重复')
              } else {
                this.$message.error('修改失败')
              }
            })
          }
        }
      })
    },

    // 修改用户时获取用户资料
    getMetroLine () {
      this.loading = true
      api.getByID(this.ID).then(res => {
        this.loading = false
        if (res.code === ApiRESULT.Success) {
          this.form.ID = res.data.id
          this.form.LineName = res.data.lineName
          this.form.Description = res.data.description
        }
      }).catch(err => console.log(err))
    },

    // 还原初始设置
    reset () {
      this.$refs['form'].resetFields()
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

      .text{
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
