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
        <router-link :to="{name:'SmallAreaList'}">返回</router-link>
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
            <span class="text"> 位置名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入位置名称" v-model="areaName.text" @keyup.native="validateInput(areaName)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ areaName.tips }}</p>
        </li>
         
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">车站<em class="validate-mark">*</em></span>
            <div class="inp"> 
              <el-select v-model="AreaType.id" clearable placeholder="请选择" @change="validateSelect()"> 
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
      </ul>
    </div>
    <div class="btn-enter">
      <x-button class="close">
        <router-link :to="{name:'SmallAreaList' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="enter">保存</x-button>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, validateNumberCommon, vInput } from '@/common/js/utils.js'
import api from '@/api/AreaApi.js'
import XButton from '@/components/button' 

export default {
  name: 'AddSmallArea',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      isShow: 'add',//this.$route.params.mark,
      editAreaID: this.$route.params.id,
      AreaID: '',
      areaName: {
        text: '',
        tips: ''
      }, 
      AreaType: {
        id: '',
        tips: ''
      }, 
      AreaTypeList: [] 
    }
  },
  created () {
    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 添加位置' 
    } else if (this.isShow === 'edit') {
      this.loading = true
      this.title = '| 修改位置' 
      alert('edit')
      this.getMidArea()
    }  
     api.GetChezhanData().then(res => {
      this.AreaTypeList=res.data  
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
    
    let TB_Config_MidArea ={  
                            areaName:this.areaName.text,
                            pid:this.AreaType.id,
                            sort:'1',
                            is_Used :'1',
                            is_Deleted:'0',
                            created_Time :'',
                            created_By:'101', 
                            remark:this.areaName.text 
                          }
      if (this.isShow === 'add') {
        // 添加权限组
        
      api.SaveConfigMidArea(TB_Config_MidArea).then(res => { 
          if (res.ret === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SmallAreaList'
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
        TB_Config_MidArea.id = this.AreaID
        // 修改权限组
         api.UpdateMidArea(TB_Config_MidArea).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SmallArealist'
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
    
    getMidArea () {
      alert('11212')
      let id=this.editAreaID
     api.GetConfigMidAreaId(id).then(res => {
        this.loading = false
         let _res = res.content
        this.AreaID = _res.id
        this.areaName.text = _res.areaName
        //this.AreaType.id=_res.pid
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
      //validateNumberCommon(this.groupOrder)
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
