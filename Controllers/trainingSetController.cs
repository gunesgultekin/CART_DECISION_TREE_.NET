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
using System.Xml.Linq;

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



        [HttpGet("calculateTrainingPerformanceScores")]
        public List<double> calculateTrainingPerformanceScores(List<string> predictions)
        {
            double accuracy;
            double recall;
            double TN_rate;
            double precision;
            double f_score;
            double TP_total=0;
            double TN_total=0;
            double FP_total = 0;
            double FN_total = 0;

            List<trainingSet> trainingSet = _context.trainingSet.ToList();

            for (int i=0; i<predictions.Count();++i)
            {
                
                
                    if (predictions[i] == trainingSet[i].Class) // IF PREDICTED CLASS == REAL TRAINING SET CLASS
                    {
                        if (predictions[i] == "good") // IF PREDICTION IS TRUE AND class "positive" TP
                        {
                            TP_total++;
                        }
                        else // IF PREDICTION IS TRUE AND class "negative" TN
                        {
                            TN_total++;
                        }

                    }

                    else
                    {
                        if (predictions[i] == "good") // IF PREDICTION IS FALSE AND class "positive" FP
                        {
                            FP_total++;
                        }
                        else // IF PREDICTION IS FALSE AND class "negative" FN
                        {
                            FN_total++;
                        }

                    }
                

            }

            accuracy = ((TP_total + TN_total) / predictions.Count());

            precision = (TP_total / (TP_total + FP_total));

            double a = TP_total + FN_total;
            recall = TP_total / a;

            f_score = 2 * (precision * recall) / (precision + recall);



            
            List<double> RESULTS = new List<double>();

            RESULTS.Add(accuracy);
            RESULTS.Add(recall);
            RESULTS.Add(precision);
            RESULTS.Add(f_score);
            RESULTS.Add(TP_total);
            RESULTS.Add(TN_total);

            return RESULTS;
        }


        [HttpGet("calculateTestPerformanceScores")]
        public List<double>  calculateTestPerformanceScores(List<string> predictions)
        {
            double accuracy;
            double recall;
            double TN_rate;
            double precision;
            double f_score;
            double TP_total = 0;
            double TN_total = 0;
            double FP_total = 0;
            double FN_total = 0;

            List<testSet> testSet = _context.testSet.ToList();

            for (int i = 0; i < predictions.Count(); ++i)
            {
                if (predictions[i] == testSet[i].Class) // IF PREDICTED CLASS == REAL TRAINING SET CLASS
                {
                    if (predictions[i] == "good") // IF PREDICTION IS TRUE AND class "positive" TP
                    {
                        TP_total++;
                    }
                    else // IF PREDICTION IS TRUE AND class "negative" TN
                    {
                        TN_total++;
                    }

                }

                else
                {
                    if (predictions[i] == "good") // IF PREDICTION IS FALSE AND class "positive" FP
                    {
                        FP_total++;
                    }
                    else // IF PREDICTION IS FALSE AND class "negative" FN
                    {
                        FN_total++;
                    }

                }


            }

            accuracy = ((TP_total + TN_total) / predictions.Count());

            precision = (TP_total / (TP_total + FP_total));

            double a = TP_total + FN_total;
            recall = TP_total / a;

            f_score = 2 * (precision * recall) / (precision + recall);



            List<double> RESULTS = new List<double>();

            RESULTS.Add(accuracy);
            RESULTS.Add(recall);
            RESULTS.Add(precision);
            RESULTS.Add(f_score);
            RESULTS.Add(TP_total);
            RESULTS.Add(TN_total);


            return RESULTS;
         


        }


        [HttpGet("TRAIN MODEL")]
        public string TRAIN_MODEL()
        {
            List<string> trainingPredictions = new List<string>();

            List<string> testPredictions = new List<string>();

            binaryTree tree = createDecisionTreeModel();

            candidateValues value1 = new candidateValues();

            Node node = tree.Root;

            foreach (var row in _context.trainingSet)
            {
                var property = row.GetType().GetProperty(tree.Root.candidateValue.Ax); // INITIAL SPLIT (ACCORDING TO WHICH ROW ?) FROM ROOT 
                var value = property.GetValue(row, null); // GET ATTRIBUTE VALUE

                node = tree.Root;
                
                if (value.ToString() == node.candidateValue.LeftVal) 
                {
                    if (node.candidateValue.isLeaf == true)
                    {
                        trainingPredictions.Add(node.candidateValue.leafResult);
                    }
                    else
                    {
                        if (node.Left!=null)
                        {
                            node = node.Left;


                        }                     
                    }
                }
                else
                {
                    if (node.candidateValue.isLeaf == true)
                    {
                        trainingPredictions.Add(node.candidateValue.leafResult);

                    }

                    else
                    {
                        if (node.Right != null)
                        {
                           node = node.Right;
                        }
                                              
                    }
                    
                }

                while ( true  )
                {
                    if ( (node.Left != null && node.Right != null) || node.candidateValue.isLeaf == true )
                    {
                        property = row.GetType().GetProperty(node.candidateValue.Ax);
                        value = property.GetValue(row, null);                    

                        if (value == node.candidateValue.LeftVal)
                        {
                            if (node.candidateValue.isLeaf == true) // IF LEAF NODE THEN ADD PREDICTION THEN END
                            {
                                trainingPredictions.Add(node.candidateValue.leafResult);
                                continue;
                            }

                            else
                            {
                                if (node.Left != null)
                                {

                                    node = node.Left;

                                }
                               
                            }

                            

                        }
                        else
                        {
                            if (node.Right != null)
                            {
                                if (node.candidateValue.isLeaf == true)
                                {
                                    trainingPredictions.Add(node.candidateValue.leafResult);
                                    continue;
                                }
                                else
                                {
                                    node = node.Right;
                                }
                                
                               
                            }
                            else
                            {
                                if (node.candidateValue.isLeaf == true)
                                {
                                    trainingPredictions.Add(node.candidateValue.leafResult);
                                    break;
                                }
                            }
                                                  
                        }

                    }
                    else
                    {
                        trainingPredictions.Add(row.Class);
                        break;
                        
                    }
                }

            }



            foreach (var row in _context.testSet)
            {
                var property = row.GetType().GetProperty(tree.Root.candidateValue.Ax); // INITIAL SPLIT (ACCORDING TO WHICH ROW ?) FROM ROOT 
                var value = property.GetValue(row, null); // GET ATTRIBUTE VALUE

                node = tree.Root;

                if (value.ToString() == node.candidateValue.LeftVal)
                {
                    if (node.candidateValue.isLeaf == true)
                    {
                        testPredictions.Add(node.candidateValue.leafResult);
                    }
                    else
                    {
                        if (node.Left != null)
                        {
                            node = node.Left;


                        }
                    }
                }
                else
                {
                    if (node.candidateValue.isLeaf == true)
                    {
                        testPredictions.Add(node.candidateValue.leafResult);

                    }

                    else
                    {
                        if (node.Right != null)
                        {
                            node = node.Right;
                        }

                    }

                }

                while (true)
                {
                    if ((node.Left != null && node.Right != null) || node.candidateValue.isLeaf == true)
                    {
                        property = row.GetType().GetProperty(node.candidateValue.Ax);
                        value = property.GetValue(row, null);

                        if (value == node.candidateValue.LeftVal)
                        {
                            if (node.candidateValue.isLeaf == true) // IF LEAF NODE THEN ADD PREDICTION THEN END
                            {
                                testPredictions.Add(node.candidateValue.leafResult);
                                continue;
                            }

                            else
                            {
                                if (node.Left != null)
                                {

                                    node = node.Left;

                                }

                            }



                        }
                        else
                        {
                            if (node.Right != null)
                            {
                                if (node.candidateValue.isLeaf == true)
                                {
                                    testPredictions.Add(node.candidateValue.leafResult);
                                    continue;
                                }
                                else
                                {
                                    node = node.Right;
                                }


                            }
                            else
                            {
                                if (node.candidateValue.isLeaf == true)
                                {
                                    testPredictions.Add(node.candidateValue.leafResult);
                                    break;
                                }
                            }

                        }

                    }
                    else
                    {
                        testPredictions.Add(row.Class);
                        break;

                    }
                }

            }



            List<double> TRAINING_SCORES = calculateTrainingPerformanceScores(trainingPredictions);
            List<double> TEST_SCORES = calculateTestPerformanceScores(testPredictions);

            return "TRAINING RESULTS:\n" + "Accuracy: " + TRAINING_SCORES[0] + "\n" + "TPRate(recall): " + TRAINING_SCORES[1] + "\n" + "Precision: " + TRAINING_SCORES[2] + "\n" + "F-Score: " + TRAINING_SCORES[3] + "\n" + "TP TOTAL: " + TRAINING_SCORES[4] + "\n" + "TN TOTAL: " + TRAINING_SCORES[5] + "\n\n" +

                "TEST RESULTS:\n" + "Accuracy: " + TEST_SCORES[0] + "\n" + "TPRate(recall): " + TEST_SCORES[1] + "\n" + "Precision: " + TEST_SCORES[2] + "\n" + "F-Score: " + TEST_SCORES[3] + "\n" + "TP TOTAL: " + TEST_SCORES[4] + "\n" + "TN TOTAL: " + TEST_SCORES[5];

        }





        // !!!!!!!!!!!!     MEMORY LEAK     !!!!!!!!!!!!!!!!!!

        [HttpGet("generateRandomForest")]
        public List<randomForestTree> generateRandomForest(int treeDensity)
        {
            List<binaryTree> trees = new List<binaryTree>();

            for (int i=0;i<treeDensity;++i)
            {
                trees.Add(new binaryTree());

            }

            Random randomAttr = new Random();


            List<candidateValues> attributes = new List<candidateValues>();

            attributes.Add(CALCULATE_A1());
            attributes.Add(CALCULATE_A2());
            attributes.Add(CALCULATE_A3());
            attributes.Add(CALCULATE_A4());
            attributes.Add(CALCULATE_A5());
            attributes.Add(CALCULATE_A6());
            attributes.Add(CALCULATE_A7());
            attributes.Add(CALCULATE_A8());
            attributes.Add(CALCULATE_A9());

            int attributeNumPerTree = 3;

            for (int i=0;i<treeDensity;++i)
            {
                for (int j=0;j<attributeNumPerTree;++j)
                {
                    trees[i].Add(trees[i], attributes[randomAttr.Next(9)]);
                }
               
            }

            

            List<randomForestTree> testPredictions = new List<randomForestTree>();


            for (int i=0;i<treeDensity;++i) 
            {
                randomForestTree currentTree = new randomForestTree();
                currentTree.treePredictions = new List<string>();
                currentTree.treeID = i;

                Node node = trees[i].Root;

                foreach (var row in _context.testSet) // CALCULATE ONLY TEST DATA SET SCORES
                {
                    var property = row.GetType().GetProperty(node.candidateValue.Ax); // GET SPLIT ATTRIBUTE
                    var value = property.GetValue(row, null); // GET ATTRIBUTE VALUE

                    if (value.ToString() == node.candidateValue.LeftVal)
                    {
                        if (node.candidateValue.isLeaf)
                        {
                            currentTree.treePredictions.Add(node.candidateValue.leafResult);
                            continue;
                            
                        }
                        else
                        {
                            if (node.Left!=null)
                            {
                                node = node.Left;

                            }
                            else
                            {
                                currentTree.treePredictions.Add(row.Class);
                                continue;
                            }
                        }

                    }
                    else
                    {
                        if (node.candidateValue.isLeaf)
                        {
                            currentTree.treePredictions.Add(node.candidateValue.leafResult);
                            continue;

                        }
                        else
                        {
                            if (node.Right!=null)
                            {
                                node = node.Right;

                            }
                            else
                            {
                                currentTree.treePredictions.Add(row.Class);
                                continue;
                            }
                        }

                    }

                    while (true)
                    {
                        if ( (node.Left == null && node.Right == null) ) // NO PLACE TO GO
                        {
                            currentTree.treePredictions.Add(row.Class);
                            break;
                        }

                        else
                        {
                            property = row.GetType().GetProperty(node.candidateValue.Ax);
                            value = property.GetValue(row, null);

                            if (value == node.candidateValue.LeftVal)
                            {
                                if (node.candidateValue.isLeaf)
                                {
                                    currentTree.treePredictions.Add(node.candidateValue.leafResult);
                                    break;
                                }

                                else
                                {
                                    if (node.Left != null)
                                    {
                                        node = node.Left;

                                    }
                                    else
                                    {
                                        currentTree.treePredictions.Add(row.Class);
                                        break;
                                    }

                                }

                            }

                            else
                            {
                                if (node.candidateValue.isLeaf)
                                {
                                    currentTree.treePredictions.Add(node.candidateValue.leafResult);
                                    break;

                                }
                                else
                                {
                                    if (node.Right != null)
                                    {
                                        node = node.Right;

                                    }
                                    else
                                    {
                                        currentTree.treePredictions.Add(row.Class);
                                        break;
                                    }
                                }

                            }



                        }

                    }

                    testPredictions.Add(currentTree);
                    continue;

                    
                }

                

                

                

            }

            List<string> TEST_PERFORMANCE_SCORES = new List<string>();

            for (int i=0;i<testPredictions.Count();++i)
            {
                //string current =  calculateTestPerformanceScores(testPredictions[i].treePredictions);

                //TEST_PERFORMANCE_SCORES.Add(string.Format(current.ToString()));
                   
            }


            return testPredictions;
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


            //candidateValues root = calculations[0];


            /*
            for (int i=1;i<calculations.Count();++i)
            {
                if (calculations[i].ϕ > root.ϕ)
                {
                    root = calculations[i];

                }
            }
            

            
            Node rootNode = new Node();
            rootNode.Left = null;
            rootNode.Right = null;
            rootNode.candidateValue = root;

            tree.Root = rootNode;
            */
            
            for (int i= 0; i<calculations.Count() ;++i)
            {
  
                tree.Add(tree, calculations[i]);
               
                
                

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
