using System;
using FP_Engine.EngineInterface;

namespace FP_Engine.HandlerModels
{
    public class AuthenticationRequest
    {
        public FingerprintTemplate Probe {  get; set; }
        public double Dpi { get; set; }
        public double MatchThreshold { get; set; }
    }
}
