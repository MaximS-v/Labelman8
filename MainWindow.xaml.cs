using Labelman8.Models;
using Labelman8.Modules;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace Labelman8
{
  public partial class MainWindow : Window
  {
		// === Коллекция для привязки к DataGrid ===
		private ObservableCollection<SwitchboardSpecItem> specItems = new ObservableCollection<SwitchboardSpecItem>();

		public MainWindow()
    {
      InitializeComponent();

			// Привязываем DataGrid к коллекции
			dgData.ItemsSource = specItems;

			// Подписываемся на событие для настройки заголовков
			dgData.AutoGeneratingColumn += DgData_AutoGeneratingColumn;
		}

		private void DgData_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			// Получаем информацию о свойстве
			var propertyDescriptor = e.PropertyDescriptor as System.ComponentModel.PropertyDescriptor;
			if (propertyDescriptor != null)
			{
				// Ищем атрибут [Display]
				var displayAttribute = propertyDescriptor.Attributes
						.OfType<DisplayAttribute>()
						.FirstOrDefault();

				if (displayAttribute != null)
				{
					// Устанавливаем заголовок из атрибута
					e.Column.Header = displayAttribute.Name;
				}
			}
		}

		private void BtnExcel_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "Excel files (*.xlsm)|*.xlsm";
      openFileDialog.Title = "Выберите файл Excel со сводной спецификацией (.xlsm)";

      if (openFileDialog.ShowDialog() == true)
      {
        string filePath = openFileDialog.FileName;
        txtStatus.Text = $"📂 Загрузка файла:{filePath}";

        try
        {
          var reader = new ExcelReader();
					var data = reader.ReadSpecItems(filePath, AppSettings.ExcelSheetPrefix);

					// Очищаем старые данные
					specItems.Clear();

					// Добавляем новые данные
					foreach (var item in data)
					{
						specItems.Add(item);
					}

					txtStatus.Text = $"✅ Загружено записей: {specItems.Count}";
				}
        catch (Exception ex)
        {
					txtStatus.Text = $"❌ Ошибка: {ex.Message}";
					MessageBox.Show($"Ошибка при чтении файла:\n{ex.Message}", "Ошибка",
													MessageBoxButton.OK, MessageBoxImage.Error);
				}
      }
    }

    private void BtnPrint_Click(object sender, RoutedEventArgs e)
    {
			if (specItems.Count == 0)
			{
				MessageBox.Show("Сначала загрузите данные из Excel!", "Информация",
												MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			txtStatus.Text = $"🖨️ Печать... (заглушка)";
			MessageBox.Show($"Печать {specItems.Count} записей (заглушка).", "Печать",
											MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			if (specItems.Count == 0)
			{
				MessageBox.Show("Нет данных для сохранения!", "Информация",
												MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			// TODO: Здесь будет логика сохранения изменений обратно в Excel
			txtStatus.Text = "💾 Сохранение... (заглушка)";
			MessageBox.Show($"Сохранение {specItems.Count} записей (заглушка).", "Сохранение",
											MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}