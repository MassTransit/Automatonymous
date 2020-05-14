namespace Automatonymous
{
    using System;
    using System.Data;
    using NHibernate.Cfg;
    using NHibernate.Cfg.Loquacious;
    using NHibernate.Dialect;


    /// <summary>
    /// Used to configure the session factory for use with SQL server
    /// </summary>
    public class SqlServerSessionFactoryProvider :
        NHibernateSessionFactoryProvider
    {
        public SqlServerSessionFactoryProvider(string connectionString, IsolationLevel isolationLevel = IsolationLevel.Serializable,
            short batchSize = 100, bool logSql = false, bool updateSchema = false, params Type[] mapTypes)
            : base(mapTypes, x => Integrate(x, connectionString, batchSize, isolationLevel, logSql, updateSchema))
        {
        }

        static void Integrate(IDbIntegrationConfigurationProperties db, string connectionString, short batchSize,
            IsolationLevel isolationLevel, bool logSql, bool updateSchema)
        {
            db.Dialect<MsSql2008Dialect>();
            db.ConnectionString = connectionString;
            db.BatchSize = batchSize;
            db.IsolationLevel = isolationLevel;
            db.LogSqlInConsole = logSql;
            db.LogFormattedSql = logSql;
            db.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
            db.SchemaAction = updateSchema ? SchemaAutoAction.Update : SchemaAutoAction.Validate;
        }
    }
}
