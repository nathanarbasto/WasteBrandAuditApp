using System;

public class NonBiodegradableWaste : Waste
{
    public NonBiodegradableWaste(string brand, int quantity, DateTime? date = null)
        : base(brand, quantity, date) { }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Non-Biodegradable] Brand: {Brand}, Quantity: {Quantity}g, Date: {Date.ToShortDateString()}");
    }
}
