using System.ComponentModel.DataAnnotations;

namespace PizzaProject.Dtos;

public record CreatePizzaRequest(
    [StringLength(100)] [Required] string Name,
    bool IsGlutenFree);