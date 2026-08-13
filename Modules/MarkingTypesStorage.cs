using System.Collections.Generic;

namespace Labelman8.Models
{
	/// <summary>
	/// Контейнер для списка типов маркировки
	/// </summary>
	public class MarkingTypesStorage
	{
		public List<MarkingType> MarkingTypes { get; set; } = new List<MarkingType>();
	}
}