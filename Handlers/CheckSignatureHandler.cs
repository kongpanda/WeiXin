using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using WeiXin.Helper;
using System.Collections.Generic;

namespace WeiXin.Handlers
{
    public class CheckSignatureHandler : DelegatingHandler
    {
        ///#region fields
        /// <summary>
        /// TOKEN
        ///// </summary>
        //private const string TOKEN = "finder";
        private static string TOKEN = System.Configuration.ConfigurationManager.AppSettings["apptoken"];
        ///// <summary>
        ///// 签名
        ///// </summary>
        //private const string SIGNATURE = "signature";
        ///// <summary>
        ///// 时间戳
        ///// </summary>
        //private const string TIMESTAMP = "timestamp";
        ///// <summary>
        ///// 随机数
        ///// </summary>
        //private const string NONCE = "nonce";
        ///// <summary>
        ///// 随机字符串
        ///// </summary>
        //private const string ECHOSTR = "echostr";
        ///// <summary>
        ///// 
        ///// </summary>
        ///// 
        ////#endregion
        private HttpRequestMessage _request { set; get; }

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _request = request;
            string method = _request.Method.Method.ToUpper();

            ///Everytime you change Server URL and token on WeiXin Admin panel, 
            ///WeiXin will send the get request to confirm whether it is OK or not.
            if (method == "GET")
            {
                if (CheckSignature())
                {
                    var message = _request.GetQueryString(Utilities.ECHOSTR) ?? "Error!";
                    return await Utilities.SetReponseResult(message, HttpStatusCode.OK);                    
                }
                else
                {
                    return await Utilities.SetReponseResult("Error!", HttpStatusCode.OK);                    
                }                
            }

            //Message and Event Handling
            if (method == "POST")
            {
                var response = await base.SendAsync(_request, cancellationToken);
                return response;
            }
            else 
            {
                return await Utilities.SetReponseResult("非常抱歉该操作不能处理！", HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// 检查签名
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool CheckSignature()
        {
            string signature = _request.GetQueryString(Utilities.SIGNATURE);
            string timestamp = _request.GetQueryString(Utilities.TIMESTAMP);
            string nonce = _request.GetQueryString(Utilities.NONCE);

            List<string> list = new List<string>();
            list.Add(TOKEN);
            list.Add(timestamp);
            list.Add(nonce);
            //排序
            list.Sort();
            //拼串
            string input = string.Empty;
            foreach (var item in list)
            {
                input += item;
            }
            //加密
            string new_signature = Utilities.SHA1Encrypt(input);
            //验证
            if (new_signature == signature)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       
    }

    
}