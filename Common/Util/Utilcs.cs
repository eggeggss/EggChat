﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Util
{
    public class WebApi
    {
        public static T DownloadJsonData<T>(String catelog)
        {
            //GetUserInfoList
            String uri = Resource.WebApi;
            uri = String.Format("{0}?catelog={1}", uri, catelog);
            HttpClient client = new HttpClient();
            var result = client.GetStringAsync(uri);

            String responseString = result.Result;

            T obj = JsonConvert.DeserializeObject<T>(responseString);

            return obj;
        }

        public static void UploadJsonData(String catelog, String content)
        {
            String uri = Resource.WebApi;
            String input = String.Format("catelog={0}&content={1}", catelog, content);
            HttpClient client = new HttpClient();
            var result = client.GetStringAsync(uri);

            String responseString = result.Result;
        }

        /*
        public async static void UpLoadJsonData(String catelog, String content)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://eggeggss.ddns.net/sse/Request.aspx");

            String input = String.Format("catelog={0}&content={1}", catelog, content);

            byte[] data = System.Text.Encoding.UTF8.GetBytes(input);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (var datastream = await request.GetRequestStreamAsync())
            {
                datastream.Write(data, 0, data.Length);
            }

            var response = await request.GetResponseAsync();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }*/
    }
}