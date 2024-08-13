using FP_Engine.EngineInterface;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FP_Engine.HandlerModels
{
    // Candidate class: Object to be stored in DB

    [Keyless]
    public class Candidate
    {
        //private Guid _id;
        //[Key]
        //public Guid ID
        //{
        //    get { return _id; }
        //}
        [NotMapped]
        public FingerprintTemplate template { get; set; }
        // candidateId: to get userData after subject is found
        public long candidate { get; set; }
        public double score { get; set; } = 0;
        public Candidate(FingerprintTemplate candidatetemplate, long id,  double maxScore)
        {
            template = candidatetemplate;
            candidate = id;
            score = maxScore;
        }
        public Candidate()
        {

        }
    }

    [Keyless]
    public class CandidateBin
    {
        //private Guid _id;
        //[Key]
        //public Guid ID
        //{
        //    get { return _id; }
        //}
        [NotMapped]
        public byte[] template { get; set; }
        // candidateId: to get userData after subject is found
        public long candidate { get; set; }
        public double score { get; set; } = 0;
        public CandidateBin(byte[] candidatetemplate, long id, double maxScore)
        {
            template = candidatetemplate;
            candidate = id;
            score = maxScore;
        }
        public CandidateBin()
        {

        }
    }
}
