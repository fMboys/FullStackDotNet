public class Order
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public DateTime DatePlaced { get; set; }

    // List of items in the order
    public List<InventoryItem> Items { get; set; }

    public Order(int orderId, string customerName, DateTime datePlaced)
    {
        OrderId = orderId;
        CustomerName = customerName;
        DatePlaced = datePlaced;
        Items = new List<InventoryItem>();
    }

    // Add an item to the order
    public void AddItem(InventoryItem item)
    {
        Items.Add(item);
    }

    // Remove an item by ItemId
    public void RemoveItem(int itemId)
    {
        InventoryItem item = Items.FirstOrDefault(i => i.ItemId == itemId);

        if (item != null)
        {
            Items.Remove(item);
            Console.WriteLine($"Item {itemId} removed.");
        }
        else
        {
            Console.WriteLine($"Item {itemId} not found.");
        }
    }

    // Return order summary
    public string GetOrderSummary()
    {
        return $"Order #{OrderId} for {CustomerName} | Items: {Items.Count} | Placed: {DatePlaced:d}";
    }
}