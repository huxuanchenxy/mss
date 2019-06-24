<template>
<div>
    <div v-if="NodeType===1">
        <el-row type="flex" class="row-bg" justify="space-between">
            <el-col :span="6"><div class="grid-content bg-purple">名称: {{comForm.Name}}</div></el-col>
            <el-col :span="6"><div class="grid-content bg-purple-light">编码: {{comForm.Code}}</div></el-col>
            <el-col :span="6"><div class="grid-content bg-purple">负责人: {{comForm.Leader}}</div></el-col>
            <el-col :span="6"><div class="grid-content bg-purple-light">电话: {{comForm.PhoneNum}}</div></el-col>
            </el-row>
            <el-row type="flex" class="row-bg" justify="space-between">
            <el-col :span="24"><div class="grid-content bg-purple">地址: {{comForm.Address}}</div></el-col>
            </el-row>
            <el-row type="flex" class="row-bg" justify="space-between">
            <el-col :span="24"><div class="grid-content bg-purple">描述: {{comForm.Desc}}</div></el-col>
        </el-row>
    </div>
    <div v-if="NodeType===2">
        <el-row type="flex" class="row-bg" justify="space-between">
            <el-col :span="6"><div class="grid-content bg-purple">名称: {{comForm.Name}}</div></el-col>
            <el-col :span="6"><div class="grid-content bg-purple-light">编码: {{comForm.Code}}</div></el-col>
            <el-col :span="12"><div class="grid-content bg-purple">负责人: {{comForm.Leader}}</div></el-col>
            </el-row>
            <el-row type="flex" class="row-bg" justify="space-between">
            <el-col :span="24"><div class="grid-content bg-purple">描述: {{comForm.Desc}}</div></el-col>
        </el-row>
    </div>

<el-table
    :data="tableData"
    style="width: 100%">
    <el-table-column
        label="Name"
        prop="name">
    </el-table-column>
    <el-table-column
        align="left">
        <template slot="header" slot-scope="scope">
        <el-button plain size="medium" icon="el-icon-s-custom" @click="onBindClick()">关联人员</el-button>
        </template>
    </el-table-column>
</el-table>
<el-dialog title="关联人员" :visible.sync="dialogTableVisible" @opened="onDialogOpened()">
    <el-table :data="Users" ref="usersTable" @selection-change="onSelectionChange">
        <el-table-column property="name" label="姓名"></el-table-column>
        <el-table-column
            type="selection"
            width="55">
        </el-table-column>
    </el-table>
    <span slot="footer" class="dialog-footer">
        <el-button @click="dialogTableVisible = false">取 消</el-button>
        <el-button type="primary" @click="onSaveBindClick()">确 定</el-button>
    </span>
</el-dialog>
</div>
</template>
<script>
import api from '@/api/OrgApi'
import {RESULT} from '@/common/js/utils.js'
// import eventBus from '@/components/EventBus'
export default {
    props: {
        nodeId: Object
    },
    data () {
        return {
            Users: [],
            NodeType: null,
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
            dialogTableVisible: false,
            UsersSelected: [],
            OrgUsers: [],
            tableData: []
        }
    },
    mounted () {
        this.getAllUsers()
        // if (this.$route.query.id !== 0) {
        //     this.comForm.ID = this.$route.query.id
        // }
    },
    methods: {
        onSaveBindClick () {
            let data = {
                ID: this.$route.query.id,
                UserIDs: []
            }
            this.UsersSelected.forEach(user => {
                data.UserIDs.push(user.id)
            })
            api.BindOrgUser(data).then((res) => {
                if (res.result === RESULT.Success) {
                    this.dialogTableVisible = false
                    // 重新加载关联的人员
                    this.getOrgUser(this.$route.query.id)
                } else {
                    this.$message.error('关联人员失败')
                }
            })
        },
        onBindClick () {
            this.dialogTableVisible = true
        },
        onSelectionChange (val) {
            this.UsersSelected = val
        },
        onDialogOpened () {
            this.$refs.usersTable.clearSelection()
            this.tableData.forEach(row => {
                this.$refs.usersTable.toggleRowSelection(row)
            })
        },
        getAllUsers () {
            api.getAllUsers().then((res) => {
                this.Users = res.data
                this.getOrgNode(this.$route.query.id)
                this.getOrgUser(this.$route.query.id)
                // if (res.result === RESULT.Success) {
                //     this.initForm(res.data)
                // } else {
                //     this.$message.error('获取节点信息失败')
                // }
            })
        },
        getOrgUser (id) {
            api.getOrgUser(id).then((res) => {
                if (res.result === RESULT.Success) {
                    this.OrgUsers = res.data
                    this.updateTableData()
                } else {
                    this.$message.error('获取节点信息失败')
                }
            })
        },
        updateTableData () {
            this.tableData = []
            this.Users.forEach(user => {
                for (let i = 0; i < this.OrgUsers.length; ++i) {
                    let orguser = this.OrgUsers[i]
                    if (orguser.userID === user.id) {
                        this.tableData.push(user)
                        break
                    }
                }
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
        getUserNameByID (id) {
            let name = ''
            if (id) {
                let iid = parseInt(id)
                for (var i = 0; i < this.Users.length; ++i) {
                    let user = this.Users[i]
                    if (user.id === iid) {
                        name = user.name
                        break
                    }
                }
            }
            return name
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
                        this.comForm.Leader = this.getUserNameByID(
                            props[prop].attrValue)
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
        }
    }
}

</script>
<style lang="scss" scoped>
</style>
