using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using static Konimbo.KonimboEntities;
using Helpers;
using Newtonsoft.Json;
using System.Web;
using System.Diagnostics;

/*
 
    /// TODO ->> ToDocument

    -- TEMP --

    get api token: 
    Login into Admin > API > Select API Account > Copy "Token" value
    
    qa_user
    c99acd23ca81f25bdde68ecd45503afa4198d799aabba9368810ad50cefedac5

    note! 
    in order to access the api services, we must ask konimbo to give us permissions on their end

    ---

    Konimbo Hybrid
    https://konimbo.co.il/pages/32401
    https://konimbo.co.il/pages/25822-API

    Reference:
    https://github.com/konimbo/API-Documentation#user-content-%D7%94%D7%92%D7%A8%D7%A1%D7%90-%D7%94%D7%A0%D7%95%D7%9B%D7%97%D7%99%D7%AA

    login with facebook and google [extension]:
    https://konimbo-plugins.co.il/items/4685745-%D7%94%D7%A8%D7%A9%D7%9E%D7%94-%D7%95%D7%94%D7%AA%D7%97%D7%91%D7%A8%D7%95%D7%AA-%D7%9C%D7%90%D7%AA%D7%A8-%D7%93%D7%A8%D7%9A-%D7%A4%D7%99%D7%99%D7%A1%D7%91%D7%95%D7%A7-%D7%95%D7%92%D7%95%D7%92%D7%9C-%D7%91%D7%90%D7%AA%D7%A8

    ---

    Konimbo Dashboard

    items:
    (right menu) מוצרים בחנות > כל המוצרים
    can use top filter to change the result view (e.g: By "QA" category)
    click on the product name to see all of its properties such as details, inventory, tags, filters, spec and etc. 
    use the top tabs to switch between screens 

    ---

    (items)
    item reference:
    https://github.com/konimbo/API-Documentation/blob/master/v1/items.md

    filter parameters:
    - created_at_min    // date (e.g: created_at_min=2017-01-01T09:00:00Z)
    - created_at_max    // date
    - updated_at_min    // date
    - updated_at_max    // date
    - min_price         // float
    - max_price         // float
    - group_value_id    // int
    - visible           // bool
    - tag_id            // int
    - store_category_id // int

    sort parameters:
    - sort_by           // field name (e.g: sort_by=price, sort_by=updated_at..)
    - sort_order        // desc or asc

    date-format:
    - format: ISO-8601
    - schema: YYYY-MM-DDTHH:MM:SSZ
    - e.g: 2017-01-01T09:00:00Z

    item fields:
    see 'REFERENCE' > items.md
    https://github.com/konimbo/API-Documentation/blob/master/v1/items.md#user-content-%D7%A4%D7%99%D7%A8%D7%95%D7%98-%D7%94%D7%A9%D7%93%D7%95%D7%AA

    paging:
    used as http headers
    - X-Pagination-Per-Page	// num of results per page (default: 20)
    - X-Pagination-Total	// @rowcount
    - X-Pagination-Links	// paging links

    limitations:
    total of 100 requests within 10 minutes range per token 
    related headers
    - X-Rate-Limit-Current	
    - X-Rate-Limit-Maximum	
    - X-Rate-Limit-Reset	




    --------------------------------------------------------------------------------    


    REFERENCE
    ---------
    https://github.com/konimbo/API-Documentation/tree/master/v1
    https://github.com/konimbo/API-Documentation/blob/master/v1/items.md
    https://github.com/konimbo/API-Documentation/blob/master/v1/orders.md
    https://github.com/konimbo/API-Documentation/blob/master/v1/integration.md
    https://github.com/konimbo/API-Documentation/blob/master/v1/customers.md
    https://github.com/konimbo/API-Documentation/blob/master/v1/carts.md
    https://github.com/konimbo/API-Documentation/blob/master/v1/amazons.md


    SUPPORT
    -------
    https://konimbo.co.il/

    SANDBOX USERS
    -------------
    

    DEMO (SOURCES)
    --------------
    

    PROCESS
    -------
    (steps)    
    

    API BASE URL
    ------------
    * SANDBOX 
      
            
    * PROD
      

    SERVICE TYPES
    -------------
    *     


    POSTMAN
    -------
    - see 'xxxxxx.json'
    - see 'xxxxxx.json' 


    IMPLEMENTATIONS
    ---------------
    - see 'CODE > xxx.cs'    

    
    RESEARCH
    --------
    - see 'Scripts > Konimbo'


    cURL
    ----        


    USING
    -----
    var konimboManager = new KonimboManager(new KonimboConfig { 
        ServerURL = "https://api.konimbo.co.il/v1",
        Token = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
    });

    var categories = await konimboManager.GetCategories();
    Console.WriteLine($"{categories.Count()} categories:");
    foreach (var category in categories) Console.WriteLine($"#{category.id}. {category.title_he}");

    -

    // get all items (no filters)
    var items = await konimboManager.GetItems();
    Console.WriteLine($"{items.Count()} items:");
    foreach (var item in items) Console.WriteLine($"#{item.id}. {item.title}");

    -

    // search items by custom filter (e.g: price-range and category)
    var items = await konimboManager.SearchItems(new SearchItemsRequest { 
        min_price = 80,
        max_price = 100,
        store_category_id = 430507  // QA                
    });
    Console.WriteLine($"{items.Count()} items:");
    foreach (var item in items) Console.WriteLine($"#{item.id}. {item.title}");

    -

    // search items by custom filter (e.g: tag)
    var items = await konimboManager.SearchItems(new SearchItemsRequest {                
        tag_id = 179585  // tagA
    });
    Console.WriteLine($"{items.Count()} items:");
    foreach (var item in items) Console.WriteLine($"#{item.id}. {item.title}");

    -

    // search items by custom filter (e.g: updated-date)
    var items = await konimboManager.SearchItems(new SearchItemsRequest
    {
        updated_at_min = DateTime.Now.AddDays(-1)
    });
    Console.WriteLine($"{items.Count()} items:");
    foreach (var item in items) Console.WriteLine($"#{item.id}. {item.title} (update at {item.updated_at})");

    -

    // search items by custom filter (e.g: created-date-range - all items created this year)
    var items = await konimboManager.SearchItems(new SearchItemsRequest
    {
        created_at_min = new DateTime(DateTime.Now.Year, 1, 1),
        created_at_max = DateTime.Now,
        sort_by = "created_at"
    });
    Console.WriteLine($"{items.Count()} items:");
    foreach (var item in items) Console.WriteLine($"#{item.id}. {item.title} (created at {item.created_at})");

    -

    // sort items by custom field (e.g: updated_at desc)
    var items = await konimboManager.SearchItems(new SearchItemsRequest
    {
        sort_by = "updated_at",
        sort_order = "desc"
    });
    Console.WriteLine($"{items.Count()} items:");
    foreach (var item in items) Console.WriteLine($"#{item.id}. {item.title} (update at {item.updated_at})");

    -

    // get all items as specific attributes (no filters)
    var items = await konimboManager.GetItemsAsAttributes(new List<string> { "id", "price", "code" });
    Console.WriteLine($"{items.Count()} items:");
    foreach (var item in items) Console.WriteLine($"{item}");

    -

    // get all items as specific attributes by custom filter (e.g: price-range and category)
    var items = await konimboManager.SearchItemsAsAttributes(new SearchItemsAsAttributesRequest { 
        Attributes = new List<string> { "id", "price", "code" },
        min_price = 80,
        max_price = 100,
        store_category_id = 430507  // QA
    });
    Console.WriteLine($"{items.Count()} items:");
    foreach (var item in items) Console.WriteLine($"{item}");

    -

    // get all items as specific attributes by custom filter (e.g: tag)
    var items = await konimboManager.SearchItemsAsAttributes(new SearchItemsAsAttributesRequest {
        Attributes = new List<string> { "id", "price", "code" },
        tag_id = 179585  // tagA
    });
    Console.WriteLine($"{items.Count()} items:");
    foreach (var item in items) Console.WriteLine($"{item}");

    -

    // get specific item (no filters)
    var item = await konimboManager.GetItem("6621200");
    Console.WriteLine($"#{item.id}. {item.title}");

    -

    // get specific item as specific attributes (no filters)
    var item = await konimboManager.GetItemAsAttributes(new List<string> { "id", "price", "code" }, "6621200");            
    Console.WriteLine($"{item}");

    -
    
    var item = await konimboManager.GetItemByCode("7290017940304");
    Console.WriteLine($"{item}");

    -

    // create item
    var item = await konimboManager.CreateItem(new Item {
        code = "#1007",
        title = "Product 1007",
        store_category_title = "QA",
        price = 102,
        origin_price = 102,
        quantity = 100
    });
    Console.WriteLine($"#{item.id}. {item.title} (created at {item.created_at})");

    -

    // delete item
    var deleted = await konimboManager.DeleteItem("6621080");
    Console.WriteLine($"{deleted}"); 

    -

    // update item details
    var itemDetails = new ItemDetails();
    itemDetails.title = mainFields?.Short_Description;
    itemDetails.brand = mainFields?.BrandName;
    itemDetails.desc = generalInfo?.Additional_Trade_Item_Description_1;
    var item = await konimboManager.UpdateItemDetails("6621200", itemDetails);
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");    

    -

    // update item images
    var item = await konimboManager.UpdateItemImages("6621200", new ItemImagesWrapper(new List<ItemImage>{
        new ItemImage {
            url = "https://www.gstatic.com/webp/gallery3/1.sm.png",
            alt = "some image",
            position = 1
        },
        new ItemImage{
            url = "https://www.gstatic.com/webp/gallery3/2.sm.png",
            alt = "other image",
            position = 2
        }
    }));
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");

    -

    // clear item images
    var item = await konimboManager.ClearItemImages("6621200");
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");

    -

    // update item spec
    var item = await konimboManager.UpdateItemSpec("6621200", new ItemSpecWrapper(new List<ItemSpec>{
        new ItemSpec {
            group = "Physical",
            name = "Size",
            value = "20 x 25 cm",
            position = 1
        },
        new ItemSpec{
            group = "Physical",
            name = "Weight",
            value = "1.2 kg",
            position = 2
        },
        new ItemSpec{
            group = "Physical",
            name = "Height",
            value = "40 cm",
            position = 3
        }
    }));
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");
            
    -

    // clear item spec
    var item = await konimboManager.ClearItemSpec("6621200");
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");

    -

    // create item inventory
    var item = await konimboManager.CreateItemInventory("6621200", new CreateItemInventoryWrapper(new List<CreateItemInventory>{
        new CreateItemInventory {
            upgrade_topic_title = "Type",
            upgrade_title = "A",
            code = "TYPE#A",
            price = 100,
            free = 50
        },
        new CreateItemInventory{
            upgrade_topic_title = "Type",
            upgrade_title = "B",
            code = "TYPE#B",
            price = 59.90F,
            free = 120
        }
    }));
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");

    -

    // update item inventory
    var item = await konimboManager.UpdateItemInventory("6621200", new ItemInventoryWrapper(new List<CreateItemInventory>{
        new CreateItemInventory { 
            id = "1075322",
            code = "TYPE#C",
            price = 90,
            free = 40
        },
        new CreateItemInventory{
            id = "1075323",
            code = "TYPE#D",
            price = 79.90F,
            free = 110
        }
    }));
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");

    -

    // create item tags
    var item = await konimboManager.CreateItemTags("6621200", new ItemTagsWrapper(new List<ItemTag>{
            new ItemTag { title = "testTag1" },
            new ItemTag { title = "testTag2" },
            new ItemTag { title = "testTag3" }
        }));
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");

    -
    
    // update (add or remove) item tags
    var item = await konimboManager.UpdateItemTags("6621200", new ItemTagsWrapper(new List<ItemTag>{
        new ItemTagToAdd { title = "testTag6" },    // add 'testTag6' tag
        new ItemTagToAdd { title = "testTag7" },    // add 'testTag7' tag
        new ItemTagToRemove { title = "testTag3" }  // remove 'testTag3' tag
    }));
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");

    -

    // clear item tags
    var item = await konimboManager.ClearItemTags("6621200");
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");

    -

    // update item filters
    var item = await konimboManager.UpdateItemFilters("6621200", new ItemFiltersWrapper(new List<ItemFilter>{
        new ItemFilter { 
            group_title = "Group B",
            group_value_title = "Item B1",                    
            group_position = 1,
            group_value_position = 1
        },
        new ItemFilter {
            group_title = "Group B",
            group_value_title = "Item B2",
            group_position = 1,
            group_value_position = 2
        }
    }));
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");
    
    -

    // delete item filters
    var item = await konimboManager.DeleteItemFilters("6621200", new ItemFiltersWrapper(new List<ItemFilter>{
        new ItemFilter { 
            group_title = "Group B",
            group_value_title = "Item B1",                                        
        },
        new ItemFilter {
            group_title = "Group B",
            group_value_title = "Item B2",                    
        }
    }));
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");

    -

    // set item related items
    var item = await konimboManager.UpdateItemRelatedItems("6621200", new ItemRelatedItemsWrapper(new List<ItemRelatedItem>{
        new ItemRelatedItem {
            friend_item_id = "6606341",
            position = 1
        },
        new ItemRelatedItem {
            friend_item_id = "6606341",
            position = 2
        },
        new ItemRelatedItem {
            friend_item_id = "6606457",
            position = 3
        }
    }));
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");
            
    -

    // clear item related items
    var item = await konimboManager.ClearItemRelatedItems("6621200");
    Console.WriteLine($"#{item.id}. {item.title} (updated at {item.updated_at})");

    -
    
    var hookName = HOOK_EVENTS.OrderAfterPayment;
    var success = await konimboManager.HookRegisteration(new HookDetails("https://rcbuilder.free.beeceptor.com", hookName));                
    Console.WriteLine($"[{success}] {hookName}");

*/

