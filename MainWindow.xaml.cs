using Labelman8.Models;
using Labelman8.Modules;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Labelman8
{
  public partial class MainWindow : Window
  {
    // === Хранилище данных ===
    private List<SwitchboardSpecItem> switchboardsData = new List<SwitchboardSpecItem>();

    public MainWindow()
    {
      InitializeComponent();
    }

    private void BtnExcel_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "Excel files (*.xlsm)|*.xlsm";
      openFileDialog.Title = "Выберите файл Excel со сводной спецификацией (.xlsm)";

      if (openFileDialog.ShowDialog() == true)
      {
        string filePath = openFileDialog.FileName;
        txtStatus.Text = $"📂 Загрузка файла:\n{filePath}\n\n";

        try
        {
          var reader = new ExcelReader();
          // Читаем данные в модель Switchboard
          switchboardsData = reader.ReadSpecItems(filePath, AppSettings.ExcelSheetPrefix);
          MessageBox.Show($"Сводная спецификация \n{filePath}\nзагружена", "Успех",
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

    private void BtnPrint_Click(object sender, RoutedEventArgs e)
    {
      txtStatus.Text = "🖨️ Печать...\n\n📌 Здесь будет отправка на печать...";
      MessageBox.Show("Печать будет здесь.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
    }
  }
}