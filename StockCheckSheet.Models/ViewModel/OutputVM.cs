using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockCheckSheetWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCheckSheet.Models.ViewModel
{
    public class OutputVM
    {
        public Output Output { get; set; }
        [ValidateNever]
        public List<Input> InputList { get; set; }
    }
}
