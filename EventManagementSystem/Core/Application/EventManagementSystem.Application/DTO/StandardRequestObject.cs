using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Application.DTO
{
    public class StandardRequestObject<T> where T : class
    {
        public T? Data { get; set; }

        public StandardRequestObject() { }

        public StandardRequestObject(T? data) { Data = data; }
    }
}
