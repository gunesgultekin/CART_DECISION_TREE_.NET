namespace CART_DECISION_TREE.Entities
{
    public class candidateValues
    {
        public string Ax {  get; set; }
        public string LeftVal { get; set; }
        public double ϕ { get; set; }
        public bool isLeaf { get; set; }
        public string leafResult { get; set; }
        public string leafAttribute { get; set; }
    }
}
