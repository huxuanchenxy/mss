<template>
  <div class="wrap" :class="isShow">
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list" v-show="false">
          <div class="inp-wrap">
            <span class="text">代码编号</span>
            <div class="inp">
              <el-input v-model="codeID" :disabled="isShow === 'edit'"></el-input>
            </div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">代码名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model="typeName.text" placeholder="请输入代码名称" @keyup.native="validateInput(typeName)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ typeName.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">代码值<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model="typeValue.text" placeholder="请输入代码值" @keyup.native="validateInput(typeValue)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ typeValue.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">子代码名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model="subTypeName.text" placeholder="请输子入代码名称" @keyup.native="validateInput(subTypeName)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ subTypeName.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">子代码值<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model="subTypeValue.text" placeholder="请输入子代码值" @keyup.native="validateNumber(subTypeValue)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ subTypeValue.tips }}</p>
        </li>
        <li class="list" v-show="isShow === 'edit'"></li>
      </ul>
    </div>
    <div class="btn-enter">
      <x-button class="close" @click.native="close">取消</x-button>
      <x-button class="active" @click.native="enter">保存</x-button>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, validateNumberCommon } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/authApi'
export default {
  name: 'AddCode',
  components: {
    XButton
  },
  data () {
    return {
      codeID: '',
      isShow: '',
      editCodeID: 0,
      typeValue: {
        text: '',
        tips: ''
      },
      typeName: {
        text: '',
        tips: ''
      },
      subTypeValue: {
        text: '',
        tips: ''
      },
      subTypeName: {
        text: '',
        tips: ''
      }
    }
  },
  created () {
    this.isShow = this.$route.query.t
    this.editCodeID = this.$route.query.id
    if (this.isShow === 'edit') {
      this.getCode()
      this.$emit('title', '| 修改代码')
    } else {
      this.$emit('title', '| 添加代码')
    }
  },
  methods: {
    // 添加代码
    enter () {
      // this.$emit('title', '| 代码定义')
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      let code = {
        code_name: this.typeName.text,
        code: this.typeValue.text,
        sub_code_name: this.subTypeName.text,
        sub_code: this.subTypeValue.text
      }
      // this.$emit('reload', Math.random())
      if (this.isShow === 'add') {
        // 添加代码
        api.addDictionary(code).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeCodeList'
                })
              }
            })
          } else {
            this.$message({
              message: res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else if (this.isShow === 'edit') {
        code.id = this.codeID
        // 修改代码
        api.updateDictionary(code).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeCodeList'
                })
              }
            })
          } else {
            this.$message({
              message: res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      }
    },

    // 修改代码时获取代码资料
    getCode () {
      api.getDictionaryByID(this.editCodeID).then(res => {
        let _res = res.data
        this.codeID = _res.id
        this.typeName.text = _res.code_name
        this.typeValue.text = _res.code
        this.subTypeName.text = _res.sub_code_name
        this.subTypeValue.text = _res.sub_code
      }).catch(err => console.log(err))
    },

    // 关闭页面
    close () {
      this.$router.push({ name: 'SeeCodeList' })
      // this.$emit('title', '| 角色定义')
    },

    validateInput (val) {
      validateInputCommon(val)
    },

    validateNumber () {
      validateNumberCommon(this.subTypeValue)
    },
    validateAll () {
      if (!validateInputCommon(this.typeName)) return false
      if (!validateInputCommon(this.typeValue)) return false
      if (!validateInputCommon(this.subTypeName)) return false
      if (!validateNumberCommon(this.subTypeValue)) return false
      return true
    }
  }
}
</script>
<style lang="scss" scoped>
$height: $content-height - 56;
.wrap{
  height: percent($height, $content-height);

  &.add{
    .operation{
      height: percent(180, $height);
    }

    .content{
      height: percent($height - 180, $height);
    }
  }

  &.edit{
    .operation{
      height: percent(130, $height);
    }

    .content{
      height: percent($height - 130, $height);
    }
  }
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
.btn-enter{
  margin: 15px 0;
  text-align: center;

  button{
    border-color: $color-main-btn;
    background: $color-main-btn;
  }
}
</style>
