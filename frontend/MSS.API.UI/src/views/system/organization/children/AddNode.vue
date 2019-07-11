<template>
  <div
    class="wrap height-full"
    v-loading="loading"
    element-loading-text="加载中"
    element-loading-spinner="el-icon-loading">
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">节点类型</span>
            <div class="inp">
              <el-select v-model="NodeType" :disabled="this.ReadOnly"  placeholder="请选择">
                <el-option value="1" label="公司" selected></el-option>
                <el-option value="2" label="部门" selected></el-option>
                <el-option value="3" label="班组" selected></el-option>
              </el-select>
            </div>
          </div>
        </li>
      </ul>
    </div>
    <div v-if="NodeType==='1'">
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">公司名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model="Name.text" :disabled="this.ReadOnly" @keyup.native="validateInput(Name)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ Name.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">公司编码</span>
            <div class="inp">
              <el-input v-model="Code.text" :disabled="this.ReadOnly"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ Code.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">负责人</span>
            <div class="inp">
              <el-select v-model="Leader.text" :disabled="this.ReadOnly" placeholder="请选择">
                <el-option v-for="item in Users" :key="item.key" :value="item.id" :label="item.user_name"></el-option>
              </el-select>
            </div>
          </div>
          <p class="validate-tips">{{ Leader.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">地址</span>
            <div class="inp">
              <el-input v-model="Address.text" :disabled="this.ReadOnly"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ Address.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">电话</span>
            <div class="inp">
              <el-input v-model="PhoneNum.text" :disabled="this.ReadOnly"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ PhoneNum.tips }}</p>
        </li>
        <li class="list"></li>
      </ul>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">描述</span>
            <div class="inp">
              <el-input type="textarea" :disabled="this.ReadOnly" v-model="Desc.text"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ Desc.tips }}</p>
        </li>
      </ul>
    </div>
    </div>
    <div v-if="NodeType==='2'">
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">部门名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model="Name.text" :disabled="this.ReadOnly" @keyup.native="validateInput(Name)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ Name.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">部门编码</span>
            <div class="inp">
              <el-input v-model="Code.text" :disabled="this.ReadOnly"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ Code.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">负责人</span>
            <div class="inp">
              <el-select v-model="Leader.text" :disabled="this.ReadOnly" placeholder="请选择">
                <el-option v-for="item in Users" :key="item.key" :value="item.id" :label="item.user_name"></el-option>
              </el-select>
            </div>
          </div>
          <p class="validate-tips">{{ Leader.tips }}</p>
        </li>
      </ul>
    </div>
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">描述</span>
            <div class="inp">
              <el-input type="textarea" v-model="Desc.text" :disabled="this.ReadOnly"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ Desc.tips }}</p>
        </li>
      </ul>
    </div>
    </div>
    <div v-if="NodeType==='3'">
    <div class="con-padding-horizontal operation">
      <ul class="input-group">
        <li class="list">
          <div class="inp-wrap">
            <span class="text">班组名称<em class="validate-mark">*</em></span>
            <div class="inp">
              <el-input v-model="Name.text" :disabled="this.ReadOnly" @keyup.native="validateInput(Name)"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ Name.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">负责人</span>
            <div class="inp">
              <el-select v-model="Leader.text" :disabled="this.ReadOnly" placeholder="请选择">
                <el-option v-for="item in Users" :key="item.key" :value="item.id" :label="item.user_name"></el-option>
              </el-select>
            </div>
          </div>
          <p class="validate-tips">{{ Leader.tips }}</p>
        </li>
        <li class="list" >
          <div class="inp-wrap">
            <span class="text">地点</span>
            <div class="inp">
              <el-input v-model="Address.text" :disabled="this.ReadOnly"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ Address.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">电话</span>
            <div class="inp">
              <el-input v-model="PhoneNum.text" :disabled="this.ReadOnly"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ PhoneNum.tips }}</p>
        </li>
        <li class="list">
          <div class="inp-wrap">
            <span class="text">描述</span>
            <div class="inp">
              <el-input type="textarea" v-model="Desc.text" :disabled="this.ReadOnly"></el-input>
            </div>
          </div>
          <p class="validate-tips">{{ Desc.tips }}</p>
        </li>
      </ul>
    </div>
    </div>
    <div class="btn-group" v-if="!ReadOnly">
      <x-button class="close">
        <router-link :to="{ name: 'OrgList' }">取消</router-link>
      </x-button>
      <x-button class="active" @click.native="submitForm">保存</x-button>
    </div>
  </div>
