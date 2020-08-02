using CrudTestAssignment.Api.Api.V1.Models;
using CrudTestAssignment.Ui.Exceptions;
using CrudTestAssignment.Ui.Services;
using CrudTestAssignment.Ui.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CrudTestAssignment.Ui.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IApiService _apiService;

        private readonly IDialogService _dialogService;

        private string _errorMessage;

        private bool _progressRingStatus;

        private UserModel _selectedUser;

        public MainWindowViewModel(IDialogService dialogService, IApiService apiService)
        {
            _dialogService = dialogService;
            _apiService = apiService;

            Users = new ObservableCollection<UserModel>();

            AddUserCommand = new DelegateCommand(ExecuteAddUserCommand);

            GetUserCommand = new DelegateCommand(ExecuteGetUserCommand);

            GetAllUsersCommand = new DelegateCommand(async () => await ExecuteGetAllUsersCommand());

            UpdateUserCommand = new DelegateCommand(ExecuteUpdateUserCommand, CanExecuteCommand);

            DeleteUserCommand = new DelegateCommand(async () => await ExecuteDeleteUserCommand(), CanExecuteCommand);
        }

        public ObservableCollection<UserModel> Users { get; set; }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool ProgressRingStatus
        {
            get => _progressRingStatus;
            set => SetProperty(ref _progressRingStatus, value);
        }

        public UserModel SelectedUser
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

        public DelegateCommand GetAllUsersCommand { get; }

        public DelegateCommand UpdateUserCommand { get; }

        public DelegateCommand DeleteUserCommand { get; }

        private void ExecuteAddUserCommand()
        {
            _dialogService.ShowDialog(nameof(AddUserView), new DialogParameters(), result =>
                 {
                     result.Parameters.TryGetValue<UserModel>(nameof(UserModel), out var user);

                     if (result.Result != ButtonResult.OK || user == null)
                         return;

                     Users.Add(user);
                     ErrorMessage = "";
                 });
        }

        private void ExecuteGetUserCommand()
        {
            _dialogService.ShowDialog(nameof(GetUserView), new DialogParameters(), result => { });
        }

        private async Task ExecuteGetAllUsersCommand()
        {
            try
            {
                ProgressRingStatus = true;

                var result = await _apiService.GetUsersAsync();
                if (result != null)
                    Users.AddRange(result);
            }
            catch (ServerRequestException ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                ProgressRingStatus = false;
            }
        }

        private void ExecuteUpdateUserCommand()
        {
            var index = Users.IndexOf(_selectedUser);

            var dialogParameter = new DialogParameters { { nameof(UserModel), _selectedUser } };

            try
            {
                _dialogService.ShowDialog(nameof(UpdateUserView), dialogParameter, result =>
                {
                    result.Parameters.TryGetValue<UserModel>(nameof(UserModel), out var user);

                    if (result.Result == ButtonResult.OK && user != null)
                        Users[index] = user;
                });
            }
            catch (ServerRequestException e)
            {
                ErrorMessage = e.Message;
            }
        }

        private async Task ExecuteDeleteUserCommand()
        {
            try
            {
                var result = await _apiService.DeleteUserAsync(_selectedUser.Id);
                if (result == false)
                    ErrorMessage = ErrorMessages.UserNameNotFound;
                else
                    Users.Remove(_selectedUser);
            }
            catch (ServerRequestException e)
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