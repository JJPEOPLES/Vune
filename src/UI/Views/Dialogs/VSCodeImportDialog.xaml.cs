using Microsoft.Win32;
using System.Windows;
using Vune.UI.Services;

namespace Vune.UI.Views.Dialogs
{
    public partial class VSCodeImportDialog : Window
    {
        private readonly IVSCodeImportService _vsCodeImportService;

        public VSCodeImportDialog(IVSCodeImportService vsCodeImportService)
        {
            InitializeComponent();
            
            _vsCodeImportService = vsCodeImportService;
            
            // Try to detect VS Code path
            DetectVSCodePath();
        }

        private void DetectVSCodePath()
        {
            string vscodePath = _vsCodeImportService.DetectVSCodePath();
            
            if (!string.IsNullOrEmpty(vscodePath))
            {
                VSCodePathTextBox.Text = vscodePath;
                StatusTextBlock.Text = "VS Code installation detected.";
            }
            else
            {
                StatusTextBlock.Text = "Could not detect VS Code installation. Please specify the path manually.";
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select VS Code Installation Folder"
            };
            
            if (dialog.ShowDialog() == true)
            {
                VSCodePathTextBox.Text = dialog.FolderName;
            }
        }

        private void DetectButton_Click(object sender, RoutedEventArgs e)
        {
            DetectVSCodePath();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            string vscodePath = VSCodePathTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(vscodePath))
            {
                MessageBox.Show("Please specify the VS Code installation path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            bool importSettings = ImportSettingsCheckBox.IsChecked == true;
            bool importExtensions = ImportExtensionsCheckBox.IsChecked == true;
            bool importThemes = ImportThemesCheckBox.IsChecked == true;
            
            if (!importSettings && !importExtensions && !importThemes)
            {
                MessageBox.Show("Please select at least one import option.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            StatusTextBlock.Text = "Importing data from VS Code...";
            ImportButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            
            // Perform import
            bool success = false;
            
            if (importSettings && importExtensions && importThemes)
            {
                success = _vsCodeImportService.ImportAll(vscodePath);
            }
            else
            {
                if (importSettings)
                {
                    success = _vsCodeImportService.ImportSettings(vscodePath);
                }
                
                if (importExtensions)
                {
                    success = success && _vsCodeImportService.ImportExtensions(vscodePath);
                }
                
                if (importThemes)
                {
                    success = success && _vsCodeImportService.ImportThemes(vscodePath);
                }
            }
            
            if (success)
            {
                MessageBox.Show("VS Code data imported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            else
            {
                StatusTextBlock.Text = "Failed to import data from VS Code. Please check the path and try again.";
                MessageBox.Show("Failed to import data from VS Code.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ImportButton.IsEnabled = true;
                CancelButton.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}