using System;
using System.Collections.Generic;
using System.Reflection;
using Labelman8.Models;

namespace Labelman8.Modules
{
  public class ExcelColumnInfo
  {
    public string PropertyName { get; set; }
    public string Keyword { get; set; }
    public bool Required { get; set; }
    public int ColumnIndex { get; set; } = -1;
    public PropertyInfo PropertyInfo { get; set; }
  }

  public static class ExcelColumnMapper
  {
    public static List<ExcelColumnInfo> GetColumnMapping<T>()
    {
      var result = new List<ExcelColumnInfo>();
      var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

      foreach (var prop in properties)
      {
        var attr = prop.GetCustomAttribute<ExcelColumnAttribute>();
        if (attr != null)
        {
          result.Add(new ExcelColumnInfo
          {
            PropertyName = prop.Name,
            Keyword = attr.Keyword,
            Required = attr.Required,
            PropertyInfo = prop
          });
        }
      }

      return result;
    }

    public static string[] GetHeaderKeywords<T>()
    {
      var mapping = GetColumnMapping<T>();
      var keywords = new List<string>();
      foreach (var info in mapping)
      {
        keywords.Add(info.Keyword);
      }
      return keywords.ToArray();
    }

    public static bool ValidateMapping(List<ExcelColumnInfo> mapping, out string errorMessage)
    {
      var missing = new List<string>();
      foreach (var info in mapping)
      {
        if (info.Required && info.ColumnIndex == -1)
        {
          missing.Add(info.Keyword);
        }
      }

      if (missing.Count > 0)
      {
        errorMessage = $"Не найдены обязательные колонки: {string.Join(", ", missing)}";
        return false;
      }

      errorMessage = null;
      return true;
    }
  }
}