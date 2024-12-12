using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskSplitter;

public class DataManagerConverter : JsonConverter<DataManager>
{
    public override DataManager? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonDocument? document = JsonSerializer.Deserialize<JsonDocument>(ref reader, options);
        if (document is null)
        {
            return null;
        }

        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException("Element is not object");
        }

        string convertedPeoplePropName = options.PropertyNamingPolicy?.ConvertName("People") ?? "People";
        string convertedTasksPropName = options.PropertyNamingPolicy?.ConvertName("Tasks") ?? "Tasks";
        string convertedAssignmentsPropName = options.PropertyNamingPolicy?.ConvertName("Assignments") ?? "Assignments";

        List<string>? people;
        if (document.RootElement.TryGetProperty(convertedPeoplePropName, out JsonElement peopleElement))
        {
            people = peopleElement.Deserialize<List<string>>(options);
        }
        else
        {
            people = null;
        }

        List<string>? tasks;
        if (document.RootElement.TryGetProperty(convertedTasksPropName, out JsonElement taskElement))
        {
            tasks = taskElement.Deserialize<List<string>>(options);
        }
        else
        {
            tasks = null;
        }

        List<Assignment>? assignments;
        if (document.RootElement.TryGetProperty(convertedAssignmentsPropName, out JsonElement assignmentsElement))
        {
            assignments = assignmentsElement.Deserialize<List<Assignment>>(options);
        }
        else
        {
            assignments = null;
        }

        return new DataManager(people, tasks, assignments);
    }

    public override void Write(Utf8JsonWriter writer, DataManager value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}