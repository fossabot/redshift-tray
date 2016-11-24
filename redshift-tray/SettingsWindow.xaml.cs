/* This file is part of redshift-tray.
   Redshift-tray is free software: you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.
   Redshift-tray is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.
   You should have received a copy of the GNU General Public License
   along with redshift-tray.  If not, see <http://www.gnu.org/licenses/>.
   Copyright (c) Michael Scholz <development@mischolz.de>
*/
using Microsoft.Win32;
using redshift_tray.Properties;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core.Input;
using System.Linq;
using System.Windows.Input;

namespace redshift_tray
{
  public partial class SettingsWindow : Window
  {

    private Redshift.ExecutableError _executableErrorState;

    private Redshift.ExecutableError ExecutableErrorState
    {
      set
      {
        _executableErrorState = value;
        switch (value)
        {
          case Redshift.ExecutableError.MissingPath:
            Run run;
            RedshiftInfo.Foreground = Brushes.Black;
            RedshiftInfo.Inlines.Clear();

            run = new Run("The required Redshift executable can be downloaded ");
            RedshiftInfo.Inlines.Add(run);

            run = new Run("here on Github");
            Hyperlink github = new Hyperlink(run) { NavigateUri = new System.Uri(Main.RELEASES_PAGE) };
            github.RequestNavigate += Hyperlink_RequestNavigate;
            RedshiftInfo.Inlines.Add(github);

            run = new Run(".");
            RedshiftInfo.Inlines.Add(run);
            break;
          case Redshift.ExecutableError.NotFound:
            RedshiftInfo.Foreground = Brushes.Red;
            RedshiftInfo.Text = "Invalid path to Redshift executable.";
            break;
          case Redshift.ExecutableError.WrongApplication:
            RedshiftInfo.Foreground = Brushes.Red;
            RedshiftInfo.Text = "Executable seems not to be a valid Redshift binary.";
            break;
          case Redshift.ExecutableError.WrongVersion:
            RedshiftInfo.Foreground = Brushes.Red;
            RedshiftInfo.Text = string.Format("The Redshift version is be too old. Please use at least version {0}.{1}.", Redshift.MIN_REDSHIFT_VERSION[0], Redshift.MIN_REDSHIFT_VERSION[1]);
            break;
          case Redshift.ExecutableError.Ok:
            RedshiftInfo.Foreground = Brushes.Green;
            RedshiftInfo.Text = "Redshift executable is suitable.";
            break;
        }
        SetOkButtonEnabled();
      }
    }

    public SettingsWindow()
    {
      InitializeComponent();
      LoadPosition();
      LoadConfig();
      ExecutableErrorState = Redshift.CheckExecutable(RedshiftPath.Text);
      SetOkButtonEnabled();
    }

    public SettingsWindow(Redshift.ExecutableError initialRedshiftErrorNote)
    {
      InitializeComponent();
      LoadPosition();
      LoadConfig();
      ExecutableErrorState = initialRedshiftErrorNote;
    }

    private void SavePosition()
    {
      Settings settings = Settings.Default;

      settings.SettingsWindowLeft = Left;
      settings.SettingsWindowTop = Top;

      settings.Save();
    }

    private void LoadPosition()
    {
      Settings settings = Settings.Default;

      if (Common.IsOutOfBounds(settings.SettingsWindowLeft, settings.SettingsWindowTop))
      {
        return;
      }

      WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
      Left = settings.SettingsWindowLeft;
      Top = settings.SettingsWindowTop;
    }

    private void SaveConfig()
    {
      Common.Autostart = (bool)Autostart.IsChecked;

      Settings.Default.Save();
    }

    private void LoadConfig()
    {
      DataContext = Settings.Default;
      Autostart.IsChecked = Common.Autostart;
    }

    private bool CheckConfig()
    {
      return (_executableErrorState == Redshift.ExecutableError.Ok);
    }

