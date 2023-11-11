using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace CART_DECISION_TREE
{


    // ***** ALGORITHM *****

    // 1) CALCULATE THE Φ  = 2 * PL * PR * Q(s/t) VALUES FOR 
    // ALL THE CANDIDATE BINARY SPLITS WITHIN ATTRIBUTES A1,A2,A3,A4,A5,A6,A7,A8 and A9.

    // ( THERE MAY BE INNER CANDIDATE SPLITS ex: A5 - CANDIDATE INNER SPLIT - 1 {cc} {q,i,k,w,c,j,d,aa,e,ff,m,x,r} , // A5 - CANDIDATE INNER SPLIT - 2 {q} {cc,i,k,w,c,j,d,aa,e,ff,m,x,r}
    // FIRST CALCULATE INNER CANDIDATE SPLIT Φ VALUES THEN PICK THE GREATEST VALUE WITHIN ATTRIBUTE

    // COMPARE ALL Φ VALUES AND CHOOSE THE GREATEST.

    // PLACE CHOSEN VALUE TO THE ROOT OF THE DECISION TREE

    // RECOMPILE TRAINING DATASET:

    // IF (DATASET.ROW.Ax = attributej && class == "class1") -> COUNTy ++
    // IF (COUNTy = attributej.count()) (IF ALL Ax.attributej values are belong exact one class (result) ) -> THEN LEAF NODE -> DELETE RELATED DATA RECOMPILE


    // CONTINUE FROM THE FIRST STEP


    public class trainingSetRepository : ItrainingSetRepository
    {
        private readonly DBContext _context;
        public trainingSetRepository(DBContext context)
        {
            _context = context;
        }


        

        public int createDecisionTree()
        {
            /*
            var decisionTree = new binaryTree<int>();
            foreach (var value in new[] { 81, 115, 611, 4, 1, 2 })
                decisionTree.Add(value);
            var root = decisionTree.Root;
            Node<int>.print2D(root);
            return decisionTree.Root.Left.Value;
            */


            return 1;
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
                allCandidates.Add(row.A6);

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

        public double calculateA5()
        {
            List<double> allCandidateResults = new List<double>(); // ALL VALUES OF INNER CANDIDATES WILL BE STORED FOR END-COMPARASION

            int total = _context.trainingSet.Count();

            // CANDIDATE INNER SPLIT - 1 {cc} {q,i,k,w,c,j,d,aa,e,ff,m,x,r}

            List<trainingSet> candidate1_attr1 = new List<trainingSet>(); // CANDIDATE SPLIT 1 ATTRIBUTE 1 LIST "{cc}"
            List<trainingSet> candidate1_attr2 = new List<trainingSet>(); // CANDIDATE SPLIT 2 ATTRIBUTE 2 LIST "{q,i,k,w,c,j,d,aa,e,ff,m,x,r}"

            foreach(var row in _context.trainingSet)
            {
                if (row.A5 == "cc")
                {
                    candidate1_attr1.Add(row); // LEFT NODE {cc}

                }
                else
                {
                    candidate1_attr2.Add(row); // RIGHT NODE {q,i,k,w,c,j,d,aa,e,ff,m,x,r}
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

            candidate1_pJtl_class2 /= candidate1_attr1.Count();

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
            allCandidateResults.Add(candidate1_result);
            // CANDIDATE INNER SPLIT - 1 {cc} {q,i,k,w,c,j,d,aa,e,ff,m,x,r}





            // CANDIDATE INNER SPLIT - 2 {q} {cc,i,k,w,c,j,d,aa,e,ff,m,x,r}


            List<trainingSet> candidate2_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {q}
            List<trainingSet> candidate2_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {cc,i,k,w,c,j,d,aa,e,ff,m,x,r}

            foreach(var row in _context.trainingSet)
            {
                if (row.A5 == "q")
                {
                    candidate2_attr1.Add(row); // if {q}
                }
                else
                {
                    candidate2_attr2.Add(row); // if if {cc,i,k,w,c,j,d,aa,e,ff,m,x,r}
                }
            }

            double candidate2_Pl = (double)candidate2_attr1.Count() / total;
            double candidate2_Pr = (double)candidate2_attr2.Count() / total;

            double candidate2_pJtl_class1 = 0;
            double candidate2_pJtl_class2 = 0;

            double candidate2_pJtr_class1 = 0;
            double candidate2_pJtr_class2 = 0;


            foreach (var element in candidate2_attr1)
            {
                if (element.Class == "good")
                {
                    candidate2_pJtl_class1++;

                }
            }
            candidate2_pJtl_class1 /= candidate2_attr1.Count();


            foreach (var element in candidate2_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate2_pJtl_class2++;

                }

            }
            candidate2_pJtl_class2 /= candidate2_attr1.Count();


            foreach (var element in candidate2_attr2)
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
            allCandidateResults.Add(candidate2_result);

            // CANDIDATE INNER SPLIT - 2 {q} {cc,i,k,w,c,j,d,aa,e,ff,m,x,r}

            // CANDIDATE INNER SPLIT - 3 {i} {cc,q,k,w,c,j,d,aa,e,ff,m,x,r}

            List<trainingSet> candidate3_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {i}
            List<trainingSet> candidate3_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {cc,q,k,w,c,j,d,aa,e,ff,m,x,r}

            foreach(var row in _context.trainingSet)
            {
                if (row.A5 == "i")
                {
                    candidate3_attr1.Add(row);

                }
                else
                {
                    candidate3_attr2.Add(row);
                }
            }

            double candidate3_Pl = (double)candidate3_attr1.Count() / total;
            double candidate3_Pr = (double)candidate3_attr2.Count() / total;

            double candidate3_pJtl_class1 = 0;
            double candidate3_pJtl_class2 = 0;

            double candidate3_pJtr_class1 = 0;
            double candidate3_pJtr_class2 = 0;

            foreach (var element in candidate3_attr1)
            {
                if (element.Class == "good")
                {
                    candidate3_pJtl_class1++;

                }
            }

            candidate3_pJtl_class1 /= candidate3_attr1.Count();


            foreach (var element in candidate3_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate3_pJtl_class2++;

                }

            }
            candidate3_pJtl_class2 /= candidate3_attr1.Count();


            foreach (var element in candidate3_attr2)
            {
                if (element.Class == "good")
                {
                    candidate3_pJtr_class1++;

                }

            }
            candidate3_pJtr_class1 /= candidate3_attr2.Count();


            foreach (var element in candidate3_attr2)
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
            allCandidateResults.Add(candidate3_result);

            // CANDIDATE INNER SPLIT - 3 {i} {cc,q,k,w,c,j,d,aa,e,ff,m,x,r}








            // CANDIDATE INNER SPLIT - 4 {k} {i,cc,q,w,c,j,d,aa,e,ff,m,x,r}

            List<trainingSet> candidate4_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {k}
            List<trainingSet> candidate4_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {i,cc,q,w,c,j,d,aa,e,ff,m,x,r}

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "k")
                {
                    candidate4_attr1.Add(row);

                }
                else
                {
                    candidate4_attr2.Add(row);
                }
            }

            double candidate4_Pl = (double)candidate4_attr1.Count() / total;
            double candidate4_Pr = (double)candidate4_attr2.Count() / total;

            double candidate4_pJtl_class1 = 0;
            double candidate4_pJtl_class2 = 0;

            double candidate4_pJtr_class1 = 0;
            double candidate4_pJtr_class2 = 0;

            foreach (var element in candidate4_attr1)
            {
                if (element.Class == "good")
                {
                    candidate4_pJtl_class1++;

                }
            }

            candidate4_pJtl_class1 /= candidate4_attr1.Count();


            foreach (var element in candidate4_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate4_pJtl_class2++;

                }

            }
            candidate4_pJtl_class2 /= candidate4_attr1.Count();


            foreach (var element in candidate4_attr2)
            {
                if (element.Class == "good")
                {
                    candidate4_pJtr_class1++;

                }

            }
            candidate4_pJtr_class1 /= candidate4_attr2.Count();


            foreach (var element in candidate4_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate4_pJtr_class2++;

                }
            }
            candidate4_pJtr_class2 /= candidate4_attr2.Count();

            double candidate_4_Qst = Math.Abs(candidate4_pJtl_class1 - candidate4_pJtr_class1) +
                Math.Abs(candidate4_pJtl_class2 - candidate4_pJtr_class2);

            double candidate4_result = candidate_4_Qst * 2 * (candidate4_Pl * candidate4_Pr);
            allCandidateResults.Add(candidate4_result);

            // CANDIDATE INNER SPLIT - 4 {k} {i,cc,q,w,c,j,d,aa,e,ff,m,x,r}

            // CANDIDATE INNER SPLIT - 5 {w} {k,i,cc,q,c,j,d,aa,e,ff,m,x,r}


            List<trainingSet> candidate5_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {w}
            List<trainingSet> candidate5_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {k,i,cc,q,c,j,d,aa,e,ff,m,x,r}

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "w")
                {
                    candidate5_attr1.Add(row);

                }
                else
                {
                    candidate5_attr2.Add(row);
                }
            }

            double candidate5_Pl = (double)candidate5_attr1.Count() / total;
            double candidate5_Pr = (double)candidate5_attr2.Count() / total;

            double candidate5_pJtl_class1 = 0;
            double candidate5_pJtl_class2 = 0;

            double candidate5_pJtr_class1 = 0;
            double candidate5_pJtr_class2 = 0;

            foreach (var element in candidate5_attr1)
            {
                if (element.Class == "good")
                {
                    candidate5_pJtl_class1++;

                }
            }

            candidate5_pJtl_class1 /= candidate5_attr1.Count();


            foreach (var element in candidate5_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate5_pJtl_class2++;

                }

            }
            candidate5_pJtl_class2 /= candidate5_attr1.Count();


            foreach (var element in candidate5_attr2)
            {
                if (element.Class == "good")
                {
                    candidate5_pJtr_class1++;

                }

            }
            candidate5_pJtr_class1 /= candidate5_attr2.Count();


            foreach (var element in candidate5_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate5_pJtr_class2++;

                }
            }
            candidate5_pJtr_class2 /= candidate5_attr2.Count();

            double candidate_5_Qst = Math.Abs(candidate5_pJtl_class1 - candidate5_pJtr_class1) +
                Math.Abs(candidate5_pJtl_class2 - candidate5_pJtr_class2);

            double candidate5_result = candidate_5_Qst * 2 * (candidate5_Pl * candidate5_Pr);
            allCandidateResults.Add(candidate5_result);

            // CANDIDATE INNER SPLIT - 5 {w} {k,i,cc,q,c,j,d,aa,e,ff,m,x,r}

            // CANDIDATE INNER SPLIT - 6 {c} {k,i,cc,q,w,j,d,aa,e,ff,m,x,r}

            List<trainingSet> candidate6_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {c}
            List<trainingSet> candidate6_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {k,i,cc,q,w,j,d,aa,e,ff,m,x,r}

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "c")
                {
                    candidate6_attr1.Add(row);

                }
                else
                {
                    candidate6_attr2.Add(row);
                }
            }

            double candidate6_Pl = (double)candidate6_attr1.Count() / total;
            double candidate6_Pr = (double)candidate6_attr2.Count() / total;

            double candidate6_pJtl_class1 = 0;
            double candidate6_pJtl_class2 = 0;

            double candidate6_pJtr_class1 = 0;
            double candidate6_pJtr_class2 = 0;

            foreach (var element in candidate6_attr1)
            {
                if (element.Class == "good")
                {
                    candidate6_pJtl_class1++;

                }
            }

            candidate6_pJtl_class1 /= candidate6_attr1.Count();


            foreach (var element in candidate6_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate6_pJtl_class2++;

                }

            }
            candidate6_pJtl_class2 /= candidate6_attr1.Count();


            foreach (var element in candidate6_attr2)
            {
                if (element.Class == "good")
                {
                    candidate6_pJtr_class1++;

                }

            }
            candidate6_pJtr_class1 /= candidate6_attr2.Count();


            foreach (var element in candidate6_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate6_pJtr_class2++;

                }
            }
            candidate6_pJtr_class2 /= candidate6_attr2.Count();

            double candidate_6_Qst = Math.Abs(candidate6_pJtl_class1 - candidate6_pJtr_class1) +
                Math.Abs(candidate6_pJtl_class2 - candidate6_pJtr_class2);

            double candidate6_result = candidate_6_Qst * 2 * (candidate6_Pl * candidate6_Pr);
            allCandidateResults.Add(candidate6_result);
            // CANDIDATE INNER SPLIT - 6 {c} {k,i,cc,q,w,j,d,aa,e,ff,m,x,r}

            // CANDIDATE INNER SPLIT - 7 {j} {k,i,cc,q,w,c,d,aa,e,ff,m,x,r}


            List<trainingSet> candidate7_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {c}
            List<trainingSet> candidate7_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {k,i,cc,q,w,j,d,aa,e,ff,m,x,r}

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "j")
                {
                    candidate7_attr1.Add(row);

                }
                else
                {
                    candidate7_attr2.Add(row);
                }
            }

            double candidate7_Pl = (double)candidate7_attr1.Count() / total;
            double candidate7_Pr = (double)candidate7_attr2.Count() / total;

            double candidate7_pJtl_class1 = 0;
            double candidate7_pJtl_class2 = 0;

            double candidate7_pJtr_class1 = 0;
            double candidate7_pJtr_class2 = 0;

            foreach (var element in candidate7_attr1)
            {
                if (element.Class == "good")
                {
                    candidate7_pJtl_class1++;

                }
            }

            candidate7_pJtl_class1 /= candidate7_attr1.Count();


            foreach (var element in candidate7_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate7_pJtl_class2++;

                }

            }
            candidate7_pJtl_class2 /= candidate7_attr1.Count();


            foreach (var element in candidate7_attr2)
            {
                if (element.Class == "good")
                {
                    candidate7_pJtr_class1++;

                }

            }
            candidate7_pJtr_class1 /= candidate7_attr2.Count();


            foreach (var element in candidate7_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate7_pJtr_class2++;

                }
            }
            candidate7_pJtr_class2 /= candidate7_attr2.Count();

            double candidate_7_Qst = Math.Abs(candidate7_pJtl_class1 - candidate7_pJtr_class1) +
                Math.Abs(candidate7_pJtl_class2 - candidate7_pJtr_class2);

            double candidate7_result = candidate_7_Qst * 2 * (candidate7_Pl * candidate7_Pr);
            allCandidateResults.Add(candidate7_result);

            // CANDIDATE INNER SPLIT - 7 {j} {k,i,cc,q,w,c,d,aa,e,ff,m,x,r}

            // CANDIDATE INNER SPLIT - 8 {d} {j,k,i,cc,q,w,c,aa,e,ff,m,x,r}


            List<trainingSet> candidate8_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {c}
            List<trainingSet> candidate8_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {k,i,cc,q,w,j,d,aa,e,ff,m,x,r}

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "d")
                {
                    candidate8_attr1.Add(row);

                }
                else
                {
                    candidate8_attr2.Add(row);
                }
            }

            double candidate8_Pl = (double)candidate8_attr1.Count() / total;
            double candidate8_Pr = (double)candidate8_attr2.Count() / total;

            double candidate8_pJtl_class1 = 0;
            double candidate8_pJtl_class2 = 0;

            double candidate8_pJtr_class1 = 0;
            double candidate8_pJtr_class2 = 0;

            foreach (var element in candidate8_attr1)
            {
                if (element.Class == "good")
                {
                    candidate8_pJtl_class1++;

                }
            }

            candidate8_pJtl_class1 /= candidate8_attr1.Count();


            foreach (var element in candidate8_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate8_pJtl_class2++;

                }

            }
            candidate8_pJtl_class2 /= candidate8_attr1.Count();


            foreach (var element in candidate8_attr2)
            {
                if (element.Class == "good")
                {
                    candidate8_pJtr_class1++;

                }

            }
            candidate8_pJtr_class1 /= candidate8_attr2.Count();


            foreach (var element in candidate8_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate8_pJtr_class2++;

                }
            }
            candidate8_pJtr_class2 /= candidate8_attr2.Count();

            double candidate_8_Qst = Math.Abs(candidate8_pJtl_class1 - candidate8_pJtr_class1) +
                Math.Abs(candidate8_pJtl_class2 - candidate8_pJtr_class2);

            double candidate8_result = candidate_8_Qst * 2 * (candidate8_Pl * candidate8_Pr);
            allCandidateResults.Add(candidate8_result);

            // CANDIDATE INNER SPLIT - 8 {d} {j,k,i,cc,q,w,c,aa,e,ff,m,x,r}

            // CANDIDATE INNER SPLIT - 9 {aa} {j,k,i,cc,q,w,c,d,e,ff,m,x,r}

            List<trainingSet> candidate9_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {c}
            List<trainingSet> candidate9_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {k,i,cc,q,w,j,d,aa,e,ff,m,x,r}

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "aa")
                {
                    candidate9_attr1.Add(row);

                }
                else
                {
                    candidate9_attr2.Add(row);
                }
            }

            double candidate9_Pl = (double)candidate9_attr1.Count() / total;
            double candidate9_Pr = (double)candidate9_attr2.Count() / total;

            double candidate9_pJtl_class1 = 0;
            double candidate9_pJtl_class2 = 0;

            double candidate9_pJtr_class1 = 0;
            double candidate9_pJtr_class2 = 0;

            foreach (var element in candidate9_attr1)
            {
                if (element.Class == "good")
                {
                    candidate9_pJtl_class1++;

                }
            }

            candidate9_pJtl_class1 /= candidate9_attr1.Count();


            foreach (var element in candidate9_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate9_pJtl_class2++;

                }

            }
            candidate9_pJtl_class2 /= candidate9_attr1.Count();


            foreach (var element in candidate9_attr2)
            {
                if (element.Class == "good")
                {
                    candidate9_pJtr_class1++;

                }

            }
            candidate9_pJtr_class1 /= candidate9_attr2.Count();


            foreach (var element in candidate9_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate9_pJtr_class2++;

                }
            }
            candidate9_pJtr_class2 /= candidate9_attr2.Count();

            double candidate_9_Qst = Math.Abs(candidate9_pJtl_class1 - candidate9_pJtr_class1) +
                Math.Abs(candidate9_pJtl_class2 - candidate9_pJtr_class2);

            double candidate9_result = candidate_9_Qst * 2 * (candidate9_Pl * candidate9_Pr);
            allCandidateResults.Add(candidate9_result);
            // CANDIDATE INNER SPLIT - 9 {aa} {j,k,i,cc,q,w,c,d,e,ff,m,x,r}


            // CANDIDATE INNER SPLIT - 10 {e} {j,k,i,cc,q,w,c,d,aa,ff,m,x,r}

            List<trainingSet> candidate10_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {c}
            List<trainingSet> candidate10_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {k,i,cc,q,w,j,d,aa,e,ff,m,x,r}

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "e")
                {
                    candidate10_attr1.Add(row);

                }
                else
                {
                    candidate10_attr2.Add(row);
                }
            }

            double candidate10_Pl = (double)candidate10_attr1.Count() / total;
            double candidate10_Pr = (double)candidate10_attr2.Count() / total;

            double candidate10_pJtl_class1 = 0;
            double candidate10_pJtl_class2 = 0;

            double candidate10_pJtr_class1 = 0;
            double candidate10_pJtr_class2 = 0;

            foreach (var element in candidate10_attr1)
            {
                if (element.Class == "good")
                {
                    candidate10_pJtl_class1++;

                }
            }

            candidate10_pJtl_class1 /= candidate10_attr1.Count();


            foreach (var element in candidate10_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate10_pJtl_class2++;

                }

            }
            candidate10_pJtl_class2 /= candidate10_attr1.Count();


            foreach (var element in candidate10_attr2)
            {
                if (element.Class == "good")
                {
                    candidate10_pJtr_class1++;

                }

            }
            candidate10_pJtr_class1 /= candidate10_attr2.Count();


            foreach (var element in candidate10_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate10_pJtr_class2++;

                }
            }
            candidate10_pJtr_class2 /= candidate10_attr2.Count();

            double candidate_10_Qst = Math.Abs(candidate10_pJtl_class1 - candidate10_pJtr_class1) +
                Math.Abs(candidate10_pJtl_class2 - candidate10_pJtr_class2);

            double candidate10_result = candidate_10_Qst * 2 * (candidate10_Pl * candidate10_Pr);
            allCandidateResults.Add(candidate10_result);
            // CANDIDATE INNER SPLIT - 10 {e} {j,k,i,cc,q,w,c,d,aa,ff,m,x,r}

            // CANDIDATE INNER SPLIT - 11 {ff} {e,j,k,i,cc,q,w,c,d,aa,m,x,r}

            List<trainingSet> candidate11_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {c}
            List<trainingSet> candidate11_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {k,i,cc,q,w,j,d,aa,e,ff,m,x,r}

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "ff")
                {
                    candidate11_attr1.Add(row);

                }
                else
                {
                    candidate11_attr2.Add(row);
                }
            }

            double candidate11_Pl = (double)candidate11_attr1.Count() / total;
            double candidate11_Pr = (double)candidate11_attr2.Count() / total;

            double candidate11_pJtl_class1 = 0;
            double candidate11_pJtl_class2 = 0;

            double candidate11_pJtr_class1 = 0;
            double candidate11_pJtr_class2 = 0;

            foreach (var element in candidate11_attr1)
            {
                if (element.Class == "good")
                {
                    candidate11_pJtl_class1++;

                }
            }

            candidate11_pJtl_class1 /= candidate11_attr1.Count();


            foreach (var element in candidate11_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate11_pJtl_class2++;

                }

            }
            candidate11_pJtl_class2 /= candidate11_attr1.Count();


            foreach (var element in candidate11_attr2)
            {
                if (element.Class == "good")
                {
                    candidate11_pJtr_class1++;

                }

            }
            candidate11_pJtr_class1 /= candidate11_attr2.Count();


            foreach (var element in candidate11_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate11_pJtr_class2++;

                }
            }
            candidate11_pJtr_class2 /= candidate11_attr2.Count();

            double candidate_11_Qst = Math.Abs(candidate11_pJtl_class1 - candidate11_pJtr_class1) +
                Math.Abs(candidate11_pJtl_class2 - candidate11_pJtr_class2);

            double candidate11_result = candidate_11_Qst * 2 * (candidate11_Pl * candidate11_Pr);
            allCandidateResults.Add(candidate11_result);
            // CANDIDATE INNER SPLIT - 11 {ff} {e,j,k,i,cc,q,w,c,d,aa,m,x,r}

            // CANDIDATE INNER SPLIT - 12 {m} {ff,e,j,k,i,cc,q,w,c,d,aa,x,r}


            List<trainingSet> candidate12_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {c}
            List<trainingSet> candidate12_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {k,i,cc,q,w,j,d,aa,e,ff,m,x,r}

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "m")
                {
                    candidate12_attr1.Add(row);

                }
                else
                {
                    candidate12_attr2.Add(row);
                }
            }

            double candidate12_Pl = (double)candidate12_attr1.Count() / total;
            double candidate12_Pr = (double)candidate12_attr2.Count() / total;

            double candidate12_pJtl_class1 = 0;
            double candidate12_pJtl_class2 = 0;

            double candidate12_pJtr_class1 = 0;
            double candidate12_pJtr_class2 = 0;

            foreach (var element in candidate12_attr1)
            {
                if (element.Class == "good")
                {
                    candidate12_pJtl_class1++;

                }
            }

            candidate12_pJtl_class1 /= candidate12_attr1.Count();


            foreach (var element in candidate12_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate12_pJtl_class2++;

                }

            }
            candidate12_pJtl_class2 /= candidate12_attr1.Count();


            foreach (var element in candidate12_attr2)
            {
                if (element.Class == "good")
                {
                    candidate12_pJtr_class1++;

                }

            }
            candidate12_pJtr_class1 /= candidate12_attr2.Count();


            foreach (var element in candidate12_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate12_pJtr_class2++;

                }
            }
            candidate12_pJtr_class2 /= candidate12_attr2.Count();

            double candidate_12_Qst = Math.Abs(candidate12_pJtl_class1 - candidate12_pJtr_class1) +
                Math.Abs(candidate12_pJtl_class2 - candidate12_pJtr_class2);

            double candidate12_result = candidate_12_Qst * 2 * (candidate12_Pl * candidate12_Pr);
            allCandidateResults.Add(candidate12_result);
            // CANDIDATE INNER SPLIT - 12 {m} {ff,e,j,k,i,cc,q,w,c,d,aa,x,r}

            // CANDIDATE INNER SPLIT - 13 {x} {ff,e,j,k,i,cc,q,w,c,d,aa,r,m}

            List<trainingSet> candidate13_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {c}
            List<trainingSet> candidate13_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {k,i,cc,q,w,j,d,aa,e,ff,m,x,r}

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "x")
                {
                    candidate13_attr1.Add(row);

                }
                else
                {
                    candidate13_attr2.Add(row);
                }
            }

            double candidate13_Pl = (double)candidate13_attr1.Count() / total;
            double candidate13_Pr = (double)candidate13_attr2.Count() / total;

            double candidate13_pJtl_class1 = 0;
            double candidate13_pJtl_class2 = 0;

            double candidate13_pJtr_class1 = 0;
            double candidate13_pJtr_class2 = 0;

            foreach (var element in candidate13_attr1)
            {
                if (element.Class == "good")
                {
                    candidate13_pJtl_class1++;

                }
            }

            candidate13_pJtl_class1 /= candidate13_attr1.Count();


            foreach (var element in candidate13_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate13_pJtl_class2++;

                }

            }
            candidate13_pJtl_class2 /= candidate13_attr1.Count();


            foreach (var element in candidate13_attr2)
            {
                if (element.Class == "good")
                {
                    candidate13_pJtr_class1++;

                }

            }
            candidate13_pJtr_class1 /= candidate13_attr2.Count();


            foreach (var element in candidate13_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate13_pJtr_class2++;

                }
            }
            candidate13_pJtr_class2 /= candidate13_attr2.Count();

            double candidate_13_Qst = Math.Abs(candidate13_pJtl_class1 - candidate13_pJtr_class1) +
                Math.Abs(candidate13_pJtl_class2 - candidate13_pJtr_class2);

            double candidate13_result = candidate_13_Qst * 2 * (candidate13_Pl * candidate13_Pr);
            allCandidateResults.Add(candidate13_result);
            // CANDIDATE INNER SPLIT - 13 {x} {ff,e,j,k,i,cc,q,w,c,d,aa,r,m}


            // CANDIDATE INNER SPLIT - 14 {r} {x,ff,e,j,k,i,cc,q,w,c,d,aa,r,m}

            List<trainingSet> candidate14_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST {c}
            List<trainingSet> candidate14_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST {k,i,cc,q,w,j,d,aa,e,ff,m,x,r}

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "r")
                {
                    candidate14_attr1.Add(row);

                }
                else
                {
                    candidate14_attr2.Add(row);
                }
            }

            double candidate14_Pl = (double)candidate14_attr1.Count() / total;
            double candidate14_Pr = (double)candidate14_attr2.Count() / total;

            double candidate14_pJtl_class1 = 0;
            double candidate14_pJtl_class2 = 0;

            double candidate14_pJtr_class1 = 0;
            double candidate14_pJtr_class2 = 0;

            foreach (var element in candidate14_attr1)
            {
                if (element.Class == "good")
                {
                    candidate14_pJtl_class1++;

                }
            }

            candidate14_pJtl_class1 /= candidate14_attr1.Count();


            foreach (var element in candidate14_attr1)
            {
                if (element.Class == "bad")

                {
                    candidate14_pJtl_class2++;

                }

            }
            candidate14_pJtl_class2 /= candidate14_attr1.Count();


            foreach (var element in candidate14_attr2)
            {
                if (element.Class == "good")
                {
                    candidate14_pJtr_class1++;

                }

            }
            candidate14_pJtr_class1 /= candidate14_attr2.Count();


            foreach (var element in candidate14_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate14_pJtr_class2++;

                }
            }
            candidate14_pJtr_class2 /= candidate14_attr2.Count();

            double candidate_14_Qst = Math.Abs(candidate14_pJtl_class1 - candidate14_pJtr_class1) +
                Math.Abs(candidate14_pJtl_class2 - candidate14_pJtr_class2);


            double candidate14_result = candidate_14_Qst * 2 * (candidate14_Pl * candidate14_Pr);
            allCandidateResults.Add(candidate14_result);

            // CANDIDATE INNER SPLIT - 14 {r} {x,ff,e,j,k,i,cc,q,w,c,d,aa,r,m}

            return allCandidateResults.Max(); // RETURN GREATEST CANDIDATE VALUE

        }

        public double calculateA6()
        {
            List<double> allCandidateResults = new List<double>(); // ALL VALUES OF INNER CANDIDATES WILL BE STORED FOR END-COMPARASION

            int total = _context.trainingSet.Count();

            // CANDIDATE INNER SPLIT - 1 {v} {bb,h,j,z,ff,n,dd,o}

            List<trainingSet> candidate1_attr1 = new List<trainingSet>(); // CANDIDATE SPLIT 1 ATTRIBUTE 1 LIST 
            List<trainingSet> candidate1_attr2 = new List<trainingSet>(); // CANDIDATE SPLIT 2 ATTRIBUTE 2 LIST 

            foreach (var row in _context.trainingSet)
            {
                if (row.A6 == "v")
                {
                    candidate1_attr1.Add(row); // if v
                }
                else
                {
                    candidate1_attr2.Add(row); // if other
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

            candidate1_pJtl_class2 /= candidate1_attr1.Count();

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
            allCandidateResults.Add(candidate1_result);
            // CANDIDATE INNER SPLIT -1 {v} {bb,h,j,z,ff,n,dd,o}


            // CANDIDATE INNER SPLIT - 2 {bb} {h,j,z,ff,n,dd,o,v}

            List<trainingSet> candidate2_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST 
            List<trainingSet> candidate2_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST 

            foreach (var row in _context.trainingSet)
            {
                if (row.A9 == "bb")
                {
                    candidate2_attr1.Add(row);

                }
                else
                {

                    candidate2_attr2.Add(row); // other than "bb"

                }
            }

            double candidate2_Pl = (double)candidate2_attr1.Count() / total;
            double candidate2_Pr = (double)candidate2_attr2.Count() / total;

            double candidate2_pJtl_class1 = 0;
            double candidate2_pJtl_class2 = 0;

            double candidate2_pJtr_class1 = 0;
            double candidate2_pJtr_class2 = 0;


            foreach (var element in candidate2_attr1)
            {
                if (element.Class == "good")
                {
                    candidate2_pJtl_class1++;

                }
            }
            candidate2_pJtl_class1 /= candidate2_attr1.Count();


            foreach (var element in candidate2_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate2_pJtl_class2++;

                }

            }
            candidate2_pJtl_class2 /= candidate2_attr1.Count();


            foreach (var element in candidate2_attr2)
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
            allCandidateResults.Add(candidate2_result);

            // CANDIDATE INNER SPLIT - 2 {bb} {h,j,z,ff,n,dd,o,v}

            // CANDIDATE INNER SPLIT - 3 {h} {bb,j,z,ff,n,dd,o,v}

            List<trainingSet> candidate3_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST
            List<trainingSet> candidate3_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST 


            foreach (var row in _context.trainingSet)
            {
                if (row.A9 == "h")
                {
                    candidate3_attr1.Add(row);

                }
                else
                {
                    candidate3_attr2.Add(row); // other than "h"
                }
            }

            double candidate3_Pl = (double)candidate3_attr1.Count() / total;
            double candidate3_Pr = (double)candidate3_attr2.Count() / total;

            double candidate3_pJtl_class1 = 0;
            double candidate3_pJtl_class2 = 0;

            double candidate3_pJtr_class1 = 0;
            double candidate3_pJtr_class2 = 0;

            foreach (var element in candidate3_attr1)
            {
                if (element.Class == "good")
                {
                    candidate3_pJtl_class1++;

                }
            }

            candidate3_pJtl_class1 /= candidate3_attr1.Count();


            foreach (var element in candidate3_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate3_pJtl_class2++;

                }

            }
            candidate3_pJtl_class2 /= candidate3_attr1.Count();


            foreach (var element in candidate3_attr2)
            {
                if (element.Class == "good")
                {
                    candidate3_pJtr_class1++;

                }

            }
            candidate3_pJtr_class1 /= candidate3_attr2.Count();


            foreach (var element in candidate3_attr2)
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
            allCandidateResults.Add(candidate3_result);

            // CANDIDATE INNER SPLIT - 3 {h} {bb,j,z,ff,n,dd,o,v}


            // CANDIDATE INNER SPLIT - 4 {j} {h,bb,z,ff,n,dd,o,v}

            List<trainingSet> candidate4_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST 
            List<trainingSet> candidate4_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST 

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "j")
                {
                    candidate4_attr1.Add(row);

                }
                else
                {
                    candidate4_attr2.Add(row);
                }
            }

            double candidate4_Pl = (double)candidate4_attr1.Count() / total;
            double candidate4_Pr = (double)candidate4_attr2.Count() / total;

            double candidate4_pJtl_class1 = 0;
            double candidate4_pJtl_class2 = 0;

            double candidate4_pJtr_class1 = 0;
            double candidate4_pJtr_class2 = 0;

            foreach (var element in candidate4_attr1)
            {
                if (element.Class == "good")
                {
                    candidate4_pJtl_class1++;

                }
            }

            candidate4_pJtl_class1 /= candidate4_attr1.Count();


            foreach (var element in candidate4_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate4_pJtl_class2++;

                }

            }
            candidate4_pJtl_class2 /= candidate4_attr1.Count();


            foreach (var element in candidate4_attr2)
            {
                if (element.Class == "good")
                {
                    candidate4_pJtr_class1++;

                }

            }
            candidate4_pJtr_class1 /= candidate4_attr2.Count();


            foreach (var element in candidate4_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate4_pJtr_class2++;

                }
            }
            candidate4_pJtr_class2 /= candidate4_attr2.Count();

            double candidate_4_Qst = Math.Abs(candidate4_pJtl_class1 - candidate4_pJtr_class1) +
                Math.Abs(candidate4_pJtl_class2 - candidate4_pJtr_class2);

            double candidate4_result = candidate_4_Qst * 2 * (candidate4_Pl * candidate4_Pr);
            allCandidateResults.Add(candidate4_result);

            // CANDIDATE INNER SPLIT - 4  {j} {h,bb,z,ff,n,dd,o,v}



            // CANDIDATE INNER SPLIT - 5 {z} {j,h,bb,z,ff,n,dd,o,v}


            List<trainingSet> candidate5_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST 
            List<trainingSet> candidate5_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST 

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "z")
                {
                    candidate5_attr1.Add(row);

                }
                else
                {
                    candidate5_attr2.Add(row);
                }
            }

            double candidate5_Pl = (double)candidate5_attr1.Count() / total;
            double candidate5_Pr = (double)candidate5_attr2.Count() / total;

            double candidate5_pJtl_class1 = 0;
            double candidate5_pJtl_class2 = 0;

            double candidate5_pJtr_class1 = 0;
            double candidate5_pJtr_class2 = 0;

            foreach (var element in candidate5_attr1)
            {
                if (element.Class == "good")
                {
                    candidate5_pJtl_class1++;

                }
            }

            candidate5_pJtl_class1 /= candidate5_attr1.Count();


            foreach (var element in candidate5_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate5_pJtl_class2++;

                }

            }
            candidate5_pJtl_class2 /= candidate5_attr1.Count();


            foreach (var element in candidate5_attr2)
            {
                if (element.Class == "good")
                {
                    candidate5_pJtr_class1++;

                }

            }
            candidate5_pJtr_class1 /= candidate5_attr2.Count();


            foreach (var element in candidate5_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate5_pJtr_class2++;

                }
            }
            candidate5_pJtr_class2 /= candidate5_attr2.Count();

            double candidate_5_Qst = Math.Abs(candidate5_pJtl_class1 - candidate5_pJtr_class1) +
                Math.Abs(candidate5_pJtl_class2 - candidate5_pJtr_class2);

            double candidate5_result = candidate_5_Qst * 2 * (candidate5_Pl * candidate5_Pr);
            allCandidateResults.Add(candidate5_result);

            // CANDIDATE INNER SPLIT - 5 {z} {h,bb,z,ff,n,dd,o,v}



            // CANDIDATE INNER SPLIT - 6 {ff} {z,h,bb,z,n,dd,o,v}

            List<trainingSet> candidate6_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST 
            List<trainingSet> candidate6_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST 

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "ff")
                {
                    candidate6_attr1.Add(row);

                }
                else
                {
                    candidate6_attr2.Add(row);
                }
            }

            double candidate6_Pl = (double)candidate6_attr1.Count() / total;
            double candidate6_Pr = (double)candidate6_attr2.Count() / total;

            double candidate6_pJtl_class1 = 0;
            double candidate6_pJtl_class2 = 0;

            double candidate6_pJtr_class1 = 0;
            double candidate6_pJtr_class2 = 0;

            foreach (var element in candidate6_attr1)
            {
                if (element.Class == "good")
                {
                    candidate6_pJtl_class1++;

                }
            }

            candidate6_pJtl_class1 /= candidate6_attr1.Count();


            foreach (var element in candidate6_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate6_pJtl_class2++;

                }

            }
            candidate6_pJtl_class2 /= candidate6_attr1.Count();


            foreach (var element in candidate6_attr2)
            {
                if (element.Class == "good")
                {
                    candidate6_pJtr_class1++;

                }

            }
            candidate6_pJtr_class1 /= candidate6_attr2.Count();


            foreach (var element in candidate6_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate6_pJtr_class2++;

                }
            }
            candidate6_pJtr_class2 /= candidate6_attr2.Count();

            double candidate_6_Qst = Math.Abs(candidate6_pJtl_class1 - candidate6_pJtr_class1) +
                Math.Abs(candidate6_pJtl_class2 - candidate6_pJtr_class2);

            double candidate6_result = candidate_6_Qst * 2 * (candidate6_Pl * candidate6_Pr);
            allCandidateResults.Add(candidate6_result);
            // CANDIDATE INNER SPLIT - 6  {ff} {z,h,bb,z,n,dd,o,v}



            // CANDIDATE INNER SPLIT - 7  {n} {z,h,bb,z,n,dd,o,v}


            List<trainingSet> candidate7_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST 
            List<trainingSet> candidate7_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST 

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "n")
                {
                    candidate7_attr1.Add(row);

                }
                else
                {
                    candidate7_attr2.Add(row);
                }
            }

            double candidate7_Pl = (double)candidate7_attr1.Count() / total;
            double candidate7_Pr = (double)candidate7_attr2.Count() / total;

            double candidate7_pJtl_class1 = 0;
            double candidate7_pJtl_class2 = 0;

            double candidate7_pJtr_class1 = 0;
            double candidate7_pJtr_class2 = 0;

            foreach (var element in candidate7_attr1)
            {
                if (element.Class == "good")
                {
                    candidate7_pJtl_class1++;

                }
            }

            candidate7_pJtl_class1 /= candidate7_attr1.Count();


            foreach (var element in candidate7_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate7_pJtl_class2++;

                }

            }
            candidate7_pJtl_class2 /= candidate7_attr1.Count();


            foreach (var element in candidate7_attr2)
            {
                if (element.Class == "good")
                {
                    candidate7_pJtr_class1++;

                }

            }
            candidate7_pJtr_class1 /= candidate7_attr2.Count();


            foreach (var element in candidate7_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate7_pJtr_class2++;

                }
            }
            candidate7_pJtr_class2 /= candidate7_attr2.Count();

            double candidate_7_Qst = Math.Abs(candidate7_pJtl_class1 - candidate7_pJtr_class1) +
                Math.Abs(candidate7_pJtl_class2 - candidate7_pJtr_class2);

            double candidate7_result = candidate_7_Qst * 2 * (candidate7_Pl * candidate7_Pr);
            allCandidateResults.Add(candidate7_result);
            // CANDIDATE INNER SPLIT - 7 {n} {z,h,bb,z,n,dd,o,v}


            // CANDIDATE INNER SPLIT - 8 {dd} {n,z,h,bb,z,n,o,v}


            List<trainingSet> candidate8_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST 
            List<trainingSet> candidate8_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST 

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "dd")
                {
                    candidate8_attr1.Add(row);

                }
                else
                {
                    candidate8_attr2.Add(row);
                }
            }

            double candidate8_Pl = (double)candidate8_attr1.Count() / total;
            double candidate8_Pr = (double)candidate8_attr2.Count() / total;

            double candidate8_pJtl_class1 = 0;
            double candidate8_pJtl_class2 = 0;

            double candidate8_pJtr_class1 = 0;
            double candidate8_pJtr_class2 = 0;

            foreach (var element in candidate8_attr1)
            {
                if (element.Class == "good")
                {
                    candidate8_pJtl_class1++;

                }
            }

            candidate8_pJtl_class1 /= candidate8_attr1.Count();


            foreach (var element in candidate8_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate8_pJtl_class2++;

                }

            }
            candidate8_pJtl_class2 /= candidate8_attr1.Count();


            foreach (var element in candidate8_attr2)
            {
                if (element.Class == "good")
                {
                    candidate8_pJtr_class1++;

                }

            }
            candidate8_pJtr_class1 /= candidate8_attr2.Count();


            foreach (var element in candidate8_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate8_pJtr_class2++;

                }
            }
            candidate8_pJtr_class2 /= candidate8_attr2.Count();

            double candidate_8_Qst = Math.Abs(candidate8_pJtl_class1 - candidate8_pJtr_class1) +
                Math.Abs(candidate8_pJtl_class2 - candidate8_pJtr_class2);

            double candidate8_result = candidate_8_Qst * 2 * (candidate8_Pl * candidate8_Pr);
            allCandidateResults.Add(candidate8_result);

            // CANDIDATE INNER SPLIT - 8 {dd} {n,z,h,bb,z,n,o,v}


            // CANDIDATE INNER SPLIT - 9 {o} {dd,n,z,h,bb,z,n,o,v}

            List<trainingSet> candidate9_attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST 
            List<trainingSet> candidate9_attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST

            foreach (var row in _context.trainingSet)
            {
                if (row.A5 == "o")
                {
                    candidate9_attr1.Add(row);

                }
                else
                {
                    candidate9_attr2.Add(row);
                }
            }

            double candidate9_Pl = (double)candidate9_attr1.Count() / total;
            double candidate9_Pr = (double)candidate9_attr2.Count() / total;

            double candidate9_pJtl_class1 = 0;
            double candidate9_pJtl_class2 = 0;

            double candidate9_pJtr_class1 = 0;
            double candidate9_pJtr_class2 = 0;

            foreach (var element in candidate9_attr1)
            {
                if (element.Class == "good")
                {
                    candidate9_pJtl_class1++;

                }
            }

            candidate9_pJtl_class1 /= candidate9_attr1.Count();


            foreach (var element in candidate9_attr1)
            {
                if (element.Class == "bad")
                {
                    candidate9_pJtl_class2++;

                }

            }
            candidate9_pJtl_class2 /= candidate9_attr1.Count();


            foreach (var element in candidate9_attr2)
            {
                if (element.Class == "good")
                {
                    candidate9_pJtr_class1++;

                }

            }
            candidate9_pJtr_class1 /= candidate9_attr2.Count();


            foreach (var element in candidate9_attr2)
            {
                if (element.Class == "bad")
                {
                    candidate9_pJtr_class2++;

                }
            }
            candidate9_pJtr_class2 /= candidate9_attr2.Count();

            double candidate_9_Qst = Math.Abs(candidate9_pJtl_class1 - candidate9_pJtr_class1) +
                Math.Abs(candidate9_pJtl_class2 - candidate9_pJtr_class2);

            double candidate9_result = candidate_9_Qst * 2 * (candidate9_Pl * candidate9_Pr);
            allCandidateResults.Add(candidate9_result);
            // CANDIDATE INNER SPLIT - 9 {o} {dd,n,z,h,bb,z,n,o,v}

            return allCandidateResults.Max(); // RETURN THE CANDIDATE WITH HIGHEST VALUE

        }

        public double calculateA7()
        {
            // SPLIT DATASET VALUES FROM MEDIAN VALUE

            int total = _context.trainingSet.Count();
            float avg = 0;

            foreach(var row in _context.trainingSet)
            {
                avg += float.Parse(row.A7, CultureInfo.InvariantCulture.NumberFormat); // FIND SUM OF NUMERICAL VALUES IN ROW A7 VALUES

            }
            avg /= total; // FIND AVERAGE OF NUMERICAL VALUES IN ROW A7

            List<trainingSet> attr1 = new List<trainingSet>(); // ATTRIBUTE 1 LIST "<= average value"
            List<trainingSet> attr2 = new List<trainingSet>(); // ATTRIBUTE 2 LIST "> average value"

            foreach(var row in _context.trainingSet)
            {
                if (float.Parse(row.A7, CultureInfo.InvariantCulture.NumberFormat) <= avg) // IF BELOW AVERAGE -> THEN LEFT NODE
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
