using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectMapper.Types
{
    public class MappingPropertyNames
    {
        public string SourceName { get; set; }
        public List<string> CustomMappingNames { get; set; }

        public MappingPropertyNames()
        {
            CustomMappingNames = new List<string>();
        }
    }
}
