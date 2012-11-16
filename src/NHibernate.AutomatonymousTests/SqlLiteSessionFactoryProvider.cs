﻿// Copyright 2011 Chris Patterson, Dru Sellers
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace NHibernate.AutomatonymousTests
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using Automatonymous.NHibernateIntegration;
    using Cache;
    using Cfg;
    using Cfg.Loquacious;
    using Dialect;
    using Tool.hbm2ddl;


    public class SqlLiteSessionFactoryProvider :
        NHibernateSessionFactoryProvider,
        IDisposable
    {
        bool _disposed;
        ISessionFactory _innerSessionFactory;
        SQLiteConnection _openConnection;
        SingleConnectionSessionFactory _sessionFactory;

        public SqlLiteSessionFactoryProvider(string connectionString, params Type[] mapTypes)
            : base(mapTypes, x => Integrate(x, connectionString))
        {
        }

        public SqlLiteSessionFactoryProvider(params Type[] mapTypes)
            : base(mapTypes, x => Integrate(x, null))
        {
            Configuration.SetProperty(Cfg.Environment.UseSecondLevelCache, "true");
            Configuration.SetProperty(Cfg.Environment.UseQueryCache, "true");
            Configuration.SetProperty(Cfg.Environment.CacheProvider,
                typeof(HashtableCacheProvider).AssemblyQualifiedName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SqlLiteSessionFactoryProvider()
        {
            Dispose(false);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                if (_openConnection != null)
                {
                    _openConnection.Close();
                    _openConnection.Dispose();
                }
            }

            _disposed = true;
        }

        public override ISessionFactory GetSessionFactory()
        {
            _innerSessionFactory = base.GetSessionFactory();

            _openConnection =
                new SQLiteConnection(Configuration.Properties[Cfg.Environment.ConnectionString]);
            _openConnection.Open();

            BuildSchema(Configuration, _openConnection);

            _innerSessionFactory.OpenSession(_openConnection);

            _sessionFactory = new SingleConnectionSessionFactory(_innerSessionFactory, _openConnection);

            return _sessionFactory;
        }

        static void BuildSchema(Configuration config, IDbConnection connection)
        {
            new SchemaExport(config).Execute(true, true, false, connection, null);
        }

        static void Integrate(IDbIntegrationConfigurationProperties db, string connectionString)
        {
            db.Dialect<SQLiteDialect>();
            db.ConnectionString = connectionString ?? "Data Source=:memory:;Version=3;New=True;Pooling=True;Max Pool Size=1;";
            db.BatchSize = 100;
            db.IsolationLevel = IsolationLevel.Serializable;
            db.LogSqlInConsole = false;
            db.LogFormattedSql = false;
            db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
            db.SchemaAction = SchemaAutoAction.Recreate;
        }
    }
}