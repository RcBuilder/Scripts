using Entities;
using System.Text.RegularExpressions;

namespace Scrapper
{
    public class DateParser : RegexParser
    {
        protected const string MONTHS_EN = "January|Jan|February|Feb|March|Mar|April|Apr|May|June|Jun|July|Jul|August|Aug|September|Sep|October|Oct|November|Nov|December|Dec";        
        public DateParser() : base($@"({MONTHS_EN})? \s+ \d{{1,2}},? \s+ \d{{4}}") { }
        // $@"(?<=born \s+ ((on) \s+)?) ({MONTHS_EN})? \s+ \d{{1,2}},? \s+ \d{{4}}"
    }
}
