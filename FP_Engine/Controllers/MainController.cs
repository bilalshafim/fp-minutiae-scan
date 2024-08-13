using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FP_Engine.HandlerModels;
using System.IO;
using FP_Engine.Interfaces;
using FP_Engine.ControllerModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using FP_Engine.Handlers;
using FP_Engine.EngineInterface;

namespace FP_Engine.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class MainController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITemplateHandler _templateHandler;
        private readonly IEngineHandler _engineHandler;
        private readonly CacheHandler _cache;
        public MainController(ILogger<MainController> logger, ITemplateHandler templateHandler, IEngineHandler engineHandler, CacheHandler inMemoryCache)
        {
            _logger = logger;
            _templateHandler = templateHandler;
            _engineHandler = engineHandler;
        }

        // TODO: Implement logic to support ANSI format.
        // TODO: Implement logic to support raw image data (byte[])
        // TODO: Implement logic to support SourceAFIS template format
        // TODO: Research if converting ANSI to SourceAFIS template is a good idea
        // TODO: Add handling for invalid parameters
        // Storage TODOs
        // Candidate object with candidate data.
        // TODO: Implement logic for generating candidate templates, serializing them to byte array and storing in DB (for both matching methods) - seperate endpoint ofc (shoud support pulling from DB)
        // TODO: Setup cache - for 1:N
        // TODO: Implement logic for retrieving templates templates from DB, caching them for quick retrieval

        /// <summary>
        /// Endpoint for 1:1 (Auhtentication) fingerprint matching.
        /// Option to either provide candidate userData (when reading from ID card) or provide candidate fingerprint image data directly in candidate property of parameter <see cref="ProbeData">.
        /// </summary>
        /// <param name="probeData" cref="FP_Engine.ControllerModels.ProbeData"></param>
        /// <param name="userData"> Logic to be implemented for getting user stored fingerprint from template database or cache through UserData.</param>
        /// <param name="probeData"> Image, binary or template data of the probe and candidate fingerprint to be authenticated</param>
        /// <returns><see cref="FP_Engine.HandlerModels.AuthenticationResult"/></returns>
        [HttpPost]
        [Route("authenticate")]
        public AuthenticationResult Authenticate([FromBody] ProbeData probeData, string userData, string format)
        {
            _logger.LogInformation($"Authenticating.... - Request: {probeData}");
            // temp Note: userId object for user EID info when identifying. 
            // temp filepaths for testing. Or for unit/mock tests maybe?
            string pathCandidate = "C:/Users/b.shafi/source/repos/SourceAFIS-API/FP_Engine/SFF_Right_Thumb.bmp";
            string filePath = "C:/Users/b.shafi/source/repos/SourceAFIS-API/FP_Engine/SFF_Right_Thumb.bmp";

            #region Set DPI

            // Since FP_Engine algorithm is not scale-invariant, all images should have DPI
            // TODO: exception if dpi not provided
            string dpiError = String.Empty;
            if (probeData.Dpi == 0)
            {
                dpiError = $"INVALID VALUE ({probeData.Dpi}) provided for parameter 'dpi' in RequestObject - CANNOT be 0.0. Since FP Engine algorithm is not scale-invariant, all images should have DPI.";
                throw new ArgumentOutOfRangeException(nameof(probeData.Dpi), dpiError);
            }
            else if (probeData.Dpi < 20 || probeData.Dpi > 20_000) 
            {
                dpiError = $"INVALID VALUE ({probeData.Dpi}) provided for parameter 'dpi' in RequestObject - CANNOT be < 20.0 OR > 20_000.0";
                throw new ArgumentOutOfRangeException(dpiError);
            }
            //_logger.LogInformation($"Error in deserializing parameter 'dpi' in RequestObject.{((!String.IsNullOrEmpty(dpiError)) ? $" {dpiError}" : "")}");

            #endregion

            #region Set Threshold

            // Set MatchThreshold
            // Certainty in fingerprint recognition is measured by FMR: false match rate. FMR is the frequency with which the system
            // incorrectly recognizes non-matching fingerprints as matching. Threshold 40 corresponds to FMR 0.01%. Applications can increase
            // the threshold to get exponentially lower FMR at the cost of slightly higher FNMR: false non-match rate. In the end, choice of
            // threshold is application-dependent, but 40 is a good starting point.
            string thresholdError = String.Empty;

            if (probeData.Threshold == 0.0)
            {
                thresholdError = $"INVALID VALUE ({probeData.Threshold}) provided for parameter 'threshold' in RequestObject - CANNOT be 0.0. Since FP Engine algorithm is not scale-invariant, all images should have DPI.";
                throw new ArgumentOutOfRangeException(thresholdError);
            }
            //_logger.LogInformation($"Error in deserializing parameter 'threshold' in RequestObject.{((!String.IsNullOrEmpty(thresholdError)) ? $" {thresholdError}" : "")}");

            #endregion

            #region Set Probe based on format parameter

            byte[] probeBin = null;
            // Convert probe to byte array(image data) based on format parameter
            switch(format)
            {
                case "SourceAFIS":
                    probeBin = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(probeData.Probe);
                    break;
                case "test":
                    probeBin = System.IO.File.ReadAllBytes(filePath);
                    break;
                case "ANSI":
                    break;
            }

            #endregion

            #region Generate probe and candidate template if image or binary data
            
            double dpi = probeData.Dpi != 0 ? probeData.Dpi : default;
            double matchThreshold = probeData.Threshold != 0 ? probeData.Threshold : 40;
            FingerprintTemplate probe = _templateHandler.GenerateTemplate(probeBin, dpi);
            FingerprintTemplate candidate = _templateHandler.GenerateTemplate(pathCandidate, dpi);

            #endregion

            #region Authenticate
            //AuthenticationResult result = new();

            // Generate and/or retrieve templates

            // Run matching algorithm
            AuthenticationResult result = _engineHandler.Authenticate(probe, candidate, matchThreshold);
                    
            _logger.LogInformation($"Similarity Score: {result.SimilarityScore}");
            _logger.LogInformation($"Match score: {result.IsMatch}");

            #endregion
            
            return result;
        }


        /// <summary>
        /// Endpoint for 1:n (Identification) fingerprint matching.
        /// </summary>
        /// <param name="probeData" cref="FP_Engine.ControllerModels.ProbeData"></param>
        /// <param name="userData"> Logic to be implemented for getting user stored fingerprint from template database or cache through UserData</param>
        /// <param name="probeData"></param>
        /// <returns><see cref="FP_Engine.HandlerModels.IdentificationResult"/></returns>
        [HttpPost]
        [Route("identify")]
        public IdentificationResult Identify([FromBody] ProbeData probeData, string format)
        {
            Stopwatch stopWatch = new();
            stopWatch.Start();
            string filePath = "C:/Users/b.shafi/source/repos/SourceAFIS-API/FP_Engine/SFF_Right_Thumb.bmp";
            byte[] probeBin = System.IO.File.ReadAllBytes(filePath);
            double dpi = 500;
            double matchThreshold = 40;
            FingerprintTemplate probe = _templateHandler.GenerateTemplate(probeBin, dpi);
            IdentificationResult result = _engineHandler.Identify(probe, matchThreshold, _templateHandler);
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            _logger.LogInformation($"Total processing time: {((int)ts.TotalMilliseconds)} ms");
            return result;
        }


        /// <summary>
        /// Endpoint for enrolling user.
        /// Takes user data, fingerprint scan(s)
        /// Stores userdata (Name, etc) separately and Candidate (processed cachable fingerprint data separately). Both have UUID.
        /// On authentication, if fingerprint image not provided (userData provided instead), gets the user's fingerprint data from DB.
        /// On identification, UUID in matched Candidate is used to get userData from DB if needed.
        /// </summary>
        /// <param name="probeData"></param>
        /// <returns><see cref="FP_Engine.HandlerModels.IdentificationResult"/></returns>
        [HttpPost]
        [Route("enroll")]
        public void Enroll()
        {

        }

        /// <summary>
        /// Endpoint for generating templates and storing in DB. Retreives raw images from DB.
        /// Stores as raw binary of FingerprintTemplate.ToByteArray()
        /// Calls the SubjectHandler class which calls the TemplateHandler class.
        /// Low priority: check if needs to be implemented for MVP.
        /// </summary>
        /// <param name="nothing"></param>
        /// <returns><see cref="FP_Engine.HandlerModels.IdentificationResult"/></returns>
        [HttpPost]
        [Route("generate")]
        public void GenerateTemplates()
        {

        }



    }
}       
