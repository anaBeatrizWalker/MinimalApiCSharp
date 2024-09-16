using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApiCSharp.Domain.Enums;

namespace MinimalApiCSharp.Domain.DTOs
{
    public class AdministratorDTO
    {
         public string Email {get;set;} = default!;

        public string Password { get; set; } = default!;

        public EProfile? Profile { get; set; } = default!;
    }
}