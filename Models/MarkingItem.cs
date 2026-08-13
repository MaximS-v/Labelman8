namespace Labelman8.Models
{
	/// <summary>
	/// Экземпляр маркировки
	/// </summary>
	public class MarkingItem
	{
		/// <summary>
		/// Тип маркировки (ссылка на MarkingType)
		/// </summary>
		public MarkingType Type { get; set; }

		/// <summary>
		/// Текст на наклейке
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Размер шрифта в пунктах (если не указан — используется DefaultFontSizePt из типа)
		/// </summary>
		public double? FontSizePt { get; set; }
	}
}