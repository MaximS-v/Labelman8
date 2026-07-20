using System;
using System.Windows;
using Microsoft.Win32;

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
        txtStatus.Text = $"✅ Выбран файл:\n{filePath}\n\n📌 Здесь будет чтение данных из Excel...";
        MessageBox.Show($"Выбран файл:\n{filePath}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
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