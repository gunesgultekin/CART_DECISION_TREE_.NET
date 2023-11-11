using Microsoft.AspNetCore.Mvc;
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
        private ItrainingSetRepository _trainingSetRepository;

        public trainingSetController(DBContext context,ItrainingSetRepository trainingSetRepository)
        {
            this._context = context;
            this._trainingSetRepository = trainingSetRepository;

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



            /*
            //string input = string.Join("", hashset);
            //List<string> permutations = GetPermutations(input);
            string input = "qwerty";
           Console.WriteLine("All permutations:");
            foreach (var permutation in permutations)
            {
                System.Diagnostics.Debug.Write(permutation+"   ");
            }
            */



















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






        [HttpGet("createDecisionTreeModel")]
        public List<double> createDecisionTreeModel()
        {
            List <double> results = new List<double>();

            results.Add(_trainingSetRepository.calculateA1());
            results.Add(_trainingSetRepository.calculateA2());
            results.Add(_trainingSetRepository.calculateA3());
            results.Add(_trainingSetRepository.calculateA4());
            results.Add(_trainingSetRepository.calculateA5());
            results.Add(_trainingSetRepository.calculateA6());
            results.Add(_trainingSetRepository.calculateA7());
            results.Add(_trainingSetRepository.calculateA8());
            results.Add(_trainingSetRepository.calculateA9());

           
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
            return await _trainingSetRepository.getAllData();


        }

        [HttpGet("getCandidateSplits")]
        public HashSet<String> getCandidateSplits()
        {
            return _trainingSetRepository.getCandidateSplits(); 


        }

       
        
        [HttpGet("calculateA1")]
        public double calculateA1()
        {
            return _trainingSetRepository.calculateA1();

        }

        [HttpGet("calculateA2")]
        public double calculateA2()
        {
            return _trainingSetRepository.calculateA2();

        }

        [HttpGet("calculateA3")]
        public double calculateA3()
        {
            return _trainingSetRepository.calculateA3();

        }

        [HttpGet("calculateA4")]
        public double calculateA4()
        {
            return _trainingSetRepository.calculateA4();

        }

        [HttpGet("calculateA5")]
        public double calculateA5()
        {
            return _trainingSetRepository.calculateA5();

        }


        [HttpGet("calculateA6")]
        public double calculateA6()
        {
            return _trainingSetRepository.calculateA6();

        }


        [HttpGet("calculateA7")]
        public double calculateA7()
        {
            return _trainingSetRepository.calculateA7();

        }

        [HttpGet("calculateA8")]
        public double calculateA8()
        {
            return _trainingSetRepository.calculateA8();

        }

        [HttpGet("calculateA9")]
        public double calculateA9()
        {
            return _trainingSetRepository.calculateA9();

        }

        [HttpGet("count")]
        public int count()
        {
            return _trainingSetRepository.count();
        }

    }
}
