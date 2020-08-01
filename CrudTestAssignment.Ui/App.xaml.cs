using System.Configuration;
using Prism.Ioc;
using CrudTestAssignment.Ui.Views;
using System.Windows;
using CrudTestAssignment.Ui.Services;
using CrudTestAssignment.Ui.ViewModels;

namespace CrudTestAssignment.Ui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var serverUrl = ConfigurationManager.AppSettings["serverUrl"];
            if (string.IsNullOrWhiteSpace(serverUrl))
                throw new ConfigurationErrorsException("Missing server url in app.config");

            var apiClient = new ApiService(serverUrl);

            containerRegistry.RegisterInstance(typeof(IApiService), apiClient);

            containerRegistry.RegisterDialog<AddUserView, AddUserViewModel>();

            containerRegistry.RegisterDialog<GetUserView, GetUserViewModel>();

            containerRegistry.RegisterDialog<UpdateUserView, UpdateUserViewModel>();
        }
    }
}
