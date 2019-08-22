<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeSparePartsList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">物资名称<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input placeholder="请输入物资名称" v-model="sparePartsName.text" @keyup.native="validateInput(sparePartsName)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ sparePartsName.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">物资类型<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="type.text" clearable filterable placeholder="请选择物资类型" @change="validateSelect(type)">
                <el-option
                  v-for="item in typeList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ type.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">计量单位<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input placeholder="请输入计量单位" v-model="unit.text" @keyup.native="validateInput(unit)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ unit.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">规格型号</span>
              <div class="inp">
                <el-input placeholder="请输入规格型号" v-model="model.text" @keyup.native="validateInputNull(model)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ model.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">适用设备类型</span>
              <div class="inp">
                <el-select v-model="eqpType" clearable filterable placeholder="请选择设备类型">
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
          <li class="list">
            <div class="inp-wrap">
              <span class="text">计划价格(RMB)</span>
              <div class="inp">
                <el-input placeholder="请输入计划价格" v-model="planPrice.text" @keyup.native="validateNumber(planPrice)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ planPrice.tips }}</p>
          </li>
          <li class="list list-block">
            <div class="inp-wrap">
              <span class="text span-block">英语描述</span>
              <el-input class="whole-line" placeholder="请输入英语描述" v-model="englishDes.text" @keyup.native="validateInputNull(englishDes)"></el-input>
            </div>
            <p class="validate-tips">{{ englishDes.tips }}</p>
          </li>
          <li class="list list-block">
            <div class="inp-wrap">
              <span class="text span-block">备注</span>
              <el-input type="textarea" :rows="3" class="whole-line" placeholder="请输入备注" v-model="remark.text" @keyup.native="validateInputNull(remark)"></el-input>
            </div>
            <p class="validate-tips">{{ remark.tips }}</p>
          </li>
        </ul>
        <!-- 按钮 -->
        <div class="btn-group">
          <x-button class="close">
            <router-link :to="{name: 'SeeSparePartsList'}">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="save">保存</x-button>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vInput, nullToEmpty, validateNumberCommon } from '@/common/js/utils.js'
import { dictionary } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import apiAuth from '@/api/authApi'
import apiEqp from '@/api/eqpApi'
import api from '@/api/wmsApi'
export default {
  name: 'AddSpareParts',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      title: '| 添加物资',
      sparePartsID: '',
      type: {text: '', tips: ''},
      typeList: [],
      eqpType: '',
      eqpTypeList: [],
      planPrice: {text: 0, tips: ''},
      englishDes: {text: '', tips: ''},
      sparePartsName: {text: '', tips: ''},
      model: {text: '', tips: ''},
      remark: {text: '', tips: ''},
      unit: {text: '', tips: ''}
    }
  },
  created () {
    this.init()
  },
  methods: {
    init () {
      if (this.$route.query.type !== 'Add') {
        this.title = '| 修改物资'
        this.loading = true
        apiAuth.getSubCode(dictionary.sparePartsType).then(res => {
          this.typeList = res.data
          // 设备类型加载
          apiEqp.getEqpTypeAll().then(res => {
            this.eqpTypeList = res.data
            this.getSpareParts()
          }).catch(err => console.log(err))
        }).catch(err => console.log(err))
      } else {
        apiAuth.getSubCode(dictionary.sparePartsType).then(res => {
          this.typeList = res.data
        }).catch(err => console.log(err))
        // 设备类型加载
        apiEqp.getEqpTypeAll().then(res => {
          this.eqpTypeList = res.data
        }).catch(err => console.log(err))
      }
    },
    getSpareParts () {
      api.getSparePartsByID(this.$route.query.id).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.sparePartsID = data.id
          this.sparePartsName.text = data.name
          this.type.text = data.type
          this.model.text = nullToEmpty(data.model)
          this.unit.text = nullToEmpty(data.unit)
          this.eqpType = nullToEmpty(data.eqpType)
          this.planPrice.text = data.planPrice
          this.englishDes.text = nullToEmpty(data.englishDes)
          this.remark.text = nullToEmpty(data.remark)
        }
        this.loading = false
      }).catch(err => console.log(err))
    },
    validateInputNull (val) {
      if (!vInput(val.text)) {
        val.tips = '此项含有非法字符'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateNumber (val) {
      validateNumberCommon(val)
    },
    validateInput (val) {
      if (!validateInputCommon(val)) {
        return false
      }
      return true
    },
    validateSelect (val) {
      if (val.text === '') {
        val.tips = '此项必选'
        return false
      } else {
        val.tips = ''
        return true
      }
    },
    validateInputAll () {
      if (!this.validateInput(this.sparePartsName) || !this.validateInput(this.unit) || !this.validateInputNull(this.model) || !this.validateSelect(this.type) || !validateNumberCommon(this.planPrice)) {
        return false
      }
      return true
    },
    save () {
      if (!this.validateInputAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      let spareParts = {
        Name: this.sparePartsName.text,
        Type: this.type.text,
        model: this.model.text,
        unit: this.unit.text,
        EqpType: this.eqpType,
        PlanPrice: this.planPrice.text,
        EnglishDes: this.englishDes.text,
        Remark: this.remark.text
      }
      if (this.$route.query.type === 'Add') {
        api.addSpareParts(spareParts).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeSparePartsList'})
            this.$message({
              message: '添加成功',
              type: 'success'
            })
          } else {
            this.$message({
              message: res.msg === '' ? '添加失败' : res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else {
        spareParts.ID = this.sparePartsID
        api.updateSpareParts(spareParts).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeSparePartsList'})
            this.$message({
              message: '修改成功',
              type: 'success'
            })
          } else {
            this.$message({
              message: res.msg === '' ? '修改失败' : res.msg,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      }
    }
  }
}
</script>
<style lang="scss" scoped>
// 显示大图容器
.el-dialog__wrapper{
  width: 100%;
  height: 100%;

  .el-carousel{
    height: 100%;
  }
}

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

  .list-block{
    width: 100%;
    .span-block{
      width: 8.5%;
    }
    .whole-line{
      width: 86.5%;
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
  width: -webkit-fill-available;
}
.left{
  text-indent: 9.5%
}
</style>
