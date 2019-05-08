using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using System.Collections.Generic;

namespace ToDoList.Controllers
{
  public class ItemsController : Controller
  {

    [HttpGet("/categories/{categoryId}/items/new")]
    public ActionResult New(int categoryId)
    {
       Category category = Category.Find(categoryId);
       return View(category);
    }

    [HttpGet("/categories/{categoryId}/items/{itemId}")]
    public ActionResult Show(int categoryId, int itemId)
    {
      Item item = Item.Find(itemId);
      Dictionary<string, object> model = new Dictionary<string, object>();
      Category category = Category.Find(categoryId);
      model.Add("item", item);
      model.Add("category", category);
      return View(model);
    }

    [HttpPost("/items/delete")]
    public ActionResult DeleteAll()
    {
      Item.ClearAll();
      return View();
    }

    //once route is added, go to Show.cshtml to access this route
    [HttpGet("/categories/{categoryId}/items/{itemId}/edit")]
    public ActionResult Edit(int categoryId, int itemId)
    {
      //create dict named model to hold the appropriate Item and Category
      Dictionary<string, object> model = new Dictionary<string, object>();
      //using each class' Find() method (category and item), we locate the appropriate Item and Category, add them to a Dict named model and pass dict into the view
      Category category = Category.Find(categoryId);
      model.Add("category", category);
      Item item = Item.Find(itemId);
      model.Add("item", item);
      //pass Dictionary into the view
      return View(model);
    }

    //create a route called Update(), we go here after form is submitted
    //Notice it takes three parameters: The itemId and categoryId from the URL path, and a newDescription string containing the updated description provided in the form.
    [HttpPost("/categories/{categoryId}/items/{itemId}")]
    public ActionResult Update(int categoryId, int itemId, string newDescription)
    {
      Item item = Item.Find(itemId);
      //locate Item we are Editing
      //call Edit() method on it
      //and pass in the newDescription that contains the updated description
      item.Edit(newDescription);
      //create new Dict, pass updated item and its parent category and pass into Show.cshtml view loaded after form submission
      Dictionary<string, object> model = new Dictionary<string, object>();
      Category category = Category.Find(categoryId);
      model.Add("category", category);
      model.Add("item", item);
      return View("Show", model);
    }


  }
}
