<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Threading.Tasks.dll</Reference>
  <Namespace>System.Threading</Namespace>
</Query>

/*
	public static readonly string[] MONTHS_HE = new string[] { 
        "ינואר", 
        "פברואר", 
        "מרץ", 
        "אפריל", 
        "מאי", 
        "יוני", 
        "יולי", 
        "אוגוסט", 
        "ספטמבר", 
        "אוקטובר", 
        "נובמבר", 
        "דצמבר" 
    };

	// Build Period List
	// Monthly Or Bi-Monthly
	(List<SelectJs> Options, int SelectedIndex) GetPeriods(int periodType) {
        var periodsList = new List<string>();

        var c = 1;
        if (periodType == 2) // Bi-monthly
        {                
            for (var i = 0; i < MONTHS_HE.Length; i+=2)
                periodsList.Add($"{MONTHS_HE[i]}-{MONTHS_HE[i+1]}");
        }
        else   // Monthly
        {
            for (var i = 0; i < MONTHS_HE.Length; i++)
                periodsList.Add(MONTHS_HE[i]);
        }
*/


static string[] lstPeriod1 = new string[] {
	"ינואר", "פברואר", "מרץ", "אפריל", "מאי", "יוני", "יולי", "אוגוסט", "ספטמבר", "אוקטובר", "נובמבר", "דצמבר" 
};
static string[] lstPeriod2 = new string[] {
	"ינואר-פברואר", "מרץ-אפריל", "מאי-יוני", "יולי-אוגוסט", "ספטמבר-אוקטובר", "נובמבר-דצמבר"
};


/// Find Selected Period - Supports Monthly Or Bi-Monthly
void Main()
{	
	foreach(var m in Enumerable.Range(1, 12))
		Console.WriteLine($"M = {m} | P = {1} | R = {lstPeriod1[GetSelectedPeriod(m, 1) - 1]}");	
	
	Console.WriteLine("----");
	
	foreach(var m in Enumerable.Range(1, 12))
		Console.WriteLine($"M = {m} | P = {2} | R = {lstPeriod2[GetSelectedPeriod(m, 2) - 1]}");	
		
	Console.WriteLine("----");	
		
	var dt = new DateTime(2023, 1, 3);
    if (dt.Day < 20) dt = dt.AddMonths(-1);   // for VAT Authority - present previous period till the 20th 
	Console.WriteLine($"M = {dt.Month} | P = {1} | R = {lstPeriod1[GetSelectedPeriod(dt.Month, 1) - 1]}");
	Console.WriteLine($"M = {dt.Month} | P = {2} | R = {lstPeriod2[GetSelectedPeriod(dt.Month, 2) - 1]}");
	
	Console.WriteLine("----");	
	
	dt = new DateTime(2023, 1, 23);
    if (dt.Day < 20) dt = dt.AddMonths(-1);   // for VAT Authority - present previous period till the 20th 
	Console.WriteLine($"M = {dt.Month} | P = {1} | R = {lstPeriod1[GetSelectedPeriod(dt.Month, 1) - 1]}");
	Console.WriteLine($"M = {dt.Month} | P = {2} | R = {lstPeriod2[GetSelectedPeriod(dt.Month, 2) - 1]}");
}   

int GetSelectedPeriod(int Month, int PeriodType) {	
	if(Month == 1) return Month;	  // January
	if(PeriodType == 1) return Month; // Monthly
	return (int)Math.Ceiling((decimal)Month / PeriodType);	
}