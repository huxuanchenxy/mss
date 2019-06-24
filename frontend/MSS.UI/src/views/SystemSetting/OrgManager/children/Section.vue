<template>
<el-form  :model="secForm" :rules="secRules" ref="secForm" style="width:500px" label-width="80px">
    <el-form-item label="部门名称" prop="Name">
        <el-input v-model="secForm.Name"></el-input>
    </el-form-item>
    <el-form-item prop="Code" label="部门编码">
        <el-input v-model="secForm.Code"></el-input>
    </el-form-item>
    <el-form-item prop="Leader" label="负责人">
        <el-select v-model="secForm.Leader" placeholder="请选择负责人">
            <el-option
            v-for="item in Users"
            :key="item.id"
            :label="item.name"
            :value="item.id">
            </el-option>
        </el-select>
    </el-form-item>
    <el-form-item prop="Desc" label="部门描述">
        <el-input type="textarea" v-model="secForm.Desc"></el-input>
    </el-form-item>
    <el-form-item>
        <el-button type="primary" @click="submitForm()">保存</el-button>
    </el-form-item>
</el-form>
</template>
<script>
import api from '@/api/OrgApi'
import {RESULT} from '@/common/js/utils.js'
import eventBus from '@/components/EventBus'
export default {
    data () {
        return {
            Users: [],
            secForm: {
                ID: 0,
                ParentID: null,
                Name: '',
                Code: '',
                Leader: '',
                Desc: ''
            },
            secRules: {
                Name: [
                    { required: true, message: '请输入公司名称', trigger: 'blur' }
                ]
            }
        }
    },
    mounted () {
        this.getAllUsers()
        if (this.$route.query.id !== 0) {
            this.secForm.ID = this.$route.query.id
        }
        if (this.$route.query.parentID) {
            this.secForm.ParentID = this.$route.query.parentID
        }
    },
    methods: {
        getAllUsers () {
            api.getAllUsers().then((res) => {
                this.Users = res.data
                if ((this.secForm.ID + '') !== '0') {
                    this.getOrgNode(this.secForm.ID)
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
            this.secForm.ParentID = data.parentID
            this.secForm.Name = data.name
            let props = data.propEx
            if (props) {
                for (var prop in props) {
                    switch (props[prop].nodeAttr) {
                    case 'Code':
                        this.secForm.Code = props[prop].attrValue
                        break
                    case 'Desc':
                        this.secForm.Desc = props[prop].attrValue
                        break
                    case 'Leader':
                        this.secForm.Leader = parseInt(props[prop].attrValue)
                        break
                    }
                }
            }
        },
        submitForm () {
            this.$refs['secForm'].validate((valid) => {
                if (valid) {
                    var orgNode = {
                        ID: this.secForm.ID,
                        ParentID: this.secForm.ParentID,
                        Name: this.secForm.Name,
                        NodeType: 2,
                        propEx: this.getPropEx(this.secForm)
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
