using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
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
        // FUNCTIONS FOR TRAINING DATASET

        private readonly IConfiguration _configuration;
        private DBContext _context;
        
        public trainingSetController(DBContext context)
        {
            this._context = context;
           
        }

        // CALCULATE A1,......,A9 will calculate Φ VALUES FOR EACH ATTRIBUTE (A1,A2...) ACCORDING TO CART ALGORITHM

        static double[][] ConvertStringListToDoubleArray(List<string> stringList)
        {
            double[][] doubleArray = new double[stringList.Count][];

            for (int i = 0; i < stringList.Count; i++)
            {
                string[] stringValues = stringList[i].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                doubleArray[i] = new double[stringValues.Length];

                for (int j = 0; j < stringValues.Length; j++)
                {
                    if (double.TryParse(stringValues[j], out double parsedValue))
                    {
                        doubleArray[i][j] = parsedValue;
                    }
                    else
                    {
                        // Handle parsing error if needed
                        Console.WriteLine($"Error parsing: {stringValues[j]}");
                    }
                }
            }

            return doubleArray;
        }


        static int[] StringToIntArray(string inputString)
        {
            string[] rows = inputString.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<int> result = new List<int>();

            foreach (string row in rows)
            {
                string[] numsStr = row.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int[] nums = Array.ConvertAll(numsStr, int.Parse);
                result.AddRange(nums);
            }

            return result.ToArray();
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


            // PL = NUMBER OF ROWS HAVING THE VALUE IN THE LEFT NODE / TOTAL
            // PR = NUMBER OF ROWS HAVING THE VALUE IN THE RIGHT NODE / TOTAL
            // PJTL GOOD = NUMBER OF ROWS HAVING THE VALUE OF LEFT NODE AND CLASS = GOOD / NUMBER OF ROWS HAVING THE VALUE OF LEFT NODE.
            // PJTL BAD = NUMBER OF ROWS HAVING THE VALUE OF LEFT NODE AND CLASS = BAD / NUMBER OF ROWS HAVING THE VALUE OF LEFT NODE.
            // PJTR GOOD = NUMBER OF ROWS HAVING THE VALUE OF RIGHT NODE AND CLASS = GOOD / NUMBER OF ROWS HAVING THE VALUE OF RIGHT NODE
            // PJTR BAD = NUMBER OF ROWS HAVING THE VALUE OF RIGHT NODE AND CLASS = BAD / NUMBER OF ROWS HAVING THE VALUE OF RIGHT NODE
            // QST = Σ |PJTL - PJTR| (FOR EACH CLASS)
            // RESULT = 2 * QST * PL * PR (SPECIFIED IN CART ALGORITHM)


            // CALCULATE ALL CART VALUES FOR ALL POSSIBLE INNER CANDIDATE SPLITS
            for (int i=0;i<elements.Count;++i)
            {
                string L = elements[i]; // LEFT NODE (for round i)
                double lCount = _context.trainingSet.Where(e => e.A1 == L).Count(); // COUNT WHERE ROW.Ax = CURRENT CANDIDATE
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

                // ADD CURRENT CALCULATED INNER CANDIDATE SPLIT TO CANDIDATE VALUES LIST
                currentCandidate.Ax = "A1";
                currentCandidate.LeftVal = elements[i];
                currentCandidate.ϕ = RESULT;
                
                candidateValues.Add(currentCandidate);
            
            }

            candidateValues MAX = candidateValues[0];

            // FIND THE CANDIDATE WITH HIGHEST Φ VALUE
            for (int i=1 ;i<candidateValues.Count();++i)
            {
                if (candidateValues[i].ϕ > MAX.ϕ)
                {
                    MAX = candidateValues[i];

                }
                
            }

            // RETURN THE CANDIDATE SPLIT WITH MAX VALUE AS RESULT
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



        // CALCULATES TRAINING PERFORMANCE SCORES
        // TAKE PREDICTIONS LIST AS PARAMETER
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

            // TAKE TRAINING SET FROM DATABASE
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

            // CALCULATE VALUES ACCORDING TO CART DEFINITON
            accuracy = ((TP_total + TN_total) / predictions.Count());

            precision = (TP_total / (TP_total + FP_total));

            double a = TP_total + FN_total;
            recall = TP_total / a;

            f_score = 2 * (precision * recall) / (precision + recall);

            List<double> RESULTS = new List<double>();

            // ADD RESULT CALCULATIONS TO RESULTS LIST
            RESULTS.Add(accuracy);
            RESULTS.Add(recall);
            RESULTS.Add(precision);
            RESULTS.Add(f_score);
            RESULTS.Add(TP_total);
            RESULTS.Add(TN_total);

            return RESULTS;
        }

        // CALCULATES TEST PERFORMANCE SCORES
        // TAKES PREDICTIONS LIST AS PARAMETER
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

            // THIS TIME TAKE THE TEST DATASET INSTEAD OF TRAINING SET
            List<testSet> testSet = _context.testSet.ToList();

            for (int i = 0; i < predictions.Count(); ++i)
            {
                if (predictions[i] == testSet[i].Class) // IF PREDICTED CLASS == REAL TEST SET CLASS
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

            // ADD RESULTS TO RESULTS LIST
            RESULTS.Add(accuracy);
            RESULTS.Add(recall);
            RESULTS.Add(precision);
            RESULTS.Add(f_score);
            RESULTS.Add(TP_total);
            RESULTS.Add(TN_total);


            return RESULTS;
         
        }


        //  CREATE A DECISION TREE MODEL , CALCULATE PREDICTIONS THEN CALCULATE PERFORMANCE SCORES
        [HttpGet("trainModel")]
        public string TRAIN_MODEL()
        {
            List<string> trainingPredictions = new List<string>();

            List<string> testPredictions = new List<string>();

            binaryTree tree = createDecisionTreeModel(); // CREATE TREE MODEL

            candidateValues value1 = new candidateValues();

            Node node = tree.Root; // START FROM ROOT NODE OF THE DT

            // SCAN ALL THE DATA WITHIN THE TRAINING SET
            foreach (var row in _context.trainingSet)
            {
                var property = row.GetType().GetProperty(tree.Root.candidateValue.Ax); // INITIAL SPLIT (ACCORDING TO WHICH ROW ?) FROM ROOT 
                var value = property.GetValue(row, null); // GET ATTRIBUTE VALUE

                node = tree.Root;
                
                // IF DECISION TREE.ROOT.LEFT VALUE = CURRENT ROW VALUE THEN GO TO LEFT NODE
                if (value.ToString() == node.candidateValue.LeftVal) 
                {
                    // CHECK IF NODE IS LEAF NODE THEN DIRECTLY ADD TO PREDICTIONS
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
                {   // IF VALUE != LEFT NODE THEN GO TO RIGHT NODE
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


                // SEARCH AND CHECK THE DECISION TREE CONTINUE UNTIL A LEAF NODE REACHED 
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
                        // ADD TO PREDICTIONS
                        trainingPredictions.Add(row.Class);
                        break;
                        
                    }
                }

            }


            // SCAN ALL THE DATA WITHIN THE TEST SET
            // SAME OPERATION AS DONE IN TRAINING SET
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

                // SCAN ALL THE DECISION TREE CONTINUE UNTIL A LEAF NODE REACHED
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
                        // ADD TO PREDICTIONS LIST
                        testPredictions.Add(row.Class);
                        break;

                    }
                }

            }


            // CREATE LIST FOR STORE THE PERFORMANCE SCORES

            // CALL TRAINING PERFORMANCE SCORE FUNCTION STORE THE RESULTS WITHIN A LIST
            List<double> TRAINING_SCORES = calculateTrainingPerformanceScores(trainingPredictions);
            // CALL TEST PERFORMANCE SCORE FUNCTION STORE THE RESULTS WITHIN A LIST
            List<double> TEST_SCORES = calculateTestPerformanceScores(testPredictions);

            // RETURN RESULTS AS A SINGLE STRING
            return "TRAINING RESULTS:\n" + "Accuracy: " + TRAINING_SCORES[0] + "\n" + "TPRate(recall): " + TRAINING_SCORES[1] + "\n" + "Precision: " + TRAINING_SCORES[2] + "\n" + "F-Score: " + TRAINING_SCORES[3] + "\n" + "TP TOTAL: " + TRAINING_SCORES[4] + "\n" + "TN TOTAL: " + TRAINING_SCORES[5] + "\n\n" +

                "TEST RESULTS:\n" + "Accuracy: " + TEST_SCORES[0] + "\n" + "TPRate(recall): " + TEST_SCORES[1] + "\n" + "Precision: " + TEST_SCORES[2] + "\n" + "F-Score: " + TEST_SCORES[3] + "\n" + "TP TOTAL: " + TEST_SCORES[4] + "\n" + "TN TOTAL: " + TEST_SCORES[5];

        }



        // GENERATES RANDOM FOREST WITH DESIRED NUMBER OF DECISION TREES
        [HttpGet("generateRandomForest")]
        public string generateRandomForest(int treeDensity)
        {
            List<binaryTree> trees = new List<binaryTree>();

            // INITIALIZE DESIRED NUMBER OF TREES
            for (int i=0;i<treeDensity;++i)
            {
                trees.Add(new binaryTree());

            }

            Random randomAttr = new Random();

            List<candidateValues> attributes = new List<candidateValues>();

            // CALCULATE CART VALUES FOR ALL ATTRIBUTES
            attributes.Add(CALCULATE_A1());
            attributes.Add(CALCULATE_A2());
            attributes.Add(CALCULATE_A3());
            attributes.Add(CALCULATE_A4());
            attributes.Add(CALCULATE_A5());
            attributes.Add(CALCULATE_A6());
            attributes.Add(CALCULATE_A7());
            attributes.Add(CALCULATE_A8());
            attributes.Add(CALCULATE_A9());

            // EACH TREE WILL TAKE 3 RANDOM ATTRIBUTES (RANDOM SUBSPACE)
            int attributeNumPerTree = 3;

            // ASSIGN 3 RANDOM ATTRIBUTES FOR EACH TREE
            for (int i=0;i<treeDensity;++i)
            {
                for (int j=0;j<attributeNumPerTree;++j)
                {
                    trees[i].Add(trees[i], attributes[randomAttr.Next(9)]);
                }
               
            }

            

            List<randomForestTree> testPredictions = new List<randomForestTree>();


            // MAKE PREDICTIONS FOR EACH DECISION TREE MODEL
            for (int i=0;i<treeDensity;++i) 
            {
                randomForestTree currentTree = new randomForestTree(); // CREATE A RANDOM FOREST TREE
                currentTree.treePredictions = new List<string>(); // INITIALIZE EMPTY LIST FOR CURRENT TREE'S PREDICTIONS
                currentTree.treeID = i; // CURRENT TREE NUMBER

                Node node = trees[i].Root; // START FROM THE ROOT DECISION NODE

                foreach (var row in _context.testSet) // CALCULATE ONLY TEST DATA SET SCORES
                {
                    var property = row.GetType().GetProperty(node.candidateValue.Ax); // GET SPLIT ATTRIBUTE
                    var value = property.GetValue(row, null); // GET ATTRIBUTE VALUE

                    // IF LEFT NODE THEN GO LEFT
                    // IF IS A LEAF NODE THEN DIRECTLY ADD TO PREDICTIONS
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
                    {   // IF RIGHT NODE THEN GO TO RIGHT DO SAME CHECKS
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

                    // SCAN ALL THE TREE UNTIL REACHING A LEAF NODE
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

                    // ADD TO PREDICTIONS LIST
                    testPredictions.Add(currentTree);
                    continue;                 
                }

            }

            double avg_accuracy = 0;
            double avg_recall = 0;
            double avg_precision = 0;
            double avg_fscore = 0;
            double avg_tp_total = 0;
            double avg_tn_total = 0;


            List<double> TEST_SCORES = new List<double>();

            List<double> tempList;

            // GET SUM OF ALL THE SCORES 
            for (int i=0;i<testPredictions.Count();++i)
            {
                tempList = calculateTestPerformanceScores(testPredictions[i].treePredictions);
                avg_accuracy += tempList[0];
                avg_recall += tempList[1];
                avg_precision += tempList[2];
                avg_fscore += tempList[3];
                avg_tp_total += tempList[4];
                avg_tn_total += tempList[5];
            }

            // CALCULATE THE AVERAGE OF THE SCORES
            avg_accuracy /= testPredictions.Count();
            avg_recall /= testPredictions.Count();
            avg_precision /= testPredictions.Count();
            avg_fscore /= testPredictions.Count();
            avg_tp_total /= testPredictions.Count();
            avg_tn_total /= testPredictions.Count();

            // ADD RESULTS TO THE RESULTS LIST
            TEST_SCORES.Add(avg_accuracy);
            TEST_SCORES.Add(avg_recall);
            TEST_SCORES.Add(avg_precision);
            TEST_SCORES.Add(avg_fscore);
            TEST_SCORES.Add(avg_tp_total);
            TEST_SCORES.Add(avg_tn_total);

            // RETURN TEST SCORES FOR RANDOM FOREST AS A SINGLE STRING
            return "RANDOM FOREST With "+ treeDensity +" trees "+ "TEST RESULTS:\n" + "Accuracy: " + TEST_SCORES[0] + "\n" + "TPRate(recall): " + TEST_SCORES[1] + "\n" + "Precision: " + TEST_SCORES[2] + "\n" + "F-Score: " + TEST_SCORES[3] + "\n" + "TP TOTAL: " + TEST_SCORES[4] + "\n" + "TN TOTAL: " + TEST_SCORES[5];

            
        }


        // CREATE DECISION TREE MODEL
        [HttpGet("createDecisionTreeModel")]
        public binaryTree createDecisionTreeModel()
        {
            binaryTree tree = new binaryTree();
            
            List<candidateValues> calculations = new List<candidateValues>();

            // CALCULATE CART SPLIT VALUES FOR EACH ATTRIBUTE
            calculations.Add(CALCULATE_A1());
            calculations.Add(CALCULATE_A2());
            calculations.Add(CALCULATE_A3());
            calculations.Add(CALCULATE_A4());
            calculations.Add(CALCULATE_A5());
            calculations.Add(CALCULATE_A6());
            calculations.Add(CALCULATE_A7());
            calculations.Add(CALCULATE_A8());
            calculations.Add(CALCULATE_A9());


            // ADD CALCULATIONS TO THE TREE
            for (int i= 0; i<calculations.Count() ;++i)
            {
                tree.Add(tree, calculations[i]);

            }

            // PRINT DECISION TREE MODEL TO THE OUTPUT CONSOLE
            binaryTree.PrintTree(tree);

            return tree;

        }

        // GET ALL ROWS WITHIN THE TRAINING DATA TABLE
        [HttpGet("getAllTrainingData")]
        public async Task<List<trainingSet>> getAllData()
        {
            var data = _context.trainingSet.ToListAsync();
            return await data;
        }

       

    }
}
