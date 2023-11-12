using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.X86;

namespace CART_DECISION_TREE
{
    [ApiController]
    public class trainingSetController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private DBContext _context;
        

        public trainingSetController(DBContext context)
        {
            this._context = context;
           

        }


        static List<string> GetPermutations(string input)
        {
            List<string> permutations = new List<string>();
            HashSet<string> visited = new HashSet<string>();
            GeneratePermutations("", input, visited, permutations);
            return permutations;
        }
        static void GeneratePermutations(string prefix, string remaining, HashSet<string> visited, List<string> permutations)
        {
            if (remaining.Length == 0)
            {
                if (!visited.Contains(prefix))
                {
                    permutations.Add(prefix);
                    visited.Add(prefix);
                }
                return;
            }

            for (int i = 0; i < remaining.Length; i++)
            {
                string newPrefix = prefix + remaining[i];
                string newRemaining = remaining.Remove(i, 1);
                GeneratePermutations(newPrefix, newRemaining, visited, permutations);
            }
        }








        [HttpGet("GET CANDIDATES")]
        public HashSet<String> GET_CANDIDATES()
        {
            HashSet<String> result = new HashSet<string>(); 
            foreach(var row in _context.trainingSet)
            {
                result.Add(row.A9);

            }
            return result;
        }
        

        [HttpGet("CALCULATE A1")]
        public double CALCULATE_A1()
        {
            var set = _context.trainingSet.ToList(); // ALL RECORDS

            int total = _context.trainingSet.Count();

            HashSet<String> hashset = new HashSet<string>(); // HASH SET USED TO STORE UNIQUE VALUES FROM DATASET
                                                             // IN ORDER TO GET CANDIDATE INNER SPLITS
            foreach (var row in _context.trainingSet)
            {
                hashset.Add(row.A1);                     

            }

            List<string> elements = hashset.ToList();

            List<double> candidateValues = new List<double>();

            for (int i=0;i<elements.Count;++i)
            {
                string L = elements[i]; // LEFT NODE (for round i)
                double lCount = _context.trainingSet.Where(e => e.A1 == L).Count();
                double PL = lCount/ total;
                double PR = Math.Abs(1 - PL);

                double count1 = _context.trainingSet.Where(u => u.A1 == L && u.Class == "good").Count();
                double PJTL_good = count1
                    / _context.trainingSet.Where(u => u.A1 == L).Count();

                double PJTL_bad = Math.Abs(1 - PJTL_good);


                double count2 = _context.trainingSet.Where(u => u.A1 != L && u.Class == "good").Count();
                double PJTR_good = count2
                    /_context.trainingSet.Where(u => u.A1 != L ).Count();

                double PJTR_bad = Math.Abs(1 - PJTR_good);

                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                candidateValues.Add(RESULT);
            
            }

            return candidateValues.Max();  
        }

        [HttpGet("CALCULATE A2")]
        public double CALCULATE_A2()
        {
            var set = _context.trainingSet.ToList(); // ALL RECORDS

            int total = _context.trainingSet.Count();

            double avg = 0; // SPLIT ACCORDING TO MEDIAN VALUE L: <= R: >
                                                             
            foreach (var row in _context.trainingSet)
            {
                avg += double.Parse(row.A2,CultureInfo.InvariantCulture.NumberFormat);  // SUM OF ALL A2 VALUES

            }

            avg /= total;  // SUM OF ALL A2 / TOTAL = MEDIAN VALUE

            List<trainingSet> leftElements = new List<trainingSet>();
            List<trainingSet> rightElements = new List<trainingSet>();

           foreach (var row in _context.trainingSet)
           {
                if (double.Parse(row.A2) <= avg)
                {
                    leftElements.Add(row);
                }

                else
                {
                    rightElements.Add(row);

                }
                

           }

            double lCount = leftElements.Count();

            double PL = lCount / total;
            double PR = Math.Abs(1 - PL);

            double count1 = 0;

            for (int i=0;i<leftElements.Count();++i)
            {
                if (leftElements[i].Class == "good")
                {
                    count1++;
                }
            }

            double PJTL_good = count1
                    / leftElements.Count();

            double PJTL_bad = Math.Abs(1 - PJTL_good);


            double count2 = 0;

            for (int i = 0; i < rightElements.Count(); ++i)
            {
                if (rightElements[i].Class == "good")
                {
                    count2++;
                }
            }

            double PJTR_good = count2
                    / rightElements.Count();

            double PJTR_bad = Math.Abs(1 - PJTR_good);

            double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

            double RESULT = 2 * PL * PR * QST;


            return RESULT;
           

        }


