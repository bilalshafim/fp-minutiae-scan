using FP_Engine.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using FP_Engine.HandlerModels;
using Microsoft.Extensions.Logging;
using FP_Engine.EngineInterface;

namespace FP_Engine.Handlers
{
    public class CacheHandler
    {
        private readonly ILogger<CacheHandler> _logger;
        private readonly IMemoryCache _cache;
        private readonly ITemplateHandler _templateHandler;
        public List<byte[]> byteArrayTemplates;

        public CacheHandler(IMemoryCache cache, ITemplateHandler templateHandler, ILogger<CacheHandler> logger)
        {
            _logger = logger;
            _cache = cache;
            _templateHandler = templateHandler;
            LoadIntoMemory();
            //LoadCandidatesIntoMemory(_templateHandler);
        }

        public void LoadCandidatesIntoMemory(ITemplateHandler templateHandler)
        {
            Stopwatch stopWatch = new();
            stopWatch.Start();
            //ConcurrentDictionary<long, byte[]> templates = templateHandler.GenerateTemplatesAsByteArrayForMemory();
            stopWatch.Stop();
            TimeSpan gt = stopWatch.Elapsed;
            Console.WriteLine($"Processing time for templates: {(int)gt.TotalMilliseconds} ms");

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
            stopWatch = new();
            stopWatch.Start();

            var fileList = Directory.GetFiles("C:/Users/b.shafi/source/repos/SourceAFIS-API/FP_Engine/DB1_B2");
            List<string> list = new List<string>(fileList); // make this list concurrent as well. or not. see if have to.
            FingerprintImageOptions options = new() { Dpi = 500.0 };

            ConcurrentDictionary<long, byte[]> ex = new();
            
            Parallel.ForEach(fileList, (file, state, index) =>
            {
                CandidateBin cand = new CandidateBin()
                {
                    template = new FingerprintTemplate(new FingerprintImage(File.ReadAllBytes(file), options)).ToByteArray(),
                    candidate = index,
                    score = 0
                };
                _cache.Set(index, cand, cacheOptions);
            });

            stopWatch.Stop();
            TimeSpan gc = stopWatch.Elapsed;
            Console.WriteLine($"Processing time for generating candidates: {(int)gc.TotalMilliseconds} ms");
        }

        public void LoadIntoMemory()
        {
            byteArrayTemplates = _templateHandler.GenerateTemplatesAsByteArrayForMemory();
        }

        public List<CandidateBin> GetCandidatesFromMemory()
        {
            Stopwatch stopWatch = new();
            stopWatch.Start();
            List<CandidateBin> candidates = new();

            // temp work. will need actual way to get data from cache. If can get without key from other cache service, good!
            var fileList = Directory.GetFiles("C:/Users/b.shafi/source/repos/SourceAFIS-API/FP_Engine/DB1_B2");

            for (int i = 0; i < fileList.Length; i++)
            {
                try
                {
                    //var fieldInfo = typeof(IMemoryCache).GetField("_coherentState", BindingFlags.Instance | BindingFlags.NonPublic);
                    //var propertyInfo = fieldInfo.FieldType.GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.NonPublic);
                    //var value = fieldInfo.GetValue(_cache);
                    //var dict = propertyInfo.GetValue(value);
                    //var cacheEntries = dict as IDictionary;
                    //var cacheEntry = cacheEntries[i] as ICacheEntry;

                    _cache.TryGetValue(i, out CandidateBin candidate);

                    // sometimes candidate is empty here. add null check or some logic. IRL, missing data will be pulled from DB. 
                    // Maybe with actual cache this wont happen.
                    candidates.Add((CandidateBin)candidate);

                }
                catch (Exception ex)
                {

                }
            }
            //if (_cache.TryGetValue(key, out List<Candidate>  candidates)) 
            //    return candidates;
            //else 
            //    return new List<Candidate>();
            stopWatch.Stop();
            TimeSpan gc = stopWatch.Elapsed;
            _logger.LogInformation($"Get byte array templates: {(int)gc.TotalMilliseconds} ms");
            return candidates;
        }


    }
}
