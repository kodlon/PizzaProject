using System.ComponentModel.DataAnnotations;

namespace PizzaProject.Dtos;

public record UpdatePizzaRequest(
    [StringLength(100)] [Required] string Name,
    bool IsGlutenFree);