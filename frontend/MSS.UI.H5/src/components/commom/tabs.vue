<template>
    <div class="wrap">
        <!-- tabs -->
        <mu-tabs class="tabs" :value="activeTab" @change="handleTabChange">
            <mu-tab value="all" title="技术资料" />
            <!-- <mu-tab value="good" title="精华" />
            <mu-tab value="weex" title="weex" />
            <mu-tab value="share" title="分享" />
            <mu-tab value="ask" title="问答" />
            <mu-tab value="job" title="招聘" /> -->
        </mu-tabs>
        
        <div class="input-group">
            <label for="name">设备类型</label>
            <div class="inp">
            <el-select v-model="eqpTypeOnly" filterable placeholder="请选择" @change="eqpTypeOnlyChange" clearable >
                <el-option
                v-for="item in eqpTypeList"
                :key="item.key"
                :label="item.tName"
                :value="item.id">
                </el-option>
            </el-select>
            </div>
        </div>
        <div class="scroll">
            <!-- <el-scrollbar> -->
                <!-- <upload-pdf class="upload-list"
                :readOnly="readOnly"
                :systemResource="systemResource"
                :fileIDs="eqpTypeFileIDs"
                :unSelectedEntity="unSelectedEqpType"
                @getFileIDs="getFileIDs">
                </upload-pdf> -->
            <!-- </el-scrollbar> -->
        </div>
        <div class="pdf-box">
            <pdf
            v-for="i in numPages"
            :key="i"
            :src="pdfSrc"
            :page="i">
            </pdf>
        </div>
        <!-- <pdf-preview :url="pdfurl"></pdf-preview> -->
    </div>
</template>
<script>
    import axios from 'axios'
    import timeago from 'timeago.js'
    import api from '@/api/eqpApi'
    import { systemResource } from '@/common/js/dictionary.js'
    import { isUploadFinished } from '@/common/js/UpDownloadFileHelper.js'
    import MyUploadPDF from '@/components/UploadPDF'
    
// import PdfPreview from "@/components/commom/PdfPreview"
import pdfPreview from '@/components/PdfPreview.vue'
import pdf from 'vue-pdf'
    export default {
        metaInfo: {
            meta: [
            { name: 'viewport', content: 'width=device-width,initial-scale=1,minimum-scale=1,maximum-scale=2,user-scalable=yes' }
            ]
        },
        components: {
            'upload-pdf': MyUploadPDF,
            pdf,
            pdfPreview,
        },
        data() {
            return {
                loading: false,
                scroller: null,
                nomore: false,
                activeTab: 'all', //当前选中tab项
                items: [],
                styleObj: {
                    backgroundColor: '#999'
                },
                url: 'http://www.vue-js.com/api/v1/topics?tab=all',
                page: 1,
                eqpTypeOnly: '',
                eqpTypeList: [],  
                unSelectedEqp: systemResource.eqp,
                saveEqpLoading: false,
                unSelectedEqpType: systemResource.eqpType,
                systemResource: systemResource.eqpType,
                eqpTypeFileIDs: '',
                readOnly: false,
                // src: '/File/25/29/a9c4aa8b-c566-4ad9-8316-19f4c1c31afe.pdf' http://10.89.36.103:8090/File/25/29/5a237af3-45af-413b-a333-019bec6033b8.pdf
                // pdfSrc: '/File/25/29/411ea423-ada4-4ae9-85ff-5b374ee48de3.PDF', // pdf文件地址
                pdfSrc: '/File/25/29/a9c4aa8b-c566-4ad9-8316-19f4c1c31afe.pdf', // pdf文件地址
                

                // pdfSrc: '/File/25/29/baidu.pdf', // pdf文件地址
                numPages: undefined,
                pdfurl: '/File/25/29/baidu.pdf'
            }
        },
        created() {
            // this.getData()
            this.InitSelect()
        },
        mounted() {
            this.scroller = this.$el
            // 有时PDF文件地址会出现跨域的情况,这里最好处理一下
        　　this.pdfSrc = pdf.createLoadingTask(this.pdfSrc)
            this.pdfSrc.then(pdf => {
            this.numPages = pdf.numPages
            })
        },
        filters: {
            timeago(val) {
                let time = new Date(val)
                var thistime = timeago()
                return thistime.format(time, 'zh_CN') //将UTC时间转换格式---> 几天前,几小时前...
            }
        },
        methods: {   
            handlePreviewFile() {
                this.$refs.pdfSearch.handleOpen();
            }, 
            getFileIDs (ids) {
                if (this.activeName === 'eqp') {
                    this.fileIDsEdit = ids
                } else {
                    this.eqpTypeFileIDsEdit = ids
                }
            },
            loadMore() {
                if (!this.nomore) {
                    this.loading = true
                    this.page +=1
                    let url = this.url + '&page=' + this.page
                    let arr = []
                    setTimeout(() => {
                        let that = this
                        axios.get(url).then(function(response) {
                            arr = response.data.data
                            if (arr.length === 0) {
                                that.loading = false
                                that.nomore = true
                                return
                            }
                            that.items = [...that.items, ...arr]
                            arr = []
                        })
                        this.loading = false
                    }, 1000)
                }
            },
            handleTabChange(val) {
                this.page = 1 //切换tab，页数重置为第一页
                this.nomore = false //切换tab，重置
                this.activeTab = val
                this.url = 'http://www.vue-js.com/api/v1/topics?tab=' + val
                this.getData()
            },
            getData() {
                let that = this
                    // let url = 'http://www.vue-js.com/api/v1/topics?tab=all'
                    // let url = this.url + '&page=' + this.num
                axios.get(this.url).then(function(response) {
                    that.items = response.data.data
                        // console.log(that.items)
                })
            },
            eqpTypeOnlyChange () {
                this.loading = true
                api.getEqpTypeByID(this.eqpTypeOnly).then(res => {
                    this.loading = false
                    let _res = res.data
                    if (_res !== null) {
                    this.eqpTypeFileIDs = _res.uploadFiles
                    } else {
                    this.eqpTypeFileIDs = ''
                    }
                    this.unSelectedEqpType = 0
                }).catch(err => console.log(err))
            },
            InitSelect () {
                api.getEqpTypeAll().then(res => {
                    this.eqpTypeList = res.data
                    // this.eqpType = this.eqpTypeList[0].id
                    // 设备加载
                    // apiMainTain.GetEqpByTypeAndLine(this.eqpType).then(res => {
                    // this.eqpList = res.data
                    // }).catch(err => console.log(err))
                }).catch(err => console.log(err))
            }
        }
    }