namespace Konimbo
{
    public class KonimboEntities
    {
        public class KonimboConfig
        {
            public string ServerURL { get; set; }
            public string Token { get; set; }
        }

        public class APIException : Exception
        {
            public APIErrorResponse ErrorResponse { get; protected set; }
            public APIException(APIErrorResponse ErrorResponse) : base(ErrorResponse.Message)
            {
                this.ErrorResponse = ErrorResponse;
            }

            public override string ToString()
            {
                return $"{this.ErrorResponse}";
            }
        }

        public class APIErrorResponse
        {
            public string Message { get; set; }
            public (string Error, string Details) InnerMessage { get; set; }

            public override string ToString()
            {
                return $"{this.Message} | {this.InnerMessage.Error} | {this.InnerMessage.Details}";
            }
        }

        public class APIResponse<T>
        {
            public string Status { get; set; }
            public T Message { get; set; }
        }

        public class ItemImageComparer : IEqualityComparer<ItemImage>
        {
            public bool Equals(ItemImage x, ItemImage y)
            {
                return x.id == y.id && x?.url.ToLower() == y.url?.ToLower();
            }

            public int GetHashCode(ItemImage obj)
            {
                return obj == null ? 0 : (obj.id ?? 0).GetHashCode() ^ (obj.url?.ToLower() ?? "").GetHashCode();
            }
        }

