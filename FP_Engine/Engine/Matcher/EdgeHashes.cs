using System;
using System.Collections.Generic;
using FP_Engine.Engine.Configuration;
using FP_Engine.Engine.Features;
using FP_Engine.Engine.Primitives;
using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Matcher
{
    static class EdgeHashes
    {
        static readonly float ComplementaryMaxAngleError = FloatAngle.Complementary(Parameters.MaxAngleError);
        public static int Hash(EdgeShape edge)
        {
            int lengthBin = edge.Length / Parameters.MaxDistanceError;
            int referenceAngleBin = (int)(edge.ReferenceAngle / Parameters.MaxAngleError);
            int neighborAngleBin = (int)(edge.NeighborAngle / Parameters.MaxAngleError);
            return (referenceAngleBin << 24) + (neighborAngleBin << 16) + lengthBin;
        }
        public static bool Matching(EdgeShape probe, EdgeShape candidate)
        {
            int lengthDelta = probe.Length - candidate.Length;
            if (lengthDelta >= -Parameters.MaxDistanceError && lengthDelta <= Parameters.MaxDistanceError)
            {
                float referenceDelta = FloatAngle.Difference(probe.ReferenceAngle, candidate.ReferenceAngle);
                if (referenceDelta <= Parameters.MaxAngleError || referenceDelta >= ComplementaryMaxAngleError)
                {
                    float neighborDelta = FloatAngle.Difference(probe.NeighborAngle, candidate.NeighborAngle);
                    if (neighborDelta <= Parameters.MaxAngleError || neighborDelta >= ComplementaryMaxAngleError)
                        return true;
                }
            }
            return false;
        }
        static List<int> Coverage(EdgeShape edge)
        {
            int minLengthBin = (edge.Length - Parameters.MaxDistanceError) / Parameters.MaxDistanceError;
            int maxLengthBin = (edge.Length + Parameters.MaxDistanceError) / Parameters.MaxDistanceError;
            int angleBins = (int)Math.Ceiling(2 * Math.PI / Parameters.MaxAngleError);
            int minReferenceBin = (int)(FloatAngle.Difference(edge.ReferenceAngle, Parameters.MaxAngleError) / Parameters.MaxAngleError);
            int maxReferenceBin = (int)(FloatAngle.Add(edge.ReferenceAngle, Parameters.MaxAngleError) / Parameters.MaxAngleError);
            int endReferenceBin = (maxReferenceBin + 1) % angleBins;
            int minNeighborBin = (int)(FloatAngle.Difference(edge.NeighborAngle, Parameters.MaxAngleError) / Parameters.MaxAngleError);
            int maxNeighborBin = (int)(FloatAngle.Add(edge.NeighborAngle, Parameters.MaxAngleError) / Parameters.MaxAngleError);
            int endNeighborBin = (maxNeighborBin + 1) % angleBins;
            var coverage = new List<int>();
            for (int lengthBin = minLengthBin; lengthBin <= maxLengthBin; ++lengthBin)
                for (int referenceBin = minReferenceBin; referenceBin != endReferenceBin; referenceBin = (referenceBin + 1) % angleBins)
                    for (int neighborBin = minNeighborBin; neighborBin != endNeighborBin; neighborBin = (neighborBin + 1) % angleBins)
                        coverage.Add((referenceBin << 24) + (neighborBin << 16) + lengthBin);
            return coverage;
        }
        public static Dictionary<int, List<IndexedEdge>> Build(FingerprintTemplate template)
        {
            var map = new Dictionary<int, List<IndexedEdge>>();
            for (int reference = 0; reference < template.Minutiae.Length; ++reference)
                for (int neighbor = 0; neighbor < template.Minutiae.Length; ++neighbor)
                    if (reference != neighbor)
                    {
                        var edge = new IndexedEdge(template.Minutiae, reference, neighbor);
                        foreach (int hash in Coverage(edge.Shape))
                        {
                            List<IndexedEdge> list;
                            if (!map.TryGetValue(hash, out list))
                                map[hash] = list = new List<IndexedEdge>();
                            list.Add(edge);
                        }
                    }
            FingerprintTransparency.Current.LogEdgeHash(map);
            return map;
        }
    }
}
