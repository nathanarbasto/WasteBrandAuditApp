using System;

public class Waste
{
    public string Brand { get; private set; }
    public int Quantity { get; private set; }
    public DateTime Date { get; private set; }

    public Waste(string brand, int quantity, DateTime? date = null)
    {
        Brand = brand;
        Quantity = quantity;
        Date = date?.Date ?? DateTime.Today;
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Brand: {Brand}, Quantity: {Quantity}g, Date: {Date.ToShortDateString()}");
    }
}
