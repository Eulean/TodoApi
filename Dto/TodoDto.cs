using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Dto
{
    public class TodoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Content { get; set; }
        public bool IsComplete { get; set; }
        public bool IsDeleted { get; set; }
    }
}