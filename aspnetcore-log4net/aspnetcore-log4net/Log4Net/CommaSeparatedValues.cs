namespace aspnetcore_log4net.Log4Net
{
  public static class CommaSeparatedValues
  {
    // constants

    private const char comma = ',';
    private const char doubleQuote = '"';
    private const char newline = '\n';
    private const char carriageReturn = '\r';

    private static readonly char[] charactersRequiringEscaping =
      { comma, doubleQuote, newline, carriageReturn };

    private const string doubleQuoteString = "\"";
    private const string escapedDoubleQuoteString = "\"\"";

    // methods

    /// <summary>
    /// Escapes a string for use as a value in CSV. 
    /// Supports multiple lines and CSV special characters.
    /// </summary>
    /// <param name="value">String to be escaped.</param>
    /// <returns>The escaped string.</returns>
    public static string Escape( string value ) =>
      SurroundWithDoubleQuotesIfRequired(
        EscapeDoubleQuotes( value ) );

    /// <summary>
    /// Unescapes a CSV-escaped value.
    /// </summary>
    /// <param name="value">A CSV-escaped value.</param>
    /// <returns>The unescaped value.</returns>
    public static string Unescape( string value )
    {
      var (ValueHadSurroundingDoubleQuotes, UnquotedValue) =
        RemoveSurroundingDoubleQuotes( value );

      // if value had surrounding double quotes, then unescape the double quotes in the string with
      //  the surrounding double quotes removed.
      return ValueHadSurroundingDoubleQuotes
        ? UnescapeEscapedDoubleQuotes( UnquotedValue )
        : value;
    }

    // helper methods

    // adding and removing surrounding double quotes

    private static string SurroundWithDoubleQuotesIfRequired( string value ) =>
      value != null && value.IndexOfAny( charactersRequiringEscaping ) > -1
        ? doubleQuoteString + value + doubleQuoteString
        : value;

    private static (bool, string) RemoveSurroundingDoubleQuotes( string value ) =>
      value.StartsWith( doubleQuoteString ) && value.EndsWith( doubleQuoteString )
      ? (true, value.Substring( 1, value.Length - 2 ))
      : (false, value);

    // double quote escaping & unescaping

    private static string EscapeDoubleQuotes( string value ) =>
      value != null && value.Contains( doubleQuoteString )
        ? value.Replace( doubleQuoteString, escapedDoubleQuoteString )
        : value;

    private static string UnescapeEscapedDoubleQuotes( string value )
    {
      return value != null && value.Contains( escapedDoubleQuoteString )
        ? value.Replace( escapedDoubleQuoteString, doubleQuoteString )
        : value;
    }
  }

}
