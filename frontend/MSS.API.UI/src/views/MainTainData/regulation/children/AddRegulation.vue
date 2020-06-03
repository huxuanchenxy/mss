<template>
  <div class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div ref="header" class="header con-padding-horizontal">
      <h2>
        <img :src="$router.navList[$route.matched[0].path].iconClsActive" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].name }} {{ title }}
      </h2>
      <x-button class="active"><router-link :to="{ name: 'SeeRegulationList' }">返回</router-link></x-button>
    </div>
    <div class="scroll">
      <el-scrollbar>
        <!-- 列表 -->
        <ul class="con-padding-horizontal input-group">
          <li class="list">
            <div class="inp-wrap">
              <span class="text">规章制度<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input placeholder="请输入规章制度名称" v-model="scene.text" @keyup.native="validateInput(scene)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ scene.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">关键词<em class="validate-mark">*</em></span>
              <div class="inp">
                <el-input v-model="keyword.text" placeholder="请输入关键词" @keyup.native="validateInput(keyword)"></el-input>
              </div>
            </div>
            <p class="validate-tips">{{ keyword.tips }}</p>
          </li>
          <li class="list">
            <div class="inp-wrap">
              <span class="text">上传部门</span>
              <div class="inp">
                <el-cascader class="cascader_width" clearable ref='dept'
                  expand-trigger="hover"
                  change-on-select
                  :props="defaultParams"
                  @change="cascader_change"
                  :show-all-levels="true"
                  :options="deptList"
                  v-model="deptPath.text">
                </el-cascader>
              </div>
            </div>
            <p class="validate-tips">{{ deptPath.tips }}</p>
          </li>
          <div class="upload-list">
            <upload-pdf :systemResource="systemResource" :fileIDs="fileIDs" @getFileIDs="getFileIDs"></upload-pdf>
          </div>
        </ul>
        <!-- 按钮 -->
        <div class="btn-group">
          <x-button class="close">
            <router-link :to="{name: 'SeeRegulationList'}">取消</router-link>
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
import api from '@/api/ExpertApi.js'
import apiOrg from '@/api/orgApi'
import MyUploadPDF from '@/components/UploadPDF'
export default {
  name: 'AddEmergency',
  components: {
    XButton,
    'upload-pdf': MyUploadPDF
  },
  data () {
    return {
      defaultParams: {
        label: 'label',
        value: 'id',
        children: 'children'
      },
      systemResource: systemResource.regulation,
      loading: false,
      title: '| 添加规章制度',
      ePlanID: '',
      scene: {text: '', tips: ''},
      keyword: {text: '', tips: ''},
      dept: '',
      deptList: [],
      deptPath: {text: [], tips: ''},
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
        this.title = '| 修改规章制度'
        this.loading = true
        // 部门加载
        apiOrg.getOrgAll().then(res => {
          this.deptList = res.data
          this.getEPlan()
        }).catch(err => console.log(err))
      } else {
        // 部门加载
        apiOrg.getOrgAll().then(res => {
          this.deptList = res.data
        }).catch(err => console.log(err))
      }
    },
    getFileIDs (ids) {
      this.fileIDsEdit = ids
      if (this.$route.query.type !== 'Add') {
        this.isEditFile = true
      }
    },
    cascader_change (val) {
      let selectedDept = val[val.length - 1]
      let obj = this.getCascaderObj(selectedDept, this.deptList)
      if (obj.node_type === 2) {
        this.dept = selectedDept
        this.deptPath.tips = ''
      } else {
        this.deptPath.tips = '您选择的不是部门'
      }
      // let el = document.querySelector('.pop-team')
      // el.style.display = 'none'
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
    getEPlan () {
      api.getEPlanByID(this.$route.query.id).then(res => {
        if (res.code === 0) {
          let data = res.data
          this.ePlanID = data.id
          this.scene.text = data.scene
          this.keyword.text = nullToEmpty(data.keyword)
          this.deptPath.text = this.strToIntArr(data.deptPath)
          this.fileIDs = data.uploadFiles
        }
        this.loading = false
      }).catch(err => console.log(err))
    },
    strToIntArr (str) {
      let arr = str.split(',')
      let ret = []
      for (let i = 0; i < arr.length; i++) {
        ret.push(parseInt(arr[i]))
      }
      return ret
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
    validateInput (val) {
      if (!validateInputCommon(val)) {
        return false
      }
      return true
    },
    validateInputAll () {
      if (!this.validateInput(this.scene) || !this.validateInput(this.keyword)) {
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
      let ePlan = {
        Scene: this.scene.text,
        Dept: this.dept,
        DeptPath: this.deptPath.text.join(','),
        Keyword: this.keyword.text,
        Type: this.systemResource,
        UploadFiles: this.fileIDsEdit.length === 0 && !this.isEditFile ? '' : JSON.stringify(this.fileIDsEdit)
      }
      if (this.$route.query.type === 'Add') {
        api.addEPlan(ePlan).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeRegulationList'})
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
        ePlan.ID = this.ePlanID
        api.updateEPlan(ePlan).then(res => {
          if (res.code === 0) {
            this.$router.push({name: 'SeeRegulationList'})
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

.cascader_width{
  width: 100%!important;
}
</style>
