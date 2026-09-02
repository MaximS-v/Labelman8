using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Labelman8.Models;

namespace Labelman8
{
  public partial class AddDeviceMarkingTypeWindow : StorableWindow
  {
    protected override string WindowRegistryName => "AddDeviceMarkingTypeWindow";

    public MarkingType Result { get; private set; }

    public AddDeviceMarkingTypeWindow()
    {
      InitializeComponent();
      this.Loaded += (s, e) => RenderSticker();
      this.SizeChanged += (s, e) => RenderSticker();
    }

    private void RenderSticker()
    {
      try
      {
        if (stickerImage == null || stickerBorder == null || previewContainer == null) return;

        // --- 1. Получаем параметры ---
        if (!double.TryParse(txtWidth.Text, out double widthMM) || widthMM <= 0 ||
            !double.TryParse(txtHeight.Text, out double heightMM) || heightMM <= 0)
        {
          stickerImage.Source = null;
          return;
        }

        string fontName = "Arial";
        if (cmbFont.SelectedItem != null)
        {
          if (cmbFont.SelectedItem is ComboBoxItem item && item.Content != null)
          {
            fontName = item.Content.ToString();
          }
          else
          {
            fontName = cmbFont.SelectedItem.ToString();
          }
        }

        double fontSize = 36;
        if (double.TryParse(txtFontSize.Text, out double parsedSize) && parsedSize > 0)
        {
          fontSize = parsedSize;
        }

        string text = string.IsNullOrWhiteSpace(txtPreviewText.Text)
            ? "Образец текста"
            : txtPreviewText.Text;

        // --- 2. Создаём изображение наклейки в высоком разрешении ---
        const double dpi = 96;
        const double scale = 20.0; // 1 мм = 20 пикселей
        int pixelWidth = (int)(widthMM * scale);
        int pixelHeight = (int)(heightMM * scale);

        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
          // Белый фон
          dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, pixelWidth, pixelHeight));

          // Рамка
          var pen = new Pen(Brushes.Black, 1);
          dc.DrawRectangle(null, pen, new Rect(0, 0, pixelWidth, pixelHeight));

          // Текст
          if (!string.IsNullOrEmpty(text))
          {
            var font = new Typeface(new FontFamily(fontName), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                font,
                fontSize * scale,
                Brushes.Black,
                dpi);

            double x = (pixelWidth - formattedText.Width) / 2;
            double y = (pixelHeight - formattedText.Height) / 2;
            dc.DrawText(formattedText, new Point(x, y));
          }
        }

        var bitmap = new RenderTargetBitmap(pixelWidth, pixelHeight, dpi, dpi, PixelFormats.Pbgra32);
        bitmap.Render(visual);

        // --- 3. Отображаем с высоким качеством ---
        stickerImage.Source = bitmap;
        stickerImage.Width = pixelWidth;
        stickerImage.Height = pixelHeight;

        // Масштабируем для предпросмотра
        double availableWidth = previewContainer.ActualWidth - 20;
        double availableHeight = previewContainer.ActualHeight - 20;

        if (availableWidth > 0 && availableHeight > 0)
        {
          double scaleX = availableWidth / pixelWidth;
          double scaleY = availableHeight / pixelHeight;
          double viewScale = Math.Min(scaleX, scaleY);

          if (viewScale < 0.1) viewScale = 0.1;
          if (viewScale > 5.0) viewScale = 5.0;

          stickerBorder.Width = pixelWidth;
          stickerBorder.Height = pixelHeight;
          stickerBorder.LayoutTransform = new ScaleTransform(viewScale*.9, viewScale*.9);
        }

        System.Diagnostics.Debug.WriteLine($"Наклейка: {widthMM}x{heightMM} мм, размер в пикселях: {pixelWidth}x{pixelHeight}");
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"Ошибка рендеринга: {ex.Message}");
      }
    }

    private void Txt_TextChanged(object sender, TextChangedEventArgs e)
    {
      RenderSticker();
    }

    private void CmbFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      RenderSticker();
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
      // Проверяем, что все поля заполнены
      if (string.IsNullOrWhiteSpace(txtName.Text))
      {
        MessageBox.Show("Введите название.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        txtName.Focus();
        return;
      }

      if (!double.TryParse(txtWidth.Text, out double width) || width <= 0)
      {
        MessageBox.Show("Введите корректную ширину (положительное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        txtWidth.Focus();
        return;
      }

      if (!double.TryParse(txtHeight.Text, out double height) || height <= 0)
      {
        MessageBox.Show("Введите корректную высоту (положительное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        txtHeight.Focus();
        return;
      }

      if (!double.TryParse(txtFontSize.Text, out double fontSize) || fontSize <= 0)
      {
        MessageBox.Show("Введите корректный размер шрифта (положительное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        txtFontSize.Focus();
        return;
      }

      string fontName = (cmbFont.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Arial";

      Result = new MarkingType
      {
        Name = txtName.Text.Trim(),
        WidthMM = width,
        HeightMM = height,
        FontName = fontName,
        DefaultFontSizePt = fontSize,
        IsSystem = false
      };

      DialogResult = true;
      Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      RenderSticker();
    }
  }
}