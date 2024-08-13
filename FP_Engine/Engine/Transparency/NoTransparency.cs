using FP_Engine.EngineInterface;

namespace FP_Engine.Engine.Transparency
{
    class NoTransparency : FingerprintTransparency
    {
        public static readonly NoTransparency Instance = new NoTransparency();
        // Dispose it immediately, so that it does not hang around in thread-local variable.
        NoTransparency() => Dispose();
        public override bool Accepts(string key) => false;
    }
}
