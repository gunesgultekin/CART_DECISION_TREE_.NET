using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Globalization;
using System.Diagnostics.Contracts;

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

        public HashSet<String>getCandidateSplits()
        {
            var set = _context.trainingSet.ToList();

            HashSet<String> allCandidates = new HashSet<string>();

           
            foreach(var row in _context.trainingSet)
            {
                allCandidates.Add(row.A5);

            }           
            return allCandidates;        
            
        }

        public int count()
        {
            int count = 0;
            foreach(var row in _context.trainingSet)
            {
                if (row.A3 == "u")
                {
                    count++;

                }

            }
            return count;
        }



        public double calculateA1()
        {
            int total = _context.trainingSet.Count(); // TOTAL RECORDS 
            
            // class good or bad
         
             
            List<trainingSet> attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST "a"
            List<trainingSet> attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST "b"

            foreach(var row in _context.trainingSet)
            {
                if (row.A1 == "a")
                {
                    attr1.Add(row);

                }
                else
                {
                    attr2.Add(row);
                }
            }

            double Pl = (double)attr1.Count() / total;
            double Pr = (double)attr2.Count() / total;

            double pJtl_class1=0;
            double pJtl_class2 = 0;

            double pJtr_class1 = 0;
            double pjtr_class2 = 0;

            

            foreach(var element in attr1)
            {
                if (element.Class == "good")
                {
                    pJtl_class1++;
                }
            }
            pJtl_class1 =  pJtl_class1 / attr1.Count();

            foreach(var element in attr1)
            {
                if(element.Class == "bad")
                {
                    pJtl_class2++;

                }
            }
            pJtl_class2 = pJtl_class2 / attr1.Count();

            foreach (var element in attr2)
            {
                if (element.Class == "good")
                {
                    pJtr_class1++;

                }
            }
            pJtr_class1 /= attr2.Count();


            foreach (var element in attr2)
            {
                if (element.Class == "bad")
                {
                    pjtr_class2++;

                }
            }

            pjtr_class2 /= attr2.Count();

            double Qst = Math.Abs(pJtl_class1 - pJtr_class1) + Math.Abs(pJtl_class2 - pjtr_class2);

            double result = Qst * 2 * (Pl * Pr);

            return result;
           
        }

        public double calculateA2()
        {
            // SPLIT DATASET VALUES FROM MEDIAN VALUE

            int total = _context.trainingSet.Count(); // TOTAL RECORDS
            float avg = 0;
            foreach(var row in _context.trainingSet)
            {
                avg += float.Parse(row.A2, CultureInfo.InvariantCulture.NumberFormat); // FIND SUM OF NUMERICAL ROW A2 VALUES
            }
            avg /= total; // FIND AVERAGE OF NUMERICAL VALUES IN ROW A2

            List<trainingSet> attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST "<= average value"
            List<trainingSet> attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST "> average value"

            foreach(var row in _context.trainingSet)
            {
                if (float.Parse(row.A2, CultureInfo.InvariantCulture.NumberFormat) <= avg) // IF BELOW AVERAGE -> THEN LEFT NODE
                {
                    attr1.Add(row);

                }
                else
                {
                    attr2.Add(row);
                }

            }

            double Pl = (double)attr1.Count() / total;
            double Pr = (double)attr2.Count() / total;

            double pJtl_class1 = 0;
            double pJtl_class2 = 0;

            double pJtr_class1 = 0;
            double pJtr_class2 = 0;

            foreach (var element in attr1)
            {
                if (element.Class == "good")
                {
                    pJtl_class1++;

                }
            }
            pJtl_class1 /= attr1.Count();

            foreach (var element in attr1)
            {
                if (element.Class == "bad")
                {
                    pJtl_class2++;

                }
            }

            pJtl_class2 /= attr1.Count();

            foreach (var element in attr2)
            {
                if (element.Class == "good")
                {
                    pJtr_class1++;
                }
            }

            pJtr_class1 /= attr2.Count();


            foreach (var element in attr2)
            {
                if (element.Class == "bad")
                {
                    pJtr_class2++;
                }
            }

            pJtr_class2 /= attr2.Count();

            double Qst = Math.Abs(pJtl_class1 - pJtr_class1) +
                Math.Abs(pJtl_class2 - pJtr_class2);

            double result = Qst * 2 * (Pl * Pr);

            return result;


        }



        public double calculateA3()
        {
            int total = _context.trainingSet.Count(); // TOTAL RECORDS 

            List<trainingSet> attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST "u"
            List<trainingSet> attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST "y"

            foreach(var row in _context.trainingSet)
            {
                if (row.A3 == "u")
                {
                    attr1.Add(row);

                }
                else
                {
                    attr2.Add(row);
                }
            }

            double Pl = (double)attr1.Count() / total;
            double Pr = (double)attr2.Count() / total;

            double pJtl_class1 = 0;
            double pJtl_class2 = 0;

            double pJtr_class1 = 0;
            double pJtr_class2 = 0;


            foreach(var element in attr1)
            {
                if (element.Class == "good")
                {
                    pJtl_class1++;

                }
            }
            pJtl_class1 /= attr1.Count();

            foreach(var element in attr1)
            {
                if (element.Class == "bad")
                {
                    pJtl_class2++;

                }
            }

            pJtl_class2 /= attr1.Count();

            foreach(var element in attr2)
            {
                if(element.Class == "good")
                {
                    pJtr_class1++;
                }
            }

            pJtr_class1 /= attr2.Count();


            foreach(var element in attr2)
            {
                if(element.Class == "bad")
                {
                    pJtr_class2++;
                }
            }

            pJtr_class2 /= attr2.Count();

            double Qst = Math.Abs(pJtl_class1 - pJtr_class1) +
                Math.Abs(pJtl_class2 - pJtr_class2);

            double result = Qst * 2 * (Pl * Pr);

            return result;

        }

        public double calculateA4()
        {
            int total = _context.trainingSet.Count();
            List<trainingSet> attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST "g"
            List<trainingSet> attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST "p"

            foreach(var row in _context.trainingSet)
            {
                if (row.A4 == "g")
                {
                    attr1.Add(row);

                }
                else
                {
                    attr2.Add(row);
                }
            }

            double Pl = (double) attr1.Count() / total;
            double Pr = (double) attr2.Count() / total;

            double pJtl_class1 = 0;
            double pJtl_class2 = 0;

            double pJtr_class1 = 0;
            double pJtr_class2 = 0;

            foreach(var element in attr1)
            {
                if (element.Class == "good")
                {
                    pJtl_class1++;

                }
            }

            pJtl_class1 /= attr1.Count();
           
            foreach(var element in attr1)
            {
                if (element.Class == "bad")
                {
                    pJtl_class2++;

                }
            }

            pJtl_class2 /= attr1.Count();

            foreach(var element in attr2)
            {
                if (element.Class == "good")
                {
                    pJtr_class1++;

                }
            }

            pJtr_class1 /= attr2.Count();

            foreach(var element in attr2)
            {
                if(element.Class == "bad")
                {
                    pJtr_class2++;
                }
            }

            pJtr_class2 /= attr2.Count();

            double Qst = Math.Abs(pJtl_class1 - pJtr_class1) +
                Math.Abs(pJtl_class2 - pJtr_class2);

            double result = Qst * 2 * (Pl * Pr);

            return result;

        }
        public double calculateA8()
        {
            int total = _context.trainingSet.Count(); // TOTAL RECORDS 

            List<trainingSet> attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST "f"
            List<trainingSet> attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST "t"

            foreach(var row in _context.trainingSet)
            {
                if (row.A8 == "f")
                {
                    attr1.Add(row);

                }
                else
                {
                    attr2.Add(row);
                }
            }

            double Pl = (double)attr1.Count() / total;
            double Pr = (double)attr2.Count() / total;

            double pJtl_class1 = 0;
            double pJtl_class2 = 0;

            double pJtr_class1 = 0;
            double pJtr_class2 = 0;


            foreach (var element in attr1)
            {
                if (element.Class == "good")
                {
                    pJtl_class1++;

                }
            }
            pJtl_class1 /= attr1.Count();

            foreach (var element in attr1)
            {
                if (element.Class == "bad")
                {
                    pJtl_class2++;

                }
            }

            pJtl_class2 /= attr1.Count();

            foreach (var element in attr2)
            {
                if (element.Class == "good")
                {
                    pJtr_class1++;
                }
            }

            pJtr_class1 /= attr2.Count();


            foreach (var element in attr2)
            {
                if (element.Class == "bad")
                {
                    pJtr_class2++;
                }
            }

            pJtr_class2 /= attr2.Count();

            double Qst = Math.Abs(pJtl_class1 - pJtr_class1) +
                Math.Abs(pJtl_class2 - pJtr_class2);

            double result = Qst * 2 * (Pl * Pr);

            return result;
        }

        public double calculateA9()
        {
            int total = _context.trainingSet.Count();


            // CANDIDATE INNER SPLIT - 1 {g} {s,p}

            List<trainingSet> candidate1_attr1 = new List<trainingSet>(); // CANDIDATE SPLIT 1 ATTRIBUTE 1 LIST "g"
            List<trainingSet> candidate1_attr2 = new List<trainingSet>(); // CANDIDATE SPLIT 2 ATTRIBUTE 2 LIST "{s,p}"

            foreach(var row in _context.trainingSet)
            {
                if (row.A9 == "g")
                {
                    candidate1_attr1.Add(row);

                }
                else
                {
                    candidate1_attr2.Add(row); // s or p
                }
            }

            double candidate1_Pl = (double)candidate1_attr1.Count() / total;
            double candidate1_Pr = (double)candidate1_attr2.Count() / total;

            double candidate1_pJtl_class1 = 0;
            double candidate1_pJtl_class2 = 0;

            double candidate1_pJtr_class1 = 0;
            double candidate1_pJtr_class2 = 0;


            foreach (var element in candidate1_attr1)
            {
                if (element.Class == "good")
                {
                    candidate1_pJtl_class1++;
                    

                }
            }
            candidate1_pJtl_class1 /= candidate1_attr1.Count();

            foreach (var element in candidate1_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate1_pJtl_class2++;

                }
            }

            candidate1_pJtl_class2 /=candidate1_attr1.Count();

            foreach (var element in candidate1_attr2)
            {
                if (element.Class == "good")
                {
                    candidate1_pJtr_class1++;
                }
            }

            candidate1_pJtr_class1 /= candidate1_attr2.Count();


            foreach (var element in candidate1_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate1_pJtr_class2++;
                }
            }

            candidate1_pJtr_class2 /= candidate1_attr2.Count();

            double candidate1_Qst = Math.Abs(candidate1_pJtl_class1 - candidate1_pJtr_class1) +
                Math.Abs(candidate1_pJtl_class2 - candidate1_pJtr_class2);

            double candidate1_result = candidate1_Qst * 2 * (candidate1_Pl * candidate1_Pr);

            // CANDIDATE INNER SPLIT - 1 {g} {s,p}








            // CANDIDATE INNER SPLIT - 2 {s} {g,p}

            List<trainingSet> candidate2_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST "u"
            List<trainingSet> candidate2_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST "y"

            foreach(var row in _context.trainingSet)
            {
                if (row.A9 == "s")
                {
                    candidate2_attr1.Add(row);

                }
                else
                {

                    candidate2_attr2.Add(row); // g or p

                }
            }

            double candidate2_Pl = (double)candidate2_attr1.Count() / total;
            double candidate2_Pr = (double)candidate2_attr2.Count() / total;

            double candidate2_pJtl_class1 = 0;
            double candidate2_pJtl_class2 = 0;

            double candidate2_pJtr_class1 = 0;
            double candidate2_pJtr_class2 = 0;


            foreach(var element in candidate2_attr1)
            {
                if (element.Class == "good")
                {
                    candidate2_pJtl_class1++;
                              
                }
            }
            candidate2_pJtl_class1 /= candidate2_attr1.Count();


            foreach(var element in candidate2_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate2_pJtl_class2++;

                }

            }
            candidate2_pJtl_class2 /= candidate2_attr1.Count();


            foreach(var element in candidate2_attr2)
            {
                if (element.Class == "good")
                {
                    candidate2_pJtr_class1++;

                }

            }
            candidate2_pJtr_class1 /= candidate2_attr2.Count();


            foreach (var element in candidate2_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate2_pJtr_class2++;

                }
            }
            candidate2_pJtr_class2 /= candidate2_attr2.Count();


            double candidate2_Qst = Math.Abs(candidate2_pJtl_class1 - candidate2_pJtr_class1) +
                Math.Abs(candidate2_pJtl_class2 - candidate2_pJtr_class2);

            double candidate2_result = candidate2_Qst * 2 * (candidate2_Pl * candidate2_Pr);


            // CANDIDATE INNER SPLIT - 2 {s} {g,p}










            // CANDIDATE INNER SPLIT - 3 {p} {g,s}


            List<trainingSet> candidate3_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST "u"
            List<trainingSet> candidate3_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST "y"


            foreach(var row in _context.trainingSet)
            {
                if (row.A9 == "p")
                {
                    candidate3_attr1.Add(row);

                }
                else
                {
                    candidate3_attr2.Add(row); // g or s
                }
            }

            double candidate3_Pl = (double)candidate3_attr1.Count() / total;
            double candidate3_Pr = (double)candidate3_attr2.Count() / total;

            double candidate3_pJtl_class1 = 0;
            double candidate3_pJtl_class2 = 0;

            double candidate3_pJtr_class1 = 0;
            double candidate3_pJtr_class2 = 0;

            foreach(var element in candidate3_attr1)
            {
                if (element.Class == "good")
                {
                    candidate3_pJtl_class1++;

                }
            }

            candidate3_pJtl_class1 /= candidate3_attr1.Count();


            foreach(var element in candidate3_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate3_pJtl_class2++;

                }

            }
            candidate3_pJtl_class2 /= candidate3_attr1.Count();


            foreach(var element in candidate3_attr2)
            {
                if (element.Class == "good")
                {
                    candidate3_pJtr_class1++;

                }

            }
            candidate3_pJtr_class1 /= candidate3_attr2.Count();


            foreach(var element in candidate3_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate3_pJtr_class2++;

                }
            }
            candidate3_pJtr_class2 /= candidate3_attr2.Count();

            double candidate_3_Qst = Math.Abs(candidate3_pJtl_class1 - candidate3_pJtr_class1) +
                Math.Abs(candidate3_pJtl_class2 - candidate3_pJtr_class2);

            double candidate3_result = candidate_3_Qst * 2 * (candidate3_Pl * candidate3_Pr);


            // CANDIDATE INNER SPLIT - 3 {p} {g,s}

            double comparasion = Math.Max(candidate1_result, candidate2_result);

            double endResult = Math.Max(comparasion,candidate3_result);

            return endResult;

        }






    }
}
