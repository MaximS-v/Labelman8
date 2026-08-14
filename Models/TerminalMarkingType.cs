namespace Labelman8.Models
{
	/// <summary>
	/// Вид маркировки для клемм (тип клеммника)
	/// </summary>
	public class TerminalMarkingType
	{
		/// <summary>
		/// Название вида (например, "Клеммник 5x10")
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Высота наклейки в миллиметрах
		/// </summary>
		public double HeightMM { get; set; }

		/// <summary>
		/// Шаг (ширина одной клеммы) в миллиметрах
		/// </summary>
		public double PitchMM { get; set; }
	}
}