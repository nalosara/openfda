using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var topReactions = await _context.AdverseEvents
            .GroupBy(a => a.Reaction)
            .Select(g => new { Reaction = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToListAsync();

        var genderDistribution = await _context.AdverseEvents
            .GroupBy(a => a.Gender)
            .Select(g => new { Gender = g.Key, Count = g.Count() })
            .ToListAsync();

        ViewData["TopReactions"] = JsonSerializer.Serialize(topReactions);
        ViewData["GenderDistribution"] = JsonSerializer.Serialize(genderDistribution);

        return View();
    }

}
