using FP_Engine.Engine.Configuration;
using FP_Engine.Engine.Primitives;
using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Extractor
{
    static class LocalHistograms
    {
        public static HistogramCube Create(BlockMap blocks, DoubleMatrix image)
        {
            var histogram = new HistogramCube(blocks.Primary.Blocks, Parameters.HistogramDepth);
            foreach (var block in blocks.Primary.Blocks.Iterate())
            {
                var area = blocks.Primary.Block(block);
                for (int y = area.Top; y < area.Bottom; ++y)
                {
                    for (int x = area.Left; x < area.Right; ++x)
                    {
                        int depth = (int)(image[x, y] * histogram.Bins);
                        histogram.Increment(block, histogram.Constrain(depth));
                    }
                }
            }
            FingerprintTransparency.Current.Log("histogram", histogram);
            return histogram;
        }
        public static HistogramCube Smooth(BlockMap blocks, HistogramCube input)
        {
            var blocksAround = new IntPoint[] { new IntPoint(0, 0), new IntPoint(-1, 0), new IntPoint(0, -1), new IntPoint(-1, -1) };
            var output = new HistogramCube(blocks.Secondary.Blocks, input.Bins);
            foreach (var corner in blocks.Secondary.Blocks.Iterate())
            {
                foreach (var relative in blocksAround)
                {
                    var block = corner + relative;
                    if (blocks.Primary.Blocks.Contains(block))
                    {
                        for (int i = 0; i < input.Bins; ++i)
                            output.Add(corner, i, input[block, i]);
                    }
                }
            }
            FingerprintTransparency.Current.Log("smoothed-histogram", output);
            return output;
        }
    }
}
