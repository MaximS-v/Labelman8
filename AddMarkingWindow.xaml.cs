using System.Windows;

namespace Labelman8
{
  public partial class AddMarkingWindow : StorableWindow
  {
    protected override string WindowRegistryName => "AddMarkingWindow";

    public AddMarkingWindow()
    {
      InitializeComponent();
    }

    private void BtnAddType_Click(object sender, RoutedEventArgs e)
    {
      // TODO: открыть окно для добавления вида маркировки
      MessageBox.Show("Добавить вид маркировки (заглушка)");
    }
  }
}