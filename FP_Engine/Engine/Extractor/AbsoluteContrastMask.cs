using FP_Engine.Engine.Configuration;
using FP_Engine.Engine.Primitives;
using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Extractor
{
    static class AbsoluteContrastMask
    {
        public static BooleanMatrix Compute(DoubleMatrix contrast)
        {
            var result = new BooleanMatrix(contrast.Size);
            foreach (var block in contrast.Size.Iterate())
                if (contrast[block] < Parameters.MinAbsoluteContrast)
                    result[block] = true;
            FingerprintTransparency.Current.Log("absolute-contrast-mask", result);
            return result;
        }
    }
}
