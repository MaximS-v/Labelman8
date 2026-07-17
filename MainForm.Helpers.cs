using System;
using System.Drawing;
using System.Windows.Forms;

namespace Labelman8
{
  public partial class MainForm
  {
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
  }
}