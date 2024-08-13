using FP_Engine.Engine.Matcher;

namespace FP_Engine.Engine.Transparency
{
    record ConsistentEdgePair(int ProbeFrom, int ProbeTo, int CandidateFrom, int CandidateTo)
    {
        public ConsistentEdgePair(MinutiaPair pair) : this(pair.ProbeRef, pair.Probe, pair.CandidateRef, pair.Candidate) { }
    }
}
