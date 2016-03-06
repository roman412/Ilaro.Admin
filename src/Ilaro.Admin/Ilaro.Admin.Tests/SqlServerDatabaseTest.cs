﻿using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Ilaro.Admin.Tests.Utils;
using Simple.Data;
using System.Web.Mvc;
using FakeItEasy;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Tests
{
    public class SqlServerDatabaseTest
    {
        protected dynamic DB { get; private set; }

        protected string ConnectionStringName
        {
            get { return "IlaroTestDb"; }
        }

        public SqlServerDatabaseTest()
        {
            RecreateDatabase();

            DB = Database.OpenNamedConnection(ConnectionStringName);
        }

        private void RecreateDatabase()
        {
            var connectionString =
                ConfigurationManager.ConnectionStrings["Init"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                DropAllObjects(connection);

                CreateDatabase(connection);
            }
        }

        protected virtual void CreateDatabase(DbConnection connection)
        {
            DatabaseCommandExecutor.ExecuteScript(
                  TestUtils.GetDatabaseScript(@"CreateDatabase.sql"),
                  connection
            );
        }

        protected virtual void DropAllObjects(IDbConnection connection)
        {
            DatabaseCommandExecutor.ExecuteScript(
                  TestUtils.GetDatabaseScript(@"DropAllObjects.sql"),
                  connection
            );
        }

        protected virtual void SetFakeResolver()
        {
            var resolver = A.Fake<IDependencyResolver>();
            DependencyResolver.SetResolver(resolver);
            A.CallTo(() => resolver.GetService(typeof(IConfiguration)))
                .Returns(A.Fake<IConfiguration>());
        }
    }
}
