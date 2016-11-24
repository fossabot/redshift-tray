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
using System.Diagnostics;
using System.Windows;

namespace redshift_tray
{
  public partial class About : Window
  {

    private string VersionText
    {
      set { Version.Text = $"Version: {value}"; }
    }

    public About()
    {
      InitializeComponent();
      LoadPosition();
      VersionText = Main.VERSION;
    }

    private void SavePosition()
    {
      Settings settings = Settings.Default;

      settings.AboutLeft = Left;
      settings.AboutTop = Top;

      settings.Save();
    }

    private void LoadPosition()
    {
      Settings settings = Settings.Default;

      if(Common.IsOutOfBounds(settings.AboutLeft, settings.AboutTop))
      {
        return;
      }

      WindowStartupLocation = WindowStartupLocation.Manual;
      Left = settings.AboutLeft;
      Top = settings.AboutTop;
    }

    private void RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      Process.Start(e.Uri.ToString());
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      SavePosition();
    }

  }
}
