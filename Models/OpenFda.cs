using System.Text.Json.Serialization;

public class OpenFda
{
    public Guid Id { get; set; } 

    [JsonPropertyName("brand_name")]
    public List<string> BrandName { get; set; } = new();

    [JsonPropertyName("generic_name")]
    public List<string>? GenericName { get; set; } = new(); 

    [JsonPropertyName("manufacturer_name")]
    public List<string> ManufacturerName { get; set; } = new();

    public virtual ICollection<Drug> Drugs { get; set; } 

}
