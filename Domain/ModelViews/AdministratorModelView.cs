using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApiCSharp.Domain.ModelViews
{
    public record AdministratorModelView
    {
        public int Id {get;set;} = default!;

        public string Email {get;set;} = default!;
              
        public string Profile { get; set; } = default!;
    }
}