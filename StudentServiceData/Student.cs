using Microsoft.WindowsAzure.Storage.Table;

namespace StudentServiceData
{
    public class Student : TableEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string JMBG { get; set; }
        public string Index { get; set; }
        public string ThumbnailUrl { get; set; }

        public Student(string rowKey)
        {
            PartitionKey = "Student";
            RowKey = rowKey;
        }
        public Student() { }
    }
}
