using System;
using System.Collections.Generic;
using System.IO;


public class FileManager
{
    private readonly string filePath;

    public FileManager(string tempDirectory, string fileName)
    {
        this.filePath = Path.Combine(tempDirectory, fileName);

        // Ensure the directory exists
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
        }
    }

    public void SaveData(List<Waste> wasteList)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (var waste in wasteList)
                {
                    string type = waste is BiodegradableWaste ? "Biodegradable" : "Non-Biodegradable";
                    sw.WriteLine($"{waste.Brand},{waste.Quantity},{type},{waste.Date}");
                }
            }
            Console.WriteLine("Data saved successfully to temporary storage.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error saving data: {ex.Message}");
        }
    }

    public List<Waste> LoadData()
    {
        List<Waste> wasteList = new List<Waste>();
        try
        {
            if (File.Exists(filePath))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var data = line.Split(',');
                        if (data.Length == 4 && int.TryParse(data[1], out int quantity) && DateTime.TryParse(data[3], out DateTime date))
                        {
                            Waste waste;
                            if (data[2] == "Biodegradable")
                            {
                                waste = new BiodegradableWaste(data[0], quantity, date);
                            }
                            else
                            {
                                waste = new NonBiodegradableWaste(data[0], quantity, date);
                            }

                            wasteList.Add(waste);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
        return wasteList;
    }

}
