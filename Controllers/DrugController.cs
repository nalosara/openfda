using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class DrugController : Controller
{
    private readonly DrugService _drugService;
    private readonly AppDbContext _context;

    public DrugController(DrugService drugService, AppDbContext context)
    {
        _drugService = drugService;
        _context = context;
    }

    [HttpGet]
    public IActionResult Drug()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> FetchDrugs(int page = 1, int itemsPerPage = 6, string searchQuery = "")
    {
        var query = _context.Drugs.Include(d => d.OpenFda).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var dru = await query.ToListAsync();

            var lowerSearchQuery = searchQuery.ToLower();

            dru = dru.Where(d => 
                d.OpenFda.GenericName != null && 
                d.OpenFda.GenericName.Any(name => name.ToLower().Contains(lowerSearchQuery))
            ).ToList();

            var totalItems = dru.Count();
            var paginatedDrugs = dru
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .Select(d => new
                {
                    d.Id,
                    d.Purpose,
                    OpenFda = new
                    {
                        GenericName = d.OpenFda.GenericName.FirstOrDefault()
                    }
                })
                .ToList();

            var total = (int)Math.Ceiling(totalItems / (double)itemsPerPage);

            return Json(new { drugs = paginatedDrugs, total });
        }

        int totalDrugs = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalDrugs / itemsPerPage);

        var drugs = await query
            .Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .Select(d => new
            {
                d.Id,
                d.Purpose,
                OpenFda = new
                {
                    GenericName = d.OpenFda.GenericName.FirstOrDefault()
                }
            })
            .ToListAsync();

        return Json(new { totalPages, drugs }); 
    }

    [HttpGet]
    public async Task<IActionResult> GetDetails(Guid id)
    {
        var drug = await _context.Drugs
            .Include(d => d.OpenFda)
            .Where(d => d.Id == id)
            .FirstOrDefaultAsync();

        if (drug == null)
        {
            return Json(new { success = false, message = "Drug not found" });
        }

        var allDrugs = await _context.Drugs
            .ToListAsync();

        var indicationsData = allDrugs
            .Where(d => d.IndicationsAndUsage != null && d.IndicationsAndUsage.Any())
            .SelectMany(d => d.IndicationsAndUsage)
            .GroupBy(i => i)
            .Select(g => new
            {
                Indication = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToList();

        var indicationLabels = indicationsData.Select(i => i.Indication).ToList();
        var indicationCounts = indicationsData.Select(i => i.Count).ToList();

        var drugDetails = new
        {
            Purpose = drug.Purpose != null && drug.Purpose.Any() ? string.Join(", ", drug.Purpose) : "No purpose available",
            ActiveIngredient = drug.ActiveIngredient != null && drug.ActiveIngredient.Any() ? string.Join(", ", drug.ActiveIngredient) : "No active ingredients available",
            Indications = drug.IndicationsAndUsage != null && drug.IndicationsAndUsage.Any() ? string.Join(", ", drug.IndicationsAndUsage) : "No indications available",
            Warnings = drug.Warnings != null && drug.Warnings.Any() ? string.Join(", ", drug.Warnings) : "No warnings available",
            Dosage = drug.DosageAndAdministration != null && drug.DosageAndAdministration.Any() ? string.Join(", ", drug.DosageAndAdministration) : "No dosage information available",
            OpenFda = new
            {
                GenericName = drug.OpenFda?.GenericName != null ? string.Join(", ", drug.OpenFda.GenericName) : "N/A",
                ManufacturerName = drug.OpenFda?.ManufacturerName != null ? string.Join(", ", drug.OpenFda.ManufacturerName) : "N/A",
                BrandName = drug.OpenFda?.BrandName != null ? string.Join(", ", drug.OpenFda.BrandName) : "N/A"
            },
            indicationLabels,
            indicationCounts
        };

        return Json(new { success = true, data = drugDetails });
    }
}
