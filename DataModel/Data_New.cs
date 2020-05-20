using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJCAM_Zone.DataModel
{
    public class Data_New
    {
        public async Task LoadAppTop()
        {
            var bannerTop = await RequestsDispatcher.GetResponse<List<AppTop>>("banner/apptop?top=5");
            var bannerRecommended = await RequestsDispatcher.GetResponse<List<Recommended>>("account/recommend?top=10");
        }

        public async void LoadMore()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("sawContentIds", "");
            //param.Add("deviceNum", "a1047301b670fcRWFS407KE0024356");
            var top = await RequestsDispatcher.PostResponse<Top>("contents/recommendContents", param);

            //sawContentIds=27813%2C8666%2C22955%2C26881%2C14083%2C3786%2C27208%2C38161%2C16492%2C12652%2C6968%2C2888%2C8960%2C42573%2C15536%2C41498%2C29807%2C18811%2C39217%2C30438%2C42280%2C198%2C36508%2C32545%2C5162%2C36859%2C14087%2C6229%2C18783%2C29636&deviceNum=3da1047301b670fcRWFS407KE0024356&
        }
    }
}
