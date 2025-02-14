using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class AdverseEventService
{
    private readonly HttpClient _httpClient;
    private readonly string? _baseUrl;
    private readonly string? _apiKey;
    private readonly AppDbContext _context;

    public AdverseEventService(HttpClient httpClient, IConfiguration configuration, AppDbContext context)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenFda:ApiKey"];
        _baseUrl = configuration["OpenFDA:BaseUrl"];
        _context = context;
    }

    public async Task<List<AdverseEvent>> GetAdverseEventsAsync(int limit = 10, int skip = 0)
    {
        try
        {
            var requestUrl = $"{_baseUrl}/drug/event.json?api_key={_apiKey}&limit={limit}&skip={skip}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return new List<AdverseEvent>();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonResponse);

            if (!jsonDocument.RootElement.TryGetProperty("results", out var results))
            {
                Console.WriteLine("The 'results' property is missing in the API response.");
                return new List<AdverseEvent>();
            }

            var adverseEvents = new List<AdverseEvent>();

            foreach (var result in results.EnumerateArray())
            {
                try
                {
                    if (!result.TryGetProperty("patient", out var patient)) continue;
                    if (!patient.TryGetProperty("reaction", out var reactions) || reactions.GetArrayLength() == 0) continue;
                    if (!patient.TryGetProperty("drug", out var drugs) || drugs.GetArrayLength() == 0) continue;

                    var reaction = reactions.EnumerateArray().FirstOrDefault();
                    var drug = drugs.EnumerateArray().FirstOrDefault();

                    var drugId = GetDrugIdFromApiResponse(drug);
                    var drugName = drug.TryGetProperty("medicinalproduct", out var drugNameProp)
                        ? drugNameProp.GetString()
                        : null;

                    var adverseEvent = new AdverseEvent
                    {
                        Reaction = reaction.TryGetProperty("reactionmeddrapt", out var reactionProp)
                            ? reactionProp.GetString()
                            : null,

                        EventDate = result.TryGetProperty("receivedate", out var dateProp) && DateTime.TryParse(dateProp.GetString(), out var date)
                            ? date
                            : null,

                        DrugIndication = drug.TryGetProperty("drugindication", out var indicationProp)
                            ? indicationProp.GetString()
                            : null,
                        Gender = patient.TryGetProperty("patientsex", out var genderProp)
                            ? genderProp.GetString() switch
                            {
                                "1" => "Male",
                                "2" => "Female",
                                _ => "Unknown"
                            }
                            : "Unknown",

                        DrugName = drugName,
                        DrugId = drugId,
                    };

                    adverseEvents.Add(adverseEvent);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing adverse event: {ex.Message}");
                }
            }

            return adverseEvents;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return new List<AdverseEvent>();
        }
    }

    private Guid? GetDrugIdFromApiResponse(JsonElement drug)
    {
        if (!drug.TryGetProperty("medicinalproduct", out var drugNameProp)) return null;

        var drugName = drugNameProp.GetString()?.Trim().ToLower();
        if (string.IsNullOrEmpty(drugName)) return null;

        var allDrugs = _context.Drugs.Include(d => d.OpenFda).ToList();

        var existingDrug = allDrugs.FirstOrDefault(d =>
            (d.ActiveIngredient != null && d.ActiveIngredient.Any(ai => ai?.Trim().ToLower().Contains(drugName) == true)) ||
            (d.OpenFda?.GenericName != null && d.OpenFda.GenericName.Any(gn => gn?.Trim().ToLower().Contains(drugName) == true))
        );

        return existingDrug?.Id;
    }

    public async Task FetchAndStoreAdverseEventsAsync(int totalLimit = 1000, int batchSize = 500)
    {
        if (_context.AdverseEvents.Any())
        {
            Console.WriteLine("Adverse events already exist in the database. Skipping API fetch.");
            return;
        }
        
        int fetchedCount = 0;
        int skip = 0;

        while (fetchedCount < totalLimit)
        {
            var adverseEvents = await GetAdverseEventsAsync(batchSize, skip);

            if (adverseEvents == null || !adverseEvents.Any())
            {
                Console.WriteLine("No more adverse events fetched from the API.");
                break;
            }

            await _context.AdverseEvents.AddRangeAsync(adverseEvents);
            await _context.SaveChangesAsync();

            fetchedCount += adverseEvents.Count;
            skip += batchSize;
            Console.WriteLine($"Fetched and stored {adverseEvents.Count} adverse events. Total fetched: {fetchedCount}");
        }

        Console.WriteLine($"Finished fetching and storing {fetchedCount} adverse events.");
    }
}
