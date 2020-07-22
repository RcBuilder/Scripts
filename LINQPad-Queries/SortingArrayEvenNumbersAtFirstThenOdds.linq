<Query Kind="Program" />

void Main()
{
	var arr = new int[] {1, 2, 3, 5, 4, 7, 10};

	var firstEven = arr.Length - 1;
	while(firstEven > 0)
		if(arr[firstEven] % 2 == 0) break; 
		else firstEven--;
			
	for(int i = 0, k = firstEven; i < k; i++)
		if(arr[i] % 2 > 0) {
			swap(arr, i, k);			
			while(arr[k] % 2 > 0) k--; // find next 'Even' pointer 
		}
		
	Console.WriteLine(string.Join(",", arr));	
}
	
void swap(int[] arr, int index1, int index2) {
	var temp = arr[index1];
	arr[index1] = arr[index2];
	arr[index2] = temp;
}	

/*
	process
	-------
	set all Even numbers to at first and only then the Odd numbers.
	-
	allocate 2 pointers, one at index 0 and the second at the very first Even number from the last.
	the first index is a "fast" index whereas the second is a "slow" index, which means it's only promoted when there's a swap. 
	once an Odd number is found, swap it and set the "slow" pointer to points the next Even number.
*/