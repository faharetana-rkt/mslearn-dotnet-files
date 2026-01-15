using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory,"stores");
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir); // Create the directory

var salesFiles = FindFiles(storesDirectory); // Get the JSON files

var salesTotal = CalculateSalestotal(salesFiles); // Calculate the total

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

var summary = new StringBuilder();
summary.AppendLine("Sales Summary");
summary.AppendLine("-----------------------");
summary.AppendLine($"Total sales: ${salesTotal:F2}");
summary.AppendLine(" ");
summary.AppendLine("Details:");
foreach (var file in salesFiles)
{
    var parts = file.Split('\\');
    string salesJson = File.ReadAllText(file);
    SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
    summary.AppendLine($"  {parts[parts.Length - 1]}: ${data?.Total}");
}

string sum = summary.ToString();

File.AppendAllText(Path.Combine(salesTotalDir, "summary.txt"), sum);

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();
    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);
    foreach (var file in foundFiles)
    {
        //var extension = Path.GetExtension(file);
        //if (extension == ".json")
        if (file.EndsWith("sales.json"))
        {
            salesFiles.Add(file);
        }
    }
    return salesFiles;
}

double CalculateSalestotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;
    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);
        // Parse the contents as JSON
        // the ? means the value can be null and if it is indeed null the compiler doesn't throw an error
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        // Add the amount found in the Total field to the salesTotal variable
        // ?. is a null conditional operator & ?? is a null coalescing operator
        // basically means if data isn't null, add total or if it is indeed null add 0 to the total
        salesTotal += data?.Total ?? 0;
    }
    return salesTotal;
}

record SalesData (double Total);


