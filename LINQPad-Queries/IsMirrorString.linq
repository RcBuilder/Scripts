<Query Kind="Program" />

void Main()
{
	var str = "abcddcba";
	
	// option 1 - using a built-in Reverse method
	var reverse = new string(str.Reverse().ToArray());
	if(str == reverse)
		Console.WriteLine("Mirror!");
		
	// option 2 - manual reverse	
	var sChars = str.ToCharArray();
	for(int i = 0, k = (sChars.Length - 1); k > i; i++, k--)	
		swap(sChars, i, k);	
	var reverse2 = new string(sChars);	
	if(str == reverse2)
		Console.WriteLine("Mirror!");
	
	// option 3 - check chars from 2 edges correspondingly 
	var isMirror = true;	
	for(int i = 0, k = (str.Length - 1); k > i; i++, k--)		
		if(str[i] != str[k]) {
			isMirror = false;
			break;
		}
	if(isMirror)
		Console.WriteLine("Mirror!");
		
	Console.WriteLine("Done");	
	
}

void swap(char[] sChars, int index1, int index2){	
	var temp = sChars[index1];
	sChars[index1] = sChars[index2];
	sChars[index2] = temp;	
}