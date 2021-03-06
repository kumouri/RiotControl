﻿using System.Data.Common;

namespace RiotControl
{
	public class Database
	{
		DbProviderFactory Factory;
		string Path;

		public Database(string path)
		{
			Factory = DbProviderFactories.GetFactory("System.Data.SQLite");
			Path = path;
		}

		public DbConnection GetConnection()
		{
			DbConnection connection = Factory.CreateConnection();
			connection.ConnectionString = string.Format("Data Source = {0}", Path);
			connection.Open();
			//Turn on SQLite foreign keys
			using (var pragma = new DatabaseCommand("pragma foreign_keys = on", connection))
			{
				pragma.Execute();
			}
			return connection;
		}
	}
}
