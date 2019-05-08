using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToDoList.Models;
using System.Collections.Generic;
using System;

namespace ToDoList.Tests
{
  [TestClass]
  public class ItemTest : IDisposable
  {

    public void Dispose()
    {
      Item.ClearAll();
    }

    public ItemTest()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=to_do_list_test;";
    }

    [TestMethod]
    public void ItemConstructor_CreatesInstanceOfItem_Item()
    {
      Item newItem = new Item("test");
      Assert.AreEqual(typeof(Item), newItem.GetType());
    }

    [TestMethod]
    public void GetDescription_ReturnsDescription_String()
    {
      //Arrange
      string description = "Walk the dog.";
      Item newItem = new Item(description);

      //Act
      string result = newItem.GetDescription();

      //Assert
      Assert.AreEqual(description, result);
    }

    [TestMethod]
    public void SetDescription_SetDescription_String()
    {
      //Arrange
      string description = "Walk the dog.";
      Item newItem = new Item(description);

      //Act
      string updatedDescription = "Do the dishes";
      newItem.SetDescription(updatedDescription);
      string result = newItem.GetDescription();

      //Assert
      Assert.AreEqual(updatedDescription, result);
    }

    [TestMethod]
    public void GetAll_ReturnsEmptyList_ItemList()
    {
      //Arrange
      List<Item> newList = new List<Item> { };

      //Act
      List<Item> result = Item.GetAll();

      //Assert
      CollectionAssert.AreEqual(newList, result);
    }

    [TestMethod]
    public void GetAll_ReturnsItems_ItemList()
    {
      //Arrange
      string description01 = "Walk the dog";
      string description02 = "Wash the dishes";
      Item newItem1 = new Item(description01);
      //We need to call Save() after creating each new Item.
      newItem1.Save();
      Item newItem2 = new Item(description02);
      newItem2.Save();
      List<Item> newList = new List<Item> { newItem1, newItem2 };

      //Act
      List<Item> result = Item.GetAll();

      //Assert
      CollectionAssert.AreEqual(newList, result);
    }

    [TestMethod]
    public void GetId_ItemsInstantiateWithAnIdAndGetterReturns_Int()
    {
      //Arrange
      string description = "Walk the dog.";
      Item newItem = new Item(description);

      //Act
      int result = newItem.GetId();

      //Assert
      Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void Find_ReturnsCorrectItemFromDatabase_Item()
    {
      //Arrange
      Item testItem = new Item("Mow the lawn");
      testItem.Save();

      //Act
      //This line uses the Item's _id to query the database and construct a new object from the information stored in that row.
      Item foundItem = Item.Find(testItem.GetId());

      //Assert
      Assert.AreEqual(testItem, foundItem);
    }

    //typecasting
    [TestMethod]
    public void Equals_ReturnsTrueIfDescriptionsAreTheSame_Item()
    {
      // Arrange, Act
      Item firstItem = new Item("Mow the lawn");
      Item secondItem = new Item("Mow the lawn");

      // Assert
      Assert.AreEqual(firstItem, secondItem);
    }

    [TestMethod]
    public void Save_SavesToDatabase_ItemList()
    {
      //Arrange
      //create new Item
      Item testItem = new Item("Mow the lawn");

      //Act
      //call built in Save() on just created new Item
      testItem.Save();
      //use GetAll() to retrieve all saved items from the database
      List<Item> result = Item.GetAll();
      List<Item> testList = new List<Item>{testItem};

      //Assert
      //We assert that the list returned from the GetAll() contains our new Item, thus confirming Save() successfully added this object's information to our database.
      CollectionAssert.AreEqual(testList, result);
    }

    //we must retrieve the ID assigned by the database each time we save a new Item with Save() - this is connected to the GetId() method in Item.cs
    [TestMethod]
    public void Save_AssignsIdToObject_Id()
    {
      //Arrange
      Item testItem = new Item("Mow the lawn");

      //Act
      testItem.Save();
      Item savedItem = Item.GetAll()[0];

      int result = savedItem.GetId();
      int testId = testItem.GetId();

      //Assert
      Assert.AreEqual(testId, result);
    }

    [TestMethod]
    public void Edit_UpdatesItemInDatabase_String()
    {
      //Arrange
      //creating and saving an Item
      string firstDescription = "Walk the Dog";
      Item testItem = new Item(firstDescription);
      testItem.Save();
      string secondDescription = "Mow the lawn";

      //Act
      //call Edit and pass in a new description
      testItem.Edit(secondDescription);
      //locate Item in database
      string result = Item.Find(testItem.GetId()).GetDescription();

      //Assert
      //assert that the located Item's description has been successfully changed to "Mow the lawn", if so we know our Edit() is working
      Assert.AreEqual(secondDescription, result);
    }

  }
}
