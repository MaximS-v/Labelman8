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

    // -------- Константы для предпросмотра (96 DPI) --------
    private const int PREVIEW_A4_WIDTH = 793;
    private const int PREVIEW_A4_HEIGHT = 1122;
    private const int PREVIEW_FRAME_WIDTH = 322;
    private const int PREVIEW_FRAME_HEIGHT = 151;
    private const int PREVIEW_GAP = 2;
    private const int PREVIEW_HEADER_HEIGHT = 50;
    private const int PREVIEW_HORIZONTAL_OFFSET = (PREVIEW_A4_WIDTH - (2 * PREVIEW_FRAME_WIDTH + PREVIEW_GAP)) / 2;
    private const int PREVIEW_VERTICAL_OFFSET = (PREVIEW_A4_HEIGHT - PREVIEW_HEADER_HEIGHT - (7 * PREVIEW_FRAME_HEIGHT + 6 * PREVIEW_GAP)) / 2;
    private const double PREVIEW_DPI = 96;

    // -------- Константы для печати (600 DPI) --------
    private const int PRINT_A4_WIDTH = 4961;
    private const int PRINT_A4_HEIGHT = 7016;
    private const int PRINT_FRAME_WIDTH = 2008;
    private const int PRINT_FRAME_HEIGHT = 945;
    private const int PRINT_GAP = 12;
    private const int PRINT_HEADER_HEIGHT = 300;
    private const int PRINT_HORIZONTAL_OFFSET = (PRINT_A4_WIDTH - (2 * PRINT_FRAME_WIDTH + PRINT_GAP)) / 2;
    private const int PRINT_VERTICAL_OFFSET = (PRINT_A4_HEIGHT - PRINT_HEADER_HEIGHT - (7 * PRINT_FRAME_HEIGHT + 6 * PRINT_GAP)) / 2;
    private const double PRINT_DPI = 600;

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
        pages.Add(RenderPreviewPage(page, itemsPerPage, cols, rows));
      }
    }

    private RenderTargetBitmap RenderPreviewPage(int pageIndex, int itemsPerPage, int cols, int rows)
    {
      int startIndex = pageIndex * itemsPerPage;
      int endIndex = Math.Min(startIndex + itemsPerPage, items.Count);
      int countOnPage = endIndex - startIndex;

      var visual = new DrawingVisual();
      using (var dc = visual.RenderOpen())
      {
        dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, PREVIEW_A4_WIDTH, PREVIEW_A4_HEIGHT));
        var pen = new Pen(Brushes.Black, 1);
        dc.DrawRectangle(null, pen, new Rect(0, 0, PREVIEW_A4_WIDTH, PREVIEW_A4_HEIGHT));

        for (int i = 0; i < countOnPage; i++)
        {
          int index = startIndex + i;
          int col = i % cols;
          int row = i / cols;

          double x = PREVIEW_HORIZONTAL_OFFSET + col * (PREVIEW_FRAME_WIDTH + PREVIEW_GAP);
          double y = PREVIEW_VERTICAL_OFFSET + row * (PREVIEW_FRAME_HEIGHT + PREVIEW_GAP);

          DrawPreviewFrame(dc, x, y, items[index]);
        }
      }

      var bitmap = new RenderTargetBitmap(PREVIEW_A4_WIDTH, PREVIEW_A4_HEIGHT, PREVIEW_DPI, PREVIEW_DPI, PixelFormats.Pbgra32);
      bitmap.Render(visual);
      return bitmap;
    }

    private void DrawPreviewFrame(DrawingContext dc, double x, double y, Switchboard item)
    {
      var pen = new Pen(Brushes.Black, 1);
      dc.DrawRectangle(null, pen, new Rect(x, y, PREVIEW_FRAME_WIDTH, PREVIEW_FRAME_HEIGHT));

      var fontNormal = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      var fontLogo = new Typeface(new FontFamily("Times New Roman"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
      var brush = Brushes.Black;

      DrawCenteredText(dc, "ООО «ЗЭО»\nЗавод электрощитового\nоборудования",
          x + PREVIEW_FRAME_WIDTH / 2, y + 2, fontNormal, 12, brush);

      DrawText(dc, "ЗЭО", x + 6, y + 2, fontLogo, 24, brush, TextAlignment.Left);

      string mainText = $"{item.Function}\n{item.Name}\nЭлектропитание ≈{item.Um}В (±10)% ({item.Fn}±1)Гц\n Зав.№ {item.SerialNumber}    Номинальный ток {item.In}А";
      DrawCenteredText(dc, mainText, x + PREVIEW_FRAME_WIDTH / 2, y + 48, fontNormal, 12, brush);

      DrawText(dc, "Г. Подольск, ул. Б. Серпуховская, д. 55",
          x + PREVIEW_FRAME_WIDTH - 6, y + 112, fontNormal, 8, brush, TextAlignment.Right);

      DrawCenteredText(dc, "www.zavod-eo.ru                                          +7-495-774-07-23",
          x + PREVIEW_FRAME_WIDTH / 2, y + 124, fontNormal, 8, brush);
    }

    // ======================================================================
    // 2. Вспомогательные методы для рисования текста (предпросмотр)
    // ======================================================================

    private void DrawText(DrawingContext dc, string text, double x, double y, Typeface font, double fontSize, Brush brush, TextAlignment alignment = TextAlignment.Left)
    {
      if (string.IsNullOrEmpty(text)) return;
      var ft = new FormattedText(text, System.Globalization.CultureInfo.InvariantCulture,
          FlowDirection.LeftToRight, font, fontSize, brush, PREVIEW_DPI);
      ft.TextAlignment = alignment;
      dc.DrawText(ft, new Point(x, y));
    }

    private void DrawCenteredText(DrawingContext dc, string text, double centerX, double y, Typeface font, double fontSize, Brush brush)
    {
      DrawText(dc, text, centerX, y, font, fontSize, brush, TextAlignment.Center);
    }

    // ======================================================================
    // 3. Векторная печать (без растровых изображений)
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
          // Получаем поля принтера (непечатаемые области)
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
            // Генерируем векторную страницу с учётом полей
            var visual = RenderPrintPageVector(page, originX, originY);
            // Зеркальное отражение
            var mirroredVisual = new DrawingVisual();
            using (var dc = mirroredVisual.RenderOpen())
            {
              // Зеркалим относительно центра печатаемой области (с учётом полей)
              double centerX = originX + (793 - originX * 2) / 2;
              double centerY = originY + (1122 - originY * 2) / 2;
              var transform = new ScaleTransform(-1, 1, centerX, centerY);
              dc.PushTransform(transform);
              dc.DrawDrawing(visual.Drawing);
              dc.Pop();
            }
            // Печать
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

    private DrawingVisual RenderPrintPageVector(int pageIndex, double offsetX, double offsetY)
    {
      int cols = 2;
      int rows = 7;
      int startIndex = pageIndex * cols * rows;
      int endIndex = Math.Min(startIndex + cols * rows, items.Count);
      int countOnPage = endIndex - startIndex;

      const double scale = 96.0 / 600.0;
      double pageWidth = 793;
      double pageHeight = 1122;

      // Масштабируем все константы
      double frameWidth = PRINT_FRAME_WIDTH * scale;
      double frameHeight = PRINT_FRAME_HEIGHT * scale;
      double gap = PRINT_GAP * scale;
      double headerHeight = PRINT_HEADER_HEIGHT * scale;
      double horOffset = (pageWidth - (2 * frameWidth + gap)) / 2;
      double verOffset = (pageHeight - headerHeight - (7 * frameHeight + 6 * gap)) / 2;

      // Добавляем смещение на поля принтера
      double offsetX_total = offsetX;
      double offsetY_total = offsetY;

      var visual = new DrawingVisual();
      using (var dc = visual.RenderOpen())
      {
        // Белый фон на всю страницу
        dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, pageWidth, pageHeight));
        var pen = new Pen(Brushes.Black, 0.5);
        dc.DrawRectangle(null, pen, new Rect(0, 0, pageWidth, pageHeight));

        for (int i = 0; i < countOnPage; i++)
        {
          int index = startIndex + i;
          int col = i % cols;
          int row = i / cols;

          double x = offsetX_total + horOffset + col * (frameWidth + gap);
          double y = offsetY_total + verOffset + row * (frameHeight + gap);

          DrawPrintFrame(dc, x, y, frameWidth, frameHeight, scale, items[index]);
        }
      }
      return visual;
    }

    private void DrawPrintFrame(DrawingContext dc, double x, double y, double width, double height, double scale, Switchboard item)
    {
      var pen = new Pen(Brushes.Black, 0.5);
      dc.DrawRectangle(null, pen, new Rect(x, y, width, height));

      var fontNormal = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      var fontLogo = new Typeface(new FontFamily("Times New Roman"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
      var brush = Brushes.Black;

      // Масштабируем размеры шрифтов и отступы
      double fontSize = 72 * scale;
      double fontSizeLogo = 160 * scale;
      double fontSizeAdr = 48 * scale;
      double fontSizeCont = 48 * scale;

      double offset1 = 12 * scale;
      double offset2 = 40 * scale;
      double offset3 = 300 * scale;
      double offset4 = 700 * scale;
      double offset5 = 780 * scale;
      double offset6 = 6 * scale;

      DrawCenteredTextPrint(dc, "ООО «ЗЭО»\nЗавод электрощитового\nоборудования",
          x + width / 2, y + offset1, fontNormal, fontSize, brush);

      DrawTextPrint(dc, "ЗЭО", x + offset2, y + offset1, fontLogo, fontSizeLogo, brush, TextAlignment.Left);

      string mainText = $"{item.Function}\n{item.Name}\nЭлектропитание ≈{item.Um}В (±10)% ({item.Fn}±1)Гц\n Зав.№ {item.SerialNumber}    Номинальный ток {item.In}А";
      DrawCenteredTextPrint(dc, mainText, x + width / 2, y + offset3, fontNormal, fontSize, brush);

      DrawTextPrint(dc, "Г. Подольск, ул. Б. Серпуховская, д. 55",
          x + width - offset6, y + offset4, fontNormal, fontSizeAdr, brush, TextAlignment.Right);

      DrawCenteredTextPrint(dc, "www.zavod-eo.ru                                          +7-495-774-07-23",
          x + width / 2, y + offset5, fontNormal, fontSizeCont, brush);
    }

    private void DrawTextPrint(DrawingContext dc, string text, double x, double y, Typeface font, double fontSize, Brush brush, TextAlignment alignment = TextAlignment.Left)
    {
      if (string.IsNullOrEmpty(text)) return;
      var ft = new FormattedText(text, System.Globalization.CultureInfo.InvariantCulture,
          FlowDirection.LeftToRight, font, fontSize, brush, 96);
      ft.TextAlignment = alignment;
      dc.DrawText(ft, new Point(x, y));
    }

    private void DrawCenteredTextPrint(DrawingContext dc, string text, double centerX, double y, Typeface font, double fontSize, Brush brush)
    {
      DrawTextPrint(dc, text, centerX, y, font, fontSize, brush, TextAlignment.Center);
    }

    // ======================================================================
    // 4. Навигация по страницам
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