using System.Windows;
using Labelman8.Models;

namespace Labelman8
{
  public partial class AddTerminalMarkingTypeWindow : StorableWindow
  {
    protected override string WindowRegistryName => "AddTerminalMarkingTypeWindow";

    public TerminalMarkingType Result { get; private set; }

    public AddTerminalMarkingTypeWindow()
    {
      InitializeComponent();
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(txtName.Text))
      {
        MessageBox.Show("Введите название.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        txtName.Focus();
        return;
      }

      if (!double.TryParse(txtHeight.Text, out double height) || height <= 0)
      {
        MessageBox.Show("Введите корректную высоту (положительное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        txtHeight.Focus();
        return;
      }

      if (!double.TryParse(txtPitch.Text, out double pitch) || pitch <= 0)
      {
        MessageBox.Show("Введите корректный шаг (положительное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        txtPitch.Focus();
        return;
      }

      Result = new TerminalMarkingType
      {
        Name = txtName.Text.Trim(),
        HeightMM = height,
        PitchMM = pitch
      };

      DialogResult = true;
      Close();
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }
  }
}