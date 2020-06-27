using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonEntities.Cache
{
    public class BlackboardAssignmentCacheKey : ICacheKey
    {
        private string customerId { set; get; }
        private string courseId { set; get; }
        private string assignmentId { set; get; }

        public BlackboardAssignmentCacheKey(string customerId, string courseId, string assignmentId)
        {
            this.customerId = customerId;
            this.courseId = courseId;
            this.assignmentId = assignmentId;
        }

        public string CacheKey
        {
            get { return string.Concat("BlackboardAssignment_", this.customerId, "_", this.courseId, "_", this.assignmentId); }
        }
    }

    public class BlackboardGradeCacheKey : ICacheKey
    {
        private string customerId { set; get; }
        private string courseId { set; get; }
        private string assignmentId { set; get; }
        private string gradeId { set; get; }

        public BlackboardGradeCacheKey(string customerId, string courseId, string assignmentId, string gradeId)
        {
            this.customerId = customerId;
            this.courseId = courseId;
            this.assignmentId = assignmentId;
            this.gradeId = gradeId;
        }

        public string CacheKey
        {
            get { return string.Concat("BlackboardGrade_", this.customerId, "_", this.courseId, "_", this.assignmentId, "_", this.gradeId); }
        }
    }

    public class BlackboardGradesListCacheKey : ICacheKey
    {
        private string gradesState { set; get; }

        public BlackboardGradesListCacheKey(string gradesState)
        {
            this.gradesState = gradesState;
        }

        public string CacheKey
        {
            get { return string.Concat("BlackboardGradesList_", this.gradesState); }
        }
    }
}
