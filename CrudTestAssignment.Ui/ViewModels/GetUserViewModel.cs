using CrudTestAssignment.Ui.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CrudTestAssignment.Ui.ViewModels
{
    public class GetUserViewModel : BindableBase, IDialogAware
    {
        private readonly IApiService _apiService;

        private string _resultMessage;

        private string _userName;

        public GetUserViewModel(IApiService apiService)
        {
            _apiService = apiService;

            GetUserCommand = new DelegateCommand(async () => await ExecuteGetUserCommand(), CanExecuteGetUserCommand);
        }

        public DelegateCommand GetUserCommand { get; }

        public string ResultMessage
        {
            get => _resultMessage;
            set => SetProperty(ref _resultMessage, value);
        }

        public string UserName
        {
            get => _userName;
            set
            {
                SetProperty(ref _userName, value);
                GetUserCommand.RaiseCanExecuteChanged();
            }
        }

        private async Task ExecuteGetUserCommand()
        {
            try
            {
                var result = await _apiService.GetUserByNameAsync(_userName);
                ResultMessage = result == null ? "User not find" : $"User with name: {result.Name} find";
            }
            catch (HttpRequestException e)
            {
                ResultMessage = e.Message;
            }
        }

        private bool CanExecuteGetUserCommand()
        {
            return !string.IsNullOrWhiteSpace(_userName);
        }

        #region Dialog
        public string Title { get; } = "Get User";

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