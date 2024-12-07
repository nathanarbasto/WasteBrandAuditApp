using System;

public class BiodegradableWaste : Waste
{
    public BiodegradableWaste(string brand, int quantity, DateTime? date = null)
        : base(brand, quantity, date) { }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Biodegradable] Brand: {Brand}, Quantity: {Quantity}g, Date: {Date.ToShortDateString()}");
    }
}
