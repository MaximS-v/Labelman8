using Labelman8.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace Labelman8.Modules
{
	public class JsonStorage
	{
		private readonly string filePath;

		// ===== ЕДИНСТВЕННЫЙ КОНСТРУКТОР =====
		public JsonStorage(string fileName = "marking_data.json")
		{
			string appFolder = AppDomain.CurrentDomain.BaseDirectory;
			filePath = Path.Combine(appFolder, fileName);
		}

		public LocalDataStorage Load()
		{
			if (!File.Exists(filePath))
				return new LocalDataStorage();

			try
			{
				string json = File.ReadAllText(filePath);
				return JsonConvert.DeserializeObject<LocalDataStorage>(json) ?? new LocalDataStorage();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
				return new LocalDataStorage();
			}
		}

		public void Save(LocalDataStorage data)
		{
			try
			{
				string json = JsonConvert.SerializeObject(data, Formatting.Indented);
				File.WriteAllText(filePath, json);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"Ошибка сохранения данных: {ex.Message}");
				throw;
			}
		}

		public bool DataExists()
		{
			return File.Exists(filePath);
		}
	}
}