<template>
  <div class="wrap" :class="isShow">
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list" v-show="false">
          <div class="inp-wrap">
            <span class="text">角色编号</span>
            <div class="inp">
              <el-input placeholder="请输入角色名称" v-model="roleID" :disabled="isShow === 'edit'" ></el-input>
            </div>
          </div>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">角色名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model.trim="roleName.text" @keyup.native="validateInput(roleName)" placeholder="请输入角色名称"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ roleName.tips }}</p>
        </li>
        <li class="list" v-show="true"></li>
      </ul>
    </div>
    <div class="content">
      <div class="con-padding-horizontal header">
        菜单名称<!--<span @click="checkAll"><x-button class="small">全选</x-button></span>-->{{roleActionInfo}}
      </div>
      <div class="scroll">
        <el-scrollbar>
          <ul class="list-wrap">
            <li class="list" v-for="(item, index) in authority" :key="item.key">
              <div class="list-con" @click="shrinkChildren(index)">
                <div class="left">{{ item.text }} <i class="el-icon-arrow-down c-pointer" :class="{ active: item.shrink }"></i></div>
                <!--<el-checkbox v-model="item.checked" @change="listCheckAll($event, item)"></el-checkbox>-->
              </div>
              <div class="list-sub-con" v-for="children in item.children" :key="children.key" v-show="item.shrink">
                <el-checkbox-group v-model="roleActionInfo">
                  <el-checkbox :label="children.id">{{ children.text }}</el-checkbox>
                  <el-checkbox-group class="chk-list" v-model="roleActionInfo" v-for="operation in children.children" :key="operation.key">
                    <el-checkbox :label="operation.id">{{ operation.text }}</el-checkbox>
                  </el-checkbox-group>
                </el-checkbox-group>
              </div>
            </li>
          </ul>
          <div class="btn-group">
            <x-button class="close" @click.native="$emit('title', '')">
              <router-link :to="{ name: 'SeeRoleList' }">取消</router-link>
            </x-button>
            <x-button class="active" @click.native="enter">保存</x-button>
          </div>
        </el-scrollbar>
      </div>
    </div>
  </div>
</template>
<script>
import { validateInputCommon } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/authApi'
export default {
  name: 'AddRole',
  components: {
    XButton
  },
  data () {
    return {
      roleName: {
        text: '',
        tips: ''
      },
      roleID: '',
      roleActionInfo: [],
      authority: [],
      // authorityIdList: [],
      listCheck: [],
      bCheckAll: false,
      isShow: ''
    }
  },
  created () {
    this.isShow = this.$route.query.t
    if (this.isShow === 'edit') {
      this.$emit('title', '| 修改角色')
      this.editRoleID = this.$route.query.id[0]
      this.getRole()
    } else {
      this.$emit('title', '| 添加角色')
    }
  },
  methods: {
    // 添加角色
    enter () {
      if (!validateInputCommon(this.roleName)) {
        this.$message({
          message: '验证失败，请查看提示信息',
          type: 'error'
        })
        return
      }
      this.$emit('reload', Math.random())
      if (this.roleActionInfo === []) {
        this.$message({
          message: '必须选择至少一个权限',
          type: 'error'
        })
        return
      }
      let role = {
        role_name: this.roleName.text,
        actions: this.roleActionInfo.join(',')
      }
      if (this.isShow === 'add') {
        // 添加角色
        api.addRole(role).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '添加成功',
              type: 'success',
              onClose: () => {
                this.$router.push({
                  name: 'SeeRoleList'
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
        role.id = this.roleID
        // 修改角色
        api.updateRole(role).then(res => {
          if (res.code === 0) {
            this.$message({
              message: '修改成功',
              type: 'success',
              onClose: () => {
                this.$emit('showChildrenEvent', 'roleList')
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

    // 收起、展开
    shrinkChildren (index) {
      this.authority[index].shrink = !this.authority[index].shrink
    },

    // 修改角色时获取角色资料
    getRole () {
      api.getRoleByID(this.editRoleID).then(res => {
        let _res = res.data
        this.roleID = _res.role.id
        this.roleName.text = _res.role.role_name
        this.roleActionInfo = _res.selectedAction
      }).catch(err => console.log(err))
    },

    // 全选
    // checkAll () {
    //   if (this.bCheckAll) {
    //     this.authority.map(item => (item.checked = false))
    //     this.roleActionInfo = []
    //   } else {
    //     // 每次赋值前先清空数组
    //     this.roleActionInfo = []
    //     this.authority.map(item => {
    //       item.checked = true
    //       item.children.map(child => {
    //         this.roleActionInfo.push(child.id)
    //       })
    //     })
    //   }
    //   this.bCheckAll = !this.bCheckAll
    // },

    // // 列表全选功能
    // listCheckAll (...arr) {
    //   let aIdList = arr[1].subids.split(',')
    //   if (arr[0]) {
    //     this.roleActionInfo = [...this.roleActionInfo, ...aIdList]
    //   } else {
    //     aIdList.forEach((item, i) => {
    //       this.roleActionInfo.forEach((val, index) => {
    //         item === val && this.roleActionInfo.splice(index, 1)
    //       })
    //     })
    //   }
    // },
    // 验证
    validateInput (val) {
      validateInputCommon(val)
    }
  },
  mounted () {
    // 获取菜单列表
    api.getActionTree().then(res => {
      res.data.map(item => {
        item.shrink = false
        item.checked = false
      })
      this.authority = res.data
      // this.authorityIdList = res.data.actions.split(',')
    }).catch(err => console.log(err))
  }
}
</script>
<style lang="scss" scoped>
$height: $content-height - 56;
.wrap{
  height: percent($height, $content-height);

  .operation{
    height: percent(80, $height);
  }

  .content{
    height: percent($height - 80, $height);
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
}

.content{
  $con-height: $height - 80;
  overflow: hidden;
  .header{
    display: flex;
    align-items: center;
    justify-content: space-between;
    height: percent(50, $con-height);
    background: rgba(36, 128, 198, 0.5);

    .btn{
      border-color: #979797;
    }
  }

  .scroll{
    height: percent($con-height - 50, $con-height);
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
      };
      @extend .con-padding-horizontal;
      background: rgba(128, 128, 128, 0.1);
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

    .chk-list{
      display: inline;
      margin-left: 5%;
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
