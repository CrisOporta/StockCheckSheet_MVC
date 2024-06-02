using System.ComponentModel.DataAnnotations;

namespace StockCheckSheetWeb.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }
  
        // Navigation properties

        public ICollection<Input>? Inputs { get; set; } 
        public ICollection<Output>? Outputs { get; set;}
        public ICollection<Stock>? Stocks { get; set;}
    }
}
