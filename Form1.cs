using System;
using System.Drawing;
using System.Windows.Forms;

namespace Labelman8
{
  public partial class Form1 : Form
  {
    // === Поля формы ===
    private TextBox txtStatus;
    private Button btnExcel;
    private Button btnDxf;
    private Button btnPrint;

    public Form1()
    {
      InitializeComponent();
      this.Text = "Приложение для печати";
      this.Size = new Size(600, 300);
      this.StartPosition = FormStartPosition.CenterScreen;

      InitializeControls();
    }

    // === Инициализация всех элементов управления ===
    private void InitializeControls()
    {
      // Создаём поле для вывода статуса
      txtStatus = new TextBox
      {
        Name = "txtStatus",
        Location = new Point(20, 80),
        Size = new Size(540, 150),
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Vertical,
        Font = new Font("Consolas", 10)
      };
      this.Controls.Add(txtStatus);

      // Создаём кнопки
      btnExcel = CreateButton("📂 Загрузить Excel", 20, 20, BtnExcel_Click);
      btnDxf = CreateButton("📐 Загрузить DXF", 200, 20, BtnDxf_Click);
      btnPrint = CreateButton("🖨️ Печать", 380, 20, BtnPrint_Click);
    }

    // === Вспомогательный метод для создания кнопки ===
    private Button CreateButton(string text, int x, int y, EventHandler clickHandler)
    {
      Button btn = new Button
      {
        Text = text,
        Location = new Point(x, y),
        Size = new Size(160, 40)
      };
      btn.Click += clickHandler;
      this.Controls.Add(btn);
      return btn;
    }

    // === Обновление статуса ===
    private void UpdateStatus(string message)
    {
      txtStatus.Text = message;
    }

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