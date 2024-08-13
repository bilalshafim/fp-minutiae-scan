using System.Collections.Generic;
using FP_Engine.Engine.Features;

namespace FP_Engine.Engine.Extractor.Minutiae
{
    static class MinutiaCollector
    {
        static void Collect(List<Minutia> minutiae, Skeleton skeleton, MinutiaType type)
        {
            foreach (var sminutia in skeleton.Minutiae)
                if (sminutia.Ridges.Count == 1)
                    minutiae.Add(new(sminutia.Position.ToShort(), sminutia.Ridges[0].Direction(), type));
        }
        public static List<Minutia> Collect(Skeleton ridges, Skeleton valleys)
        {
            var minutiae = new List<Minutia>();
            Collect(minutiae, ridges, MinutiaType.Ending);
            Collect(minutiae, valleys, MinutiaType.Bifurcation);
            return minutiae;
        }
    }
}
