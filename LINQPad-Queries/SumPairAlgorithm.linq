<Query Kind="Program" />

// O(N^2)
// Nested Loop
(int n1, int n2) FindPair1(int[] arr, int sum) 
{     
    for (int i = 0; i < arr.Length; i++) 
		for (int j = i; j < arr.Length; j++)	         
        	if (arr[i] + arr[j] == sum)
				return (arr[i], arr[j]);                
	
	return (-1, -1);
} 

// O(2N)
// Create a BIT array representation, Use the Number value as the index
(int n1, int n2) FindPair2(int[] arr, int sum) 
{ 
    var a1 = new int[sum]; 	
	for (int i = 0; i < arr.Length; i++)
		if(arr[i] <= sum && arr[i] > -1)
        	a1[arr[i]] = 1; 
    	
    for (int i = 0; i < arr.Length; i++) 
		if(a1[sum - arr[i]] == 1)
			return (i+1, sum - arr[i]);
	
	return (-1, -1);
} 

// O(N)
// store already-passed numbers to a Set
(int n1, int n2) FindPair3(int[] arr, int sum) 
{ 
    var hashSet = new HashSet<int>(); 
    for (int i = 0; i < arr.Length; i++) { 
        int temp = sum - arr[i]; 

        if (hashSet.Contains(temp))
			return (temp, arr[i]);            
			
        hashSet.Add(arr[i]); 
    }
	
	return (-1, -1);
} 

// Driver Code 
void Main() 
{ 
    int[] arr = new int[] { 1, 6, 3, -2, 4, 7 }; 
    int sum = 5;     
	Console.WriteLine(FindPair1(arr, sum));  // O(N^2)
	Console.WriteLine(FindPair2(arr, sum));  // O(2N)
	Console.WriteLine(FindPair3(arr, sum));  // O(N)
}