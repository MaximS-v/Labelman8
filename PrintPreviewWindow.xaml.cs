using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Labelman8.Models;

namespace Labelman8
{
  public partial class PrintPreviewWindow : Window
  {
    private List<Switchboard> items;
    private List<RenderTargetBitmap> pages = new List<RenderTargetBitmap>();
    private int currentPage = 0;

    // Размеры A4 в пикселях при 96 DPI
    private const int A4_WIDTH = 793;
    private const int A4_HEIGHT = 1122;

    // Размер рамки в пикселях при 96 DPI
    private const int FRAME_WIDTH = 322;   // 85 мм
    private const int FRAME_HEIGHT = 151;  // 40 мм

    // Зазор между рамками (0.5 мм ≈ 1.89 px → 2 px)
    private const int GAP = 2;

    // Высота заголовка
    private const int HEADER_HEIGHT = 50;

    // Вычисленные отступы
    private const int HORIZONTAL_OFFSET = (A4_WIDTH - (2 * FRAME_WIDTH + GAP)) / 2;
    private const int VERTICAL_OFFSET = (A4_HEIGHT - HEADER_HEIGHT - (7 * FRAME_HEIGHT + 6 * GAP)) / 2;

    public PrintPreviewWindow(List<Switchboard> items)
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

      if (items == null || items.Count == 0)
        return;

      int cols = 2;
      int rows = 7;
      int itemsPerPage = cols * rows;

      int totalPages = (int)Math.Ceiling((double)items.Count / itemsPerPage);

      for (int page = 0; page < totalPages; page++)
      {
        var pageBitmap = RenderPage(page, itemsPerPage, cols, rows);
        pages.Add(pageBitmap);
      }
    }

    private RenderTargetBitmap RenderPage(int pageIndex, int itemsPerPage, int cols, int rows)
    {
      int startIndex = pageIndex * itemsPerPage;
      int endIndex = Math.Min(startIndex + itemsPerPage, items.Count);
      int countOnPage = endIndex - startIndex;

      var visual = new DrawingVisual();
      using (var dc = visual.RenderOpen())
      {
        // Белый фон
        dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, A4_WIDTH, A4_HEIGHT));

        // Рамка для каждой страницы (граница листа)
        var pen = new Pen(Brushes.Black, 1);
        dc.DrawRectangle(null, pen, new Rect(0, 0, A4_WIDTH, A4_HEIGHT));

        for (int i = 0; i < countOnPage; i++)
        {
          int index = startIndex + i;
          int col = i % cols;
          int row = i / cols;

          double x = HORIZONTAL_OFFSET + col * (FRAME_WIDTH + GAP);
          double y = VERTICAL_OFFSET + row * (FRAME_HEIGHT + GAP);

          DrawFrame(dc, x, y, items[index]);
        }
      }

      var bitmap = new RenderTargetBitmap(A4_WIDTH, A4_HEIGHT, 96, 96, PixelFormats.Pbgra32);
      bitmap.Render(visual);
      return bitmap;
    }

    private void DrawFrame(DrawingContext dc, double x, double y, Switchboard item)
    {
      // Рамка
      var pen = new Pen(Brushes.Black, 1);
      dc.DrawRectangle(null, pen, new Rect(x, y, FRAME_WIDTH, FRAME_HEIGHT));

      // === Шрифты ===
      var fontNormal = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      var fontLogo = new Typeface(new FontFamily("Times New Roman"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);

      var brush = Brushes.Black;

      // === 1. Заголовок компании (вверху рамки, по центру) ===
      string companyName = "ООО «ЗЭО»\nЗавод электрощитового\nоборудования";
      double headerFontSize = 14;

      var headerText = new FormattedText(
          companyName,
          System.Globalization.CultureInfo.InvariantCulture,
          FlowDirection.LeftToRight,
          fontNormal,
          headerFontSize,
          brush,
          96)
      {
        TextAlignment = TextAlignment.Center  // ← Центрирование
      };

      double headerX = x + FRAME_WIDTH / 2; // Центрируем по ширине рамки
      double headerY = y + 2;
      dc.DrawText(headerText, new Point(headerX, headerY));

      // === 2. Надпись "ЗЭО" (левый верхний угол, удвоенный шрифт) ===
      string label = "ЗЭО";
      double labelFontSize = 32;

      var labelText = new FormattedText(
          label,
          System.Globalization.CultureInfo.InvariantCulture,
          FlowDirection.LeftToRight,
          fontLogo,
          labelFontSize,
          brush,
          96);

      double labelX = x + 8;
      double labelY = y + 2;
      dc.DrawText(labelText, new Point(labelX, labelY));

      // === 3. Основной текст с данными ===
      double textFontSize = 14;
      double textX = x + FRAME_WIDTH / 2; // Центрируем по ширине рамки
      double textY = y + 56;

      string text = $"{item.Function}\n{item.Name}\nЭлектропитание ≈{item.Um}В (±10)% ({item.Fn}±1)Гц\n Зав.№ {item.SerialNumber}    Номинальный ток {item.In}А";

      var formattedText = new FormattedText(
          text,
          System.Globalization.CultureInfo.InvariantCulture,
          FlowDirection.LeftToRight,
          fontNormal,
          textFontSize,
          brush,
          96)
      {
        TextAlignment = TextAlignment.Center  // ← Центрирование
      };

      dc.DrawText(formattedText, new Point(textX, textY));

      // === 4. Адрес ===
      double adressFontSize = 10;
      double adressX = x + FRAME_WIDTH - 8;
      double adressY = y + 122;

      string adress = "Г. Подольск, ул. Б. Серпуховская, д. 55";

      var addressText = new FormattedText(
          adress,
          System.Globalization.CultureInfo.InvariantCulture,
          FlowDirection.LeftToRight,
          fontNormal,
          adressFontSize,
          brush,
          96)
      {
        TextAlignment = TextAlignment.Right
      };

      dc.DrawText(addressText, new Point(adressX, adressY));

      // === 5. Сайт и телефон ===
      double contFontSize = 10;
      double contX = x + FRAME_WIDTH / 2; // Центрируем по ширине рамки
      double contY = y + 136;

      string cont = "www.zavod-eo.ru                                          +7-495-774-07-23";

      var contText = new FormattedText(
          cont,
          System.Globalization.CultureInfo.InvariantCulture,
          FlowDirection.LeftToRight,
          fontNormal,
          contFontSize,
          brush,
          96)
      {
        TextAlignment = TextAlignment.Center
      };

      dc.DrawText(contText, new Point(contX, contY));
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
      if (currentPage > 0)
        ShowPage(currentPage - 1);
    }

    private void BtnNext_Click(object sender, RoutedEventArgs e)
    {
      if (currentPage < pages.Count - 1)
        ShowPage(currentPage + 1);
    }

    private void BtnPrint_Click(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("Печать будет реализована позже.", "Информация",
                      MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}