using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.ModelViews
{
    public struct ValidationErrors
    {
        public List<string> Messages { get; set; }
    }
}