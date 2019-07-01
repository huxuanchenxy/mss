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
        <router-link :to="{name:'ExpertDatalist'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list" v-show="false">
          <div class="inp-wrap">
            <span class="text">ID</span>
            <div class="inp">
              <el-input v-model="ExpertID" :disabled="isShow === 'edit'" ></el-input>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">关键字<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入关键字" v-model="keyword.text" @keyup.native="validateInput(keyword)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ keyword.tips }}</p>
        </li>
         <li class="list" >
          <div class="inp-wrap">
            <span class="text">标题<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入标题" v-model="Experttitle.text" @keyup.native="validateInput(Experttitle)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ Experttitle.tips }}</p>
        </li>
         <li class="list" >
          <div class="inp-wrap">
            <span class="text">内容<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入内容" v-model="content.text" @keyup.native="validateInput(content)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ content.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">设备类型<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-select v-model="deviceType.id" clearable placeholder="请选择" @change="validatedeviceTypeSelect()">
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
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">所属部门<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-select v-model="dept.id" clearable placeholder="请选择" @change="validatedeptSelect()">
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
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">视频上传<em class="validate-mark">*</em></span>
            <div class="inp">
                <el-upload
                  class="upload-demo"
                  action="https://jsonplaceholder.typicode.com/posts/"
                  :on-preview="handlePreview"
                  :on-remove="handleRemove"
                  :file-list="fileList"
                  list-type="picture">
                  <el-button size="small" type="primary">点击上传</el-button>
                  <div slot="tip" class="el-upload__tip">只能上传pdf文件，且不超过500kb</div>
                </el-upload>
            </div>
          </div>
          <p class="validate-tips">{{ dept.tips }}</p>
        </li>
         <li class="list" >
          <div class="inp-wrap">
            <span class="text">附件上传<em class="validate-mark">*</em></span>
            <div class="inp">
                <el-upload
                    class="upload-demo"
                    action="https://jsonplaceholder.typicode.com/posts/"
                    :on-preview="handlePreview"
                    :on-remove="handleRemove"
                    :file-list="fileList"
                    list-type="picture">
                    <el-button size="small" type="primary">点击上传</el-button>
                    <div slot="tip" class="el-upload__tip">只能上传pdf文件，且不超过500kb</div>
                  </el-upload>
            </div>
          </div>
          <p class="validate-tips">{{ dept.tips }}</p>
        </li>
      </ul>
    </div>
    <div class="btn-enter">
      <x-button class="close">
        <router-link :to="{ name: 'ExpertDatalist' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="enter">保存</x-button>
    </div>
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
      ExpertID: '',
      fileList: [{name: 'food.jpeg', url: 'https://fuss10.elemecdn.com/3/63/4e7f3a15429bfda99bce42a18cdd1jpeg.jpeg?imageMogr2/thumbnail/360x360/format/webp/quality/100'}, {name: 'food2.jpeg', url: 'https://fuss10.elemecdn.com/3/63/4e7f3a15429bfda99bce42a18cdd1jpeg.jpeg?imageMogr2/thumbnail/360x360/format/webp/quality/100'}],
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
      attchfile: {}
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
  methods: {
    // 添加权限组
    enter () {
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
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
        created_Time: '',
        created_By: '1',
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
                  name: 'ExpertDatalist'
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
      let id = this.ExpertID
      api.GetExpertDataById(id).then(res => {
        this.loading = false
        let _res = res.data
        this.ExpertID = _res.id
        this.title.text = _res.title
        this.content.text = _res.text
        this.deviceType.id = _res.deviceTypeid
        this.dept.id = _res.deviceid
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
      if (this.deviceType.text === '') {
        this.deviceType.tips = '此项必选'
        return false
      } else {
        this.deviceType.tips = ''
        return true
      }
    },
    validatedeptSelect () {
      if (this.deptType.text === '') {
        this.deptType.tips = '此项必选'
        return false
      } else {
        this.deptType.tips = ''
        return true
      }
    },
    validateNumber () {
      // validateNumberCommon(this.groupOrder)
    },

    validateAll () {
      // if (!validateInputCommon(this.keyword)) return false
      // if (!validateInputCommon(this.title)) return false
      // if (!validateInputCommon(this.content)) return false
      // if (!this.validateSelect()) return false
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
