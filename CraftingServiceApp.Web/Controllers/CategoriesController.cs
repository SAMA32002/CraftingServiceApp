using CraftingServiceApp.Application.Interfaces;
using CraftingServiceApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CraftingServiceApp.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IRepository<Category> _repository;

        public CategoriesController(IRepository<Category> repository)
        {
            _repository = repository;
        }

        //private readonly ApplicationDbContext _context;

        //public CategoryController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        // عرض جميع الفئات
        public IActionResult Index()
        {
            //var categories = _context.Categories.ToList();
            var categories = _repository.GetAll();
            return View(categories);
        }

        // عرض تفاصيل فئة معينة
        public IActionResult Details(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = _repository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // عرض نموذج إنشاء فئة جديدة
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // إرسال وحفظ بيانات الفئة الجديدة
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            _repository.Add(category);
            _repository.SaveChanges();
            return RedirectToAction(nameof(Index));

            //if (ModelState.IsValid)
            //{
            //    _repository.Add(category);
            //    _repository.SaveChanges();
            //    //_context.Categories.Add(category);
            //    //_context.SaveChanges();
            //    return RedirectToAction(nameof(Index));
            //}
            return View(category);
        }

        // عرض نموذج تعديل فئة
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _repository.GetById(id);
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // إرسال وحفظ التعديلات على الفئة
        [HttpPost]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id) // التحقق من أن `id` يطابق `category.Id`
            {
                return BadRequest();
            }
            _repository.Update(category);
            _repository.SaveChanges();
            return RedirectToAction(nameof(Index));

            //if (ModelState.IsValid)
            //{
            //    _repository.Update(category);
            //    _repository.SaveChanges();
            //    //_context.Categories.Update(category);
            //    //_context.SaveChanges();
            //    return RedirectToAction(nameof(Index));
            //}
            return View(category);
        }

        // حذف فئة معينة (عرض نموذج التأكيد)
        [HttpGet]
        public IActionResult Delete(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = _repository.GetById(id);
            if (category == null)
            {
                return BadRequest();
            }
            return View(category);
        }

        // تأكيد وحذف الفئة
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = _repository.GetById(id);
            if (category != null)
            {
                _repository.Delete(category);
                _repository.SaveChanges();
                //_context.Categories.Remove(category);
                //_context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }

}
