using CrudTestAssignment.Api.Api.V1.Models;
using CrudTestAssignment.Ui.Exceptions;
using CrudTestAssignment.Ui.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;

namespace CrudTestAssignment.Ui.ViewModels
{
    public class AddUserViewModel : BindableBase, IDialogAware
    {
        private readonly IApiService _apiService;

        private string _errorMessage;

        private string _userName;

        public AddUserViewModel(IApiService apiService)
        {
            _apiService = apiService;

            AddUserCommand = new DelegateCommand(async () => await ExecuteAddUserCommand(), CanExecuteAddUserCommand);
        }

        public DelegateCommand AddUserCommand { get; }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string UserName
        {
            get => _userName;
            set
            {
                SetProperty(ref _userName, value);
                AddUserCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task ExecuteAddUserCommand()
        {
            try
            {
                var result = await _apiService.CreateUserAsync(_userName);
                var dialogParameter = new DialogParameters { { nameof(UserModel), result } };

                OnRequestClose(new DialogResult(ButtonResult.OK, dialogParameter));
            }
            catch (ServerRequestException ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private bool CanExecuteAddUserCommand()
        {
            return !string.IsNullOrWhiteSpace(_userName);
        }

        #region Dialog

        public string Title { get; } = "Add user";

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        public void OnDialogOpened(IDialogParameters parameters) { }

        public event Action<IDialogResult> RequestClose;

        protected virtual void OnRequestClose(IDialogResult obj)
        {
            RequestClose?.Invoke(obj);
        }

        #endregion
    }
}