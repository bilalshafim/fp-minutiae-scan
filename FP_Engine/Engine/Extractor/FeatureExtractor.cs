using FP_Engine.Engine.Configuration;
using FP_Engine.Engine.Extractor.Minutiae;
using FP_Engine.Engine.Extractor.Skeletons;
using FP_Engine.Engine.Features;
using FP_Engine.Engine.Primitives;
using FP_Engine.Engine.Templates;
using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Extractor
{
    static class FeatureExtractor
    {
        public static FeatureTemplate Extract(DoubleMatrix raw, double dpi)
        {
            FingerprintTransparency.Current.Log("decoded-image", raw);
            raw = ImageResizer.Resize(raw, dpi);
            FingerprintTransparency.Current.Log("scaled-image", raw);
            var blocks = new BlockMap(raw.Width, raw.Height, Parameters.BlockSize);
            FingerprintTransparency.Current.Log("blocks", blocks);
            var histogram = LocalHistograms.Create(blocks, raw);
            var smoothHistogram = LocalHistograms.Smooth(blocks, histogram);
            var mask = SegmentationMask.Compute(blocks, histogram);
            var equalized = ImageEqualization.Equalize(blocks, raw, smoothHistogram, mask);
            var orientation = BlockOrientations.Compute(equalized, mask, blocks);
            var smoothed = OrientedSmoothing.Parallel(equalized, orientation, mask, blocks);
            var orthogonal = OrientedSmoothing.Orthogonal(smoothed, orientation, mask, blocks);
            var binary = BinarizedImage.Binarize(smoothed, orthogonal, mask, blocks);
            var pixelMask = SegmentationMask.Pixelwise(mask, blocks);
            BinarizedImage.Cleanup(binary, pixelMask);
            FingerprintTransparency.Current.Log("pixel-mask", pixelMask);
            var inverted = BinarizedImage.Invert(binary, pixelMask);
            var innerMask = SegmentationMask.Inner(pixelMask);
            var ridges = SkeletonGraphs.Create(binary, SkeletonType.Ridges);
            var valleys = SkeletonGraphs.Create(inverted, SkeletonType.Valleys);
            var template = new FeatureTemplate(raw.Size.ToShort(), MinutiaCollector.Collect(ridges, valleys));
            FingerprintTransparency.Current.Log("skeleton-minutiae", template);
            InnerMinutiaeFilter.Apply(template.Minutiae, innerMask);
            FingerprintTransparency.Current.Log("inner-minutiae", template);
            MinutiaCloudFilter.Apply(template.Minutiae);
            FingerprintTransparency.Current.Log("removed-minutia-clouds", template);
            template = new(template.Size, TopMinutiaeFilter.Apply(template.Minutiae));
            FingerprintTransparency.Current.Log("top-minutiae", template);
            return template;
        }
    }
}
