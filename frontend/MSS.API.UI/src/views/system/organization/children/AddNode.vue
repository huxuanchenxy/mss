<template>
  <div
    class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal operation">
      <el-form ref="form" :rules="rules" label-position="left" :model="form"
        class="custom-form" label-width="80px">
        <el-row >
          <el-col>
            <el-form-item label="节点类型">
              <el-select v-model="form.NodeType" :disabled="this.ReadOnly"  placeholder="请选择">
                <el-option value="1" label="公司" selected></el-option>
                <el-option value="2" label="部门" selected></el-option>
                <el-option value="3" label="班组" selected></el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <div v-if="form.NodeType==='1'">
          <el-row :gutter="40">
            <el-col :span="8">
              <el-form-item label="公司名称" prop="Name">
                <el-input v-model="form.Name" :disabled="this.ReadOnly" ></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="公司编码">
                <el-input v-model="form.Code" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="负责人">
                <el-select v-model="form.Leader" :disabled="this.ReadOnly" style="width:100%" placeholder="请选择">
                  <el-option v-for="item in Users" :key="item.key" :value="item.id" :label="item.user_name"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="40">
            <el-col :span="8">
              <el-form-item label="地址">
                <el-input v-model="form.Address" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="电话" prop="PhoneNum">
                <el-input v-model="form.PhoneNum" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row >
            <el-col>
              <el-form-item label="描述">
                <el-input type="textarea" v-model="form.Desc" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
        </div>
        <div v-if="form.NodeType==='2'">
          <el-row :gutter="40">
            <el-col :span="8">
              <el-form-item label="部门名称" prop="Name">
                <el-input v-model="form.Name" :disabled="this.ReadOnly" ></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="部门编码">
                <el-input v-model="form.Code" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="负责人">
                <el-select v-model="form.Leader" :disabled="this.ReadOnly" style="width:100%" placeholder="请选择">
                  <el-option v-for="item in Users" :key="item.key" :value="item.id" :label="item.user_name"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row >
            <el-col>
              <el-form-item label="描述">
                <el-input type="textarea" v-model="form.Desc" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
        </div>
        <div v-if="form.NodeType==='3'">
          <el-row :gutter="40">
            <el-col :span="8">
              <el-form-item label="班组名称" prop="Name">
                <el-input v-model="form.Name" :disabled="this.ReadOnly" ></el-input>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="负责人">
                <el-select v-model="form.Leader" :disabled="this.ReadOnly" style="width:100%" placeholder="请选择">
                  <el-option v-for="item in Users" :key="item.key" :value="item.id" :label="item.user_name"></el-option>
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="地址">
                <el-input v-model="form.Address" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="40">
            <el-col :span="8">
            </el-col>
            <el-col :span="8">
              <el-form-item label="电话" prop="PhoneNum">
                <el-input v-model="form.PhoneNum" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row >
            <el-col :span="24">
              <el-form-item label="描述">
                <el-input type="textarea" v-model="form.Desc" :disabled="this.ReadOnly"></el-input>
              </el-form-item>
            </el-col>
          </el-row>
        </div>
        <el-row>
          <el-col align="center">
            <el-form-item>
              <x-button class="close">
                <router-link :to="{ name: 'OrgList' }">取消</router-link>
              </x-button>
              <x-button class="active" @click.native="submitForm">保存</x-button>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vInput, vPhone, validateNumberCommon, ApiRESULT } from '@/common/js/utils.js'
