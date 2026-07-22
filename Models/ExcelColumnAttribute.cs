using System;

namespace Labelman8.Models
{
  /// <summary>
  /// Атрибут для сопоставления свойства модели с колонкой в Excel
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public sealed class ExcelColumnAttribute : Attribute
  {
    public string Keyword { get; }
    public bool Required { get; set; } = true;

    public ExcelColumnAttribute(string keyword)
    {
      Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
    }
  }
}