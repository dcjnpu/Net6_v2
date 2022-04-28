using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Cx.Data
{
    /// <summary>
    /// 图片上传，转发
    /// </summary>
    public class FileUpload
    {
        private readonly IHttpClientFactory _httpClientfactory;

        /// <summary>
        /// 图片上传，转发,ioc，，请在program里注册，并同时注册httpclient
        /// </summary>
        /// <param name="httpClientfactory"></param>
        public FileUpload(IHttpClientFactory httpClientfactory)
        {
            _httpClientfactory = httpClientfactory;
        }

        /// <summary>
        /// 多文件上传转发
        /// </summary>
        /// <param name="info"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> TransmitFiles(IDictionary<string, string> info, IList<IFormFile> files)
        {
            try
            {
                var postContent = new MultipartFormDataContent();
                string boundary = string.Format("--{0}", DateTime.Now.Ticks.ToString("x"));
                postContent.Headers.Add("ContentType", $"multipart/form-data, boundary={boundary}");
                var requestUri = "/uploads?username=TestUp&token=ofNSNgvCwO691Igq";
                if (files.Any())
                {
                    var stream = files[0].OpenReadStream();
                    //files为文件key, files[0].FileName 为文件名称
                    postContent.Add(new StreamContent(stream, (int)stream.Length), "files", files[0].FileName);
                    //Region为请求文件接口需要的参数，根据调用接口参数而定
                    foreach (var item in info)
                        postContent.Add(new StringContent(item.Value), item.Key);
                }
                var _httpclient = _httpClientfactory.CreateClient("upload_net6");
                var response = await _httpclient.PostAsync(requestUri, postContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseStr = await response.Content.ReadAsStringAsync();
                    //var responseObj = JsonConvert.DeserializeObject<WebApiResult>(responseStr);
                    return responseStr;
                }
                //WebApiResult为自定义反会值
                return "";
            }
            catch (Exception ex)
            {
                Logger.Default.Process("system", "FileUpload.TransmitFiles", "上传图片失败."+ex.Message);
                throw new Exception("保存file异常");
            }
        }

        /// <summary>
        /// 单图片上传
        /// </summary>
        /// <param name="info"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> TransmitFile(IDictionary<string, string> info, IFormFile file)
        {
            try
            {
                var postContent = new MultipartFormDataContent();
                string boundary = string.Format("--{0}", DateTime.Now.Ticks.ToString("x"));
                postContent.Headers.Add("ContentType", $"multipart/form-data, boundary={boundary}");
                var requestUri = "/upload?username=TestUp&token=ofNSNgvCwO691Igq";

                var stream = file.OpenReadStream();
                //files为文件key, files[0].FileName 为文件名称
                postContent.Add(new StreamContent(stream, (int)stream.Length), "files", file.FileName);
                //Region为请求文件接口需要的参数，根据调用接口参数而定
                foreach (var item in info)
                    postContent.Add(new StringContent(item.Value), item.Key);

                var _httpclient = _httpClientfactory.CreateClient("upload_net6");
                var response = await _httpclient.PostAsync(requestUri, postContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseStr = await response.Content.ReadAsStringAsync();
                    //var responseObj = JsonConvert.DeserializeObject<WebApiResult>(responseStr);
                    return responseStr;
                }
                //WebApiResult为自定义反会值
                return "";
            }
            catch (Exception ex)
            {
                Logger.Default.Process("system", "FileUpload.TransmitFile", "上传图片失败."+ex.Message);
                throw new Exception("保存file异常");
            }
        }

        /// <summary>
        /// base64格式图片转发
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="imgcode"></param>
        /// <returns></returns>
        public async Task<string> TransmitFile(string filename, string imgcode)
        {
            if (string.IsNullOrEmpty(filename)) { return String.Empty; }
            if (string.IsNullOrEmpty(imgcode)) { return String.Empty; }

            var _httpclient = _httpClientfactory.CreateClient("upload_net6");
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(filename), "filename");
            content.Add(new StringContent(imgcode), "base64code");

            var response = await _httpclient.PostAsync("UploadBase64?username=TestUp&token=ofNSNgvCwO691Igq", content);
            response.EnsureSuccessStatusCode();
            var b = await response.Content.ReadAsStringAsync();
            return b;
        }
    }
}