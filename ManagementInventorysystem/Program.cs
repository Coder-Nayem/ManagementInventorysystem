using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum ItemType
{
    Electronics,
    Clothing,
    Food,
    Furniture
}

public interface IEntity : IDisposable
{
    int Id { get; }
    bool IsValid();
}

public interface IInventoryRepository : IDisposable, IEnumerable<Item>
{
    void Add(Item item);
    bool Delete(Item item);
    void Update(Item item);
    Item FindById(int id);
    IEnumerable<Item> Search(string value);
}

public class Item : IEntity
{
    public int Id { get; }
    public string Name { get; set; }
    public ItemType Type { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }

    public Item()
    {

    }

    public Item(int itemId, string name, ItemType type, double price, int quantity)
    {
        this.Id = itemId;
        this.Name = name;
        this.Type = type;
        this.Price = price;
        this.Quantity = quantity;
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) && Price > 0 && Quantity >= 0;
    }

    public override string ToString()
    {
        string text = "Item Info\n";
        text += $"Item ID : {this.Id}\n";
        text += $"Name  : {this.Name}\n";
        text += $"Type  : {this.Type}\n";
        text += $"Price : {this.Price:C2}\n";
        text += $"Quantity : {this.Quantity}\n";
        text += "************************\n";

        return text;
    }

    public void Dispose()
    {

    }
}

public sealed class InventoryRepository : IInventoryRepository
{
    private static InventoryRepository _instance;
    private List<Item> data;

    public static InventoryRepository Instance
    {
        get
        {
            _instance = _instance ?? new InventoryRepository();
            return _instance;
        }
    }

    private InventoryRepository()
    {
        data = new List<Item>
        {
            new Item(itemId: 1, name: "Laptop", type: ItemType.Electronics, price: 999.99, quantity: 10),
            new Item(itemId: 2, name: "T-Shirt", type: ItemType.Clothing, price: 19.99, quantity: 50),
            new Item(itemId: 3, name: "Bread", type: ItemType.Food, price: 2.49, quantity: 100),
            new Item(itemId: 4, name: "Chair", type: ItemType.Furniture, price: 49.99, quantity: 20),
        };
    }

    public void Dispose()
    {
        data.Clear();
    }

    public void Add(Item item)
    {
        if (data.Any(i => i.Id == item.Id))
        {
            throw new Exception("Duplicate item ID, try another.");
        }
        else if (item.IsValid())
        {
            data.Add(item);
        }
        else
        {
            throw new Exception("Item is invalid.");
        }
    }

    public bool Delete(Item item)
    {
        return data.Remove(item);
    }

    public void Update(Item item)
    {
        Item existingItem = FindById(item.Id);
        if (existingItem != null)
        {
            existingItem.Name = item.Name;
            existingItem.Type = item.Type;
            existingItem.Price = item.Price;
            existingItem.Quantity = item.Quantity;
        }
    }

    public Item FindById(int id)
    {
        return data.FirstOrDefault(i => i.Id == id);
    }

    public IEnumerable<Item> Search(string value)
    {
        return data.Where(i =>
            i.Id.ToString().Contains(value) ||
            i.Name.Contains(value) ||
            i.Type.ToString().Contains(value) ||
            i.Price.ToString().Contains(value) ||
            i.Quantity.ToString().Contains(value)
        ).OrderBy(i => i.Name);
    }

    public IEnumerator<Item> GetEnumerator()
    {
        return data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return data.GetEnumerator();
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            using (IInventoryRepository inventory = InventoryRepository.Instance)
            {
                #region Add items

                inventory.Add(new Item(5, "Smartphone", ItemType.Electronics, 799.99, 15));
                inventory.Add(new Item(6, "Jeans", ItemType.Clothing, 39.99, 30));
                inventory.Add(new Item(7, "Milk", ItemType.Food, 1.99, 50));

                #endregion

                var itemToUpdate = inventory.FindById(5);
                itemToUpdate.Name = "Updated Smartphone";
                itemToUpdate.Price = 899.99;
                itemToUpdate.Quantity = 20;

                inventory.Update(itemToUpdate);

                Console.WriteLine($"Item {itemToUpdate.Id} updated successfully");
                Console.WriteLine(itemToUpdate.ToString());

                if (inventory.Delete(itemToUpdate))
                    Console.WriteLine($"Item {itemToUpdate.Id} deleted successfully");

                #region Search from inventory

                var searchData = inventory.Search("Chair");
                Console.WriteLine();
                Console.WriteLine($"Total Items: {searchData.Count()}");
                Console.WriteLine("----------------------------------");

                foreach (var item in searchData)
                {
                    Console.WriteLine(item.ToString());
                }

                #endregion
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            Console.ReadLine();
        }
        Console.Read();

    }
}
