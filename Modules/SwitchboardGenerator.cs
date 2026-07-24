using System;
using System.Collections.Generic;
using Labelman8.Models;

namespace Labelman8.Modules
{
  /// <summary>
  /// Генератор готовых щитов из спецификации
  /// </summary>
  public class SwitchboardGenerator
  {
    /// <summary>
    /// Преобразует список спецификации в список готовых щитов
    /// </summary>
    public List<Switchboard> GenerateFromSpec(List<SwitchboardSpecItem> specItems)
    {
      var result = new List<Switchboard>();

      foreach (var item in specItems)
      {
        if (item.Quantity <= 1)
        {
          // Если количество 1 или меньше — создаём один щит
          result.Add(new Switchboard
          {
            SerialNumber = item.BaseSerialNumber,
            Name = item.Name,
            In = item.In,
            Fn = item.Fn,
            Um = item.Um,
            Function = item.Function
          });
        }
        else
        {
          // Определяем разделитель
          string separator = item.BaseSerialNumber.Contains("-") ? "-" : "-";

          // Определяем количество цифр для порядкового номера
          int digits = item.Quantity.ToString().Length;
          string format = $"D{digits}";  // "D1", "D2", "D3" и т.д.

          for (int i = 1; i <= item.Quantity; i++)
          {
            result.Add(new Switchboard
            {
              SerialNumber = $"{item.BaseSerialNumber}{separator}{i.ToString(format)}",
              Name = item.Name,
              In = item.In,
              Fn = item.Fn,
              Um = item.Um,
              Function = item.Function
            });
          }
        }
      }

      return result;
    }
  }
}