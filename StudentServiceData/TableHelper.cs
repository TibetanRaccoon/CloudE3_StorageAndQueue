using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Diagnostics;
using System.Linq;

namespace StudentServiceData
{
    public class TableHelper
    {
        private CloudStorageAccount _storageAccount;
        private CloudTable _table;
        public TableHelper(string tableName)
        {
            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudTableClient tableClient = new CloudTableClient(new Uri(_storageAccount.TableEndpoint.AbsoluteUri), _storageAccount.Credentials);
            _table = tableClient.GetTableReference(tableName);
            if (_table.Exists() == false)
                _table.Create();
        }

        public IQueryable<Student> RetrieveAllStudents()
        {
            var results = from g in _table.CreateQuery<Student>()
                          where g.PartitionKey == "Student"
                          select g;
            return results;
        }

        public Student GetStudent(string RowKey)
        {
            foreach (var item in RetrieveAllStudents())
            {
                if (item.Index == RowKey)
                    return item;
            }
            return null;
        }

        public bool Exists(string RowKey)
        {
            foreach (var item in RetrieveAllStudents())
            {
                if (item.Index == RowKey)
                    return true;
            }
            return false;
        }

        public void InsertOrRelace(Student newStudent)
        {
            TableOperation insertOperation = TableOperation.InsertOrReplace(newStudent);
            _table.Execute(insertOperation);
        }

        public void Remove(string RowKey)
        {
            var results = from g in _table.CreateQuery<Student>()
                          where g.PartitionKey == "Student"
                          select g;

            TableOperation insertOperation = TableOperation.Delete(results.First());
            _table.Execute(insertOperation);
        }

        public void ClearTable()
        {
            _table.Delete();
            _table.Create();
        }
    }
}
