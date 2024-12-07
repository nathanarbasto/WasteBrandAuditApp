using System;

namespace WasteBrandAuditApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                WasteAudit audit = new WasteAudit();
                audit.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
