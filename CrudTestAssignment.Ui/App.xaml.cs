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
            containerRegistry.Register<IApiService, ApiService>();

            containerRegistry.RegisterDialog<AddUserView, AddUserViewModel>();
        }
    }
}
