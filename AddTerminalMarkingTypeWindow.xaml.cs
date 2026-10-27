using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Labelman8.Helpers;
using Labelman8.Models;

namespace Labelman8
{
  public partial class AddTerminalMarkingTypeWindow : StorableWindow
  {
    protected override string WindowRegistryName => "AddTerminalMarkingTypeWindow";

    public TerminalMarkingType Result { get; private set; }
        
    public AddTerminalMarkingTypeWindow()
    {
      InitializeComponent();
      this.Loaded += (s, e) => RenderPreview();
      this.SizeChanged += (s, e) => RenderPreview();
    }

    /// <summary>
    /// Конструктор для редактирования существующего вида
    /// </summary>
    public AddTerminalMarkingTypeWindow(TerminalMarkingType existingType) : this()
    {
      this.Title = "Изменить вид маркировки для клемм";

      // Заполняем поля
      txtName.Text = existingType.Name;
      txtHeight.Text = existingType.HeightMM.ToString();
      txtPitch.Text = existingType.PitchMM.ToString();
    }

    private void Txt_TextChanged(object sender, TextChangedEventArgs e)
    {
      RenderPreview();
    }

    private void RenderPreview()
    {
      try
      {
        if (stickerCanvas == null || previewContainer == null) return;

        stickerCanvas.Children.Clear();

        // --- 1. Получаем параметры ---
        if (!double.TryParse(txtHeight.Text, out double heightMM) || heightMM <= 0 ||
            !double.TryParse(txtPitch.Text, out double pitchMM) || pitchMM <= 0)
        {
          return;
        }

        string name = string.IsNullOrWhiteSpace(txtName.Text) ? "1X5" : txtName.Text;

        // Ширина = шаг × 10
        double widthMM = pitchMM * 10;

        // --- 2. Переводим мм в DIP ---
        double widthDip = DpiHelper.MmToDip(widthMM);
        double heightDip = DpiHelper.MmToDip(heightMM);

        if (widthDip <= 0 || heightDip <= 0) return;

        // --- 3. Настраиваем Canvas ---
        stickerCanvas.Width = widthDip;
        stickerCanvas.Height = heightDip;

        // --- 4. Рамка ---
        var border = new Rectangle
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

        // --- 5. Горизонтальная линия, разделяющая на 2 части ---
        double dividerY = heightDip * 0.5;
        var divider = new Line
        {
          X1 = 0,
          Y1 = dividerY,
          X2 = widthDip,
          Y2 = dividerY,
          Stroke = Brushes.Black,
          StrokeThickness = 1
        };
        stickerCanvas.Children.Add(divider);

        // --- 6. Вертикальные перегородки между цифрами (в нижней половине) ---
        double cellWidth = widthDip / 10;

        for (int i = 1; i < 10; i++)
        {
          double x = i * cellWidth;
          var verticalLine = new Line
          {
            X1 = x,
            Y1 = dividerY,
            X2 = x,
            Y2 = heightDip,
            Stroke = Brushes.Black,
            StrokeThickness = 1
          };
          stickerCanvas.Children.Add(verticalLine);
        }

        // --- 7. Верхняя часть: название клеммника (по центру) ---
        double topFontSize = heightDip * 0.35;
        var nameText = new TextBlock
        {
          Text = name,
          FontFamily = new FontFamily("Arial"),
          FontSize = topFontSize,
          Foreground = Brushes.Black,
          TextAlignment = TextAlignment.Center
        };
        nameText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        double nameX = (widthDip - nameText.DesiredSize.Width) / 2;
        double nameY = (dividerY - nameText.DesiredSize.Height) / 2;
        Canvas.SetLeft(nameText, nameX);
        Canvas.SetTop(nameText, nameY);
        stickerCanvas.Children.Add(nameText);

        // --- 8. Нижняя часть: цифры от 1 до 10 ---
        double bottomFontSize = heightDip * 0.35;

        for (int i = 1; i <= 10; i++)
        {
          var numberText = new TextBlock
          {
            Text = i.ToString(),
            FontFamily = new FontFamily("Arial"),
            FontSize = bottomFontSize,
            Foreground = Brushes.Black,
            TextAlignment = TextAlignment.Center
          };
          numberText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

          double cellX = (i - 1) * cellWidth;
          double numberX = cellX + (cellWidth - numberText.DesiredSize.Width) / 2;
          double numberY = dividerY + (dividerY - numberText.DesiredSize.Height) / 2;

          Canvas.SetLeft(numberText, numberX);
          Canvas.SetTop(numberText, numberY);
          stickerCanvas.Children.Add(numberText);
        }

        // --- 9. Масштабируем Canvas, чтобы он поместился в previewContainer ---
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

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(txtName.Text))
      {
        MessageBox.Show("Введите название.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        txtName.Focus();
        return;
      }

      if (!double.TryParse(txtHeight.Text, out double height) || height <= 0)
      {
        MessageBox.Show("Введите корректную высоту (положительное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        txtHeight.Focus();
        return;
      }

      if (!double.TryParse(txtPitch.Text, out double pitch) || pitch <= 0)
      {
        MessageBox.Show("Введите корректный шаг (положительное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        txtPitch.Focus();
        return;
      }

      Result = new TerminalMarkingType
      {
        Name = txtName.Text.Trim(),
        HeightMM = height,
        PitchMM = pitch
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