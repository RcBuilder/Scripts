<Query Kind="Program" />

void Main()
{
	perm(4);
}

void perm(int n) {
	var arr = new List<int>();
	var k = n;
	while(k > 0) arr.Add(k--);
	while(arr.Count > 0){
		var i = rand(arr.Count-1);
		Console.Write(arr[i]);	
		arr.RemoveAt(i);
	}
}

int rand(int n) {
	return new Random().Next(n);
}

/*
	process:
	we need to return a random permutation of n numbers.
	create and array of n with all possible values.
	use the rand function to get a random index which later be used for getting an item from the array.
	once get a n item, remove it from the possible-values array and change the n value of the rand function to fit the size of the new array.
	
	e.g:
	perm(4); 
	arr [1,2,3,4]
	first execution > rand(3) > index 3 > arr[3] > value 4 
	arr [1,2,3]
	second execution > rand(2) > index 0 > arr[0] > value 1
	arr [2,3]
	third execution > rand(1) > index 0 > arr[0] > value 2
	arr [3]
	forth execution > rand(0) > index 0 > arr[0] > value 3
	arr []
	result: 4123
*/