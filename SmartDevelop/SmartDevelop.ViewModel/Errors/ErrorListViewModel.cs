﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using SmartDevelop.Model.Errors;
using Archimedes.Patterns.WPF.ViewModels;
using System.Windows;
using Archimedes.Patterns.Utils;
using System.Windows.Input;
using Archimedes.Patterns.WPF.Commands;

namespace SmartDevelop.ViewModel.Errors
{
    public class ErrorListViewModel : WorkspaceViewModel
    {
        IErrorService _errorService;

        public ErrorListViewModel(IErrorService errorService) {

            ThrowUtil.ThrowIfNull(errorService);
            _errorService = errorService;


            AllErrors = new ObservableCollection<ErrorListItemVM>();

            ImportExisting();

            _errorService.ErrorAdded += (s, e) => SyncInvoke(() => AddError(e.Value));
            _errorService.ErrorRemoved += (s, e) => SyncInvoke(() => RemoveError(e.Value));
        }

        public ErrorListItemVM CurrentError {
            get;
            set;
        }

        public ICommand JumpToSelectedErrorCommand {
            get {
                return new RelayCommand(x => {
                    if(CurrentError != null) {
                        CurrentError.ErrorItem.BringIntoView();
                    }
                }, x => (CurrentError != null));
            }
        }

        void ImportExisting() {
            _errorService.GetAllErrors().ToList()
                .ForEach(x => AddError(x));
        }

        void AddError(ErrorItem error) {
            AllErrors.Add(new ErrorListItemVM(error));
        }

        void RemoveError(ErrorItem error) {
            var errorVM = AllErrors.ToList().Find(x => x.ErrorItem.Equals(error));
            AllErrors.Remove(errorVM);
        }

        public ObservableCollection<ErrorListItemVM> AllErrors {
            get;
            protected set;
        }
    }
}
