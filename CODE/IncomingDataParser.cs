using System;
using System.Collections.Generic;
using System.Data;

namespace DataParsers
{
    /*        
        var IncomingParser = new IncomingDataParser(<SourceTable>)); 

        IncomingParser.TotalCalls;
        IncomingParser.TotalTalking;
        DataParser.Minutes2Time(IncomingParser.AverageTalkingAsMinutes);
    */
    public class IncomingDataParser : OutgoingDataParser
    {
        public IncomingDataParser(DataTable SourceTable) : base(SourceTable) { }
        
        public override int TotalAnswered {
            get {
                return this.CountBySearchValue("answered", 4);
            }
        }

        public override int TotalUnAnswered {
            get {                
                return this.CountBySearchValue("unanswered", 4);
            }
        }
    }
}
