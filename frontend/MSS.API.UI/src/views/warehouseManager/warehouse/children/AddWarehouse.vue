<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeWarehouseList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">仓库名称<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input placeholder="请输入仓库名称" v-model="warehouseName.text" @keyup.native="validateInput()"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ warehouseName.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">联系电话</span>
              <div class="inp">
                <el-input placeholder="请输入联系电话" v-model="mobile.text" @keyup.native="validateInputNull(mobile)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ mobile.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">联系人</span>
              <div class="inp">
                <el-input placeholder="请输入联系人" v-model="contact.text" @keyup.native="validateInputNull(contact)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ contact.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">地址</span>
              <div class="inp">
                <el-input placeholder="请输入地址" v-model="address.text" @keyup.native="validateInputNull(address)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ address.tips }}</p>
          </li>
        </ul>
        <!-- 按钮 -->
        <div class="btn-group">
          <x-button class="close">
            <router-link :to="{name: 'SeeWarehouseList'}">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="save">保存</x-button>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vInput, nullToEmpty } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/wmsApi'
export default {
  name: 'AddWarehouse',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      title: '| 添加仓库',
      warehouseID: '',
      warehouseName: {text: '', tips: ''},
      mobile: {text: '', tips: ''},
      contact: {text: '', tips: ''},
      address: {text: '', tips: ''}
    }
  },
  created () {
    this.init()
  },
  methods: {
    init () {
      if (this.$route.query.type !== 'Add') {
        this.title = '| 修改仓库'
        this.loading = true
        this.getWarehouse()
      }
    },
    getWarehouse () {
      api.getWarehouseByID(this.$route.query.id).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.warehouseID = data.id
          this.warehouseName.text = data.name
          this.mobile.text = nullToEmpty(data.mobile)
          this.contact.text = nullToEmpty(data.contact)
          this.address.text = nullToEmpty(data.address)
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
    validateInput () {
      if (!validateInputCommon(this.warehouseName)) {
        return false
      }
      return true
    },
    validateInputAll () {
      if (!this.validateInput() || !this.validateInputNull(this.mobile) || !this.validateInputNull(this.contact) || !this.validateInputNull(this.address)) {
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
      let warehouse = {
        Name: this.warehouseName.text,
        Mobile: this.mobile.text,
        Contact: this.contact.text,
        Address: this.address.text
      }
      debugger
      if (this.$route.query.type === 'Add') {
        api.addWarehouse(warehouse).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeWarehouseList'})
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
        warehouse.ID = this.warehouseID
        api.updateWarehouse(warehouse).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeWarehouseList'})
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
