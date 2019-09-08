using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LagojaMarketingServices
{
    /*  USING:
               
        Parse(string strCSV)
        Load(string filePath)

        ---------------------

        // source.csv
        Email,Domain,PH1,PH2,PH3
        a@a.com,http://a.com,a1,a2,a3
        b@b.com,http://b.com,b1,b2,b3
        c@c.com,http://c.com,c1,c2,c3
        d@d.com,http://d.com,d1,d2,d3
        
        ---
        
        var csvFile = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\", "source.csv");
        var csvContent = App_Code.CsvParser.Load(csvFile);
        if (csvContent == null)
            return; 
     
        foreach (var row in csvContent.Rows.Where(x => !x.IsHeader)) {                
            Console.WriteLine("email:{0}", row["Email"].Value);
            Console.WriteLine("domain:{0}", row["Domain"].Value);
        }
    */
    
    public class CsvParser {
        
        public class CsvContent {
            public List<CsvRow> Rows { get; set; }

            public CsvContent() {
                this.Rows = new List<CsvRow>();
            }

            public void Append(CsvContent other) {
                this.Rows.AddRange(other.Rows);
            }
        }

        public class CsvRow {
            public List<CsvColumn> Columns { get; set; }
            public bool IsHeader { get; set; }

            public CsvRow(){
                this.Columns = new List<CsvColumn>();
            }

            public CsvColumn this[string Name]{
                get{
                    try {
                        if (string.IsNullOrEmpty(Name))
                            return null;
                        return this.Columns.FirstOrDefault(x => x.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));
                    }
                    catch { return null; }
                }
            } 
        }

        public class CsvColumn {
            public string Name { get; set; }
            public string Value { get; set; }

            public static implicit operator CsvColumn(string value) {
                return new CsvColumn {
                    Value = value,
                    Name = value
                };
            }
        }


        public static CsvContent Parse(string strCSV, bool contentOnly = false, char SEPERATOR = ',') {
            try {                
                if (string.IsNullOrEmpty(strCSV)) return null;
                
                var rows = strCSV.Replace("\r", string.Empty).Split('\n');
                if (rows.Length == 0) return null;

                var csvContent = new CsvContent();

                // special case, seperator as part of a content (which means that the content is wrapped with quotes "")
                // replace it to a temporary placeholder to prevent split issues and later return it back
                var patternPH = string.Format("(?<= \" [^\"]+? ) \\{0} (?= [^\"]+? \" )", SEPERATOR);
                var tempPH = "#PH#";

                // handle double quotes
                /// var patternDQ = "\"{2}";
                var tempDQ = "#DQ#";

                for (var i = 0; i < rows.Length; i++) {
                    var row = rows[i];

                    var hasDQ = row.Contains("\"\"");

                    // temporary remove DQ to prevent it from damaging the PH matches
                    if (hasDQ) row = row.Replace("\"\"", tempDQ);
                    // use temporary PH to prevent the content to be splitted incorrectly
                    row = Regex.Replace(row, patternPH, tempPH, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);                    
                    if (hasDQ) row = row.Replace(tempDQ, "\"\"");

                    rows[i] = row;
                }

                CsvRow csvHeaderRow = null;
                if (!contentOnly) {
                    // header
                    csvHeaderRow = new CsvRow {
                        IsHeader = true,
                    };

                    foreach (var col in rows.First().Split(SEPERATOR))
                        csvHeaderRow.Columns.Add(col.Trim().Replace(tempPH, SEPERATOR.ToString()));
                    csvContent.Rows.Add(csvHeaderRow);
                }

                // body
                foreach (var row in rows.Skip(1)) {
                    var csvRow = new CsvRow();
                    var cols = row.Split(SEPERATOR);

                    // empty rows
                    if (cols.Length == 0 || (cols.Length == 1 && string.IsNullOrWhiteSpace(cols[0])))
                        continue;

                    for (int i = 0; i < cols.Length; i++)
                        csvRow.Columns.Add(new CsvColumn {
                            Name = csvHeaderRow == null ? "" : csvHeaderRow.Columns[i].Value, // set header name as the name of the column - for indexer row["Email"] etc.
                            Value = cols[i].Trim().Replace(tempPH, SEPERATOR.ToString())
                        });                    

                    csvContent.Rows.Add(csvRow);
                }
                return csvContent;
            }
            catch { return null; }
        }

        public static CsvContent Load(string filePath, bool contentOnly = false, char SEPERATOR = ',') {
           return CsvParser.Parse(File.ReadAllText(filePath, Encoding.UTF8), contentOnly, SEPERATOR);
        }
    }
}
