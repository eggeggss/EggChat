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
        public static T DownloadJsonData<T>(String catelog)
        {


            String uri = "";
            String responseString = "";
            //GetUserInfoList
            uri = Resource.WebApi;
            uri = String.Format("{0}?catelog={1}", uri, catelog);
            HttpClient client = new HttpClient();
            var result = client.GetStringAsync(uri);
            responseString = CommonUtil.Base64DecodeingToString(result.Result);
            T obj = JsonConvert.DeserializeObject<T>(responseString);


            return obj;
        }

        public static String UploadJsonData(String catelog, String content)
        {
            String uri = "";
            String input = "";
            uri = Resource.WebApi;
            content = CommonUtil.StringEncodingToBase64(content);
            input = String.Format("catelog={0}&content={1}", catelog, content);
            HttpClient client = new HttpClient();
            var result = client.GetStringAsync(uri);
            var responseString = result.Result;

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