        public class ItemTagComparer : IEqualityComparer<ItemTag>
        {
            public bool Equals(ItemTag x, ItemTag y)
            {
                return x.id == y.id && x.title?.ToLower() == y.title?.ToLower();
            }

            public int GetHashCode(ItemTag obj)
            {
                return obj == null ? 0 : (obj.id ?? 0).GetHashCode() ^ (obj.title?.ToLower() ?? "").GetHashCode();
            }
        }
        
        public class HOOK_EVENTS {
            public const string TicketCreated = "ticket_created";
            public const string OrderCreated = "order_created";
            public const string OrderAfterPayment = "order_after_payment";
            public const string CustomerCreated = "customer_created";
            public const string CustomerLoggedIn = "customer_logged_in";
            public const string OrderCreatedFuture = "order_created_future";
            public const string OrderStatusCreated = "order_status_created";
        }
        
        public class Category
        {
            public int id { get; set; }
            public string title_en { get; set; }
            public string title_he { get; set; }
            public int position { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }            
        }

        public class CategoryExtended : Category {
            public string seo_description { get; set; }
            public string seo_keywords { get; set; }
            public string seo_title { get; set; }
            public string image_url { get; set; }
            public string css_class { get; set; }
            public string desc { get; set; }
            public string parent_id { get; set; }
            public string slug { get; set; }
            public string group_title { get; set; }
            public string image_id { get; set; }
            public string nav_content { get; set; }
            public string tag_id { get; set; }
        }


        /// TODO ->> Complete + Fields Names + Extended        
        public class ItemDetails {
            public string id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public string store_category_id { get; set; }
            public string title { get; set; }
            public string store_category_title { get; set; }
            public float? price { get; set; }
            public float? origin_price { get; set; }
            public string code { get; set; }
            public string second_code { get; set; }
            public string desc { get; set; }
            public string visible { get; set; }
            public string model_title { get; set; }
            public string brand { get; set; }
            public float? eilat_price { get; set; }
            public string warranty { get; set; }
            public string position { get; set; }
            public int? quantity { get; set; }
            public string color_group { get; set; }
            public string note { get; set; }
            public bool? zap_visable { get; set; }
            public bool? store_visable { get; set; }
            public int? delivery_time { get; set; }
            public bool? default_shipping { get; set; }
            public string matching_models { get; set; }
            public string spec_text { get; set; }
            public string paypal_payment { get; set; }
            public string mirror_price { get; set; }
            public string zap_product_type { get; set; }
            public bool? personal_sell { get; set; }
            public string destroy_at { get; set; }
            public string kit { get; set; }
            public float? store_layout_title { get; set; }
            public string features { get; set; }
            public string save_price { get; set; }
            public string deal_items_limit { get; set; }
            public string seo_description { get; set; }
            public string offer_code { get; set; }
            public string personal_sell_min_price { get; set; }
            public string mirror_title { get; set; }
            public string content { get; set; }
            public bool? only_contact { get; set; }
            public string foot_html { get; set; }
            public string head_html { get; set; }
            public int? free_payment { get; set; }
            public string seo_title { get; set; }
            public string coupon_group_name { get; set; }
            public string prev_url { get; set; }
            public float? related_item_price { get; set; }
            public int? max_payment { get; set; }
            public float? quantity_step { get; set; }
            public float? cost { get; set; }
            public string slug { get; set; }
            public string video { get; set; }
            public string css_class { get; set; }
            public float? restore_price { get; set; }
            public string visit_link { get; set; }
            public string discount_group_name { get; set; }
            public string experiment_group { get; set; }
            public string promotions { get; set; }
            public string seo_keywords { get; set; }
            public string personal_sell_win_price { get; set; }
        }

