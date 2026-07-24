using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Printing;
using System.Windows.Documents;
using Labelman8.Models;

namespace Labelman8
{
  public partial class PrintPreviewWindow : Window
  {
    private List<Switchboard> items;
    private List<RenderTargetBitmap> pages = new List<RenderTargetBitmap>();
    private int currentPage = 0;

    // Константы для предпросмотра (используются для рендеринга в 96 DPI)
    private const double PREVIEW_DPI = 96;

    public PrintPreviewWindow(List<Switchboard> items)
    {
      InitializeComponent();
      this.items = items;
      txtStatus.Text = $"Всего элементов: {items.Count}";
      GeneratePages();
      ShowPage(0);
    }

    // ======================================================================
    // 1. Генерация страниц для предпросмотра (96 DPI)
    // ======================================================================

    private void GeneratePages()
    {
      pages.Clear();
      if (items == null || items.Count == 0) return;

      int cols = 2;
      int rows = 7;
      int itemsPerPage = cols * rows;
      int totalPages = (int)Math.Ceiling((double)items.Count / itemsPerPage);

      for (int page = 0; page < totalPages; page++)
      {
        pages.Add(RenderPreviewPage(page));
      }
    }

    private RenderTargetBitmap RenderPreviewPage(int pageIndex)
    {
      var visual = new DrawingVisual();
      using (var dc = visual.RenderOpen())
      {
        // Рисуем содержимое страницы без смещения (для предпросмотра)
        DrawPageContent(dc, pageIndex, 0, 0);
      }

      var bitmap = new RenderTargetBitmap(793, 1122, PREVIEW_DPI, PREVIEW_DPI, PixelFormats.Pbgra32);
      bitmap.Render(visual);
      return bitmap;
    }

    // ======================================================================
    // 2. Единый метод рисования содержимого страницы (в WPF-единицах 793x1122)
    // ======================================================================

    /// <summary>
    /// Рисует содержимое одной страницы на DrawingContext в WPF-единицах (793x1122)
    /// </summary>
    private void DrawPageContent(DrawingContext dc, int pageIndex, double offsetX, double offsetY)
    {
      int cols = 2;
      int rows = 7;
      int startIndex = pageIndex * cols * rows;
      int endIndex = Math.Min(startIndex + cols * rows, items.Count);
      int countOnPage = endIndex - startIndex;

      // Параметры расположения (в WPF-единицах, 1/96 дюйма)
      double pageWidth = 793;
      double pageHeight = 1122;
      double frameWidth = 322;     // 85 мм
      double frameHeight = 151;    // 40 мм
      double gap = 2;              // 0.5 мм
      // double headerHeight = 50;    // резерв для заголовка (не используется)

      double horOffset = (pageWidth - (2 * frameWidth + gap)) / 2;
      double verOffset = 5;

      // Белый фон
      dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, pageWidth, pageHeight));
      var pen = new Pen(Brushes.Black, 0.5);
      dc.DrawRectangle(null, pen, new Rect(0, 0, pageWidth, pageHeight));

