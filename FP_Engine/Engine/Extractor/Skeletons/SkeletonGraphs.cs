
using FP_Engine.Engine.Features;
using FP_Engine.Engine.Primitives;
using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Extractor.Skeletons
{
    static class SkeletonGraphs
    {
        public static Skeleton Create(BooleanMatrix binary, SkeletonType type)
        {
            FingerprintTransparency.Current.Log(type.Prefix() + "binarized-skeleton", binary);
            var thinned = BinaryThinning.Thin(binary, type);
            var skeleton = SkeletonTracing.Trace(thinned, type);
            SkeletonFilters.Apply(skeleton);
            return skeleton;
        }
    }
}
