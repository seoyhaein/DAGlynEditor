using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAGlynEditor
{
    public interface ILocatable
    {
        Point Location { get; }
    }
}
