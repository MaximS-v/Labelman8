namespace Labelman8.Models
{
  /// <summary>
  /// Позиция в спецификации (соответствует одной строке Excel)
  /// </summary>
  public class SwitchboardSpecItem
  {
    [ExcelColumn("Зав. номер")]
    public string BaseSerialNumber { get; set; }   // Базовый номер (без порядкового номера)

    [ExcelColumn("Наименование")]
    public string Name { get; set; }

    [ExcelColumn("Кол-во")]
    public int Quantity { get; set; }              // Количество

    [ExcelColumn("In, A")]
    public double In { get; set; }

    [ExcelColumn("Fn, Hz")]
    public double Fn { get; set; }

    [ExcelColumn("Um, V")]
    public double Um { get; set; }

    [ExcelColumn("Назначение")]
    public string Function { get; set; }

  }
}