using FP_Engine.EngineInterface;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FP_Engine.Interfaces
{
    public interface ITemplateHandler
    {
        public FingerprintTemplate GenerateTemplate(string filePath, double dpi);
        public FingerprintTemplate GenerateTemplate(byte[] img, double dpi);
        public List<byte[]> GenerateTemplatesAsByteArray(List<string> filePaths, double dpi);
        public List<FingerprintTemplate> GenerateTemplatesForLocalTest();
        public List<byte[]> GenerateTemplatesAsByteArrayForMemory();

    }
}
