using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Spectre.Console;


public class WasteAudit
{
    private List<Waste> wasteList; //list of waste entries before being placed on temp file .txt or used to load files from temp files/auditreport.x;s to terminal
    private FileManager fileManager; //filemanager method from FileManager class to save temp files into .xls
    private readonly string reportsDirectory; //reads report from AuditReport.xls via ClosedXML.Excel
    public class Contributor
    {
        public string Brand { get; set; }
        public int TotalQuantity { get; set; }
    }

    public WasteAudit()
    {
        string tempDirectory = "C:\\Users\\natha\\source\\repos\\WasteBrandAuditApp\\WasteBrandAuditApp\\TempFiles";
        string reportsDir = "C:\\Users\\natha\\source\\repos\\WasteBrandAuditApp\\WasteBrandAuditApp\\Reports";
        fileManager = new FileManager(tempDirectory, "WasteAudit.txt");
        wasteList = fileManager.LoadData();

        // Set up the reports directory
        reportsDirectory = reportsDir;
        if (!Directory.Exists(reportsDirectory))
        {
            Directory.CreateDirectory(reportsDirectory);
        }
    }

    //application dashboard 
    public void Run()
    {
        while (true)
        {
            Console.WriteLine("Welcome to Waste Brand Audit Application!");
            Console.WriteLine("\n1. Log Waste");
            Console.WriteLine("2. View Audit Reports");
            Console.WriteLine("3. Delete Waste Data");
            Console.WriteLine("4. Save Data to Temporary Storage");
            Console.WriteLine("5. Export to Excel");
            Console.WriteLine("6. View Analytics");
            Console.WriteLine("7. Exit");
            Console.Write("Select an option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    LogWaste();
                    break;
                case "2":
                    ViewAuditReports();
                    break;
                case "3":
                    DeleteWasteData();
                    break;
                case "4":
                    SaveDataToTemporaryStorage();
                    break;
                case "5":
                    ExportToExcel();
                    break;
                case "6":
                    ViewAnalytics();
                    break;
                case "7":
                    Console.WriteLine("Thank you for using the Waste Brand Audit Application\nClosing now...");
                    return;
                default:
                    Console.WriteLine("Invalid option, try again.");
                    break;
            }
            Console.ReadKey();
            Console.Clear();
        }
    }

    //logging of waste (bio/non-bio) to List<Waste>
    private void LogWaste()
    {
        Console.Clear();
        Console.WriteLine("LOG WASTE\n\n");

        string brand;
        do
        {
            Console.Write("Enter brand: ");
            brand = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(brand))
            {
                Console.WriteLine("Invalid input. Brand cannot be empty.");
            }
        } while (string.IsNullOrWhiteSpace(brand));

        int quantity;
        do
        {
            Console.Write("Enter quantity (in grams): ");
            string quantityInput = Console.ReadLine();
            if (!int.TryParse(quantityInput, out quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid quantity. Please enter a positive number.");
            }
        } while (quantity <= 0);

        string wasteType;
        Waste waste = null;
        do
        {
            Console.WriteLine("Select waste type:");
            Console.WriteLine("1. Biodegradable");
            Console.WriteLine("2. Non-Biodegradable");
            wasteType = Console.ReadLine();

            if (wasteType == "1")
            {
                waste = new BiodegradableWaste(brand, quantity);
            }
            else if (wasteType == "2")
            {
                waste = new NonBiodegradableWaste(brand, quantity);
            }
            else
            {
                Console.WriteLine("Invalid selection. Please enter 1 or 2.");
            }
        } while (waste == null);

        wasteList.Add(waste);
        Console.WriteLine("Waste logged successfully.");
    }


