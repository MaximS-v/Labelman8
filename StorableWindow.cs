using System;
using System.Windows;
using Microsoft.Win32;

namespace Labelman8
{
	/// <summary>
	/// Базовый класс для окон с сохранением состояния в реестре
	/// </summary>
	public abstract class StorableWindow : Window
	{
		private const string REGISTRY_PATH = @"Software\Labelman8\Windows";

		protected StorableWindow()
		{
			this.SourceInitialized += (s, e) => LoadWindowState();
			this.Closing += (s, e) => SaveWindowState();
		}

		/// <summary>
		/// Уникальное имя окна для сохранения в реестре
		/// </summary>
		protected abstract string WindowRegistryName { get; }

		private void LoadWindowState()
		{
			try
			{
				using (var key = Registry.CurrentUser.OpenSubKey($@"{REGISTRY_PATH}\{WindowRegistryName}"))
				{
					if (key == null) return;

					// Загружаем размер
					var width = key.GetValue("Width") as int?;
					var height = key.GetValue("Height") as int?;
					var left = key.GetValue("Left") as int?;
					var top = key.GetValue("Top") as int?;
					var state = key.GetValue("State") as string;

					if (width.HasValue && height.HasValue && width.Value > 0 && height.Value > 0)
					{
						this.Width = width.Value;
						this.Height = height.Value;
					}

					if (left.HasValue && top.HasValue)
					{
						this.Left = left.Value;
						this.Top = top.Value;
					}

					if (!string.IsNullOrEmpty(state) && Enum.TryParse(state, out WindowState windowState))
					{
						this.WindowState = windowState;
					}
				}
			}
			catch
			{
				// Если что-то пошло не так — просто игнорируем
			}
		}

		private void SaveWindowState()
		{
			try
			{
				using (var key = Registry.CurrentUser.CreateSubKey($@"{REGISTRY_PATH}\{WindowRegistryName}"))
				{
					if (key == null) return;

					// Если окно свёрнуто или развёрнуто — сохраняем состояние, но размеры берём до сворачивания
					var state = this.WindowState;
					double width = this.Width;
					double height = this.Height;
					double left = this.Left;
					double top = this.Top;

					if (state == WindowState.Normal)
					{
						width = this.ActualWidth;
						height = this.ActualHeight;
						left = this.Left;
						top = this.Top;
					}

					key.SetValue("Width", (int)width);
					key.SetValue("Height", (int)height);
					key.SetValue("Left", (int)left);
					key.SetValue("Top", (int)top);
					key.SetValue("State", state.ToString());
				}
			}
			catch
			{
				// Если сохранить не удалось — игнорируем
			}
		}
	}
}