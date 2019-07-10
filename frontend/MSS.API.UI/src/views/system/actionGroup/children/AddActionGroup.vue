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
        <router-link :to="{name:'SeeActionGroupList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list" v-show="false">
          <div class="inp-wrap">
            <span class="text">权限组ID</span>
            <div class="inp">
              <el-input v-model="actionGroupID" :disabled="isShow === 'edit'" ></el-input>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">权限组名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入权限组别名称" v-model="groupName.text" @keyup.native="validateInput(groupName)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ groupName.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">权限组URL<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入权限组权限组URL" v-model="groupUrl.text" @keyup.native="validateInput(groupUrl)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ groupUrl.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">权限组类型<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-select v-model="groupType.text" clearable placeholder="请选择" @change="validateSelect()">
                <el-option value="" label="请选择"></el-option>
                <el-option v-for="item in groupTypeList" :key="item.key" :value="item.sub_code" :label="item.sub_code_name"></el-option>
              </el-select>
            </div>
          </div>
          <p class="validate-tips">{{ groupType.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">权限组顺序</span>
            <div class="inp">
              <el-input placeholder="请输入权限组顺序" v-model="groupOrder.text" @keyup.native="validateNumber()"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ groupOrder.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">权限组图标</span>
            <div class="inp">
              <el-input placeholder="请输入权限组图标" v-model="groupIcon.text" @keyup.native="validateInputNull(groupIcon)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ groupIcon.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">标题图标</span>
            <div class="inp">
              <el-input placeholder="请输入标题图标" v-model="titleIcon.text" @keyup.native="validateInputNull(titleIcon)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ titleIcon.tips }}</p>
        </li>
      </ul>
    </div>
    <div class="btn-enter">
      <x-button class="close">
        <router-link :to="{ name: 'SeeActionGroupList' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="enter">保存</x-button>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, validateNumberCommon, vInput } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/authApi'
export default {
  name: 'AddActionGroup',
  components: {
    XButton
  },
  data () {
    return {
      loading: false,
      isShow: this.$route.params.mark,
      editActionGroupID: this.$route.params.id,
      actionGroupID: '',
      groupName: {
        text: '',
        tips: ''
      },
      groupUrl: {
        text: '',
        tips: ''
      },
      groupType: {
        text: '',
        tips: ''
      },
      groupTypeList: [],
      groupOrder: {
        text: '',
        tips: ''
      },
      groupIcon: {
        text: '',
        tips: ''
      },
      titleIcon: {
        text: '',
        tips: ''
      }
    }
  },
  created () {
    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 添加权限组'
      // this.$emit('title', '| 添加权限组')
      // this.btnText = '确认'
    } else if (this.isShow === 'edit') {
      this.loading = true
      this.title = '| 修改权限组'
      // this.$emit('title', '| 修改权限组')
      // this.btnText = '保存'
      this.getActionGroup()
    }
    // 权限组类型列表
    api.getSubCode('group_type').then(res => {
      this.groupTypeList = res.data
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
      let actionGroup = {
        group_name: this.groupName.text,
        group_type: this.groupType.text,
        group_order: this.groupOrder.text,
        request_url: this.groupUrl.text,
        icon: this.groupIcon.text,
        active_icon: this.titleIcon.text
      }
      if (this.isShow === 'add') {
        // 添加权限组
        api.addActionGroup(actionGroup).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeActionGroupList'
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
        actionGroup.id = this.actionGroupID
        // 修改权限组
        api.updateActionGroup(actionGroup).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeActionGroupList'
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
    getActionGroup () {
      api.getActionGroupByID(this.editActionGroupID).then(res => {
        this.loading = false
        let _res = res.data
        this.actionGroupID = _res.id
        this.groupName.text = _res.group_name
        this.groupType.text = _res.group_type
        this.groupOrder.text = _res.group_order.toString()
        this.groupUrl.text = _res.request_url
        this.groupIcon.text = _res.icon
        this.titleIcon.text = _res.active_icon
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
      if (this.groupType.text === '') {
        this.groupType.tips = '此项必选'
        return false
      } else {
        this.groupType.tips = ''
        return true
      }
    },

    validateNumber () {
      validateNumberCommon(this.groupOrder)
    },

    validateAll () {
      if (!validateInputCommon(this.groupName)) return false
      if (!validateInputCommon(this.groupUrl)) return false
      if (!validateNumberCommon(this.groupOrder)) return false
      if (!this.validateInputNull(this.groupIcon)) return false
      if (!this.validateInputNull(this.titleIcon)) return false
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
