using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StockCheckSheet.DataAccess.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public int Name { get; set; }
    }
}