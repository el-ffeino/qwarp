using System.Text.RegularExpressions;
namespace Qwarp.Utility;

public static class Terminal
{
    private static readonly string Accent = "#62a0ea";
    private static readonly string Red = "#ed333b";
    private static readonly string Green = "#57e389";
    private static readonly string Yellow = "#f8e45c";
    private static readonly string Orange = "#ffbe6f";

    public static void Out(string str) => Console.WriteLine(Format(str));
    public static void Write(string str) => Console.Write(Format(str));
    public static void Error(string str) => Out("[r][b]Error:[/r][r] " + str);
    public static void Warning(string str) => Out("[y][b]Warning:[/r][y] " + str);

    private static readonly Dictionary<string, string> AnsiCodes = new()
    {
        { "[b]", "\u001b[1m" },         // Bold
        { "[f]", "\u001b[2m" },         // Faint
        { "[i]", "\u001b[3m" },         // Italic
        { "[u]", "\u001b[4m" },         // Underline
        { "[v]", "\u001b[7m" },         // Inverse
        { "[/r]", "\u001b[0m" },        // Reset
        { "[/w]", "\u001b[22m" },       // ResetWeight
        { "[/u]", "\u001b[24m" },       // ResetUnderline
        { "[accent]", $"[{Accent}]"},
        { "[r]", $"[{Red}]" },
        { "[g]", $"[{Green}]" },
        { "[y]", $"[{Yellow}]" },
        { "[o]", $"[{Orange}]"}
    };

    public static string Format(string str)
    {
        string Formatted = str;

        if (!Formatted.EndsWith("[/r]"))
        {
            Formatted += "[/r]";
        }

        foreach (var kvp in AnsiCodes) 
        {
            Formatted = Formatted.Replace(kvp.Key, kvp.Value);
        }

        var hexPattern = @"\[#([0-9a-fA-F]{6})\]";
        Formatted = Regex.Replace(Formatted, hexPattern, match =>
        {
            string hex = "#" + match.Groups[1].Value;
            var (r, g, b) = HexToRgb(hex);
            return $"\u001b[38;2;{r};{g};{b}m";
        });

        return Formatted;
    }

    public static (int R, int G, int B) HexToRgb(string hex)
    {
        hex = hex.TrimStart('#');
        int r = Convert.ToInt32(hex.Substring(0, 2), 16);
        int g = Convert.ToInt32(hex.Substring(2, 2), 16);
        int b = Convert.ToInt32(hex.Substring(4, 2), 16);
        return (r, g, b);
    }

    public static string Read(string str = "")
    {
        if (string.IsNullOrEmpty(str))
        {
            Terminal.Write("[accent]> ");
        }
        else
        {
            Terminal.Write(str);
        }
        
        string? Input = Console.ReadLine();
        if (Input == null) Input = string.Empty;

        return Input;
    }
}