    private void ImportConfig(string file)
    {
      string[] config = File.ReadAllLines(file);

      var items = (
        from s in config
        where !s.StartsWith(";") && s.Contains("=")
        select s.Split('=')
        ).Select(s => new { key = s[0], value = s[1].Split(';')[0] });

      foreach (var item in items)
      {
        switch (item.key)
        {
          case "lat":
            decimal latitude;

            if (decimal.TryParse(item.value, NumberStyles.Float, new CultureInfo("en-US"), out latitude))
            {
              Latitude.Value = latitude;
            }
            break;
          case "lon":
            decimal longitude;

            if (decimal.TryParse(item.value, NumberStyles.Float, new CultureInfo("en-US"), out longitude))
            {
              Longitude.Value = longitude;
            }
            break;
          case "temp-day":
            int tempDay;

            if (int.TryParse(item.value, NumberStyles.Float, new CultureInfo("en-US"), out tempDay))
            {
              TemperatureDay.Value = tempDay;
            }
            break;
          case "temp-night":
            int tempNight;

            if (int.TryParse(item.value, NumberStyles.Float, new CultureInfo("en-US"), out tempNight))
            {
              TemperatureNight.Value = tempNight;
            }
            break;
          case "transition":
            Transition.IsChecked = (item.value == "1");
            break;
          case "brightness":
            decimal brightness;

            if (decimal.TryParse(item.value, NumberStyles.Float, new CultureInfo("en-US"), out brightness))
            {
              BrightnessDay.Value = brightness;
              BrightnessNight.Value = brightness;
            }
            break;
          case "brightness-day":
            decimal brightnessDay;

            if (decimal.TryParse(item.value, NumberStyles.Float, new CultureInfo("en-US"), out brightnessDay))
            {
              BrightnessDay.Value = brightnessDay;
            }
            break;
          case "brightness-night":
            decimal brightnessNight;

            if (decimal.TryParse(item.value, NumberStyles.Float, new CultureInfo("en-US"), out brightnessNight))
            {
              BrightnessNight.Value = brightnessNight;
            }
            break;
          case "gamma":
          case "gamma-day":
            string[] gammaS = item.value.Split(':');
            decimal[] gammaD = new decimal[gammaS.Length];
            for (int i = 0; i < gammaS.Length; i++)
            {
              if (!decimal.TryParse(gammaS[i], NumberStyles.Float, new CultureInfo("en-US"), out gammaD[i]))
              {
                break;
              }
            }

            if (gammaD.Length == 1)
            {
              GammaRed.Value = gammaD[0];
              GammaGreen.Value = gammaD[0];
              GammaBlue.Value = gammaD[0];
            }
            else if (gammaD.Length == 3)
            {
              GammaRed.Value = gammaD[0];
              GammaGreen.Value = gammaD[1];
              GammaBlue.Value = gammaD[2];
            }

            break;
        }
      }
    }

    private void SetOkButtonEnabled()
    {
      OkButton.IsEnabled = CheckConfig();
    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      System.Diagnostics.Process.Start("https://github.com/jonls/redshift/releases");
    }

    private void redshiftPath_LostFocus(object sender, RoutedEventArgs e)
    {
      ExecutableErrorState = Redshift.CheckExecutable(RedshiftPath.Text);
    }

    private void ButtonRedshift_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog
      {
        Title = "Redshift path",
        Filter = "Redshift|redshift.exe|All executables|*.exe",
        CheckFileExists = true
      };

      if (File.Exists(RedshiftPath.Text))
      {
        openFileDialog.InitialDirectory = Path.GetDirectoryName(RedshiftPath.Text);
      }

      if ((bool)openFileDialog.ShowDialog())
      {
        Settings.Default.RedshiftAppPath = openFileDialog.FileName;

        ExecutableErrorState = Redshift.CheckExecutable(RedshiftPath.Text);
      }
    }

    private void DetectLocationButton_Click(object sender, RoutedEventArgs e)
    {
      Mouse.OverrideCursor = Cursors.Wait;

      AutoLocation autoLocation = Common.DetectLocation();

      if (!autoLocation.Success)
      {
        System.Windows.MessageBox.Show(autoLocation.Errortext, "Error while detecting location", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      else
      {
        Settings.Default.RedshiftLatitude = autoLocation.Latitude;
        Settings.Default.RedshiftLongitude = autoLocation.Longitude;
      }

      Mouse.OverrideCursor = null;
    }

    private void ImportButton_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog
      {
        Title = "Import redshift config",
        Filter = "redshift.conf|redshift.conf|All files|*.*",
        CheckFileExists = true
      };

      if ((bool)openFileDialog.ShowDialog())
      {
        ImportConfig(openFileDialog.FileName);
      }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
      SaveConfig();
      DialogResult = true;
      Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      SavePosition();
    }

    private void Decimal_InputValidationError(object sender, InputValidationErrorEventArgs e)
    {
      DecimalUpDown dSender = (DecimalUpDown)sender;

      string value = dSender.Text;
      value = value.Replace(',', '.');

      decimal parseValue;
      if (decimal.TryParse(value, NumberStyles.Float, new CultureInfo("en-US"), out parseValue))
      {
        if (parseValue > dSender.Maximum)
        {
          dSender.Value = dSender.Maximum;
        }
        else if (parseValue < dSender.Minimum)
        {
          dSender.Value = dSender.Minimum;
        }
        else
        {
          dSender.Value = parseValue;
        }
      }
    }

  }
}
