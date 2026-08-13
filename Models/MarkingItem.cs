namespace Labelman8.Models
{
	/// <summary>
	/// Конкретная маркировка (экземпляр)
	/// </summary>
	public class MarkingItem
	{
		/// <summary>
		/// Текст на наклейке
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Тип маркировки (ссылка на MarkingType)
		/// </summary>
		public MarkingType Type { get; set; }
	}
}