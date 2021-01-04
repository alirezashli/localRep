using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Helper.UIHelper
{
    public enum HttpVerb
    {
        Get,
        Post,
        Put,
        Delete
    }
    public class RestClient
    {
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public int TimeOut { get; set; }        

        public RestClient(string endpoint, HttpVerb method, string postData, int timeout = 180)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = postData;
            TimeOut = timeout * 1000;
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public string MakeRequest()
        {
            if (string.IsNullOrEmpty(PostData) || Method != HttpVerb.Post) return null;

            var request = (HttpWebRequest)WebRequest.Create(EndPoint);
            request.Timeout = TimeOut;
            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;

            var bytes = Encoding.GetEncoding("utf-8").GetBytes(PostData);
            request.ContentLength = bytes.Length;
            using (var writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = $"Request failed. Received HTTP {response.StatusCode}";
                    throw new ApplicationException(message);
                }

                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null) return responseValue;
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseValue = reader.ReadToEnd();
                    }
                }

                return responseValue;
            }
        }
    }
}
