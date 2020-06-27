using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon;
using Amazon.ElastiCache;
using Amazon.ElastiCache.Model;

namespace AWSRedisUtility
{
    /* TODO 
        var indexA = this.redisClient.AddToList<string>("ListA", "AAAA");
        var indexB = this.redisClient.AddToList<string>("ListA", "BBBB");
        var indexC = this.redisClient.AddToList<string>("ListA", "CCCC");
            
        var deleted = this.redisClient.RemoveFromList("ListA", "AAAA");

        var top = this.redisClient.GetTopXFromList<string>("ListA", 20);
        lstResult.Items.Add(top.Count());

        var added = this.redisClient.AddToUniqueList<string>("UniqueListC", "1");
        added = this.redisClient.AddToUniqueList<string>("UniqueListC", "2");
        added = this.redisClient.AddToUniqueList<string>("UniqueListC", "3");
        added = this.redisClient.AddToUniqueList<string>("UniqueListC", "4");
        added = this.redisClient.AddToUniqueList<string>("UniqueListC", "5");
        added = this.redisClient.AddToUniqueList<string>("UniqueListC", "6");
        added = this.redisClient.AddToUniqueList<string>("UniqueListC", "7");
        added = this.redisClient.AddToUniqueList<string>("UniqueListC", "7");
        added = this.redisClient.AddToUniqueList<string>("UniqueListC", "7");
        added = this.redisClient.AddToUniqueList<string>("UniqueListC", "8");
        added = this.redisClient.AddToUniqueList<string>("UniqueListC", "10");
        added = this.redisClient.AddToUniqueList<string>("UniqueListC", "9");

        var added = this.redisClient.AddToUniqueList<string>("UniqueListC", new string[] { "A", "B", "B", "D", "C", "E", "F", "F", "G" });
        var result = this.redisClient.GetTopXFromUniqueList<string>("UniqueListC", 5); 
     
        -----------------------
     
        var action1 = new RedisTransactionAction(eRedisTransactionActionType.Set, "KEY_AAA", "VALUE_AAA");
        action1.Properties.Add("EXPIRY", DateTime.Now.AddDays(2));

        var user = new Entities.User { Id = 500, Name = "User 500", Age = 50 };
        var action2 = new RedisTransactionAction(eRedisTransactionActionType.Set, "KEY_BBB", user);
        action2.Properties.Add("EXPIRY", DateTime.Now.AddDays(2));

        var action3 = new RedisTransactionAction(eRedisTransactionActionType.AddToUniqueList, "LIST_1", "KEY_AAA");
        var action4 = new RedisTransactionAction(eRedisTransactionActionType.AddToUniqueList, "LIST_1", "KEY_BBB");

        var result = this.redisClient.MakeTransaction(action1, action2, action3, action4);
        lstResult.Items.Add(result);

        var keyA = this.redisClient.Get<string>("KEY_AAA");
        var keyB = this.redisClient.Get<Entities.User>("KEY_BBB");

        lstResult.Items.Add(keyA);
        lstResult.Items.Add(keyB);
    */



    public partial class Form1 : Form
    {
        private string RedisEndpoint = ConfigurationManager.AppSettings["RedisEndpoint"];
        private string RedisPort = ConfigurationManager.AppSettings["RedisPort"];

        private IAmazonElastiCache elastiCacheClient;
        private IRedisClient redisClient;

        private List<Entities.User> users = new Entities.User[] { 
                new Entities.User { Id = 1, Name = "User A", Age = 31 }, 
                new Entities.User { Id = 2, Name = "User B", Age = 32 }, 
                new Entities.User { Id = 3, Name = "User C", Age = 33 }, 
                new Entities.User { Id = 4, Name = "User D", Age = 34 }, 
                new Entities.User { Id = 5, Name = "User E", Age = 35 }
            }.ToList();

