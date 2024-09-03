using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAdressBook.Models
{
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(13)]
        public string Hp { get; set; }
    }
}

--------

  using Microsoft.EntityFrameworkCore;

namespace WebAdressBook.Models
{
    public class PersonDbContext : DbContext
    {
        public PersonDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Person> Persons { get; set; }
    }
}
-----------

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Diagnostics;
using WebAdressBook.Models;

namespace WebAdressBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly PersonDbContext context;

        public HomeController(PersonDbContext _context, ILogger<HomeController> _logger)
        {
            logger = _logger;
            context = _context;
        }

        public IActionResult Index()
        {
            var persons = context.Persons.ToList();
            return View(persons);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Person person)
        {
            if (ModelState.IsValid)
            {
                context.Persons.Add(person);
                context.SaveChanges();

                return RedirectToAction("Index", "Home"); //두번째 나오는 매개변수는 컨트롤러 
            }

            return View(person);
        }

        public IActionResult Edit(int? id) //읽기
        {
            if(id == null || context.Persons == null)
            {
                return NotFound();
            }
            var personData = context.Persons.Find(id);

            if (personData == null)
            {
                return NotFound();
            }
            return View(personData);
        }

        [HttpPost]
        public IActionResult Edit(int? id, Person person)
        {
            if (id != person.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //_context.Student.Update(std);
                context.Update(person);
                context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(person);

        }

        public IActionResult Delete(int? id) //읽기
        {
            if (id == null || context.Persons == null)
            {
                return NotFound();
            }

            //id값을 읽어오기 
            var personData = context.Persons.FirstOrDefault(x => x.Id == id);

            if (personData == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost, ActionName("Delete")] //쓰기
        public IActionResult DeleteConfirmed(int? id)
        {
            var personData = context.Persons.Find(id);
            if (personData != null)
            {
                context.Persons.Remove(personData);
            }
            context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Details(int? id)
        {
            if(id == null || context.Persons == null)
            {
                return NotFound();
            }

            var personData = context.Persons.FirstOrDefault(x => x.Id == id);

            if (personData == null)
            {
                return NotFound();
            }
            
            return View(personData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

----------

public IActionResult Index()
{
    var persons =context.Persons.ToList();
    return View(persons);
}

----------

  public IActionResult Create() //읽기
{
    return View();
}

[HttpPost]
public IActionResult Create(Person person) //쓰기
{
    if (ModelState.IsValid)
    {
        context.Persons.Add(person);
        context.SaveChanges();

        return RedirectToAction("Index", "Home"); //두번째 나오는 매개변수는 컨트롤러 
    }

    return View(person);
}

-----------------

public IActionResult Edit(int? id) //읽기
{
    if(id == null || context.Persons == null)
    {
        return NotFound();
    }
    var personData = context.Persons.Find(id);

    if (personData == null)
    {
        return NotFound();
    }
    return View(personData);
}

[HttpPost]
public IActionResult Edit(int? id, Person person) //쓰기
{
    if (id != person.Id)
    {
        return NotFound();
    }
    if (ModelState.IsValid)
    {
        //_context.Student.Update(std);
        context.Update(person);
        context.SaveChanges();
        return RedirectToAction("Index", "Home");
    }
    return View(person);

}

---------------

public IActionResult Delete(int? id) //읽기
{
    if (id == null || context.Persons == null)
    {
        return NotFound();
    }

    //id값을 읽어오기 
    var personData = context.Persons.FirstOrDefault(x => x.Id == id);

    if (personData == null)
    {
        return NotFound();
    }

    return View();
}

[HttpPost, ActionName("Delete")] //쓰기
public IActionResult DeleteConfirmed(int? id)
{
    var personData = context.Persons.Find(id);
    if (personData != null)
    {
        context.Persons.Remove(personData);
    }
    context.SaveChanges();
    return RedirectToAction("Index", "Home");
}

public IActionResult Details(int? id)
{
    if(id == null || context.Persons == null)
    {
        return NotFound();
    }

    var personData = context.Persons.FirstOrDefault(x => x.Id == id);

    if (personData == null)
    {
        return NotFound();
    }
    
    return View(personData);
}

-------------

  using Microsoft.EntityFrameworkCore;
using WebAdressBook.Models;


namespace WebAdressBook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            ///////////////////////////////////////////////////////////

            var provider = builder.Services.BuildServiceProvider();
            var config = provider.GetRequiredService<IConfiguration>();
            builder.Services.AddDbContext<PersonDbContext>(item => item.UseSqlServer(config.GetConnectionString("DefaultConnection")));


            //////////////////////////////////////////////////////////
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

--------------------

  using Microsoft.EntityFrameworkCore;
using WebAdressBook.Models;


namespace WebAdressBook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            ///////////////////////////////////////////////////////////

            var provider = builder.Services.BuildServiceProvider();
            var config = provider.GetRequiredService<IConfiguration>();
            builder.Services.AddDbContext<PersonDbContext>(item => item.UseSqlServer(config.GetConnectionString("DefaultConnection")));


            //////////////////////////////////////////////////////////
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
