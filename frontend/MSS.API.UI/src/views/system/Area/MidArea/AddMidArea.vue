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
        <router-link :to="{name:'MidAreaList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list" v-show="false">
          <div class="inp-wrap">
            <span class="text">ID</span>
            <div class="inp">
              <el-input v-model="AreaID" :disabled="isShow === 'edit'" ></el-input>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">线路<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-select v-model="LineID" clearable placeholder="请选择" @change="validateSelect()">
                 <el-option
                 v-for="item in MetroLineList"
                 :key="item.key"
                 :value="item.id"
                 :label="item.lineName">
                 </el-option>
              </el-select>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">类型<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-select v-model="AreaType.text" clearable placeholder="请选择" @change="validateSelect()">
                 <el-option
                 v-for="item in AreaTypeList"
                 :key="item.key"
                 :value="item.id"
                 :label="item.areaName">
                 </el-option>
              </el-select>
            </div>
          </div>
          <p class="validate-tips">{{ AreaType.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">站区名<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入站区名名称" v-model="areaName.text" @keyup.native="validateInput(areaName)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ areaName.tips }}</p>
        </li>
      </ul>
    </div>
    <div class="btn-enter">
      <x-button class="close">
        <router-link :to="{ name: 'MidAreaList' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="enter">保存</x-button>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vInput } from '@/common/js/utils.js'
import api from '@/api/AreaApi'
import lineApi from '@/api/metroLineApi'
import XButton from '@/components/button'
export default {
  name: 'AddMidArea',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      isShow: this.$route.params.mark,
      editAreaID: this.$route.params.id,
      AreaID: '',
      areaName: {
        text: '',
        tips: ''
      },
      AreaType: {
        id: '',
        text: '',
        tips: ''
      },
      AreaTypeList: [], // 场区类型: 车站\正线轨行区\保护区\车场生产区
      MetroLineList: [],
      LineID: ''
    }
  },
  created () {
    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 添加站区'
    } else if (this.isShow === 'edit') {
      this.loading = true
      this.title = '| 修改站区'
    }
    api.SelectDicAreaData('1').then(res => {
      this.AreaTypeList = res.data
    }).catch(err => {
      console.log(err)
    })
    lineApi.getAll('1').then(res => {
      this.MetroLineList = res.data
      if (this.isShow === 'edit') {
        this.getMidArea()
      } else {
        this.LineID = res.data[0]
      }
    }).catch(err => {
      console.log(err)
    })
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
      let tbConfigBigArea = {
        MetroLineID: this.LineID,
        areaName: this.areaName.text,
        configType: this.AreaType.text,
        sort: '1',
        is_Used: '1',
        is_Deleted: '0',
        created_Time: '',
        created_By: '101',
        remark: this.areaName.text
      }
      if (this.isShow === 'add') {
        // 添加站区
        api.SaveConfigBigArea(tbConfigBigArea).then(res => {
          if (res.ret === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'MidAreaList'
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
        tbConfigBigArea.id = this.AreaID
        // 修改权限组
        api.UpdateConfigBigArea(tbConfigBigArea).then(res => {
          if (res.ret === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'MidAreaList'
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
    getMidArea () {
      let id = this.editAreaID
      api.GetConfigBigAreaId(id).then(res => {
        this.loading = false
        let _res = res.data
        this.AreaID = _res.id
        this.LineID = _res.metroLineID
        this.areaName.text = _res.areaName
        this.AreaType.text = _res.configType
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
    validateSelect () {
      if (this.AreaType.text === '') {
        this.AreaType.tips = '此项必选'
        return false
      } else {
        this.AreaType.tips = ''
        return true
      }
    },

    validateNumber () {
      // validateNumberCommon(this.groupOrder)
    },

    validateAll () {
      if (!validateInputCommon(this.areaName)) return false
      if (!this.validateSelect()) return false
      return true
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
