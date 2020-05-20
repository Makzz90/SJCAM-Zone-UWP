using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml;
//using Windows.UI.Xaml.Media.Animation.PointAnimation;

namespace SJCAM_Zone
{
    public static class AnimationUtil
    {
        public static Storyboard AnimateSeveral(List<AnimationInfo> animInfoList, int? startTime, Action completed = null)
        {
            List<DoubleAnimation> doubleAnimationList = new List<DoubleAnimation>();
            foreach (AnimationInfo animInfo in animInfoList)
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.To = new double?(animInfo.to);
                doubleAnimation.From = new double?(animInfo.from);
                doubleAnimation.EasingFunction = animInfo.easing;
                doubleAnimation.Duration = (Duration)TimeSpan.FromMilliseconds((double)animInfo.duration);
                Storyboard.SetTarget((Timeline)doubleAnimation, animInfo.target);
                Storyboard.SetTargetProperty((Timeline)doubleAnimation, new PropertyPath(animInfo.propertyPath).Path);
                doubleAnimationList.Add(doubleAnimation);
            }
            Storyboard storyboard = new Storyboard();
            if (startTime.HasValue)
                storyboard.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds((double)startTime.Value));
            else
                storyboard.BeginTime = new TimeSpan?();
            if (completed != null)
                storyboard.Completed += ((s, e) => completed());
            foreach (DoubleAnimation doubleAnimation in doubleAnimationList)
                storyboard.Children.Add((Timeline)doubleAnimation);
            storyboard.Begin();
            return storyboard;
        }

        public static Storyboard Animate(this DependencyObject target, double from, double to, string propertyPath, int duration, int? startTime, EasingFunctionBase easing = null, Action completed = null)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.To = new double?(to);
            doubleAnimation.From = new double?(from);
            doubleAnimation.EasingFunction = easing;
            doubleAnimation.Duration = (Duration)TimeSpan.FromMilliseconds((double)duration);
            Storyboard.SetTarget((Timeline)doubleAnimation, target);
            Storyboard.SetTargetProperty((Timeline)doubleAnimation, new PropertyPath(propertyPath).Path);
            Storyboard storyboard = new Storyboard();
            if (startTime.HasValue)
                storyboard.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds((double)startTime.Value));
            else
                storyboard.BeginTime = new TimeSpan?();
            if (completed != null)
                storyboard.Completed += ((s, e) => completed());
            storyboard.Children.Add((Timeline)doubleAnimation);
            storyboard.Begin();
            return storyboard;
        }
    }



    public class AnimationInfo
    {
        public DependencyObject target { get; set; }

        public double from { get; set; }

        public double to { get; set; }

        public string/*object*/ propertyPath { get; set; }

        public int duration { get; set; }

        public EasingFunctionBase easing { get; set; }
    }
}
