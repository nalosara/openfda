using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class OpenFdaService
{
    private readonly HttpClient _httpClient;
    private readonly string? _baseUrl;
    private readonly string? _apiKey; 

    public OpenFdaService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenFda:ApiKey"];
        _baseUrl = configuration["OpenFDA:BaseUrl"];
    }

    public async Task<List<Drug>> GetDrugsAsync(int limit = 500)
    {
        try
        {
            var requestUrl = $"{_baseUrl}/drug/label.json?api_key={_apiKey}&limit={limit}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                return new List<Drug>();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var drugsResponse = JsonSerializer.Deserialize<OpenFdaResponse<Drug>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (drugsResponse?.results != null)
            {
                var filteredDrugs = drugsResponse.results
                    .Where(drug => drug.OpenFda != null && 
                                drug.OpenFda.GenericName != null && 
                                drug.OpenFda.GenericName.Any(name => !string.IsNullOrWhiteSpace(name)))
                    .ToList();

                return filteredDrugs;
            }

            return new List<Drug>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return new List<Drug>();
        }
    }

    public async Task<List<Drug>> FetchAndStoreDrugsAsync()
    {
        return await GetDrugsAsync(); 
    }
}

public class OpenFdaResponse<T>
{
    public List<T> results { get; set; } = new List<T>();
}
