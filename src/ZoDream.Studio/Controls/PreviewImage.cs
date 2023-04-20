using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.Studio.Extensions;
using ZoDream.Studio.Pages;

namespace ZoDream.Studio.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Studio.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.Studio.Controls;assembly=ZoDream.Studio.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:PreviewImage/>
    ///
    /// </summary>
    [TemplatePart(Name = ImagePanelName, Type = typeof(Canvas))]
    [TemplatePart(Name = InnerImageName, Type = typeof(Image))]
    public class PreviewImage : Control
    {
        const string ImagePanelName = "PART_ImagePanel";
        const string InnerImageName = "PART_InnerImage";
        
        static PreviewImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PreviewImage), new FrameworkPropertyMetadata(typeof(PreviewImage)));
        }

        public PreviewImage()
        {
            Loaded += PreviewImage_Loaded;
        }

        private void PreviewImage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        public object? ImageSource {
            get { return GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(object), typeof(PreviewImage), new PropertyMetadata(null, (d,s) => {
                (d as PreviewImage)?.UpdatePreview();
            }));



        public int ImageWidth {
            get { return (int)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(int), typeof(PreviewImage), new PropertyMetadata(0));



        public int ImageHeight {
            get { return (int)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(int), typeof(PreviewImage), new PropertyMetadata(0));

        private Canvas? ImagePanel;
        private Image? InnerImage;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ImagePanel = GetTemplateChild(ImagePanelName) as Canvas;
            InnerImage = GetTemplateChild(InnerImageName) as Image;
        }


        private void UpdatePreview()
        {
            if (ImagePanel is null || InnerImage is null)
            {
                return;
            }
            if ((ImageHeight == 0 || ImageWidth == 0) && ImageSource is System.Drawing.Bitmap bit)
            {
                ImageHeight = bit.Height;
                ImageWidth = bit.Width;
            }
            if (ActualWidth == 0 || ImageHeight == 0 || ImageWidth == 0)
            {
                return;
            }
            var project = App.ViewModel!.Project;
            if (project is null)
            {
                return;
            }
            var (width, height) = GetSize(ActualWidth, ActualHeight, project.ScreenWidth, project.ScreenHeight);
            ImagePanel.Width = width;
            ImagePanel.Height = height;

            var (w, h) = GetSize(width, height, ImageWidth, ImageHeight);
            InnerImage.Width = w;
            InnerImage.Height = h;
            Canvas.SetTop(InnerImage, (height - h)/ 2);
            Canvas.SetLeft(InnerImage, (width - w) / 2);
            if (ImageSource is System.Drawing.Bitmap o)
            {
                InnerImage.Source = o.ToBitmapSource();
            } else if (ImageSource is ImageSource j)
            {
                InnerImage.Source = j;
            }
        }

        private (double, double) GetSize(double maxWidth, double maxHeight, double imageWidth, double imageHeight)
        {
            var wScale = maxWidth / imageWidth;
            var hScale = maxHeight / imageHeight;
            if (wScale < hScale)
            {
                return (maxWidth, imageHeight * wScale);
            }
            else
            {
                return (imageWidth * hScale, maxHeight);
            }
        }
    }
}
