<Query Kind="Statements" />

var target = 7;
var arr = new int[] { 3, 8, 1, 2, 4, 6 };

var map = new HashSet<int>(); // to store visited numbers and check them in 0(1)

for(var i = 0; i < arr.Length; i++) {	
	if(map.Contains(target - arr[i])) {
		Console.WriteLine($"{arr[i]} + {target - arr[i]} = {target}");  // 4 + 3 = 7
		break;
	}
	map.Add(arr[i]);		
}

/*
	process
	-------
	index 0 > 7 - 3 > contains(4) > NO > add(4)
	index 1 > 7 - 8 > contains(-1) > NO > add(-1)	
	index 2 > 7 - 1 > contains(6) > NO > add(6)	
	index 3 > 7 - 2 > contains(5) > NO > add(5)	
	index 4 > 7 - 4 > contains(3) > YES > [4, 3] > break	
*/
