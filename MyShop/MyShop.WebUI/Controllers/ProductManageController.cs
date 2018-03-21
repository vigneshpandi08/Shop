using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShop.Core.Models;
using MyShop.DataAccess.InMemory;
using MyShop.Core.ViewModel;
using MyShop.Core.Contracts;
using System.IO;

namespace MyShop.WebUI.Controllers
{
    public class ProductManageController : Controller
    {
        IRepo<Product> Context;
        IRepo<ProductCategory> productCategories;
        public ProductManageController(IRepo<Product> productContext, IRepo<ProductCategory> productCategoryContext)
        {
            Context = productContext;
            productCategories = productCategoryContext;
        }
        // GET: ProductManage
        public ActionResult Index()
        {
            List<Product> products = Context.Collection().ToList();
            return View(products);
        }
        public ActionResult Create()
        {
            ProductViewModel viewModel = new ProductViewModel();
            viewModel.Product = new Product();
            viewModel.ProductCategories = productCategories.Collection();
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Create(Product product, HttpPostedFileBase file)
        {
            if(!ModelState.IsValid)
            {
                return View(product);
            }
            else 
            {
                if(file!=null)
                {
                    product.Image = product.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("//Content//ProductImages//") +product .Image );
                }

                Context.Insert(product);
                Context.Commit();
                return RedirectToAction("Index");
            }
            
        }
        public ActionResult Edit(string Id)
        {
            Product product = Context.Find(Id);
            if(product==null)
            {
                return HttpNotFound();
            }
            else
            {
                ProductViewModel viewModel = new ProductViewModel();
                viewModel.Product = product;
                viewModel.ProductCategories = productCategories.Collection();
                return View(viewModel);
            }
        }
        [HttpPost]
        public ActionResult Edit(Product product,string Id, HttpPostedFileBase file)
        {
            Product productToEdit = Context.Find(Id);
            if (productToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                if(!ModelState.IsValid)
                {
                    return View(product);
                }

                if (file != null)
                {
                    productToEdit.Image = product.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("//Content//ProductImages//") + productToEdit.Image);
                }

                productToEdit.Category = product.Category;
                productToEdit.Description = product.Description;
                productToEdit.Name = product.Name;
                productToEdit.Image = product.Image;
                productToEdit.Price = product.Price;

                Context.Commit();
                return RedirectToAction("Index");
            }
        }
        public ActionResult Delete(string Id)
        {
            Product productToDelete = Context.Find(Id);
            if (productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productToDelete);
            }
        }
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            Product productToDelete = Context.Find(Id);
            if (productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                Context.Delete(Id);
                Context.Commit();
                return RedirectToAction("Index");
                //return View(productToDelete);
            }
        }
    }
}