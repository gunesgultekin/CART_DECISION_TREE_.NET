namespace CART_DECISION_TREE
{
    public interface ItrainingSetRepository
    {
        public Task<List<trainingSet>> getAllData();
    }
}
