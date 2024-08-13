using System;

namespace FP_Engine.Engine.Features
{
    static class SkeletonTypes
    {
        public static string Prefix(this SkeletonType type)
        {
            return type switch
            {
                SkeletonType.Ridges => "ridges-",
                SkeletonType.Valleys => "valleys-",
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }
    }
}
