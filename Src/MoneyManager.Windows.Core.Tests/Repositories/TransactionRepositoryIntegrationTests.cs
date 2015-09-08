﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MoneyManager.Core;
using MoneyManager.Core.DataAccess;
using MoneyManager.Core.Repositories;
using MoneyManager.Foundation;
using MoneyManager.Foundation.Model;
using MoneyManager.Foundation.OperationContracts;
using MoneyManager.Windows.Core.Tests.Helper;
using SQLite.Net.Platform.WinRT;
using SQLiteNetExtensions.Extensions;
using Xunit;

namespace MoneyManager.Windows.Core.Tests.Repositories
{
    public class TransactionRepositoryIntegrationTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void TransactionRepository_LoadDataFromDbThroughRepository()
        {
            var dbHelper = new DbHelper(new SQLitePlatformWinRT(), new TestDatabasePath());

            using (var db = dbHelper.GetSqlConnection())
            {
                db.DeleteAll<FinancialTransaction>();
                db.InsertWithChildren(new FinancialTransaction
                {
                    Amount = 999,
                    ChargedAccount = new Account
                    {
                        Name = "testAccount"
                    }
                });
            }

            var repository = new TransactionRepository(new TransactionDataAccess(dbHelper), new RecurringTransactionDataAccess(dbHelper));

            repository.Data.Any().ShouldBeTrue();
            repository.Data[0].Amount.ShouldBe(999);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void TransactionRepository_Update()
        {
            var dbHelper = new DbHelper(new SQLitePlatformWinRT(), new TestDatabasePath());

            using (var db = dbHelper.GetSqlConnection())
            {
                db.DeleteAll<FinancialTransaction>();
            }

            var repository = new TransactionRepository(new TransactionDataAccess(dbHelper), new RecurringTransactionDataAccess(dbHelper));
            var account = new Account
            {
                Name = "TestAccount"
            };

            var transaction = new FinancialTransaction
            {
                ChargedAccount = account,
                Amount = 20
            };

            repository.Save(transaction);
            repository.Data.Count.ShouldBe(1);
            repository.Data[0].ShouldBeSameAs(transaction);

            transaction.Amount = 30;

            repository.Save(transaction);

            repository.Data.Count.ShouldBe(1);
            repository.Data[0].Amount.ShouldBe(30);
        }

        [Fact]
        public void LoadRecurringList_ListWithRecurringTransaction()
        {
            var transactionDataAccess = new TransactionDataAccess(new DbHelper(new SQLitePlatformWinRT(), new TestDatabasePath()));
            var recTransactionDataAccess = new RecurringTransactionDataAccess(new DbHelper(new SQLitePlatformWinRT(), new TestDatabasePath()));
            var repository = new TransactionRepository(transactionDataAccess, recTransactionDataAccess);

            transactionDataAccess.Save(new FinancialTransaction { Id = 3, Amount = 999, IsRecurring = false });
            transactionDataAccess.Save(new FinancialTransaction { Id = 4, Amount = 123, IsRecurring = true, RecurringTransaction = new RecurringTransaction { Id = 12 } });

            var result = repository.LoadRecurringList().ToList();

            result.Count.ShouldBe(1);

            result.First().Id.ShouldBe(4);
            result.First().RecurringTransaction.Id.ShouldBe(12);
        }

    }
}