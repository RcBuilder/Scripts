using AAIRSCommon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CommonEntities.BlackBoardPartnerCloudEntities
{
    [DataContract]
    public class BlackboardAssignment
    {
        [DataMember(Name = "@id")]
        public string id { get; set; }
        [DataMember(Name = "@type")]
        public string __type{
            set{
                try{
                    this.type = (eResourceType)Enum.Parse(typeof(eResourceType), value, true);
                }
                catch{
                    this.type = default(eResourceType);
                }
            }
            get { return this.type.ToString(); }
        }
        public eResourceType type { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public UriEntity targetUri { get; set; }

        [DataMember(Name = "category")]
        public string gradebookColumnCategory { get; set; }

        //"YYYY-MM-DDThh:mm:ssZ", // item due date in ISO-8601 format (e.g. 2013-09-10T17:30:25-0500), must use RFC 822 time zone format (e.g. -0500)
        [DataMember(Name = "dueDate")]
        public string __dueDate
        {
            get {                 
                var strDueDate = DueDate.ToString("yyyy-MM-ddTHH:mm:sszzz");
                return strDueDate.Remove(strDueDate.LastIndexOf(':'), 1); 
            }
            set {
                try
                {
                    this.DueDate = DateTime.Parse(value, CultureInfo.GetCultureInfo("en-US"));
                }
                catch { }
            }
        }
        public DateTime DueDate { get; set; } 

        [DataMember]
        public float pointsPossible { get; set; }
        [DataMember]
        public string gradingSchema { get; set; } // eGradingSchema
        [DataMember]
        public bool includeInTotal { get; set; }
        [DataMember]
        public bool visibility { get; set; }

        [DataMember(Name = "custom")] 
        public customParameterMap custom;

        public string RequestId { get; set; }
        public string ProviderId { get; set; }

        [JsonProperty]
        public string CourseId { get; set; }
        [JsonProperty]
        public string CustomerId { get; set; }

        public BlackboardAssignment() {
            this.type = eResourceType.Item;
            this.CourseId = string.Empty;
            this.CustomerId = string.Empty;
        }

        #region Converters:
        public static explicit operator BlackboardAssignment(ScorableItemsData.ScorableItem scorableItem)
        {
            var result = new BlackboardAssignment
            {
                id = scorableItem.AssignmentID,
                title = scorableItem.AssignmentTitle,
                description = scorableItem.Description,
                DueDate = scorableItem.DueDateTime,
                includeInTotal = scorableItem.IsIncludedInGrade,
                pointsPossible = Convert.ToSingle(scorableItem.ScorePossible),
                gradebookColumnCategory = scorableItem.Category,
                visibility = scorableItem.IsStudentViewable,
                custom = new customParameterMap() { ToolId = "Simnet" },
                targetUri = new UriEntity 
                {   
                    type = eResourceType.Uri, 
                    scheme = eUriScheme.ACTION, 
                    uri = "action:openItem", 
                    id = DateTime.UtcNow.Ticks.ToString() /*an arbitrary val so satisy JSON LD compliance*/ 
                },
                CustomerId = scorableItem.Customer_Number, 
                CourseId = scorableItem.CourseID
            };

            switch (scorableItem.ScoreType)
            {
                case ScorableItemsData.eScoreType.Percentage: result.gradingSchema = "PERCENT";
                    break;
                case ScorableItemsData.eScoreType.Points: result.gradingSchema = "SCORE";
                    break;
            }

            return result;
        }
        #endregion

        public override string ToString()
        {
            return string.Format("[BlackboardAssignment] #{0}", this.id); 
        }
    }
}
