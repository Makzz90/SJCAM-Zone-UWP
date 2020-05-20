using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace SJCAM_Zone
{
    public class RequestsDispatcher
    {
        public static async Task<T> GetResponse<T>(string methodName)
        {
            string query = "http://eapi.sjcamzone.cc/api/" +  methodName;

            string json = await GetAsync(query);
            
            T response = default(T);
            if (string.IsNullOrEmpty(json))
                return response;

            try
            {
                response = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
            }
            
            return response;
        }

        public static async Task<T> PostResponse<T>(string methodName, Dictionary<string, string> parameters)
        {
            string query = "http://eapi.sjcamzone.cc/api/" + methodName;

            string json = await PostAsync(query, parameters);

            T response = default(T);
            if (string.IsNullOrEmpty(json))
                return response;

            try
            {
                response = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
            }

            return response;
        }

        private static async Task<string> PostAsync(string requestURL, Dictionary<string, string> parameters)
        {
            string result = String.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-SJCAM-Secret", "dGltZXN0YW1wPTE1MzQxNTQ4NDgmbm9uY2U9RWpycmh4JmFwcHR5cGU9YW5kcm9pZCZzaWduYXR1cmU9ZDRkZTdiYjQyMzMyZDQ3ODEwZThjYmUxY2ZlYzg1Zjg=");
                    var response = await client.PostAsync(requestURL, new FormUrlEncodedContent(parameters));
                    result = await response.Content.ReadAsStringAsync();
                }
                catch (Exception) { }
            }
            return result;
        }

        public static async Task<string> GetAsync(string query)
        {
            string result = String.Empty;

            //HttpClientHandler handler = new HttpClientHandler();
            //if(Settings.Instance.UseProxy)
            //{
            //    MyProxy proxy = new MyProxy(Settings.Instance.ProxyAdress);
            //    handler.Proxy = proxy;
            //}

            using (HttpClient client = new HttpClient(/*handler*/))
            {
                try
                {
                    client.DefaultRequestHeaders.Add("X-SJCAM-Secret", "dGltZXN0YW1wPTE1MzQxNTQ4NDgmbm9uY2U9RWpycmh4JmFwcHR5cGU9YW5kcm9pZCZzaWduYXR1cmU9ZDRkZTdiYjQyMzMyZDQ3ODEwZThjYmUxY2ZlYzg1Zjg=");
                    var response = await client.GetAsync(query);
                    result = await response.Content.ReadAsStringAsync();
                }
                catch (Exception)
                {

                }
            }
            return result;
        }
    }
}
