﻿using System.Collections.Generic;
using GalaSoft.MvvmLight.Command;
using MoneyFox.Presentation.Models;

namespace MoneyFox.Presentation.ViewModels.Statistic
{
    public interface IStatisticSelectorViewModel
    {
        List<StatisticSelectorType> StatisticItems { get; }

        RelayCommand<StatisticSelectorType> GoToStatisticCommand { get; }
    }
}
