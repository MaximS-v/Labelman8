using System;
using System.IO;
using System.Web.Script.Serialization;
using Labelman8.Models;

namespace Labelman8.Modules
{
	public class MarkingTypesLoader
	{
		private readonly string filePath;

		public MarkingTypesLoader(string fileName = "marking_types.json")
		{
			string appFolder = AppDomain.CurrentDomain.BaseDirectory;
			filePath = Path.Combine(appFolder, fileName);
		}

		public MarkingTypesStorage Load()
		{
			if (!File.Exists(filePath))
			{
				System.Diagnostics.Debug.WriteLine($"Файл {filePath} не найден");
				return new MarkingTypesStorage();
			}

			try
			{
				string json = File.ReadAllText(filePath);
				var serializer = new JavaScriptSerializer();
				return serializer.Deserialize<MarkingTypesStorage>(json) ?? new MarkingTypesStorage();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка загрузки: {ex.Message}");
				return new MarkingTypesStorage();
			}
		}
	}
}