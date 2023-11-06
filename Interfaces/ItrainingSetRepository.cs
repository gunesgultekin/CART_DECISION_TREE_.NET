namespace CART_DECISION_TREE
{
    public interface ItrainingSetRepository
    {
        public Task<List<trainingSet>> getAllData();
        public HashSet<String> getCandidateSplits();
        public int count();
        public double calculateA1();
        public double calculateA2();
        public double calculateA3();
        public double calculateA4();
        public double calculateA5();


        public double calculateA7();
        public double calculateA8();
        public double calculateA9();

    }
}
