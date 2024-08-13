using System.Collections.Generic;
using System.Linq;
using FP_Engine.Engine.Configuration;
using FP_Engine.Engine.Features;
using FP_Engine.Engine.Primitives;

namespace FP_Engine.Engine.Extractor.Minutiae
{
    static class MinutiaCloudFilter
    {
        public static void Apply(List<Minutia> minutiae)
        {
            var radiusSq = Integers.Sq(Parameters.MinutiaCloudRadius);
            var kept = minutiae.Where(m => Parameters.MaxCloudSize >= minutiae.Where(n => (n.Position - m.Position).LengthSq <= radiusSq).Count() - 1).ToList();
            minutiae.Clear();
            minutiae.AddRange(kept);
        }
    }
}
