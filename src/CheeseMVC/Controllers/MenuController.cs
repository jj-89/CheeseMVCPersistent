using CheeseMVC.Data;
using CheeseMVC.Models;
using CheeseMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheeseMVC.Controllers
{
    public class MenuController : Controller
    {
        private readonly CheeseDbContext context;

        public MenuController(CheeseDbContext dbContext)
        {
            context = dbContext;
        }

        public IActionResult Index()
        {
            List<Menu> menus = context.Menus.ToList();

            return View(menus);
        }

        [HttpGet]
        public IActionResult Add()
        {
            AddMenuViewModel addMenuViewModel = new AddMenuViewModel();

            return View(addMenuViewModel);
        }

        [HttpPost]
        public IActionResult Add(AddMenuViewModel addMenuViewModel)
        {
            if (ModelState.IsValid)
            {
                Menu newMenu = new Menu
                {
                    Name = addMenuViewModel.Name
                };
                context.Menus.Add(newMenu);
                context.SaveChanges();

                return Redirect("/Menu/ViewMenu/" + newMenu.ID);
            }

            return View();
        }

        [HttpGet]
        public IActionResult ViewMenu(int id)
        {
            List<CheeseMenu> items = context
                .CheeseMenus
                .Include(item => item.Cheese)
                .Where(cm => cm.MenuID == id)
                .ToList();

            Menu menu = context.Menus.Single(m => m.ID == id);

            ViewMenuViewModel viewMenuViewModel = new ViewMenuViewModel
            {
                Menu = menu,
                Items = items
            };

            return View(viewMenuViewModel);
        }

        [HttpGet]
        public IActionResult AddItem(int id)
        {
            Menu menu = context.Menus.Single(m => m.ID == id);
            List<Cheese> cheeses= context.Cheeses.ToList();

            return View(new AddMenuItemViewModel(menu, cheeses));
        }

        [HttpPost]
        public IActionResult AddItem(AddMenuItemViewModel addMenuItemViewModel)
        {
            

            if (ModelState.IsValid)
            {
                var cheeseID = addMenuItemViewModel.CheeseID;
                var menuID = addMenuItemViewModel.MenuID;

                IList<CheeseMenu> existingItems = context.CheeseMenus.Where(cm => cm.CheeseID == cheeseID).Where(cm => cm.MenuID == menuID).ToList();

                CheeseMenu newMenuItem = new CheeseMenu
                {
                    Cheese = context.Cheeses.Single(c => c.ID == cheeseID),
                    Menu = context.Menus.Single(m => m.ID == menuID)
                };

                if (existingItems.Count == 0)
                {
                    
                    context.CheeseMenus.Add(newMenuItem);
                    context.SaveChanges();
                }
                return Redirect("/Menu/ViewMenu/" + newMenuItem.Menu.ID);
            }

            return View();
        }


    }
}
