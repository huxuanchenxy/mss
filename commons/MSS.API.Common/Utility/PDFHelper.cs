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
        /// 上传一个pdf文件
        /// </summary>
        /// <param name="file">上传的文件列表</param>
        /// <param name="folder">所保存的文件夹名称</param>
        /// <returns>所保存的相对路径</returns>
        public string SavePDF(List<IFormFile> file,string folder)
        {
            string basepath = (BASEFILE + folder).Replace('/', '\\');
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
                    string filePath = basepath + fileNameNew + ext;
                    using (FileStream fs = File.Create(filePath))
                    {
                        item.CopyTo(fs);
                        fs.Flush();
                    }
                    return folder + fileNameNew + ext;
                }
            }
            return "";
        }

    }
}
