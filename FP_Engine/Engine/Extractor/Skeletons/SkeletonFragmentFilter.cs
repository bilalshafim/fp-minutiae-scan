
using FP_Engine.Engine.Configuration;
using FP_Engine.Engine.Features;
using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Extractor.Skeletons
{
    static class SkeletonFragmentFilter
    {
        public static void Apply(Skeleton skeleton)
        {
            foreach (var minutia in skeleton.Minutiae)
                if (minutia.Ridges.Count == 1)
                {
                    var ridge = minutia.Ridges[0];
                    if (ridge.End.Ridges.Count == 1 && ridge.Points.Count < Parameters.MinFragmentLength)
                        ridge.Detach();
                }
            SkeletonDotFilter.Apply(skeleton);
            FingerprintTransparency.Current.LogSkeleton("removed-fragments", skeleton);
        }
    }
}
