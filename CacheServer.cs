﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching; // cache

namespace Common
{
    /*  USING SAMPLE
        ------------
        public static List<PaymentStatus> GetPaymentStatuses()
        {
            var cacheKey = CacheKeys.PaymentStatuses;
            var result = CacheServer.Instance.Get<List<PaymentStatus>>(cacheKey);
            if (result == null)
            {
                var dr = DataBase.Deliveries.GetPaymentStatuses();
                result = DeliveriesLoader.LoadPaymentStatuses(dr);

                if(result != null)
                    CacheServer.Instance.Set(cacheKey, result, CacheServer.HighExpiry);
            }
            return result;
        } 
    */

    public class CacheKeys
    {
        public const string PaymentStatuses = "PaymentStatuses";
        public const string DeliveryStatuses = "DeliveryStatuses";
        public const string Manufacturers = "Manufacturers";
        public const string Cities = "Cities";

        public const string EmployeeTemplate = "Employee_{0}";
        public const string RestaurantTemplate = "Employee_{0}";
    }

    public class CacheServer {
        private const string _cacheName = "MemoryCacheServer";
        private MemoryCache _cache { set; get; }
        
        public static DateTime LowExpiry {
            get {
                return DateTime.Now.AddMinutes(5);
            }
        }

        public static DateTime MediumExpiry
        {
            get{
                return DateTime.Now.AddMinutes(30);
            }
        }

        public static DateTime HighExpiry
        {
            get{
                return DateTime.Now.AddMinutes(60);
            }
        }

        // singleTon
        private static CacheServer _Instance = null;
        public static CacheServer Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new CacheServer();  // lazy loading 
                return _Instance;
            }
        }
        private CacheServer() {
            this._cache = new MemoryCache(_cacheName);
        } // private constructor (prevent the new clause)

        public T Get<T>(string Key)
        {
            return (T)_cache.Get(Key);
        }

        public void Set<T>(string Key, T Item, DateTime Expiry)
        {
            _cache.Set(Key, Item, Expiry);
        }

        public void Remove(string Key)
        {
            _cache.Remove(Key);
        }

        public void Clear()
        {
            _cache.Dispose();
            _cache = new MemoryCache(_cacheName);
        }
    }
}
