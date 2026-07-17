using System.Drawing;
using System.Windows.Forms;

namespace Labelman8
{
  public partial class MainForm : Form
  {
    // === Поля формы ===
    private TextBox txtStatus;
    private Button btnExcel;
    private Button btnDxf;
    private Button btnPrint;

    public MainForm()
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
  }
}