      for (int i = 0; i < countOnPage; i++)
      {
        int index = startIndex + i;
        int col = i % cols;
        int row = i / cols;

        double x = offsetX + horOffset + col * (frameWidth + gap);
        double y = offsetY + verOffset + row * (frameHeight + gap);

        DrawFrameContent(dc, x, y, frameWidth, frameHeight, items[index]);
      }
    }

    /// <summary>
    /// Рисует одну рамку с данными (в WPF-единицах)
    /// </summary>
    private void DrawFrameContent(DrawingContext dc, double x, double y, double width, double height, Switchboard item)
    {
      var pen = new Pen(Brushes.Black, 0.5);
      dc.DrawRectangle(null, pen, new Rect(x, y, width, height));

      var fontNormal = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      var fontLogo = new Typeface(new FontFamily("Times New Roman"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
      var brush = Brushes.Black;

      // Размеры шрифтов и отступы (в WPF-единицах)
      double fontSize = 12;
      double fontSizeLogo = 24;
      double fontSizeAdr = 8;
      double fontSizeCont = 8;

      double offset0 = 4;  // отступ сверху для "ЗЭО"
      double offset1 = 16;   // отступ сверху для заголовка
      double offset2 = 6;   // отступ слева для "ЗЭО"
      double offset3 = 62;  // отступ сверху для основного текста (после "ЗЭО")
      double offset4 = 126; // отступ сверху для адреса
      double offset5 = 138; // отступ сверху для сайта/телефона
      double offset6 = 12;   // отступ справа для адреса

      // Заголовок компании (центрирован)
      DrawCenteredTextContent(dc, "ООО «ЗЭО»\nЗавод электрощитового\nоборудования",
          x + width / 2, y + offset1, fontNormal, fontSize, brush);

      // Логотип "ЗЭО" (слева)
      DrawTextContent(dc, "ЗЭО", x + offset2, y + offset0, fontLogo, fontSizeLogo, brush, TextAlignment.Left);

      // Основной текст (центрирован)
      string mainText = $"{item.Function}\n{item.Name}\nЭлектропитание ≈{item.Um}В (±10)% ({item.Fn}±1)Гц\n Зав.№ {item.SerialNumber}    Номинальный ток {item.In}А";
      DrawCenteredTextContent(dc, mainText, x + width / 2, y + offset3, fontNormal, fontSize, brush);

      // Адрес (выравнивание вправо)
      DrawTextContent(dc, "Г. Подольск, ул. Б. Серпуховская, д. 55",
          x + width - offset6, y + offset4, fontNormal, fontSizeAdr, brush, TextAlignment.Right);

      // Сайт и телефон (центрирован)
      DrawCenteredTextContent(dc, "www.zavod-eo.ru                                          +7-495-774-07-23",
          x + width / 2, y + offset5, fontNormal, fontSizeCont, brush);
    }

    // ======================================================================
    // 3. Вспомогательные методы для рисования текста
    // ======================================================================

    private void DrawTextContent(DrawingContext dc, string text, double x, double y, Typeface font, double fontSize, Brush brush, TextAlignment alignment = TextAlignment.Left)
    {
      if (string.IsNullOrEmpty(text)) return;
      var ft = new FormattedText(text, System.Globalization.CultureInfo.InvariantCulture,
          FlowDirection.LeftToRight, font, fontSize, brush, 96);
      ft.TextAlignment = alignment;
      dc.DrawText(ft, new Point(x, y));
    }

    private void DrawCenteredTextContent(DrawingContext dc, string text, double centerX, double y, Typeface font, double fontSize, Brush brush)
    {
      DrawTextContent(dc, text, centerX, y, font, fontSize, brush, TextAlignment.Center);
    }

    // ======================================================================
    // 4. Печать (векторная, зеркальная, с учётом полей принтера)
    // ======================================================================

    private void PrintAllPages()
    {
      if (items == null || items.Count == 0)
      {
        MessageBox.Show("Нет данных для печати.", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
        return;
      }

      try
      {
        var printDialog = new PrintDialog();
        printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
        printDialog.PrintTicket.PageOrientation = System.Printing.PageOrientation.Portrait;
        printDialog.PrintTicket.OutputQuality = OutputQuality.High;

        if (printDialog.ShowDialog() == true)
        {
          // Получаем поля принтера
          var capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
          var imageableArea = capabilities.PageImageableArea;

          double originX = imageableArea.OriginWidth;   // левое поле
          double originY = imageableArea.OriginHeight;  // верхнее поле

          int cols = 2;
          int rows = 7;
          int itemsPerPage = cols * rows;
          int totalPages = (int)Math.Ceiling((double)items.Count / itemsPerPage);

          for (int page = 0; page < totalPages; page++)
          {
            // Рисуем содержимое со смещением на поля принтера
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
              DrawPageContent(dc, page, originX, originY);
            }

            // Зеркальное отражение по горизонтали
            var mirroredVisual = new DrawingVisual();
            using (var dc = mirroredVisual.RenderOpen())
            {
              double centerX = originX + (793 - originX * 2) / 2;
              double centerY = originY + (1122 - originY * 2) / 2;
              var transform = new ScaleTransform(-1, 1, centerX, centerY);
              dc.PushTransform(transform);
              dc.DrawDrawing(visual.Drawing);
              dc.Pop();
            }

            printDialog.PrintVisual(mirroredVisual, $"Страница {page + 1}");
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Ошибка при печати:\n{ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    // ======================================================================
    // 5. Навигация по страницам
    // ======================================================================

    private void ShowPage(int index)
    {
      if (pages.Count == 0) return;
      currentPage = Math.Max(0, Math.Min(index, pages.Count - 1));
      pageImage.Source = pages[currentPage];
      txtPageInfo.Text = $"Страница {currentPage + 1} из {pages.Count}";
    }

    private void BtnPrev_Click(object sender, RoutedEventArgs e)
    {
      if (currentPage > 0) ShowPage(currentPage - 1);
    }

    private void BtnNext_Click(object sender, RoutedEventArgs e)
    {
      if (currentPage < pages.Count - 1) ShowPage(currentPage + 1);
    }

    private void BtnPrint_Click(object sender, RoutedEventArgs e)
    {
      PrintAllPages();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }
  }
}