using CART_DECISION_TREE.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CART_DECISION_TREE.Repositories
{
    public class testSetRepository : ItestSetRepository
    {
        private readonly DBContext _context;
        public testSetRepository(DBContext context)
        {
            _context = context;
        }
        public async Task<List<testSet>> getAllData()
        {
            var set = await _context.testSet.ToListAsync();
            return set;
        }

    }
}
