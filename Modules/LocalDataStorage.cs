using System.Collections.Generic;
using Labelman8.Models;

namespace Labelman8.Modules
{
	/// <summary>
	/// Контейнер для всех данных, хранящихся локально
	/// </summary>
	public class LocalDataStorage
	{
		public List<MarkingType> MarkingTypes { get; set; } = new List<MarkingType>();
	}
}