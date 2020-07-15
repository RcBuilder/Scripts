<Query Kind="Program" />

void Main()
{	
	var nums = new int[] { 1, 2, 3, 4 };
	var perms = new List<int>();  
	findPerms(perms, nums);
	/// Console.WriteLine(perms);
	
	var maxValid = 0;
	foreach(var item in perms)
		if(item > maxValid && isValidTime(item))
			maxValid = item;
			
	Console.WriteLine(maxValid);  // 23:41
}

void findPerms(List<int> perms, int[] nums, int index = 0) {
	if(index == 4){
		/// Console.WriteLine($"{string.Join(",", nums)} ({index})");
		perms.Add(Convert.ToInt32(string.Join("", nums)));  // num array as single number (e.g: [1,2,3,4] -> 1234)
	}
	for(var i = index; i < nums.Length; i++){
		swap(nums, i, index);
		findPerms(perms, nums, index + 1);
		swap(nums, index, i);
	}	
}

void swap(int[] arr, int index1, int index2) {
	var temp = arr[index1];
	arr[index1] = arr[index2];
	arr[index2] = temp;
}

bool isValidTime(int num) {
	var sNum = num.ToString();
	var HH = Convert.ToInt32(sNum.Substring(0, 2));	
	var MM = Convert.ToInt32(sNum.Substring(2, 2));	
	return (HH < 24 && MM < 60);
}

/*
	process
	-------
	maximim available (valid) time from 4 digits.	
	-
	find ALL permutations of the provided 4 digits and store them to a set as whole numbers.  
	loop through the set and find the max and valid date.
	-
	number of options:
	N = 3 -> 6 options (3 x 2 x 1)
	N = 4 -> 24 options (4 x 3 x 2 x 1)
	
	// stats trick view
	[4 options] [3 options] [2 options] [1 options] = 24
*/