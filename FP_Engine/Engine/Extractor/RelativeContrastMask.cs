using System;
using System.Collections.Generic;
using FP_Engine.Engine.Configuration;
using FP_Engine.Engine.Primitives;
using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Extractor
{
    static class RelativeContrastMask
    {
        public static BooleanMatrix Compute(DoubleMatrix contrast, BlockMap blocks)
        {
            var sortedContrast = new List<double>();
            foreach (var block in contrast.Size.Iterate())
                sortedContrast.Add(contrast[block]);
            sortedContrast.Sort();
            sortedContrast.Reverse();
            int pixelsPerBlock = blocks.Pixels.Area / blocks.Primary.Blocks.Area;
            int sampleCount = Math.Min(sortedContrast.Count, Parameters.RelativeContrastSample / pixelsPerBlock);
            int consideredBlocks = Math.Max(Doubles.RoundToInt(sampleCount * Parameters.RelativeContrastPercentile), 1);
            double averageContrast = 0;
            for (int i = 0; i < consideredBlocks; ++i)
                averageContrast += sortedContrast[i];
            averageContrast /= consideredBlocks;
            var limit = averageContrast * Parameters.MinRelativeContrast;
            var result = new BooleanMatrix(blocks.Primary.Blocks);
            foreach (var block in blocks.Primary.Blocks.Iterate())
                if (contrast[block] < limit)
                    result[block] = true;
            FingerprintTransparency.Current.Log("relative-contrast-mask", result);
            return result;
        }
    }
}
