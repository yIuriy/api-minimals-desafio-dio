using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.ModelViews
{
    public struct Home
    {
        public string Msg { get => "Welcome do Vehicle API - Minimal Api"; }
        public string Documentation { get => "/swagger"; }
    }
}