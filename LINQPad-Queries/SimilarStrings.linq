<Query Kind="Program" />

void Main()
{
	// strings similarity - only a single word has changed
	var list = new List<string>{
		"Naomi is getting into the car", 
		"Naomi is eating at a restaurant",
		"George is getting into the car",
		"George is eating at a diner",
		"George is getting into the house",
		"Naomi is eating at a diner",    
		"Naomi is eating at a friend",    
		"Naomi is eating at a diner",    
		"Naomi is drinking at a diner"
	};	
		
	for(int i=0;i<list.Count; i++){
		for(int j=i;j<list.Count; j++){			
			var cRes = Compare(list[i], list[j]);			
			if(cRes.sucess){
				Console.WriteLine($"\"{list[i]}\" [VS] \"{list[j]}\"");
				Console.WriteLine($"{cRes.w1}, {cRes.w2}\n");
			}			
		}
	}	
}

(bool sucess, string w1, string w2) Compare(string s1, string s2){
	var a = s1.Split(' ');
	var b = s2.Split(' ');
	var a_b = a.Except(b);
	var b_a = b.Except(a);
	
	return (a_b.Count() == 1 && b_a.Count() == 1, a_b.FirstOrDefault(), b_a.FirstOrDefault());
}
