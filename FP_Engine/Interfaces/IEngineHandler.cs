using FP_Engine.EngineInterface;
using FP_Engine.HandlerModels;

namespace FP_Engine.Interfaces
{
    public interface IEngineHandler
    {
        public AuthenticationResult Authenticate(FingerprintTemplate probe, FingerprintTemplate candidate, double matchThreshold);
        public IdentificationResult Identify(FingerprintTemplate probe, double threshold, ITemplateHandler templateHandler);
    }
}
