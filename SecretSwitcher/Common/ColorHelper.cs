namespace SecretSwitcher.Common;

public static class ColorHelper
{
    public static void SetConsoleColor(ConsoleColor color, Action action)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        action();
        Console.ForegroundColor = originalColor;
    }

    public static void PrintWarning(string message) =>
        SetConsoleColor(ConsoleColor.Yellow, () => Console.WriteLine(message));

    public static void PrintInfo(string message) =>
        SetConsoleColor(ConsoleColor.Cyan, () => Console.WriteLine(message));

    public static void PrintSuccess(string message) =>
        SetConsoleColor(ConsoleColor.Green, () => Console.WriteLine(message));
}