    //views audit report from List<Waste> (daily, weekly, monthly)
    //note: only works with either the current day, current week, or current month
    private void ViewAuditReports()
    {
        Console.Clear();
        Console.WriteLine("VIEW AUDIT REPORTS\n\n");
        Console.WriteLine("\nEnter the choice of report you want to view: ");
        Console.WriteLine("1. Daily Report\n2. Weekly Report\n3. Monthly Report");
        string option = Console.ReadLine();

        DateTime startDate;
        switch (option)
        {
            case "1":
                startDate = DateTime.Today;
                GenerateReport(startDate, DateTime.Today);
                break;
            case "2":
                startDate = DateTime.Today.AddDays(-7);
                GenerateReport(startDate, DateTime.Today);
                break;
            case "3":
                startDate = DateTime.Today.AddMonths(-1);
                GenerateReport(startDate, DateTime.Today);
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }
    //generate report from List<Waste> to AuditReport.xls
    private void GenerateReport(DateTime startDate, DateTime endDate)
    {
        var filteredWaste = wasteList.Where(w => w.Date >= startDate && w.Date <= endDate).ToList();

        if (filteredWaste.Count == 0)
        {
            Console.WriteLine("No waste data available for the selected period.");
        }
        else
        {
            Console.WriteLine($"\nWaste Audit Report ({startDate.ToShortDateString()} - {endDate.ToShortDateString()}):");

            // Compute total weights for biodegradable and non-biodegradable waste
            int totalBiodegradableWeight = filteredWaste
                .Where(w => w is BiodegradableWaste)
                .Sum(w => w.Quantity);

            int totalNonBiodegradableWeight = filteredWaste
                .Where(w => w is NonBiodegradableWaste)
                .Sum(w => w.Quantity);

            // Print report details
            foreach (var waste in filteredWaste)
            {
                waste.DisplayInfo();
            }

            Console.WriteLine("\nSummary:");
            Console.WriteLine($"Total Biodegradable Waste: {totalBiodegradableWeight}g");
            Console.WriteLine($"Total Non-Biodegradable Waste: {totalNonBiodegradableWeight}g");
        }
    }

    private void DeleteWasteData()
    {
        Console.Clear();
        Console.WriteLine("DELETE WASTE ENTRY\n\n");
        ViewWasteData();
        Console.Write("Enter the number of the waste entry to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index <= 0 || index > wasteList.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        wasteList.RemoveAt(index - 1);
        Console.WriteLine("Waste entry deleted.");
    }

    private void SaveDataToTemporaryStorage()
    {
        Console.Clear();
        fileManager.SaveData(wasteList);
        Console.WriteLine("\nPress any key to continue...");
    }

    private void ExportToExcel()
    {
        Console.Clear();
        Console.WriteLine("EXPORT TO EXCEL\n");
        Console.Clear();
        try
        {
            string reportFilePath = Path.Combine(reportsDirectory, "WasteAuditReport.xlsx");
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("WasteAudit");

                // Adding headers
                worksheet.Cell(1, 1).Value = "Brand";
                worksheet.Cell(1, 2).Value = "Quantity (g)";
                worksheet.Cell(1, 3).Value = "Type";
                worksheet.Cell(1, 4).Value = "Date";

                // Adding data rows
                for (int i = 0; i < wasteList.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = wasteList[i].Brand;
                    worksheet.Cell(i + 2, 2).Value = wasteList[i].Quantity;
                    worksheet.Cell(i + 2, 3).Value = wasteList[i] is BiodegradableWaste ? "Biodegradable" : "Non-Biodegradable";
                    worksheet.Cell(i + 2, 4).Value = wasteList[i].Date.ToShortDateString();
                }

                // Adding total weights
                int totalBiodegradableWeight = wasteList
                    .Where(w => w is BiodegradableWaste)
                    .Sum(w => w.Quantity);
                int totalNonBiodegradableWeight = wasteList
                    .Where(w => w is NonBiodegradableWaste)
                    .Sum(w => w.Quantity);

                worksheet.Cell(wasteList.Count + 3, 1).Value = "Total Biodegradable Waste";
                worksheet.Cell(wasteList.Count + 3, 2).Value = totalBiodegradableWeight + "g";
    
                worksheet.Cell(wasteList.Count + 4, 1).Value = "Total Non-Biodegradable Waste";
                worksheet.Cell(wasteList.Count + 4, 2).Value = totalNonBiodegradableWeight + "g";

                workbook.SaveAs(reportFilePath);
            }
            Console.WriteLine($"Data exported successfully to {reportFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting to Excel: {ex.Message}");
        }
        Console.WriteLine("\nPress any key to continue...");
    }
    private void CheckTotalWaste()
    {
        int totalBiodegradableWeight = wasteList
            .Where(w => w is BiodegradableWaste)
            .Sum(w => w.Quantity);

        int totalNonBiodegradableWeight = wasteList
            .Where(w => w is NonBiodegradableWaste)
            .Sum(w => w.Quantity);

        Console.WriteLine("\nTotal Waste Weights:");
        Console.WriteLine($"Total Biodegradable Waste: {totalBiodegradableWeight}g");
        Console.WriteLine($"Total Non-Biodegradable Waste: {totalNonBiodegradableWeight}g");

        var largestContributor = wasteList
            .Where(w => w is NonBiodegradableWaste)
            .GroupBy(w => w.Brand)
            .Select(g => new { Brand = g.Key, TotalQuantity = g.Sum(w => w.Quantity) })
            .OrderByDescending(g => g.TotalQuantity)
            .FirstOrDefault();

        if (largestContributor != null)
        {
            Console.WriteLine($"\n");
        }
        else
        {
            Console.WriteLine("\nNo non-biodegradable waste data available.");
        }
    }

    public void ViewAnalytics()
    {
        Console.Clear();
        Console.WriteLine("VIEW ANALYTICS\n\n");
        var topContributors = wasteList
            .Where(w => w is NonBiodegradableWaste)
            .GroupBy(w => w.Brand)
            .Select(g => new Contributor { Brand = g.Key, TotalQuantity = g.Sum(w => w.Quantity) })
            .OrderByDescending(g => g.TotalQuantity)
            .Take(5)
            .ToList();

        if (topContributors.Any())
        {

            CheckTotalWaste();
            Console.WriteLine("\nTop Non-Biodegradable Waste Contributors:");
            // Display the bar chart using Spectre.Console
            DisplayBarChart(topContributors);
        }
        else
        {
            Console.WriteLine("\nNo non-biodegradable waste data available.");
        }
    }

    private void DisplayBarChart(List<Contributor> topContributors)
    {
        var chart = new BarChart()
            .Width(60) 
            .Label("[green]Top Contributors[/]")
            .CenterLabel();

        foreach (var contributor in topContributors)
        {
            chart.AddItem($"[cyan]{contributor.Brand}[/]", contributor.TotalQuantity);
        }

        AnsiConsole.Write(chart);
    }



    private void ViewWasteData()
    {
        if (wasteList.Count == 0)
        {
            Console.WriteLine("No waste data logged.");
        }
        else
        {
            for (int i = 0; i < wasteList.Count; i++)
            {
                Console.Write($"{i + 1}. ");
                wasteList[i].DisplayInfo();
            }
        }

    }
}
