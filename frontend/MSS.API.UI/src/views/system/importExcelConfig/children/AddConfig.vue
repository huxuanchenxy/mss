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
        <router-link :to="{name:'SeeImportExcelConfigList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">导入表字段</span>
            <div class="inp">
              <!--:headers="uploadHeaders"-->
              <!--http://10.89.36.154:5801/eqpapi/ImportExcelConfig/GetExcelField-->
              <!--http://localhost:3851/api/v1/ImportExcelConfig/GetExcelField-->
              <el-upload
                :disabled="isShow==='detail'"
                action="http://10.89.36.154:5801/eqpapi/ImportExcelConfig/GetExcelField"
                :headers="uploadHeaders"
                :multiple="false"
                accept=".xls,.xlsx"
                :show-file-list="false"
                :before-upload="beforeUpload"
                :on-success="onSuccess">
                <el-button size="small" type="primary" class="import-btn" :loading="loading">导入</el-button>
              </el-upload>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">匹配内容</span>
            <div class="inp">
              <el-select v-model="matchedClass" placeholder="请选择" @change="classChange" :disabled="isShow === 'detail'">
                <el-option v-for="item in classList" :key="item.key" :value="item.id" :label="item.showName"></el-option>
              </el-select>
            </div>
          </div>
        </li>
        <li class="list" />
      </ul>
    </div>
    <div class="content-wrap">
      <ul class="content-header">
        <li class="list number">Excel导入字段</li>
        <li class="list number">匹配内容字段</li>
        <li class="list number">是否必填</li>
      </ul>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item) in configList" :key="item.key">
              <div class="list-content">
                <div class="number">{{ item.importField }}</div>
                <div class="number">
                  <el-select v-model="item.config" placeholder="请选择匹配内容" :disabled="isShow==='detail'">
                    <el-option v-for="property in propertyList" :key="property.key" :value="property.order" :label="property.name"></el-option>
                  </el-select>
                </div>
                <div class="number">
                  <el-radio v-model="item.required" class="color-white" label="1" :disabled="isShow==='detail'">是</el-radio>
                  <el-radio v-model="item.required" class="color-white" label="0" :disabled="isShow==='detail'">否</el-radio>
                </div>
              </div>
            </li>
          </ul>
          <ul class="btn-enter" v-show="isShow!=='detail'">
            <x-button class="close">
              <router-link :to="{ name: 'SeeImportExcelConfigList' }">取消</router-link>
            </x-button>
            <x-button class="active" @click.native="enter">保存</x-button>
          </ul>
        </el-scrollbar>
      </div>
    </div>
  </div>
