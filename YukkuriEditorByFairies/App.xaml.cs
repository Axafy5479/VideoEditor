using ControlzEx.Theming;
using FFMediaToolkit;
using MahApps.Metro.Theming;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace YukkuriEditorByFairies
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string exePath = Assembly.GetExecutingAssembly().Location;
            string dir = Path.GetDirectoryName(exePath);

            string path = Path.Combine(dir, @"ffmpeg\\x86_64\");

            FFmpegLoader.FFmpegPath = path;
            // Set the application theme to Dark.Green
            //var theme = ThemeManager.Current.AddLibraryTheme(
            //    new LibraryTheme(
            //        new Uri("pack://application:,,,/Data;component/ColorThemes/LightTheme.xaml"),
            //        MahAppsLibraryThemeProvider.DefaultInstance
            //        )
            //    );

            //ThemeManager.Current.ChangeTheme(this, theme);

        }

        static App()
        {
            TextOptions.TextFormattingModeProperty.OverrideMetadata(typeof(MainWindow),
                    new FrameworkPropertyMetadata(TextFormattingMode.Display, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

            TextOptions.TextRenderingModeProperty.OverrideMetadata(typeof(MainWindow),
                new FrameworkPropertyMetadata(TextRenderingMode.ClearType,
                FrameworkPropertyMetadataOptions.AffectsMeasure | 
                FrameworkPropertyMetadataOptions.AffectsRender | 
                FrameworkPropertyMetadataOptions.Inherits));
        }
    }
}
