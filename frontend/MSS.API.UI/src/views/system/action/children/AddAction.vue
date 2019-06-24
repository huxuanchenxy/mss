<template>
  <div
    class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal header">
      <h2 class="title">
        <img :src="$router.navList[$route.matched[0].path].TitleIcon" alt="" class="icon"> {{ $router.navList[$route.matched[0].path].GroupName }} {{ title }}
      </h2>
      <x-button class="active">
        <router-link :to="{name:'SeeActionList'}">返回</router-link>
      </x-button>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list" v-show="false">
          <div class="inp-wrap">
            <span class="text">权限ID</span>
            <div class="inp">
              <el-input v-model="actionID" :disabled="isShow === 'edit'"></el-input>
            </div>
          </div>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">权限名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model="actionName.text" placeholder="请输入权限名称" @keyup.native="validateInput(actionName)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ actionName.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">权限URL<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model="actionUrl.text" placeholder="请输入权限URL" @keyup.native="validateInput(actionUrl)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ actionUrl.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">权限组<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-select v-model="actionGroup.text" placeholder="请选择" @change="validateSelect(actionGroup)">
                <el-option v-for="item in actionGroupList" :key="item.key" :value="item.ActionGroupID" :label="item.GroupName"></el-option>
              </el-select>
            </div>
          </div>
          <p class="validate-tips">{{ actionGroup.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">权限顺序</span>
            <div class="inp">
              <el-input v-model="menuOrder.text" placeholder="请输入权限顺序" @keyup.native="validateNumber()"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ menuOrder.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">是否是菜单<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-select v-model="isMenu.text" placeholder="请选择" @change="validateSelect(isMenu)">
                <el-option v-for="item in isMenuList" :key="item.key" :value="item.value" :label="item.label"></el-option>
              </el-select>
            </div>
          </div>
          <p class="validate-tips">{{ isMenu.tips }}</p>
        </li>
        <li class="list"></li>
      </ul>
    </div>
    <div class="btn-group">
      <x-button class="close">
        <router-link :to="{ name: 'SeeActionList' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="enter">保存</x-button>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, validateNumberCommon } from '@/common/js/utils.js'
import XButton from '@/components/button'
export default {
  name: 'AddActionGroup',
  components: {
    XButton
  },
  data () {
    return {
      title: '',
      loading: true,
      isShow: this.$route.params.mark,
      editActionID: this.$route.params.id,
      btnText: '',
      actionID: '',
      actionName: {
        text: '',
        tips: ''
      },
      actionUrl: {
        text: '',
        tips: ''
      },
      actionGroup: {
        text: '',
        tips: ''
      },
      actionGroupList: [],
      menuOrder: {
        text: '',
        tips: ''
      },
      isMenu: {
        text: '',
        tips: ''
      },
      isMenuList: [
        {
          value: 0,
          label: '否'
        },
        {
          value: 1,
          label: '是'
        }
      ]
    }
  },
  created () {
    if (this.isShow === 'add') {
      this.loading = false
      this.title = '| 添加权限'
      // this.$emit('title', '| 添加权限')
      this.btnText = '确认'
    } else if (this.isShow === 'edit') {
      this.loading = true
      this.title = '| 修改权限'
      // this.$emit('title', '| 修改权限')
      this.btnText = '保存'
      this.getAction()
    }
    // 权限组列表
    window.axios.post('/ActionGroup/GetAll').then(res => {
      this.actionGroupList = res.data
    }).catch(err => console.log(err))
  },
  methods: {
    // 添加权限
    enter () {
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      if (this.isShow === 'add') {
        // 添加权限
        window.axios.post('/ActionInfo/AddActionInfo', {
          ActionName: this.actionName.text,
          ActionGroupID: this.actionGroup.text,
          MenuOrder: this.menuOrder.text,
          RequestUrl: this.actionUrl.text,
          IsMenu: this.isMenu.text
        }).then(res => {
          if (res.data === 'OK') {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeActionList'
                })
              }
            })
          } else if (res.data === 'Fail') {
            this.$message({
              message: '添加失败',
              type: 'error'
            })
          } else {
            this.$message({
              message: res.data,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      } else if (this.isShow === 'edit') {
        // 修改权限
        window.axios.post('/ActionInfo/UpdateActionInfo', {
          ActionID: this.actionID,
          ActionName: this.actionName.text,
          ActionGroupID: this.actionGroup.text,
          MenuOrder: this.menuOrder.text,
          RequestUrl: this.actionUrl.text,
          IsMenu: this.isMenu.text
        }).then(res => {
          if (res.data === 'OK') {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeActionList'
                })
              }
            })
          } else if (res.data === 'Fail') {
            this.$message({
              message: '修改失败',
              type: 'error',
              onClose: () => {
                this.$router.push({
                  name: 'SeeActionList'
                })
              }
            })
          } else {
            this.$message({
              message: res.data,
              type: 'error'
            })
          }
        }).catch(err => console.log(err))
      }
    },
    // 修改权限时获取权限资料
    getAction () {
      window.axios.post('/ActionInfo/BindActionInfo', {
        ID: this.editActionID
      }).then(res => {
        this.loading = false
        let _res = res.data
        this.actionID = _res.ActionID
        this.actionName.text = _res.ActionName
        this.actionGroup.text = _res.ActionGroupID
        this.menuOrder.text = _res.MenuOrder.toString()
        this.actionUrl.text = _res.RequestUrl
        this.isMenu.text = _res.IsMenu
      }).catch(err => console.log(err))
    },
    // 验证
    validateInput (val) {
      validateInputCommon(val)
    },

    validateSelect (item) {
      if (item.text === '') {
        item.tips = '此项必选'
        return false
      } else {
        item.tips = ''
        return true
      }
    },

    validateNumber () {
      validateNumberCommon(this.menuOrder)
    },

    validateAll () {
      if (!validateInputCommon(this.actionName)) return false
      if (!validateInputCommon(this.actionUrl)) return false
      if (!validateNumberCommon(this.menuOrder)) return false
      if (!this.validateSelect(this.actionGroup)) return false
      if (!this.validateSelect(this.isMenu)) return false
      return true
    }
  }
}
</script>
<style lang="scss" scoped>
.header{
  display: flex;
  justify-content: space-between;
}
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
.btn-group{
  margin: 15px 0;
  text-align: center;

  button{
    border-color: $color-main-btn;
    background: $color-main-btn;
  }
}
</style>
