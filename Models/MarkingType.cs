namespace Labelman8.Models
{
	public class MarkingType
	{
		public string Name { get; set; }
		public double WidthMM { get; set; }
		public double HeightMM { get; set; }
		public string FontName { get; set; }
		public double DefaultFontSizePt { get; set; }
		public bool IsSystem { get; set; }  // ← новый атрибут
	}
}