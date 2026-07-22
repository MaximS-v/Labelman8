using System;

namespace Labelman8.Models
{
  /// <summary>
  /// Атрибут для сопоставления свойства модели с колонкой в Excel
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public sealed class ExcelColumnAttribute : Attribute
  {
    /// <summary>
    /// Ключевое слово для поиска колонки в заголовке Excel
    /// </summary>
    public string Keyword { get; }

    /// <summary>
    /// Обязательное ли поле (если true, то отсутствие колонки вызовет ошибку)
    /// </summary>
    public bool Required { get; set; } = true;

    public ExcelColumnAttribute(string keyword)
    {
      Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
    }
  }
}