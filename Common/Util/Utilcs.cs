using Newtonsoft.Json;
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
        public async static Task<T> DownloadJsonDataCustom<T>(String uri)
        {         
            String responseString = "";
            //GetUserInfoList
            HttpClient client = new HttpClient();
            var result =await client.GetStringAsync(uri);

            responseString = CommonUtil.Base64DecodeingToString(result);
            T obj = JsonConvert.DeserializeObject<T>(responseString);

            //responseString = CommonUtil.Base64DecodeingToString(result.Result);
            //T obj = JsonConvert.DeserializeObject<T>(responseString);

            return obj;
        } 

        public async static Task<T> DownloadJsonData<T>(String catelog)
        {
            String uri =  WebResource.WebApi;
            uri = String.Format("{0}?catelog={1}", uri, catelog);
            var obj =await WebApi.DownloadJsonDataCustom<T>(uri);
            
            //T obj = WebApi.DownloadJsonDataCustom<T>(uri);
            return obj;
        }

        public static String UploadJsonData(String catelog, String content)
        {
            String uri = "";
            String input = "";
            uri = WebResource.WebApi;
            content = CommonUtil.StringEncodingToBase64(content);
            input = String.Format("{0}?catelog={1}&content={2}", uri,catelog, content);
            HttpClient client = new HttpClient();
            var result = client.GetStringAsync(input);
            var responseString = result.Result;
            responseString=CommonUtil.Base64DecodeingToString(responseString);
            return responseString;
        }
    }

    public class CommonUtil
    {
        public static String StringEncodingToBase64(String ls_input)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ls_input);

            String st_64 = Convert.ToBase64String(bytes);

            String result = Pattern(st_64);

            return result;
        }

        public static String Base64DecodeingToString(String ls_input)
        {
            String ls_depattern = DePattern(ls_input);

            byte[] bytes = Convert.FromBase64String(ls_depattern);

            String result = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            return result;
        }

        private static String Pattern(String input)
        {
            input = input.Replace("0", "00");
            input = input.Replace("+", "01");
            input = input.Replace("/", "02");
            input = input.Replace("=", "03");
            return input;
        }

        private static String DePattern(String input)
        {
            input = input.Replace("00", "0");
            input = input.Replace("01", "+");
            input = input.Replace("02", "/");
            input = input.Replace("03", "=");
            return input;
        }
    }
}