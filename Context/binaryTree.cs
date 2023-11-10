using System.Xml.Linq;

public class binaryTree<T> where T : IComparable<T>
{
    public Node<T> Root { get; private set; } = null!;

    public void Add(T value)
    {
        if (Root == null)
        {
            Root = new Node<T>(value);
        }
        else
        {
            Root.Add(value);
        }
    }
}

public class Node<T> where T : IComparable<T>
{
    public T Value { get; private set; }
    public Node<T> Left { get; private set; } = null!;
    public Node<T> Right { get; private set; } = null!;

    public Node(T value) => Value = value;

    public void Add(T newValue)
    {
        if (newValue.CompareTo(Value) < 0)
        {
            if (Left == null)
            {
                Left = new Node<T>(newValue);
            }
            else
            {
                Left.Add(newValue);
            }
        }
        else
        {
            if (Right == null)
            {
                Right = new Node<T>(newValue);
            }
            else
            {
                Right.Add(newValue);
            }
        }
    }

    public static int COUNT = 15;

    public static void print2DUtil(Node<T> root, int space)
    {
        // Base case
        if (root == null)
            return;

        // Increase distance between levels
        space += COUNT;

        // Process right child first
        print2DUtil(root.Right, space);

        // Print current node after space
        // count
        System.Diagnostics.Debug.Write("\n");
        for (int i = COUNT; i < space; i++)
            System.Diagnostics.Debug.Write(" ");
        System.Diagnostics.Debug.Write(root.Value + "\n");

        // Process left child
        print2DUtil(root.Left, space);
    }

    // Wrapper over print2DUtil()
    public static void print2D(Node<T> root)
    {
        // Pass initial space count as 0
        print2DUtil(root, 0);
    }
}