        [HttpGet("CALCULATE A3")]
        public double CALCULATE_A3()
        {
            var set = _context.trainingSet.ToList(); // ALL RECORDS

            int total = _context.trainingSet.Count();

            HashSet<String> hashset = new HashSet<string>(); // HASH SET USED TO STORE UNIQUE VALUES FROM DATASET
                                                             // IN ORDER TO GET CANDIDATE INNER SPLITS
            foreach (var row in _context.trainingSet)
            {
                hashset.Add(row.A3);

            }

            List<string> elements = hashset.ToList();

            List<double> candidateValues = new List<double>();

            for (int i = 0; i < elements.Count; ++i)
            {
                string L = elements[i]; // LEFT NODE (for round i)
                double lCount = _context.trainingSet.Where(e => e.A3 == L).Count();
                double PL = lCount / total;
                double PR = Math.Abs(1 - PL);

                double count1 = _context.trainingSet.Where(u => u.A3 == L && u.Class == "good").Count();
                double PJTL_good = count1
                    / _context.trainingSet.Where(u => u.A3 == L).Count();

                double PJTL_bad = Math.Abs(1 - PJTL_good);


                double count2 = _context.trainingSet.Where(u => u.A3 != L && u.Class == "good").Count();
                double PJTR_good = count2
                    / _context.trainingSet.Where(u => u.A3 != L).Count();

                double PJTR_bad = Math.Abs(1 - PJTR_good);

                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                candidateValues.Add(RESULT);

            }

            return candidateValues.Max();


        }


        [HttpGet("CALCULATE A4")]
        public double CALCULATE_A4()
        {
            var set = _context.trainingSet.ToList(); // ALL RECORDS

            int total = _context.trainingSet.Count();

            HashSet<String> hashset = new HashSet<string>(); // HASH SET USED TO STORE UNIQUE VALUES FROM DATASET
                                                             // IN ORDER TO GET CANDIDATE INNER SPLITS
            foreach (var row in _context.trainingSet)
            {
                hashset.Add(row.A4);

            }

            List<string> elements = hashset.ToList();

            List<double> candidateValues = new List<double>();

            for (int i = 0; i < elements.Count; ++i)
            {
                string L = elements[i]; // LEFT NODE (for round i)
                double lCount = _context.trainingSet.Where(e => e.A4 == L).Count();
                double PL = lCount / total;
                double PR = Math.Abs(1 - PL);

                double count1 = _context.trainingSet.Where(u => u.A4 == L && u.Class == "good").Count();
                double PJTL_good = count1
                    / _context.trainingSet.Where(u => u.A4 == L).Count();

                double PJTL_bad = Math.Abs(1 - PJTL_good);


                double count2 = _context.trainingSet.Where(u => u.A4 != L && u.Class == "good").Count();
                double PJTR_good = count2
                    / _context.trainingSet.Where(u => u.A4 != L).Count();

                double PJTR_bad = Math.Abs(1 - PJTR_good);

                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                candidateValues.Add(RESULT);

            }

            return candidateValues.Max();


        }


