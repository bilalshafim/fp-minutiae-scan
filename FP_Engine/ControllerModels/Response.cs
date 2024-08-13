namespace FP_Engine.ControllerModels
{
    public class Response
    {
        public string Method { get; set; }
        public double Score { get; set; }
        public bool IsMatch { get; set; }
        public object CandidateData { get; set; }
    }

}
