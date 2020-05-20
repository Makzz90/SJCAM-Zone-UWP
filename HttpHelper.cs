using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Text.RegularExpressions;

namespace SJCAM_Zone
{
    public static class HttpHelper
    {
        public static bool IsLocked = false;

        public static HttpClient httpClient = new HttpClient();

        private static readonly Regex reg_Cmd = new Regex("<Cmd>([0-9]+)<\\/Cmd>");
        private static readonly Regex reg_Status = new Regex("<Status>([\\-0-9]+)<\\/Status>");//todo: minus
        private static readonly Regex reg_Value = new Regex("<Value>([0-9]+)<\\/Value>");
        private static readonly Regex reg_String = new Regex("<String>(.+)<\\/String>");


        /*
         * <Cmd>3012</Cmd>
<Status>0</Status>
<String>655</String>
<String>SJ5000WIFI</String>
<String>V1.9</String>
<String>Dec 29 2017</String>
<String>SJCAM</String>
*/





        public static async Task<string> SendCmd_retStatus(String cmd)
        {
            if (IsLocked)
                return "";
            /*
            <?xml version="1.0" encoding="UTF-8" ?>
            <Function>
            <Cmd>3001</Cmd>
            <Status>0</Status>
            </Function>
             */

        string str = "";
            try
            {

                str = await HttpHelper.httpClient.GetStringAsync(new Uri("http://192.168.1.254/?custom=1&" + cmd));
                //MatchCollection mc = reg_Cmd.Matches(str);
                MatchCollection mc2 = reg_Status.Matches(str);
                /*
                for (int i = 0; i < mc.Count; i++)
                {
                    Match m = mc[i];
                    Match m2 = mc2[i];



                    string s = m.Groups[1].Value;
                    string s2 = m2.Groups[1].Value;

                    str = s2;

                    System.Diagnostics.Debug.WriteLine(">" + s + " " + s2);
                }
                */
                str = mc2[0].Groups[1].Value;

            }
            catch
            {

            }

            return str;
        }

        public static async Task<string> SendCmd_retValue(String cmd)
        {
            if (IsLocked)
                return "";

            string str = "";
            try
            {

                str = await HttpHelper.httpClient.GetStringAsync(new Uri("http://192.168.1.254/?custom=1&" + cmd));
                MatchCollection mc2 = reg_Value.Matches(str);
                str = mc2[0].Groups[1].Value;

            }
            catch
            {

            }

            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir">Папка</param>
        /// <returns></returns>
        public static async Task<string> Explore(String dir)
        {
            if (IsLocked)
                return "";

            string str = "";
            try
            {
                str = await HttpHelper.httpClient.GetStringAsync(new Uri("http://192.168.1.254/DCIM/" + dir));
            }
            catch
            {

            }

            return str;
        }

        public static async Task<Dictionary<string, string>> SendCmd_retDictionary(String cmd)
        {
            /*
            <?xml version="1.0" encoding="UTF-8" ?>
            <Function>
            <Cmd>3001</Cmd>
            <Status>0</Status>
            </Function>
             */

            string str = "";
            Dictionary<string, string> output = new Dictionary<string, string>();
            try
            {

                str = await HttpHelper.httpClient.GetStringAsync(new Uri("http://192.168.1.254/?custom=1&" + cmd));
                MatchCollection mc = reg_Cmd.Matches(str);
                MatchCollection mc2 = reg_Status.Matches(str);
                
                for (int i = 0; i < mc.Count; i++)
                {
                    Match m = mc[i];
                    Match m2 = mc2[i];



                    string s = m.Groups[1].Value;
                    string s2 = m2.Groups[1].Value;

                    output.Add(s, s2);
                }
                
                

            }
            catch
            {

            }

            return output;
        }

        public static async Task<List<string>> SendCmd_retList(String cmd)
        {
            string str = "";
            List<string> output = new List<string>();
            try
            {

                str = await HttpHelper.httpClient.GetStringAsync(new Uri("http://192.168.1.254/?custom=1&" + cmd));
                MatchCollection mc = reg_String.Matches(str);

                for (int i = 0; i < mc.Count; i++)
                {
                    Match m = mc[i];
                    string s = m.Groups[1].Value;
                    output.Add(s);
                }
            }
            catch
            {

            }

            return output;
        }
    }
}
