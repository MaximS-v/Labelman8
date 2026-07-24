using Labelman8.Models;
using Labelman8.Modules;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Labelman8
{
  public partial class MainWindow : Window
  {
		// === Коллекция для привязки к DataGrid ===
		private ObservableCollection<SwitchboardSpecItem> specItems = new ObservableCollection<SwitchboardSpecItem>();

		// === Коллекция для печати ===
    private ObservableCollection<Switchboard> preparedItems = new ObservableCollection<Switchboard>();

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

					// НЕ Очищаем старые данные
					// specItems.Clear();

					// Добавляем новые данные
					foreach (var item in data)
					{
						specItems.Add(item);
					}

					txtStatus.Text = $"✅ Добавлено записей: {data.Count}, всего: {specItems.Count}";
				}
        catch (Exception ex)
        {
					txtStatus.Text = $"❌ Ошибка: {ex.Message}";
					MessageBox.Show($"Ошибка при чтении файла:\n{ex.Message}", "Ошибка",
													MessageBoxButton.OK, MessageBoxImage.Error);
				}
      }
    }

    // === Обработчик: Подготовка печати ===
    private void BtnPreparePrint_Click(object sender, RoutedEventArgs e)
    {
      if (specItems.Count == 0)
      {
        MessageBox.Show("Сначала загрузите данные из Excel!", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
        return;
      }

      try
      {
        // Очищаем предыдущие подготовленные данные
        preparedItems.Clear();

        // Генерируем готовые щиты из спецификации
        var generator = new SwitchboardGenerator();
        var result = generator.GenerateFromSpec(specItems.ToList());

        foreach (var item in result)
        {
          preparedItems.Add(item);
        }

        txtStatus.Text = $"✅ Подготовлено щитов: {preparedItems.Count}";

        // Показываем результат в отдельном окне (или в этом же гриде)
        ShowPreparedDataWindow();
      }
      catch (Exception ex)
      {
        txtStatus.Text = $"❌ Ошибка подготовки: {ex.Message}";
        MessageBox.Show($"Ошибка при подготовке данных:\n{ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    // === Показать подготовленные данные ===
    private void ShowPreparedDataWindow()
    {
      var window = new Window
      {
        Title = "Подготовленные щиты для печати",
        Width = 900,
        Height = 500,
        WindowStartupLocation = WindowStartupLocation.CenterOwner
      };

      var grid = new DataGrid
      {
        ItemsSource = preparedItems,
        AutoGenerateColumns = true,
        IsReadOnly = true,
        AlternatingRowBackground = System.Windows.Media.Brushes.LightGray,
        RowHeaderWidth = 0,
        Margin = new Thickness(10)
      };

      // Создаём контейнер и добавляем грид
      var content = new Grid();
      content.Children.Add(grid);
      content.Margin = new Thickness(10);

      window.Content = content;
      window.ShowDialog();
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