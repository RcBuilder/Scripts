<Query Kind="Program" />

void Main()
{
	var tree = new TreeNode
	{
	    Value = 1,
	    Left = new TreeNode
	    {
	        Value = 3,
	        Left = new TreeNode
	        {
	            Value = 1
	        },
	        Right = new TreeNode
	        {
	            Value = 4,
				Left = new TreeNode
	        	{
		            Value = -4
		        },
		        Right = new TreeNode
		        {
		            Value = 1
		        },
	        },
	    },
	    Right = new TreeNode
	    {
	        Value = -2,
			Left = new TreeNode
        	{
	            Value = 5
	        },
	        Right = new TreeNode
	        {
	            Value = 3,
				Left = new TreeNode
	        	{
		            Value = 2
		        },
	            Right = new TreeNode
	            {
	                Value = 2
	            }
	        }
	    },
	};
		
	Tree_Calc(tree, 0);	
}

public class TreeNode
{
    public int Value { get; set; }
    public TreeNode Left { get; set; }
    public TreeNode Right { get; set; }
}

void Tree_Calc(TreeNode node, int sum)
{
    if (node == null){
		Console.WriteLine(sum);
        return;
	}

    /// Console.Write("{0} ", node.Value);
	sum+=node.Value;
    Tree_Calc(node.Left, sum);
    Tree_Calc(node.Right, sum);
}