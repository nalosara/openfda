using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class AdverseEvent
{
    [Key]
    public Guid Id { get; set; }
    
    [JsonPropertyName("reactionmeddrapt")]
    public string? Reaction { get; set; }
    
    [JsonPropertyName("receivedate")]
    public DateTime? EventDate { get; set; }
    
    [JsonPropertyName("drugindication")]
    public string? DrugIndication { get; set; }

    [JsonPropertyName("medicinalproduct")]
    public string? DrugName { get; set; }

    [JsonPropertyName("patientsex")]
    public string? Gender { get; set; }

    public Guid? DrugId { get; set; }
    [ForeignKey("DrugId")]
    public Drug? Drug { get; set; }
}