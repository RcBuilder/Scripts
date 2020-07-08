<Query Kind="Program" />

void Main()
{
	Console.WriteLine(IsPOW_2(256));  // (true, 7)
	Console.WriteLine(IsPOW_X(256, 3)); // (false, 5)
}

(bool result, int steps) IsPOW_2(int num) {
	return IsPOW_X(num, 2);
}

(bool result, int steps) IsPOW_X(int num, int pow, int current = 0, int steps = 0) {
	if(current == 0) current = pow;
	if(current == num) return (true, steps);
	if(current > num) return (false, steps);
	return IsPOW_X(num, pow, current * pow, steps + 1);
}
