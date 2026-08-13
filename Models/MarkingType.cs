namespace Labelman8.Models
{
	/// <summary>
	/// Тип маркировки (шаблон)
	/// </summary>
	public class MarkingType
	{
		/// <summary>
		/// Ширина наклейки в миллиметрах
		/// </summary>
		public double WidthMM { get; set; }

		/// <summary>
		/// Высота наклейки в миллиметрах
		/// </summary>
		public double HeightMM { get; set; }

		/// <summary>
		/// Название шрифта (например, "Arial")
		/// </summary>
		public string FontName { get; set; }

		/// <summary>
		/// Размер шрифта по умолчанию в пунктах
		/// </summary>
		public double DefaultFontSizePt { get; set; }
	}
}