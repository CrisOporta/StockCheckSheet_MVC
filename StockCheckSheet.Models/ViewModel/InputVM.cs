using Microsoft.AspNetCore.Mvc.Rendering;
using StockCheckSheetWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockCheckSheet.Models.ViewModel
{
    public class InputVM
    {
        public Input Input { get; set; }
        public List<Output> OutputList { get; set; }
    }
}
