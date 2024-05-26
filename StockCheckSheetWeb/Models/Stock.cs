using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockCheckSheetWeb.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date is required.")]
        [DisplayName("Date")]
        public DateTime Date { get; set; }


        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public int Amount { get; set; }


        [Required(ErrorMessage = "Unit Cost is required.")]
        [DisplayName("Unit Cost")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Cost must be greater than 0.")]
        public double UnitCost { get; set; }


        [Required(ErrorMessage = "Total Cost is required.")]
        [DisplayName("Total Cost")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Cost must be greater than 0.")]
        public double TotalCost { get; set; }



        // Navigation properties
        //public StockCheckSheet StockCheckSheet { get; set; }
    }
}
