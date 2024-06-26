<Query Kind="Program" />

void Main()
{
	var tree = new TreeNode {
	    Value = "1",
	    Left = new TreeNode {
	        Value = "2",
	        Left = new TreeNode {
	            Value = "4"
	        },
	        Right = new TreeNode {
	            Value = "5"
	        },
	    },
	    Right = new TreeNode {
	        Value = "3",
	        Left = new TreeNode {
	            Value = "6",
	        }
	    },
	};
	
	Console.WriteLine(GetTreeLevel(tree));
}

public class TreeNode {
    public string Value { get; set; }
    public TreeNode Left { get; set; }
    public TreeNode Right { get; set; }
}

static int GetTreeLevel(TreeNode node, int current = 1, int max = 1) {
    if (node == null)
        return max;

	if(current > max)
		max = current;
		
    var maxleft = GetTreeLevel(node.Left, current + 1, max);
    var maxright = GetTreeLevel(node.Right, current + 1, max);
	return Math.Max(maxleft, maxright);
}