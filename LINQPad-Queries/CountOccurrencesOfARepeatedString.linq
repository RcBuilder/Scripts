<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>System.Threading</Namespace>
</Query>

/*
	TASK:
	Count occurrences of a character in a repeated string. 
	Given an integer N and a lowercase string S. The string is repeated until reaches the length of N letters. 
	The task is to find the No. of occurrences of small character ‘a’ in the final string. 
	Write a function that receives the string S, the N letters to be repeated and returns the occurrences of ‘a’ letter. The code must be complied successfully 
*/

void Main()
{
	Console.WriteLine("abcd".DuplicateByN(10).CountOccurrences('a'));
}

public static class Extensions {
	public static string DuplicateByN(this string me, int length) {
		var sb = new StringBuilder();
		for(var i=0;i<length;i++) sb.Append(me);
		return sb.ToString();
	}

	public static int CountOccurrences(this string me, char charToCount) {
		var counter = 0;
		for(var i=0;i<me.Length;i++) 
			if(me[i] == charToCount) counter++;
		return counter;
	}
}