using System;
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

            _errorService.ErrorAdded += (s, e) => {
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        AddError(e.Value);
                    }), null);
                };

            _errorService.ErrorRemoved += (s, e) => {
                    Application.Current.Dispatcher.Invoke(new Action(() => {
                        RemoveError(e.Value);
                    }), null);   
                };
        }

        public ErrorListItemVM CurrentError {
            get;
            set;
        }

        public ICommand JumpToSelectedErrorCommand {
            get {
                return new RelayCommand(x => JumpToSelectedError());
            }
        }

        void JumpToSelectedError() {
            if(CurrentError != null) {

                if(CurrentError.ErrorItem.Segment != null) {
                    CurrentError.ErrorItem
                        .Segment.BringIntoView();
                } else {
                    CurrentError.ErrorItem
                        .CodeItem.ShowInWorkSpace();
                }
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
