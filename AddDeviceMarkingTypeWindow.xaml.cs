using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Labelman8.Models;
using Labelman8.Helpers;

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
        if (stickerCanvas == null || previewContainer == null) return;

        stickerCanvas.Children.Clear();

        if (!double.TryParse(txtWidth.Text, out double widthMM) || widthMM <= 0 ||
            !double.TryParse(txtHeight.Text, out double heightMM) || heightMM <= 0)
        {
          return;
        }

        string fontName = "Arial";
        if (cmbFont.SelectedItem != null)
        {
          if (cmbFont.SelectedItem is ComboBoxItem item && item.Content != null)
            fontName = item.Content.ToString();
          else
            fontName = cmbFont.SelectedItem.ToString();
        }

        double fontSize = 36;
        if (double.TryParse(txtFontSize.Text, out double parsedSize) && parsedSize > 0)
          fontSize = parsedSize;

        string text = string.IsNullOrWhiteSpace(txtPreviewText.Text) ? "Текст" : txtPreviewText.Text;

        // Переводим мм в DIP
        double widthDip = DpiHelper.MmToDip(widthMM);
        double heightDip = DpiHelper.MmToDip(heightMM);

        if (widthDip <= 0 || heightDip <= 0) return;

        // Задаём размер Canvas в DIP
        stickerCanvas.Width = widthDip;
        stickerCanvas.Height = heightDip;

        // Рамка
        var border = new System.Windows.Shapes.Rectangle
        {
          Width = widthDip,
          Height = heightDip,
          Stroke = Brushes.Black,
          StrokeThickness = 1,
          Fill = Brushes.White
        };
        Canvas.SetLeft(border, 0);
        Canvas.SetTop(border, 0);
        stickerCanvas.Children.Add(border);

        // Текст
        var textBlock = new TextBlock
        {
          Text = text,
          FontFamily = new FontFamily(fontName),
          FontSize = fontSize * (96.0 / 72.0), // pt → DIP
          Foreground = Brushes.Black,
          TextWrapping = TextWrapping.NoWrap,
          TextAlignment = TextAlignment.Center
        };

        // Измеряем текст
        textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var textSize = textBlock.DesiredSize;

        // Центрируем
        double textX = (widthDip - textSize.Width) / 2;
        double textY = (heightDip - textSize.Height) / 2;
        Canvas.SetLeft(textBlock, textX);
        Canvas.SetTop(textBlock, textY);
        stickerCanvas.Children.Add(textBlock);

        // Масштабируем Canvas, чтобы он поместился в previewContainer
        double availableWidth = previewContainer.ActualWidth - 20;
        double availableHeight = previewContainer.ActualHeight - 20;

        if (availableWidth > 0 && availableHeight > 0)
        {
          double scaleX = availableWidth / widthDip;
          double scaleY = availableHeight / heightDip;
          double scale = Math.Min(scaleX, scaleY);

          stickerCanvas.LayoutTransform = new ScaleTransform(scale, scale);
        }
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