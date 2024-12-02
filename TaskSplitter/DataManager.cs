using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskSplitter;

public class DataManager
{
    [JsonIgnore]
    private readonly List<string> _people = [];
    [JsonIgnore]
    private readonly List<string> _tasks = [];
    [JsonIgnore]
    private readonly List<Assignment> _assignments = [];

    public IReadOnlyList<string> People => _people;
    public IReadOnlyList<string> Tasks => _tasks;
    public IReadOnlyList<Assignment> Assignments => _assignments;

    public DataManager()
    {
    }

    [JsonConstructor]
    public DataManager(List<string>? people, List<string>? tasks, List<Assignment>? assignments)
    {
        _people = people ?? [];
        _tasks = tasks ?? [];
        _assignments = assignments ?? [];
    }

    // Lägger till en ny person
    public void AddPerson(string name) => _people.Add(name);
    
    // Lägger till en ny uppgift
    public void AddTasks(string task) => _tasks.Add(task);

    // Tar bort en person
    public void RemovePerson(string name)
    {
        bool contains = false;
        // Tittar om personens namn är i tilldelade uppgifter
        foreach (var a in _assignments)
        {
            // Är personens namn desamma?
            if (a.Person == name)
            {
                // Ja det är dem, byt värdet på contains och avsluta loopen
                contains = true;
                break;
            }
        }

        // Om `contains` är true släng en error som säger att personen finns i tilldelade uppgifter
        if (contains)
        {
            throw new InvalidOperationException("Cannot remove a person that has an assignment");
        }

        // Ta bort personen
        _people.Remove(name);
    }
    
    // Tar bort en uppgift
    public void RemoveTask(string name)
    {
        // Tittar om uppgiftens namn är i tilldelade uppgifter
        bool contains = false;
        foreach (var a in _assignments)
        {
            // Är uppgiftens namn desamma?
            if (a.Task == name)
            {
                // Ja det är dem, byt värdet på contains och avsluta loopen
                contains = true;
                break;
            }
        }

        // Om `contains` är true släng en error som säger att personen finns i tilldelade uppgifter
        if (contains)
        {
            throw new InvalidOperationException("Cannot remove a task that has an assignment");
        }

        // Ta bort uppgiften
        _tasks.Remove(name);
    }

    // Tar bort en tilldelad uppgift
    public void RemoveAssignment(Assignment assignment) => _assignments.Remove(assignment);

    // Tar bort en tilldelade uppgift(er) genom personens namn
    public void RemoveAssignmentByPerson(string name) => _assignments.RemoveAll(a => a.Person == name);

    // Tar bort en tilldelade uppgift(er) genom uppgiftens namn
    public void RemoveAssignmentByTask(string name) => _assignments.RemoveAll(a => a.Task == name);

    // Tilldela uppgifter till personen
    public void Assign()
    {
        // Töm tilldelade uppgifter
        _assignments.Clear();
        _assignments.Capacity = _tasks.Capacity;

        // Gör en shallow copy av personer
        List<string> copyOfPeople = [.._people];

        // Gå igenom varje uppgift
        foreach (string task in _tasks)
        {
            // Om det inte finns några mer personen avsluta loopen
            if (copyOfPeople.Count <= 0)
            {
                break;
            }

            // Hitta en random index i personer
            int randomIndex = Random.Shared.Next(0, copyOfPeople.Count);
            // Lägg till det i tilldelade uppgifter
            _assignments.Add(new Assignment(copyOfPeople[randomIndex], task));
            // Ta bort den indexen
            copyOfPeople.RemoveAt(randomIndex);
        }
    }

    // Spara all data
    public void SaveData()
    {
        // Gör om datan till json
        string json = JsonSerializer.Serialize(this);
        // Spara det i filen `./tasks.json`
        File.WriteAllText("./tasks.json", json);
    }

    public static DataManager? LoadData()
    {
        // Öppna en fill stream till `./tasks.json`
        using Stream file = File.OpenRead("./tasks.json");
        // Läs json fill streamen och gör om det till `DataManager`
        return JsonSerializer.Deserialize<DataManager>(file);
    }
}