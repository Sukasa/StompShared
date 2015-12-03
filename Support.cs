using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using STOMP.Frames;

namespace STOMP.Shared
{
    static public class Support
    {
        static public IEnumerable<Type> GetBaseFrameTypes()
        {
            return typeof(StompFrame).Assembly.GetTypes().Where(x => typeof(StompFrame).IsAssignableFrom(x) && !x.IsAbstract);
        }
    }
}
