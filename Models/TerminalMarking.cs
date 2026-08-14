using System.Collections.Generic;

namespace Labelman8.Models
{
	/// <summary>
	/// Маркировка для конкретного клеммника
	/// </summary>
	public class TerminalMarking
	{
		/// <summary>
		/// Название клеммника (например, "XT1")
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Вид маркировки (ссылка на TerminalMarkingType)
		/// </summary>
		public TerminalMarkingType Type { get; set; }

		/// <summary>
		/// Список обозначений клемм в порядке следования
		/// </summary>
		public List<string> Labels { get; set; } = new List<string>();

		/// <summary>
		/// Количество клемм (определяется по длине Labels)
		/// </summary>
		public int TerminalCount => Labels?.Count ?? 0;
	}
}