using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Drug
{
    [Key]
    public Guid Id { get; set; }

    [JsonPropertyName("purpose")]
    public List<string>? Purpose { get; set; }

    [JsonPropertyName("warnings")]
    public List<string> Warnings { get; set; } = new();

    [JsonPropertyName("dosage_and_administration")]
    public List<string> DosageAndAdministration { get; set; } = new();

    [JsonPropertyName("indications_and_usage")]
    public List<string> IndicationsAndUsage { get; set; } = new();

    [JsonPropertyName("active_ingredient")]
    public List<string> ActiveIngredient { get; set; } = new();

    [JsonPropertyName("spl_product_data_elements")]
    public List<string>? SplProductDataElements { get; set; } = new();
    
    [JsonPropertyName("inactive_ingredient")]
    public List<string>? InactiveIngredient { get; set; } = new();
    public List<string>? PackageLabelPrincipalDisplayPanel { get; set; } = new();

    [ForeignKey("OpenFdaId")]  
    public virtual OpenFda OpenFda { get; set; }

    public Guid OpenFdaId { get; set; }

    public List<AdverseEvent> AdverseEvents { get; set; } = new List<AdverseEvent>();
}
