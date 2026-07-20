using System;
using System.Data;
using System.IO;
using ClosedXML.Excel;  // <-- Добавить!

namespace Labelman8.Modules
{
  /// <summary>
  /// Класс для чтения данных из Excel-файлов (.xlsm, .xlsx) с использованием ClosedXML
  /// </summary>
  public class ExcelReader
  {
    /// <summary>
    /// Результат чтения Excel-файла
    /// </summary>
    public class ExcelData
    {
      /// <summary>
      /// Имя листа
      /// </summary>
      public string SheetName { get; set; }

      /// <summary>
      /// Данные в виде DataTable (строки и колонки)
      /// </summary>
      public DataTable Data { get; set; }

      /// <summary>
      /// Количество строк
      /// </summary>
      public int RowCount => Data?.Rows.Count ?? 0;

      /// <summary>
      /// Количество колонок
      /// </summary>
      public int ColumnCount => Data?.Columns.Count ?? 0;

      /// <summary>
      /// Текстовое представление данных для вывода в UI
      /// </summary>
      public string ToText(int maxRows = 20, int maxCols = 10)
      {
        if (Data == null || Data.Rows.Count == 0)
          return "Нет данных";

        string result = $"📊 Лист: {SheetName}\n";
        result += new string('-', 50) + "\n";

        // Заголовки колонок
        for (int col = 0; col < Math.Min(Data.Columns.Count, maxCols); col++)
        {
          result += $"{Data.Columns[col].ColumnName,-15}";
        }
        result += "\n";
        result += new string('-', 50) + "\n";

        // Данные
        int rowCount = Math.Min(Data.Rows.Count, maxRows);
        for (int row = 0; row < rowCount; row++)
        {
          for (int col = 0; col < Math.Min(Data.Columns.Count, maxCols); col++)
          {
            string value = Data.Rows[row][col]?.ToString() ?? "";
            result += $"{value,-15}";
          }
          result += "\n";
        }

        if (Data.Rows.Count > maxRows)
        {
          result += $"... и ещё {Data.Rows.Count - maxRows} строк\n";
        }

        result += new string('-', 50) + "\n";
        result += $"✅ Всего строк: {Data.Rows.Count}\n";
        result += $"✅ Всего колонок: {Data.Columns.Count}\n";

        return result;
      }
    }

    /// <summary>
    /// Читает Excel-файл и возвращает данные первого листа
    /// </summary>
    /// <param name="filePath">Путь к файлу</param>
    /// <param name="maxRows">Максимальное количество строк для чтения (0 - все строки)</param>
    /// <returns>Объект ExcelData с данными</returns>
    public ExcelData ReadExcelFile(string filePath, int maxRows = 0)
    {
      if (!File.Exists(filePath))
        throw new FileNotFoundException($"Файл не найден: {filePath}");

      var result = new ExcelData();

      using (var workbook = new XLWorkbook(filePath))
      {
        // Получаем первый лист
        var worksheet = workbook.Worksheet(1);
        if (worksheet == null)
          throw new Exception("В файле нет листов");

        result.SheetName = worksheet.Name;

        // Определяем размеры таблицы (используем UsedRange)
        var usedRange = worksheet.RangeUsed();
        if (usedRange == null)
          throw new Exception("Лист пуст");

        int totalRows = usedRange.RowCount();
        int totalCols = usedRange.ColumnCount();

        if (totalRows == 0 || totalCols == 0)
          throw new Exception("Лист пуст");

        // Если maxRows == 0, читаем все строки
        int rowsToRead = maxRows > 0 ? Math.Min(maxRows, totalRows) : totalRows;

        // Создаём DataTable
        var dataTable = new DataTable();

        // Добавляем колонки (используем значения из первой строки как заголовки)
        var firstRow = usedRange.Row(1);
        for (int col = 1; col <= totalCols; col++)
        {
          string columnName = firstRow.Cell(col).GetString();
          if (string.IsNullOrEmpty(columnName))
            columnName = $"Column{col}";
          dataTable.Columns.Add(columnName);
        }

        // Читаем данные (начиная со второй строки)
        int startRow = 2; // Пропускаем заголовки
        int endRow = Math.Min(startRow + rowsToRead - 1, totalRows);

        for (int row = startRow; row <= endRow; row++)
        {
          var dataRow = dataTable.NewRow();
          var excelRow = usedRange.Row(row);
          for (int col = 1; col <= totalCols; col++)
          {
            dataRow[col - 1] = excelRow.Cell(col).GetString();
          }
          dataTable.Rows.Add(dataRow);
        }

        result.Data = dataTable;
      }

      return result;
    }

    /// <summary>
    /// Читает Excel-файл и возвращает данные в виде текста
    /// </summary>
    public string ReadExcelFileAsText(string filePath, int maxRows = 20, int maxCols = 10)
    {
      var data = ReadExcelFile(filePath, maxRows);
      return data.ToText(maxRows, maxCols);
    }

    /// <summary>
    /// Читает Excel-файл и возвращает только значение из указанной ячейки
    /// Пока нигде не используется
    /// </summary>
    public string ReadCellValue(string filePath, string cellAddress)
    {
      if (!File.Exists(filePath))
        throw new FileNotFoundException($"Файл не найден: {filePath}");

      using (var workbook = new XLWorkbook(filePath))
      {
        var worksheet = workbook.Worksheet(1);
        if (worksheet == null)
          throw new Exception("В файле нет листов");

        var cell = worksheet.Cell(cellAddress);
        return cell.GetString();
      }
    }
  }
}