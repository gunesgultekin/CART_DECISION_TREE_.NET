using Microsoft.EntityFrameworkCore;

namespace CART_DECISION_TREE
{
    public class trainingSetRepository : ItrainingSetRepository
    {
        private readonly DBContext _context;
        public trainingSetRepository(DBContext context)
        {
            _context = context;
        }
        public async Task<List<trainingSet>> getAllData()
        {
            var set = await _context.trainingSet.ToListAsync();
            return set;
        }


    }
}
