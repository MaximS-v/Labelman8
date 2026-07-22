namespace Labelman8.Models
{
  public class Switchboard
  {
    [ExcelColumn("Зав. номер")]
    public string serialNumber { get; set; }

    [ExcelColumn("Наименование")]
    public string Name { get; set; }

    [ExcelColumn("In")]
    public double In { get; set; }

    [ExcelColumn("Fn")]
    public double Fn { get; set; }

    [ExcelColumn("Um")]
    public double Um { get; set; }

    [ExcelColumn("Назначение")]
    public string Function { get; set; }
  }
}
