<Query Kind="Statements" />

var list = new List<int> { 20, 30, 10, 1, 7, 21, 40, 40, 91, 2 };

// O(N^2)
var item = list.OrderByDescending(x => x).Skip(2).Take(1); 
Console.WriteLine(item); // 40

// ------

int first = 0, second = 0;
int firstIndex = 0, secondIndex = 0;

// O(N)
for(var i = 0; i < list.Count; i++){
	if(list[i] > first) {
		first = list[i];
		firstIndex = i;
	}
	else if(list[i] > second) {
		second = list[i];
		secondIndex = i;
	}
}

Console.WriteLine(list[firstIndex]);   // 91
Console.WriteLine(list[secondIndex]);  // 40