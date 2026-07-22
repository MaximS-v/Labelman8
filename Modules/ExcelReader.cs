using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Labelman8.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Labelman8.Modules
{
  /// <summary>
  /// Класс для чтения данных из Excel-файлов (.xlsm, .xlsx) с использованием ClosedXML
  /// </summary>
  public class ExcelReader
  {
    public List<Switchboard> ReadSwitchboards (string filePath, string sheetNamePrefix)
    {
      if (!File.Exists(filePath))
        throw new FileNotFoundException($"Файл не найден: {filePath}");

      var switchboards = new List<Switchboard>();

      using (var workbook = new XLWorkbook(filePath))
      {
        var worksheet = FindWorksheetByPrefix(workbook, sheetNamePrefix);

        if (worksheet == null) 
        {
          throw new Exception($"Лист с префиксом '{sheetNamePrefix}' не найден.");
        }
          
      }

      // === Ключевые слова берём из настроек ===
      string[] headerKeywords = AppSettings.HeaderKeywords;

      int headerRowIndex = FindHeaderRow(worksheet, headerKeywords);

      if (headerRowIndex == -1)
        return switchboards;

      return switchboards;
    }

    /// <summary>
    /// Находит первый лист, имя которого начинается с указанного префикса
    /// </summary>
    private IXLWorksheet FindWorksheetByPrefix(XLWorkbook workbook, string prefix)
    {
      foreach (var worksheet in workbook.Worksheets)
      {
        if (worksheet.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
          return worksheet;
        }
      }
      return null;
    }

    private int FindHeaderRow(IXLWorksheet worksheet, string[] headerKeywords)
    {
      var usedRange = worksheet.RangeUsed();
      if (usedRange == null)
        return -1;

      int totalRows = usedRange.RowCount();
      int totalCols = usedRange.ColumnCount();

      for (int row = 1;  row <= totalRows; row++)
      {
        string rowText = "";
        for (int col = 1; col <= totalCols; col++)
        {
          string cellValue = usedRange.Row(row).Cell(col).GetString();
          if (!string.IsNullOrEmpty(cellValue))
          {
            rowText += cellValue + " ";
          }
        }

        bool allKewordsFound = true;
        foreach (var keyword in headerKeywords)
        {
          if (!rowText.Contains(keyword, StringComparison.OrdinalIgnoreCase))
          {
            allKewordsFound = false;
            break;
          }
        }

        if (allKewordsFound) return row;
      }
      return -1;
    }

  }
}