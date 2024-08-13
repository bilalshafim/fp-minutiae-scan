namespace FP_Engine.HandlerModels
{
    public class AuthenticationResult
    {
        public bool IsMatch { get; set;}
        public double SimilarityScore { get; set;}
        public string info { get; set; }

    }
}
