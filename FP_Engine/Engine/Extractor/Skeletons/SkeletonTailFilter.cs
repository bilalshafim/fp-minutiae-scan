
using FP_Engine.Engine.Configuration;
using FP_Engine.Engine.Features;
using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Extractor.Skeletons
{
    static class SkeletonTailFilter
    {
        public static void Apply(Skeleton skeleton)
        {
            foreach (var minutia in skeleton.Minutiae)
            {
                if (minutia.Ridges.Count == 1 && minutia.Ridges[0].End.Ridges.Count >= 3)
                    if (minutia.Ridges[0].Points.Count < Parameters.MinTailLength)
                        minutia.Ridges[0].Detach();
            }
            SkeletonDotFilter.Apply(skeleton);
            SkeletonKnotFilter.Apply(skeleton);
            FingerprintTransparency.Current.LogSkeleton("removed-tails", skeleton);
        }
    }
}
