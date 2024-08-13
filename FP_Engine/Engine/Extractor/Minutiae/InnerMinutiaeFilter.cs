using System.Collections.Generic;
using FP_Engine.Engine.Configuration;
using FP_Engine.Engine.Features;
using FP_Engine.Engine.Primitives;

namespace FP_Engine.Engine.Extractor.Minutiae
{
    static class InnerMinutiaeFilter
    {
        public static void Apply(List<Minutia> minutiae, BooleanMatrix mask)
        {
            minutiae.RemoveAll(minutia =>
            {
                var arrow = (-Parameters.MaskDisplacement * DoubleAngle.ToVector(minutia.Direction)).Round();
                return !mask.Get(minutia.Position.ToInt() + arrow, false);
            });
        }
    }
}
