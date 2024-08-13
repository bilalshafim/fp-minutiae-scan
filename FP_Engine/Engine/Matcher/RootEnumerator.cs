using System.Collections.Generic;
using FP_Engine.Engine.Configuration;
using FP_Engine.Engine.Features;
using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Matcher
{
    static class RootEnumerator
    {
        public static void Enumerate(FingerprintMatcher probe, FingerprintTemplate candidate, RootList roots)
        {
            var cminutiae = candidate.Minutiae;
            int lookups = 0;
            int tried = 0;
            foreach (bool shortEdges in new bool[] { false, true })
            {
                for (int period = 1; period < cminutiae.Length; ++period)
                {
                    for (int phase = 0; phase <= period; ++phase)
                    {
                        for (int creference = phase; creference < cminutiae.Length; creference += period + 1)
                        {
                            int cneighbor = (creference + period) % cminutiae.Length;
                            var cedge = new EdgeShape(cminutiae[creference], cminutiae[cneighbor]);
                            if (cedge.Length >= Parameters.MinRootEdgeLength ^ shortEdges)
                            {
                                List<IndexedEdge> matches;
                                if (probe.Hash.TryGetValue(EdgeHashes.Hash(cedge), out matches))
                                {
                                    foreach (var match in matches)
                                    {
                                        if (EdgeHashes.Matching(match.Shape, cedge))
                                        {
                                            int duplicateKey = match.Reference << 16 | creference;
                                            if (roots.Duplicates.Add(duplicateKey))
                                            {
                                                var pair = roots.Pool.Allocate();
                                                pair.Probe = match.Reference;
                                                pair.Candidate = creference;
                                                roots.Add(pair);
                                            }
                                            ++tried;
                                            if (tried >= Parameters.MaxTriedRoots)
                                                return;
                                        }
                                    }
                                }
                                ++lookups;
                                if (lookups >= Parameters.MaxRootEdgeLookups)
                                    return;
                            }
                        }
                    }
                }
            }
        }
    }
}
