using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace OCRApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public App()
    {
        // Add global exception handling
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        DispatcherUnhandledException += App_DispatcherUnhandledException;
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        System.Windows.MessageBox.Show($"An unhandled exception occurred: {e.Exception.Message}\n\nStack Trace:\n{e.Exception.StackTrace}", 
            "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception ex = e.ExceptionObject as Exception;
        System.Windows.MessageBox.Show($"An unhandled exception occurred: {ex?.Message}\n\nStack Trace:\n{ex?.StackTrace}", 
            "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);
            
            // Create a test image in the application directory
            string testImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_image.png");
            TestImageGenerator.CreateTestImage(testImagePath);
            Console.WriteLine($"Test image created at: {testImagePath}");
            
            // Ensure the main window is shown
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error during startup: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", 
                "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
