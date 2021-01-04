using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Helper.UIHelper;
using Microsoft.AspNet.SignalR;
using SignalRChat;
using testThreadAlongMainWebTread.Util;

namespace testThreadAlongMainWebTread.Helper
{
    public static class helper
    {
      

    
        public static Dictionary<string, string> users = new Dictionary<string, string>();
        public static void AddUsers(string connectionId,string MainKey )
        {
            string p; 
            users.TryGetValue(MainKey, out p);
            if (string.IsNullOrEmpty(p))
                users.Add(MainKey, connectionId);
        }
        public static string GetCaptcha(string token)
        {
            var capData = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(120, 45, /*Helper.EnglishToPersianNumber(*/capData/*)*/);
            try
            {
                var signTokenCaptcha = CaptchaKeySpec.SignCaptcha(token, capData);
                dynamic obj = new { fileStream = "data:image/png;base64," + result.CaptchBase64Data, signData = signTokenCaptcha, status = true };
                return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            }
            catch (Exception err)
            {
                throw;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { fileStream = "", signData = "", status = false });
        }
        //  GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new MyIdProvider());
    }
}