using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;
using Ttg.MvcCodeGenerator.Domain;

namespace Ttg.MvcCodeGenerator.Service
{
    public class DatabaseSchemeService
    {
        public IEnumerable<string> GetDatabaseTables(string connectionString)
        {
            if (connectionString == null)
            {
                return new List<string>();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                DataTable schema = connection.GetSchema("Tables");

                List<string> TableNames = new List<string>();

                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row[2].ToString());
                }

                TableNames.Sort();

                return TableNames;
            }
        }

        public List<TableColumn> GetTableColumns(string connectionString, string table)
        {
            List<TableColumn> tableColumns = new List<TableColumn>();

            if (connectionString == null)
            {
                return tableColumns;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                DbCommand command = connection.CreateCommand();
                command.CommandText = "select * from " + table + " where 1 = 0";
                command.CommandType = CommandType.Text;

                DbDataReader reader = command.ExecuteReader();
                DataTable schemaTable = reader.GetSchemaTable();

                if (schemaTable != null)
                {
                    foreach (DataRow row in schemaTable.Rows)
                    {
                        TableColumn tableColumn = new TableColumn();

                        tableColumn.Name = row.Field<string>("ColumnName");
                        tableColumn.Type = row.Field<Type>("DataType");
                        tableColumn.ColumnLength = row.Field<int>("ColumnSize");

                        tableColumns.Add(tableColumn);
                    }
                }

                connection.Close();
            }

            return tableColumns;
        }

        public IEnumerable<string> GetDatabaseColumns(string connectionString, string tableName)
        {
            if (connectionString == null)
            {
                return new List<string>();
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                DataTable schema = connection.GetSchema("Tables");

                List<string> TableNames = new List<string>();

                foreach (DataRow row in schema.Rows)
                {
                    TableNames.Add(row[2].ToString());
                }

                TableNames.Sort();

                return TableNames;
            }
        }

        public NameValueCollection GetConnectionStrings(string webConfig)
        {
            if (webConfig == null)
            {
                return new NameValueCollection();
            }

            XmlDocument doc = new XmlDocument();

            doc.Load(webConfig);

            XmlNode connectionStrings = doc.SelectSingleNode("/configuration/connectionStrings");

            NameValueCollection nameValueCollection = new NameValueCollection();

            if (connectionStrings != null)
            {
                foreach (XmlNode item in connectionStrings.ChildNodes)
                {
                    if (item.Attributes != null)
                    {
                        nameValueCollection.Add(item.Attributes["name"].Value, item.Attributes["connectionString"].Value);
                    }
                }
            }

            return nameValueCollection;
        }
    }
}
