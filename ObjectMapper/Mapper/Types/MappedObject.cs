using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectMapper.Types
{
    public class MappedObject<T> where T : new()
    {
        public T RegularMapping { get; set; }
        public dynamic Extras { get; set; }
    }
}
