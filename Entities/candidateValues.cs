namespace CART_DECISION_TREE.Entities
{
    public class candidateValues

    {   // CANDIDATE SPLIT
        public string Ax {  get; set; } // ATTRIBUTE NAME (WHICH ATTIBUTE ?)
        public string LeftVal { get; set; } // LEFT NODE 
        public double ϕ { get; set; } // CALCULATED CART ALGORITHM VALUE
        public bool isLeaf { get; set; } // IS LEAF NODE ? IF one of the P(J/TL) Good or P(J/TL) Bad values are = 1 then leaf node
        public string leafResult { get; set; } // WHICH CLASS LEAF NODE ENDS
        public string leafAttribute { get; set; } 
    }
}
