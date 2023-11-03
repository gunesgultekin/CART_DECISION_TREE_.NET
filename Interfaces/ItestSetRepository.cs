namespace CART_DECISION_TREE.Interfaces
{
    public interface ItestSetRepository
    {
        public Task<List<testSet>> getAllData();
    }
}
