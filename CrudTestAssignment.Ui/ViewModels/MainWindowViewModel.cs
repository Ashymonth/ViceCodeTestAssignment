using CrudTestAssignment.DAL.Models;
using CrudTestAssignment.Ui.Services;
using CrudTestAssignment.Ui.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace CrudTestAssignment.Ui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IApiService _apiService;

        private readonly IDialogService _dialogService;

        private string _errorMessage;

        private User _selectedUser;

        public MainWindowViewModel(IDialogService dialogService, IApiService apiService)
        {
            _dialogService = dialogService;
            _apiService = apiService;

            Users = new ObservableCollection<User>();

            AddUserCommand = new DelegateCommand(ExecuteAddUserCommand);

            GetUserCommand = new DelegateCommand(ExecuteGetUserCommand);

            UpdateUserCommand = new DelegateCommand(ExecuteUpdateUserCommand, CanExecuteCommand);

            DeleteUserCommand = new DelegateCommand(async ()=> await ExecuteDeleteUserCommand(),CanExecuteCommand);
        }

        public ObservableCollection<User> Users { get; set; }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                UpdateUserCommand.RaiseCanExecuteChanged();
                DeleteUserCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand AddUserCommand { get; }

        public DelegateCommand GetUserCommand { get; }

        public DelegateCommand UpdateUserCommand { get; }

        public DelegateCommand DeleteUserCommand { get; }

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
            _dialogService.ShowDialog(nameof(GetUserView), new DialogParameters(), result => { });
        }

        private void ExecuteUpdateUserCommand()
        {
            var index = Users.IndexOf(_selectedUser);

            var dialogParameter = new DialogParameters { { nameof(User), _selectedUser } };

            try
            {
                _dialogService.ShowDialog(nameof(UpdateUserView), dialogParameter, result =>
                {
                    result.Parameters.TryGetValue<User>(nameof(User), out var user);

                    if (result.Result == ButtonResult.OK && user != null)
                        Users[index] = user;
                });
            }
            catch (ArgumentNullException e)
            {
                ErrorMessage = e.Message;
            }
        }

        private async Task ExecuteDeleteUserCommand()
        {
            try
            {
                ErrorMessage = "";

                var result = await _apiService.DeleteUserAsync(_selectedUser.Id);
                if (result == false)
                    ErrorMessage = "User not found";
                else
                    Users.Remove(_selectedUser);
            }
            catch (HttpRequestException e)
            {
                ErrorMessage = e.Message;
            }
        }

        private bool CanExecuteCommand()
        {
            return _selectedUser != null;
        }
    }
}