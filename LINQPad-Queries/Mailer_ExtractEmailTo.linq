<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>System.Threading</Namespace>
</Query>

void Main()
{
	Console.WriteLine(ExtractEmailTo("(test1@cpa.co.il)O550970_11673.pdf").To);
	Console.WriteLine(ExtractEmailTo("(test2@cpa.co.il;test2_2@cpa.co.il)O550970_11673.pdf").To);
	Console.WriteLine(ExtractEmailTo("(test3@cpa.co.il,test3_2@cpa.co.il)O550970_11673.pdf").To);
	Console.WriteLine(ExtractEmailTo("(test4@cpa.co.il ; test4_2@cpa.co.il)O550970_11673.pdf").To);
	Console.WriteLine(ExtractEmailTo("(test5@cpa.co.il     ,     test5_2@cpa.co.il)O550970_11673.pdf").To);
	Console.WriteLine(ExtractEmailTo("(    test6@cpa.co.il     )O550970_11673.pdf").To);	
	Console.WriteLine(ExtractEmailTo("(test7@cpa.co.il,test7_2@cpa.co.il;test7_3@cpa.co.il)O550970_11673.pdf").To);
}

(string To, string FileName) ExtractEmailTo(string FileName) {
    /// (test1@cpa.co.il)O550970_11673.pdf
    /// (test2@cpa.co.il;test2_2@cpa.co.il)O550970_11673.pdf
    /// (test3@cpa.co.il,test3_2@cpa.co.il)O550970_11673.pdf
    /// (test4@cpa.co.il ; test4_2@cpa.co.il)O550970_11673.pdf
    /// (test5@cpa.co.il     ,     test5_2@cpa.co.il)O550970_11673.pdf
    /// (    test6@cpa.co.il     )O550970_11673.pdf
	/// (test7@cpa.co.il,test7_2@cpa.co.il;test3_2@cpa.co.il)O550970_11673.pdf

    string ConvertMultiple(string Origin, char Seperator) {
        return string.Join(Seperator.ToString(), Origin.Split(Seperator)?.Select(o => o.Trim()) ?? Enumerable.Empty<string>());
    }

    var regex = new Regex(@"\((?<to>[\s\S]+)\)(?<file>.*)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
    var match = regex.Match(FileName);

    if (match == null || match.Groups == null) 
        return ("", FileName);

    // fix 'to' value - support multiple by ',' or ';' and remove whitespaces
    var to = match.Groups["to"].Value.Trim();

    if (to.Contains(",")) to = ConvertMultiple(to, ',');
    if (to.Contains(";")) to = ConvertMultiple(to, ';');
	to = to.Replace(";", ",");
	
    return (to, match.Groups["file"].Value.Trim());
}

/*
	OUTPUT:
	test1@cpa.co.il
	test2@cpa.co.il,test2_2@cpa.co.il
	test3@cpa.co.il,test3_2@cpa.co.il
	test4@cpa.co.il,test4_2@cpa.co.il
	test5@cpa.co.il,test5_2@cpa.co.il
	test6@cpa.co.il
	test7@cpa.co.il,test7_2@cpa.co.il,test7_3@cpa.co.il
*/