using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AdverseEventController : Controller
{
    private readonly AdverseEventService _adverseEventService;
    private readonly AppDbContext _context;


    public AdverseEventController(AdverseEventService adverseEventService, AppDbContext context)
    {
        _adverseEventService = adverseEventService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        await _adverseEventService.FetchAndStoreAdverseEventsAsync(totalLimit: 1000, batchSize: 100);

        var adverseEvents = await _context.AdverseEvents.ToListAsync();
        return View(adverseEvents);
    }
}