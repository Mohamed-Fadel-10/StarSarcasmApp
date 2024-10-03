using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSarcasm.Application.Response
{
    public class ResponseModel
    {
        public bool IsSuccess {  get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public object Model { get; set; }
    }
}
