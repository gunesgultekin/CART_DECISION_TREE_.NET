


using CART_DECISION_TREE.Entities;



public class binaryTree
{
    public Node Root { get; set; } = null!;
    public void Add(binaryTree tree, candidateValues newValue)
    {

        Node newNode = new Node();
        newNode.candidateValue = newValue;
        newNode.Left = null;
        newNode.Right = null;


        if (tree.Root == null)
        {
            tree.Root = newNode;
            return;
        }


        Node currentNode = tree.Root;

        while (true)
        {
            if (newNode.candidateValue.ϕ < currentNode.candidateValue.ϕ)
            {
                if (currentNode.Left == null)
                {
                    currentNode.Left = newNode;
                    return;
                }
                else
                {
                    currentNode = currentNode.Left;
                }

            }
            else
            {
                if (currentNode.Right == null)
                {
                    currentNode.Right = newNode;
                    return;
                }
                else
                {
                    currentNode = currentNode.Right;
                }
            }





        }
    }

    public static void PrintTree(Node node, int level)
    {
        if (node == null)
            return;

        PrintTree(node.Right, level + 1);
        System.Diagnostics.Debug.WriteLine(new string(' ', 10 * level) + node.candidateValue.Ax + " " + Math.Round(node.candidateValue.ϕ,3));
        PrintTree(node.Left, level + 1);
    }

    // Wrapper function to print the binary tree
    public static void PrintTree(binaryTree tree)
    {
        PrintTree(tree.Root, 0);
    }

}

public class Node

{
    public candidateValues candidateValue { get; set; }
    public Node Left { get; set; } = null!;
    public Node Right { get; set; } = null!;

    

    
}

   

    
    

