using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mss.Common
{
    public class Ref<T>
    {
        public T Value { get; set; }
        public Ref(T value) => Value = value;
    }
}
