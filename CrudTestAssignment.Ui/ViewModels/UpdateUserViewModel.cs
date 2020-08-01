using CrudTestAssignment.Api.Api.V1.Models;
using CrudTestAssignment.Ui.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;

namespace CrudTestAssignment.Ui.ViewModels
{
    public class UpdateUserViewModel : BindableBase, IDialogAware
    {
        private readonly IApiService _apiService;

        private UserModel _user;

        private string _errorMessage;

        private string _userName;

        private string _newUserName;

        public UpdateUserViewModel(IApiService apiService)
        {
            _apiService = apiService;

            UpdateUserCommand = new DelegateCommand(async () => await ExecuteUpdateUserCommand(), CanExecuteUpdateUserCommand);
        }

        public DelegateCommand UpdateUserCommand { get; }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string NewUserName
        {
            get => _newUserName;
            set
            {
                SetProperty(ref _newUserName, value);
                UpdateUserCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task ExecuteUpdateUserCommand()
        {
            try
            {
                ErrorMessage = "";

                var result = await _apiService.UpdateUserAsync(_user.Id, _newUserName);
                {
                    var dialogParameter = new DialogParameters { { nameof(UserModel), result } };
                    OnRequestClose(new DialogResult(ButtonResult.OK, dialogParameter));
                }
            }
            catch (ServerRequestException ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private bool CanExecuteUpdateUserCommand()
        {
            return !string.IsNullOrWhiteSpace(_newUserName) && _userName.Trim().Length >= 5;
        }

        #region Dialog

        public string Title { get; } = "Update user";

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            parameters.TryGetValue<UserModel>(nameof(UserModel), out var user);

            _user = user ?? throw new ArgumentNullException(nameof(user));

            UserName = _user.Name;
        }

        public event Action<IDialogResult> RequestClose;

        protected virtual void OnRequestClose(IDialogResult obj)
        {
            RequestClose?.Invoke(obj);
        }

        #endregion
    }
}