</template>
<script>
// import { dictionary } from '@/common/js/dictionary.js'
import XButton from '@/components/button'
import api from '@/api/eqpApi'
export default {
  name: 'AddImportExcelConfig',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      isShow: this.$route.params.mark,
      editID: this.$route.params.id,
      titel: '',
      fileName: '',
      uploadHeaders: {Authorization: ''},
      configList: [],
      matchedClass: '',
      classList: [],
      propertyList: []
    }
  },
  mounted () {
    let token = window.sessionStorage.getItem('token')
    if (token) { // 判断是否存在token，如果存在的话，则每个http header都加上token
      this.uploadHeaders.Authorization = `Bearer ${token}`
    }
  },
  created () {
    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 添加Excel导入配置'
      // this.$emit('title', '| 添加权限组')
      // this.btnText = '确认'
    } else {
      this.loading = true
      // this.$emit('title', '| 修改权限组')
      // this.btnText = '保存'
      this.getConfig()
      if (this.isShow === 'edit') this.title = '| 修改Excel导入配置'
    }
    // 导入数据库列表
    api.getImportExcelClass().then(res => {
      this.classList = res.data
    }).catch(err => console.log(err))
  },
  methods: {
    // 添加权限组
    enter () {
      let field = ''
      let config = ''
      let required = ''
      for (let i = 0; i < this.configList.length; i++) {
        let item = this.configList[i]
        if (item.config === undefined) {
          this.$message({
            message: '匹配内容字段必选',
            type: 'warning'
          })
          return
        }
        field += item.importField + ','
        config += item.config + ','
        required += item.required + ','
      }
      console.log(this.configList)
      let importExcelConfig = {
        field: field.slice(0, field.length - 1),
        config: config.slice(0, config.length - 1),
        required: required.slice(0, required.length - 1),
        classID: this.matchedClass,
        fileName: this.fileName
      }
      if (this.isShow === 'add') {
        // 新增配置
        api.addImportExcelConfig(importExcelConfig).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeImportExcelConfigList'
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
        importExcelConfig.id = this.editID
        // 修改配置
        api.updateImportExcelConfig(importExcelConfig).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeImportExcelConfigList'
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
    // 修改配置时获取配置信息
    getConfig () {
      api.getImportExcelConfigByID(this.editID).then(res => {
        this.loading = false
        let _res = res.data
        this.matchedClass = _res.classID
        this.fileName = _res.fileName
        if (this.isShow === 'detail') this.title = '| ' + this.fileName + '文件配置明细'
        this.classChange(this.matchedClass)
        let field = _res.field.split(',')
        let config = _res.config.split(',')
        let required = _res.required.split(',')
        this.configList = []
        for (let i = 0; i < field.length; i++) {
          this.configList.push({
            importField: field[i],
            config: parseInt(config[i]),
            required: required[i].toString()
          })
        }
      }).catch(err => console.log(err))
    },
    beforeUpload (file) {
      if (file.size > 52428800) {
        this.$message({
          message: '单个文件不允许超过50M',
          type: 'warning'
        })
        return false
      } else {
        let arr = file.name.split('.')
        let extTmp = arr[arr.length - 1]
        if (extTmp !== 'xls' && extTmp !== 'xlsx') {
          this.$message({
            message: '不支持非excel的文件类型',
            type: 'warning'
          })
          return false
        }
        this.fileName = file.name.slice(0, file.name.length - extTmp.length - 1)
      }
    },
    onSuccess (response, file, fileList) {
      if (response.code === 0) {
        this.configList = []
        response.data.map(item => {
          let tmp = {
            importField: item,
            // config: -1,
            required: '0'
          }
          this.configList.push(tmp)
        })
      } else {
        this.$message({
          message: response.msg,
          type: 'error'
        })
      }
    },
    classChange (val) {
      api.getPropertyByClass(val).then(res => {
        this.propertyList = [{order: -1, name: '无匹配'}]
        res.data.map(item => {
          if (item.visible) this.propertyList.push(item)
        })
      }).catch(err => console.log(err))
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
    margin-bottom: 10px;

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
$con-height: $content-height - 145 - 186;
// 内容区
.content-wrap{
  overflow: hidden;
  height: percent($con-height, $content-height);
  text-align: center;
  .content-header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: percent(50, $con-height);
    padding: 0 PXtoEm(24);
    background: rgba(36,128,198,.5);

    .last-update-time{
      color: $color-white;
    }
  }

  .scroll{
    height: percent($con-height - 50, $con-height);
  }

  .list-wrap{
    .list{
      &:nth-of-type(even){
        .list-content{
          background: rgba(186,186,186,.5);
        }
      }
    }

    .list-content{
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: PXtoEm(15) PXtoEm(24);

      div{
        word-break: break-all;
      }
    }

    .left-title{
      margin-right: 10px;
      font-weight: bold;
    }

    // 隐藏内容
    .sub-content{
      overflow: hidden;
      height: 0;
      font-size: $font-size-small;
      text-align: left;
      color: $color-content-text;

      &.active{
        overflow: inherit;
        height: auto;
        transition: .7s .2s;
      }
    }

    .sub-con-list{
      display: flex;
      padding: PXtoEm(15) PXtoEm(24);
      border-top: 1px solid $color-main-background;
      background: rgba(0,0,0,.2);

      .right-wrap{
        display: flex;
        flex-wrap: wrap;
      }

      .list{
        margin-right: 10px;
      }
    }
  }

  .number,
  .name,
  .btn-wrap{
    width: 20%;
  }

  .name{
    a{
      color: #42abfd;
    }
  }

  .last-update-time{
    width: 18%;
    color: $color-content-text;
  }

  .last-maintainer{
    width: 10%;
  }

  .state{
    width: 5%;
  }
}
.import-btn{
  border-color: $color-main-btn!important;
  background: $color-main-btn!important;
  &:hover{
    border-color: $color-main-btn!important;
    background: $color-main-btn!important;
  }
}
</style>
