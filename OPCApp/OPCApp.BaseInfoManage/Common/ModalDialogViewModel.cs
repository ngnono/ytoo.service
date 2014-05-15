using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Microsoft.Practices.Prism.Mvvm;

namespace Intime.OPC.Modules.Dimension.Common
{
    public abstract class ModalDialogViewModel<TDimension> : ViewModelBase, INotification, IInteractionRequestAware
        where TDimension : Intime.OPC.Modules.Dimension.Models.Dimension, new()
    {
        private TDimension model;

        public ModalDialogViewModel()
        {
            OKCommand = new DelegateCommand(() =>
            {
                Accepted = true;
                FinishInteraction();
            });
            CancelCommand = new DelegateCommand(() =>
            {
                Accepted = false;
                FinishInteraction();
            });
        }

        public virtual TDimension Model 
        {
            get { return model; }
            set { SetProperty(ref model, value); } 
        }

        public ICommand OKCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public bool Accepted { get; set; }

        public Action FinishInteraction { get; set; }

        public INotification Notification { get; set; }

        public object Content { get; set; }

        public string Title { get; set; }
    }
}
