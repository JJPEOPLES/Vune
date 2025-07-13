using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;
using Vune.UI.Services;

namespace Vune.UI
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register services
            services.AddSingleton<IExtensionService, ExtensionService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IEditorService, EditorService>();
            services.AddSingleton<IWorkspaceService, WorkspaceService>();
            services.AddSingleton<IVSCodeImportService, VSCodeImportService>();

            // Register logging
            services.AddLogging(configure => configure.AddConsole());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize services
            var extensionService = _serviceProvider.GetRequiredService<IExtensionService>();
            var themeService = _serviceProvider.GetRequiredService<IThemeService>();
            
            // Load extensions and themes
            extensionService.Initialize();
            themeService.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider.Dispose();
            base.OnExit(e);
        }
    }
}