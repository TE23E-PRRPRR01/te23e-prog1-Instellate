using System;

namespace HelloWorld;

public static class Program
{
    public static void Main()
    {
        Console.Write("Hej, vad heter du? ");

        string? name = Console.ReadLine();
        Console.Write(string.IsNullOrWhiteSpace(name)
            ? "Har du inget namn? Men det är okej! Hur mår du? "
            : $"Ah {name}, vilket trevligt namn! Hur mår du? ");

        string? mood = Console.ReadLine()?.ToLower();
        switch (mood)
        {
            case "bra":
                Console.WriteLine("Det är trevligt att höra!");
                break;
            case "dåligt":
                Console.WriteLine("Aj, krya på dig!");
                break;
            default:
                Console.WriteLine("Det är ett humör jag har aldrigt hört om förut, intressant!");
                break;
        }
    }
}