<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'ExpertDataList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">请输入关键字</span>
              <div class="inp">
              <el-input placeholder="请输入关键字" v-model="keyword.text" @keyup.native="validateInput(keyword)"></el-input>
            </div>
            </div>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">设备类型<em class="validate-mark">*</em></span>
              <div class="inp">
              <el-select v-model="deviceType.id" clearable placeholder="请选择" @change="validatedeviceTypeSelect()">
                <option disabled value="" selected>请选择</option>
                 <el-option
                 v-for="item in deviceTypeList"
                 :key="item.key"
                 :value="item.id"
                 :label="item.deviceTypeName">
                 </el-option>
              </el-select>
            </div>
            </div>
            <p class="validate-tips">{{ deviceType.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">所属部门</span>
              <div class="inp">
              <el-select v-model="dept.id" clearable placeholder="请选择" @change="validatedeptSelect()">
                <option disabled value="" selected>请选择</option>
                 <el-option
                 v-for="item in deptList"
                 :key="item.key"
                 :value="item.id"
                 :label="item.deptName">
                 </el-option>
              </el-select>
            </div>
            </div>
            <p class="validate-tips">{{ dept.tips }}</p>
          </li>
        </ul>
        <div class="con-padding-horizontal cause">
          <span class="lable">内容</span>
          <el-input type="textarea" v-model="content.text" placeholder="请输入内容"></el-input>
          <p class="validate-tips">{{ content.tips }}</p>
        </div>
        <!-- 上传图片列表 -->
        <div class="upload-wrap con-padding-horizontal">
          <span class="lable">视频上传</span>
          <el-upload
            ref="uploadvedioing"
            :action="``"
            accept="application/pdf"
            :file-list="fileList.vedioing"
            :show-file-list="true"
            list-type="picture-card"
            :auto-upload="false"
            :http-request="uploadvedio"
            :on-change="onChangevedio"
            :on-preview="preview">
            <i class="iconfont icon-pdf"></i>
          </el-upload>
        </div>
        <div class="upload-wrap con-padding-horizontal">
          <span class="lable">附件上传</span>
          <el-upload
            ref="uploadattaching"
            :action="``"
            accept="application/pdf"
            :file-list="fileList.attaching"
            :show-file-list="true"
            list-type="picture-card"
            :auto-upload="false"
            :http-request="uploadattaching"
            :on-change="onChangeattaching"
            :on-preview="preview">
            <i class="iconfont icon-pdf"></i>
          </el-upload>
        </div>
        <!-- 按钮 -->
        <div class="btn-group">
          <x-button class="close">
            <router-link :to="{name: 'ExpertDataList'}">取消</router-link>
          </x-button>
          <x-button class="active" @click.native="enter">保存</x-button>
        </div>
      </el-scrollbar>
    </div>

    <el-dialog
      :visible.sync="centerDialogVisible"
      :modal-append-to-body="false"
      custom-class="show-list-wrap"
      center>
      <iframe :src="previewUrl" width="100%" height="100%" frameborder="0"></iframe>
    </el-dialog>
  </div>
</template>
<script>
import { validateInputCommon, vInput } from '@/common/js/utils.js'
import api from '@/api/ExpertApi'
import XButton from '@/components/button'
export default {
  name: 'AddExpertData',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      isShow: this.$route.params.mark,
      editExpertID: this.$route.params.id,
      keyword: {
        text: '',
        tips: ''
      },
      Experttitle: {
        text: '',
        tips: ''
      },
      content: {
        text: '',
        tips: ''
      },
      dept: {
        id: '',
        tips: ''
      },
      deptList: [], // 场区类型: 车站\正线轨行区\保护区\车场生产区
      deviceType: {
        id: '',
        tips: ''
      },
      deviceTypeList: [], // 场区类型: 车站\正线轨行区\保护区\车场生产区
      videofile: {},
      attchfile: {},
      ExpertID: '',
      // title: '| 添加设备类型',
      // eqpTypeID: '',
      // eqpTypeName: {text: '', tips: ''},
      // model: {text: '', tips: ''},
      // eqpTypeDesc: {text: '', tips: ''},
      centerDialogVisible: false,
      fileList: {
        vedioing: [],
        attaching: []
      },
      needUpload: {
        vedioing: [],
        attaching: []
      },
      previewUrl: {
        vedioing: '',
        attaching: ''
      },
      pageType: ''
    }
  },
  created () {
    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 添加专家库'
    } else if (this.isShow === 'edit') {
      this.loading = true
      this.title = '| 修改专家库'
      this.getExpertData()
    }
    // 设备配置类型列表
    api.GetDeviceTypeList().then(res => {
      this.deviceTypeList = res.data
    }).catch(err => console.log(err))

    // 部门列表
    api.GetdeptList().then(res => {
      this.deptList = res.data
    }).catch(err => console.log(err))
  },
  methods: { // 添加权限组
    enter () {
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      let fd = new FormData()
      if (this.$refs.uploadvedioing.uploadFiles.length > 0) {
        if (this.$refs.uploadvedioing.uploadFiles[0].status !== 'success') {
          this.$refs.uploadvedioing.submit()
          fd.append('file', this.needUpload.vedioing[0])
          fd.append('Pvedioing', this.needUpload.vedioing[0].name)
        }
      }
      if (this.$refs.uploadattaching.uploadFiles.length > 0) {
        if (this.$refs.uploadattaching.uploadFiles[0].status !== 'success') {
          this.$refs.uploadattaching.submit()
          fd.append('file', this.needUpload.attaching[0])
          fd.append('Pattaching', this.needUpload.attaching[0].name)
        }
      }
      let tbexpertdata = {
        device_type: this.deviceType.id,
        deptID: this.dept.id,
        keyword: this.keyword.text,
        title: this.Experttitle.text,
        content: this.content.text,
        video_file: '1',
        attch_file: '1',
        sort: 'asc',
        is_Used: '1',
        is_Deleted: '0',
        remark: ''
      }
      if (this.isShow === 'add') {
        // 添加站区
        api.Save(tbexpertdata).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'ExpertDataList'
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
        debugger
        tbexpertdata.id = this.ExpertID
        // 修改权限组
        api.Update(tbexpertdata).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'ExpertDataList'
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
    // 修改权限组时获取权限组资料
    getExpertData () {
      debugger
      let id = this.editExpertID
      api.GetExpertDataById(id).then(res => {
        this.loading = false
        let _res = res.data
        this.ExpertID = _res.id
        this.keyword.text = _res.keyword
        this.Experttitle.text = _res.title
        this.content.text = _res.content
        this.deviceType.id = _res.device_type
        this.dept.id = _res.deptid
      }).catch(err => console.log(err))
    },
    // 验证
    validateInput (val) {
      validateInputCommon(val)
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
    validatedeviceTypeSelect () {
      if (this.deviceType.id === '') {
        this.deviceType.tips = '此项必选'
        return false
      } else {
        this.deviceType.tips = ''
        return true
      }
    },
    validatedeptSelect () {
      if (this.dept.id === '') {
        this.dept.tips = '此项必选'
        return false
      } else {
        this.dept.tips = ''
        return true
      }
    },
    validateNumber () {
      // validateNumberCommon(this.groupOrder)
    },

    validateAll () {
      if (!validateInputCommon(this.keyword)) return false
      if (!validateInputCommon(this.Experttitle)) return false
      if (!validateInputCommon(this.content)) return false
      if (!this.validatedeviceTypeSelect()) return false
      if (!this.validatedeptSelect()) return false
      return true
    },
    handleRemove (file, fileList) {
      console.log(file, fileList)
    },
    handlePreview (file) {
      console.log(file)
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
// 图片预览
/deep/ .show-list-wrap{
  position: absolute;
  z-index: 9;
  top: 0;
  left: 0;
  overflow: hidden;
  width: 100%;
  height: 100%;
  background: #1c1c1c;
  transform: translate(0px);

  .el-carousel{
    height: 100%;
  }

  img{
    max-width: 100%;
    height: 100%;
  }

  .icon-close{
    position: absolute;
    z-index: 99;
    top: 3px;
    right: 3px;
    width: 27px;
    height: 27px;
    background: url(../../../../common/images/icon-close.png) no-repeat 0 0/100% 100%;
    cursor: pointer;
  }

  .el-dialog__header{
    display: block;
    padding: 0;

    .el-dialog__headerbtn{
      top: -14px;
      right: -14px;
      width: 27px;
      height: 27px;
      background: url(../../../../common/images/icon-close.png) no-repeat 0 0/100% 100%;
    }

    .el-dialog__close{
      display: none;
    }
  }

  .el-dialog__body{
    height: 100%;
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

// 上传图片列表
.picture-list-wrap{
  .list{
    width: 80px;
    height: 80px;
    background-color: $color-white;
    border-radius: $border-radius;
    text-align: center;
    line-height: 80px;

    &:before{
      content: "\e6cc";
      font-size: 28px;
      font-family: "iconfont";
      color: #8c939d;
    }
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
</style>
