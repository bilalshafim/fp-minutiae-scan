using System.Collections.Generic;
using System.Linq;
using FP_Engine.Engine.Features;
using FP_Engine.Engine.Primitives;

namespace FP_Engine.Engine.Transparency
{
    record ConsistentSkeleton(int Width, int Height, List<IntPoint> Minutiae, List<ConsistentSkeletonRidge> Ridges)
    {
        public static ConsistentSkeleton Of(Skeleton skeleton)
        {
            var offsets = new Dictionary<SkeletonMinutia, int>();
            for (int i = 0; i < skeleton.Minutiae.Count; ++i)
                offsets[skeleton.Minutiae[i]] = i;
            return new(
                skeleton.Size.X,
                skeleton.Size.Y,
                (from m in skeleton.Minutiae select m.Position).ToList(),
                (from m in skeleton.Minutiae
                 from r in m.Ridges
                 where r.Points is CircularList<IntPoint>
                 select new ConsistentSkeletonRidge(offsets[r.Start], offsets[r.End], r.Points)).ToList());
        }
    }
}
