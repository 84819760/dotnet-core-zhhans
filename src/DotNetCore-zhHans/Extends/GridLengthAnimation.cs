using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DotNetCorezhHans
{
    public class GridLengthAnimation : AnimationTimeline
    {
        public GridLengthAnimation()
        {

        }

        /// <summary>
        /// 返回要设置动画的对象的类型
        /// </summary>
        public override Type TargetPropertyType => typeof(GridLength);

        /// <summary>
        /// 创建动画对象的实例
        /// </summary>
        protected override Freezable CreateInstanceCore() => new GridLengthAnimation();

        /// <summary>
        ///From属性的依赖项属性
        /// </summary>
        public static readonly DependencyProperty FromProperty = DependencyProperty
            .Register("From", typeof(GridLength), typeof(GridLengthAnimation));

        /// <summary>
        ///From Dependency属性的CLR包装器
        /// </summary>
        public GridLength From
        {
            get => (GridLength)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        /// <summary>
        /// To属性的依赖项属性
        /// </summary>
        public static readonly DependencyProperty ToProperty = DependencyProperty
            .Register("To", typeof(GridLength), typeof(GridLengthAnimation));

        /// <summary>
        /// To属性的CLR包装器
        /// </summary>
        public GridLength To
        {
            get => (GridLength)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        /// <summary>
        /// 设置栅格let集的动画
        /// </summary>
        /// <param name="defaultOriginValue">要设置动画的原始值</param>
        /// <param name="defaultDestinationValue">终值</param>
        /// <param name="animationClock">动画时钟（计时器）</param>
        /// <returns>返回要设置的新网格长度</returns>
        public override object GetCurrentValue(object defaultOriginValue,
          object defaultDestinationValue, AnimationClock animationClock)
        {
            var fromVal = ((GridLength)GetValue(FromProperty)).Value;
            var toVal = ((GridLength)GetValue(ToProperty)).Value;
            return fromVal > toVal
                ? new GridLength(((1 - animationClock.CurrentProgress.Value) * (fromVal - toVal)) + toVal, GridUnitType.Star)
                : (object)new GridLength((animationClock.CurrentProgress.Value * (toVal - fromVal)) + fromVal, GridUnitType.Star);
        }

    }
}
