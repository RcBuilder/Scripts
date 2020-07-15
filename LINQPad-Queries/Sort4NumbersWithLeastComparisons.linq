<Query Kind="Program" />

void Main()
{
	sort(12, 4, 9, 5);		// 4,5,9,12
	sort(9, 4, 12, 5); 		// 4,5,9,12
	sort(2, 5, 3, 4);		// 2,3,4,5
	sort(20, 15, 10, 5);	// 5,10,15,20
}

void sort(int n1, int n2, int n3, int n4) { 
	var temp = new int[] { n1, n2, n3, n4 };		
	if(temp[0] > temp[1]) swap(temp, 0, 1);  // compare n1 vs n2
	if(temp[2] > temp[3]) swap(temp, 2, 3);  // compare n3 vs n4
	if(temp[0] > temp[2]) swap(temp, 0, 2);  // compare min(n1, n2) vs min(n3, n4)
	if(temp[1] > temp[3]) swap(temp, 1, 3);  // compare max(n1, n2) vs max(n3, n4)
	
	// at this point, we can be sure that the min value (out of the all 4) is the most left organ and the max is the most right.
	// we just need one more comparison for the 2 numbers in the middle 	
	
	if(temp[1] > temp[2]) swap(temp, 1, 2);  // compare middle pair
	
	Console.WriteLine(string.Join(",", temp));
}

void swap(int[] arr, int index1, int index2) {
	var temp = arr[index1];
	arr[index1] = arr[index2];
	arr[index2] = temp;
}

/*
	process
	-------
	sort 4 numbers with least comparisons. 
	we only need 5 comparisons to achieve that. 
	-
	split into 2 pairs, compare each pair and find the max and min. 
	compare the max results > max(pair1) vs max(pair2)
	compare the min results > min(pair1) vs min(pair2)
	-
	at this point, we can be sure that the min value (out of the all 4) is the most left organ and the max is the most right.
	we just need one more comparison for the 2 numbers in the middle 
	compare middle numbers
	-
	1st comparison: n1 vs n2
	2nd comparison: n3 vs n4
	3rd comparison: max(n1,n2) vs max(n3,n4)
	4th comparison: min(n1,n2) vs min(n3,n4)
	5th comparison: middle items
*/