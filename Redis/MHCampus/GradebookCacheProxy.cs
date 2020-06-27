using AWSCommon.Redis;
using CommonEntities.BlackBoardPartnerCloudEntities;
using CommonEntities.Redis;
using CustomIntegrationModules.Interfaces;
using MHCampus.Common;
using MHCommon.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tegrity.Utils;

namespace CustomIntegrationModules.Blackboard
{
    //todo - document the functions
    public class GradebookCacheProxy : IBlackboardCacheProxy
    {
        public List<BlackboardAssignment> GetAssignments(string CustomerId, string CourseId, string assignmentId) {
            return GetAssignments(CustomerId, CourseId, new List<string> { assignmentId });   
        }
        public List<BlackboardAssignment> GetAssignments(string CustomerId, string CourseId, List<string> assignmentIds)
        {
            var cacheKeys = new List<ICacheKey>();
            foreach (string assId in assignmentIds)
            {
                var cacheKey = CacheSchema.KeyBlackboardAssignment(typeof(BlackboardAssignment), CustomerId, CourseId, assId);
                cacheKeys.Add(cacheKey);
            }

            var assignments = new List<BlackboardAssignment>();
            using (var redisCacheManager = new RedisCacheManager())
            {
                assignments = redisCacheManager.GetMultiple<BlackboardAssignment>(cacheKeys).ToList();
            }

            return assignments;
        }

        public bool SaveAssignment(BlackboardAssignment assignment)
        {
            if (assignment == null)
                return false;

            var cacheKey = CacheSchema.KeyBlackboardAssignment(typeof(BlackboardAssignment), assignment.CustomerId, assignment.CourseId, assignment.id);

            var DueDate = assignment.DueDate <= DateTime.Now.AddHours(2) ? DateTime.Now.AddHours(2) : assignment.DueDate;
            var ExpiryDate = DueDate.AddMinutes(CommonConfig.Cache.LowCacheTimeOutInMinutes);
            using (var redisCacheManager = new RedisCacheManager())
            {
                redisCacheManager.Set(cacheKey, assignment, ExpiryDate);
            }

            return true;
        }

        public List<string> PeekGrades(int RowsCount)
        {
            var unpulledGradesListCacheKey = CacheSchema.SortedSets.KeyBlackboardGradeUnpulled();

            using (var redisCacheManager = new RedisCacheManager())
            {
                var gradesKeys = redisCacheManager.GetTopXFromUniqueList<string>(unpulledGradesListCacheKey, RowsCount - 1);
                return gradesKeys.ToList();
            }
        }

        public List<BlackboardGrade> GetGrades(int RowsCount)
        {
            var grades = new List<BlackboardGrade>();
            var unpulledGradesListCacheKey = CacheSchema.SortedSets.KeyBlackboardGradeUnpulled();
            var pulledGradesListCacheKey = CacheSchema.SortedSets.KeyBlackboardGradePulled(RequestIdManager.RequestId);
            var pulledGradesHistoryListCacheKey = CacheSchema.SortedSets.KeyBlackboardGradePulledHistory();
            
            using (var redisCacheManager = new RedisCacheManager())
            {
                var gradesKeys = PeekGrades(RowsCount);
                var gradesCacheKeys = gradesKeys.Select(x => new SimpleCacheKey(x)).ToList();
                grades = redisCacheManager.GetMultiple<BlackboardGrade>(gradesCacheKeys).ToList();

                // transfer keys from UNPULLED list to PULLED list
                // + add the current PULLED list key to the PULLED History list (to avoid the search on 'SearchPulledGradesLists')
                if (grades.Count > 0)
                {
                    var actionAddGradeKeysToPulledList = new RedisTransactionAction(eRedisTransactionActionType.AddToUniqueList, pulledGradesListCacheKey, gradesKeys);
                    var actionRemoveGradeKeysFromUnpulledList = new RedisTransactionAction(eRedisTransactionActionType.RemoveFromUniqueList, unpulledGradesListCacheKey, gradesKeys);
                    var actionAddPulledListToHistory = new RedisTransactionAction(eRedisTransactionActionType.AddToUniqueList, pulledGradesHistoryListCacheKey, new List<string>{ pulledGradesListCacheKey.KeyString });
                    redisCacheManager.MakeTransaction(actionAddGradeKeysToPulledList, actionRemoveGradeKeysFromUnpulledList, actionAddPulledListToHistory);
                }
            }

            return grades;
        }

