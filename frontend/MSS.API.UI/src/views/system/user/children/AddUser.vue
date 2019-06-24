<template>
  <div class="wrap" :class="isShow">
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">用户编号<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入用户编号" v-model="userID.text" :disabled="isShow === 'edit'" @keyup.native="validateInput(userID)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ userID.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">用户名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入用户名称" v-model="userName.text" @keyup.native="validateInput(userName)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ userName.tips }}</p>
        </li>
        <li class="list" v-if="isShow === 'add'">
          <div class="inp-wrap">
            <span class="text">登录密码<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请输入登录密码" v-model="userPassword.text" type="password" @keyup.native="validatePwd"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ userPassword.tips }}</p>
        </li>
        <li class="list" v-if="isShow === 'add'">
          <div class="inp-wrap">
            <span class="text">确认密码<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input placeholder="请再次输入密码" v-model="userPasswordSure.text" type="password" @keyup.native="validatePwdSure"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ userPasswordSure.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">手机号码</span>
            <div class="inp">
              <el-input placeholder="请输入11位手机号码" v-model="userPhone.text" @keyup.native="validatePhone(userPhone)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ userPhone.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">电子邮箱</span>
            <div class="inp">
              <el-input placeholder="请输入电子邮箱" v-model="userEmail.text" @keyup.native="validateEmail(userEmail)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ userEmail.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">参考角色</span>
            <div class="inp">
              <el-select v-model="relativeRole" placeholder="请选择" @change="getRoleAuthority">
                <el-option value="" label="请选择"></el-option>
                <el-option v-for="item in role" :key="item.key" :value="item.RoleID" :label="item.RoleName"></el-option>
              </el-select>
            </div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">所属部门<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-cascader
                :options="department"
                v-model="departmentList"
                change-on-select>
              </el-cascader>
            </div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">所属管廊<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-select v-model="tunnels" collapse-tags size="small" multiple placeholder="请选择">
                <el-option v-for="item in tunnelList" :key="item.key" :value="item.TunnelID" :label="item.TunnelName"></el-option>
              </el-select>
            </div>
          </div>
        </li>
        <!-- 补位 -->
        <li class="list" v-if="isShow === 'edit'"></li>
      </ul>
    </div>
    <div class="content">
      <div class="con-padding-horizontal header">
        菜单名称<span @click="checkAll"><x-button class="small">全选</x-button></span>
      </div>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item, index) in authority" :key="item.key">
              <div class="list-con" @click="shrinkChildren(index)">
                <div class="left">
                  {{ item.text }} <i class="el-icon-arrow-down" :class="{ active: item.shrink }"></i>
                </div>
                <el-checkbox v-model="item.checked" @change="listCheckAll($event, item)"></el-checkbox>
              </div>
              <div class="list-sub-con" v-for="(children, j) in item.children" :key="children.key" v-show="item.shrink">
                <el-checkbox-group v-model="userActionInfo">
                  <el-checkbox :label="children.id" @change="listChange(index, j, children.id)">{{ children.text }}</el-checkbox>
                </el-checkbox-group>
              </div>
            </li>
          </ul>
          <div class="btn-group">
            <x-button class="close" @click.native="$emit('title', '')">
              <router-link :to="{ name: 'SeeUserList' }">取消</router-link>
            </x-button>
            <x-button class="active" @click.native="enter">保存</x-button>
          </div>
        </el-scrollbar>
      </div>
    </div>
  </div>
