//using FYPAPI.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Web;
//using System.Web.Mvc;

//namespace FYPAPI.Controllers
//{
//    public class STDController : Controller
//    {


//        HttpClient client = new HttpClient();
//        //public ActionResult Index()
//        //{
//        //    List<Tlogin> e_list = new List<Tlogin>();
//        //    client.BaseAddress = new Uri("https://localhost:44313/api/Student");
//        //    var response = client.GetAsync("Student");
//        //    response.Wait();
//        //    var test = response.Result;
//        //    if (test.IsSuccessStatusCode)
//        //    {

//        //        var display = test.Content.ReadAsAsync<List<Tlogin>>();
//        //        display.Wait();
//        //        e_list = display.Result;

//        //    }
//        //    return View(e_list);
//        //}

//        //public ActionResult Create()
//        //{
//        //    return View();
//        //}

//        //[HttpPost]
//        //public ActionResult Create(Tlogin t)
//        //{
//        //    client.BaseAddress = new Uri("https://localhost:44313/api/Student/GetStudent");
//        //    var response = client.PostAsJsonAsync<Tlogin>("Student", t);
//        //    response.Wait();
//        //    var test = response.Result;
//        //    if (test.IsSuccessStatusCode)
//        //    {

//        //        return RedirectToAction("Index");

//        //    }
//        //    return View("Create");
//        //}


//        //public ActionResult Details(int id)
//        //{
//        //    Tlogin s = null;
//        //    client.BaseAddress = new Uri("https://localhost:44313/api/Student");
//        //    var response = client.GetAsync("Student?id=" + id.ToString());

//        //    response.Wait();
//        //    var test = response.Result;
//        //    if (test.IsSuccessStatusCode)
//        //    {

//        //        var display = test.Content.ReadAsAsync<Tlogin>();
//        //        display.Wait();
//        //        s = display.Result;

//        //    }

//        //    return View(s);
//        //}
//        //[HttpDelete]
//        //public ActionResult Delet(int id)
//        //{
//        //    Tlogin s = null;
//        //    client.BaseAddress = new Uri("https://localhost:44313/api/Student");
//        //    var response = client.GetAsync("Student?id=" + id.ToString());

//        //    response.Wait();
//        //    var test = response.Result;
//        //    if (test.IsSuccessStatusCode)
//        //    {

//        //        var display = test.Content.ReadAsAsync<Tlogin>();
//        //        display.Wait();
//        //        s = display.Result;

//        //    }

//        //    return View(s);
//        //}
//        //[HttpPost,ActionName("Delet")]
//        //public ActionResult DeletConfirm(int id)
//        //{
//        //    return View();
//        //}
//        //[HttpPut]
//        //public ActionResult Edit(Tlogin s)
//        //{
           
//        //    client.BaseAddress = new Uri("https://localhost:44313/api/Student");
//        //    var response = client.PutAsJsonAsync<Tlogin>("Student", s);

//        //    response.Wait();
//        //    var test = response.Result;
//        //    if (test.IsSuccessStatusCode)
//        //    {
//        //        RedirectToAction("Index");
                

//        //    }

//        //    return View("Edit");
//        //}

//        // GET: STD

//    }
//}