</template>
<script>
import { validateInputCommon, vPhone, validateNumberCommon, RESULT, ApiRESULT } from '@/common/js/utils.js'
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
      title: '',
      loading: true,
      Users: [],
      ReadOnly: false,
      ID: '0',
      ParentID: null,
      NodeType: '1',
      Name: {
        text: '',
        tips: ''
      },
      Code: {
        text: '',
        tips: ''
      },
      Leader: {
        text: '',
        tips: ''
      },
      Address: {
        text: '',
        tips: ''
      },
      PhoneNum: {
        text: '',
        tips: ''
      },
      Desc: {
        text: '',
        tips: ''
      }
    }
  },
  created () {
    this.loading = false
    if (this.$route.query.id !== 0) {
      this.ID = this.$route.query.id
    }
    if (this.$route.query.parentID) {
      this.ParentID = this.$route.query.parentID
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
    getAllUsers () {
      // if ((this.ID + '') !== '0') {
      //   this.getOrgNode(this.ID)
      // }
      api.getAllUsers().then((res) => {
        this.Users = res.data
        if ((this.ID + '') !== '0') {
          this.getOrgNode(this.ID)
        }
      })
    },
    getOrgNode (id) {
      api.getOrgNode(id).then((res) => {
        if (res.result === RESULT.Success) {
          this.NodeType = res.data.nodeType + ''
          if (this.NodeType === '1') {
            this.initcomForm(res.data)
          } else if (this.NodeType === '2') {
            this.initsecForm(res.data)
          } else if (this.NodeType === '3') {
            this.initteamForm(res.data)
          }
        } else {
          this.$message.error('获取节点信息失败')
        }
      })
    },
    initcomForm (data) {
      this.ParentID = data.parentID
      this.Name.text = data.name
      let props = data.propEx
      if (props) {
        for (var prop in props) {
          switch (props[prop].nodeAttr) {
            case 'Code':
              this.Code.text = props[prop].attrValue
              break
            case 'Leader':
              this.Leader.text = this.parseLeader(props[prop].attrValue)
              break
            case 'Address':
              this.Address.text = props[prop].attrValue
              break
            case 'PhoneNum':
              this.PhoneNum.text = props[prop].attrValue
              break
            case 'Desc':
              this.Desc.text = props[prop].attrValue
              break
          }
        }
      }
    },
    initsecForm (data) {
      this.ParentID = data.parentID
      this.Name.text = data.name
      let props = data.propEx
      if (props) {
        for (var prop in props) {
          switch (props[prop].nodeAttr) {
            case 'Code':
              this.Code.text = props[prop].attrValue
              break
            case 'Leader':
              this.Leader.text = this.parseLeader(props[prop].attrValue)
              break
            case 'Desc':
              this.Desc.text = props[prop].attrValue
              break
          }
        }
      }
    },
    initteamForm (data) {
      this.ParentID = data.parentID
      this.Name.text = data.name
      let props = data.propEx
      if (props) {
        for (var prop in props) {
          switch (props[prop].nodeAttr) {
            case 'Leader':
              this.Leader.text = this.parseLeader(props[prop].attrValue)
              break
            case 'Address':
              this.Address.text = props[prop].attrValue
              break
            case 'PhoneNum':
              this.PhoneNum.text = props[prop].attrValue
              break
            case 'Desc':
              this.Desc.text = props[prop].attrValue
              break
          }
        }
      }
    },
    parseLeader (val) {
      if (val) {
        return parseInt(val)
      } else {
        return ''
      }
    },
    submitForm () {
      if (this.validateAll()) {
        var orgNode = {
          ID: this.ID,
          ParentID: this.ParentID,
          Name: this.Name.text,
          NodeType: this.NodeType,
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
              this.$message.error('父节点类型不能添加子节点')
            } else {
              this.$message.error('保存失败')
            }
          })
        } else {
          api.updateOrgNode(orgNode.ID, orgNode).then((res) => {
            if (res.result === RESULT.Success) {
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
            } else if (res.result === RESULT.Reinsert) {
              this.$message.error('名称重复')
            } else {
              this.$message.error('保存失败')
            }
          })
        }
      }
    },
    getPropEx (obj) {
      var propEx = []
      switch (this.NodeType + '') {
        case '1':
          propEx.push({
            NodeAttr: 'Code',
            AttrValue: this.Code.text
          })
          propEx.push({
            NodeAttr: 'Leader',
            AttrValue: this.Leader.text
          })
          propEx.push({
            NodeAttr: 'Address',
            AttrValue: this.Address.text
          })
          propEx.push({
            NodeAttr: 'PhoneNum',
            AttrValue: this.PhoneNum.text
          })
          propEx.push({
            NodeAttr: 'Desc',
            AttrValue: this.Desc.text
          })
          break
        case '2':
          propEx.push({
            NodeAttr: 'Code',
            AttrValue: this.Code.text
          })
          propEx.push({
            NodeAttr: 'Leader',
            AttrValue: this.Leader.text
          })
          propEx.push({
            NodeAttr: 'Desc',
            AttrValue: this.Desc.text
          })
          break
        case '3':
          propEx.push({
            NodeAttr: 'Leader',
            AttrValue: this.Leader.text
          })
          propEx.push({
            NodeAttr: 'Address',
            AttrValue: this.Address.text
          })
          propEx.push({
            NodeAttr: 'PhoneNum',
            AttrValue: this.PhoneNum.text
          })
          propEx.push({
            NodeAttr: 'Desc',
            AttrValue: this.Desc.text
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
</style>
