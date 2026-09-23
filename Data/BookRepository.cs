using System.Data;
using HomeLibrary.Models;
using Microsoft.Data.SqlClient;

namespace HomeLibrary.Data;

public class BookRepository
{
    private readonly string _connectionString;

    public BookRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Book> GetAll()
    {
        var books = new List<Book>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("dbo.Books_SelectAll", connection);

        command.CommandType = CommandType.StoredProcedure;

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var book = new Book
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),

                Title = reader.GetString(reader.GetOrdinal("Title")),

                Author = reader.GetString(reader.GetOrdinal("Author")),

                PublicationYear =
                    reader.GetInt32(reader.GetOrdinal("PublicationYear")),

                Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),

                ContentsXml = reader["ContentsXml"] == DBNull.Value ?
                    string.Empty :
                    reader["ContentsXml"].ToString() ?? string.Empty,

                ContentsFilePath = reader["ContentsFilePath"] == DBNull.Value ?
                    null :
                    reader["ContentsFilePath"].ToString()
            };

            books.Add(book);
        }

        return books;
    }

    public int Insert(Book book)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("dbo.Books_Insert", connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add("@Title", SqlDbType.NVarChar, 300)
            .Value = book.Title;

        command.Parameters.Add("@Author", SqlDbType.NVarChar, 300)
            .Value = book.Author;

        command.Parameters.Add("@PublicationYear", SqlDbType.Int)
            .Value = book.PublicationYear;

        command.Parameters.Add("@Description", SqlDbType.NVarChar, -1)
            .Value = (object?)book.Description ?? DBNull.Value;

        command.Parameters.Add("@ContentsXml", SqlDbType.Xml)
            .Value = string.IsNullOrWhiteSpace(book.ContentsXml)
                ? DBNull.Value
                : book.ContentsXml;

        command.Parameters.Add("@ContentsFilePath", SqlDbType.NVarChar, 500)
            .Value = (object?)book.ContentsFilePath ?? DBNull.Value;

        connection.Open();

        var result = command.ExecuteScalar();

        return Convert.ToInt32(result);
    }

    public void Update(Book book)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("dbo.Books_Update", connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add("@Id", SqlDbType.Int)
            .Value = book.Id;

        command.Parameters.Add("@Title", SqlDbType.NVarChar, 300)
            .Value = book.Title;

        command.Parameters.Add("@Author", SqlDbType.NVarChar, 300)
            .Value = book.Author;

        command.Parameters.Add("@PublicationYear", SqlDbType.Int)
            .Value = book.PublicationYear;

        command.Parameters.Add("@Description", SqlDbType.NVarChar, -1)
            .Value = (object?)book.Description ?? DBNull.Value;

        command.Parameters.Add("@ContentsXml", SqlDbType.Xml)
            .Value = string.IsNullOrWhiteSpace(book.ContentsXml)
                ? DBNull.Value
                : book.ContentsXml;

        command.Parameters.Add("@ContentsFilePath", SqlDbType.NVarChar, 500)
            .Value = (object?)book.ContentsFilePath ?? DBNull.Value;

        connection.Open();

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("dbo.Books_Delete", connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add("@Id", SqlDbType.Int)
            .Value = id;

        connection.Open();

        command.ExecuteNonQuery();
    }

    public List<Book> Search(string searchText)
    {
        var books = new List<Book>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand("dbo.Books_Search", connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add("@SearchText", SqlDbType.NVarChar, 500)
            .Value = searchText;

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            books.Add(new Book
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Author = reader.GetString(reader.GetOrdinal("Author")),
                PublicationYear = reader.GetInt32(reader.GetOrdinal("PublicationYear")),

                Description = reader["Description"] == DBNull.Value
                    ? null
                    : reader["Description"].ToString(),

                ContentsXml = reader["ContentsXml"] == DBNull.Value
                    ? string.Empty
                    : reader["ContentsXml"].ToString() ?? string.Empty,

                ContentsFilePath = reader["ContentsFilePath"] == DBNull.Value
                    ? null
                    : reader["ContentsFilePath"].ToString()
            });
        }

        return books;
    }
}