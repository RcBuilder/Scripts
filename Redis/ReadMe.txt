// TODO: re-arrange the data, split it into Redis mechanism and AWS ElastiCache instance
//		 in addition, document both StackExchange and ServiceStack clients 


Redis commands
--------------
http://redis.io/commands

redis-cli.exe
-------------
redis client implementation instance

Commands:
- FLUSHALL // clear all 
- type [key] // get the redis type of the specified key (set, string, list etc.)

Redis vs Memcached
------------------
- Memcached is more mature (since 2003) whereas Redis developed in 2009
- Both serve as in-memory, key-value data stores
- Redis has more features and thus make it more flexible
- Memcached supports only objects as serialized strings whereas Redis is Data Structered and can store object's fields and values.
- Redis supports Hashes, Sorted Sets, Lists ...
- Redis can store values up to 512MB in size whereas memcached limited to 1MB per key
- Memcached is multithreaded whereas Redis is a single core events driven
- Redis supports a Disk Persistence
- Redis supports Replication
- Auto Discovery is ONLY available for cache clusters running Memcached

Auto Discovery:
---------------
ElastiCache supports Auto Discovery.
ability to automatically identify all of the nodes in a cache cluster.
access to a cluster (which can consists of a lot of nodes) instead of a specific node.
a client program only needs to connect to the config endpoint. after that, the Auto Discovery library connects asynchronously to all of the other nodes in the cache cluster. 


AWS ElastiCache:
----------------
note! need to use a client utility in order to store and get items to and from the cluster via the app

---

access:
- if the EC2 Instance is EC2-VPC the ElastiCache will be accessable only from within the VPC 
- if the EC2 Instance is EC2-Classic and shared as public the ElastiCache will be accessable from anywhere 

note! we have to open the suitable ports in the security group 

---

Define a cache service:
* open AWS console
* choose 'ElastiCache' service
* (menu item) Cache Clusters -> Launch cache cluster
* choose 'Redis' engine -> next 
  note! can also choose 'Memcached' engine
* enable the 'Enable Replication' option and set a cluster name 
* choose cache.t2.micro as the nodeType (free) -> next
* set the security group (have to be accessable)

---

cluster endpoints:
* open AWS console
* choose 'ElastiCache' service
* (menu item) Cache Clusters
* Memcached: 
  get the enpoint from the presented cluster list
* Redis:
  click on nodes -> and get the endpoint from the node level

---

Example:
// get the clusters list
var client_Cache = AWSClientFactory.CreateAmazonElastiCacheClient();
var request = new DescribeCacheClustersRequest();
var response = client_Cache.DescribeCacheClusters(request);
Console.WriteLine("### Clusters ###");
foreach (var cluster in response.CacheClusters)
    Console.WriteLine("#{0} {1} (engine)", cluster.CacheClusterId, cluster.Engine);


AWS ElastiCache Memcached Engine:
---------------------------------

Clients:
EnyimMemcached *
BeITMemcached 

---

Nuget:
Install-Package AWSSDK
Install-Package Amazon.ElastiCacheCluster

note! Enyim is a dependency of ElastiCacheCluster package

---

add log:
LogManager.AssignFactory(new DiagnosticsLogFactory("[filePath]"));

---

implementation:

- option1
<configuration>
    <configSections>
        <section name="clusterclient" type="Amazon.ElastiCacheCluster.ClusterConfigSettings, Amazon.ElastiCacheCluster" />
    </configSections>

    <clusterclient>
        <endpoint hostname="[Endpoint]" port="11211" /> 	
    </clusterclient>
</configuration>

using Amazon.ElastiCacheCluster;
using Enyim.Caching;
using Enyim.Caching.Memcached;

var config = new ElastiCacheClusterConfig();
config.Protocol = MemcachedProtocol.Text;
using (var memClient = new MemcachedClient(config))
{
    var success = memClient.Store(StoreMode.Add, "key1", "value1");                    
    Console.WriteLine(memClient.Get<string>("key1"));
}

- option2

var config = new ElastiCacheClusterConfig([Endpoint], 11211);
config.Protocol = MemcachedProtocol.Text;
using (var memClient = new MemcachedClient(config))
{
    var success = memClient.Store(StoreMode.Add, "key1", "value1");
    Console.WriteLine(memClient.Get<string>("key1"));
}


AWS ElastiCache Redis Engine:
-----------------------------

