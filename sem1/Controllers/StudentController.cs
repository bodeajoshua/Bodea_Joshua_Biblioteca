using Microsoft.AspNetCore.Mvc;
using sem1.Models;

namespace sem1.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Info()
        {
            var student = new Student { Name = "Ana", Age = 21 };
            return View(student);
        }
    }
}
