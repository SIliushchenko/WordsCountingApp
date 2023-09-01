using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using WordsCountingApp.Services.PathSelection;
using WordsCountingApp.ViewModels;
using WordsExtraction.Services;

namespace WordsCountingApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<ShellWindow>();
                    services.AddSingleton<ShellViewModel>();
                    services.AddSingleton<WordsCounterViewModel>();
                    services.AddTransient<IFilePathSelector, FilePathSelector>();
                    services.AddTransient<IWordsExtractorService, WordsExtractorService>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<ShellWindow>();
            var shellVm = AppHost.Services.GetRequiredService<ShellViewModel>();
            var wordsCounterVm = AppHost.Services.GetRequiredService<WordsCounterViewModel>();
            shellVm.SetContent(wordsCounterVm);
            startupForm.DataContext = shellVm;

            startupForm.Show();
            
            base.OnStartup(e);
        }


        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}
