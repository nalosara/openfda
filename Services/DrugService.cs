public class DrugService
{
    private readonly OpenFdaService _openFdaService;
    private readonly AppDbContext _context;

    public DrugService(OpenFdaService openFdaService, AppDbContext context)
    {
        _openFdaService = openFdaService;
        _context = context;
    }

    public async Task FetchAndStoreDrugsAsync()
    {
        if (_context.Drugs.Any())
        {
            Console.WriteLine("Drugs already exist in the database. Skipping API fetch.");
            return;
        }
        var drugs = await _openFdaService.GetDrugsAsync();

        if (drugs != null && drugs.Any())
        {
            await _context.Drugs.AddRangeAsync(drugs);
            await _context.SaveChangesAsync();
        }
        else
        {
            Console.WriteLine("No drugs fetched from the API.");
        }
    }

}