<template>
  <el-dialog
    :visible.sync="pdfDialog"
    :close-on-click-modal="false"
    :show-close="false"
    width="900px"
    top="52px"
  >
    <div class="pdf" v-show="fileType == 'pdf'">
      <p class="arrow">
        <!-- 上一页 -->
        <span
          @click="changePdfPage(0)"
          class="currentPage"
          :class="{ grey: currentPage == 1 }"
          >上一页&nbsp;&nbsp;</span
        >
        <span style="color: #8c8e92;">{{ currentPage }} / {{ pageCount }}</span>
        <!-- 下一页 -->
        <span
          @click="changePdfPage(1)"
          class="currentPage"
          :class="{ grey: currentPage == pageCount }"
          >&nbsp;&nbsp;下一页</span
        >&nbsp;&nbsp;&nbsp;&nbsp;<button @click="$refs.pdf.print()">下载</button>

        <span
          style="float :right;padding-right:40px;font-size: 20px;color: #8c8e92;cursor: pointer;"
          @click="close"
          ><i class="el-icon-close"></i
        ></span>
      </p>
      <!-- loadPdfHandler：加载事件 src：需要展示的PDF地址；currentPage：当前展示的PDF页码；pageCount=$event：PDF文件总页码；currentPage=$event：一开始加载的页面-->
      <pdf
         ref="pdf"
        :src="src"
        :page="currentPage"
        @num-pages="pageCount = $event"
        @page-loaded="currentPage = $event"
        @loaded="loadPdfHandler"
      ></pdf>
            <!-- <pdf
         ref="pdf"
        :src="src"></pdf> -->
    </div>
  </el-dialog>
</template>

<script>
import pdf from "vue-pdf";
export default {
  components: { pdf },
  props: ["src"],
  data() {
    return {
      filesProps: {
        label: "originName"
      },
      pdfDialog: false,
      currentPage: 0, // pdf文件页码
      pageCount: 0, // pdf文件总页数
      fileType: "pdf" // 文件类型
    };
  },
  methods: {
    // 改变PDF页码,val传过来区分上一页下一页的值,0上一页,1下一页
    changePdfPage(val) {
      if (val === 0 && this.currentPage > 1) {
        this.currentPage--;
      }
      if (val === 1 && this.currentPage < this.pageCount) {
        this.currentPage++;
      }
    },
    // pdf加载时
    loadPdfHandler() {
      this.currentPage = 1; // 加载的时候先加载第一页
    },
    handleOpen() {
      this.pdfDialog = true;
    },
    //关闭弹框
    close() {
      this.pdfDialog = false;
    }
  }
};
</script>

<style lang="scss" scoped>
.currentPage {
    cursor: pointer;
    color: #8c8e92;
}

.currentPage:hover {
    color: #2761ff;
}
.arrow{
    position: fixed;
    top: 0px;
    left :0px;
    z-index: 2;
    width: 100%;
    background-color: #191919;
    padding: 12px 0;
    margin: 0;
    text-align :center;
}
.el-dialog__body {
    color: #606266;
    font-size: 14px;
    padding:0;
}
</style>

