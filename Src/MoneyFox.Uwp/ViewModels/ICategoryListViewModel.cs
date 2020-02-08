﻿using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using MoneyFox.Presentation.Commands;
using MoneyFox.Presentation.Groups;

namespace MoneyFox.Presentation.ViewModels
{
    /// <summary>
    ///     Defines the interface for a category list.
    /// </summary>
    public interface ICategoryListViewModel
    {
        /// <summary>
        ///     List of categories.
        /// </summary>
        ObservableCollection<AlphaGroupListGroupCollection<CategoryViewModel>> CategoryList { get; }

        /// <summary>
        ///     Command to handle when the view is appearing
        /// </summary>
        AsyncCommand AppearingCommand { get; }

        /// <summary>
        ///     Command for the item click.
        /// </summary>
        RelayCommand<CategoryViewModel> ItemClickCommand { get; }

        /// <summary>
        ///     Search command
        /// </summary>
        AsyncCommand<string> SearchCommand { get; }

        /// <summary>
        ///     Indicates if the category list is empty.
        /// </summary>
        bool IsCategoriesEmpty { get; }
    }
}