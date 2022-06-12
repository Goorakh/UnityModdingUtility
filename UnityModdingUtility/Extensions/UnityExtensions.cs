using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityModdingUtility.Extensions
{
    public static class Unity
    {
        public static bool Exists(this UnityEngine.Object obj)
        {
            return obj && obj != null;
        }
    }
}