        public class Item : ItemDetails
        {            
            public ItemStoreCategoryTitleWithParent store_category_title_with_parent { get; set; }            
            public IEnumerable<string> shipping_option_ids { get; set; }                                 
            public List<ItemRelatedItem> related_items { get; set; }
            public List<ItemInventory> inventory { get; set; }
            public List<ItemImage> images { get; set; }
            public List<ItemSpec> spec { get; set; }
            public List<ItemFilter> filters { get; set; }
            public List<ItemTag> related_tags { get; set; }
            public List<ItemSecondaryCategoryTitle> secondary_category_titles { get; set; }
            public ItemIconsSet set_icons { get; set; }
            public ItemZapBot zap_item { get; set; }

            public ItemDetails ToItemDetails() {                
                return JsonConvert.DeserializeObject<ItemDetails>(JsonConvert.SerializeObject(this));
            }
        }

        public class ItemExtended : Item { }
        
        public class ItemZapBot {
            public string url { get; set; }
            public string price_limit { get; set; }
        }
        public class ItemIconsSet {
            public string image_title { get; set; }
            public string title { get; set; }
            public string url { get; set; }

            [JsonProperty(PropertyName = "visible")]
            public string isVisible { get; set; }

            [JsonProperty(PropertyName = "index-visible")]
            public string isVisibleOnCategoryPage { get; set; }        
        }

        public class ItemInventory
        {
            public string id { get; set; }
            public string title { get; set; }                  
            public string code { get; set; }
            public float? price { get; set; }
            public float? free { get; set; }
        }

        public class CreateItemInventory : ItemInventory {
            public string upgrade_topic_title { get; set; }
            public string upgrade_title { get; set; }
        }

        public class ItemInventoryWrapper
        {
            [JsonProperty(PropertyName = "inventory")]
            public IEnumerable<ItemInventory> inventory { get; set; }

            public ItemInventoryWrapper(IEnumerable<ItemInventory> inventory) {
                this.inventory = inventory;
            }
        }

        public class CreateItemInventoryWrapper
        {
            [JsonProperty(PropertyName = "inventory")]
            public IEnumerable<CreateItemInventory> inventory { get; set; }

            public CreateItemInventoryWrapper(IEnumerable<CreateItemInventory> inventory)
            {
                this.inventory = inventory;
            }
        }

        public class ItemFilter
        {
            public int? id { get; set; }
            public string group_title { get; set; }
            public string group_value_title { get; set; }
            public int? group_position { get; set; }
            public int? group_value_position { get; set; }            
        }

        public class ItemFilterToRemove : ItemFilter
        {
            public string remove_from_item { get; set; }

            public ItemFilterToRemove() : this(new ItemFilter()) { }
            public ItemFilterToRemove(ItemFilter itemFilter)
            {
                this.group_title = itemFilter.group_title;
                this.group_value_title = itemFilter.group_value_title;
                this.remove_from_item = "true";
            }
        }

        public class ItemFiltersWrapper
        {
            [JsonProperty(PropertyName = "filters")]
            public IEnumerable<ItemFilter> filters { get; set; }

            public ItemFiltersWrapper(IEnumerable<ItemFilter> filters) {
                this.filters = filters;
            }
        }

        public class ItemImage
        {
            public int? id { get; set; }
            public string url { get; set; }
            public int? position { get; set; }
            public string alt { get; set; }            
        }

        public class ClearItemImage : ItemImage
        {
            public string delete { get; set; }
        }

        public class ItemImagesWrapper
        {
            [JsonProperty(PropertyName = "images")]
            public IEnumerable<ItemImage> images { get; set; }

            public ItemImagesWrapper(IEnumerable<ItemImage> images) {
                this.images = images.Distinct(new ItemImageComparer()); // x => x.id && x.url
            }
        }

        public class ItemRelatedItem
        {
            public string friend_item_id { get; set; }
            public int? position { get; set; }            
        }

        public class ClearItemRelatedItem : ItemRelatedItem
        {
            public string delete { get; set; }
        }

        public class ItemRelatedItemsWrapper
        {
            [JsonProperty(PropertyName = "related_items")]
            public IEnumerable<ItemRelatedItem> relatedItems { get; set; }

            public ItemRelatedItemsWrapper(IEnumerable<ItemRelatedItem> relatedItems) {
                this.relatedItems = relatedItems;
            }
        }

        public class ItemTag
        {
            public int? id { get; set; }
            public string title { get; set; }
            public string group_name { get; set; }                             
        }

        public class ItemTagToAdd : ItemTag
        {
            public string add_to_item { get; set; }

            public ItemTagToAdd() {                                
                this.add_to_item = "true";
            }
        }

        public class ItemTagToRemove : ItemTag {
            public string remove_from_item { get; set; }

            public ItemTagToRemove()
            {                                
                this.remove_from_item = "true";
            }
        }

        public class ItemTagsWrapper
        {
            [JsonProperty(PropertyName = "related_tags")]
            public IEnumerable<ItemTag> tags { get; set; }

