using System;
using System.Collections.Generic;
using System.Text;

namespace Project123.Dto
{
    public class ResponseModel
    {
        public string Status { get; set; } 
        public string Message { get; set; }
   
        public bool Success { get { return Status == "S"; } }
        public ResponseModel()
        {
            Status = "E";
            //Message = "An error occurred. Please try again later.";
            Message = "";
        }
    }
}