using System;
using System.Linq;
using FP_Engine.Engine.Extractor;
using FP_Engine.Engine.Features;
using FP_Engine.Engine.Primitives;
using FP_Engine.Engine.Templates;

namespace FP_Engine.EngineInterface
{
    /// <summary>Biometric description of a fingerprint suitable for efficient matching.</summary>
    /// <remarks>
    /// <para>
    /// Fingerprint template holds high-level fingerprint features, specifically ridge endings and bifurcations (together called minutiae).
    /// Original image is not preserved in the fingerprint template and there is no way to reconstruct the original fingerprint from its template.
    /// </para>
    /// <para>
    /// <see cref="FingerprintImage" /> can be converted to template by calling <see cref="FingerprintTemplate(FingerprintImage)" /> constructor.
    /// </para>
    /// <para>
    /// Since image processing is expensive, applications should cache serialized templates.
    /// Serialization into CBOR format is performed by <see cref="ToByteArray()" /> method.
    /// CBOR template can be deserialized by calling <see cref="FingerprintTemplate(byte[])" /> constructor.
    /// </para>
    /// <para>
    /// Matching is performed by constructing <see cref="FingerprintMatcher" />,
    /// passing probe fingerprint to its <see cref="FingerprintMatcher(FingerprintTemplate)" /> constructor,
    /// and then passing candidate fingerprints to its <see cref="FingerprintMatcher.Match(FingerprintTemplate)" /> method.
    /// </para>
    /// <para>
    /// <c>FingerprintTemplate</c> contains two kinds of data: fingerprint features and search data structures.
    /// Search data structures speed up matching at the cost of some RAM.
    /// Only fingerprint features are serialized. Search data structures are recomputed after every deserialization.
    /// </para>
    /// </remarks>
    /// <seealso cref="FingerprintImage" />
    /// <seealso cref="FingerprintMatcher" />
    public class FingerprintTemplate
    {
        const int Prime = 1610612741;

        internal readonly ShortPoint Size;
        internal readonly Minutia[] Minutiae;
        internal readonly NeighborEdge[][] Edges;

        /// <summary>Gets the empty fallback template with no biometric data.</summary>
        /// <value>Empty template.</value>
        /// <remarks>
        /// Empty template is useful as a fallback to simplify code.
        /// It contains no biometric data and it does not match any other template including itself.
        /// There is only one global instance. This property does not instantiate any new objects.
        /// </remarks>
        public static readonly FingerprintTemplate Empty = new FingerprintTemplate();

        FingerprintTemplate()
        {
            Size = new(1, 1);
            Minutiae = new Minutia[0];
            Edges = new NeighborEdge[0][];
        }

        FingerprintTemplate(FeatureTemplate mutable)
        {
            Size = mutable.Size;
            var minutiae =
                from m in mutable.Minutiae
                orderby (m.Position.X * Prime + m.Position.Y) * Prime, m.Position.X, m.Position.Y, m.Direction, m.Type
                select m;
            Minutiae = minutiae.ToArray();
            FingerprintTransparency.Current.Log("shuffled-minutiae", () => ToFeatureTemplate());
            Edges = NeighborEdge.BuildTable(Minutiae);
        }

        /// <summary>Creates fingerprint template from fingerprint image.</summary>
        /// <remarks>
        /// This constructor runs an expensive feature extractor algorithm,
        /// which analyzes the image and collects identifiable biometric features from it.
        /// </remarks>
        /// <param name="image">Fingerprint image to process.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="image" /> is <c>null</c>.</exception>
        public FingerprintTemplate(FingerprintImage image) : this(FeatureExtractor.Extract(image.Matrix, image.Dpi)) { }

        static FeatureTemplate Deserialize(byte[] serialized)
        {
            var persistent = SerializationUtils.Deserialize<PersistentTemplate>(serialized);
            persistent.Validate();
            return persistent.Decode();
        }

        /// <summary>Deserializes fingerprint template from byte array.</summary>
        /// <remarks>
        /// <para>
        /// This constructor reads <see href="https://cbor.io/">CBOR</see>-encoded
        /// template produced by <see cref="ToByteArray()" />
        /// and reconstructs an exact copy of the original fingerprint template.
        /// </para>
        /// <para>
        /// Templates produced by previous versions of FP_Engine may fail to deserialize correctly.
        /// Applications should re-extract all templates from original images when upgrading FP_Engine.
        /// </para>
        /// </remarks>
        /// <param name="serialized">Serialized fingerprint template in <see href="https://cbor.io/">CBOR</see> format
        /// produced by <see cref="ToByteArray()" />.</param>
        /// <exception cref="NullReferenceException">Thrown when <paramref name="serialized" /> is <c>null</c>.</exception>
        /// <exception cref="Exception">Thrown when <paramref name="serialized" /> is not in the correct format or it is corrupted.</exception>
        public FingerprintTemplate(byte[] serialized) : this(Deserialize(serialized)) { }

        FeatureTemplate ToFeatureTemplate() => new FeatureTemplate(Size, Minutiae.ToList());

        /// <summary>Serializes fingerprint template into byte array.</summary>
        /// <remarks>
        /// <para>
        /// Serialized template can be stored in a database or sent over network.
        /// It can be then deserialized by calling <see cref="FingerprintTemplate(byte[])" /> constructor.
        /// Persisting templates alongside fingerprint images allows applications to start faster,
        /// because template deserialization is more than 100x faster than re-extraction from fingerprint image.
        /// </para>
        /// <para>
        /// Serialized template excludes search structures that <c>FingerprintTemplate</c> keeps to speed up matching.
        /// Serialized template is therefore much smaller than in-memory <c>FingerprintTemplate</c>.
        /// </para>
        /// <para>
        /// Serialization format can change with every FP_Engine version. There is no backward compatibility of templates.
        /// Applications should preserve raw fingerprint images, so that templates can be re-extracted after FP_Engine upgrade.
        /// Template format for current version of FP_Engine is
        /// </para>
        /// </remarks>
        /// <returns>Serialized fingerprint template in <see href="https://cbor.io/">CBOR</see> format.</returns>
        /// <seealso cref="FingerprintTemplate(byte[])" />
        public byte[] ToByteArray() => SerializationUtils.Serialize(new PersistentTemplate(ToFeatureTemplate()));

        /// <summary>
        /// Estimates memory consumed by this template.
        /// </summary>
        /// <returns>Memory footprint estimate in bytes.</returns>
        /// <remarks>
        /// .NET does not have any convenient mechanism to measure RAM footprint of objects.
        /// This method calculates best effort estimate based on documentation of CLR memory layout.
        /// It will return estimate appropriate for bitness of the current process.
        /// </remarks>
        public int Memory()
        {
            // ShortPoint will consume 8 bytes in 64-bit runtime due to alignment.
            int self = 5 * IntPtr.Size;
            int minutiae = 3 * IntPtr.Size + Minutiae.Length * Minutia.Memory;
            // First level of edge array.
            int edges1 = 3 * IntPtr.Size + Edges.Length * IntPtr.Size;
            // Second level of edge array.
            int edges2 = Edges.Sum(star => 3 * IntPtr.Size + star.Length * NeighborEdge.Memory);
            return self + minutiae + edges1 + edges2;
        }
    }
}
