## CART Decision Tree Algorithm and Random Forest implementation in .NET
This project involves the practical application of the CART decision tree algorithm acquired through comprehensive study in the "Machine Learning" course.
 The algorithm has been implemented using a dataset comprising 9 attributes and prediction classes denoted as "good" or "bad."
 
 * You can construct a CART decision tree model and assess the essential metrics, including "Accuracy," "TP Rate (recall)," "Precision," and "F-Score," across both training and test datasets.
 * Furthermore you can construct a Random Forest Ensemble with n-number of CART decision trees, each created with using "Random Subspace" method (3 random attributes), and observe he performance metrics.
   
# Example Metric Calcualtions:

* Performance Metrics of a Random Forest with 100 CART Decision Trees, each generated using Random Subspace method with 3 attributes

  ![Random Forest with 100-Trees](https://github.com/gunesgultekin/CART_DECISION_TREE/assets/126399958/932fe920-dced-4fda-bfca-cdada82442b6)


* Performance Metrics of a Random Forest with 15 CART Decision Trees, each generated using Random Subspace method with 3 attributes

![Random Forest with 15-Trees](https://github.com/gunesgultekin/CART_DECISION_TREE/assets/126399958/85169b35-4431-4b2c-97d8-a45ac736a0dd)

* Performance Metrics of trained CART Model
  
![performanceMetrics](https://github.com/gunesgultekin/CART_DECISION_TREE/assets/126399958/b6115071-39f8-45f7-b15b-783da4a03dfb)


## To run:
-	Within the Context/connectionConfiguration.cs change the connectionString with your local database server connection string.
•	Connection string format (for mssql) :
 "Data Source= (your pc username)\\SQLEXPRESS;Initial Catalog = (your database name); Integrated Security = True; TrustServerCertificate=True";
(!) Also your database table names should be “testSet” and “trainingSet” for Entity Framework ORM Configuration
