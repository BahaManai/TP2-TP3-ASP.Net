using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ASPCoreFirstApp.Models;

public class MovieReview
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("movieId")]
    public int MovieId { get; set; }

    [BsonElement("rating")]
    [Range(1, 5, ErrorMessage = "La note doit etre comprise entre 1 et 5.")]
    public int Rating { get; set; }

    [BsonElement("comment")]
    [Required(ErrorMessage = "Le commentaire est obligatoire.")]
    [StringLength(500, ErrorMessage = "Le commentaire ne doit pas dépasser 500 caractères.")]
    public string Comment { get; set; } = string.Empty;

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = [];
    [BsonElement("spoiler")]
    public bool Spoiler { get; set; }
    [BsonElement("language")]
    public string? Language { get; set; }
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