import XButton from '@/components/button'
import api from '@/api/orgApi'
// import eventBus from '@/components/Bus'
export default {
  name: 'Node',
  components: {
    XButton
  },
  data () {
    return {
      form: {
        ID: '0',
        ParentID: null,
        NodeType: '1',
        Name: '',
        Code: '',
        Leader: '',
        Address: '',
        PhoneNum: '',
        Desc: ''
      },
      rules: {
        Name: [
          { required: true, validator: this.validateName, message: '该字段不能为空,或含有非法字符', trigger: 'blur' }
        ],
        PhoneNum: [
          { validator: this.validatePhone, trigger: 'blur' }
        ]
      },
      title: '',
      loading: true,
      Users: [],
      ReadOnly: false
    }
  },
  created () {
    this.loading = false
    if (this.$route.query.id !== 0) {
      this.form.ID = this.$route.query.id
    }
    if (this.$route.query.parentID) {
      this.form.ParentID = this.$route.query.parentID
    }
    if (this.$route.query.readonly) {
      this.ReadOnly = true
    }
    this.getAllUsers()
    let subTitle = this.ReadOnly ? '节点查看' : '节点配置'
    this.$emit('pageInfo', {
      title: ' | 组织架构 | ' + subTitle,
      isShowBackBtn: true,
      url: 'OrgList'
    })
  },
  methods: {
    validateNumber () {
      validateNumberCommon(this.menuOrder)
    },
    // 验证
    validateInput (val) {
      validateInputCommon(val)
    },
    validateAll () {
      if (!validateInputCommon(this.Name)) return false
      if (this.PhoneNum.text) {
        if (!vPhone(this.PhoneNum.text)) {
          this.PhoneNum.tips = '电话格式不正确'
          return false
        }
      }

      return true
    },
    validateName (rule, value, callback) {
      value = value.replace(/\s*/g, '')
      if (value === '') {
        callback(new Error('该字段不能空'))
      } else if (!vInput(value)) {
        callback(new Error('此项含有非法字符'))
      } else {
        callback()
      }
    },
    validatePhone (rule, value, callback) {
      if (value) {
        if (!vPhone(value)) {
          callback(new Error('电话号码验证错误'))
        } else {
          callback()
        }
      } else {
        callback()
      }
    },
    getAllUsers () {
      // if ((this.ID + '') !== '0') {
      //   this.getOrgNode(this.ID)
      // }
      api.getAllUsers().then((res) => {
        this.Users = res.data
        if ((this.form.ID + '') !== '0') {
          this.getOrgNode(this.form.ID)
        }
      })
    },
    getOrgNode (id) {
      api.getOrgNode(id).then((res) => {
        if (res.code === ApiRESULT.Success) {
          this.form.NodeType = res.data.nodeType + ''
          if (this.form.NodeType === '1') {
            this.initcomForm(res.data)
          } else if (this.form.NodeType === '2') {
            this.initsecForm(res.data)
          } else if (this.form.NodeType === '3') {
            this.initteamForm(res.data)
          }
        } else {
          this.$message.error('获取节点信息失败')
        }
      })
    },
    initcomForm (data) {
      this.form.ParentID = data.parentID
      this.form.Name = data.name
      let props = data.propEx
      if (props) {
        for (var prop in props) {
          switch (props[prop].nodeAttr) {
            case 'Code':
              this.form.Code = props[prop].attrValue
              break
            case 'Leader':
              this.form.Leader = this.parseLeader(props[prop].attrValue)
              break
            case 'Address':
              this.form.Address = props[prop].attrValue
              break
            case 'PhoneNum':
              this.form.PhoneNum = props[prop].attrValue
              break
            case 'Desc':
              this.form.Desc = props[prop].attrValue
              break
          }
        }
      }
    },
    initsecForm (data) {
      this.form.ParentID = data.parentID
      this.form.Name = data.name
      let props = data.propEx
      if (props) {
        for (var prop in props) {
          switch (props[prop].nodeAttr) {
            case 'Code':
              this.form.Code = props[prop].attrValue
              break
            case 'Leader':
              this.form.Leader = this.parseLeader(props[prop].attrValue)
              break
            case 'Desc':
              this.form.Desc = props[prop].attrValue
              break
          }
        }
      }
    },
    initteamForm (data) {
      this.form.ParentID = data.parentID
      this.form.Name = data.name
      let props = data.propEx
      if (props) {
        for (var prop in props) {
          switch (props[prop].nodeAttr) {
            case 'Leader':
              this.form.Leader = this.parseLeader(props[prop].attrValue)
              break
            case 'Address':
              this.form.Address = props[prop].attrValue
              break
            case 'PhoneNum':
              this.form.PhoneNum = props[prop].attrValue
              break
            case 'Desc':
              this.form.Desc = props[prop].attrValue
              break
          }
        }
      }
    },
    parseLeader (val) {
      if (val) {
        let leaderid = parseInt(val)
        let exist = this.Users.filter(user => {
          return user.id === leaderid
        })
        if (exist.length > 0) {
          return leaderid
        } else {
          return ''
        }
      } else {
        return ''
      }
    },
    submitForm () {
      this.$refs['form'].validate((valid) => {
        if (valid) {
          var orgNode = {
            ID: this.form.ID,
            ParentID: this.form.ParentID,
            Name: this.form.Name,
            NodeType: this.form.NodeType,
            propEx: this.getPropEx()
          }
          // eventBus.$emit('submit', orgNode)
          if ((orgNode.ID + '') === '0') {
            api.addOrgNode(orgNode).then((res) => {
              if (res.code === ApiRESULT.Success) {
              // this.$refs.tree.append({
              //   id: res.data.id,
              //   label: res.data.name,
              //   node_type: res.data.nodeType
              // }, res.data.parentID)
                this.$message.success('保存成功')
                this.$router.push({
                  name: 'OrgList'
                })
              } else if (res.code === ApiRESULT.DataIsExist) {
                this.$message.error('名称重复')
              } else if (res.code === ApiRESULT.CheckDataRulesFail) {
                this.$message.error('父节点类型不能添加子节点，或用户关联规则冲突')
              } else {
                this.$message.error('保存失败')
              }
            })
          } else {
            api.updateOrgNode(orgNode.ID, orgNode).then((res) => {
              if (res.code === ApiRESULT.Success) {
              // let node = this.$refs.tree.getNode(orgNode.ID)
              // node.setData({
              //   id: res.data.id,
              //   label: res.data.name,
              //   node_type: res.data.nodeType
              // })
              // this.$set(this.data, 'label', node.name)
                this.$message.success('更新成功')
                this.$router.push({
                  name: 'OrgList'
                })
              } else if (res.code === ApiRESULT.DataIsExist) {
                this.$message.error('名称重复')
              } else if (res.code === ApiRESULT.CheckDataRulesFail) {
                this.$message.error('此节点有子节点，不能修改为非父节点类型，或用户关联规则冲突')
              } else {
                this.$message.error('更新失败')
              }
            })
          }
        }
      })
    },
    getPropEx (obj) {
      var propEx = []
      switch (this.form.NodeType + '') {
        case '1':
          propEx.push({
            NodeAttr: 'Code',
            AttrValue: this.form.Code
          })
          propEx.push({
            NodeAttr: 'Leader',
            AttrValue: this.form.Leader
          })
          propEx.push({
            NodeAttr: 'Address',
            AttrValue: this.form.Address
          })
          propEx.push({
            NodeAttr: 'PhoneNum',
            AttrValue: this.form.PhoneNum
          })
          propEx.push({
            NodeAttr: 'Desc',
            AttrValue: this.form.Desc
          })
          break
        case '2':
          propEx.push({
            NodeAttr: 'Code',
            AttrValue: this.form.Code
          })
          propEx.push({
            NodeAttr: 'Leader',
            AttrValue: this.form.Leader
          })
          propEx.push({
            NodeAttr: 'Desc',
            AttrValue: this.form.Desc
          })
          break
        case '3':
          propEx.push({
            NodeAttr: 'Leader',
            AttrValue: this.form.Leader
          })
          propEx.push({
            NodeAttr: 'Address',
            AttrValue: this.form.Address
          })
          propEx.push({
            NodeAttr: 'PhoneNum',
            AttrValue: this.form.PhoneNum
          })
          propEx.push({
            NodeAttr: 'Desc',
            AttrValue: this.form.Desc
          })
          break
      }
      return propEx
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
// 表单验证
.custom-form /deep/ .el-form-item.is-success .el-input__inner{
  border-color: #fff
}
.custom-form /deep/ .el-form-item.is-error .el-input__inner{
  border-color: #ffffff
}
.custom-form /deep/ .el-form-item__error{
  color: red;
}
.custom-form /deep/ .el-form-item__label{
  color: #fff
}
</style>
