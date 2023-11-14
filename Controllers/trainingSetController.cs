using CART_DECISION_TREE.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
        public candidateValues CALCULATE_A1()
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

            List<candidateValues> candidateValues = new List<candidateValues>();

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

                candidateValues currentCandidate = new candidateValues();

                if ((PJTL_good == 0 && PJTL_bad == 1) || (PJTR_good == 1 && PJTR_bad == 0))   
                {
                    // is leaf = 1 leaf result = bad
                    // if row.A1 == attr1 then class is exactly bad
                    // if row.A1 == node.candidateValue.leafAttribute
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "bad";
                    
                }
                else if((PJTL_good == 1 && PJTL_bad == 0) || (PJTL_good == 1 && PJTL_bad == 0))
                {
                    // is leaf = 1 leaf result = good
                    // if row.A1 == attr1 then class is exactly good
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "good";

                }
                

                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                
                currentCandidate.Ax = "A1";
                currentCandidate.LeftVal = elements[i];
                currentCandidate.ϕ = RESULT;
                
                candidateValues.Add(currentCandidate);
            
            }

            candidateValues MAX = candidateValues[0];

            for (int i=1 ;i<candidateValues.Count();++i)
            {
                if (candidateValues[i].ϕ > MAX.ϕ)
                {
                    MAX = candidateValues[i];

                }
                
            }

            return MAX;

             
        }

        [HttpGet("CALCULATE A2")]
        public candidateValues CALCULATE_A2()
        {
            var set = _context.trainingSet.ToList(); // ALL RECORDS

            int total = _context.trainingSet.Count();

            HashSet<String> hashset = new HashSet<string>(); // HASH SET USED TO STORE UNIQUE VALUES FROM DATASET
                                                             // IN ORDER TO GET CANDIDATE INNER SPLITS
            foreach (var row in _context.trainingSet)
            {
                hashset.Add(row.A2);

            }

            List<string> elements = hashset.ToList();

            List<candidateValues> candidateValues = new List<candidateValues>();

            for (int i = 0; i < elements.Count; ++i)
            {
                string L = elements[i]; // LEFT NODE (for round i)
                double lCount = _context.trainingSet.Where(e => e.A2 == L).Count();
                double PL = lCount / total;
                double PR = Math.Abs(1 - PL);

                double count1 = _context.trainingSet.Where(u => u.A2 == L && u.Class == "good").Count();
                double PJTL_good = count1
                    / _context.trainingSet.Where(u => u.A2 == L).Count();

                double PJTL_bad = Math.Abs(1 - PJTL_good);


                double count2 = _context.trainingSet.Where(u => u.A2 != L && u.Class == "good").Count();
                double PJTR_good = count2
                    / _context.trainingSet.Where(u => u.A2 != L).Count();

                double PJTR_bad = Math.Abs(1 - PJTR_good);

                candidateValues currentCandidate = new candidateValues();

                if ((PJTL_good == 0 && PJTL_bad == 1) || (PJTR_good == 1 && PJTR_bad == 0))
                {
                    // is leaf = 1 leaf result = bad
                    // if row.A1 == attr1 then class is exactly bad
                    // if row.A1 == node.candidateValue.leafAttribute
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "bad";

                }
                else if ((PJTL_good == 1 && PJTL_bad == 0) || (PJTL_good == 1 && PJTL_bad == 0))
                {
                    // is leaf = 1 leaf result = good
                    // if row.A1 == attr1 then class is exactly good
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "good";

                }

                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                
                currentCandidate.Ax = "A2";
                currentCandidate.LeftVal = elements[i];
                currentCandidate.ϕ = RESULT;

                candidateValues.Add(currentCandidate);

            }

            candidateValues MAX = candidateValues[0];

            for (int i = 1; i < candidateValues.Count(); ++i)
            {
                if (candidateValues[i].ϕ > MAX.ϕ)
                {
                    MAX = candidateValues[i];

                }

            }

            return MAX;


        }


        [HttpGet("CALCULATE A3")]
        public candidateValues CALCULATE_A3()
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

            List<candidateValues> candidateValues = new List<candidateValues>();

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

                candidateValues currentCandidate = new candidateValues();

                if ((PJTL_good == 0 && PJTL_bad == 1) || (PJTR_good == 1 && PJTR_bad == 0))
                {
                    // is leaf = 1 leaf result = bad
                    // if row.A1 == attr1 then class is exactly bad
                    // if row.A1 == node.candidateValue.leafAttribute
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "bad";

                }
                else if ((PJTL_good == 1 && PJTL_bad == 0) || (PJTL_good == 1 && PJTL_bad == 0))
                {
                    // is leaf = 1 leaf result = good
                    // if row.A1 == attr1 then class is exactly good
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "good";

                }

                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                
                currentCandidate.Ax = "A3";
                currentCandidate.LeftVal = elements[i];
                currentCandidate.ϕ = RESULT;

                candidateValues.Add(currentCandidate);

                

            }

            candidateValues MAX = candidateValues[0];

            for (int i = 1; i < candidateValues.Count(); ++i)
            {
                if (candidateValues[i].ϕ > MAX.ϕ)
                {
                    MAX = candidateValues[i];

                }

            }

            return MAX;







        }


        [HttpGet("CALCULATE A4")]
        public candidateValues CALCULATE_A4()
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

            List<candidateValues> candidateValues = new List<candidateValues>();

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

                candidateValues currentCandidate = new candidateValues();

                if ((PJTL_good == 0 && PJTL_bad == 1) || (PJTR_good == 1 && PJTR_bad == 0))
                {
                    // is leaf = 1 leaf result = bad
                    // if row.A1 == attr1 then class is exactly bad
                    // if row.A1 == node.candidateValue.leafAttribute
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "bad";

                }
                else if ((PJTL_good == 1 && PJTL_bad == 0) || (PJTL_good == 1 && PJTL_bad == 0))
                {
                    // is leaf = 1 leaf result = good
                    // if row.A1 == attr1 then class is exactly good
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "good";

                }


                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                
                currentCandidate.Ax = "A4";
                currentCandidate.LeftVal = elements[i];
                currentCandidate.ϕ = RESULT;

                candidateValues.Add(currentCandidate);

            }


            candidateValues MAX = candidateValues[0];

            for (int i = 1; i < candidateValues.Count(); ++i)
            {
                if (candidateValues[i].ϕ > MAX.ϕ)
                {
                    MAX = candidateValues[i];

                }

            }

            return MAX;


        }


        [HttpGet("CALCULATE A5")]
        public candidateValues CALCULATE_A5()
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

            List<candidateValues> candidateValues = new List<candidateValues>();

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

                candidateValues currentCandidate = new candidateValues();

                if ((PJTL_good == 0 && PJTL_bad == 1) || (PJTR_good == 1 && PJTR_bad == 0))
                {
                    // is leaf = 1 leaf result = bad
                    // if row.A1 == attr1 then class is exactly bad
                    // if row.A1 == node.candidateValue.leafAttribute
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "bad";

                }
                else if ((PJTL_good == 1 && PJTL_bad == 0) || (PJTL_good == 1 && PJTL_bad == 0))
                {
                    // is leaf = 1 leaf result = good
                    // if row.A1 == attr1 then class is exactly good
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "good";

                }


                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

               
                currentCandidate.Ax = "A5";
                currentCandidate.LeftVal = elements[i];
                currentCandidate.ϕ = RESULT;

                candidateValues.Add(currentCandidate);

            }

            candidateValues MAX = candidateValues[0];

            for (int i = 1; i < candidateValues.Count(); ++i)
            {
                if (candidateValues[i].ϕ > MAX.ϕ)
                {
                    MAX = candidateValues[i];

                }

            }

            return MAX;


        }

        [HttpGet("CALCULATE A6")]
        public candidateValues CALCULATE_A6()
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

            List<candidateValues> candidateValues = new List<candidateValues>();

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

                candidateValues currentCandidate = new candidateValues();

                if ((PJTL_good == 0 && PJTL_bad == 1) || (PJTR_good == 1 && PJTR_bad == 0))
                {
                    // is leaf = 1 leaf result = bad
                    // if row.A1 == attr1 then class is exactly bad
                    // if row.A1 == node.candidateValue.leafAttribute
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "bad";

                }
                else if ((PJTL_good == 1 && PJTL_bad == 0) || (PJTL_good == 1 && PJTL_bad == 0))
                {
                    // is leaf = 1 leaf result = good
                    // if row.A1 == attr1 then class is exactly good
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "good";

                }


                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;


                
                currentCandidate.Ax = "A6";
                currentCandidate.LeftVal = elements[i];
                currentCandidate.ϕ = RESULT;

                candidateValues.Add(currentCandidate);

            }

            candidateValues MAX = candidateValues[0];

            for (int i = 1; i < candidateValues.Count(); ++i)
            {
                if (candidateValues[i].ϕ > MAX.ϕ)
                {
                    MAX = candidateValues[i];

                }

            }

            return MAX;


        }

        [HttpGet("CALCULATE A7")]
        public candidateValues CALCULATE_A7()
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

            List<candidateValues> candidateValues = new List<candidateValues>();

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

                candidateValues currentCandidate = new candidateValues();

                if ((PJTL_good == 0 && PJTL_bad == 1) || (PJTR_good == 1 && PJTR_bad == 0))
                {
                    // is leaf = 1 leaf result = bad
                    // if row.A1 == attr1 then class is exactly bad
                    // if row.A1 == node.candidateValue.leafAttribute
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "bad";

                }
                else if ((PJTL_good == 1 && PJTL_bad == 0) || (PJTL_good == 1 && PJTL_bad == 0))
                {
                    // is leaf = 1 leaf result = good
                    // if row.A1 == attr1 then class is exactly good
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "good";

                }


                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                
                currentCandidate.Ax = "A7";
                currentCandidate.LeftVal = elements[i];
                currentCandidate.ϕ = RESULT;

                candidateValues.Add(currentCandidate);

            }

            candidateValues MAX = candidateValues[0];

            for (int i = 1; i < candidateValues.Count(); ++i)
            {
                if (candidateValues[i].ϕ > MAX.ϕ)
                {
                    MAX = candidateValues[i];

                }

            }

            return MAX;


        }


        [HttpGet("CALCULATE A8")]
        public candidateValues CALCULATE_A8()
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

            List<candidateValues> candidateValues = new List<candidateValues>();

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

                candidateValues currentCandidate = new candidateValues();

                if ((PJTL_good == 0 && PJTL_bad == 1) || (PJTR_good == 1 && PJTR_bad == 0))
                {
                    // is leaf = 1 leaf result = bad
                    // if row.A1 == attr1 then class is exactly bad
                    // if row.A1 == node.candidateValue.leafAttribute
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "bad";

                }
                else if ((PJTL_good == 1 && PJTL_bad == 0) || (PJTL_good == 1 && PJTL_bad == 0))
                {
                    // is leaf = 1 leaf result = good
                    // if row.A1 == attr1 then class is exactly good
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "good";

                }


                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                
                currentCandidate.Ax = "A8";
                currentCandidate.LeftVal = elements[i];
                currentCandidate.ϕ = RESULT;

                candidateValues.Add(currentCandidate);

            }

            candidateValues MAX = candidateValues[0];

            for (int i = 1; i < candidateValues.Count(); ++i)
            {
                if (candidateValues[i].ϕ > MAX.ϕ)
                {
                    MAX = candidateValues[i];

                }

            }

            return MAX;


        }


        [HttpGet("CALCULATE A9")]
        public candidateValues CALCULATE_A9()
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

            List<candidateValues> candidateValues = new List<candidateValues>();

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

                candidateValues currentCandidate = new candidateValues();

                if ((PJTL_good == 0 && PJTL_bad == 1) || (PJTR_good == 1 && PJTR_bad == 0))
                {
                    // is leaf = 1 leaf result = bad
                    // if row.A1 == attr1 then class is exactly bad
                    // if row.A1 == node.candidateValue.leafAttribute
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "bad";

                }
                else if ((PJTL_good == 1 && PJTL_bad == 0) || (PJTL_good == 1 && PJTL_bad == 0))
                {
                    // is leaf = 1 leaf result = good
                    // if row.A1 == attr1 then class is exactly good
                    currentCandidate.isLeaf = true;
                    currentCandidate.leafAttribute = elements[i];
                    currentCandidate.leafResult = "good";

                }



                double QST = Math.Abs(PJTL_good - PJTR_good) + Math.Abs(PJTL_bad - PJTR_bad);

                double RESULT = 2 * PL * PR * QST;

                
                currentCandidate.Ax = "A9";
                currentCandidate.LeftVal = elements[i];
                currentCandidate.ϕ = RESULT;

                candidateValues.Add(currentCandidate);

            }

            candidateValues MAX = candidateValues[0];

            for (int i = 1; i < candidateValues.Count(); ++i)
            {
                if (candidateValues[i].ϕ > MAX.ϕ)
                {
                    MAX = candidateValues[i];

                }

            }

            return MAX;


        }

        [HttpGet("TRAIN MODEL")]
        public List<string> TRAIN_MODEL()
        {
            List<string> predictions = new List<string>();

            binaryTree tree = createDecisionTreeModel();

            candidateValues value1 = new candidateValues();

            Node node = tree.Root;

            
            foreach (var row in _context.trainingSet)
            {
                var property = row.GetType().GetProperty(tree.Root.candidateValue.Ax); // INITIAL SPLIT (ACCORDING TO WHICH ROW ?) FROM ROOT 
                var value = property.GetValue(row, null); // GET ATTRIBUTE VALUE

                if (value == tree.Root.candidateValue.LeftVal) 
                {
                    if (node.candidateValue.isLeaf == true)
                    {
                        predictions.Add(node.candidateValue.leafResult);
                    }
                    else
                    {
                        if (node.Left!=null)
                        {
                            node = node.Left;


                        }
                        else
                        {
                            predictions.Add(node.candidateValue.leafResult);
                        }



                    }
                }
                else
                {
                    if (node.candidateValue.isLeaf == true)
                    {
                        predictions.Add(node.candidateValue.leafResult);

                    }

                    else
                    {
                        if (node.Right != null)
                        {
                           node = node.Right;
                        }
                        else
                        {
                            predictions.Add(node.candidateValue.leafResult);
                        }
                        

                    }
                    
                }

                while (true)
                {
                    if (node.Left != null || node.Right != null)
                    {
                        property = row.GetType().GetProperty(node.candidateValue.Ax);
                        value = property.GetValue(row, null);                    

                        if (value == node.candidateValue.LeftVal)
                        {
                            if (node.candidateValue.isLeaf == true) // IF LEAF NODE THEN ADD PREDICTION THEN END
                            {
                                predictions.Add(node.candidateValue.leafResult);
                                continue;
                            }

                            else
                            {
                                if (node.Left != null)
                                {

                                    node = node.Left;

                                }
                                else
                                {
                                    continue;
                                }

                            }

                            

                        }
                        else
                        {
                            if (node.Right != null)
                            {
                                if (node.candidateValue.isLeaf == true)
                                {
                                    predictions.Add(node.candidateValue.leafResult);
                                    continue;
                                }
                                else
                                {
                                    node = node.Right;
                                }
                                
                               
                            }
                            else
                            {
                                continue;
                            }
                            
                        }

                    }
                    else
                    {
                        predictions.Add(row.Class);
                        break;
                        
                    }
                }

            }

            //System.Diagnostics.Debug.Write(string.Join("\n", predictions));
            return predictions;
        }



        [HttpGet("createDecisionTreeModel")]
        public binaryTree createDecisionTreeModel()
        {
            binaryTree tree = new binaryTree();
            
            List<candidateValues> calculations = new List<candidateValues>();

            calculations.Add(CALCULATE_A1());
            calculations.Add(CALCULATE_A2());
            calculations.Add(CALCULATE_A3());
            calculations.Add(CALCULATE_A4());
            calculations.Add(CALCULATE_A5());
            calculations.Add(CALCULATE_A6());
            calculations.Add(CALCULATE_A7());
            calculations.Add(CALCULATE_A8());
            calculations.Add(CALCULATE_A9());

            for (int i=0; i<calculations.Count();++i)
            {
                tree.Add(tree,calculations[i]);

            }

            binaryTree.PrintTree(tree);


            return tree;





        }


        [HttpGet("getAllTrainingData")]
        public async Task<List<trainingSet>> getAllData()
        {
            var data = _context.trainingSet.ToListAsync();
            return await data;
        }

       

    }
}