            public ItemTagsWrapper(IEnumerable<ItemTag> tags) {
                this.tags = tags.Distinct(new ItemTagComparer());
                this.tags = this.tags.Select(x =>
                {
                    x.title = x.title?.Replace("-", string.Empty); // remove special characters
                    return x;
                });
            }
        }

        public class ItemSecondaryCategoryTitle
        {
            public int id { get; set; }
            public string title { get; set; }
            public object group_name { get; set; }            
        }

        public class ItemSpec
        {
            public int? id { get; set; }
            public string group { get; set; }
            public string name { get; set; }
            public string value { get; set; }
            public int? position { get; set; }            
        }

        public class ClearItemSpec : ItemSpec
        {
            public string delete { get; set; }
        }

        public class ItemSpecWrapper
        {
            [JsonProperty(PropertyName = "spec")]
            public IEnumerable<ItemSpec> spec { get; set; }

            public ItemSpecWrapper(IEnumerable<ItemSpec> spec) {
                this.spec = spec;
            }
        }

        public class ItemStoreCategoryTitleWithParent
        {
            public string child_title { get; set; }
            public string parent_title { get; set; }
        }

        public class SearchItemsRequest
        {
            public DateTime? created_at_min { get; set; }    // date (e.g: created_at_min=2017-01-01T09:00:00Z)
            public DateTime? created_at_max { get; set; }
            public DateTime? updated_at_min { get; set; }
            public DateTime? updated_at_max { get; set; }
            public float? min_price { get; set; }
            public float? max_price { get; set; }
            public int? group_value_id { get; set; }
            public bool? visible { get; set; }
            public int? tag_id { get; set; }
            public int? store_category_id { get; set; }
            public string sort_by { get; set; }     // field name (e.g: sort_by=price, sort_by=updated_at..)
            public string sort_order { get; set; }  // desc or asc
        }

        public class ItemAsAttributes {
            public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

            public string Find(string Key) {
                return this.Find<string>(Key);
            }
            public T Find<T>(string Key)
            {
                if (!this.Data.ContainsKey(Key)) return default;
                return (T)Convert.ChangeType(this.Data[Key], typeof(T));
                ///return (T)this.Data[Key];
            }

            public IEnumerable<string> Attributes {
                get {
                    return this.Data?.Keys;
                }
            }

            public string sAttributes {
                get {
                    return string.Join(",", this.Attributes);
                }
            }

            public ItemAsAttributes(Dictionary<string, string> Data) {
                this.Data = Data;
            }

            public override string ToString()
            {
                return string.Join(",", this.Data?.Select(x => $"{x.Key}={x.Value}"));
            }
        }

        public abstract class TokenBaseRequest {
            public string token { get; protected set; }            

            public TokenBaseRequest(string token) {
                this.token = token;                
            }
        }

        public class CreateItemRequest : TokenBaseRequest
        {
            [JsonProperty(PropertyName = "item")]
            public Item item { get; protected set; }

            public CreateItemRequest(string token, Item item) : base(token) {
                this.item = item;
            }
        }

        public class UpdateItemRequest : TokenBaseRequest
        {
            [JsonProperty(PropertyName = "item")]
            public ItemDetails itemDetails { get; protected set; }

            public UpdateItemRequest(string token, ItemDetails itemDetails) : base(token)
            {
                this.itemDetails = itemDetails;
            }
        }

        public class UpdateItemImagesRequest : TokenBaseRequest
        {
            [JsonProperty(PropertyName = "item")]
            public ItemImagesWrapper itemImages { get; protected set; }

            public UpdateItemImagesRequest(string token, ItemImagesWrapper itemImages) : base(token) {
                this.itemImages = itemImages;
            }
        }

        public class ClearItemImagesRequest : UpdateItemImagesRequest {            
            public ClearItemImagesRequest(string token) : base(token, new ItemImagesWrapper(
                new List<ItemImage> { 
                    new ClearItemImage {                        
                        delete = "true"
                    } 
                })
            ) { }
        }

        public class UpdateItemSpecRequest : TokenBaseRequest
        {
            [JsonProperty(PropertyName = "item")]
            public ItemSpecWrapper itemSpecWrapper { get; protected set; }

            public UpdateItemSpecRequest(string token, ItemSpecWrapper itemSpecWrapper) : base(token) {
                this.itemSpecWrapper = itemSpecWrapper;
            }
        }

        public class ClearItemSpecRequest : UpdateItemSpecRequest
        {            
            public ClearItemSpecRequest(string token) : base(token, new ItemSpecWrapper(
                new List<ItemSpec> {
                    new ClearItemSpec {
                        delete = "true"
                    }
                })
            ) {}
        }

        public class CreateItemInventoryRequest : TokenBaseRequest
        {
            [JsonProperty(PropertyName = "item")]
            public CreateItemInventoryWrapper itemInventoryWrapper { get; protected set; }

            public CreateItemInventoryRequest(string token, CreateItemInventoryWrapper itemInventoryWrapper) : base(token) {
                this.itemInventoryWrapper = itemInventoryWrapper;
            }
        }

        public class UpdateItemInventoryRequest : TokenBaseRequest
        {
            [JsonProperty(PropertyName = "item")]
            public ItemInventoryWrapper itemInventoryWrapper { get; protected set; }

            public UpdateItemInventoryRequest(string token, ItemInventoryWrapper itemInventoryWrapper) : base(token)
            {
                this.itemInventoryWrapper = itemInventoryWrapper;
            }
        }

        public class CreateItemTagsRequest : TokenBaseRequest
        {
            [JsonProperty(PropertyName = "item")]
            public ItemTagsWrapper itemTagsWrapper { get; protected set; }

            public CreateItemTagsRequest(string token, ItemTagsWrapper itemTagsWrapper) : base(token) {
                this.itemTagsWrapper = itemTagsWrapper;
            }
        }

        public class UpdateItemTagsRequest : CreateItemTagsRequest
        { 
            public UpdateItemTagsRequest(string token, ItemTagsWrapper itemTagsWrapper) : base(token, itemTagsWrapper)                
            {
                base.itemTagsWrapper.tags = base.itemTagsWrapper.tags.Select(
                    x => {
                        x.group_name = null;
                        x.id = null;                        
                        return x;
                    });
            }
        }

