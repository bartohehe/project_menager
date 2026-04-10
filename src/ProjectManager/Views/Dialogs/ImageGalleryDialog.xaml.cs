using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ProjectManager.Views.Dialogs;

public partial class ImageGalleryDialog : Window
{
    private readonly List<byte[]> _images;

    public ImageGalleryDialog(List<byte[]> images, int startIndex = 0)
    {
        InitializeComponent();
        _images = images;
        ThumbnailList.ItemsSource = _images;

        if (_images.Count > 0)
            ShowImage(startIndex);
    }

    private void ShowImage(int index)
    {
        if (index < 0 || index >= _images.Count) return;

        var image = new BitmapImage();
        using var stream = new MemoryStream(_images[index]);
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();
        image.Freeze();
        MainImage.Source = image;
    }

    private void OnThumbnailClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button { DataContext: byte[] imageData })
        {
            var index = _images.IndexOf(imageData);
            ShowImage(index);
        }
    }

    private void OnClose(object sender, RoutedEventArgs e) => Close();
}
