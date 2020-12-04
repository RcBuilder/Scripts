using Entities;
using System;
using System.Collections.Generic;

// Install-Package MySql.Web -Version 8.0.21
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;

namespace DistributionServiceDAL
{
    /*
        DALExtensions:
        public static class DALExtensions
        {
            public static string ToMySQLDate(this DateTime me) {
                return me.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    */

    [Obsolete("Use NotificationsDAL Instead")]
    public class MySQLNotificationsDAL : INotificationsDAL
    {
        protected string ConnStr { get; set; }

        public MySQLNotificationsDAL(string ConnStr) {
            this.ConnStr = ConnStr;
        }
        
        public async Task<List<Subscription>> GetSubscriptions()
        {   
            using (var connection = new MySqlConnection(this.ConnStr))
            {
                connection.Open();

                string query = $@"
                    SELECT  u.user_id as Id, 
		                    config.lastReceivedMessage as 'LastReceivedMessage', 
                            DATEDIFF(NOW(), config.lastReceivedMessage) as 'DiffDays',
                            IFNULL(config.indexFemale, 0) as 'IndexFemale', 
                            IFNULL(config.indexMale, 0) as 'IndexMale',
                            true as UseAudioMessage,
                            true as UseTextMessage,        
                            props.FirstName,
                            props.LastName,
                            props.NickName,
                            props.UserPhone,
                            props.Gender,
                            props.VoiceGender,
                            props.WakeupTime,
                            props.WakeupState,
                            props.IsServiceActive        
                    FROM    wp_wc_customer_lookup u 
		                    INNER JOIN (
		                    SELECT 	user_id as 'subscriptionId', 
				                    IFNULL(MAX(CASE WHEN meta_key = 'nickname' THEN meta_value END), '') as 'NickName', 
				                    IFNULL(MAX(CASE WHEN meta_key = 'first_name' THEN meta_value END), '') as 'FirstName', 
				                    IFNULL(MAX(CASE WHEN meta_key = 'last_name' THEN meta_value END), '') as 'LastName', 
				                    IFNULL(MAX(CASE WHEN meta_key = 'billing_phone' THEN meta_value END), '') as 'UserPhone',
                                    IFNULL(MAX(CASE WHEN meta_key = 'paying_customer' THEN meta_value END), 0) as 'IsPay',
				                    IFNULL(MAX(CASE WHEN meta_key = 'afreg_additional_359' THEN meta_value END), 'Male') as 'Gender',
				                    IFNULL(MAX(CASE WHEN meta_key = 'afreg_additional_361' THEN meta_value END), 'Female') as 'VoiceGender',
				                    IFNULL(MAX(CASE WHEN meta_key = 'afreg_additional_48' THEN meta_value END), '') as 'WakeupTime',
                                    IFNULL(MAX(CASE WHEN meta_key = 'afreg_additional_893' THEN meta_value END), 'allWeek') as 'WakeupState', -- allWeek or noWeekend
				                    IFNULL(MAX(CASE WHEN meta_key = 'afreg_additional_49' THEN meta_value END), 'yes') as 'IsServiceActive'                                
		                    FROM 	wp_usermeta
		                    GROUP BY user_id
		                    ORDER BY user_id
	                    ) as props ON(props.subscriptionId = u.user_id)    
	                    LEFT JOIN                             
	                    tbl_notificationsconfig config ON(config.subscriptionId = u.user_id)

                    WHERE 	props.IsServiceActive = 'yes'
                    AND 	props.UserPhone <> ''
                    AND 	(config.lastReceivedMessage IS NULL OR DATEDIFF(NOW(), config.lastReceivedMessage) <> 0)
                    AND 	props.IsPay = 1 -- IsPay
                ";

                var cmd = new MySqlCommand(query, connection);
                cmd.CommandType = CommandType.Text;

                var result = new List<Subscription>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                        while (reader.Read())
                            result.Add(new Subscription { 
                                Details = new SubscriptionDetails {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    FirstName = reader["FirstName"].ToString().Split(' ')[0],
                                    LastName = reader["LastName"].ToString(),
                                    Phone = reader["UserPhone"].ToString(),
                                },
                                WakeupSettings = new SubscriptionWakeupSettings {
                                    UseAudioMessage = Convert.ToBoolean(reader["UseAudioMessage"]),
                                    UseTextMessage = Convert.ToBoolean(reader["UseTextMessage"]),
                                    Gender = reader["Gender"]?.ToString()?.ToLower() == "female" ? eGender.Female : eGender.Male,
                                    VoiceGender = reader["VoiceGender"]?.ToString()?.ToLower() == "male" ? eVoiceType.Male : eVoiceType.Female,
                                    WakeupTime = TimeSpan.Parse(reader["WakeupTime"].ToString()),
                                    WakeupState = reader["WakeupState"]?.ToString()?.ToLower() == "allweek" ? eWakeupState.AllWeek : eWakeupState.NoWeekend,
                                    IsServiceActive = reader["IsServiceActive"]?.ToString()?.ToLower() == "yes"
                                },
                                SystemSettings = new SubscriptionSystemSettings {
                                    LastReceivedMessage = reader["LastReceivedMessage"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["LastReceivedMessage"]),
                                    IndexFemale = Convert.ToInt32(reader["IndexFemale"]),
                                    IndexMale = Convert.ToInt32(reader["IndexMale"]),
                                    IndexAlexa = 1
                                }
                            });
                }

                return result;                
            }                     
        }

        public async Task<bool> SaveConfig(int subscriptionId, SubscriptionSystemSettings systemSettings)
        {
            using (var connection = new MySqlConnection(this.ConnStr))
            {
                connection.Open();

                string query = $@"
                    -- INSERT IF NOT EXISTS
                    INSERT INTO tbl_NotificationsConfig(subscriptionId)
	                    SELECT * FROM (SELECT {subscriptionId}) as T
	                    WHERE NOT EXISTS(SELECT 1 FROM tbl_NotificationsConfig WHERE subscriptionId = {subscriptionId}) 
                        LIMIT 1;

                    UPDATE  tbl_NotificationsConfig
                    SET     lastReceivedMessage = '{systemSettings.LastReceivedMessage.Value.ToMySQLDate()}',
                            indexFemale = {systemSettings.IndexFemale},
                            indexMale = {systemSettings.IndexMale}
                    WHERE   subscriptionId = {subscriptionId};
                ";

                var cmd = new MySqlCommand(query, connection);
                cmd.CommandType = CommandType.Text;
                var changesCount = await cmd.ExecuteNonQueryAsync();
                return changesCount > 0;
            }
        }

        public async Task<bool> Save(Notification notification)
        {
            using (var connection = new MySqlConnection(this.ConnStr))
            {
                connection.Open();

                string query = $@"
                    INSERT INTO tbl_NotificationsHistory(subscriptionId, messageId, callId, createdDate)
                    VALUES({notification.SubscriptionId}, '{notification.MessageId}', '{notification.CallId}', '{DateTime.Now.ToMySQLDate()}');
                ";

                var cmd = new MySqlCommand(query, connection);
                cmd.CommandType = CommandType.Text;
                var changesCount = await cmd.ExecuteNonQueryAsync();
                return changesCount > 0;
            }
        }

        public Task<List<Notification>> Get()
        {
            throw new NotImplementedException();
        }

        public Task<List<Notification>> GetIncompleted()
        {
            throw new NotImplementedException();
        }
    }
}
