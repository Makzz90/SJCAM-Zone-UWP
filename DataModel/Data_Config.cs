using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SJCAM_Zone.DataModel
{
    public class LaunchImage
    {
        public string launchImage_6s { get; set; }
        public string launchImage_6p { get; set; }
        public string launchImage_5s { get; set; }
        public string launchImage_4s { get; set; }
    }

    public class NoticeList
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class RowArray : INotifyPropertyChanged
    {

        public int isQuickSettings { get; set; }
        public int orderNo { get; set; }
        public string title { get; set; }
        public string cmd { get; set; }
        public List<string> list { get; set; }
        public string paramValue { get; set; }
        public int type { get; set; }

        //
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.RaisePropertyChanged(propertyName);
        }

        private void RaisePropertyChanged(string property)
        {
            if (this.PropertyChanged == null)
                return;
            
            this.PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        private int selected;
        public int Selected
        {
            get { return this.selected; }
            set { this.selected = value;this.NotifyPropertyChanged();  } }
    }

    public class SectionArray
    {
        public int orderNo { get; set; }
        public List<RowArray> row_array { get; set; }
        public string title { get; set; }
    }

    public class International
    {
        public string Language { get; set; }
        public List<SectionArray> section_array { get; set; }
    }

    public class Data_Config
    {
        public List<LaunchImage> launchImage { get; set; }
        public string modify_on { get; set; }
        public bool is_enabled { get; set; }
        public string devicename { get; set; }
        public string version { get; set; }
        public string support { get; set; }
        public string company { get; set; }
        public string logo { get; set; }
        public string download { get; set; }
        public int chip { get; set; }
        public List<NoticeList> noticeList { get; set; }
        public int panoramictype { get; set; }
        public string firmware { get; set; }
        public List<International> International { get; set; }

        public int CameraMode;
    }
}
