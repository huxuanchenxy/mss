<template>
<el-form  :model="comForm" :rules="comRules" ref="comForm" style="width:500px" label-width="80px">
    <el-form-item label="公司名称" prop="Name">
        <el-input v-model="comForm.Name"></el-input>
    </el-form-item>
    <el-form-item prop="Code" label="公司编码">
        <el-input v-model="comForm.Code"></el-input>
    </el-form-item>
    <el-form-item prop="Leader" label="负责人">
        <el-select v-model="comForm.Leader" placeholder="请选择负责人">
            <el-option
            v-for="item in Users"
            :key="item.id"
            :label="item.name"
            :value="item.id">
            </el-option>
        </el-select>
    </el-form-item>
    <el-form-item prop="Address" label="地址">
        <el-input v-model="comForm.Address"></el-input>
    </el-form-item>
    <el-form-item prop="PhoneNum" label="电话">
        <el-input v-model="comForm.PhoneNum"></el-input>
    </el-form-item>
    <el-form-item prop="Desc" label="公司描述">
        <el-input type="textarea" v-model="comForm.Desc"></el-input>
    </el-form-item>
    <el-form-item>
        <el-button type="primary" @click="submitForm()">保存</el-button>
    </el-form-item>
</el-form>
</template>
<script>
import api from '@/api/OrgApi'
import {RESULT, vPhone} from '@/common/js/utils.js'
import eventBus from '@/components/EventBus'
export default {
    data () {
        return {
            Users: [],
            comForm: {
                ID: 0,
                ParentID: null,
                Name: '',
                Code: '',
                Leader: '',
                Address: '',
                PhoneNum: '',
                Desc: ''
            },
            comRules: {
                Name: [
                    { required: true, message: '请输入公司名称', trigger: 'blur' }
                ],
                PhoneNum: [
                    { validator: this.checkPhone, required: false, message: '电话号码不合格', trigger: 'blur' }
                ]
            }
        }
    },
    mounted () {
        // this.initForm()
        this.getAllUsers()
        if (this.$route.query.id !== 0) {
            this.comForm.ID = this.$route.query.id
        }
        if (this.$route.query.parentID) {
            this.comForm.ParentID = this.$route.query.parentID
        }
    },
    methods: {
        checkPhone (rule, value, callback) {
            if (value) {
                if (!vPhone(value)) {
                    callback(new Error(rule.message))
                }
            }
        },
        getAllUsers () {
            api.getAllUsers().then((res) => {
                this.Users = res.data
                if ((this.comForm.ID + '') !== '0') {
                    this.getOrgNode(this.comForm.ID)
                }
                // if (res.result === RESULT.Success) {
                //     this.initForm(res.data)
                // } else {
                //     this.$message.error('获取节点信息失败')
                // }
            })
        },
        getOrgNode (id) {
            api.getOrgNode(id).then((res) => {
                if (res.result === RESULT.Success) {
                    this.initForm(res.data)
                } else {
                    this.$message.error('获取节点信息失败')
                }
            })
        },
        initForm (data) {
            this.comForm.ParentID = data.parentID
            this.comForm.Name = data.name
            let props = data.propEx
            if (props) {
                for (var prop in props) {
                    switch (props[prop].nodeAttr) {
                    case 'Code':
                        this.comForm.Code = props[prop].attrValue
                        break
                    case 'Leader':
                        this.comForm.Leader = parseInt(props[prop].attrValue)
                        break
                    case 'Address':
                        this.comForm.Address = props[prop].attrValue
                        break
                    case 'PhoneNum':
                        this.comForm.PhoneNum = props[prop].attrValue
                        break
                    case 'Desc':
                        this.comForm.Desc = props[prop].attrValue
                        break
                    }
                }
            }
        },
        submitForm () {
            this.$refs['comForm'].validate((valid) => {
                if (valid) {
                    var orgNode = {
                        ID: this.comForm.ID,
                        ParentID: this.comForm.ParentID,
                        Name: this.comForm.Name,
                        NodeType: 1,
                        propEx: this.getPropEx(this.comForm)
                    }
                    eventBus.$emit('submit', orgNode)
                }
            })
        },
        getPropEx (obj) {
            var propEx = []
            for (var item in obj) {
                if (item !== 'Name') {
                    propEx.push({
                        NodeAttr: item,
                        AttrValue: obj[item]
                    })
                }
            }
            return propEx
        }
    }
}

</script>
<style lang="scss" scoped>
</style>
