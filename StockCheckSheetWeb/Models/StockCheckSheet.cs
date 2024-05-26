using System.ComponentModel.DataAnnotations;

namespace StockCheckSheetWeb.Models
{
    public class StockCheckSheet
    {
        [Key]
        public int Id { get; set; }
  
        // Navigation properties

        //public ICollection<Inputs> Inputs { get; set; } 
        //public ICollection<Outputs> Outputs { get; set;}
        //public ICollection<Stocks> Stocks { get; set;}
    }
}
