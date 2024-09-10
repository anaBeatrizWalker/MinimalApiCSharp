using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApiCSharp.Domain.ModelViews
{
    public struct ValidationErrors
    {
          public List<string> Messages {get; set;}
    }
}