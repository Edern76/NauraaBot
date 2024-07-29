using NauraaBot.Discord.Types.Search;

namespace NauraaBot.Core.Exception;

public class InvalidQueryFormatException : System.Exception
{
    public string Query { get; set; }
    public SearchIntent? Intent { get; set; }

    public InvalidQueryFormatException(string query)
    {
        this.Query = query;
    }

    public InvalidQueryFormatException(string query, SearchIntent? intent)
    {
        this.Query = query;
        this.Intent = intent;
    }

    public InvalidQueryFormatException(string query, string message) : base(message)
    {
        this.Query = query;
    }

    public InvalidQueryFormatException(string query, string message, SearchIntent? intent) : base(message)
    {
        this.Query = query;
        this.Intent = intent;
    }

    public InvalidQueryFormatException(string query, string message, System.Exception inner) : base(message, inner)
    {
        this.Query = query;
    }

    public InvalidQueryFormatException(string query, string message, System.Exception inner, SearchIntent? intent) :
        base(message, inner)
    {
        this.Query = query;
        this.Intent = intent;
    }
}