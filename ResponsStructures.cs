using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJCAM_Zone
{
    public class LaunchImage
    {
        public string launch_image_4s { get; set; }
        public string launch_image_5s { get; set; }
        public string launch_image_6p { get; set; }
        public string launch_image_6s { get; set; }
        public string launch_image_x { get; set; }
    }

    public class DriverNew
    {
        public string firmware { get; set; }
        public string version { get; set; }
        public int versionmodifydate { get; set; }
        public string path { get; set; }
        public LaunchImage launch_image { get; set; }
    }













    public class Image
    {
        public string url { get; set; }
        public int type { get; set; }
    }

    public class ModelList
    {
        public int id { get; set; }
        public string title { get; set; }
        public string link { get; set; }
        public List<Image> images { get; set; }
    }

    /// <summary>
    /// Баннер на главной странице
    /// </summary>
    public class Banner
    {
        public List<ModelList> model_list { get; set; }
    }











    /// <summary>
    /// Карусель из топовых фотографий
    /// </summary>
    public class AppTop
    {
        public int content_id { get; set; }
        public int content_type_id { get; set; }
        public string content_url { get; set; }
        public string content_key { get; set; }
        public string content_image { get; set; }
        public string content_image_size { get; set; }
        public int watch { get; set; }
        public string area_code { get; set; }
        public int publish_time { get; set; }
        public string nickname { get; set; }
        public string avatar { get; set; }
        public int tag_id { get; set; }
        public bool is_panoram { get; set; }
        public bool is_praise { get; set; }
        public bool is_collect { get; set; }
        public bool is_share { get; set; }
    }




    /// <summary>
    /// Горизонтальный список рекомендуемых людей
    /// </summary>
    public class Recommended
    {
        public int id { get; set; }
        public string avatar { get; set; }
        public string nickname { get; set; }
    }









    public class Item
    {
        public int content_id { get; set; }
        public int tag_id { get; set; }
        public int content_type_id { get; set; }
        public int account_id { get; set; }
        public string nickname { get; set; }
        public string avatar { get; set; }
        public string area_name { get; set; }
        public string area_code { get; set; }
        public string content_desc { get; set; }
        public int publish_time { get; set; }
        public string content_url { get; set; }
        public string content_key { get; set; }
        public string content_image { get; set; }
        public string content_image_size { get; set; }
        public int watch { get; set; }
        public int praise { get; set; }
        public int comment { get; set; }
        public int collect { get; set; }
        public int share { get; set; }
        public bool is_panoram { get; set; }
        public int resolution { get; set; }
        public bool is_praise { get; set; }
        public bool is_collect { get; set; }
        public bool is_share { get; set; }
        public bool is_enable { get; set; }
//        public List<object> view_content_details { get; set; }
    }

    public class Top
    {
        public List<Item> items { get; set; }
        public int total_page { get; set; }
        public int total_count { get; set; }
        public int page { get; set; }
        public int per_page { get; set; }
        public int total_skiped { get; set; }
    }
}
