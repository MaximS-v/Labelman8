using ClosedXML.Excel;
using Labelman8.Models;
using System;
using System.Collections.Generic;

namespace Labelman8.Modules
{
  /// <summary>
  /// Класс для чтения данных из Excel-файлов (.xlsm, .xlsx) с использованием ClosedXML
  /// </summary>
  public class ExcelReader
  {
    public List<Switchboard> ReadSwitchboards (string filePath, string sheetNamePrefix)
    {
      var switchboards = new List<Switchboard>();
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



  }
}