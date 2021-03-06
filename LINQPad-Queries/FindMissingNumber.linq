<Query Kind="Program" />

// find a missing number in a given  array of 0......N
// note: only one is missing 

void Main()
{
	var arr = new int[] { 4, 1, 2, 10, 8, 9, 3, 5, 7 };	
	var sum10 = SumN(10);  // find sigma of N (sum all numbers from 0...N)
	var sumArr = SumArr(arr);  // sum current array 
	Console.WriteLine($" missing number is {sum10 - sumArr}");  // compare to find the number 
}

int SumN(int n){
	var result = 0;
	for(var i = 0; i <= n; result+=i, i++);
	return result;
}

int SumArr(int[] arr){
	var result = 0;
	for(var i = 0; i < arr.Length; result+=arr[i], i++);
	return result;
}