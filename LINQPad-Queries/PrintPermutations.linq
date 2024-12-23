<Query Kind="Program" />

// Print all possible combinations in a given array

void Main()
{
	var arr = new int[] { 1,2,3 };
	PrintPermutations(arr);	
}

void PrintPermutations(int[] arr, int index = 0) {
	/// Console.WriteLine($"{index}) {String.Join(",", arr)}");
	
	if(index >= arr.Length - 1) {      
	 	Print(arr);        			
	    return;
    }

	for(int i = index; i < arr.Length; i++)
	{    
		Swap(arr, index, i);        
        PrintPermutations(arr, index+1);
		Swap(arr, i, index); // to change the sort back to its original state
    }
}

void Swap(int[] arr, int index1, int index2) {
	int temp = arr[index1];
    arr[index1] = arr[index2];
    arr[index2] = temp;
}

void Print(int[] arr){
	for(int i = 0; i < arr.Length - 1; i++)
        Console.Write(arr[i] + ", ");  
		
    if(arr.Length > 0) 
        Console.WriteLine(arr[arr.Length - 1]);
}


