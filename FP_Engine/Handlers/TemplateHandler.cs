using Microsoft.Extensions.Logging;
using FP_Engine.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FP_Engine.HandlerModels;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Internal;
using FP_Engine.EngineInterface;

namespace FP_Engine.Handlers
{
    public class TemplateHandler : ITemplateHandler
    {
        // TODO: Add disposing if objects not destroyed by GC.

        private readonly ILogger<TemplateHandler> _logger;
        public TemplateHandler(ILogger<TemplateHandler> logger)
        {
            _logger = logger;
            logger.LogInformation("TemplateHandler cosntructed");
        }

        ~TemplateHandler() { }


        // Generates templates from images. 
        // In future implementation, there will be seperate methods for generating probe templates during runtime
        // In case of 1:1 (authentication), candidate template will also be generated during runtime
        // In case of 1:n (identification), candidate templates will be pre-processed and stored in DB and cached for quick retrieval
        public FingerprintTemplate GenerateTemplate(string filePath, double dpi)
        {
            FingerprintImageOptions options = new() { Dpi = dpi };
            return dpi != 0.0 ?
                new FingerprintTemplate(new FingerprintImage(File.ReadAllBytes(filePath), options))
                : new FingerprintTemplate(new FingerprintImage(File.ReadAllBytes(filePath)));
        }

        // Generate templates from image binaries (byte[]) - Image converted to byte[] with File.ReadAllBytes()
        public FingerprintTemplate GenerateTemplate(byte[] img, double dpi)
        {
            FingerprintImageOptions options = new() { Dpi = dpi };
            return dpi != 0.0 ?
                new FingerprintTemplate(new FingerprintImage(img, options))
                : new FingerprintTemplate(new FingerprintImage(img));
        }


        // This method is to be used to generate and store multiple templates (as byte array - for efficient storage) at once. 
        // probe templates should not be generated using this function.
        // TODO: Implement storage logic in DB Handler. Should accept DB object or something that can be used to write data to DB.
        public List<byte[]> GenerateTemplatesAsByteArray(List<string> filePaths, double dpi)
        {
            List<byte[]> templates = new();
            FingerprintImageOptions options = new() { Dpi = dpi };
            for (int n = 0; n < filePaths.Count; n++)
            {
                FingerprintTemplate template = dpi != 0.0 ?
                new FingerprintTemplate(new FingerprintImage(File.ReadAllBytes(filePaths[n]), options))
                : new FingerprintTemplate(new FingerprintImage(File.ReadAllBytes(filePaths[n])));
                templates.Add(template.ToByteArray());
            }
            return templates;
        }

        public List<byte[]> GenerateTemplatesAsByteArrayForMemory() 
        {
            List<byte[]> templates = new();
            var fileList = Directory.GetFiles("C:/Users/b.shafi/source/repos/SourceAFIS-API/FP_Engine/DB1_B2");
            List<string> list = new List<string>(fileList); // make this list concurrent as well. or not. see if have to.
            FingerprintImageOptions options = new() { Dpi = 500.0 };
            Parallel.ForEach(fileList, (file, state, index) =>
            {
                FingerprintTemplate template = new FingerprintTemplate(new FingerprintImage(File.ReadAllBytes(fileList[index]), options));
                templates.Add(template.ToByteArray());
            });
            return templates;
        }

        public List<FingerprintTemplate> GenerateTemplatesForLocalTest()
        {
            // Template: 6800 prints - 4 minutes - 35 MB (check effect if store template as byte[] in cache)
            //      - 28.3 templates/second  |  35 ms per template
            // Identify API: - Total request takes 300ms avg.
            //      - 22,500 templates/second  |  0.0441 ms per template (44.1 microseconds)  |  22 templates/millisecond
            // Identify algo: Avg 100 ms for 6800 prints in cache  
            //      - 68000 templates/second  |  0.0147 ms per template (14.7 microseconds)  |  68 templates/millisecond  
            var fileList = Directory.GetFiles("C:/Users/b.shafi/source/repos/SourceAFIS-API/FP_Engine/DB1_B2");
            List<string> list = new List<string>(fileList); // make this list concurrent as well.
            List<FingerprintTemplate> templates = new();
            FingerprintImageOptions options = new() { Dpi = 500.0 };
            Parallel.ForEach(fileList, (file, state, index) =>
            {
                FingerprintTemplate template = new FingerprintTemplate(new FingerprintImage(File.ReadAllBytes(fileList[index]), options));
                templates.Add(template);
            });
            //for (int n = 0; n < fileList.Length; n++)
            //{
            //    FingerprintTemplate template = new FingerprintTemplate(new FingerprintImage(File.ReadAllBytes(fileList[n]), options));
            //    templates.Add(template);
            //}
            return templates;
        }

        // TODO: Implement logic to retrieve template objects (byte[]) from DB and convert to FingerprintTemplate for matching.

        // TODO: Seperate handler for db/cache related tasks
        // TODO: Implement caching logic (FingerprintTemplate)
    }
}
