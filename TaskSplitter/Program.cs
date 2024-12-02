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
        throw new NotImplementedException();
    }

    private static void SaveData(Data data)
    {
        throw new NotImplementedException();
    }

    private static void RandomizeTasks(Data data)
    {
        throw new NotImplementedException();
    }

    private static void LoadPeople(Data data)
    {
        throw new NotImplementedException();
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