using Labelman8.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Labelman8
{
  public partial class AddDeviceMarkingTypeWindow : StorableWindow
  {
    protected override string WindowRegistryName => "AddDeviceMarkingTypeWindow";

    public MarkingType Result { get; private set; }

    public AddDeviceMarkingTypeWindow()
    {
      InitializeComponent();
      this.Loaded += (s, e) => UpdatePreview();
    }

    private void UpdatePreview()
    {
      try
      {
        if (cmbFont == null || previewTextBlock == null || stickerBorder == null) return;

        // Шрифт
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
        previewTextBlock.FontFamily = new FontFamily(fontName);

        // Размер шрифта
        if (double.TryParse(txtFontSize.Text, out double fontSize) && fontSize > 0)
        {
          previewTextBlock.FontSize = fontSize;
        }

        // Текст
        previewTextBlock.Text = string.IsNullOrWhiteSpace(txtPreviewText.Text)
            ? "Образец текста"
            : txtPreviewText.Text;

        // Размеры наклейки
        if (double.TryParse(txtWidth.Text, out double widthMM) && widthMM > 0 &&
            double.TryParse(txtHeight.Text, out double heightMM) && heightMM > 0)
        {
          // Используем постоянный масштаб 5, чтобы наклейка была видна
          double scale = 5.0;

          // Ограничиваем максимальный размер, чтобы не вылезать за пределы окна
          double maxWidth = 400;
          double maxHeight = 200;

          double scaledWidth = widthMM * scale;
          double scaledHeight = heightMM * scale;

          if (scaledWidth > maxWidth || scaledHeight > maxHeight)
          {
            double scaleX = maxWidth / widthMM;
            double scaleY = maxHeight / heightMM;
            scale = Math.Min(scaleX, scaleY);
          }

          stickerBorder.Width = widthMM * scale;
          stickerBorder.Height = heightMM * scale;
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"Ошибка обновления предпросмотра: {ex.Message}");
      }
    }

    private void Txt_TextChanged(object sender, TextChangedEventArgs e)
    {
      UpdatePreview();
    }

    private void CmbFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      UpdatePreview();
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
  }
}