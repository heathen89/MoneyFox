﻿using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediatR;
using MoneyFox.Application.Accounts.Commands.DeleteAccountById;
using MoneyFox.Application.Accounts.Queries.GetAccountCount;
using MoneyFox.Application.Common.Facades;
using MoneyFox.Application.Common.Interfaces;
using MoneyFox.Application.Common.Messages;
using MoneyFox.Application.Resources;
using MoneyFox.Domain;
using MoneyFox.Presentation.Services;
using MoneyFox.Presentation.ViewModels.Interfaces;
using MoneyFox.Ui.Shared.Commands;

namespace MoneyFox.Presentation.ViewModels
{
    /// <inheritdoc cref="IPaymentListViewActionViewModel" />
    /// />
    public class PaymentListViewActionViewModel : ViewModelBase, IPaymentListViewActionViewModel
    {
        private readonly IMediator mediator;
        private readonly ISettingsFacade settingsFacade;
        private readonly IDialogService dialogService;
        private readonly IBalanceViewModel balanceViewModel;
        private readonly INavigationService navigationService;

        private readonly int accountId;
        private bool isClearedFilterActive;
        private bool isRecurringFilterActive;
        private DateTime timeRangeStart = DateTime.Now.AddMonths(-2);
        private DateTime timeRangeEnd = DateTime.Now.AddMonths(6);
        private bool isTransferAvailable;
        private bool isAddIncomeAvailable;
        private bool isAddExpenseAvailable;

        /// <summary>
        ///     Constructor
        /// </summary>
        public PaymentListViewActionViewModel(int accountId,
                                              IMediator mediator,
                                              ISettingsFacade settingsFacade,
                                              IDialogService dialogService,
                                              IBalanceViewModel balanceViewModel,
                                              INavigationService navigationService)
        {
            this.accountId = accountId;

            this.mediator = mediator;
            this.settingsFacade = settingsFacade;
            this.dialogService = dialogService;
            this.balanceViewModel = balanceViewModel;
            this.navigationService = navigationService;

#pragma warning disable S4462 // Calls to "async" methods should not be blocking
            int accountCount = mediator.Send(new GetAccountCountQuery()).Result;
#pragma warning restore S4462 // Calls to "async" methods should not be blocking

            IsTransferAvailable = accountCount >= 2;
            IsAddIncomeAvailable = accountCount >= 1;
            IsAddExpenseAvailable = accountCount >= 1;
        }

        /// <inheritdoc />
        public RelayCommand GoToAddIncomeCommand =>
            new RelayCommand(() => navigationService.NavigateTo(ViewModelLocator.AddPayment, PaymentType.Income));

        /// <inheritdoc />
        public RelayCommand GoToAddExpenseCommand =>
            new RelayCommand(() => navigationService.NavigateTo(ViewModelLocator.AddPayment, PaymentType.Expense));

        /// <inheritdoc />
        public RelayCommand GoToAddTransferCommand =>
            new RelayCommand(() => navigationService.NavigateTo(ViewModelLocator.AddPayment, PaymentType.Transfer));

        /// <inheritdoc />
        public AsyncCommand DeleteAccountCommand => new AsyncCommand(DeleteAccount);

        /// <summary>
        ///     Indicates if the transfer option is available or if it shall be hidden.
        /// </summary>
        public bool IsTransferAvailable
        {
            get => isTransferAvailable;
            set
            {
                if (isTransferAvailable == value) return;
                isTransferAvailable = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Indicates if the button to add new income should be enabled.
        /// </summary>
        public bool IsAddIncomeAvailable
        {
            get => isAddIncomeAvailable;
            set
            {
                if (isAddIncomeAvailable == value) return;
                isAddIncomeAvailable = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     Indicates if the button to add a new expense should be enabled.
        /// </summary>
        public bool IsAddExpenseAvailable
        {
            get => isAddExpenseAvailable;
            set
            {
                if (IsAddExpenseAvailable == value) return;
                isAddExpenseAvailable = value;
                RaisePropertyChanged();
            }
        }

        /// <inheritdoc />
        public bool IsClearedFilterActive
        {
            get => isClearedFilterActive;
            set
            {
                if (isClearedFilterActive == value) return;
                isClearedFilterActive = value;
                RaisePropertyChanged();
                UpdateList();
            }
        }

        /// <inheritdoc />
        public bool IsRecurringFilterActive
        {
            get => isRecurringFilterActive;
            set
            {
                if (isRecurringFilterActive == value) return;
                isRecurringFilterActive = value;
                RaisePropertyChanged();
                UpdateList();
            }
        }

        /// <inheritdoc />
        public DateTime TimeRangeStart
        {
            get => timeRangeStart;
            set
            {
                if (timeRangeStart == value) return;
                timeRangeStart = value;
                RaisePropertyChanged();
                UpdateList();
            }
        }

        /// <inheritdoc />
        public DateTime TimeRangeEnd
        {
            get => timeRangeEnd;
            set
            {
                if (timeRangeEnd == value) return;
                timeRangeEnd = value;
                RaisePropertyChanged();
                UpdateList();
            }
        }

        private async Task DeleteAccount()
        {
            if (await dialogService.ShowConfirmMessageAsync(Strings.DeleteTitle, Strings.DeleteAccountConfirmationMessage))
            {
                await mediator.Send(new DeleteAccountByIdCommand(accountId));
                settingsFacade.LastDatabaseUpdate = DateTime.Now;
                navigationService.GoBack();
            }

            await balanceViewModel.UpdateBalanceCommand.ExecuteAsync();
        }

        private void UpdateList()
        {
            MessengerInstance.Send(new PaymentListFilterChangedMessage
            {
                IsClearedFilterActive = IsClearedFilterActive,
                IsRecurringFilterActive = IsRecurringFilterActive,
                TimeRangeStart = TimeRangeStart,
                TimeRangeEnd = TimeRangeEnd
            });
        }
    }
}
