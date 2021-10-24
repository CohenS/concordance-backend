using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Dapper;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SearchByLocation
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

        public class Location
        {
            public int Line { get; set; }
            public int Paragraph { get; set; }
            public int Chapter { get; set; }
        }

        public BookInfo[] SearchByLocation(Location location, ILambdaContext context)
        {
            string wordSearchSproc = "SearchByLocation";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                var result = connection.QueryFirst<BookInfo[]>(wordSearchSproc,
                    new { Line = location.Line, Paragraph = location.Paragraph, Chapter = location.Chapter },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        private string DatabaseConnectionString = "Server=concordance.c2odbwzsa8gx.us-east-2.rds.amazonaws.com;Database=concordance;Uid=admin;Pwd=Aa123456;";
    }
}
