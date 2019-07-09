using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using static MSS.API.Common.FilePath;
using System.Text;
using System.IO;

namespace MSS.API.Common.Utility
{
    public class PDFHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folder">所保存的文件夹名称</param>
        /// <returns>所保存的相对路径</returns>

        /// <summary>
        /// 上传一个pdf文件
        /// </summary>
        /// <param name="file">上传的文件列表</param>
        /// <param name="type">上传的文件类型，即与哪张表相关联</param>
        /// <returns>存入数据库的路径</returns>
        public string GetSavePDFPath(List<IFormFile> file, int type)
        {
            string folder = Enum.GetName(typeof(FileType), type) + "/";
            string basepath = (BASEFILE + SHAREFILE + folder).Replace('/', '\\');
            if (!Directory.Exists(basepath))
            {
                Directory.CreateDirectory(basepath);
            }
            if (file.Count > 0)
            {
                foreach (IFormFile item in file)
                {
                    string fileName = item.FileName;
                    string ext = fileName.Substring(fileName.LastIndexOf("."));
                    string fileNameNew = Guid.NewGuid().ToString();
                    return SHAREFILE + folder + fileNameNew + ext;
                }
            }
            return "";
        }

        /// <summary>
        /// 删除一个已上传的pdf文件
        /// </summary>
        /// <param name="path"></param>
        public void DeletePDF(string path)
        {
            string myPath = (BASEFILE + path).Replace('/', '\\');
            File.Delete(myPath);
        }
        public void SavePDF(IFormFile file,string path)
        {
            string filePath = (BASEFILE + path).Replace('/', '\\');
            using (FileStream fs = File.Create(filePath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
        }
    }
}