</template>
<script>
import { vPhone, vEmail, validateInputCommon } from '@/common/js/utils.js'
import XButton from '@/components/button'
export default {
  name: 'AddUser',
  components: {
    XButton
  },
  data () {
    return {
      relativeRole: '',
      userName: {
        text: '',
        tips: ''
      },
      userID: {
        text: '',
        tips: ''
      },
      userPassword: {
        text: '',
        tips: ''
      },
      userPasswordSure: {
        text: '',
        tips: ''
      },
      userPhone: {
        text: '',
        tips: ''
      },
      userEmail: {
        text: '',
        tips: ''
      },
      editUserID: '',
      role: [],
      userActionInfo: [],
      authority: [],
      authorityIdList: [],
      bCheckAll: false,
      // 级联选择测试数据
      department: [],
      departmentList: [],
      // 管廊测试数据
      tunnelList: [],
      tunnels: [],
      isShow: ''
    }
  },
  created () {
    this.isShow = this.$route.query.t
    if (this.$route.query.t === 'edit') {
      this.$emit('title', '| 修改用户')
      this.editUserID = this.$route.query.id[0]
      window.axios.post('/UserInfo/BindUserInfo', {
        UserID: this.editUserID
      }).then(res => {
        let _res = res.data.userInfo
        this.userID.text = _res.UserID
        this.userName.text = _res.UserName
        this.userPhone.text = _res.Mobile
        this.userEmail.text = _res.Email
        this.departmentList = res.data.department
        this.tunnels = res.data.tunnels
        this.userActionInfo = res.data.actions.split(',')
      }).catch(err => console.log(err))
    } else {
      this.$emit('title', '| 添加用户')
    }
  },
  methods: {
    // 添加用户
    enter () {
      if (!this.validateAll()) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      } else if (this.departmentList.length === 0) {
        this.$message({
          message: '所属部门必选',
          type: 'error'
        })
        return
      } else if (this.tunnels.length === 0) {
        this.$message({
          message: '所属管廊必选',
          type: 'error'
        })
        return
      }
      let postData = {
        UserID: this.userID.text,
        UserName: this.userName.text,
        Password: this.userPassword.text,
        Mobile: this.userPhone.text,
        Email: this.userEmail.text,
        OrgTreeID: this.departmentList[this.departmentList.length - 1],
        TunnelIDs: this.tunnels.join(','),
        ActionInfos: this.userActionInfo.join(',')
      }
      if (this.isShow === 'add') {
        // 添加用户
        window.axios.post('/UserInfo/AddUserInfo', postData).then(res => {
          if (res.data === 'OK') {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeUserList'
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
        // 修改用户
        window.axios.post('/UserInfo/UpdateUserInfo', postData).then(res => {
          if (res.data === 'OK') {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$emit('showChildrenEvent', 'userList')
              }
            })
          } else if (res.data === 'Fail') {
            this.$message({
              message: '修改失败',
              type: 'error'
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

    // 收起、展开
    shrinkChildren (index) {
      this.authority[index].shrink = !this.authority[index].shrink
    },

    // 根据角色获取对应的权限列表
    getRoleAuthority () {
      if (this.relativeRole === '') {
        this.userActionInfo = []
      } else {
        window.axios.post('/Role/BindRole', {
          RoleID: this.relativeRole
        }).then(res => (this.userActionInfo = res.data.actions.split(','))).catch(err => console.log(err))
      }
    },

    // 全选
    checkAll () {
      if (this.bCheckAll) {
        this.authority.map(item => {
          item.checked = false
          item.newList = []
        })
        this.userActionInfo = []
      } else {
        // 每次赋值前先清空数组
        this.userActionInfo = []
        this.authority.map(item => {
          item.checked = true
          item.children.map(child => {
            child.flag = false
            this.userActionInfo.push(child.id)
          })
          item.newList = item.subids.split(',')
        })
      }
      this.bCheckAll = !this.bCheckAll
    },

    // 列表全选功能
    listCheckAll (...arr) {
      let [flag, _arr] = arr
      let aIdList = _arr.subids.split(',')
      if (flag) {
        this.userActionInfo = [...new Set([...this.userActionInfo, ...aIdList])]
        _arr.newList = aIdList
      } else {
        _arr.newList = []
        aIdList.forEach((item, i) => {
          this.userActionInfo.forEach((val, index) => {
            item === val && this.userActionInfo.splice(index, 1)
          })
        })
      }
    },

    listChange (...arr) {
      let [index, j, val] = arr
      if (this.authority[index].children[j].flag) {
        this.authority[index].newList = [...new Set([...this.authority[index].newList, ...[val]])]
        if (this.authority[index].newList.length === this.authority[index].children.length) {
          this.authority[index].checked = true
        }
      } else {
        this.authority[index].checked = false
        this.authority[index].newList.forEach((_item, _j) => {
          val === _item && this.authority[index].newList.splice(_j, 1)
        })
        this.userActionInfo.forEach((_item, _j) => {
          val === _item && this.userActionInfo.splice(_j, 1)
        })
      }
      this.authority[index].children[j].flag = !this.authority[index].children[j].flag
    },

    // 验证
    validateInput (val) {
      validateInputCommon(val)
    },
    validatePhone (val) {
      if (val.text === '' || val.text === null || vPhone(val.text)) {
        val.tips = ''
        return true
      } else {
        val.tips = '11位手机号码格式不正确'
        return false
      }
    },
    validateEmail (val) {
      if (val.text === '' || val.text === null || vEmail(val.text)) {
        val.tips = ''
        return true
      } else {
        val.tips = 'Email格式不正确'
        return false
      }
    },
    validatePwd () {
      if (this.userPassword.text === '') {
        this.userPassword.tips = '密码必填'
        return false
      } else {
        this.userPassword.tips = ''
        return true
      }
    },
    validatePwdSure () {
      if (this.userPasswordSure.text === this.userPassword.text) {
        this.userPasswordSure.tips = ''
        return true
      } else {
        this.userPasswordSure.tips = '密码不一致'
        return false
      }
    },
    validateAll () {
      if (!validateInputCommon(this.userID)) return false
      if (!validateInputCommon(this.userName)) return false
      if (this.isShow === 'add') {
        if (!this.validatePwd()) return false
        if (!this.validatePwdSure()) return false
      }
      if (!this.validatePhone(this.userPhone)) return false
      if (!this.validateEmail(this.userEmail)) return false
      return true
    }
  },
  mounted () {
    // 获取菜单列表
    window.axios.post('/ActionInfo/GetAuthority').then(res => {
      res.data.Authority.map(item => {
        item.shrink = false
        item.checked = item.subids.split(',').length === item.children.length && this.$route.query.t === 'edit'
        item.newList = []
        item.children.map(child => (child.flag = true))
      })
      this.authority = res.data.Authority
      this.authorityIdList = res.data.actions.split(',')
    }).catch(err => console.log(err))

    // 获取部门
    window.axios.post('/Organization/GetOrgForElCascader').then(res => (this.department = res.data)).catch(err => console.log(err))

    // 获取管廊
    window.axios.post('/UtilityTunnel/GetUtilityTunnel').then(res => (this.tunnelList = res.data)).catch(err => console.log(err))

    // 参考角色
    window.axios.post('/Role/GetAll').then(res => (this.role = res.data)).catch(err => console.log(err))
  }
}
</script>
<style lang="scss" scoped>
$height: $content-height - 56;
.wrap{
  height: percent($height, $content-height);

  .operation{
    height: percent(200, $height);
  }

  .content{
    height: percent($height - 200, $height);
  }

  .header{
    height: percent(50, $height - 200) !important;
  }

  .scroll{
    height: percent($height - 250, $height - 200);
  }
}
// 功能区
.operation{
  background: rgba(186,186,186,.1);

  .input-group{
    display: flex;
    justify-content: space-between;
    flex-wrap: wrap;

    .list{
      width: 30%;
      margin-top: PXtoEm(15);

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
}
.content{
  overflow: hidden;

  .header{
    display: flex;
    justify-content: space-between;
    align-items: center;
    background: rgba(36, 128, 198, 0.5);
  }

  .list{
    &:nth-of-type(even){
      .list-con{
        background-color: rgba(186, 186, 186, 0.2);
      }
    }

    .list-con{
      display: flex;
      justify-content: space-between;
      padding: {
        top: PXtoEm(15);
        bottom: PXtoEm(15);
      }
      @extend .con-padding-horizontal;
      background: rgba(128, 128, 128, 0.1);
      cursor: pointer;
    }

    .list-sub-con{
      padding: {
        top: PXtoEm(15);
        left: PXtoEm(50);
        bottom: PXtoEm(15);
      };
      border-top: 1px solid #27252b;
      background-color: rgba(0,0,0,.1);
      &:first-of-type{
        border-top: 0;
      }
    }
  }

  .el-icon-arrow-down{
    transition: .5s;
    &.active{
      transform: rotateZ(-180deg);
    }
  }

  .btn-group{
    margin: 30px 0;
    text-align: center;

    .btn{
      margin-left: $btn-margin-left;
    }
  }
}
</style>
