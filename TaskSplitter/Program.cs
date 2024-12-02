using System.Text.Json;

namespace TaskSplitter;

public class Program
{
    public static int Main(string[] args)
    {
        Console.Clear();
        Console.WriteLine("Välkommen! Ska vi ordna vem som gör vilken uppgift nu då?");

        // Initiera all data som ska användas
        Data data = new();

        while (true)
        {
            Task? task = null;
            do
            {
                Console.Write("""
                              Alternativ:
                              1. Lägg, visa eller redigera personer
                              2. Lägg, visa eller redigera uppgifter
                              3. Ladda in sparade uppgifter
                              4. Ladda in sparade personer
                              5. Spara data
                              6. Slumpa!
                              7. Lämna
                              Input: 
                              """);

                // Läss användarens input
                string? userInput = Console.ReadLine();
                // Gör om användarens string input till ett nummer
                if (int.TryParse(userInput, out int num))
                {
                    // Ta det minus 0 för att vi börjar på 0
                    task = (Task)num - 1;
                }
                else
                {
                    // Säg till användaren att välja ett alternativ som finns i menyn
                    Console.Clear();
                    Console.WriteLine("Välj ett alternativ som finns!");
                }
                // Fortsätt loopa tills användaren väljer en input som finns
            } while (task is null);
            
            // Switch case för olika typer av uppgifter
            switch (task.Value)
            {
                // Användaren har valt handlingar angående personer
                case Task.People:
                    Console.Clear();
                    AddOrEdit(data.People, "Person");
                    break;
                // Användaren har valt handlingar angående uppgifter
                case Task.Tasks:
                    Console.Clear();
                    AddOrEdit(data.Tasks, "Uppgift");
                    break;
                // Användaren har valt att ladda in sparade personer
                case Task.LoadPeople:
                    Console.Clear();
                    LoadPeople(data);
                    break;
                // Användaren har valt att ladda in sparade uppgifter
                case Task.LoadTasks:
                    Console.Clear();
                    LoadTasks(data);
                    break;
                // Användaren har valt att spara data
                case Task.SaveData:
                    Console.Clear();
                    SaveData(data);
                    break;
                // Användaren har valt att slumpa ut uppgifterna
                case Task.Randomize:
                    Console.Clear();
                    RandomizeTasks(data);
                    break;
                // Användaren har valt att avsluta programmet
                case Task.Quit:
                    Console.WriteLine("Hejdå!");
                    return 0;
                // Användaren har valt något som inte existerar
                default:
                    Console.WriteLine($"Jag vet inte om {task + 1}");
                    break;
            }
        }
    }

    private static void LoadTasks(Data data)
    {
        // Initierar variable där all text kommer vara
        string text;
        try
        {
            // Läs all text från fil `./tasks.json`
            text = File.ReadAllText("./tasks.json");
        }
        catch (FileNotFoundException)
        {
            // Printa att användaren har ingen sparad data om filen inte blir hittad och gå tillbaka
            Console.WriteLine("Du har ingen sparad data");
            return;
        }

        try
        {
            // Läs data från `text` variable
            Data? newData = JsonSerializer.Deserialize<Data>(text);
            if (newData is null)
            {
                // Om data är null så är content i filen dålig
                Console.WriteLine("Datan i filen är dålig! Spara om");
            }
            else
            {
                // Annars lada in enbart person data
                data.Tasks = newData.Tasks;
                Console.WriteLine("Laddade in uppgifter");
            }
        }
        catch (JsonException)
        {
            // Datan är inte JSON, eller JSON som inte kan läsas in till `Data` 
            Console.WriteLine("Datan i filen är dålig! Spara om");
        }
    }

    private static void SaveData(Data data)
    {
                // Loopa tills vi säger annars
        while (true)
        {
            // Visa en lista över alternativ till användaren
            Console.Write("""
                          Vad vill du spara?
                          1. Spara personer
                          2. Spara uppgifter
                          3. Spara båda
                          4. Gå tillbaka
                          Input:
                          """);

            // Ta input från användaren
            string? input = Console.ReadLine();

            // Titta om input är ett valid nummer
            if (!int.TryParse(input, out int number))
            {
                // Om det är inte ett nummer skriv det och loopa tillbaka till menyn
                Console.Clear();
                Console.WriteLine("Du måste ge mig ett nummer");
                continue;
            }

            // Variabel som används för att hålla json data
            string json;
            switch (number)
            {
                // Spara alla personer
                case 1:
                    // Gör om data om personer från ett objekt till json
                    json = JsonSerializer.Serialize(new Data()
                    {
                        Tasks = data.People
                    });
                    // Skriv det till tasks.json
                    File.WriteAllText("./tasks.json", json);
                    Console.Clear();
                    Console.WriteLine("Sparade personer");
                    break;
                case 2:
                    // Gör om data om uppgifter från ett objekt till json
                    json = JsonSerializer.Serialize(new Data()
                    {
                        Tasks = data.Tasks
                    });
                    // Skriv det till tasks.json
                    File.WriteAllText("./tasks.json", json);
                    Console.Clear();
                    Console.WriteLine("Sparade uppgifter");
                    break;
                case 3:
                    // Gör om både personer och uppgifter från ett objekt till json
                    json = JsonSerializer.Serialize(data);
                    File.WriteAllText("./tasks.json", json);
                    Console.Clear();
                    Console.WriteLine("Sparade allting");
                    break;
                case 4:
                    // Lämna loopen
                    Console.Clear();
                    return;
                default:
                    // Användaren gav ett nummer som inte är i listan. Loopa om
                    Console.Clear();
                    Console.WriteLine("Känner inte igen det numret. Ta en från listan");
                    break;
            }
        }
    }

