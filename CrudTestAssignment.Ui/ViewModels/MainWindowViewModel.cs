using CrudTestAssignment.DAL.Models;
using CrudTestAssignment.Ui.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;

namespace CrudTestAssignment.Ui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;

        public MainWindowViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Users = new ObservableCollection<User>();

            AddUserCommand = new DelegateCommand(ExecuteAddUserCommand);

            GetUserCommand = new DelegateCommand(ExecuteGetUserCommand);
        }

        public ObservableCollection<User> Users { get; set; }

        public DelegateCommand AddUserCommand { get; }

        public DelegateCommand GetUserCommand { get; }

        private void ExecuteAddUserCommand()
        {
            _dialogService.ShowDialog(nameof(AddUserView), new DialogParameters(), result =>
                 {
                     result.Parameters.TryGetValue<User>(nameof(User), out var user);

                     if (result.Result == ButtonResult.OK && user != null)
                         Users.Add(user);
                 });
        }

        private void ExecuteGetUserCommand()
        {
            _dialogService.ShowDialog(nameof(GetUserView),new DialogParameters(),result => {} );
        }
    }
}