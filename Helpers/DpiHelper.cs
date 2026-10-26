using System;
using System.Configuration;

namespace Labelman8.Helpers
{
  /// <summary>
  /// Пересчёт миллиметров в WPF-единицы (DIP) на основе шага пикселя из App.config.
  /// </summary>
  public static class DpiHelper
  {
    /// <summary>
    /// Шаг пикселя монитора в миллиметрах (из App.config).
    /// По умолчанию 0.264 мм (стандарт 96 DPI).
    /// </summary>
    public static double PixelPitchMM
    {
      get
      {
        string value = ConfigurationManager.AppSettings["PixelPitchMM"];
        if (double.TryParse(value, out double result) && result > 0)
          return result;
        return 0.264;
      }
    }

    /// <summary>
    /// Переводит миллиметры в WPF-единицы (DIP) с учётом реального шага пикселя
    /// и системного масштабирования Windows.
    /// </summary>
    public static double MmToDip(double mm)
    {
      // DIP = mm / шаг_пикселя * (96 / системный_DPI)
      // Системный DPI = 96 * масштаб (например, 144 при 150%)
      double systemDpi = GetSystemDpi();
      return mm / PixelPitchMM * (96.0 / systemDpi);
    }

    /// <summary>
    /// Системный DPI (виртуальный), с учётом масштабирования Windows.
    /// </summary>
    private static double GetSystemDpi()
    {
      // Используем стандартный способ WPF: через PresentationSource
      var source = System.Windows.PresentationSource.FromVisual(
          System.Windows.Application.Current.MainWindow);

      if (source?.CompositionTarget != null)
      {
        return 96.0 * source.CompositionTarget.TransformToDevice.M11;
      }

      return 96.0; // По умолчанию
    }
  }
}