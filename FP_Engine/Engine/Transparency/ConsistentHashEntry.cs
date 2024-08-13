using System.Collections.Generic;
using FP_Engine.Engine.Features;

namespace FP_Engine.Engine.Transparency
{
    record ConsistentHashEntry(int Key, List<IndexedEdge> Edges)
    {
    }
}
