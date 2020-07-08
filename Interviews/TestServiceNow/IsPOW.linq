<Query Kind="Program" />

void Main()
{
	Console.WriteLine(IsPOW_2(256));  // (true, 7)
}

(bool result, int steps) IsPOW_2(int num, int current = 2, int steps = 0) {	
	if(current == num) return (true, steps);
	if(current > num) return (false, steps);
	return IsPOW_2(num, current * 2, steps + 1);
}