note! we can use any redis client to connect to the AWS elasticache redis (engine) nodes

---

Clients:
StackExchange 
ServiceStack *

---

Nuget:

// ServiceStack
Install-Package AWSSDK
Install-Package ServiceStack.Redis

// StackExchange
Install-Package AWSSDK
Install-Package StackExchange.Redis
Install-Package Newtonsoft.Json

---

implementation:

// using StackExchange client

/*
	note! NOT supports in POCO objects - have to serialize/deserialize the objects manually 
	we can use xml, binary or json serializators (Newtonsoft.Json etc.)
	[free license]

	note! the version for framework 4.0 has a dependency on Microsoft.Bcl and Microsoft.Bcl.Async!
	the version for 4.5 has NO dependencies

	functions:
	----------
	* dataBase.StringSet(key, value, expiry) - add/update data as string (need to serialize it before)
				tip: use JsonConvert (Newtonsoft.Json namespace) 
	* dataBase.KeyDelete(key) - delete item
	* dataBase.StringGet(key) - get a single item
	* dataBase.StringGet(keys) - get multiple items

	* server.Keys() - get all keys
	* server.Keys(pattern) - get keys by pattern
	* dataBase.ListRightPush(key, value) - add item to the tail of the list
	* dataBase.ListRightPush(key, values) - add items to the tail of the list
	* dataBase.ListLeftPush(key, value) - add item to the head of the list	
	* dataBase.ListLeftPush(key, values) - add items to the head of the list
	* dataBase.ListRange(Key, start, end) - get values range from list
	* dataBase.ListRemove(Key, redisValue) - remove items from list 

	// use score in order to prevent the default chronological order and make the insertion time the valid order 
    // note! if two items inserted with the very same score - the secondary order will be the chronological value
	* dataBase.SortedSetAdd(key, value, score) - add/update item to sorted list (sorted by score)
	* dataBase.SortedSetAdd(Key, sortedSetEntries) - add/update items to sorted list (sorted by score)

	// SortedSetRangeByRank - get top x ordered by insertion index
    // SortedSetRangeByScore - get top x ordered by score values
	* dataBase.SortedSetRangeByRank(Key, start, end, Order.Descending) - get values range from sorted list

	transactions:
	-------------
	var trans = this.dataBase.CreateTransaction();

	// conditions
	// e.g: trans.AddCondition(Condition.KeyExists("KEY_A")); // KEY_A is mandatory - if not exists in the cache - transaction will fail
	trans.AddCondition([Condition]);
	trans.AddCondition([Condition]);
	...

	// actions
	trans.StringSetAsync([key], [value]);
	trans.StringSetAsync([key], [value]);
	...

	return trans.Execute();


	enable admin mode:
	------------------
    var config = ConfigurationOptions.Parse([Endpoint:port]);
    config.AllowAdmin = true;
	ConnectionMultiplexer.Connect(config)

	expiry issue:
	-------------
	- ExpiryDate with Unspecified kind is causing redis to crash!!! 	
	- How to fix:
	  change to Local or UTC time

	- e.g:
	  if (ExpiryDate.HasValue && ExpiryDate.Value.Kind == DateTimeKind.Unspecified)
		  ExpiryDate = new DateTime(ExpiryDate.Value.Ticks, DateTimeKind.Local);
	  var result = this.redisClient.Set<T>(key.KeyString.ToUpper(), value, ExpiryDate); // OK

*/
using ServiceStack.Redis;

using (var redisClient = new RedisClient([Endpoint], 6379))
{
    var success = redisClient.Set<string>("key1", "value1");
    Console.WriteLine(redisClient.Get<string>("key1"));
}

---

// using ServiceStack client
// https://github.com/StackExchange/StackExchange.Redis/tree/master/Docs

/*
	note! free quota is limited to '6000 Redis requests per hour 
*/

using StackExchange.Redis;

using (var redisClient = ConnectionMultiplexer.Connect([Endpoint:port]))
{
    redisClient.GetDatabase().StringSet("key1", "value1");
    Console.WriteLine(redisClient.GetDatabase().StringGet("key1"));
}

---

local server:
we can connect to any redis server we'd like
for local server just run the file 'redis-server.exe' which will launch a local server listening on port 6379
use the 127.0.0.1 as the server endpoint.

using (var redisClient = new RedisClient(127.0.0.1, 6379))
{
  ...
}