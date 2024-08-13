
using System.Collections.Generic;
using FP_Engine.Engine.Features;

namespace FP_Engine.Engine.Extractor.Skeletons
{
    static class SkeletonDotFilter
    {
        public static void Apply(Skeleton skeleton)
        {
            var removed = new List<SkeletonMinutia>();
            foreach (var minutia in skeleton.Minutiae)
                if (minutia.Ridges.Count == 0)
                    removed.Add(minutia);
            foreach (var minutia in removed)
                skeleton.RemoveMinutia(minutia);
        }
    }
}
