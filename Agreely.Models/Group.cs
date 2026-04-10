using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agreely.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public required string Name { get; set; } 
        public string? Description { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public int CreatedByUserId { get; set; }
    }
}
