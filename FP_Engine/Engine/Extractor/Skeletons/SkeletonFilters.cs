
using FP_Engine.Engine.Features;
using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Extractor.Skeletons
{
    static class SkeletonFilters
    {
        public static void Apply(Skeleton skeleton)
        {
            SkeletonDotFilter.Apply(skeleton);
            FingerprintTransparency.Current.LogSkeleton("removed-dots", skeleton);
            SkeletonPoreFilter.Apply(skeleton);
            SkeletonGapFilter.Apply(skeleton);
            SkeletonTailFilter.Apply(skeleton);
            SkeletonFragmentFilter.Apply(skeleton);
        }
    }
}
