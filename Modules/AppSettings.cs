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

    /// <summary>
    /// Список ключевых слов для поиска строки заголовка
    /// </summary>
    public static string[] HeaderKeywords
    {
      get
      {
        string value = ConfigurationManager.AppSettings["HeaderKeyword"];
        if (string.IsNullOrWhiteSpace(value))
        {
          return new string[]
          {
            "Зав. номер",
            "Наименование",
            "Кол-во",
            "In",
            "Um"
          };
        }

        // Разделяем по '|' и удаляем пустые элементы
        return value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
      }
  }
}