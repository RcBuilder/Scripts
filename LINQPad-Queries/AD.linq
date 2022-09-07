<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>System.DirectoryServices</Namespace>
  <Namespace>System.DirectoryServices.ActiveDirectory</Namespace>
  <Namespace>System.Threading</Namespace>
</Query>

/*
	[AD] Active Directory

	powershell:
	https://docs.microsoft.com/en-us/powershell/module/activedirectory/
	
	c#:
	https://docs.microsoft.com/en-us/dotnet/api/system.directoryservices.searchresultcollection
	https://docs.microsoft.com/en-us/dotnet/api/system.directoryservices.searchresult
	https://docs.microsoft.com/en-us/dotnet/api/system.directoryservices.propertycollection
	https://www.c-sharpcorner.com/article/active-directory-and-net/
*/

void Main()
{	
	var ldapAddress = "LDAP://crtv.local";  // LDAP://rootDSE
	var de = new DirectoryEntry(ldapAddress, "xxxx", "xxxxxxx");
	var ds = new DirectorySearcher(de);
	ds.SearchScope = SearchScope.Subtree;
	
	/*
	// - find account by userName or email -
	
	ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(samaccountname="+ "moti" + "))";  // by userName
	/// ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(mail=" + "moti@crtv.co.il" + "))";  // by email
	
	var rs = ds.FindOne(); /// ds.FindAll();
	if(rs == null) {
		Console.WriteLine("No Such Account");
		return;
	}

	var current = rs.GetDirectoryEntry();
	var user = new {	
		UserName = current.Properties["samaccountname"].Value,
		FirstName = current.Properties["givenName"].Value,
		LastName = current.Properties["sn"].Value,
		Email = current.Properties["mail"].Value,
		Displayname = current.Properties["displayname"].Value
	};
	
	Console.WriteLine(user);
	*/
	
	/*
	// - get all accounts -
		
	var rsAll = ds.FindAll();
	
	Console.WriteLine($"({rsAll.Count})");	
	foreach(SearchResult rs in rsAll){
		var current = rs.GetDirectoryEntry();
		var user = new {
			UserName = current.Properties["samaccountname"].Value,
			FirstName = current.Properties["givenName"].Value,
			LastName = current.Properties["sn"].Value,
			Email = current.Properties["mail"].Value,
			Displayname = current.Properties["displayname"].Value
		};
		
		Console.WriteLine(user);
	}
	*/
	
	/*
	// - get property keys -	
		
	var rs = ds.FindOne();  // get top-1, note! only works if all accounts have the same properties!
	var props = rs.Properties;
	var propsNames = props.PropertyNames;
	
	Console.WriteLine($"({propsNames.Count})");	
	foreach(var p in propsNames)
		Console.WriteLine($"{p}");
	
	/// useraccountcontrol
	/// sn
	/// whenchanged
	/// objectclass
	/// badpwdcount
	/// lockouttime
	/// pwdlastset
	/// dscorepropagationdata
	/// adspath
	/// givenname
	/// lastlogoff
	/// objectguid
	/// logoncount
	/// lastlogontimestamp
	/// lastlogon
	/// displayname
	/// cn
	/// accountexpires
	/// managedobjects
	/// telephonenumber
	/// primarygroupid
	/// memberof
	/// mail
	/// instancetype
	/// usnchanged
	/// distinguishedname
	/// samaccounttype
	/// objectsid
	/// whencreated
	/// admincount
	/// samaccountname
	/// objectcategory
	/// name
	/// countrycode
	/// usncreated
	/// badpasswordtime
	/// codepage
	*/
	
		
	/*
	// - get property values -	
		
	var rsAll = ds.FindAll();
	
	var propertyKey = "telephonenumber"; // displayname, telephonenumber
	var counter = 0;
	foreach(SearchResult rs in rsAll){
		var props = rs.Properties;
		if(!props.Contains(propertyKey)) continue; // no such property - skip...
		
		counter++;
		foreach(var p in props[propertyKey])
			Console.WriteLine($"{p}");		
	}
	
	Console.WriteLine($"({counter})");	
	*/
	
	/*
	// - update user property -
	
	ds.Filter = "(&((&(objectCategory=Person)(objectClass=User)))(samaccountname="+ "rubi" + "))";  // by userName
	var rs = ds.FindOne();	
	if(rs == null) {
		Console.WriteLine("No Such Account");
		return;
	}

	var propertyKey = "telephonenumber";
	var propertyValue = "0545614020";
	
	var current = rs.GetDirectoryEntry();
	if(current.Properties.Contains(propertyKey))
	{
		Console.WriteLine("Update Property");
		current.Properties[propertyKey][0] = propertyValue;
	}
	else 
	{
		Console.WriteLine("Add Property");
		current.Properties[propertyKey].Add(propertyValue);
	}
	
	current.CommitChanges(); // save changes
	*/
}