        public bool SaveGrade(BlackboardGrade grade)
        {
            if (string.IsNullOrWhiteSpace(grade.CustomerId))
                throw new ArgumentNullException("parameter customerId is null/whitespace");
            if (string.IsNullOrWhiteSpace(grade.CourseId))
                throw new ArgumentNullException("parameter courseId is null/whitespace");
            if (string.IsNullOrWhiteSpace(grade.AssignmentId))
                throw new ArgumentNullException("parameter assignmentId is null/whitespace");
            if (grade == null)
                throw new ArgumentNullException("parameter grade is null");

            var gradeCacheKey = CacheSchema.KeyBlackboardGrade(typeof(BlackboardGrade), grade.CustomerId, grade.CourseId, grade.AssignmentId, grade.id);
            var unpulledGradesListCacheKey = CacheSchema.SortedSets.KeyBlackboardGradeUnpulled();

            var gradeExpiryOnRedisInDays = Convert.ToInt32(CloudConfig.Get("BLACKBOARD_GRADE_EXPIRY_ON_REDIS_IN_DAYS", "7"));
            var ExpiryDate = DateTime.Now.AddDays(gradeExpiryOnRedisInDays).AddMinutes(CommonConfig.Cache.LowCacheTimeOutInMinutes);            

            //redisCacheManager.Set(gradeCacheKey, grade, ExpiryDate);
            var actionSetGrade = new RedisTransactionAction(eRedisTransactionActionType.Set, gradeCacheKey, grade);
            actionSetGrade.Properties.Add("EXPIRY", ExpiryDate);

            //redisCacheManager.AddToUniqueList(unpulledGradesListCacheKey, gradeCacheKey.CacheKey);
            var actionAddGradeKeyToList = new RedisTransactionAction(eRedisTransactionActionType.AddToUniqueList, unpulledGradesListCacheKey, new List<string> { gradeCacheKey.KeyString });

            using (var redisCacheManager = new RedisCacheManager())
                return redisCacheManager.MakeTransaction(actionSetGrade, actionAddGradeKeyToList);
        }

        public bool SaveLatestUpdatesRequestId() {
            var cacheKey = CacheSchema.KeyBlackboardGetUpdatesLatestRequestId();
            using (var redisCacheManager = new RedisCacheManager())
            {
                redisCacheManager.Set<string>(cacheKey, RequestIdManager.RequestId);
            }

            return true;
        }

        public string GetLatestUpdatesRequestId()
        {
            var cacheKey = CacheSchema.KeyBlackboardGetUpdatesLatestRequestId();
            using (var redisCacheManager = new RedisCacheManager())            
                return redisCacheManager.Get<string>(cacheKey);            
        }

        public List<string> PopPulledGradesKeys(string RequestId, int range)
        {
            var gradesKeys = new List<string>();
            var pulledGradesListCacheKey = CacheSchema.SortedSets.KeyBlackboardGradePulled(RequestId);
            var pulledGradesHistoryListCacheKey = CacheSchema.SortedSets.KeyBlackboardGradePulledHistory();

            using (var redisCacheManager = new RedisCacheManager())
            {
                gradesKeys = redisCacheManager.GetTopXFromUniqueList<string>(pulledGradesListCacheKey, range).ToList();
                if (gradesKeys != null)
                {
                    // remove the PULLED list and the PULLED HISTORY list (which is a List of ALL the PULLED lists keys)
                    redisCacheManager.Remove(pulledGradesListCacheKey);
                    redisCacheManager.RemoveFromUniqueList<string>(pulledGradesHistoryListCacheKey, pulledGradesListCacheKey.KeyString);
                }
            }

            return gradesKeys;
        }

        /*
        public bool SaveRequestGrades(List<string> gradeKeys)
        {
            var pulledGradesListCacheKey = CacheSchema.SortedSets.KeyBlackboardGradePulled(RequestIdManager.RequestId);

            using (var redisCacheManager = new RedisCacheManager())
                return redisCacheManager.AddToUniqueList<List<string>>(pulledGradesListCacheKey, gradeKeys);
        }
        */

        public bool SaveGradesStatuses(string RequestId, List<GradeStatus> gradeStatuses)
        {
            var statusGradesListCacheKey = CacheSchema.SortedSets.KeyBlackboardGradeStatus(RequestId);
            using (var redisCacheManager = new RedisCacheManager())
                return redisCacheManager.AddToUniqueList<List<GradeStatus>>(statusGradesListCacheKey, gradeStatuses);
        }

        /*
        public List<string> SearchPulledGradesLists()
        {
            // send an empty request id in order to get only the key prefix!
            var prefix = CacheSchema.SortedSets.KeyBlackboardGradePulled(string.Empty); 
            using (var redisCacheManager = new RedisCacheManager())
                return redisCacheManager.SearchKeys(string.Format("{0}*", prefix.KeyString)).ToList();//todo: find a way without doing key searches - suggest changing the cache schema.
        }
        */

        public List<string> SearchPulledGradesLists()
        {
            var pulledGradesHistoryListCacheKey = CacheSchema.SortedSets.KeyBlackboardGradePulledHistory();
            using (var redisCacheManager = new RedisCacheManager())
                return redisCacheManager.GetAllFromUniqueList<string>(pulledGradesHistoryListCacheKey).ToList();
        }
    }
}