<Query Kind="Program" />

void Main()
{
	var bitArray = new int[] { 1, 0, 0, 1, 0, 1, 1, 1, 0, 0, 1 };

	var first0 = bitArray.Length - 1;
	while(first0 > 0)
		if(bitArray[first0] == 0) break; 
		else first0--;
			
	for(int i = 0, k = first0; i < k; i++)
		if(bitArray[i] == 1){
			swap(bitArray, i, k);			
			k--;
		}
		
	Console.WriteLine(string.Join(",", bitArray));	// 0,0,0,0,0,1,1,1,1,1,1
}
	
void swap(int[] arr, int index1, int index2) {
	var temp = arr[index1];
	arr[index1] = arr[index2];
	arr[index2] = temp;
}	

/*
	process
	-------
	sort bit array so all 1's should be after all 0's.
	-
	allocate 2 pointer, one at index 0 and the second at the very first 0 from the last.
	the first index is a "fast" index whereas the second is a "slow" index, which means it's only promoted when there's a swap. 
	once a '1' bit value is found, swap it.
*/