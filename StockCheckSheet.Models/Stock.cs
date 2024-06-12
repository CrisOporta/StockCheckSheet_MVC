using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockCheckSheetWeb.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }


        [ValidateNever]
        [Required(ErrorMessage = "Date is required.")]
        [DisplayName("Date")]
        public DateTime Date { get; set; }

        [ValidateNever]
        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public int Amount { get; set; }

        [ValidateNever]
        [Required(ErrorMessage = "Unit Cost is required.")]
        [DisplayName("Unit Cost")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Cost must be greater than 0.")]
        public double UnitCost { get; set; }

        [ValidateNever]
        [Required(ErrorMessage = "Total Cost is required.")]
        [DisplayName("Total Cost")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Cost must be greater than 0.")]
        public double TotalCost { get; set; }

        // Foreign key
        [ForeignKey("ReportId")]
        public int ReportId { get; set; }

        // Navigation properties
        [ValidateNever]
        public Report? Report { get; set; }
    }
}
