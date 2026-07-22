using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Labelman8.Models
{
  /// <summary>
  /// Позиция в спецификации (соответствует одной строке Excel)
  /// </summary>
  public class SwitchboardSpecItem
  {
		private string baseSerialNumber;
		private string name;
		private int quantity;
		private double inValue;
		private double fnValue;
		private double umValue;
		private string function;

		[ExcelColumn("Зав. номер")]
		[Display(Name = "Зав. номер")]
		public string BaseSerialNumber
		{
			get => baseSerialNumber;
			set { baseSerialNumber = value; OnPropertyChanged(); }
		}

		[ExcelColumn("Наименование")]
		[Display(Name = "Наименование")]
		public string Name
		{
			get => name;
			set { name = value; OnPropertyChanged(); }
		}

		[ExcelColumn("Кол-во")]
		[Display(Name = "Кол-во")]
		public int Quantity
		{
			get => quantity;
			set { quantity = value; OnPropertyChanged(); }
		}

		[ExcelColumn("In, A")]
		[Display(Name = "In, A")]
		public double In
		{
			get => inValue;
			set { inValue = value; OnPropertyChanged(); }
		}

		[ExcelColumn("Fn, Hz")]
		[Display(Name = "Fn, Hz")]
		public double Fn
		{
			get => fnValue;
			set { fnValue = value; OnPropertyChanged(); }
		}

		[ExcelColumn("Um, V")]
		[Display(Name = "Um, V")]
		public double Um
		{
			get => umValue;
			set { umValue = value; OnPropertyChanged(); }
		}

		[ExcelColumn("Назначение")]
		[Display(Name = "Назначение")]
		public string Function
		{
			get => function;
			set { function = value; OnPropertyChanged(); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}