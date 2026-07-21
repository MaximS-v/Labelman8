using ClosedXML.Excel;
using Labelman8.Models;  // <-- Добавить!
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Documents;

namespace Labelman8.Modules
{
  /// <summary>
  /// Класс для чтения данных из Excel-файлов (.xlsm, .xlsx) с использованием ClosedXML
  /// </summary>
  public class ExcelReader
  {
    public List<Switchboard> ReadSwitchboards (string filePath)
    {
      var switchboards = new List<Switchboard>();
      return switchboards;
    }
    

    
   
  }
}