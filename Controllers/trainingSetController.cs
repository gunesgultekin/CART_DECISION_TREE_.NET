using Microsoft.AspNetCore.Mvc;

namespace CART_DECISION_TREE
{
    [ApiController]
    public class trainingSetController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private DBContext _context;
        private ItrainingSetRepository _trainingSetRepository;

        public trainingSetController(DBContext context,ItrainingSetRepository trainingSetRepository)
        {
            this._context = context;
            this._trainingSetRepository = trainingSetRepository;

        }
        [HttpGet("getAllTrainingData")]
        public async Task<List<trainingSet>> getAllData()
        {
            return await _trainingSetRepository.getAllData();


        }

        [HttpGet("getCandidateSplits")]
        public HashSet<String> getCandidateSplits()
        {
            return _trainingSetRepository.getCandidateSplits(); 


        }

        [HttpGet("calculateA1")]
        public double calculateA1()
        {
            return _trainingSetRepository.calculateA1();

        }

        [HttpGet("calculateA2")]
        public double calculateA2()
        {
            return _trainingSetRepository.calculateA2();

        }

        [HttpGet("calculateA3")]
        public double calculateA3()
        {
            return _trainingSetRepository.calculateA3();

        }

        [HttpGet("calculateA4")]
        public double calculateA4()
        {
            return _trainingSetRepository.calculateA4();

        }

        [HttpGet("calculateA8")]
        public double calculateA8()
        {
            return _trainingSetRepository.calculateA8();

        }

        [HttpGet("calculateA9")]
        public double calculateA9()
        {
            return _trainingSetRepository.calculateA9();

        }

        [HttpGet("count")]
        public int count()
        {
            return _trainingSetRepository.count();
        }

    }
}
