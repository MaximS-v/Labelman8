namespace Labelman8.Models
{
	public class MarkingType
	{
		public int Id { get; set; } // <-- Добавляем первичный ключ
		public string Name { get; set; }
		public double HeightMM { get; set; }
		public double WidthMM { get; set; }
		public string FontName { get; set; }
		public double FontSizePt { get; set; }
	}
}