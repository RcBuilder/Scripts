using System;
using System.Collections.Generic;
using System.Data;

namespace DataParsers
{
    /*
        var OutgoingParser = new OutgoingDataParser(<SourceTable>));

        OutgoingParser.TotalCalls;
        OutgoingParser.TotalTalking;
        DataParser.Minutes2Time(OutgoingParser.AverageTalkingAsMinutes);
    */
    public class OutgoingDataParser : DataParser
    {
        public OutgoingDataParser(DataTable SourceTable) : base(SourceTable) {}

        public int TotalCalls {
            get {
                return this.TotalAnswered + this.TotalUnAnswered;
            }
        }

        public string TotalRinging {
            get{
                return this.GetValue(this.LastRowIndex - 1, 5);
            }
        }

        public string TotalTalking {
            get {
                return this.GetValue(this.LastRowIndex - 1, 6);
            }
        }

        public double TotalTalkingAsMinutes {
            get {
                return Math.Round(DataParser.Time2Minutes(this.TotalTalking), 2);
            }
        }

        public double AverageTalkingAsMinutes
        {
            get
            {
                if (this.TotalAnswered == 0) return 0;
                return Math.Round(this.TotalTalkingAsMinutes / this.TotalAnswered, 2);
            }
        }

        public string TotalDuration {
            get {
                return this.GetValue(this.LastRowIndex - 1, 7);
            }
        }

        public virtual int TotalAnswered {
            get {
                return this.CountBySearchValue("answered", 4);
            }
        }

        public virtual int TotalUnAnswered {
            get
            {
                return this.CountBySearchValue("unanswered", 4);
            }
        }
    }
}
