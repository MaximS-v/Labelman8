using System.ComponentModel.DataAnnotations;

namespace Labelman8.Models
{
  public class Switchboard
  {
    [Display(Name = "Зав. номер")]
    public string SerialNumber { get; set; }

    [Display(Name = "Наименование")]
    public string Name { get; set; }

    [Display(Name = "In, A")]
    public double In { get; set; }

    [Display(Name = "Fn, Hz")]
    public double Fn { get; set; }

    [Display(Name = "Um, V")]
    public double Um { get; set; }

    [Display(Name = "Тип")]
    public string Function { get; set; }

    public override string ToString()
    {
      return $"{SerialNumber,-12} {Name,-20} {In,8:F2} {Fn,8:F2} {Um,8:F2} {Function,-15}";
    }

    public static string GetHeaders()
    {
      return $"{"Serial",-12} {"Name",-20} {"In",8} {"Fn",8} {"Um",8} {"Function",-15}";
    }

    public static string GetSeparator()
    {
      return new string('-', 80);
    }
  }
}