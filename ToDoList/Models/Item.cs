using System.Collections.Generic;
using MySql.Data.MySqlClient;
//this above accesses the SQL methods used in ClearAll()

namespace ToDoList.Models
{
  public class Item
  {
    private string _description;
    private int _id;

    //if an argument is not provided in the constructor, 0 will be used as default. That what int id = 0 declares. The constructor accepts an optional int argument to use as the Item's _id property.
    public Item (string description, int id = 0)
    {
      _description = description;
      _id = id;
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

    //this method assigns _id properties when we instantiate new Items.
    public int GetId()
    {
      return _id;
    }

    // For testing only:
    // public int GetId()
    // {
    // Temporarily returning dummy id to get beyond compiler errors, until we refactor to work with database.
    // return 0;
    // }

    //the GetAll() method returns all Item objects
    public static List<Item> GetAll()
    {
      //First we'll instantiate and return a new empty list meant to hold Items. This is where Item objects retrieved from our database will be placed:
      List<Item> allItems = new List<Item> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM items;";
      //For the reader object, we must call a built-in Read() method that sends the SQL Commands to the database and collects the database's response (we need to READ() the response when we collect data).  However, the act of reading a database can take a few moments. Especially if it contains a lot of records. So we house this method call in a while loop. Code within this loop will continue to run for the entire time the database is being read:
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      // rows from the database are returned by the rdr.Read() method as indexed arrays.
      while(rdr.Read())
      {
        int itemId = rdr.GetInt32(0);
        //So in the loop above, we define our itemId as rdr.GetInt32(0); because this will return the integer at the 0th index of the array returned by the reader.
        string itemDescription = rdr.GetString(1);
        // instantiate new Item objects and add them to our allItems list:
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

    //deletes all table entries in the database
    //we are connecting to the database, make sure Database.cs and Item.cs both have the same namespace
    public static void ClearAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM items;";
      //in GetAll() we needed to read the response, but we don't need to read anything when we delete something. In fact, we don't need anything returned to us at all. We use ExecuteNonQuery() for deletion.
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
      conn.Dispose();
      }
      //when done with this, don't forget to write the test, first test for an empty database! For that, first comment out all GetAll() content expect the lines creating and returning a List and the return dummyItem line; - also add "dummy item" to List<Item> dummyItem = new List<Item> {"dummy item"}; MySqlConnection conn = DB.Connection( dummyItem );
    }

    //this is a boilerplate, standard code we need to open and close our database connection:
    // public static Item Find(int id)
    // {
    //   MySqlConnection conn = DB.Connection();
    //   conn.Open();
    //
    //   // more logic will go here!
    //
    //   conn.Close();
    //   if (conn != null)
    //   {
    //     conn.Dispose();
    //   }
    // }

    //for testing only, so we can test first and have it fail:
    // public static Item Find(int searchId)
    // {
      // Temporarily returning dummy item to get beyond compiler errors, until we refactor to work with database.
    //   Item dummyItem = new Item("dummy item");
    //   return dummyItem;
    // }

    public static Item Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM `items` WHERE id = @thisId;";
      //create a MySqlParameter called thisId.
      MySqlParameter thisId = new MySqlParameter();
      //We define its ParameterName as @thisId, matching the parameter name used in our SQL command exactly.
      thisId.ParameterName = "@thisId";
      //The Value is what will replace the parameter in the command string when executed. We declare the Value of thisId as id, referring to the id passed into Find() as a parameter.
      thisId.Value = id;
      //Next we associate the MySqlParameter object we've just defined with the SqlCommand to be executed on our database. Remember, if we had multiple parameters we would need to call Add() on each.
      cmd.Parameters.Add(thisId);
      //SQL to locate a specific Item is a query because it will return something; the Item data it found!
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      //To initiate reading the database we run a while loop, passing the Read() action in as a parameter.
      //We define a itemId and itemDescription above the while loop. This ensures that we don't hit unanticipated errors if the code in while isn't able to define these values.
      int itemId = 0;
      string itemDescription = "";
      //The while loop will take each returned row of data and perform actions with it. Within the while loop, we can call methods on the rdr object. The rows are returned as indexed arrays.
      //We execute two methods on rdr; GetInt32() and GetString(). We pass integers into these methods as arguments. These integers correspond to the index positions within the row of the data we're collecting.
      while (rdr.Read())
      {
        itemId = rdr.GetInt32(0);
        itemDescription = rdr.GetString(1);
      }
      //we'll create and return a new Item object with the values we located.
      Item foundItem= new Item(itemDescription, itemId);
      
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    //this should be done before writing our Save() method
    public override bool Equals(System.Object otherItem)
    {
      if (!(otherItem is Item))
      {
        return false;
      }
      else
      {
        Item newItem = (Item) otherItem;
        bool idEquality = (this.GetId() == newItem.GetId());
        bool descriptionEquality = (this.GetDescription() == newItem.GetDescription());
        return (idEquality && descriptionEquality);
      }
    }

    // After overriding Equals(), Save() method saves new Items to database, we use void because we don't need to return anything. Make it first empty to be able to run test that should first fail
    public void Save()
    {
      //create a new MySqlConnection called conn just like before
      MySqlConnection conn = DB.Connection();
      conn.Open();
      //We create a new cmd object with CreateCommand(), casting it to the MySqlCommand type our database requires.
      var cmd = conn.CreateCommand() as MySqlCommand;
      //We use the parameter placeholder @ItemDescription. We want to use parameter placeholders like this whenever we are entering data that a user enters.
      cmd.CommandText = @"INSERT INTO items (description) VALUES (@ItemDescription);";
      //Next we create a MySqlParameter object for each parameter required in our MySqlCommand. We call the MySqlParameter - description.
      MySqlParameter description = new MySqlParameter();
      //We define the ParameterName property of description as @ItemDescription (matching with above)
      description.ParameterName = "@ItemDescription";
      //The Value is what will replace the parameter in the command string when it is executed.
      //We define the Value property of description as this._description. This refers to the private _description property of the Item we're saving. (eg: "Mow the lawn", "Do the dishes", etc.)
      description.Value = this._description;
      //Next we make sure these MySqlParameter object(s) are associated with the SqlCommand executed on our database. We add it to the MySqlCommand's Parameters property using Add() and passing in the parameter as its argument. If we had more parameters to add, we would need to Add each one.
      cmd.Parameters.Add(description);
      //We use the ExecuteNonQuery() method because saving a new entry is considered a "non-query" because it doesn't need to return anything in response.
      cmd.ExecuteNonQuery();
      //gather the SQL-assigned primary key ID each time a new Item entry is added to the database, and override the Item's _id property with this database-provided ID.
      //explicit conversion, by including that (int) statement on the right side of the =. We are converting _id which is a long datatype to int.s
      _id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }



  }
}
