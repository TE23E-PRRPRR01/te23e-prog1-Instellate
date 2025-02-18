using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskSplitter;

[JsonConverter(typeof(DataManagerConverter))]
public class DataManager
{
    private readonly List<string> _people = [];
    private readonly List<string> _tasks = [];
    private readonly List<Assignment> _assignments = [];

    public IReadOnlyList<string> People => _people;
    public IReadOnlyList<string> Tasks => _tasks;
    public IReadOnlyList<Assignment> Assignments => _assignments;

    public DataManager()
    {
    }

    public DataManager(List<string>? people, List<string>? tasks, List<Assignment>? assignments)
    {
        _people = people ?? [];
        _tasks = tasks ?? [];
        _assignments = assignments ?? [];
    }

    /// <summary>
    /// Lägger till en ny person
    /// </summary>
    /// <param name="name">Personens namn</param>
    public void AddPerson(string name) => _people.Add(name);

    /// <summary>
    /// Lägger till en ny uppgift
    /// </summary>
    /// <param name="task"></param>
    public void AddTasks(string task) => _tasks.Add(task);

    /// <summary>
    /// Lägger till en ny tilldelad uppgift
    /// </summary>
    /// <param name="assignment">Tilldelning som ska användas</param>
    /// <exception cref="InvalidOperationException">Om personen eller uppgiften så slängs detta</exception>
    public void AddAssignment(Assignment assignment)
    {
        // Titta om personen existerar
        bool containsPerson = false;
        foreach (var s in this.People)
        {
            if (string.Equals(s, assignment.Person))
            {
                containsPerson = true;
                break;
            }
        }

        // Om personen inte existerar kasta ett exception
        if (!containsPerson)
        {
            throw new InvalidOperationException("Person does not exist");
        }

        // Titta om uppgiften existerar
        bool containsTasks = false;
        foreach (var task in this.Tasks)
        {
            if (string.Equals(task, assignment.Task))
            {
                containsTasks = true;
                break;
            }
        }

        // Om uppgiften inte existerar kasta ett exception
        if (!containsTasks)
        {
            throw new InvalidOperationException("Task does not exist");
        }

        // Lägg till den nya tilldelade uppgiften
        this._assignments.Add(assignment);
    }

    /// <summary>
    /// Overload för att bara lägga till genom att ge bara namn och uppgift
    /// </summary>
    /// <param name="person">Personens namn</param>
    /// <param name="task">Uppgiftens namn</param>
    public void AddAssignment(string person, string task) => AddAssignment(new Assignment(person, task));

    /// <summary>
    /// Tar bort en person
    /// </summary>
    /// <param name="name">Namn på personen</param>
    /// <exception cref="InvalidOperationException">Om personen har en tilldelning så slängs detta</exception>
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

    /// <summary>
    /// Tar bort en uppgift
    /// </summary>
    /// <param name="name">Uppgiftens namn</param>
    /// <exception cref="InvalidOperationException">Om uppgiften har en tilldelning så slängs detta</exception>
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

    /// <summary>
    /// Tar bort en tilldelad uppgift
    /// </summary>
    /// <param name="assignment">Tilldelningens objekt</param>
    public void RemoveAssignment(Assignment assignment) => _assignments.Remove(assignment);

    /// <summary>
    /// Tar bort en tilldelade uppgift(er) genom personens namn
    /// </summary>
    /// <param name="name">Personens namn</param>
    public void RemoveAssignmentsByPerson(string name) => _assignments.RemoveAll(a => a.Person == name);

    /// <summary>
    /// Tar bort en tilldelade uppgift(er) genom uppgiftens namn
    /// </summary>
    /// <param name="name"></param>
    public void RemoveAssignmentsByTask(string name) => _assignments.RemoveAll(a => a.Task == name);

    /// <summary>
    /// Redigera en persons namn
    /// </summary>
    /// <param name="oldName">Gamla namn</param>
    /// <param name="newName">Nya namn</param>
    public void EditPerson(string oldName, string newName) => _people[_people.IndexOf(oldName)] = newName;

    /// <summary>
    /// Redigera en uppgfts namn
    /// </summary>
    /// <param name="oldName">Gamla namn</param>
    /// <param name="newName">Nya namn</param>
    public void EditTask(string oldName, string newName) => _tasks[_tasks.IndexOf(oldName)] = newName;

    /// <summary>
    /// Tilldela uppgfter till personer
    /// </summary>
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

    /// <summary>
    /// Spara all data till `./tasks.json`
    /// </summary>
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