using Labelman8.Modules;
using Microsoft.Win32;
using System;
using System.Windows;

namespace Labelman8
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void BtnExcel_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "Excel files (*.xlsm)|*.xlsm";
      openFileDialog.Title = "Выберите файл Excel (.xlsm)";

      if (openFileDialog.ShowDialog() == true)
      {
        string filePath = openFileDialog.FileName;
        txtStatus.Text = $"📂 Загрузка файла:\n{filePath}\n\n";

        try
        {
          var reader = new ExcelReader();
          var data = reader.ReadExcelFile(filePath, maxRows: 0);
          txtStatus.Text += data.ToText(maxRows: 20, maxCols: 10);

          MessageBox.Show("Данные из Excel успешно загружены!", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
          txtStatus.Text += $"❌ Ошибка при чтении файла:\n{ex.Message}";
          MessageBox.Show($"Ошибка при чтении файла:\n{ex.Message}", "Ошибка",
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
    }

    private void BtnDxf_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "DXF files (*.dxf)|*.dxf|All files (*.*)|*.*";
      openFileDialog.Title = "Выберите файл DXF";

      if (openFileDialog.ShowDialog() == true)
      {
        string filePath = openFileDialog.FileName;
        txtStatus.Text = $"✅ Выбран DXF-файл:\n{filePath}\n\n📌 Здесь будет чтение данных из DXF...";
        MessageBox.Show($"Выбран DXF-файл:\n{filePath}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
      }
    }

    private void BtnPrint_Click(object sender, RoutedEventArgs e)
    {
      txtStatus.Text = "🖨️ Печать...\n\n📌 Здесь будет отправка на печать...";
      MessageBox.Show("Печать будет здесь.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
    }
  }
}