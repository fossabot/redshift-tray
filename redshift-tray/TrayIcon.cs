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
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls;
using System.Windows;

namespace redshift_tray
{
  class TrayIcon
  {

    private static TrayIcon TrayIconInstance;

    private readonly TaskbarIcon TaskbarIconInstance;

    public event RoutedEventHandler OnTrayIconLeftClick;
    private void TrayIconLeftClick(RoutedEventArgs e)
    {
      OnTrayIconLeftClick?.Invoke(this, e);
    }

    public event RoutedEventHandler OnMenuItemExitClicked;
    private void MenuItemExitClicked(RoutedEventArgs e)
    {
      OnMenuItemExitClicked?.Invoke(this, e);
    }

    public event RoutedEventHandler OnMenuItemLogClicked;
    private void MenuItemLogClicked(RoutedEventArgs e)
    {
      OnMenuItemLogClicked?.Invoke(this, e);
    }

    public event RoutedEventHandler OnMenuItemSettingsClicked;
    private void MenuItemSettingsClicked(RoutedEventArgs e)
    {
      OnMenuItemSettingsClicked?.Invoke(this, e);
    }

    public Status TrayStatus
    {
      set
      {
        switch(value)
        {
          case Status.Automatic:
            TaskbarIconInstance.Icon = Properties.Resources.TrayIconAuto;
            TaskbarIconInstance.ToolTipText = "Redshift Tray";
            break;
          case Status.Off:
            TaskbarIconInstance.Icon = Properties.Resources.TrayIconOff;
            TaskbarIconInstance.ToolTipText = "Redshift Tray (disabled)";
            break;
        }
      }
    }

    private TrayIcon(Status initialStatus)
    {
      TrayIconInstance?.TaskbarIconInstance.Dispose();

      TaskbarIconInstance = new TaskbarIcon();
      TrayStatus = initialStatus;
      TaskbarIconInstance.ContextMenu = getContextMenu();

      TaskbarIconInstance.TrayLeftMouseUp += TaskbarIconInstance_TrayLeftMouseUp;
    }

    public static TrayIcon Create(Status initialStatus)
    {
      TrayIconInstance = new TrayIcon(initialStatus);
      return TrayIconInstance;
    }

    public static TrayIcon CreateOrGet(Status initialStatus)
    {
      if(TrayIconInstance == null)
      {
        return Create(initialStatus);
      }
      return TrayIconInstance;
    }

    private ContextMenu getContextMenu()
    {
      ContextMenu contextMenu = new ContextMenu();

      MenuItem menuItemSettings = new MenuItem {Header = "Settings"};
      menuItemSettings.Click += menuItemSettings_Click;
      contextMenu.Items.Add(menuItemSettings);

      MenuItem menuItemLog = new MenuItem {Header = "Show log"};
      menuItemLog.Click += menuItemLog_Click;
      contextMenu.Items.Add(menuItemLog);

      MenuItem menuItemAbout = new MenuItem {Header = "About"};
      menuItemAbout.Click += menuItemAbout_Click;
      contextMenu.Items.Add(menuItemAbout);

      contextMenu.Items.Add(new Separator());

      MenuItem menuItemExit = new MenuItem {Header = "Exit"};
      menuItemExit.Click += menuItemExit_Click;
      contextMenu.Items.Add(menuItemExit);

      return contextMenu;
    }

    private void TaskbarIconInstance_TrayLeftMouseUp(object sender, RoutedEventArgs e)
    {
      if(Common.WindowExists<SettingsWindow>())
      {
        return;
      }
      TrayIconLeftClick(e);
    }

    private void menuItemSettings_Click(object sender, RoutedEventArgs e)
    {
      MenuItemSettingsClicked(e);
    }

    private void menuItemLog_Click(object sender, RoutedEventArgs e)
    {
      MenuItemLogClicked(e);
    }

    private void menuItemAbout_Click(object sender, RoutedEventArgs e)
    {
      About aboutDialog;
      if(!Common.WindowExistsFocus(out aboutDialog))
      {
        aboutDialog = new About();
        aboutDialog.ShowDialog();
      }
    }

    private void menuItemExit_Click(object sender, RoutedEventArgs e)
    {
      MenuItemExitClicked(e);
    }

  }
}
