using System.Collections.Generic;
using FP_Engine.Engine.Features;
using FP_Engine.Engine.Primitives;

namespace FP_Engine.Engine.Templates
{
    record FeatureTemplate(ShortPoint Size, List<Minutia> Minutiae)
    {
    }
}
