namespace NauraaBot.Core.Exception;

public class InvalidQueryFormatException : System.Exception
{
    public string Query { get; set; }
    public InvalidQueryFormatException(string query)
    {
        this.Query = query;
    }
    
    public InvalidQueryFormatException(string query, string message) : base(message)
    {
        this.Query = query;
    }
    
    public InvalidQueryFormatException(string query, string message, System.Exception inner) : base(message, inner)
    {
        this.Query = query;
    }
}