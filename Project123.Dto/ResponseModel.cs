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

        public string access_token { get; set; }
        public string user_roles { get; set; }
        public string user_id { get; set; }

        public ResponseModel()
        {
            Status = "E";
            //Message = "An error occurred. Please try again later.";
            Message = "";
        }
    }
}