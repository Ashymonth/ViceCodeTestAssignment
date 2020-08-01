using System;
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

        private User _selectedUser;

        public MainWindowViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Users = new ObservableCollection<User>();

            AddUserCommand = new DelegateCommand(ExecuteAddUserCommand);

            GetUserCommand = new DelegateCommand(ExecuteGetUserCommand);

            UpdateUserCommand = new DelegateCommand(ExecuteUpdateUserCommand, CanExecuteUpdateUserCommand);
        }

        public ObservableCollection<User> Users { get; set; }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                UpdateUserCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand AddUserCommand { get; }

        public DelegateCommand GetUserCommand { get; }

        public DelegateCommand UpdateUserCommand { get; }

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
                
            }
        }

        private bool CanExecuteUpdateUserCommand()
        {
            return _selectedUser != null;
        }
    }
}