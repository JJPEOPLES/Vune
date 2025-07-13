using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Vune.UI.Services;
using Vune.UI.Views.Dialogs;

namespace Vune.UI.Views
{
    public partial class MainWindow : Window
    {
        private readonly IExtensionService _extensionService;
        private readonly IVSCodeImportService _vsCodeImportService;

        public MainWindow()
        {
            InitializeComponent();
            
            // Get services
            var serviceProvider = ((App)Application.Current)._serviceProvider;
            _extensionService = serviceProvider.GetRequiredService<IExtensionService>();
            _vsCodeImportService = serviceProvider.GetRequiredService<IVSCodeImportService>();
            
            // Initialize UI
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Populate extensions list
            RefreshExtensionsList();
        }

        private void RefreshExtensionsList()
        {
            ExtensionsList.Items.Clear();
            
            foreach (var extension in _extensionService.GetInstalledExtensions())
            {
                ExtensionsList.Items.Add($"{extension.Name} ({extension.Version})");
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ManageExtensionsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ExtensionManagerDialog(_extensionService);
            dialog.Owner = this;
            dialog.ShowDialog();
            
            // Refresh extensions list after dialog is closed
            RefreshExtensionsList();
        }

        private void ImportFromVSCodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VSCodeImportDialog(_vsCodeImportService);
            dialog.Owner = this;
            dialog.ShowDialog();
            
            // Refresh extensions list after dialog is closed
            RefreshExtensionsList();
        }
    }
}