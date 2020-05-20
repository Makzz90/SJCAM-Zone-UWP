using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using SJCAM_Zone.DataModel;

namespace SJCAM_Zone.Library
{
    public class ConfigItemTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            RowArray vm = item as RowArray;

            if (vm.type == 5)
            {
                //Format yes no
                return this.Button;
            }
            else if (vm.type == 4)
            {
                //popup WIFI Setting
                return this.Button;
            }
            else if (vm.type == 6)
            {
                //Default Setting - yes no
                return this.Button;
            }

            return this.Combo;
        }
        
        public DataTemplate Combo { get; set; }

        public DataTemplate Text { get; set; }

        public DataTemplate Button { get; set; }
    }
}
