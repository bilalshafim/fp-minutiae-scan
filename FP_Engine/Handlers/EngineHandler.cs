using System;
using FP_Engine.HandlerModels;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using FP_Engine.Interfaces;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using FP_Engine.EngineInterface;

namespace FP_Engine.Handlers
{
    public class EngineHandler : IEngineHandler
    {
        private readonly ILogger<EngineHandler> _logger;
        private readonly CacheHandler _cache;
        public EngineHandler(ILogger<EngineHandler> logger, CacheHandler cache)
        {
            _logger = logger;
            _cache = cache;

        }

        ~EngineHandler() { }

        public AuthenticationResult Authenticate(FingerprintTemplate probe, FingerprintTemplate candidate, double threshold)
        {
            Stopwatch stopWatch = new();
            AuthenticationResult result = new();
            FingerprintMatcher matcher = new(probe);
            stopWatch.Start();
            result.SimilarityScore = matcher.Match(candidate);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            //return max >= threshold ? match : null;
            result.IsMatch = result.SimilarityScore >= threshold;
            result.info = $"Processing time: {ts.TotalMilliseconds} ms.";
            return result;
        }


        //TODO: Implement 1:N matching
        public IdentificationResult Identify(FingerprintTemplate probe, double threshold, ITemplateHandler templateHandler)
        {
            IdentificationResult idresult = new();

            // TODO: implement testing env with fp files in projec dir. Generate their templates and save them as byte arrays and cache.
            // Check if can use byte array cache or have to store actual FingerprintTemplates

            // get test templates from local dir or DB
            Stopwatch stopWatch = new();

            // Retrieve candidates
            //List<CandidateBin> byteArrays = _cache.GetCandidatesFromMemory();
            //stopWatch.Start();
            ////ConcurrentBag<byte[]> byteArrays = (_cache.byteArrayTemplates.Count != 0) ? _cache.byteArrayTemplates : throw new Exception();
            //List<Candidate> candidates = new();

            //Parallel.ForEach(byteArrays, (array, state, index) =>
            //{
            //    Candidate cand = new Candidate()
            //    {
            //        template = new FingerprintTemplate(array.template),
            //        candidate = 0,
            //        score = 0
            //    };
            //    candidates.Add(cand);
            //});
            //stopWatch.Stop();
            //TimeSpan gcfb = stopWatch.Elapsed;


            // Checking work for just having an instance of concurrent dict storing all byte arrays at startup and then this code to retrieve it.
            // See FastCache: https://www.jitbit.com/alexblog/fast-memory-cache/
            //List<byte[]> candidates = _cache.byteArrayTemplates;
            //var fileList = Directory.GetFiles("C:/Users/b.shafi/source/repos/SourceAFIS-API/FP_Engine/DB1_B2");
            //List<string> list = new List<string>(fileList); // make this list concurrent as well. or not. see if have to.
            //FingerprintImageOptions options = new() { Dpi = 500.0 };

            //ConcurrentDictionary<long, byte[]> ex = new();

            //Parallel.ForEach(fileList, (file, state, index) =>
            //{
            //    Candidate cand = new Candidate()
            //    {
            //        template = new FingerprintTemplate(candidates[index]),
            //        candidate = index,
            //        score = 0
            //    };
            //});

            //// Init
            //FingerprintMatcher matcher = new(probe);

            //Candidate theChosenOne = new() { score = 0 };

            //// Match
            //// score > threshold && 
            //stopWatch = new();
            //stopWatch.Start();
            //ConcurrentBag<double> scores = new();
            //Parallel.ForEach(candidates, candidate =>
            //{
            //    double score = matcher.Match(candidate.template);
            //    if (score > theChosenOne.score)
            //    {
            //        theChosenOne.candidate = candidate.candidate;
            //        theChosenOne.score = score;
            //    }
            //});

            //stopWatch.Stop();
            //TimeSpan mc = stopWatch.Elapsed;

            //idresult.candidateId = theChosenOne.candidate.ToString();
            //idresult.score = theChosenOne.score.ToString();
            //var arr = scores.ToArray();
            //idresult.info = $"identification: {((int)mc.TotalMilliseconds)} ms. templates from cached byte arrays: {(int)gcfb.TotalMilliseconds} ms.[{String.Join(", ", arr)}]";


            return idresult;
        }

    }

}
