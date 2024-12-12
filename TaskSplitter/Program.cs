using System.Text.Json;

namespace TaskSplitter;

public static class Program
{
    public static int Main(string[] args)
    {
        Console.Clear();
        Console.WriteLine("Välkommen! Ska vi ordna vem som gör vilken uppgift nu då?");

        // Initiera all data som ska användas
        DataManager manager = new();

        while (true)
        {
            Task? task = null;
            do
            {
                Console.Write("""
                              Alternativ:
                              1. Lägg, visa eller redigera personer
                              2. Lägg, visa eller redigera uppgifter
                              3. Visa eller checka av tilldelade uppgifter
                              4. Ladda in all sparad data
                              5. Spara all data
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
                    AddOrEdit(manager, "Person");
                    break;
                // Användaren har valt handlingar angående uppgifter
                case Task.Tasks:
                    Console.Clear();
                    AddOrEdit(manager, "Uppgift");
                    break;
                // Användaren har valt handlingar angående tilldelade uppgifter
                case Task.Assignments:
                    Console.Clear();
                    ManageAssignments(manager);
                    break;
                // Användaren har valt att ladda in data
                case Task.LoadData:
                    Console.Clear();
                    DataManager? newManager = LoadData();
                    // Om nya managern är null så är datan invalid och då ska vi inte skriva över `manager`
                    if (newManager is not null)
                    {
                        manager = newManager;
                    }

                    break;
                // Användaren har valt att spara data
                case Task.SaveData:
                    Console.Clear();
                    SaveData(manager);
                    break;
                // Användaren har valt att slumpa ut uppgifterna
                case Task.Randomize:
                    Console.Clear();
                    RandomizeTasks(manager);
                    break;
                // Användaren har valt att avsluta programmet
                case Task.Quit:
                    Console.WriteLine("Hejdå!");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    return 0;
                // Användaren har valt något som inte existerar
                default:
                    Console.WriteLine($"Jag vet inte om {task + 1}");
                    break;
            }
        }
    }

    private static DataManager? LoadData()
    {
        // Tittar om filen för sparad data finns
        if (!File.Exists("./tasks.json"))
        {
            // Om den inte finns berätta det och gå tillbaka till menyn
            Console.WriteLine("Filen `./tasks.json` existerar inte");
            return null;
        }

        try
        {
            // Försök ladda in data
            DataManager? newManager = DataManager.LoadData();
            if (newManager is not null)
            {
                // Om datan inte är null ge tillbaka den
                return newManager;
            }
            else
            {
                // Om den är null säg att datan är ogiltig
                Console.WriteLine("Datan som blev inladdad är inte giltig data");
            }
        }
        catch (JsonException)
        {
            // Om vi får JsonException säg att datan är ogiltigt
            Console.WriteLine("Datan som blev inladdad är inte giltig data");
        }

        return null;
    }

    private static void SaveData(DataManager manager)
    {
        // Kalla `SaveDatà` för att spara data
        manager.SaveData();
        // Spara datta till `./tasks.json`
        Console.WriteLine("Sparade datan till `./tasks.json`");
    }

    private static void RandomizeTasks(DataManager manager)
    {
        // Denna funktion tittar om det finns några element i funktionen
        if (manager.Assignments.Any())
        {
            // Om det finns element, fråga om 
            Console.WriteLine("Det finns fortfarande tilldelade uppgifter, " +
                              "om du fortsätter så kommer alla förra tilldelade uppgifter att gå bort");
            Console.Write("Vill du fortsätta? [j/N]: ");
            string input = Console.ReadLine()?.ToLower() ?? "n";
            // Om input är inte `j` så ta det som ett nej eftersom nej är default
            // Om det är ett nej, lämna och gå tillbaka till menyn
            if (input != "j")
            {
                Console.Clear();
                Console.WriteLine("Går tillbaka till menyn");
                return;
            }
        }

        // Kalla `DataManager#Assign` för att tilldela uppgifter på `DataManager`
        manager.Assign();
        // Printa ut dem nya tilldelade uppgifterna
        foreach ((string person, string task) in manager.Assignments)
        {
            Console.WriteLine($"{person} ska {task}");
        }

        Console.Write("Tryck enter för att fortsätta: ");
        // Väntar på att användaren ska trycka enter för att fortsätta
        Console.ReadLine();
        Console.Clear();
    }

    private static void AddOrEdit(DataManager manager, string typeName)
    {
        // Data är för data, typeName är för att printa vilken typ av sak som används
        // Gör så har för att hindra dupliserad kod.
        Action<string> remove = typeName == "Person" ? manager.RemovePerson : manager.RemoveTask;
        Action<string> add = typeName == "Person" ? manager.AddPerson : manager.AddTasks;
        Action<string, string> edit = typeName == "Person" ? manager.EditPerson : manager.EditTask;
        IReadOnlyList<string> data = typeName == "Person" ? manager.People : manager.Tasks;

        // Ge en beskrivning över hur det fungerar att spara data
        Console.Clear();
        while (true)
        {
            // Printa ut vad olika saker funkar. Skriv `a` för att  lägga till en {typeName}, q för tillbaka.
            // Siffror för att redigera en användare 
            Console.WriteLine(
                $"Välj ett nummer för att redigera {typeName.ToLower()} (eller skriv inget om du vill ta bort).");
            Console.WriteLine($"Skriv in ett nytt namn du vill lägga till en {typeName.ToLower()}.");
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
                                edit(oldName, newName);
                                Console.Clear();
                                Console.WriteLine($"{oldName} heter nu {newName.Trim()}");
                            }
                            else
                            {
                                // Om stringen är tom, ta bort {typeName}
                                try
                                {
                                    remove(oldName);
                                    Console.Clear();
                                    Console.WriteLine($"Tog bort {oldName}");
                                }
                                catch (InvalidOperationException)
                                {
                                    Console.Clear();
                                    Console.WriteLine($"{oldName} är fortfarande tilldelad!");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            // Om namn är inte null, tom eller bara spaces. Trimma och lägg till det
                            add(input.Trim());
                            Console.Clear();
                            Console.WriteLine($"La till {input.Trim()}");
                        }
                    }

                    break;
                }
            }
        }
    }

    private static void ManageAssignments(DataManager manager)
    {
        while (true)
        {
            // Ge instruktioner över hur det använts
            Console.WriteLine("Skriv in numret för en tilldelad uppgift för att checka av den");
            Console.WriteLine("Skriv 'q' om du vill lämna");
            for (int i = 0; i < manager.Assignments.Count; i++)
            {
                // Printa ut alla uppgifter
                Assignment assignment = manager.Assignments[i];
                Console.WriteLine($"{i + 1}. {assignment.Person} ska {assignment.Task}");
            }

            // Fråga om input
            Console.Write("Input: ");
            string? input = Console.ReadLine();
            if (input is null)
            {
                // Om input är null säg det till användaren
                Console.WriteLine("Du måste ge input!");
                continue;
            }

            // Titta om input är ett nummer
            if (int.TryParse(input, out int num))
            {
                // Om numret är mindre eller lika med noll, säg det och gå tillbaka till menyn
                if (num <= 0)
                {
                    Console.Clear();
                    Console.WriteLine("Input kan inte vara mindre än noll");
                    continue;
                }

                // Om numret är mer än tilldelade uppgifter, säg det och gå tillbaka till menyn
                if (num > manager.Assignments.Count)
                {
                    Console.Clear();
                    Console.WriteLine("Numret är större än hur många tilldelade uppgifter som finns");
                    continue;
                }

                // Ta bort tilldelad uppgift
                manager.RemoveAssignment(manager.Assignments[num - 1]);
                Console.Clear();
                Console.WriteLine("Tog bort uppgiften");
            }
            else if (input == "q")
            {
                // Lämna
                Console.Clear();
                return;
            }
            else
            {
                // Input existerar inte
                Console.Clear();
                Console.WriteLine($"Jag vet inte om {input}");
            }
        }
    }
}