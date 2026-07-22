using Labelman8.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Labelman8.Modules
{
  /// <summary>
  /// Класс для чтения данных из Excel-файлов (.xlsm, .xlsx) с использованием EPPlus
  /// </summary>
  public class ExcelReader
  {
    public List<SwitchboardSpecItem> ReadSpecItems(string filePath, string sheetNamePrefix)
    {
      if (!File.Exists(filePath))
        throw new FileNotFoundException($"Файл не найден: {filePath}");

      var items = new List<SwitchboardSpecItem>();

      using (var package = new ExcelPackage(new FileInfo(filePath)))
      {
        var workbook = package.Workbook;

        var worksheet = FindWorksheetByPrefix(workbook, sheetNamePrefix);

        if (worksheet == null)
        {
          throw new Exception($"Лист с префиксом '{sheetNamePrefix}' не найден.");
        }

        string[] headerKeywords = ExcelColumnMapper.GetHeaderKeywords<SwitchboardSpecItem>();
        int headerRowIndex = FindHeaderRow(worksheet, headerKeywords);

        if (headerRowIndex == -1)
          return items;

        var columnInfoList = ExcelColumnMapper.GetColumnMapping<SwitchboardSpecItem>();
        var columnIndexMap = FindColumnIndexes(worksheet, headerRowIndex, columnInfoList);

        if (!ExcelColumnMapper.ValidateMapping(columnIndexMap, out string errorMessage))
        {
          throw new Exception(errorMessage);
        }

        int totalRows = worksheet.Dimension?.Rows ?? 0;
        if (totalRows == 0)
          return items;

        for (int row = headerRowIndex + 1; row <= totalRows; row++)
        {
          try
          {
            var item = new SwitchboardSpecItem();

            foreach (var info in columnIndexMap)
            {
              if (info.ColumnIndex != -1)
              {
                object cellValue = worksheet.Cells[row, info.ColumnIndex].Value;
                string cellString = cellValue?.ToString() ?? "";
                SetPropertyValue(item, info.PropertyInfo, cellString);
              }
            }

            // Если поле Function пустое — прекращаем чтение
            if (string.IsNullOrWhiteSpace(item.Function))
              break;

            items.Add(item);
          }
          catch (Exception ex)
          {
            System.Diagnostics.Debug.WriteLine($"Ошибка чтения строки {row}: {ex.Message}");
            break;
          }
        }
      }

      return items;
    }

    /// <summary>
    /// Находит первый лист, имя которого начинается с указанного префикса
    /// </summary>
    private ExcelWorksheet FindWorksheetByPrefix(ExcelWorkbook workbook, string prefix)
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

    /// <summary>
    /// Находит строку с заголовками в переданном листе
    /// </summary>
    private int FindHeaderRow(ExcelWorksheet worksheet, string[] headerKeywords)
    {
      int totalRows = worksheet.Dimension?.Rows ?? 0;
      int totalCols = worksheet.Dimension?.Columns ?? 0;

      if (totalRows == 0 || totalCols == 0)
        return -1;

      for (int row = 1; row <= totalRows; row++)
      {
        string rowText = "";
        for (int col = 1; col <= totalCols; col++)
        {
          object cellValue = worksheet.Cells[row, col].Value;
          if (cellValue != null)
          {
            rowText += cellValue.ToString() + " ";
          }
        }

        bool allKeywordsFound = true;
        foreach (var keyword in headerKeywords)
        {
          if (rowText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == -1)
          {
            allKeywordsFound = false;
            break;
          }
        }

        if (allKeywordsFound) return row;
      }
      return -1;
    }

    /// <summary>
    /// Находит номера колонок для всех свойств модели
    /// </summary>
    private List<ExcelColumnInfo> FindColumnIndexes(ExcelWorksheet worksheet, int headerRowIndex, List<ExcelColumnInfo> columnInfoList)
    {
      int totalCols = worksheet.Dimension?.Columns ?? 0;
      if (totalCols == 0)
        return columnInfoList;

      var result = new List<ExcelColumnInfo>(columnInfoList);
      var remainingInfos = new List<ExcelColumnInfo>(result);

      for (int col = 1; col <= totalCols; col++)
      {
        object cellValue = worksheet.Cells[headerRowIndex, col].Value;
        string cellString = cellValue?.ToString() ?? "";
        if (string.IsNullOrEmpty(cellString))
          continue;

        for (int i = remainingInfos.Count - 1; i >= 0; i--)
        {
          var info = remainingInfos[i];
          if (cellString.IndexOf(info.Keyword, StringComparison.OrdinalIgnoreCase) >= 0)
          {
            info.ColumnIndex = col;
            remainingInfos.RemoveAt(i);
          }
        }

        if (remainingInfos.Count == 0)
          break;
      }

      return result;
    }

    /// <summary>
    /// Устанавливает значение свойства объекта из строки
    /// </summary>
    private void SetPropertyValue(object target, PropertyInfo prop, string value)
    {
      if (string.IsNullOrEmpty(value))
        return;

      Type propType = prop.PropertyType;

      if (propType == typeof(string))
      {
        prop.SetValue(target, value);
      }
      else if (propType == typeof(double))
      {
        if (double.TryParse(value, out double result))
        {
          prop.SetValue(target, result);
        }
      }
      else if (propType == typeof(int))
      {
        if (int.TryParse(value, out int result))
        {
          prop.SetValue(target, result);
        }
      }
      else if (propType == typeof(bool))
      {
        if (bool.TryParse(value, out bool result))
        {
          prop.SetValue(target, result);
        }
      }
    }
  }
}