        public class ClearItemTagsRequest : CreateItemTagsRequest
        {            
            public ClearItemTagsRequest(string token) : base(token, new ItemTagsWrapper(
                new List<ItemTag> {
                    new ItemTagToRemove()
                })
            ) {}
        }

        public class UpdateItemFiltersRequest : TokenBaseRequest
        {
            [JsonProperty(PropertyName = "item")]
            public ItemFiltersWrapper itemFiltersWrapper { get; protected set; }

            public UpdateItemFiltersRequest(string token, ItemFiltersWrapper itemFiltersWrapper) : base(token)
            {
                this.itemFiltersWrapper = itemFiltersWrapper;
            }
        }

        public class DeleteItemFiltersRequest : UpdateItemFiltersRequest
        { 
            public DeleteItemFiltersRequest(string token, ItemFiltersWrapper itemFiltersWrapper) : base(token, itemFiltersWrapper)                
            {
                base.itemFiltersWrapper.filters = base.itemFiltersWrapper.filters.Select(x => new ItemFilterToRemove(x));
            }
        }

        public class UpdateItemRelatedItemsRequest : TokenBaseRequest
        {
            [JsonProperty(PropertyName = "item")]
            public ItemRelatedItemsWrapper itemRelatedItemsWrapper { get; protected set; }

            public UpdateItemRelatedItemsRequest(string token, ItemRelatedItemsWrapper itemRelatedItemsWrapper) : base(token)
            {
                this.itemRelatedItemsWrapper = itemRelatedItemsWrapper;
            }
        }

        public class ClearItemRelatedItemsRequest : UpdateItemRelatedItemsRequest
        {
            public ClearItemRelatedItemsRequest(string token) : base(token, new ItemRelatedItemsWrapper(
                new List<ItemRelatedItem> {
                    new ClearItemRelatedItem {
                        delete = "true"
                    }
                })
            )
            { }
        }

        public class HookDetails
        {
            [JsonProperty(PropertyName = "callback_url")]
            public string CallbackURL { get; set; }

            [JsonProperty(PropertyName = "event")]
            public string EventName { get; set; }

            public HookDetails(string CallbackURL, string EventName) {
                this.CallbackURL = CallbackURL;
                this.EventName = EventName;
            }
        }

        public class HookRegisterationRequest : TokenBaseRequest
        {
            [JsonProperty(PropertyName = "webhook")]
            public HookDetails hookDetails { get; protected set; }

            public HookRegisterationRequest(string token, HookDetails hookDetails) : base(token)
            {
                this.hookDetails = hookDetails;
            }
        }        
    }

    public interface IKonimboManager
    {
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<Item>> GetItems();        
        Task<IEnumerable<Item>> SearchItems(SearchItemsRequest Request);
        Task<IEnumerable<ItemAsAttributes>> GetItemsAsAttributes(IEnumerable<string> Attributes);
        Task<IEnumerable<ItemAsAttributes>> SearchItemsAsAttributes(IEnumerable<string> Attributes, SearchItemsRequest Request);
        Task<Item> GetItem(string ItemId);
        Task<Item> GetItemByCode(string Code, bool ThrowNotFoundException = false);
        Task<ItemAsAttributes> GetItemAsAttributes(IEnumerable<string> Attributes, string ItemId);
        Task<Item> CreateItem(Item Item);
        Task<Item> UpdateItem(string ItemId, Item Item);
        Task<bool> DeleteItem(string ItemId);
        Task<Item> UpdateItemDetails(string ItemId, ItemDetails ItemDetails);
        Task<Item> UpdateItemImages(string ItemId, ItemImagesWrapper ItemImagesWrapper);
        Task<Item> ClearItemImages(string ItemId);
        Task<Item> UpdateItemSpec(string ItemId, ItemSpecWrapper ItemSpecWrapper);
        Task<Item> ClearItemSpec(string ItemId);
        Task<Item> CreateItemInventory(string ItemId, CreateItemInventoryWrapper ItemInventoryWrapper);
        Task<Item> UpdateItemInventory(string ItemId, ItemInventoryWrapper ItemInventoryWrapper);
        Task<Item> CreateItemTags(string ItemId, ItemTagsWrapper ItemTagsWrapper);
        Task<Item> UpdateItemTags(string ItemId, ItemTagsWrapper ItemTagsWrapper);
        Task<Item> ClearItemTags(string ItemId);
        Task<Item> UpdateItemFilters(string ItemId, ItemFiltersWrapper ItemFiltersWrapper);
        Task<Item> DeleteItemFilters(string ItemId, ItemFiltersWrapper ItemFiltersWrapper);
        Task<Item> UpdateItemRelatedItems(string ItemId, ItemRelatedItemsWrapper ItemRelatedItemsWrapper);
        Task<Item> ClearItemRelatedItems(string ItemId);        
        Task<bool> HookRegisteration(HookDetails HookDetails);
        Task<IEnumerable<HookDetails>> GetRegisteredHooks();
    }

    public class KonimboManager : IKonimboManager
    {        
        protected KonimboConfig Config { get; set; }
        protected HttpServiceHelper HttpService { get; set; }

