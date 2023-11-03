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

    }
}
