// See https://aka.ms/new-console-template for more information

using System.Text;
using System.Text.RegularExpressions;

Console.WriteLine("Enrollment Parser Availity 101!");

var enrollmentSolutions = new BenefitsManagement();

var getResult = enrollmentSolutions.SaveDocumentCSVPathLocal(enrollmentSolutions.EnrollmentParser("Enrollments").Result, "Enrollments");

Console.WriteLine(getResult);

Console.ReadLine();

public class BenefitsManagement
{
    private readonly Regex sWP = new Regex(@"\s+");
    /// <summary>
    /// Responsible for writing to local path csv files separating enrollees by insurance company in its own file...TCDW
    /// </summary>
    /// <param name="write"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task SaveDocumentCSVPathLocal(Dictionary<string, Insured> write, string fileName)
    {
        var writeFileCSV = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        var savePath = writeFileCSV + @"/CSVProviderData/";

        if (!Directory.Exists(savePath))
        {
            _ = Directory.CreateDirectory(savePath);
        }

        //This writes the entire file showing the order names were put in but don't write them anywhere but in memory...TCDW
        var writeTocsv = String.Join(
            Environment.NewLine,
            write.Select(d => $"{d.Key};{d.Value};")
            );

        var getCSV = Directory.GetFiles(@"./Repo").Where(f => f.Contains($"{fileName}")).FirstOrDefault();

        var fileIn = await File.ReadAllLinesAsync(getCSV);

        using (var fileWriter = new StringWriter())
        {
            if (write != null)
            {
                if (write.Keys.Count > 0)
                {
                    write.Keys.ToList().ForEach(async k =>
                    {
                        var itemToArray = new List<string>();

                        write.Values.Where(p => p.InsuranceProvider == write[k].InsuranceProvider).ToList().ForEach(val =>
                        {
                            Console.WriteLine("User ID: " + val.UserId + " Insurance Company: " + val.InsuranceProvider + " First Name: " + val.FirstName + " Last Name: " + val.LastName);

                            itemToArray.Add($"{val.UserId},{val.FirstName},{val.LastName},{val.Version.ToString()},{val.InsuranceProvider}");
                        });

                        var replaceWrite = Path.Combine(savePath, $"{write[k].InsuranceProvider}, {DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}, {".csv"}");

                        await File.WriteAllLinesAsync(replaceWrite, fileIn.Take(1).Concat((IEnumerable<string>)itemToArray));
                    });
                }
            }
        }
    }

    public async Task<Dictionary<string, Insured>> EnrollmentParser(string fileName)
    {
        var getCSV = Directory.GetFiles(@"./Repo").Where(f => f.Contains($"{fileName}")).FirstOrDefault();

        var instance = new Dictionary<string, Insured>();

        if (File.Exists(getCSV))
        {
            var fileIn = await File.ReadAllLinesAsync(getCSV);

            var deserializeToObject = new List<Insured>();

            fileIn.Skip(1).ToList().ForEach(l =>
            {
                deserializeToObject.Add(
                        new Insured
                        {
                            UserId = l.Split(',')[0].ToString(),
                            FirstName = l.Split(',')[1].ToString(),
                            LastName = l.Split(',')[2].ToString(),
                            InsuranceProvider = l.Split(',')[4].ToString(),
                            Version = Convert.ToInt32(l.Split(',')[3])
                        });
            });
                        
            deserializeToObject.ForEach(r =>
            {
                var getProviderWithoutWhitespace = sWP.Replace(r.InsuranceProvider, string.Empty);
                var instanceKey = r.UserId + r.LastName + r.FirstName + getProviderWithoutWhitespace;

                if (!instance.ContainsKey(instanceKey))
                {
                    instance.Add(instanceKey,
                        new Insured
                        {
                            UserId = r.UserId,
                            FirstName = r.FirstName,
                            LastName = r.LastName,
                            InsuranceProvider = r.InsuranceProvider,
                            Version = r.Version
                        });
                }
                else
                {
                    if (instance[instanceKey].Version < r.Version && instance[instanceKey].InsuranceProvider.ToLower() == r.InsuranceProvider.ToLower())
                    {
                        instance[instanceKey] = new Insured
                        {
                            UserId = r.UserId,
                            FirstName = r.FirstName,
                            LastName = r.LastName,
                            InsuranceProvider = r.InsuranceProvider,
                            Version = r.Version
                        };
                    }
                    else
                    {
                        instance.Add(instanceKey,
                        new Insured
                        {
                            UserId = r.UserId,
                            FirstName = r.FirstName,
                            LastName = r.LastName,
                            InsuranceProvider = r.InsuranceProvider,
                            Version = r.Version
                        });
                    }
                }
            });

            instance.OrderBy(p => p.Value.LastName).OrderBy(p => p.Value.FirstName).Select(item => item.Value).ToList();
        }
        return instance;
    }
}


public class Insured
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string InsuranceProvider { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
}