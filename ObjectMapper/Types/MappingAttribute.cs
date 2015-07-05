using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectMapper.Types
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MappingAttribute : Attribute
    {
        public List<string> Matches;

        public MappingAttribute(params string[] matches)
        {
            Matches = new List<string>();
            foreach(string match in matches)
            {
                Matches.Add(match);
            }
        }
    }
}
