<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeStorageLocationList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">库位代码<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input placeholder="请输入库位代码" v-model="storageLocationName.text" @keyup.native="validateInput(storageLocationName)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ storageLocationName.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">仓库<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-select v-model="warehouse.text" clearable filterable placeholder="请选择仓库" @change="validateSelect(warehouse)">
                <el-option
                  v-for="item in warehouseList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id">
                </el-option>
              </el-select>
              </div>
            </div>
            <p class="validate-tips">{{ warehouse.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">行号</span>
              <div class="inp">
                <el-input placeholder="请输入计量单位" v-model="row.text" @keyup.native="validateInputNull(row)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ row.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">列号</span>
              <div class="inp">
                <el-input placeholder="请输入列号" v-model="column.text" @keyup.native="validateInputNull(column)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ column.tips }}</p>
          </li>
          <li class="list list-block">
            <div class="inp-wrap">
              <span class="text span-block">描述</span>
              <el-input type="textarea" :rows="3" class="whole-line" placeholder="请输入备注" v-model="des.text" @keyup.native="validateInputNull(des)"></el-input>
            </div>
            <p class="validate-tips">{{ des.tips }}</p>
          </li>
        </ul>
        <!-- 按钮 -->
        <div class="btn-group">
          <x-button class="close">
            <router-link :to="{name: 'SeeStorageLocationList'}">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="save">保存</x-button>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vInput, nullToEmpty, validateNumberCommon } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/wmsApi'
export default {
  name: 'AddStorageLocation',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      title: '| 添加库位',
      storageLocationID: '',
      warehouse: {text: '', tips: ''},
      warehouseList: [],
      storageLocationName: {text: '', tips: ''},
      column: {text: '', tips: ''},
      des: {text: '', tips: ''},
      row: {text: '', tips: ''}
    }
  },
  created () {
    this.init()
  },
  methods: {
    init () {
      api.getWarehouseAll().then(res => {
        this.warehouseList = res.data
        if (this.$route.query.type !== 'Add') {
          this.title = '| 修改库位'
          this.loading = true
          this.getStorageLocation()
        }
      }).catch(err => console.log(err))
    },
    getStorageLocation () {
      api.getStorageLocationByID(this.$route.query.id).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.storageLocationID = data.id
          this.storageLocationName.text = data.name
          this.warehouse.text = data.warehouse
          this.column.text = nullToEmpty(data.column)
          this.row.text = nullToEmpty(data.row)
          this.des.text = nullToEmpty(data.des)
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
      if (!this.validateInput(this.storageLocationName) || !this.validateInputNull(this.row) || !this.validateInputNull(this.column) || !this.validateSelect(this.warehouse) || !this.validateInputNull(this.des)) {
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
      let storageLocation = {
        Name: this.storageLocationName.text,
        Warehouse: this.warehouse.text,
        Column: this.column.text,
        Row: this.row.text,
        Des: this.des.text
      }
      if (this.$route.query.type === 'Add') {
        api.addStorageLocation(storageLocation).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeStorageLocationList'})
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
        storageLocation.ID = this.storageLocationID
        api.updateStorageLocation(storageLocation).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeStorageLocationList'})
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
