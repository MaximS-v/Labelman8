using Labelman8.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Labelman8
{
  public partial class PackagingPreviewWindow : StorableWindow
  {
		protected override string WindowRegistryName => "PackagingPreview";
		private List<Switchboard> items;
    private List<RenderTargetBitmap> pages = new List<RenderTargetBitmap>();
    private int currentPage = 0;
    private const double PREVIEW_DPI = 96;

    // Размеры в WPF-единицах (1/96 дюйма)
    private const double PAGE_WIDTH = 793;
    private const double PAGE_HEIGHT = 1122;
    private const double FRAME_WIDTH = 700;      // 185 мм
    private const double FRAME_HEIGHT = 113;     // 30 мм
    private const double GAP = 2;                // 0.5 мм

    private const int COLS = 1;
    private const int ROWS = 9;

    public PackagingPreviewWindow(List<Switchboard> items)
    {
      InitializeComponent();
      this.items = items;
      txtStatus.Text = $"Всего элементов: {items.Count}";
      GeneratePages();
      ShowPage(0);
    }

    private void GeneratePages()
    {
      pages.Clear();
      if (items == null || items.Count == 0) return;

      int itemsPerPage = COLS * ROWS;
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
        DrawPageContent(dc, pageIndex, 0, 0, isPrinting: false);
      }

      var bitmap = new RenderTargetBitmap((int)PAGE_WIDTH, (int)PAGE_HEIGHT, PREVIEW_DPI, PREVIEW_DPI, PixelFormats.Pbgra32);
      bitmap.Render(visual);
      return bitmap;
    }

    private void DrawPageContent(DrawingContext dc, int pageIndex, double offsetX, double offsetY, bool isPrinting = false)
    {
      int startIndex = pageIndex * COLS * ROWS;
      int endIndex = Math.Min(startIndex + COLS * ROWS, items.Count);
      int countOnPage = endIndex - startIndex;

      // Размеры области, в которой центрируем
      double containerWidth = isPrinting ? PAGE_WIDTH - offsetX * 2 : PAGE_WIDTH;
      double containerHeight = isPrinting ? PAGE_HEIGHT - offsetY * 2 : PAGE_HEIGHT;

      // Вычисляем отступы для центрирования внутри контейнера
      double totalWidth = COLS * FRAME_WIDTH + (COLS - 1) * GAP;
      double totalHeight = ROWS * FRAME_HEIGHT + (ROWS - 1) * GAP;
      double horOffset = (containerWidth - totalWidth) / 2;
      double verOffset = (containerHeight - totalHeight) / 2;

      // Фон: жёлтый для предпросмотра, белый для печати
      Brush backgroundBrush = isPrinting ? Brushes.White : Brushes.Yellow;
      dc.DrawRectangle(backgroundBrush, null, new Rect(0, 0, PAGE_WIDTH, PAGE_HEIGHT));

      // Если печать, добавляем смещение на поля
      double startX = isPrinting ? offsetX : 0;
      double startY = isPrinting ? offsetY : 0;

      for (int i = 0; i < countOnPage; i++)
      {
        int index = startIndex + i;
        int col = i % COLS;
        int row = i / COLS;

        double x = startX + horOffset + col * (FRAME_WIDTH + GAP);
        double y = startY + verOffset + row * (FRAME_HEIGHT + GAP);

        DrawFrameContent(dc, x, y, FRAME_WIDTH, FRAME_HEIGHT, items[index]);
      }
    }

    private void DrawFrameContent(DrawingContext dc, double x, double y, double width, double height, Switchboard item)
    {
      var pen = new Pen(Brushes.Black, 0.5);
      dc.DrawRectangle(null, pen, new Rect(x, y, width, height));

      var fontNormal = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      var brush = Brushes.Black;

      double padding = 4;
      double maxFontSize = 100;

      // Определяем максимальный размер шрифта для каждой части
      double serialWidth = MeasureTextWidth(item.SerialNumber, fontNormal, maxFontSize);
      double nameWidth = MeasureTextWidth(item.Name, fontNormal, maxFontSize);

      // Подбираем размер шрифта, чтобы текст поместился в рамку
      double availableWidth = width - padding * 2;
      double fontSize = maxFontSize;

      // Уменьшаем размер, пока текст не поместится по ширине
      while (fontSize > 6)
      {
        double currentSerialWidth = MeasureTextWidth(item.SerialNumber, fontNormal, fontSize);
        double currentNameWidth = MeasureTextWidth(item.Name, fontNormal, fontSize);

        if (currentSerialWidth + currentNameWidth + padding * 2 <= availableWidth)
        {
          break;
        }
        fontSize -= 1;
      }

      // Заводской номер (центрирован по вертикали, прижат к левому краю)
      double serialX = x + padding;
      double yCenter = y + height / 2;

      var serialFormatted = new FormattedText(
          item.SerialNumber,
          System.Globalization.CultureInfo.InvariantCulture,
          FlowDirection.LeftToRight,
          fontNormal,
          fontSize,
          brush,
          96);

      double serialY = yCenter - serialFormatted.Height / 2;
      dc.DrawText(serialFormatted, new Point(serialX, serialY));

      // Наименование (центрирован по вертикали, прижат к правому краю)
      var nameFormatted = new FormattedText(
          item.Name,
          System.Globalization.CultureInfo.InvariantCulture,
          FlowDirection.LeftToRight,
          fontNormal,
          fontSize,
          brush,
          96);

      double nameX = x + width - padding - nameFormatted.Width;
      double nameY = yCenter - nameFormatted.Height / 2;

      dc.DrawText(nameFormatted, new Point(nameX, nameY));
    }

    private double MeasureTextWidth(string text, Typeface font, double fontSize)
    {
      if (string.IsNullOrEmpty(text)) return 0;
      var ft = new FormattedText(
          text,
          System.Globalization.CultureInfo.InvariantCulture,
          FlowDirection.LeftToRight,
          font,
          fontSize,
          Brushes.Black,
          96);
      return ft.Width;
    }

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
          var capabilities = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
          var imageableArea = capabilities.PageImageableArea;

          double originX = imageableArea.OriginWidth;
          double originY = imageableArea.OriginHeight;

          int itemsPerPage = COLS * ROWS;
          int totalPages = (int)Math.Ceiling((double)items.Count / itemsPerPage);

          var document = new FixedDocument();

          for (int page = 0; page < totalPages; page++)
          {
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
              DrawPageContent(dc, page, originX, originY, isPrinting: true);
            }

            var pageContent = new PageContent();
            var fixedPage = new FixedPage();
            fixedPage.Width = PAGE_WIDTH;
            fixedPage.Height = PAGE_HEIGHT;

            var image = new Image
            {
              Source = new RenderTargetBitmap((int)PAGE_WIDTH, (int)PAGE_HEIGHT, 96, 96, PixelFormats.Pbgra32)
            };
            (image.Source as RenderTargetBitmap).Render(visual);

            fixedPage.Children.Add(image);
            ((IAddChild)pageContent).AddChild(fixedPage);
            document.Pages.Add(pageContent);
          }

          printDialog.PrintDocument(document.DocumentPaginator, "Печать упаковки");
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Ошибка при печати:\n{ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

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