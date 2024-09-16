using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApiCSharp.Domain.ModelViews
{
    public struct Home
    {
        public string Message {get => "Bem-vindo a API de Veículos com Minimal Api .NET";}
        public string Documentation {get => "/swagger";}
    }
}