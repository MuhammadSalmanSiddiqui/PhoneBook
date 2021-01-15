using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternetPhoneBook.Helpers;
using InternetPhoneBook.Models;
using Microsoft.AspNetCore.Mvc;


namespace InternetPhoneBook.Controllers
{
	public class PersonController : Controller
	{
		public IActionResult Index(int page = 1)
		{
			ViewBag.AllRows = Math.Ceiling(SourceManager.NumberAllRows() / 3.0);
			ViewBag.page = page;
			List < PersonModel > persons = SourceManager.Get(page, 3, out int num);
			if (num != -1)
			{
				return View(persons);
			}
			else
			{
				ViewBag.Info = "no contacts";
				return View();
			}
		}
		[HttpGet]
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Add(PersonModel person)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrEmpty(person.Email)) person.Email = "no email adress";
				var id = SourceManager.Add(person);
				TempData["Info"] = $"Dodano: {person.FirstName} {person.LastName}";
				return Redirect("Index");
			}
			return View(person);
		}

		[HttpGet]
		public IActionResult RemoveConfirm(int id)
		{
			PersonModel person = SourceManager.GetByID(id);
			return View(person);
		}

		[HttpPost]
		public IActionResult Remove(int id)
		{
			PersonModel person = SourceManager.GetByID(id);
			SourceManager.Delete(id);
			TempData["Info"] = $"Usunięto {person.FirstName} {person.LastName}";
			return Redirect("Index");
		}

		[HttpGet]
		public IActionResult Edit(int id)
		{
			PersonModel person = SourceManager.GetByID(id);
			return View(person);
		}

		[HttpPost]
		public IActionResult Edit(PersonModel person)
		{
			if (ModelState.IsValid)
			{
				if (string.IsNullOrEmpty(person.Email)) person.Email = "no email adress";
				int id = SourceManager.Edit(person);
				TempData["Info"] = $"Change data for {person.FirstName} {person.LastName}";
				return Redirect("/Person/Index");
			}

			return View(person);
		}

		public IActionResult Search(string name, int page = 1)
		{
			if (string.IsNullOrEmpty(name))
			{
				TempData["Info"] = $"Enter full or part of name";
				return Redirect("/Person/Index");
			}

			List<PersonModel> personsPag = SourceManager.GetByName(name, out int num, page);

			if (num == 0)
			{
				TempData["Info"] = $"No results";
				return Redirect("/Person/Index");
			}

			ViewBag.Page = page;
			ViewBag.Rows = (int)Math.Ceiling(num / 3.0);
			ViewBag.Name = name;
			return View(personsPag);
		}
	}
}

