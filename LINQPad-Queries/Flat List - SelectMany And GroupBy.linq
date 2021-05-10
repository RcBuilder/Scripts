<Query Kind="Program" />

void Main()
{
	var orders = new List<Order>();

	orders.Add(new Order{
		Id = 1,	
		Rows = new List<OrderRow>{
			new OrderRow { ItemName = "Item-A" },
			new OrderRow { ItemName = "Item-B" }
		}
	});
	orders.Add(new Order{
		Id = 2,	
		Rows = new List<OrderRow>{
			new OrderRow { ItemName = "Item-A" },
			new OrderRow { ItemName = "Item-C" },
		}
	});
	orders.Add(new Order{
		Id = 3,	
		Rows = new List<OrderRow>{
			new OrderRow { ItemName = "Item-A" },
			new OrderRow { ItemName = "Item-B" },
			new OrderRow { ItemName = "Item-C" },
		}
	});

	var flat = orders.SelectMany(o => o.Rows, (o, r) => r);
	var group = flat.GroupBy(x => x.ItemName);
	foreach (var row in group)	
		Console.WriteLine($"{row.Key} ({row.Count()})");	
}

class Order{
	public int Id {get; set;}	
	public List<OrderRow> Rows {get; set;}
}

class OrderRow{	
	public string ItemName {get; set;}
}

/*
	Item-A (3)
	Item-B (2)
	Item-C (2)
*/