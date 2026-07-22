using System;
using System.Collections.Generic;
using System.Configuration;

namespace Labelman8
{
  /// <summary>
  /// Класс для доступа к настройкам приложения из App.config
  /// </summary>
  public static class AppSettings
  {
    /// <summary>
    /// Префикс имени листа Excel с данными
    /// </summary>
    public static string ExcelSheetPrefix
    {
      get
      {
        string value = ConfigurationManager.AppSettings["ExcelSheetPrefix"];
        return !string.IsNullOrWhiteSpace(value) ? value : "Сводная"; // значение по умолчанию
      }
    }
  }
}