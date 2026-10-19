using System.ComponentModel.DataAnnotations;

namespace PizzaProject.Models;

public class Pizza
{
    public int Id { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public required string Name { get; set; }

    public bool IsGlutenFree { get; set; }
}