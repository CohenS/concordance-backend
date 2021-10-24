using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Dapper;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SearchWord
{
    public class BookInfo {
       public int ID  { get; set;}
       public int BookID  { get; set;}
       public int Line  { get; set; }
       public int WordID { get; set; }
       public int Paragrath { get; set; }
    }

    public class Function
    {
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<BookInfo[]> FunctionHandler(string word, ILambdaContext context)
        {
            string wordSearchSproc = "SearchByValue";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                var result = await connection.QueryAsync<BookInfo>(wordSearchSproc,
                    new { wordValue = word },
                    commandType: CommandType.StoredProcedure);

                return result.ToArray();
            }
        }

        private string DatabaseConnectionString = "Server=concordance.c2odbwzsa8gx.us-east-2.rds.amazonaws.com;Database=concordance;Uid=admin;Pwd=Aa123456;";
    }
}
