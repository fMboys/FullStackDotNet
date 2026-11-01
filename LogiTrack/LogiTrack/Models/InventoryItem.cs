public class InventoryItem
{
    public int ItemId { get; set; }
   public string Name { get; set; }
    public int Quantity { get; set; }
    public string Location { get; set; }

    // Default Constructor
    public InventoryItem()
    {
    }

    // Parameterized Constructor
    public InventoryItem(int itemId, string name, int quantity, string location)
    {
        ItemId = itemId;
        Name = name;
        Quantity = quantity;
        Location = location;
    }
    public void DisplayInfo()
    {
        Console.WriteLine($"Item: {Name} | Quantity: {Quantity} | Location: {Location}");
    }
}