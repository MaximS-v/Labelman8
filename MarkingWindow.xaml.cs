using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Labelman8
{
	/// <summary>
	/// Interaction logic for MarkingWindow.xaml
	/// </summary>
	public partial class MarkingWindow : StorableWindow
	{
		protected override string WindowRegistryName => "MarkingWindow";
		public MarkingWindow()
		{
			InitializeComponent();
		}

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
      var window = new AddMarkingWindow();
      window.Owner = this;
      window.ShowDialog();
    }

    private void BtnPrint_Click(object sender, RoutedEventArgs e)
		{
			// TODO: печать маркировки
			MessageBox.Show("Печать маркировки (заглушка)");
		}
	}
}
