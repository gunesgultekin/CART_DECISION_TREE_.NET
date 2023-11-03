using CART_DECISION_TREE.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CART_DECISION_TREE
{
    [ApiController()]
    public class testSetController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private DBContext _context;
        private ItestSetRepository _testSetRepository;

        public testSetController(DBContext context, ItestSetRepository testSetRepository)
        {
            this._context = context;
            this._testSetRepository = testSetRepository;

        }

        [HttpGet("getAllTestData")]
        public async Task<List<testSet>> getAllData()
        {
            return await _testSetRepository.getAllData();


        }
    }
}
