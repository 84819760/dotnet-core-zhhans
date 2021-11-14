using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows;

namespace NearExtend.WpfPrism
{
    public static class DependencyObjectExtends
    {
        /// <summary>
        /// 调用故事模板,并控制动画
        /// </summary>
        /// <param name="storyname">故事模板名字</param>
        /// <param name="targetname">对象名</param>
        public static void CallStory(this DependencyObject ui, string storyName)
        {
            if (storyName is null || ui is not FrameworkElement fe) return;
            var ControlStory = (Storyboard)fe.Resources[storyName];
            ControlStory?.Begin(fe, true);
        }      
    }
}
