using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApiCSharp.Domain.DTOs
{
    public record VehicleDTO
    {
              
        public string Name {get;set;} = default!;

        public string Brand { get; set; } = default!;

        public int Year { get; set; } = default!;
    }
}