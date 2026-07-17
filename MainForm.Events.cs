using System;
using System.Windows.Forms;

namespace Labelman8
{
  public partial class MainForm
  {
    // === Обработчик: Загрузить Excel ===
    private void BtnExcel_Click(object sender, EventArgs e)
    {
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        openFileDialog.Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*";
        openFileDialog.Title = "Выберите файл Excel";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
          string filePath = openFileDialog.FileName;
          UpdateStatus($"✅ Выбран файл:\n{filePath}\n\n📌 Здесь будет чтение данных из Excel...");
          MessageBox.Show($"Выбран файл:\n{filePath}", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
      }
    }

    // === Обработчик: Загрузить DXF ===
    private void BtnDxf_Click(object sender, EventArgs e)
    {
      using (OpenFileDialog openFileDialog = new OpenFileDialog())
      {
        openFileDialog.Filter = "DXF files (*.dxf)|*.dxf|All files (*.*)|*.*";
        openFileDialog.Title = "Выберите файл DXF";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
          string filePath = openFileDialog.FileName;
          UpdateStatus($"✅ Выбран DXF-файл:\n{filePath}\n\n📌 Здесь будет чтение данных из DXF...");
          MessageBox.Show($"Выбран DXF-файл:\n{filePath}", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
      }
    }

    // === Обработчик: Печать ===
    private void BtnPrint_Click(object sender, EventArgs e)
    {
      UpdateStatus("🖨️ Печать...\n\n📌 Здесь будет отправка на печать...");
      MessageBox.Show("Печать будет здесь.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
  }
}