        [HttpGet("CALCULATE A5")]
        public double CALCULATE_A5()
        {
            var set = _context.trainingSet.ToList(); // ALL RECORDS

            int total = _context.trainingSet.Count();

            HashSet<String> hashset = new HashSet<string>(); // HASH SET USED TO STORE UNIQUE VALUES FROM DATASET
                                                             // IN ORDER TO GET CANDIDATE INNER SPLITS
            foreach (var row in _context.trainingSet)
            {
                hashset.Add(row.A5);

            }

            List<string> elements = hashset.ToList();

            List<double> candidateValues = new List<double>();

            for (int i = 0; i < elements.Count; ++i)
            {
                string L = elements[i]; // LEFT NODE (for round i)
                double lCount = _context.trainingSet.Where(e => e.A5 == L).Count();
                double PL = lCount / total;
                double PR = Math.Abs(1 - PL);

                double count1 = _context.trainingSet.Where(u => u.A5 == L && u.Class == "good").Count();
                double PJTL_good = count1
                    / _context.trainingSet.Where(u => u.A5 == L).Count();

                double PJTL_bad = Math.Abs(1 - PJTL_good);


                double count2 = _context.trainingSet.Where(u => u.A5 != L && u.Class == "good").Count();
                double PJTR_good = count2
                    / _context.trainingSet.Where(u => u.A5 != L).Count();

                double PJTR_bad = Math.Abs(1 - PJTR_good);

                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                candidateValues.Add(RESULT);

            }

            return candidateValues.Max();


        }

        [HttpGet("CALCULATE A6")]
        public double CALCULATE_A6()
        {
            var set = _context.trainingSet.ToList(); // ALL RECORDS

            int total = _context.trainingSet.Count();

            HashSet<String> hashset = new HashSet<string>(); // HASH SET USED TO STORE UNIQUE VALUES FROM DATASET
                                                             // IN ORDER TO GET CANDIDATE INNER SPLITS
            foreach (var row in _context.trainingSet)
            {
                hashset.Add(row.A6);

            }

            List<string> elements = hashset.ToList();

            List<double> candidateValues = new List<double>();

            for (int i = 0; i < elements.Count; ++i)
            {
                string L = elements[i]; // LEFT NODE (for round i)
                double lCount = _context.trainingSet.Where(e => e.A6 == L).Count();
                double PL = lCount / total;
                double PR = Math.Abs(1 - PL);

                double count1 = _context.trainingSet.Where(u => u.A6 == L && u.Class == "good").Count();
                double PJTL_good = count1
                    / _context.trainingSet.Where(u => u.A6 == L).Count();

                double PJTL_bad = Math.Abs(1 - PJTL_good);


                double count2 = _context.trainingSet.Where(u => u.A6 != L && u.Class == "good").Count();
                double PJTR_good = count2
                    / _context.trainingSet.Where(u => u.A6 != L).Count();

                double PJTR_bad = Math.Abs(1 - PJTR_good);

                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                candidateValues.Add(RESULT);

            }

            return candidateValues.Max();


        }

        [HttpGet("CALCULATE A7")]
        public double CALCULATE_A7()
        {
            var set = _context.trainingSet.ToList(); // ALL RECORDS

            int total = _context.trainingSet.Count();

            HashSet<String> hashset = new HashSet<string>(); // HASH SET USED TO STORE UNIQUE VALUES FROM DATASET
                                                             // IN ORDER TO GET CANDIDATE INNER SPLITS
            foreach (var row in _context.trainingSet)
            {
                hashset.Add(row.A7);

            }

            List<string> elements = hashset.ToList();

            List<double> candidateValues = new List<double>();

            for (int i = 0; i < elements.Count; ++i)
            {
                string L = elements[i]; // LEFT NODE (for round i)
                double lCount = _context.trainingSet.Where(e => e.A7 == L).Count();
                double PL = lCount / total;
                double PR = Math.Abs(1 - PL);

                double count1 = _context.trainingSet.Where(u => u.A7 == L && u.Class == "good").Count();
                double PJTL_good = count1
                    / _context.trainingSet.Where(u => u.A7 == L).Count();

                double PJTL_bad = Math.Abs(1 - PJTL_good);


                double count2 = _context.trainingSet.Where(u => u.A7 != L && u.Class == "good").Count();
                double PJTR_good = count2
                    / _context.trainingSet.Where(u => u.A7 != L).Count();

                double PJTR_bad = Math.Abs(1 - PJTR_good);

                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                candidateValues.Add(RESULT);

            }

            return candidateValues.Max();


        }


