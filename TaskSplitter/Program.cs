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

    private static void AddOrEdit(List<string> dataPeople, string person)
    {
        throw new NotImplementedException();
    }
}