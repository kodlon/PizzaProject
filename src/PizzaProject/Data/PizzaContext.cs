using Microsoft.EntityFrameworkCore;
using PizzaProject.Models;

namespace PizzaProject.Data;

public class PizzaContext(DbContextOptions<PizzaContext> options) : DbContext(options)
{
    public DbSet<Pizza> Pizzas => Set<Pizza>();
}
