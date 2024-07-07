using Microsoft.AspNetCore.Identity;
using StockCheckSheetWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace StockCheckSheet.DataAccess.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public int Name { get; set; }

        // Variables de navegación
        public ICollection<Input> Inputs { get; set; } 
        public ICollection<Output> Outputs { get; set;}
        public ICollection<Stock> Stocks { get; set;}
    }
}