using System.ComponentModel.DataAnnotations;

namespace PizzaProject.Models;

public class Pizza
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string? Name { get; set; }

    public bool IsGlutenFree { get; set; }
}