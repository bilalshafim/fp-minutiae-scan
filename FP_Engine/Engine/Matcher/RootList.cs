using System.Collections.Generic;
using FP_Engine.Engine.Configuration;

namespace FP_Engine.Engine.Matcher
{
    class RootList
    {
        public readonly MinutiaPairPool Pool;
        public int Count;
        public readonly MinutiaPair[] Pairs = new MinutiaPair[Parameters.MaxTriedRoots];
        public readonly HashSet<int> Duplicates = new HashSet<int>();
        public RootList(MinutiaPairPool pool) => Pool = pool;
        public void Add(MinutiaPair pair)
        {
            Pairs[Count] = pair;
            ++Count;
        }
        public void Discard()
        {
            for (int i = 0; i < Count; ++i)
            {
                Pool.Release(Pairs[i]);
                Pairs[i] = null;
            }
            Count = 0;
            Duplicates.Clear();
        }
    }
}
