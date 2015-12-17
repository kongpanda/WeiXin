using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using WeiXin.Helper;
using Yank.WeiXin.Messages;

namespace WeiXin.Handlers
{
    public class ServicesHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Task<string> taskrequest = request.Content.ReadAsStringAsync();
            
            if (taskrequest.IsCompleted)
            {
                var requestXml = taskrequest.Result;    

                if (!string.IsNullOrEmpty(requestXml))
                {
                    return await WeixinEventMessageHandler(requestXml);
                }
                else {
                    return await Utilities.SetReponseResult("错误，请稍后！", HttpStatusCode.OK);
                }
            }
            else
                return await Utilities.SetReponseResult("错误，请稍后！", HttpStatusCode.OK);

        }

        private async Task<HttpResponseMessage> WeixinEventMessageHandler(string requestXml)
        {
            //解析数据
            HttpResponseMessage response = null;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(requestXml);
            XmlNode node = doc.SelectSingleNode("/xml/MsgType");
            if (node != null)
            {
                XmlCDataSection section = node.FirstChild as XmlCDataSection;
                if (section != null)
                {
                    string msgType = section.Value;
                    switch (msgType)
                    {
                        case "text":
                            response = await WeixinEventHandler(requestXml);
                            break;
                        case "event":
                            response = await WeixinEventHandler(requestXml);
                            break;
                    }
                    return response;
                }
                else
                {
                    return await Utilities.SetReponseResult("错误，请稍后！", HttpStatusCode.OK);
                }

            }
            else
            {
                return await Utilities.SetReponseResult("错误，请稍后！", HttpStatusCode.OK);
            }
        }

        private async Task<HttpResponseMessage> WeixinEventHandler(string requestXml)
        {
            string responsemessage;
            EventMessage em = EventMessage.LoadFromXml(requestXml);
            if (em.Event.Equals("subscribe", StringComparison.OrdinalIgnoreCase))
            {
                //retrun the welcome message，after followd weixin
                TextMessage tm = new TextMessage();
                tm.ToUserName = em.FromUserName;
                tm.FromUserName = em.ToUserName;
                tm.CreateTime = Utilities.GetNowTime();
                tm.Content = "欢迎您关注***，我是大哥大，有事就问我，呵呵！\n\n";
                responsemessage = tm.GenerateContent();
                return await Utilities.SetReponseResult(responsemessage, HttpStatusCode.OK);
            }
            else if(em.Event.Equals("click", StringComparison.OrdinalIgnoreCase)) {
                string result = string.Empty;
                if (em != null && em.EventKey != null)
                {
                    switch (em.EventKey.ToUpper())
                    {
                        case "BTN_GOOD":
                            result = "BTN_GOOD";//btnGoodClick(em);
                            break;                        
                        case "BTN_HELP":
                            result = "BTN_HELP";
                            break;
                    }
                }
                return await Utilities.SetReponseResult(result, HttpStatusCode.OK);
            }else{
                return await Utilities.SetReponseResult("该事件操作还未支持！", HttpStatusCode.OK);
            }


        }

        private async Task<HttpResponseMessage> WeixinMessageHandler(string requestXml)
        {
            throw new NotImplementedException();
        }
    }  
}