        public Form1()
        {
            InitializeComponent();

            this.elastiCacheClient = AWSClientFactory.CreateAmazonElastiCacheClient();

            try
            {
                //this.redisClient = new ServiceStackRedisClient(RedisEndpoint, Convert.ToInt32(RedisPort));
                this.redisClient = new StackExchangeRedisClient(RedisEndpoint, Convert.ToInt32(RedisPort));
            }
            catch(Exception ex) {
                MessageBox.Show(string.Format("Can't load redis Client! {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cmbUsers.DataSource = users;
            cmbUsers.DisplayMember = "Name";
            cmbUsers.ValueMember = "Id";
        }

        private void btnClusters_Click(object sender, EventArgs e)
        {
            var request = new DescribeCacheClustersRequest();
            var response = this.elastiCacheClient.DescribeCacheClusters(request);

            if (response == null || response.CacheClusters.Count == 0)
            {
                lstResult.Items.Add("No Clusters Found!");
                return;
            }

            foreach (var cluster in response.CacheClusters.Where(c => string.Equals(c.Engine, "redis", StringComparison.OrdinalIgnoreCase)))
                lstResult.Items.Add(cluster.CacheClusterId);  
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            lstResult.Items.Clear();

            var key = chkIsPOCO.Checked ? string.Concat("user", cmbUsers.SelectedValue) : txtKey.Text.Trim();
            if (key == string.Empty) {
                lstResult.Items.Add("Key is Missing!");
                return;
            }

            lstResult.Items.Add(string.Format("adding {0}", key));

            bool result = false;
            if(chkIsPOCO.Checked)
            {
                result = this.redisClient.Set<Entities.User>(key, users.Single(x => x.Id == Convert.ToInt32(cmbUsers.SelectedValue)));
            }
            else{
                var value = txtValue.Text.Trim();
                if (value == string.Empty)
                {
                    lstResult.Items.Add("value is Missing!");
                    return;
                }
                result = this.redisClient.Set<string>(key, value);
            }
            
            lstResult.Items.Add(result ? "success" : "failure");                        
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            lstResult.Items.Clear();

            var key = chkIsPOCO.Checked ? string.Concat("user", cmbUsers.SelectedValue) : txtKey.Text.Trim();
            if (key == string.Empty)
            {
                lstResult.Items.Add("Key is Missing!");
                return;
            }

            lstResult.Items.Add(string.Format("removing {0}", key));
            var result = this.redisClient.Remove(key);
            lstResult.Items.Add(result ? "success" : "failure");
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            lstResult.Items.Clear();

            var key = chkIsPOCO.Checked ? string.Concat("user", cmbUsers.SelectedValue) : txtKey.Text.Trim();
            if (key == string.Empty)
            {
                lstResult.Items.Add("Key is Missing!");
                return;
            }

            lstResult.Items.Add(string.Format("getting {0}", key));

            dynamic value;
            if (chkIsPOCO.Checked)
                value = this.redisClient.Get<Entities.User>(key);
            else
                value = this.redisClient.Get<string>(key);

            if (value == null)
            {
                lstResult.Items.Add("value is NULL!");
                return;
            }

            lstResult.Items.Add(value);
        }

        private void btnGetAll_Click(object sender, EventArgs e)
        {
            lstResult.Items.Clear();

            var values = this.redisClient.GetAll<string>();

            var userKeys = this.redisClient.SearchKeys("user*");
            var users = this.redisClient.GetMultiple<Entities.User>(userKeys);
            lstResult.Items.Add(string.Format("{0} items found", values.Count() + users.Count()));

            foreach (var value in values)
                lstResult.Items.Add(value);

            foreach (var user in users)
                lstResult.Items.Add(user);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lstResult.Items.Clear();

            var phrase = txtPhrase.Text.Trim();

            if (phrase == string.Empty)
            {
                lstResult.Items.Add("phrase is Missing!");
                return;
            }
            
            var keys = this.redisClient.SearchKeys(string.Format("*{0}*", phrase));            
            foreach (var key in keys)
                lstResult.Items.Add(key);
        }

        private void chkIsPOCO_CheckedChanged(object sender, EventArgs e)
        {
            groupPOCO.Enabled = chkIsPOCO.Checked;
            groupKeyValue.Enabled = !groupPOCO.Enabled;
        }

        private void btnGetAllKeys_Click(object sender, EventArgs e)
        {
            lstResult.Items.Clear();

            var keys = this.redisClient.GetAllKeys();
            lstResult.Items.Add(string.Format("{0} keys found", keys.Count()));

            foreach (var key in keys)
                lstResult.Items.Add(key);
        }
        
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.elastiCacheClient.Dispose();

            if (this.redisClient != null)
                this.redisClient.Dispose();
        }

        private void btnFlushALL_Click(object sender, EventArgs e)
        {
            lstResult.Items.Clear();

            if (!chkFlushALL.Checked)
            {
                lstResult.Items.Add("must confirm this action!");
                return;
            }

            lstResult.Items.Add("flushing ALL");
            this.redisClient.FlushALL();
        }
    }
}