        [HttpGet("CALCULATE A8")]
        public double CALCULATE_A8()
        {
            var set = _context.trainingSet.ToList(); // ALL RECORDS

            int total = _context.trainingSet.Count();

            HashSet<String> hashset = new HashSet<string>(); // HASH SET USED TO STORE UNIQUE VALUES FROM DATASET
                                                             // IN ORDER TO GET CANDIDATE INNER SPLITS
            foreach (var row in _context.trainingSet)
            {
                hashset.Add(row.A8);

            }

            List<string> elements = hashset.ToList();

            List<double> candidateValues = new List<double>();

            for (int i = 0; i < elements.Count; ++i)
            {
                string L = elements[i]; // LEFT NODE (for round i)
                double lCount = _context.trainingSet.Where(e => e.A8 == L).Count();
                double PL = lCount / total;
                double PR = Math.Abs(1 - PL);

                double count1 = _context.trainingSet.Where(u => u.A8 == L && u.Class == "good").Count();
                double PJTL_good = count1
                    / _context.trainingSet.Where(u => u.A8 == L).Count();

                double PJTL_bad = Math.Abs(1 - PJTL_good);


                double count2 = _context.trainingSet.Where(u => u.A8 != L && u.Class == "good").Count();
                double PJTR_good = count2
                    / _context.trainingSet.Where(u => u.A8 != L).Count();

                double PJTR_bad = Math.Abs(1 - PJTR_good);

                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                candidateValues.Add(RESULT);

            }

            return candidateValues.Max();


        }


        [HttpGet("CALCULATE A9")]
        public double CALCULATE_A9()
        {
            var set = _context.trainingSet.ToList(); // ALL RECORDS

            int total = _context.trainingSet.Count();

            HashSet<String> hashset = new HashSet<string>(); // HASH SET USED TO STORE UNIQUE VALUES FROM DATASET
                                                             // IN ORDER TO GET CANDIDATE INNER SPLITS
            foreach (var row in _context.trainingSet)
            {
                hashset.Add(row.A9);

            }

            List<string> elements = hashset.ToList();

            List<double> candidateValues = new List<double>();

            for (int i = 0; i < elements.Count; ++i)
            {
                string L = elements[i]; // LEFT NODE (for round i)
                double lCount = _context.trainingSet.Where(e => e.A9 == L).Count();
                double PL = lCount / total;
                double PR = Math.Abs(1 - PL);

                double count1 = _context.trainingSet.Where(u => u.A9 == L && u.Class == "good").Count();
                double PJTL_good = count1
                    / _context.trainingSet.Where(u => u.A9 == L).Count();

                double PJTL_bad = Math.Abs(1 - PJTL_good);


                double count2 = _context.trainingSet.Where(u => u.A9 != L && u.Class == "good").Count();
                double PJTR_good = count2
                    / _context.trainingSet.Where(u => u.A9 != L).Count();

                double PJTR_bad = Math.Abs(1 - PJTR_good);

                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                candidateValues.Add(RESULT);

            }

            return candidateValues.Max();


        }







        [HttpGet("createDecisionTreeModel")]
        public List<double> createDecisionTreeModel()
        {
            List <double> results = new List<double>();


            var decisionTree = new binaryTree<String>(); // INITIALIZE A BINARY DECISION TREE

            decisionTree.Add(Math.Round(results.Max(),4).ToString()); // ROOT OF THE TREE (ACCORDING TO HIGHEST Φ VALUE AFTER CALCULATIONS)

            Random random = new Random();

            
            for (int i=0;i<9;++i)
            {
                if (results[i] != results.Max())
                {
                    decisionTree.Add(Math.Round(results[i], 4).ToString());
                    //decisionTree.Add(""+random.Next(1,100));

                }
                


            }

     
            Node<String>.print2D(decisionTree.Root);

            return results;



        }


        [HttpGet("getAllTrainingData")]
        public async Task<List<trainingSet>> getAllData()
        {
            var data = _context.trainingSet.ToListAsync();
            return await data;
        }

       

    }
}