    private static void RandomizeTasks(Data data)
    {
        // Shallow kopia listan av strings så att det kan återanvändas senare
        List<string> copyOfPeople = [..data.People];

        // Itererar genom alla uppgifter
        foreach (string dataTask in data.Tasks)
        {
            // Om kopian är tom sluta loopa
            if (copyOfPeople.Count <= 0)
            {
                break;
            }

            // Skaffa en slumpmäsig position i listan
            int randomIndex = Random.Shared.Next(0, copyOfPeople.Count);
            // Printa ut vad den slumpmäsiga personen ska göra
            Console.WriteLine($"{copyOfPeople[randomIndex]} ska {dataTask}");
            // Ta bort personen från listan
            copyOfPeople.RemoveAt(randomIndex);
        }

        Console.Write("Tryck enter för att fortsätta: ");
        // Väntar på att användaren ska trycka enter för att fortsätta
        Console.ReadLine();
        Console.Clear();
    }

    private static void LoadPeople(Data data)
    {
        // Initierar variable där all text kommer vara
        string text;
        try
        {
            // Läs all text från fil `./tasks.json`
            text = File.ReadAllText("./tasks.json");
        }
        catch (FileNotFoundException)
        {
            // Printa att användaren har ingen sparad data om filen inte blir hittad och gå tillbaka
            Console.WriteLine("Du har ingen sparad data");
            return;
        }

        try
        {
            // Läs data från `text` variable
            Data? newData = JsonSerializer.Deserialize<Data>(text);
            if (newData is null)
            {
                // Om data är null så är content i filen dålig
                Console.WriteLine("Datan i filen är dålig! Spara om");
            }
            else
            {
                // Annars lada in enbart person data
                data.People = newData.People;
                Console.WriteLine("Laddade in personer!");
            }
        }
        catch (JsonException)
        {
            // Datan är inte JSON, eller JSON som inte kan läsas in till `Data` 
            Console.WriteLine("Datan i filen är dålig! Spara om");
        }
    }

    private static void AddOrEdit(List<string> data, string typeName)
    {
        // Data är för data, typeName är för att printa vilken typ av sak som används
        // Gör så har för att hindra dupliserad kod.
        
        // Ge en beskrivning över hur det fungerar att spara data
        Console.Clear();
        Console.WriteLine(
            "Välj ett nummer för att redigera dem (eller ge inget på nya namnet om du vill ta bort).");
        while (true)
        {
            // Printa ut vad olika saker funkar. Skriv `a` för att  lägga till en {typeName}, q för tillbaka.
            // Siffror för att redigera en användare 
            Console.WriteLine($"Skriv a om du vill lägga till en {typeName.ToLower()}.");
            Console.WriteLine("Skriv q om du vill gå tillbaka.");
            for (int i = 0; i < data.Count; i++)
            {
                // Olika personer som finns och deras position
                Console.WriteLine($"{i + 1}. {data[i]}");
            }

            Console.Write("Input: ");

            // Läs input från användare
            string? input = Console.ReadLine();
            switch (input)
            {
                // Om det är q, lämna loopen.
                case "q":
                    Console.Clear();
                    return;
                case "a":
                    // Om det är a, fråga efter namn
                    Console.Write("Namnet: ");
                    string? name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        // Om namn är inte null, tom eller bara spaces. Trimma och lägg till det
                        data.Add(name.Trim());
                        Console.Clear();
                        Console.WriteLine($"La till {name.Trim()}");
                    }

                    break;
                default:
                {
                    // Om det är inget av ovanför. Försök se om det är ett nummer
                    if (int.TryParse(input, out int index))
                    {
                        // Ta bort `1` från index eftersom positioner börjar på 0 istället för 1
                        index--;
                        // Om det är mindre än 0 (positoner mindre än 0 finns inte) eller om det är större än vad vi har, lämna
                        if (index < 0 || index >= data.Count)
                        {
                            Console.Clear();
                            Console.WriteLine($"{typeName} finns inte");
                        }
                        else
                        {
                            // Kopiera ut det gamla namnet och fråga om nya namnet
                            string oldName = data[index];
                            Console.Write("Nytt namn: ");
                            string? newName = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(newName))
                            {
                                // Om stringen är inte tom, döpp om {typeName} till ett nytt namn
                                data[index] = newName;
                                Console.Clear();
                                Console.WriteLine($"{oldName} heter nu {newName.Trim()}");
                            }
                            else
                            {
                                // Om stringen är tom, ta bort {typeName}
                                data.RemoveAt(index);
                                Console.Clear();
                                Console.WriteLine($"Tog bort {oldName}");
                            }
                        }
                    }

                    break;
                }
            }
        }
    }
}