using System.Data;
using System.Data.SqlClient;
using Amazon.Lambda.Core;
using Dapper;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SearchByBookName
{
    public class Function
    {
        public class BookInfo
        {
            public int ID { get; set; }
            public int BookID { get; set; }
            public int Line { get; set; }
            public int WordID { get; set; }
            public int Paragrath { get; set; }
        }

        public BookInfo[] SearchByBookName(string bookName, ILambdaContext context)
        {
            string wordSearchSproc = "SearchBybook";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                var result = connection.QueryFirst<BookInfo[]>(wordSearchSproc,
                    new { BookName = bookName },
                    commandType: CommandType.StoredProcedure);

                return result;
            }

        }

        private string DatabaseConnectionString = "Server=concordance.c2odbwzsa8gx.us-east-2.rds.amazonaws.com;Database=concordance;Uid=admin;Pwd=Aa123456;";
    }
}
