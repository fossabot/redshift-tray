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
using redshift_tray.Properties;
using System;
using System.Windows;

namespace redshift_tray
{
  public partial class DebugConsole : Window
  {
    private bool isShown;

    public DebugConsole()
    {
      InitializeComponent();
      LoadPosition();
    }

    private void SavePosition()
    {
      Settings settings = Settings.Default;

      if(WindowState == WindowState.Maximized)
      {
        settings.DebugConsoleWindowState = WindowState;
      }
      else
      {
        settings.DebugConsoleLeft = Left;
        settings.DebugConsoleTop = Top;
        settings.DebugConsoleWidth = Width;
        settings.DebugConsoleHeight = Height;
      }
      settings.Save();
    }

    private void LoadPosition()
    {
      Settings settings = Settings.Default;

      if(Common.IsOutOfBounds(settings.DebugConsoleLeft, settings.DebugConsoleTop))
      {
        return;
      }

      WindowStartupLocation = WindowStartupLocation.Manual;

      if(settings.DebugConsoleWindowState == WindowState.Maximized)
      {
        WindowState = settings.DebugConsoleWindowState;
        return;
      }

      Left = settings.DebugConsoleLeft;
      Top = settings.DebugConsoleTop;
      Width = settings.DebugConsoleWidth;
      Height = settings.DebugConsoleHeight;
    }

    public void ShowOrUnhide()
    {
      if(isShown)
      {
        Visibility = Visibility.Visible;
      }
      else
      {
        Show();
        isShown = true;
      }
    }

    public new void Hide()
    {
      Visibility = Visibility.Hidden;
    }

    private void ButtonClipboard_Click(object sender, RoutedEventArgs e)
    {
      Clipboard.SetText(Output.Text);
    }

    private void ButtonClose_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      Hide();
      SavePosition();
      e.Cancel = true;
    }

    public void WriteLog(string message, LogType logType)
    {
      if(message.Length == 0)
      {
        return;
      }

      Output.Dispatcher.Invoke(() =>
      {
        string log = $"{DateTime.Now:HH:mm:ss} {logType}: {message}";
        Output.Text += log + Environment.NewLine;
      });
    }

    private void Output_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      Output.ScrollToEnd();
    }

    public enum LogType
    {
      Info,
      Error,
      Redshift
    }

  }
}
