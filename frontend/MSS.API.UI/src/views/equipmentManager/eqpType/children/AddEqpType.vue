<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeEqpTypeList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">设备类型ID</span>
              <div class="inp disabled">
                <el-input v-model="eqpTypeID" :disabled="true"></el-input>
              </div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">设备类型名称<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input placeholder="请输入设备类型名称" v-model="eqpTypeName.text" @keyup.native="validateInput()"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ eqpTypeName.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">型号</span>
              <div class="inp">
                <el-input v-model="model.text" placeholder="请输入设备类型型号" @keyup.native="validateInputNull(model)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ model.tips }}</p>
          </li>
          <div class="upload-list">
            <upload-pdf :systemResource="systemResource" :fileIDs="fileIDs" @getFileIDs="getFileIDs"></upload-pdf>
          </div>
        </ul>
        <div class="con-padding-horizontal cause">
            <span class="lable">设备类型描述：</span>
            <el-input type="textarea" v-model="eqpTypeDesc.text" placeholder="请输入设备类型描述" @keyup.native="validateInputNull(eqpTypeDesc)"></el-input>
        </div>
        <div class="validate-tips left">{{ eqpTypeDesc.tips }}</div>
        <!-- 按钮 -->
        <div class="btn-group">
          <x-button class="close">
            <router-link :to="{name: 'SeeEqpTypeList'}">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="save">保存</x-button>
        </div>
      </el-scrollbar>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vInput, nullToEmpty } from '@/common/js/utils.js'
import { systemResource } from '@/common/js/dictionary.js'
import { isUploadFinished } from '@/common/js/UpDownloadFileHelper.js'
import XButton from '@/components/button'
import api from '@/api/eqpApi'
import MyUploadPDF from '@/components/UploadPDF'
export default {
  name: 'AddEqpType',
  components: {
    XButton,
    'upload-pdf': MyUploadPDF
  },
  data () {
    return {
      systemResource: systemResource.eqpType,
      loading: false,
      title: '| 添加设备类型',
      eqpTypeID: '',
      eqpTypeName: {text: '', tips: ''},
      model: {text: '', tips: ''},
      eqpTypeDesc: {text: '', tips: ''},
      isEditFile: false,
      fileIDs: '',
      fileIDsEdit: []
    }
  },
  created () {
    this.init()
  },
  methods: {
    init () {
      if (this.$route.query.type !== 'Add') {
        this.title = '| 修改设备类型'
        this.loading = true
        this.getEqpType()
      }
    },
    getFileIDs (ids) {
      this.fileIDsEdit = ids
      if (this.$route.query.type !== 'Add') {
        this.isEditFile = true
      }
    },
    getEqpType () {
      api.getEqpTypeByID(this.$route.query.id).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.eqpTypeID = data.id
          this.eqpTypeName.text = data.tName
          this.model.text = nullToEmpty(data.model)
          this.eqpTypeDesc.text = data.desc
          this.fileIDs = data.uploadFiles
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
      if (!validateInputCommon(this.eqpTypeName)) {
        return false
      }
      return true
    },
    validateInputAll () {
      if (!this.validateInput() || !this.validateInputNull(this.model) || !this.validateInputNull(this.eqpTypeDesc)) {
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
      if (this.fileIDsEdit.length !== 0 && !isUploadFinished(this.fileIDsEdit)) {
        this.$message({
          message: '文件正在上传中，请耐心等待',
          type: 'warning'
        })
        return
      }
      let eqpType = {
        TName: this.eqpTypeName.text,
        Desc: this.eqpTypeDesc.text,
        Model: this.model.text,
        UploadFiles: this.fileIDsEdit.length === 0 && !this.isEditFile ? '' : JSON.stringify(this.fileIDsEdit)
      }
      if (this.$route.query.type === 'Add') {
        api.addEqpType(eqpType).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeEqpTypeList'})
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
        eqpType.ID = this.eqpTypeID
        api.updateEqpType(eqpType).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeEqpTypeList'})
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
  width: 50%;
}
.left{
  text-indent: 9.5%
}
</style>
