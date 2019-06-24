<template>
<el-form  :model="comForm" :rules="comRules" ref="comForm" label-width="80px">
    <el-form-item label="名称" prop="Name">
        <el-input v-model="comForm.Name"></el-input>
    </el-form-item>
    <el-form-item prop="Code" label="编码1">
        <el-input v-model="comForm.Code"></el-input>
    </el-form-item>
    <el-form-item prop="Desc" label="描述1">
        <el-input v-model="comForm.Desc"></el-input>
    </el-form-item>
    <el-form-item>
        <el-button type="primary" @click="submitForm()">立即创建</el-button>
        <el-button @click="resetForm()">重置</el-button>
    </el-form-item>
</el-form>
</template>
<script>
// import api from '@/api/SystemSettingApi'
// import {PageSize, PagerCount, transformDate, ERR_SHOW, SUCCESS_SHOW, FORM_TITLE} from '@/common/js/utils.js'
export default {
    props: ['nodeType'],
    data () {
        return {
            comForm: {
                Name: '新节点',
                Code: '',
                Desc: ''
            },
            comRules: {
                Name: [
                    { required: true, message: '请输入活动名称', trigger: 'blur' }
                ],
                Code: [
                    { required: true, message: '请输入活动名称', trigger: 'blur' }
                ],
                Desc: [
                    { required: true, message: '请填写活动形式', trigger: 'blur' }
                ]
            }
        }
    },
    created () {
    },
    methods: {
        submitForm () {
            this.$refs['comForm'].validate((valid) => {
                if (valid) {
                    alert(this.nodeType)
                    this.$emit('submit', 'params')
                }
            })
        }
    }
}

</script>
<style lang="scss" scoped>
</style>
