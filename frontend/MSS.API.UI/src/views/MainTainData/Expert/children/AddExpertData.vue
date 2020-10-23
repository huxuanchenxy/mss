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
              <span class="text">关键字<em class="validate-mark">*</em></span>
              <div class="inp">
              <el-input placeholder="请输入关键字" v-model="keyword.text" @keyup.native="validateInput(keyword)"></el-input>
            </div>
            </div>
            <p class="validate-tips">{{ keyword.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">主题<em class="validate-mark">*</em></span>
              <div class="inp">
              <el-input placeholder="请输入主题" v-model="Experttitle.text" @keyup.native="validateInput(Experttitle)"></el-input>
            </div>
            </div>
             <p class="validate-tips">{{ Experttitle.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">设备类型<em class="validate-mark">*</em></span>
              <div class="inp">
              <!-- <el-select v-model="deviceType.id" clearable placeholder="请选择" @change="validatedeviceTypeSelect()">
                <option disabled value="" selected>请选择</option>
                 <el-option
                 v-for="item in deviceTypeList"
                 :key="item.key"
                 :value="item.id"
                 :label="item.deviceTypeName">
                 </el-option>
              </el-select> -->
              <el-select v-model="deviceType.id" clearable filterable placeholder="请选择" @change="validatedeviceTypeSelect()">
                <option disabled value="" selected>请选择</option>
                <el-option
                  v-for="item in deviceTypeList"
                  :key="item.key"
                  :label="item.tName"
                  :value="item.id">
                </el-option>
              </el-select>
            </div>
            </div>
            <p class="validate-tips">{{ deviceType.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">录入部门<em class="validate-mark">*</em></span>
              <div class="inp">
              <!-- <el-select v-model="teamPath.text" clearable placeholder="请选择" @change="validatedeptSelect()">
                <option disabled value="" selected>请选择</option>
                 <el-option
                 v-for="item in deptList"
                 :key="item.key"
                 :value="item.id"
                 :label="item.deptName">
                 </el-option> </el-select>-->
               <el-cascader class="cascader_width" clearable
                    :props="defaultParams"
                    change-on-select
                    @change="cascader_change_copy"
                    :show-all-levels="true"
                    :options="teamList"
                    v-model="teamPath.text">
                  </el-cascader>
            </div>
            </div>
            <p class="validate-tips">{{ teamPath.tips }}</p>
          </li>
        </ul>
        <div class="con-padding-horizontal cause">
          <span class="lable">内容</span>
          <el-input type="textarea" v-model="content.text" placeholder="请输入内容"></el-input>
          <p class="validate-tips">{{ content.tips }}</p>
        </div>
        <!-- 上传图片列表 -->
        <br/>
        <div class="upload-wrap con-padding-horizontal">
          <!-- <upload-pdf :fileType="filevedioType"
                      label="视频上传"
                      :fileIDs="filevedioIDs"
                      @getFileIDs="getvedioFileID"></upload-pdf> -->
               <!-- <upload-vedio :fileType="filevedioType"
                             label="视频上传"
                             :fileIDs="filevedioIDs"
                             @getFileIDs="getvedioFileID" ></upload-vedio> -->
        </div>
        <br/>
        <div class="upload-wrap con-padding-horizontal">
          <!-- <upload-pdf :fileType="fileattachType"
                      label="附件上传"
                      :fileIDs="fileattachIDs"
                      @getFileIDs="getattachFileID"></upload-pdf> -->
          <upload-pdf :systemResource="systemResource" :fileIDs="fileIDs" @getFileIDs="getattachFileID"></upload-pdf>
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
import { validateInputCommon, vInput, FileType } from '@/common/js/utils.js'
import api from '@/api/ExpertApi'
import axios from '@/api/interceptors'
import { systemResource } from '@/common/js/dictionary.js'
import eqpApi from '@/api/eqpApi.js'
import apiOrg from '@/api/orgApi'
import XButton from '@/components/button'
import UploadPDF from '@/components/UploadPDF'
import UploadVedio from '@/components/UploadVideo'
export default {
  name: 'AddExpertData',
  components: {
    XButton,
    'upload-pdf': UploadPDF,
    'upload-vedio': UploadVedio
  },
  data () {
    return {
      loading: false,
      isShow: this.$route.params.mark,
      editExpertID: this.$route.params.id,
      filevedioIDs: '',
      filevedioIDsEdit: '',
      filevedioType: FileType.ExpertData_vedio,
      fileIDs: '',
      fileattachIDsEdit: [], // '',
      fileattachType: FileType.ExpertData_attach,
      systemResource: systemResource.expert,
      attch_file: '',
      nodeType: '',
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
      deptList: [],
      deviceType: {
        id: '',
        tips: ''
      },
      team: '',
      teamPath: {
        text: [],
        tips: ''
      },
      teamList: [],
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      deviceTypeList: [],
      ExpertID: '',
      centerDialogVisible: false,
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
    // // 设备类型列表
    // api.GetDeviceTypeList().then(res => {
    //   this.deviceTypeList = res.data
    // }).catch(err => console.log(err))
    eqpApi.getEqpTypeAll().then(res => {
      this.deviceTypeList = res.data
    }).catch(err => console.log(err))
    // 部门列表
    // api.GetdeptList().then(res => {
    //   this.deptList = res.data
    // }).catch(err => console.log(err))
    // 部门加载
    apiOrg.getOrgAll().then(res => {
      this.teamList = res.data
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
      this.attch_file = ''
      if (this.fileattachIDsEdit.length > 0) {
        for (var i = 0; i < this.fileattachIDsEdit.length; i++) {
          this.attch_file += this.fileattachIDsEdit[i].type + ':' + this.fileattachIDsEdit[i].ids + '$'
        }
      }
      let tbexpertdata = {
        device_type: this.deviceType.id,
        deptID: this.dept.id,
        dept_path: this.teamPath.text.join(','),
        keyword: this.keyword.text,
        title: this.Experttitle.text,
        content: this.content.text,
        video_file: this.filevedioIDsEdit,
        attch_file: this.attch_file,
        origin_file: JSON.stringify(this.fileattachIDsEdit),
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
        this.teamPath.text = this.strToIntArr(_res.dept_path)
        this.nodeType = 2
        // this.fileIDs = data.uploadFiles
        // this.filevedioIDs = _res.video_file
        // this.filevedioIDsEdit = _res.video_file
        console.log(_res.uploadFiles)
        this.fileIDs = _res.uploadFiles
        // this.fileattachIDsEdit = _res.uploadFiles
      }).catch(err => console.log(err))
    },
    strToIntArr (str) {
      let arr = str.split(',')
      let ret = []
      for (let i = 0; i < arr.length; i++) {
        if (arr[i] === 0) {
          continue
        }
        ret.push(parseInt(arr[i]))
      }
      return ret
    },
    // 验证
    validateInput (val) {
      validateInputCommon(val)
    },
    cascader_change_copy (val) {
      var arr = this.teamPath.text
      let id = arr[arr.length - 1]
      this.dept.id = id
      let pmObj = new Promise((resolve, reject) => {
        axios(
          {
            url: `http://127.0.0.1:5801/orgapi/org/node/${id}`,
            method: 'get'
          })
          .then(res => {
            resolve(res.data)
          })
          .catch(function (error) {
            reject(error)
          })
      })
      pmObj.then(data => {
        if (data.code === 0) {
          this.nodeType = data.data.nodeType
        }
      })
    },
    // 班组下拉选中，过滤非班组
    cascader_change (val) {
      this.teamPath.tips = ''
      let selectedTeam = val[val.length - 1]
      let obj = this.getCascaderObj(selectedTeam, this.teamList)
      if (obj.node_type === 2) {
        this.team = val[val.length - 1]
      }
    },
    getCascaderObj (val, opt) {
      for (let i = 0; i < opt.length; ++i) {
        let item = opt[i]
        if (val === item.id) {
          return item
        } else {
          if (item.children) {
            let ret = this.getCascaderObj(val, item.children)
            if (ret) {
              return ret
            }
          }
        }
      }
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
      if (this.teamPath.text.length === 0) {
        this.teamPath.tips = '此项必选'
        return false
      }
      if (this.nodeType !== 2) {
        this.teamPath.tips = '您选的并非是部门，请选择部门'
        return false
      } else {
        this.teamPath.tips = ''
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
      if (!this.validatedeptSelect()) {
        return false
      }
      return true
    },
    getvedioFileID (val) {
      this.filevedioIDsEdit = val
    },
    getattachFileID (val) {
      this.fileattachIDsEdit = val
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
