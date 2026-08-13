using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FirebirdSql.Data.FirebirdClient;
using Labelman8.Models;

namespace Labelman8.Modules
{
	public class DatabaseHelper
	{
		private const string ConnectionStringTemplate =
				"User=EN11105KLF;Password=45sgc2018zh7;Database=e:\\db\\1.fdb;DataSource={0};Port=3050;Connection Timeout=5;";

		private static readonly string[] Servers = new[]
		{
						"192.168.0.5",
						"77.220.150.87"
				};

		private const int ConnectionTimeoutSeconds = 5;

		public string ConnectedServer { get; private set; } = "";

		/// <summary>
		/// Пытается подключиться в отдельном потоке с принудительным таймаутом.
		/// </summary>
		private FbConnection TryConnectWithTimeout(string server, int timeoutSeconds)
		{
			FbConnection connection = null;
			Exception threadException = null;
			bool completed = false;

			Thread thread = new Thread(() =>
			{
				try
				{
					string connectionString = string.Format(ConnectionStringTemplate, server);
					Debug.WriteLine($"Попытка подключения к {server}");

					connection = new FbConnection(connectionString);
					connection.Open();

					Debug.WriteLine($"✅ Подключено к {server}");
					completed = true;
				}
				catch (Exception ex)
				{
					threadException = ex;
				}
			});

			thread.IsBackground = true;
			thread.Start();

			// Ждём завершения потока с таймаутом
			if (!thread.Join(timeoutSeconds * 1000))
			{
				// Таймаут — принудительно прерываем поток
				Debug.WriteLine($"❌ Таймаут подключения к {server} ({timeoutSeconds} сек)");
				try
				{
					thread.Abort();
				}
				catch { }

				// Принудительно закрываем и уничтожаем соединение, если оно было создано
				if (connection != null)
				{
					try
					{
						if (connection.State == System.Data.ConnectionState.Open)
							connection.Close();
					}
					catch { }

					try
					{
						connection.Dispose();
					}
					catch { }
				}

				throw new TimeoutException($"Таймаут подключения к {server} ({timeoutSeconds} сек)");
			}

			// Если поток завершился с ошибкой
			if (threadException != null)
			{
				// Принудительно закрываем и уничтожаем соединение, если оно было создано
				if (connection != null)
				{
					try
					{
						if (connection.State == System.Data.ConnectionState.Open)
							connection.Close();
					}
					catch { }

					try
					{
						connection.Dispose();
					}
					catch { }
				}

				throw threadException;
			}

			// Если успешно — возвращаем соединение
			if (completed && connection != null)
				return connection;

			// Если дошли сюда — что-то пошло не так
			throw new Exception($"Неизвестная ошибка подключения к {server}");
		}

		public FbConnection CreateConnection()
		{
			List<string> errors = new List<string>();

			foreach (string server in Servers)
			{
				try
				{
					var connection = TryConnectWithTimeout(server, ConnectionTimeoutSeconds);

					// Проверка на null (на случай, если метод вернул null)
					if (connection == null)
					{
						throw new Exception($"Соединение с {server} вернуло null");
					}

					ConnectedServer = server;
					return connection;
				}
				catch (TimeoutException ex)
				{
					errors.Add($"Таймаут на {server}");
					Debug.WriteLine($"❌ {ex.Message}");
				}
				catch (Exception ex)
				{
					errors.Add($"{server}: {ex.Message}");
					Debug.WriteLine($"❌ Ошибка на {server}: {ex.Message}");
				}
			}

			ConnectedServer = "";
			throw new Exception($"Не удалось подключиться. Ошибки: {string.Join("; ", errors)}");
		}

		public string GetConnectedServer()
		{
			FbConnection connection = null;
			try
			{
				connection = CreateConnection();
				return ConnectedServer;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Итоговая ошибка: {ex.Message}");
				return "";
			}
			finally
			{
				if (connection != null)
				{
					try
					{
						if (connection.State == System.Data.ConnectionState.Open)
							connection.Close();
					}
					catch { }

					try
					{
						connection.Dispose();
					}
					catch { }
				}
			}
		}

		public List<MarkingType> GetAllMarkingTypes()
		{
			var types = new List<MarkingType>();

			using (var connection = CreateConnection())
			{
				string query = "SELECT ID, TYPE_NAME, HEIGHT_MM, WIDTH_MM, FONT_NAME, FONT_SIZE_PT FROM MARKING_TYPES ORDER BY TYPE_NAME";

				using (var command = new FbCommand(query, connection))
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						types.Add(new MarkingType
						{
							Id = Convert.ToInt32(reader["ID"]),
							Name = reader["TYPE_NAME"]?.ToString() ?? "",
							HeightMM = Convert.ToDouble(reader["HEIGHT_MM"]),
							WidthMM = Convert.ToDouble(reader["WIDTH_MM"]),
							FontName = reader["FONT_NAME"]?.ToString() ?? "",
							FontSizePt = Convert.ToDouble(reader["FONT_SIZE_PT"])
						});
					}
				}
			}

			return types;
		}

		public void AddMarkingType(MarkingType type)
		{
			using (var connection = CreateConnection())
			{
				string query = @"INSERT INTO MARKING_TYPES (ID, TYPE_NAME, HEIGHT_MM, WIDTH_MM, FONT_NAME, FONT_SIZE_PT) 
                                 VALUES (GEN_ID(GEN_MARKING_TYPES_ID, 1), @name, @height, @width, @font, @size)";

				using (var command = new FbCommand(query, connection))
				{
					command.Parameters.AddWithValue("@name", type.Name);
					command.Parameters.AddWithValue("@height", type.HeightMM);
					command.Parameters.AddWithValue("@width", type.WidthMM);
					command.Parameters.AddWithValue("@font", type.FontName);
					command.Parameters.AddWithValue("@size", type.FontSizePt);
					command.ExecuteNonQuery();
				}
			}
		}

		public void UpdateMarkingType(MarkingType type)
		{
			using (var connection = CreateConnection())
			{
				string query = @"UPDATE MARKING_TYPES SET 
                                 TYPE_NAME = @name, 
                                 HEIGHT_MM = @height, 
                                 WIDTH_MM = @width, 
                                 FONT_NAME = @font, 
                                 FONT_SIZE_PT = @size 
                                 WHERE ID = @id";

				using (var command = new FbCommand(query, connection))
				{
					command.Parameters.AddWithValue("@id", type.Id);
					command.Parameters.AddWithValue("@name", type.Name);
					command.Parameters.AddWithValue("@height", type.HeightMM);
					command.Parameters.AddWithValue("@width", type.WidthMM);
					command.Parameters.AddWithValue("@font", type.FontName);
					command.Parameters.AddWithValue("@size", type.FontSizePt);
					command.ExecuteNonQuery();
				}
			}
		}

		public void DeleteMarkingType(int id)
		{
			using (var connection = CreateConnection())
			{
				string query = "DELETE FROM MARKING_TYPES WHERE ID = @id";

				using (var command = new FbCommand(query, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					command.ExecuteNonQuery();
				}
			}
		}
	}
}