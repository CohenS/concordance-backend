	using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace concordance_backend.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class BookController : ControllerBase
	{

		public BookController()
		{
		}

		private string DatabaseConnectionString = "Server=concordance.c2odbwzsa8gx.us-east-2.rds.amazonaws.com;Database=concordance;Uid=admin;Pwd=Aa123456;";

		[HttpGet(Name = "SearchByBookName")]
		public IEnumerable<BookInfo> SearchByBookName(string bookName)
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
		
		[HttpGet(Name = "SearchByLocation")]
		public IEnumerable<BookInfo> SearchByLocation(Location location)
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

		[HttpGet(Name = "SearchByWord")]
		public async Task<BookInfo[]> SearchByWord(string word)
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

		[HttpGet(Name = "SearchByWord")]
		public async Task<BookInfo[]> SearchByWord(string word)
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
	}

	public class Location
	{
		public int Line { get; set; }
		public int Paragraph { get; set; }
		public int Chapter { get; set; }
	}

	public class BookInfo
	{
		public int ID { get; set; }
		public int BookID { get; set; }
		public int Line { get; set; }
		public int WordID { get; set; }
		public int Paragrath { get; set; }
	}
}