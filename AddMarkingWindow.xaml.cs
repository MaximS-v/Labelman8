using Labelman8.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace Labelman8
{
  public partial class AddMarkingWindow : StorableWindow
  {
    protected override string WindowRegistryName => "AddMarkingWindow";

    private ObservableCollection<MarkingType> markingTypes = new ObservableCollection<MarkingType>();
    private ObservableCollection<TerminalMarkingType> terminalMarkingTypes = new ObservableCollection<TerminalMarkingType>();

    private void SaveMarkingTypes()
    {
      try
      {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "marking_types.json");
        System.Diagnostics.Debug.WriteLine($"Сохранение в: {filePath}");

        var storage = new MarkingTypesStorage
        {
          MarkingTypes = markingTypes.ToList()
        };

        var serializer = new JavaScriptSerializer();
        string json = serializer.Serialize(storage);

        File.WriteAllText(filePath, json);

        // Проверяем, что файл создан
        if (File.Exists(filePath))
        {
          System.Diagnostics.Debug.WriteLine($"Файл создан, размер: {new FileInfo(filePath).Length} байт");
        }
        else
        {
          System.Diagnostics.Debug.WriteLine("Файл НЕ создан!");
        }

        System.Diagnostics.Debug.WriteLine($"Сохранено {markingTypes.Count} типов маркировки");
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"Ошибка сохранения: {ex.Message}");
        MessageBox.Show($"Ошибка сохранения данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    public AddMarkingWindow()
		{
			InitializeComponent();

			// Привязываем таблицу "Аппараты"
			dgMarkingTypes.ItemsSource = markingTypes;

			// Привязываем таблицу "Клеммы"
			dgTerminalMarkingTypes.ItemsSource = terminalMarkingTypes;

			LoadMarkingTypes();
			LoadTerminalMarkingTypes();
			UpdateButtonsState();
		}

		private void LoadMarkingTypes()
    {
      try
      {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "marking_types.json");
        if (!File.Exists(filePath))
        {
          System.Diagnostics.Debug.WriteLine("Файл marking_types.json не найден");
          return;
        }

        string json = File.ReadAllText(filePath);
        var serializer = new JavaScriptSerializer();
        var storage = serializer.Deserialize<MarkingTypesStorage>(json);

        if (storage?.MarkingTypes != null)
        {
          markingTypes.Clear();
          foreach (var type in storage.MarkingTypes)
          {
            markingTypes.Add(type);
          }
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"Ошибка загрузки marking_types.json: {ex.Message}");
      }
    }

    private void LoadTerminalMarkingTypes()
    {
      try
      {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "terminal_marking_types.json");
        if (!File.Exists(filePath))
        {
          System.Diagnostics.Debug.WriteLine("Файл terminal_marking_types.json не найден");
          return;
        }

        string json = File.ReadAllText(filePath);
        var serializer = new JavaScriptSerializer();
        var storage = serializer.Deserialize<TerminalMarkingTypesStorage>(json);

        if (storage?.TerminalMarkingTypes != null)
        {
          terminalMarkingTypes.Clear();
          foreach (var type in storage.TerminalMarkingTypes)
          {
            terminalMarkingTypes.Add(type);
          }
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"Ошибка загрузки terminal_marking_types.json: {ex.Message}");
      }
    }

    private void BtnAddDeviceType_Click(object sender, RoutedEventArgs e)
    {
      var window = new AddDeviceMarkingTypeWindow();
      window.Owner = this;

      if (window.ShowDialog() == true && window.Result != null)
      {
        markingTypes.Add(window.Result);
        SaveMarkingTypes(); // ← сохраняем после добавления
        System.Diagnostics.Debug.WriteLine($"Добавлен вид маркировки: {window.Result.Name}");
      }
    }

    private void BtnAddTerminalType_Click(object sender, RoutedEventArgs e)
		{
			// TODO: открыть окно добавления вида маркировки для клемм
			MessageBox.Show("Добавить вид маркировки для клемм (заглушка)");
		}

		private void BtnEditType_Click(object sender, RoutedEventArgs e)
		{
			// TODO: открыть окно изменения вида маркировки
			MessageBox.Show("Изменить вид маркировки (заглушка)");
		}

		private void BtnDeleteType_Click(object sender, RoutedEventArgs e)
		{
			// TODO: удаление вида маркировки
			MessageBox.Show("Удалить вид маркировки (заглушка)");
		}

		private void UpdateButtonsState()
		{
			// Проверяем, выбран ли элемент в таблице "Аппараты" и не является ли он системным
			bool isDeviceSelected = dgMarkingTypes.SelectedItem is MarkingType deviceType && !deviceType.IsSystem;

			// Для клемм проверяем только наличие выделения(IsSystem не используется)

		bool isTerminalSelected = dgTerminalMarkingTypes.SelectedItem is TerminalMarkingType;

			// Кнопки активны, если выбран НЕсистемный элемент в любой из таблиц
			bool canEditDelete = isDeviceSelected || isTerminalSelected;

			btnEditType.IsEnabled = canEditDelete;
			btnDeleteType.IsEnabled = canEditDelete;
		}

		private void DgMarkingTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateButtonsState();
		}

		private void DgTerminalMarkingTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateButtonsState();
		}
	}
}