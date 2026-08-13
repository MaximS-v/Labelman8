namespace Labelman8.Models
{
	/// <summary>
	/// Тип маркировки (шаблон)
	/// </summary>
	public class MarkingType
	{
		/// <summary>
		/// Уникальный идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Название типа (например, "Основная", "Малая")
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Ширина наклейки в миллиметрах
		/// </summary>
		public double WidthMM { get; set; }

		/// <summary>
		/// Высота наклейки в миллиметрах
		/// </summary>
		public double HeightMM { get; set; }

		/// <summary>
		/// Название шрифта (например, "Arial", "Calibri")
		/// </summary>
		public string FontName { get; set; }

		/// <summary>
		/// Размер шрифта по умолчанию в пунктах
		/// </summary>
		public double DefaultFontSizePt { get; set; }
	}
}