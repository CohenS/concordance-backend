using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Dapper;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace concordance_backend
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
        public BookInfo[] SearchByValueHandler(string word, ILambdaContext context)
        {
            string wordSearchSproc = "[dbo].[SearchByValue]";

            using (var connection = new SqlConnection("connectionString"))
            {
                var result = connection.QueryFirst<BookInfo[]>(wordSearchSproc,
                    new { wordValue = word },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
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

            using (var connection = new SqlConnection("connectionString"))
            {
                var result = connection.QueryFirst<BookInfo[]>(wordSearchSproc,
                    new { Line = location.Line, Paragraph = location.Paragraph, Chapter = location.Chapter },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public BookInfo[] SearchByLocation(string bookName, ILambdaContext context)
        {
            string wordSearchSproc = "SearchBybook";

            using (var connection = new SqlConnection("connectionString"))
            {
                var result = connection.QueryFirst<BookInfo[]>(wordSearchSproc,
                    new { BookName = bookName },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        public class BookInsertInformation
        {
            public string BookName { get; set; }
            public string Author { get; set; }
            public string PublishedDate { get; set; }
            public string[][][][] Words { get; set; }
        }

        public class Paragraph
        {
            public string Value { get; set; }
            public int ParagraphNumber  { get; set; }
            public int BookID { get; set; }
        }

        public class Word
        {
            public string Value { get; set; }
            public int Paragraph { get; set; }
            public int Chapter { get; set; }
            public int Line { get; set; }
            public int WordNumber { get; set; }
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<int> FunctionHandler(BookInsertInformation bookInfo, ILambdaContext context)
        {
            var words = 
                bookInfo.Words
                .Select((chapter, cIndex) =>
                    chapter.Select((paragraph, pIndex) =>
                        paragraph.Select((line, lineIndex) =>
                            line.Select((word, wordIndex) =>
                                new { Chapter = cIndex, Paragraph = pIndex, Line = lineIndex, WordNumber = wordIndex, Value = word })))).SelectMany(x => x).SelectMany(x => x).SelectMany(x => x);
            
            string wordSearchSproc = "[dbo].[InsertBook]";

            var wordsDataTable = new DataTable();
            wordsDataTable.Columns.Add("WordValue", typeof(string));
            wordsDataTable.Columns.Add("Paragraph", typeof(Int32));
            wordsDataTable.Columns.Add("Chapter", typeof(Int32));
            wordsDataTable.Columns.Add("Line", typeof(Int32));
            wordsDataTable.Columns.Add("WordNumber", typeof(Int32));

            foreach (var w in words)
            {
                DataRow wordRow = wordsDataTable.NewRow();
                wordRow["WordValue"] = w.Value;
                wordRow["Paragraph"] = w.Paragraph;
                wordRow["Chapter"] = w.Chapter;
                wordRow["Line"] = w.Line;
                wordRow["WordNumber"] = w.WordNumber;
                wordsDataTable.Rows.Add(wordRow);
            }

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                var result = await connection.QueryFirstAsync<int>(wordSearchSproc,
                    new
                    {
                        BookName = bookInfo.BookName,
                        Author = bookInfo.Author,
                        PublishedDate = bookInfo.PublishedDate,
                        Words = wordsDataTable.AsTableValuedParameter("[Words]")
                    },
                    commandTimeout:30,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
        }

        private string DatabaseConnectionString = "Server=concordance.c2odbwzsa8gx.us-east-2.rds.amazonaws.com;Database=concordance;Uid=admin;Pwd=Aa123456;";
    }

}