</script>
<style lang="scss" scoped>
    .main {
        /* overflow: auto; */
        /*overflow-scrolling: touch;*/
        border: 1px solid #d9d9d9;
        /* width:400px; */
    }

    .tab .tabs {
        margin-top: 4rem;
        height: 3rem;
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        border-top: 1px solid rgba(255,255,255,.1);
        display: flex;
        justify-content: space-around;
        z-index:0;
    }
    .mu-tab-link,.mu-tab-active{
        white-space: nowrap;
    }
    .list {
        display: flex;
        border-bottom: 1px solid #999;
        padding: 1rem;
    }

    .list>img {
        width: 4rem;
        height: 4rem;
        border-radius: 50%;
        margin-right: 1rem;
    }

    .content {
        flex: 1;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
    }

    .list_title {
        /*display: flex;*/
    }

    .list_title>span {
        /*white-space:nowrap;*/
        background-color: #009688;
        color: #fff;
        padding: 0.2rem;
        border-radius: 0.2rem;
    }

    .list_title>h3 {
        display: inline;
        font-weight: 700;
    }

    .timer {
        display: flex;
        justify-content: space-between;
        color: #999;
    }

    .nomore {
        text-align: center;
        padding: 1rem 0;
    }
    .el-scrollbar {
        width:100%;
        position: absolute;
    }
    .scroll {
        /* width:400px; */
        position:absolute;
        height:100%;
        overflow: scroll;
    }
    .scroll::-webkit-scrollbar {
        display: none;
    }
    // .wrapper {
    //     /* width:400px; */
    //     touch-action:none !important;
    //     touch-action:pan-y !important;
    // }
    body {
        background:unset !important;
    }
    .wrap{
        position: relative;
        width: 100%;
        height: 100%;
    }
    .upload-list {
        // overflow: scroll;
        height:100%;
    }
 .pdf-box {
    -webkit-box-sizing: border-box;
    box-sizing: border-box;
    max-width: 1024px;
    width: 100%;
    margin: 0 auto;
    overflow-x: hidden;
    height: 100%;
    overflow-y: auto;
    -webkit-overflow-scrolling: touch;
    font-size: .28rem;
    span {
      width: 100%;
    }
  }
</style>
<style>
    .v-modal{
        z-index: -2 !important;
    }
</style>
