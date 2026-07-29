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

      // Изначально кнопки неактивны
      UpdateButtonsState();
    }

    // === Управление состоянием кнопок ===
    private void UpdateButtonsState()
    {
      bool hasData = specItems != null && specItems.Count > 0;

      btnPreparePrint.IsEnabled = hasData;
      btnPackaging.IsEnabled = hasData;
      btnMarking.IsEnabled = hasData;
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

          // Обновляем состояние кнопок после загрузки
          UpdateButtonsState();
        }
        catch (Exception ex)
        {
					txtStatus.Text = $"❌ Ошибка: {ex.Message}";
					MessageBox.Show($"Ошибка при чтении файла:\n{ex.Message}", "Ошибка",
													MessageBoxButton.OK, MessageBoxImage.Error);

          // Если ошибка — кнопки остаются неактивными
          UpdateButtonsState();
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
        preparedItems.Clear();
        var generator = new SwitchboardGenerator();
        var result = generator.GenerateFromSpec(specItems.ToList());

        foreach (var item in result)
        {
          preparedItems.Add(item);
        }

        txtStatus.Text = $"✅ Подготовлено щитов: {preparedItems.Count}";

        // Открываем окно предпросмотра
        var previewWindow = new PrintPreviewWindow(preparedItems.ToList());
        previewWindow.Owner = this;
        previewWindow.ShowDialog();
      }
      catch (Exception ex)
      {
        txtStatus.Text = $"❌ Ошибка подготовки: {ex.Message}";
        MessageBox.Show($"Ошибка при подготовке данных:\n{ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    // === Обработчик: Маркировка ===
    private void BtnMarking_Click(object sender, RoutedEventArgs e)
    {
      if (specItems.Count == 0)
      {
        MessageBox.Show("Сначала загрузите данные из Excel!", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
        return;
      }

      try
      {
        var generator = new SwitchboardGenerator();
        var preparedItems = generator.GenerateFromSpec(specItems.ToList());

        if (preparedItems.Count == 0)
        {
          MessageBox.Show("Нет данных для маркировки!", "Информация",
                          MessageBoxButton.OK, MessageBoxImage.Information);
          return;
        }

        txtStatus.Text = $"🏷️ Подготовка маркировки...";

        // TODO: Здесь будет логика генерации маркировочных листов
        // Например, создание этикеток с QR-кодами, штрих-кодами или другой информацией

        MessageBox.Show($"Подготовлено {preparedItems.Count} изделий для маркировки.", "Маркировка",
                        MessageBoxButton.OK, MessageBoxImage.Information);

        txtStatus.Text = $"✅ Подготовлено {preparedItems.Count} изделий для маркировки";
      }
      catch (Exception ex)
      {
        txtStatus.Text = $"❌ Ошибка: {ex.Message}";
        MessageBox.Show($"Ошибка при подготовке маркировки:\n{ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    // === Обработчик: Упаковка ===
    private void BtnPackaging_Click(object sender, RoutedEventArgs e)
    {
      if (specItems.Count == 0)
      {
        MessageBox.Show("Сначала загрузите данные из Excel!", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
        return;
      }

      try
      {
        var generator = new SwitchboardGenerator();
        var preparedItems = generator.GenerateFromSpec(specItems.ToList());

        if (preparedItems.Count == 0)
        {
          MessageBox.Show("Нет данных для упаковки!", "Информация",
                          MessageBoxButton.OK, MessageBoxImage.Information);
          return;
        }

        txtStatus.Text = $"📦 Подготовка упаковки...";

        var previewWindow = new PackagingPreviewWindow(preparedItems);
        previewWindow.Owner = this;
        previewWindow.ShowDialog();

        txtStatus.Text = $"✅ Подготовлено {preparedItems.Count} изделий для упаковки";
      }
      catch (Exception ex)
      {
        txtStatus.Text = $"❌ Ошибка: {ex.Message}";
        MessageBox.Show($"Ошибка при подготовке упаковки:\n{ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }
}