        public KonimboManager(KonimboConfig Config)
        {
            this.Config = Config;
            this.HttpService = new HttpServiceHelper();
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            var response = await this.HttpService.GET_ASYNC<IEnumerable<Category>>(
                $"{this.Config.ServerURL}/store_categories",                
                $"token={this.Config.Token}",
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<IEnumerable<Item>> GetItems()
        {
            var response = await this.HttpService.GET_ASYNC<IEnumerable<Item>>(
                $"{this.Config.ServerURL}/items",
                $"token={this.Config.Token}",
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<IEnumerable<Item>> SearchItems(SearchItemsRequest Request)
        {
            var querySearchParams = this.BuildSearchFiltersQuery(Request);            
            if (querySearchParams != string.Empty) querySearchParams = $"&{querySearchParams}";

            var response = await this.HttpService.GET_ASYNC<IEnumerable<Item>>(
                $"{this.Config.ServerURL}/items",
                $"token={this.Config.Token}{querySearchParams}",
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<IEnumerable<ItemAsAttributes>> GetItemsAsAttributes(IEnumerable<string> Attributes)
        {
            var queryParams = $"&attributes={string.Join(",", Attributes)}";

            var response = await this.HttpService.GET_ASYNC<IEnumerable<Dictionary<string, string>>>(
                $"{this.Config.ServerURL}/items",
                $"token={this.Config.Token}{queryParams}",
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model.Select(x => new ItemAsAttributes(x));
        }

        public async Task<IEnumerable<ItemAsAttributes>> SearchItemsAsAttributes(IEnumerable<string> Attributes, SearchItemsRequest Request)
        {
            var querySearchParams = this.BuildSearchFiltersQuery(Request);
            if (querySearchParams != string.Empty) querySearchParams = $"&{querySearchParams}";

            var queryParams = $"&attributes={string.Join(",", Attributes)}";            
            queryParams = $"{queryParams}{querySearchParams}";

            var response = await this.HttpService.GET_ASYNC<IEnumerable<Dictionary<string, string>>>(
                $"{this.Config.ServerURL}/items",
                $"token={this.Config.Token}{queryParams}",
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model.Select(x => new ItemAsAttributes(x));
        }

        public async Task<Item> GetItem(string ItemId)
        {
            var response = await this.HttpService.GET_ASYNC<Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                $"token={this.Config.Token}",
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );
            
            if (!response.Success) {
                // StatusCode: NotFound (404) indicates on NoResult - skip error
                if (response.StatusCode == HttpStatusCode.NotFound)
                    response.Success = true;                                    
                else throw new APIException(this.ParseError(response.Content));                
            }

            return response.Model;
        }

        public async Task<Item> GetItemByCode(string Code, bool ThrowNotFoundException = false)
        {
            var items = await this.GetItemsAsAttributes(new List<string> { "id", "code" });
            var found = items.FirstOrDefault(x => string.Equals(x.Data["code"], Code, StringComparison.OrdinalIgnoreCase));
            if (found == null)
            {
                if (ThrowNotFoundException)
                    throw new APIException(new APIErrorResponse
                    {
                        Message = $"Code '{Code}' could not be found!"
                    });
                else return null;
            }

            var itemId = found.Data["id"]?.Trim();
            return await this.GetItem(itemId);
        }

        public async Task<ItemAsAttributes> GetItemAsAttributes(IEnumerable<string> Attributes, string ItemId)
        {
            var queryParams = $"&attributes={string.Join(",", Attributes)}";

            var response = await this.HttpService.GET_ASYNC<Dictionary<string, string>>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                $"token={this.Config.Token}{queryParams}",
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return new ItemAsAttributes(response.Model);
        }

        public async Task<Item> CreateItem(Item Item)
        {
            var response = await this.HttpService.POST_ASYNC<CreateItemRequest, Item>(
                $"{this.Config.ServerURL}/items",
                new CreateItemRequest(this.Config.Token, Item),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        [Obsolete("Use UpdateItemXXX to update each item section")]
        public async Task<Item> UpdateItem(string ItemId, Item Item)
        {
            await this.UpdateItemDetails(ItemId, Item as ItemDetails);
            await this.UpdateItemImages(Item.id, new ItemImagesWrapper(Item.images));
            await this.UpdateItemSpec(Item.id, new ItemSpecWrapper(Item.spec));
            await this.UpdateItemInventory(Item.id, new ItemInventoryWrapper(Item.inventory));
            await this.UpdateItemTags(Item.id, new ItemTagsWrapper(Item.related_tags));
            await this.UpdateItemFilters(Item.id, new ItemFiltersWrapper(Item.filters));
            await this.UpdateItemRelatedItems(Item.id, new ItemRelatedItemsWrapper(Item.related_items));
            return await this.GetItem(Item.id);
        }

        public async Task<Item> UpdateItemDetails(string ItemId, ItemDetails ItemDetails)
        {
            var response = await this.HttpService.PUT_ASYNC<UpdateItemRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new UpdateItemRequest(this.Config.Token, ItemDetails),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<bool> DeleteItem(string ItemId)
        {
            var response = await this.HttpService.DELETE_ASYNC(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new { },
                $"token={this.Config.Token}",
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Success;
        }

        public async Task<Item> UpdateItemImages(string ItemId, ItemImagesWrapper ItemImagesWrapper)
        {
            var response = await this.HttpService.PUT_ASYNC<UpdateItemImagesRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new UpdateItemImagesRequest(this.Config.Token, ItemImagesWrapper),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> ClearItemImages(string ItemId)
        {
            var response = await this.HttpService.PUT_ASYNC<ClearItemImagesRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new ClearItemImagesRequest(this.Config.Token),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> UpdateItemSpec(string ItemId, ItemSpecWrapper ItemSpecWrapper)
        {
            var response = await this.HttpService.PUT_ASYNC<UpdateItemSpecRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new UpdateItemSpecRequest(this.Config.Token, ItemSpecWrapper),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> ClearItemSpec(string ItemId)
        {
            var response = await this.HttpService.PUT_ASYNC<ClearItemSpecRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new ClearItemSpecRequest(this.Config.Token),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> CreateItemInventory(string ItemId, CreateItemInventoryWrapper ItemInventoryWrapper)
        {
            var response = await this.HttpService.PUT_ASYNC<CreateItemInventoryRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new CreateItemInventoryRequest(this.Config.Token, ItemInventoryWrapper),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> UpdateItemInventory(string ItemId, ItemInventoryWrapper ItemInventoryWrapper)
        {
            var response = await this.HttpService.PUT_ASYNC<UpdateItemInventoryRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new UpdateItemInventoryRequest(this.Config.Token, ItemInventoryWrapper),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> CreateItemTags(string ItemId, ItemTagsWrapper ItemTagsWrapper)
        {
            var response = await this.HttpService.PUT_ASYNC<CreateItemTagsRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new CreateItemTagsRequest(this.Config.Token, ItemTagsWrapper),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> UpdateItemTags(string ItemId, ItemTagsWrapper ItemTagsWrapper)
        {
            var response = await this.HttpService.PUT_ASYNC<UpdateItemTagsRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new UpdateItemTagsRequest(this.Config.Token, ItemTagsWrapper),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> ClearItemTags(string ItemId)
        {
            var response = await this.HttpService.PUT_ASYNC<ClearItemTagsRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new ClearItemTagsRequest(this.Config.Token),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> UpdateItemFilters(string ItemId, ItemFiltersWrapper ItemFiltersWrapper)
        {
            var response = await this.HttpService.PUT_ASYNC<UpdateItemFiltersRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new UpdateItemFiltersRequest(this.Config.Token, ItemFiltersWrapper),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> DeleteItemFilters(string ItemId, ItemFiltersWrapper ItemFiltersWrapper)
        {
            var response = await this.HttpService.PUT_ASYNC<DeleteItemFiltersRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new DeleteItemFiltersRequest(this.Config.Token, ItemFiltersWrapper),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> UpdateItemRelatedItems(string ItemId, ItemRelatedItemsWrapper ItemRelatedItemsWrapper)
        {
            var response = await this.HttpService.PUT_ASYNC<UpdateItemRelatedItemsRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new UpdateItemRelatedItemsRequest(this.Config.Token, ItemRelatedItemsWrapper),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<Item> ClearItemRelatedItems(string ItemId)
        {
            var response = await this.HttpService.PUT_ASYNC<ClearItemRelatedItemsRequest, Item>(
                $"{this.Config.ServerURL}/items/{ItemId}",
                new ClearItemRelatedItemsRequest(this.Config.Token),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<bool> HookRegisteration(HookDetails HookDetails)
        {
            var response = await this.HttpService.POST_ASYNC<HookRegisterationRequest>(
                $"{this.Config.ServerURL}/webhooks",
                new HookRegisterationRequest(this.Config.Token, HookDetails),
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Success;
        }

        // TODO ->> Implement
        public async Task<IEnumerable<HookDetails>> GetRegisteredHooks() {
            throw new NotImplementedException();
        }

        /*                
        public async Task<string> HealthCheck() 
        {            
            var response = await this.HttpService.GET_ASYNC(
                $"{this.Config.ServerURL}/Invoices/v1/Health",
                null,                
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",                    
                    ["Authorization"] = $"Bearer {this.Config.AccessToken}"
                }
            );

            // Unauthorized (401) - refresh token and try again
            if (!response.Success && response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrWhiteSpace(this.Config.RefreshToken))
            {
                
            }

            if (!response.Success)
                return $"ERROR | {this.ParseError(response.Content).InnerMessage.Error}";
            return "OK";
        }
        */

        // ---

        private string BuildSearchFiltersQuery(SearchItemsRequest Request) {
            string FormatDate(DateTime Date) {
                return Date.ToString("yyyy-MM-ddThh:mm:ssZ");
            }

            var searchFilters = new List<string>();

            if (Request.created_at_min.HasValue || Request.created_at_max.HasValue)
            {
                searchFilters.Add($"created_at_min={FormatDate(Request.created_at_min ?? DateTime.MinValue)}");
                searchFilters.Add($"created_at_max={FormatDate(Request.created_at_max ?? DateTime.MaxValue)}");
            }
            if (Request.updated_at_min.HasValue || Request.updated_at_max.HasValue)
            {
                searchFilters.Add($"updated_at_min={FormatDate(Request.updated_at_min ?? DateTime.MinValue)}");
                searchFilters.Add($"updated_at_max={FormatDate(Request.updated_at_max ?? DateTime.MaxValue)}");
            }
            if (Request.min_price.HasValue || Request.max_price.HasValue)
            {
                searchFilters.Add($"min_price={(Request.min_price ?? 0)}");
                searchFilters.Add($"max_price={(Request.max_price ?? float.MaxValue)}");
            }
            if (Request.group_value_id.HasValue) searchFilters.Add($"group_value_id={Request.group_value_id}");
            if (Request.visible.HasValue) searchFilters.Add($"visible={Request.visible}");
            if (Request.tag_id.HasValue) searchFilters.Add($"tag_id={Request.tag_id}");
            if (Request.store_category_id.HasValue) searchFilters.Add($"store_category_id={Request.store_category_id}");
            if (!string.IsNullOrEmpty(Request.sort_by)) searchFilters.Add($"sort_by={Request.sort_by}");
            if (!string.IsNullOrEmpty(Request.sort_order)) searchFilters.Add($"sort_order={Request.sort_order}");

            return string.Join("&", searchFilters);
        } 

        private APIErrorResponse ParseError(string ErrorRaw)
        {
            /*
                // schema
                <Http-Error>|<Request-Error>

                ---
                
                // schema types
                {
                    "status": "unauthorized",
                    "model": "webhook",
                    "message": "You are not authorized for any of these attributes"
                }

                -

                {
                    "status": "unauthorized",
                    "model": "webhook",
                    "message": "The token provided is not registered"
                }

                -

                {
                    "status": "not_found",
                    "message": "The endpoint you requested was not found"
                }

                -
                    
                {
                    "status": "not_found",
                    "model": "item",
                    "message": "There are no items for your query"
                }

                -

                {
                    "status": "not_found",
                    "model": "item",
                    "id": "7290016867008",
                    "message": "Item with code 7290016867008 was not found"
                }
            */
            var errorRawParts = ErrorRaw.Split('|');
            var httpError = errorRawParts[0];
            var requestError = errorRawParts[1];

            var result = new APIErrorResponse {
                Message = httpError?.Trim()                
            };

            // parse by Schema type
            var errorSchema = new
            {
                status = string.Empty,
                model = string.Empty,
                message = string.Empty
            };

            var exData = JsonConvert.DeserializeAnonymousType(requestError, errorSchema);
            result.InnerMessage = (
                exData?.status?.Trim() ?? string.Empty,
                exData?.message?.Trim() ?? string.Empty
            );

            return result;
        }
    }
}
