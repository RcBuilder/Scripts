using CommonEntities.Redis;
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
    public class BlackboardGrade
    {
        public const eResourceType GRADE_TYPE = eResourceType.Result;  //TODO - move to somewhere in the BlackBoardPartnerCloudEntities.cs
        public const string GRADE_STATUS = "COMPLETED";  //DITTO
        
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
        public eResourceType type { set; get; }
        [DataMember(Name = "userId")]
        public string UserId { set; get; }
        [DataMember(Name = "score")]
        public float Score { set; get; }
        //"YYYY-MM-DDThh:mm:ssZ", // item due date in ISO-8601 format (e.g. 2013-09-10T17:30:25-0500), must use RFC 822 time zone format (e.g. -0500)
        [DataMember(Name = "date")]
        public string __AttemptDate
        {

            get {                 
                var strAttemptDate = AttemptDate.ToString("yyyy-MM-ddTHH:mm:sszzz");
                return strAttemptDate.Remove(strAttemptDate.LastIndexOf(':'), 1); 
            }
            set
            {
                try
                {
                    this.AttemptDate = DateTime.Parse(value, CultureInfo.GetCultureInfo("en-US"));
                }
                catch { }
            }
        }
        public DateTime AttemptDate { get; set; }

        [DataMember(Name = "status")]
        public string Status { set; get; }
        [DataMember(Name = "instructorComments")]
        public string InstructorComments { set; get; }
        [DataMember(Name = "exempted")]
        public bool Exempted { set; get; }
        [DataMember(Name = "duration")]
        public int Duration { set; get; }
        
        [JsonProperty]
        public string AssignmentId { get; set; }
        [JsonProperty]
        public string CourseId { get; set; }
        [JsonProperty]
        public string CustomerId { get; set; }

        public override string ToString()
        {
            return string.Format("[BlackboardGrade] #{0}", this.id);
        }

        public BlackboardGrade()
        {
            this.type = eResourceType.Item;
            this.AssignmentId = string.Empty;
            this.CourseId = string.Empty;
            this.CustomerId = string.Empty;
        }

        public static explicit operator BlackboardGrade(AAIRSCommon.ScorableItemsData.Score score)
        {
            var grade = new BlackboardGrade();
            grade.id = score.AttemptId;
            grade.AssignmentId = score.AssignmentID;
            grade.CourseId = score.CourseID;
            grade.CustomerId = score.Customer_Number;
            grade.type = GRADE_TYPE;
            grade.UserId = score.UserID; //Elad: blackboard user id is Tool user id.
            Single scoreReceived;
            if (!Single.TryParse(score.ScoreReceived, out scoreReceived))
                throw new Exception(string.Format("Unable to parse score {0} to a Single", score.ScoreReceived));
            else
                grade.Score = scoreReceived;
            grade.AttemptDate = score.DateTimeSubmitted;
            grade.Status = GRADE_STATUS;
            grade.InstructorComments = score.Comment;
            grade.Exempted = false;
            

            // No need to provide Blackboard with duration
            //if (score.DateTimeStarted > DateTime.MinValue && score.DateTimeStarted < DateTime.MaxValue &&
            //    score.DateTimeSubmitted > DateTime.MinValue && score.DateTimeSubmitted < DateTime.MaxValue &&
            //    score.DateTimeSubmitted > score.DateTimeStarted)
            //{
            //    grade.Duration = Convert.ToInt32(Math.Round((score.DateTimeSubmitted - score.DateTimeStarted).TotalSeconds, 0));
            //}

            /*
            grade.CacheAssignmentId = CacheSchema.KeyBlackboardAssignment(typeof(BlackboardAssignment),
                score.Customer_Number, score.CourseID, score.AssignmentID).KeyString;
            */
            return grade;
        }
    }

    public enum eGradeStatus { NULL, FAILURE, SUCCESS, LOST }
    public class GradeStatus {

        [JsonProperty]
        public string GradeRedisKey { set; get; }

        [JsonProperty]
        public eGradeStatus Status = eGradeStatus.NULL;
    }
}
