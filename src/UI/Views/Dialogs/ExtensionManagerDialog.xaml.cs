using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vune.UI.Models;
using Vune.UI.Services;

namespace Vune.UI.Views.Dialogs
{
    public partial class ExtensionManagerDialog : Window
    {
        private readonly IExtensionService _extensionService;
        private Extension _selectedExtension;

        public ExtensionManagerDialog(IExtensionService extensionService)
        {
            InitializeComponent();
            
            _extensionService = extensionService;
            
            // Load installed extensions
            RefreshInstalledExtensions();
            
            // Update UI state
            UpdateUIState();
        }

        private void RefreshInstalledExtensions()
        {
            InstalledExtensionsListView.ItemsSource = _extensionService.GetInstalledExtensions();
        }

        private void SearchExtensions()
        {
            string query = SearchTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(query))
            {
                MarketplaceExtensionsListView.ItemsSource = null;
                return;
            }
            
            var results = _extensionService.SearchExtensions(query);
            MarketplaceExtensionsListView.ItemsSource = results;
        }

        private void UpdateUIState()
        {
            bool isExtensionSelected = _selectedExtension != null;
            bool isInstalled = isExtensionSelected && _extensionService.IsExtensionInstalled(_selectedExtension.Id);
            
            // Update extension details
            SelectedExtensionName.Text = isExtensionSelected ? _selectedExtension.Name : string.Empty;
            SelectedExtensionPublisher.Text = isExtensionSelected ? $"Publisher: {_selectedExtension.Publisher}" : string.Empty;
            SelectedExtensionDescription.Text = isExtensionSelected ? _selectedExtension.Description : string.Empty;
            
            // Update buttons
            InstallButton.IsEnabled = isExtensionSelected && !isInstalled;
            UninstallButton.IsEnabled = isExtensionSelected && isInstalled;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchExtensions();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchExtensions();
        }

        private void ExtensionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            _selectedExtension = listView?.SelectedItem as Extension;
            
            UpdateUIState();
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedExtension == null)
            {
                return;
            }
            
            if (_extensionService.InstallExtension(_selectedExtension.Id, _selectedExtension.Version))
            {
                MessageBox.Show($"Extension '{_selectedExtension.Name}' installed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshInstalledExtensions();
                UpdateUIState();
            }
            else
            {
                MessageBox.Show($"Failed to install extension '{_selectedExtension.Name}'.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedExtension == null)
            {
                return;
            }
            
            if (MessageBox.Show($"Are you sure you want to uninstall '{_selectedExtension.Name}'?", "Confirm Uninstall", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }
            
            if (_extensionService.UninstallExtension(_selectedExtension.Id))
            {
                MessageBox.Show($"Extension '{_selectedExtension.Name}' uninstalled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshInstalledExtensions();
                UpdateUIState();
            }
            else
            {
                MessageBox.Show($"Failed to uninstall extension '{_selectedExtension.Name}'.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}