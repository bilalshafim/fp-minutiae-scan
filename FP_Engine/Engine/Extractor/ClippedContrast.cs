using FP_Engine.Engine.Configuration;
using FP_Engine.Engine.Primitives;
using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Extractor
{
    static class ClippedContrast
    {
        public static DoubleMatrix Compute(BlockMap blocks, HistogramCube histogram)
        {
            var result = new DoubleMatrix(blocks.Primary.Blocks);
            foreach (var block in blocks.Primary.Blocks.Iterate())
            {
                int volume = histogram.Sum(block);
                int clipLimit = Doubles.RoundToInt(volume * Parameters.ClippedContrast);
                int accumulator = 0;
                int lowerBound = histogram.Bins - 1;
                for (int i = 0; i < histogram.Bins; ++i)
                {
                    accumulator += histogram[block, i];
                    if (accumulator > clipLimit)
                    {
                        lowerBound = i;
                        break;
                    }
                }
                accumulator = 0;
                int upperBound = 0;
                for (int i = histogram.Bins - 1; i >= 0; --i)
                {
                    accumulator += histogram[block, i];
                    if (accumulator > clipLimit)
                    {
                        upperBound = i;
                        break;
                    }
                }
                result[block] = (upperBound - lowerBound) * (1.0 / (histogram.Bins - 1));
            }
            FingerprintTransparency.Current.Log("contrast", result);
            return result;
        }
    }
}
