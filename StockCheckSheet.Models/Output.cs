﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using StockCheckSheet.DataAccess.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockCheckSheetWeb.Models
{
    public class Output
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DisplayName("Date")]
        public DateTime Date { get; set; }


        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public int Amount { get; set; }


        [Required(ErrorMessage = "Unit Cost is required.")]
        [DisplayName("Unit Cost ($)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Cost must be greater than 0.")]
        public double UnitCost { get; set; }


        [Required(ErrorMessage = "Total Cost is required.")]
        [DisplayName("Total Cost ($)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total Cost must be greater than 0.")]
        public double TotalCost { get; set; }

        // Foreign key
        [ForeignKey("ApplicationUserId")]
        public string ApplicationUserId { get; set; }

        // Navigation properties
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
