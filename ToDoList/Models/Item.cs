using System.Collections.Generic;

namespace ToDoList.Models
{
  public class Item
  {
    private string _description;
    // private int _id;

    public Item (string description)
    {
      _description = description;
      // _id = _instances.Count;
    }

    public string GetDescription()
    {
      return _description;
    }

    public void SetDescription(string newDescription)
    {
      _description = newDescription;
    }

    // public int GetId()
    // {
    //   return _id;
    // }

    public static List<Item> GetAll()
    {
      //First we'll instantiate and return a new empty list meant to hold Items. This is where Item objects retrieved from our database will be placed:
      List<Item> allItems = new List<Item> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM items;";
      //For the reader object, we must call a built-in Read() method that sends the SQL Commands to the database and collects the database's response.  However, the act of reading a database can take a few moments. Especially if it contains a lot of records. So we house this method call in a while loop. Code within this loop will continue to run for the entire time the database is being read:
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      // rows from the database are returned by the rdr.Read() method as indexed arrays.
      while(rdr.Read())
      {
        int itemId = rdr.GetInt32(0);
        //So in the loop above, we define our itemId as rdr.GetInt32(0); because this will return the integer at the 0th index of the array returned by the reader.
        string itemDescription = rdr.GetString(1);
        // instantiate new Item objects and add them to our allItems list, like this:
        Item newItem = new Item(itemDescription, itemId);
        allItems.Add(newItem);

        //Here we simply pass itemIds and itemDescriptions retrieved for each entry in the database into our existing Item constructor, then add each new Item to the allItems list this method returns.

        //  But why do we have to use our constructor again? This is because the database doesn't contain actual objects. It just contains all of an object's data. So when we retrieve this from the database we must reconstruct them into C# objects.
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allItems;
    }

    // public static void ClearAll()
    // {
    //   _instances.Clear();
    // }

    // public static Item Find(int searchId)
    // {
    //   return _instances[searchId-1];
    // }

  }
}
