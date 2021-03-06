### ZCompare ###
object comparison utility (reflection based)

Source:
https://www.codeproject.com/Articles/1167403/Typesafe-NET-Object-Comparison-with-ZCompare

Nuget:
Install-Package Zaybu.ZCompare 

------------------------

-- Methods -- 

* Compare:
  var rootResult = ZCompare.Compare(object, object); // compare between 2 objects 

  e.g:
  var item1 = new Item { ID = 1, Name = "A", Stock = 10 };
  var item2 = new Item { ID = 1, Name = "A", Stock = 10 };
  var result = ZCompare.Compare(item1, item2);  

* GetResult:
  var nodeResult = [rootResult].GetResult(object);  // get specific node result

  e.g:
  var result = ZCompare.Compare(itemCollection1, itemCollection2);              
  var nodeResult = result.GetResult(itemCollection1.Items);

* GetSummary:
  [nodeResult].GetSummary() // get summary for the specified node result

* IgnoreProperty:
  ZCompare.IgnoreProperty(type, propertyName, type);

  e.g:
  ZCompare.IgnoreProperty(typeof(Item), "Stock", typeof(byte));
  var result = ZCompare.Compare(itemCollection1, itemCollection2);

* RegisterComparitor:
  ZCompare.RegisterComparitor(comparer); // override the ZCompare comparer with a custom one for a specific T object

  e.g:
  public class ItemZComparer : ComparitorBase<Item>{
    public override void Compare(Item originalObject, Item compareToObject, ZCompareResults results) {
        if (originalObject.ID != compareToObject.ID)
            results.AddResult("ID Comparison", ResultStatus.Changed, originalObject.ID, compareToObject.ID);            
    }

    public override string GetStringValue(Item value) {
        return "ITEM #" + value.ID;
    }
  } 

  ZCompare.RegisterComparitor(new ItemZComparer()); // register custom comparer T
  var item1 = new Item { ID = 3, Name = "A", Stock = 10 };
  var item2 = new Item { ID = 1, Name = "A", Stock = 10 };
  var result = ZCompare.Compare(item1, item2);

------------------------

-- Attributes -- 

* IgnoreProperty 
  // Zaybu.Compare.IgnorePropertyAttribute

  e.g:
  public class Item {
    public int ID { get; set; }     
    public string Name { get; set; }
	[IgnoreProperty]
    public byte Stock { get; set; }
  }

------------------------

-- Using -- 

using Zaybu.Compare;


// compare 2 simple objects 
var item1 = new Item { ID = 1, Name = "A", Stock = 10 };
var item2 = new Item { ID = 1, Name = "A", Stock = 10 };
var result = ZCompare.Compare(item1, item2);
Console.WriteLine("Identical: {0}, Differences: {1}", result.Identical, result.NumberOfDifferences);

var itemCollection1 = new ItemCollection {
    ID = 1,
    Items = new List<Item> {
        new Item { ID = 10, Name = "AA", Stock = 8 },
        new Item { ID = 11, Name = "AB", Stock = 22 }
    }
};

var itemCollection2 = new ItemCollection
{
    ID = 1,
    Items = new List<Item> {
        new Item { ID = 10, Name = "AA", Stock = 8 },
        new Item { ID = 11, Name = "AB", Stock = 23 }
    }
};

// compare 2 complicated objects 
result = ZCompare.Compare(itemCollection1, itemCollection2);
Console.WriteLine("Identical: {0}, Differences: {1}", result.Identical, result.NumberOfDifferences); // Identical: False, Differences: 1
            
// parse results on specific property (Items)
var nodeResult = result.GetResult(itemCollection1.Items);
Console.WriteLine("Property: {0}, Status: {1}", nodeResult.PropertyName, nodeResult.Status); // Property: Items, Status: Changed
            
// parse results on specific field (ID)
nodeResult = result.GetResult(itemCollection1.Items[0], "ID");
Console.WriteLine("Property: {0}, Status: {1}", nodeResult.PropertyName, nodeResult.Status); // Property: ID, Status: NoChange

// parse results on specific field (Stock)
nodeResult = result.GetResult(itemCollection1.Items[0], "Stock");
Console.WriteLine("Property: {0}, Status: {1}", nodeResult.PropertyName, nodeResult.Status); // Property: Stock, Status: NoChange

nodeResult = result.GetResult(itemCollection1.Items[1], "Stock");
Console.WriteLine("Property: {0}, Status: {1}", nodeResult.PropertyName, nodeResult.Status); // Property: Stock, Status: Changed

// result Summary
nodeResult = result.GetResult(itemCollection1.Items);
Console.WriteLine(nodeResult.GetSummary());

---

public class ItemCollection {        
    public int ID { get; set; }
    public List<Item> Items { get; set; }

    public ItemCollection() {
        this.Items = new List<Item>();
    }
}

public class Item {
    public int ID { get; set; }     
    public string Name { get; set; }
    public byte Stock { get; set; }
}