using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cx.Data
{
    /// <summary>
    /// 保存文件
    /// </summary>
    public class FileSave
    {
        /// <summary>
        /// 保存文件到fileUpload文件夹下指定文件夹
        /// </summary>
        /// <param name="foldername">指定文件夹</param>
        /// <param name="topFolder">第一个文件夹名</param>
        /// <returns></returns>
        public static async Task<string[]> SaveFilesAsyn(string foldername, string topFolder = "fileUpload")
        {
            var context = Cx.Data.CxHttpContextExtensions.Current;
            var files = context.Request.Form.Files;
            string uploadPath = Cx.Data.FileHelperCore.MapPath($"/{topFolder}/" + (!string.IsNullOrEmpty(foldername) ? foldername + "/" : ""));
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            string[] paths = new string[files.Count];
            for (int i = 0; i<files.Count; i++)
            {
                var file = files[i];
                string path = await _SaveAsyn(file, uploadPath);
                paths[i]= path;
            }
            return paths;
        }
        /// <summary>
        /// 保存文件到fileUpload文件夹下指定文件夹
        /// </summary>
        ///   <param name="files">上传的文件</param>
        /// <param name="foldername">指定文件夹</param>
        /// <param name="topFolder">第一个文件夹名</param>
        /// <returns></returns>
        public static async Task<string[]> SaveFilesAsyn(IFormFileCollection files, string foldername,  string topFolder = "fileUpload")
        {
            string uploadPath = Cx.Data.FileHelperCore.MapPath($"/{topFolder}/" + (!string.IsNullOrEmpty(foldername) ? foldername + "/" : ""));
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            string[] paths = new string[files.Count];
            for (int i = 0; i<files.Count; i++)
            {
                var file = files[i];
                string path = await _SaveAsyn(file, uploadPath);
                paths[i]= path;
            }
            return paths;
        }

        /// <summary>
        /// 保存指定文件fileUpload文件夹下指定文件夹
        /// </summary>
        /// <param name="foldername">第二个文件夹</param>
        /// <param name="topFolder">第一个文件夹名</param>
        /// <returns></returns>
        public static async Task<string> SaveFileAsyn(string foldername, string topFolder = "fileUpload")
        {
            var context = Cx.Data.CxHttpContextExtensions.Current;
            var file = context.Request.Form.Files[0];
            string uploadPath = Cx.Data.FileHelperCore.MapPath($"/{topFolder}/" + (!string.IsNullOrEmpty(foldername) ? foldername + "/" : ""));
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            return await _SaveAsyn(file, uploadPath);
        }
        /// <summary>
        /// 保存指定文件fileUpload文件夹下指定文件夹
        /// </summary>
        /// <param name="file">上传的文件</param>
        /// <param name="foldername">指定文件夹</param>
        /// <param name="topFolder">第一个文件夹名</param>
        /// <returns></returns>
        public static async Task<string> SaveFileAsyn(IFormFile file, string foldername, string topFolder = "fileUpload")
        {
            string uploadPath = Cx.Data.FileHelperCore.MapPath($"/{topFolder}/" + (!string.IsNullOrEmpty(foldername) ? foldername + "/" : ""));
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            return await _SaveAsyn(file, uploadPath);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="fullprepath">保存的文件夹全路径</param>
        /// <returns></returns>
        private static async Task<string> _SaveAsyn(IFormFile file, string fullprepath)
        {
            string fileName1 = Path.GetFileNameWithoutExtension(file.FileName);//System.IO.Path.GetFileName(file.FileName);
            string fileName2 = Path.GetExtension(file.FileName);
            string path = fullprepath + fileName1 + DateTime.Now.ToString("yyyyMMddHHmmss") + fileName2;

            using (var stream = System.IO.File.Create(path))
            {
                await file.CopyToAsync(stream);
            }
            return path;
        }
    }

}
