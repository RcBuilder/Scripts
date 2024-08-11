using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;
using Helpers;
using Newtonsoft.Json;
using System.Web;
using System.Diagnostics;
using System.Text;
using static GS1.GS1Entities;
using FieldInfo = GS1.GS1Entities.FieldInfo;
using System.Drawing;

/*
    REFERENCE
    ---------    
    see 'application users - retailer api requests-2.pdf'
    

    PROCESS
    -------
    (steps)    
    1. get messages (from unread queue or by custom search) 
    2. get each message type (e.g: New_Publish)
    3. extract the GTIN from the field 'message_identifier' 
       GET {{ServerURL}}/messages_queue/get_by_date/from/2024-01-01/to/2024-02-01

       e.g: 
       Update_Product = 7290016867008        
       New_Publish = IL_7290452900000_7290117860540_1663237481362 (OR 7290117860540)
    4. use the GTIN value to pull the product details  
       GET {{ServerURL}}product/7290117860540.json?hq=1


    API BASE URL
    ------------
    https://retailer.gs1ildigital.org/external
    

    MESSAGE TYPES
    -------------
    1. Update_Product                    // update an existing product
    2. New_Publish                       // publish of a new product
    3. Hand_Shake / Hand_Shake_Reject    // connection-request supplier-retailer
    
    notes!
    - message_identifier = product code 
      ** for New_Publish type, we can be use the full code or only the GTIN part
      ** e.g IL_7290197900006_7290011923327_1705566520542 -> IL_<GLN>_<GTIN>_1705566520542 
    - from_Gln = Supplier Id
    - to_Gln = Retailer Id (YOU)

    // message-types (samples)
    {
        "message_type": "Update_Product",
        "message_identifier": "7290016867008",
        "message_subject": "עדכון עבור מוצר שוחרר על ידי הספק",
        "message_content": "",
        "message_creation_date": "2024-01-10 11:29:56",
        "instance_from": "gs1-supplier-be",
        "instance_to": "gs1-retailer-be",
        "gln_from": "7290934800002",
        "gln_to": "7290388300004",
        "important": "0"
    },
    {
        "message_type": "New_Publish",
        "message_identifier": "IL_7290197900006_7290011923327_1705566520542",
        "message_subject": "מוצר חדש הופץ עבורך",
        "message_content": "",
        "message_creation_date": "2024-01-18 08:54:24",
        "instance_from": "gs1-supplier-be",
        "instance_to": "gs1-retailer-be",
        "gln_from": "7290197900006",
        "gln_to": "7290388300004",
        "important": "0"
    }


    FIELD TYPES
    -----------    
    1. Text (type #1)
    2. Yes/No (type #2)  
    3. Select (type #3)
    4. MultiSelect (type #4)
    5. Date (type #12)  

    // field-types (samples)
    {
        "id": "1",
        "name": "product_code",
        "field_type": "1",
        "field_type_name": "Text"
    },
    {
        "id": "5729",
        "name": "Is_Trade_Item_An_Orderable_Unit",
        "field_type": "2",
        "field_type_name": "Yes/No"
    },
    {
        "id": "5749",
        "name": "Delivery_Method",
        "field_type": "3",
        "field_type_name": "Select",
        "select_id": "76",
        "select_name": "Delivery Method",
        "select_options": [
            {
                "name": "Cross-Docking",
                "code": "7603"
            },
            {
                "name": "אספקה ישירה לסניפים",
                "code": "7601"
            },
            {
                "name": "מרלוג",
                "code": "7602"
            }
        ]
    },
    {
        "id": "5719",
        "name": "Country_of_Origin",
        "field_type": "4",
        "field_type_name": "MultiSelect",
        "select_id": "66",
        "select_name": "Countries Hebrew",
        "select_options": [            
            {
                "name": "אוסטריה",
                "code": "AT"
            },
            {
                "name": "אוסטרליה",
                "code": "AU"
            },
            {
                "name": "אוקראינה",
                "code": "UA"
            },            
            {
                "name": "איטליה",
                "code": "IT"
            }
            ....
            ....
        ]
    },
    {
        "id": "5721",
        "name": "Effective_Date_Time",
        "field_type": "12",
        "field_type_name": "Date"
    }


    POSTMAN
    -------
    - see 'GS1.postman_collection.json'    


    IMPLEMENTATIONS
    ---------------
    - see 'GS1 > PROJECT > GS1Manager'        
    - see 'Scripts > CODE > GS1 > GS1Manager'        

    
    RESEARCH
    --------
    - see 'Scripts > CODE > GS1'


    cURL
    ----        
    get messages by date:
    https://retailer.gs1ildigital.org/external/messages_queue/get_by_date/from/yyyy-mm-dd/to/yyyy-mm-dd

    sample:
    https://retailer.gs1ildigital.org/external/messages_queue/get_by_date/from/2024-01-01/to/2024-02-01

    output:
    [
        {
            "message_type": "Update_Product",
            "message_identifier": "7290016867008",
            "message_subject": "עדכון עבור מוצר שוחרר על ידי הספק",
            "message_content": "",
            "message_creation_date": "2024-01-10 11:29:56",
            "instance_from": "gs1-supplier-be",
            "instance_to": "gs1-retailer-be",
            "gln_from": "7290934800002",
            "gln_to": "7290388300004",
            "important": "0"
        },
        {
            "message_type": "Update_Product",
            "message_identifier": "IL_7290032100004_7290019153832_1676461845754",
            "message_subject": "עדכון עבור מוצר שוחרר על ידי הספק",
            "message_content": "",
            "message_creation_date": "2024-01-14 10:02:03",
            "instance_from": "gs1-supplier-be",
            "instance_to": "gs1-retailer-be",
            "gln_from": "7290032100004",
            "gln_to": "7290388300004",
            "important": "0"
        },
        {
            "message_type": "New_Publish",
            "message_identifier": "IL_7290197900006_7290011923327_1705566520542",
            "message_subject": "מוצר חדש הופץ עבורך",
            "message_content": "",
            "message_creation_date": "2024-01-18 08:54:24",
            "instance_from": "gs1-supplier-be",
            "instance_to": "gs1-retailer-be",
            "gln_from": "7290197900006",
            "gln_to": "7290388300004",
            "important": "0"
        },
        {
            "message_type": "New_Publish",
            "message_identifier": "IL_7290197900006_7290011923365_1705568109269",
            "message_subject": "מוצר חדש הופץ עבורך",
            "message_content": "",
            "message_creation_date": "2024-01-18 09:00:09",
            "instance_from": "gs1-supplier-be",
            "instance_to": "gs1-retailer-be",
            "gln_from": "7290197900006",
            "gln_to": "7290388300004",
            "important": "0"
        }
    ]

    USING
    -----
    var gs1Manager = new GS1Manager(new GS1Entities.GS1Config
    {
        ServerURL = "https://retailer.gs1ildigital.org/external",
        ApiUserName = "xxxxxxxxxxxxxxx",
        ApiPassword = "xxxxxxxxxxxxxxx"
    });

    var fields = await gs1Manager.GetProductFields();
    Console.WriteLine($"{fields.Count()} fields:");            
    foreach (var field in fields) Console.WriteLine($"#{field.id} {field.name}");

    -

    var field = await gs1Manager.GetProductField("modification_timestamp");                  
    Console.WriteLine($"#{field.id} {field.name}");

    -

    var messages = await gs1Manager.GetUnReadMessages();
    if (messages != null) {
        Console.WriteLine($"{messages.Count()} messages:");
        foreach (var message in messages) Console.WriteLine($"{message.message_type} | #{message.message_identifier}");
    }

    -

    var messages = await gs1Manager.GetMessagesByDateRange(DateTime.Now.AddDays(-3));
    if (messages != null) {
        Console.WriteLine($"{messages.Count()} messages:");
        foreach (var message in messages) Console.WriteLine($"{message.message_type} | #{message.message_identifier}");
    }

    -

    var messages = await gs1Manager.GetMessagesByDateRange(new DateTime(2024, 1, 1), new DateTime(2024, 1, 20));
    if (messages != null) {
        Console.WriteLine($"{messages.Count()} messages:");
        foreach (var message in messages) Console.WriteLine($"{message.message_type} | #{message.message_identifier}");
    }

    -

    var product = await gs1Manager.GetProductDetails("7290016867008");                         
    Console.WriteLine($"{product?.product_info?.Main_Fields?.GTIN} | {product?.product_info?.Main_Fields?.Trade_Item_Description}");      
    
    -

    var assetPath = await gs1Manager.DownloadProductMediaAsset("7290016867008", "226879", "7290016867008_S1_15.jpg");
    Console.WriteLine(assetPath);
    var image = Image.FromFile(assetPath);
    image.Save(assetPath.Replace(".jpg", "_COPY.jpg"));
    Console.WriteLine($"image {image.Size.Width}x{image.Size.Height}");

    -

    var filePath = await gs1Manager.DownloadProductMediaAssetsAsZip("7290016867008", "7290016867008_Assets.zip");
    Console.WriteLine(filePath);

    -
*/

namespace GS1
{
    public class GS1Entities
    {
        public class GS1Config
        {
            public string ServerURL { get; set; }
            public string ApiUserName { get; set; }
            public string ApiPassword { get; set; }
            public string AssetsTempFolder { get; set; }
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

        /// TODO ->> Complete + Fields Names + Extended        
        public class FieldInfo
        {            
            public string id { get; set; }
            public string name { get; set; }
            public string field_type { get; set; }
            public string field_type_name { get; set; }
            public string select_id { get; set; }
            public string select_name { get; set; }
            public IEnumerable<SelectOption> select_options { get; set; }   
        }

        public class SelectOption {
            public string code { get; set; }
            public string name { get; set; }            
        }

        public class Message
        {
            public string message_type { get; set; }
            public string message_identifier { get; set; }
            public string message_subject { get; set; }
            public string message_content { get; set; }
            public string message_creation_date { get; set; }
            public string instance_from { get; set; }
            public string instance_to { get; set; }
            public string gln_from { get; set; }
            public string gln_to { get; set; }
            public string important { get; set; }
        }

        #region [Product] Auto Generated Code 
        /// <summary>
        /// https://json2csharp.com/ 
        /// </summary>        
        public class AdditionalFeatures
        {
            public List<PrivateBrand> Private_Brand { get; set; }
            public List<ParallelImport> Parallel_Import { get; set; }
            public List<ItemFranchisee> Item_Franchisee { get; set; }
            public List<RegulatedPrice> Regulated_Price { get; set; }
            public string Alternate_Item { get; set; }
            public List<NewItemClassification> New_Item_Classification { get; set; }
            public List<ItemWithADeposit> Item_with_a_Deposit { get; set; }
            public string Internal_Classification { get; set; }
            public string Remarks { get; set; }
            public string Product_Categories_Classification { get; set; }
            public string internal_product_description_additional { get; set; }
            public string Substituted_Item { get; set; }
            public string Remark_to_Customer { get; set; }
            public string Imported_Item { get; set; }
            public string Produced_in_Israel { get; set; }
            public string animal_tested { get; set; }
            public string expose_for_app_from { get; set; }
        }

        public class AdditionalInformation
        {
            public string Serving_Suggestion { get; set; }
            public string Consumer_Storage_Instructions { get; set; }
            public string Fat_Percentage_in_Product { get; set; }
            public string Cream_Percentage_in_Product { get; set; }
            public string Fruit_Percentage_in_Product { get; set; }
            public string Alcohol_Percentage_in_Product { get; set; }
            public List<ForbiddenUnderTheAgeOf18> Forbidden_Under_the_Age_of_18 { get; set; }
            public string Hazard_Precautionary_Statement { get; set; }
            public ServingSizeDescription Serving_Size_Description { get; set; }
            public string pH { get; set; }
            public string Color_Number { get; set; }
            public string Color { get; set; }
            public string Color_Code { get; set; }
            public string Skin_Type { get; set; }
            public string Hair_Type { get; set; }
            public List<ContainsSulfurDioxide> Contains_Sulfur_Dioxide { get; set; }
            public List<FoodSymbolRed> Food_Symbol_Red { get; set; }
        }

        public class AllergenDisplayed
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class AllergenTypeCodeAndContainment
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class AllergenTypeCodeAndContainmentMayContain
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class BoardOfSupervision
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class CaseOrCartonDimensions
        {
            public List<PackagingTypeCode> Packaging_Type_Code { get; set; }
            public string Amount_of_Products_in_Package_or_Carton { get; set; }
            public PackageHeight Package_Height { get; set; }
            public PackageDepth Package_Depth { get; set; }
            public PackageWidth Package_Width { get; set; }
            public PackageGrossWeight Package_Gross_Weight { get; set; }
            public PackageVolume Package_Volume { get; set; }
            public string GTIN_Package { get; set; }
        }

        public class ColLabel
        {
            public string label { get; set; }
            public string code { get; set; }
            public string field_id { get; set; }
        }

        public class ContainsSulfurDioxide
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class CookingIsrael
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class CountryOfOrigin
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class DeliveryMethod
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class DietInformation
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class Field
        {
            public string col_label { get; set; }
            public string col_code { get; set; }
            public string col_field_id { get; set; }
            public string field_id { get; set; }
            public string field_name { get; set; }
            public string name { get; set; }
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class FoodSymbolRed
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class ForbiddenUnderTheAgeOf18
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class GeneralInformation
        {
            public List<TargetMarket> Target_Market { get; set; }
            public string Manufacturer_Name { get; set; }
            public string Additional_Trade_Item_Description_1 { get; set; }
            public string Product_Description_English { get; set; }
            public string Manufacturer_Address { get; set; }
            public string Search_Words { get; set; }
            public string Manufacturer_Product_Code { get; set; }
            public string Discontinued_Date_Time { get; set; }
            public string drained_weight { get; set; }
        }

        public class IngredientDisplay
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class InternalSystemFields
        {
            public string group_id { get; set; }
            public string id { get; set; }
            public string DisplayName { get; set; }
            public string Name { get; set; }
            public string ProductName { get; set; }
            public List<UnavailabilityType> unavailability_type { get; set; }
            public string product_code { get; set; }
            public List<ProductStatus> Product_Status { get; set; }
        }

        public class IsNonSoldTradeItemReturnable
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class IsPackagingMarkedReturnable
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class IsraelMilk
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class IsTradeItemABaseUnit
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class IsTradeItemAConsumerUnit
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class IsTradeItemAnInvoiceUnit
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class IsTradeItemAnOrderableUnit
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class IsTradeItemAVariableUnit
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class ItemFranchisee
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class ItemWithADeposit
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class Kashrut
        {
            public List<KosherForPassover> Kosher_for_Passover { get; set; }
            public List<KosherSupervisionType> Kosher_Supervision_Type { get; set; }
            public List<Rabbinate> Rabbinate { get; set; }
            public List<BoardOfSupervision> Board_of_Supervision { get; set; }
            public List<CookingIsrael> Cooking_Israel { get; set; }
            public List<IsraelMilk> Israel_Milk { get; set; }
            public List<SabbathObservingPlant> Sabbath_Observing_Plant { get; set; }
            public string Sheviit_Orlah_Tevel { get; set; }
            public string Kosher_for_Passover_Remark { get; set; }
        }

        public class KosherForPassover
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class KosherSupervisionType
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class LogisticsAndCommercialPropertiesAdditional
        {
            public string First_Order_Date { get; set; }
            public string Last_Order_Date { get; set; }
            public ProductShelfLife Product_Shelf_Life { get; set; }
            public OrderQuantityMinimumSize Order_Quantity_Minimum_Size { get; set; }
            public OrderQuantityMaximumSize Order_Quantity_Maximum_Size { get; set; }
            public List<IsNonSoldTradeItemReturnable> Is_Non_Sold_Trade_Item_Returnable { get; set; }
            public OrderingLeadTime Ordering_Lead_Time { get; set; }
            public List<IsPackagingMarkedReturnable> Is_Packaging_Marked_Returnable { get; set; }
            public List<DeliveryMethod> Delivery_Method { get; set; }
            public string Transportation_Temperature_Minimum { get; set; }
            public string Transportation_Temperature_Maximum { get; set; }
            public string Storage_Temperature_Minimum { get; set; }
            public string Storage_Temperature_Maximum { get; set; }
            public string Distribution_Temperature_Minimum { get; set; }
            public string Distribution_Temperature_Maximum { get; set; }
        }

        public class LogisticsAndCommercialPropertiesGeneral
        {
            public List<IsTradeItemABaseUnit> Is_Trade_Item_A_Base_Unit { get; set; }
            public List<IsTradeItemAConsumerUnit> Is_Trade_Item_A_Consumer_Unit { get; set; }
            public List<IsTradeItemAnOrderableUnit> Is_Trade_Item_An_Orderable_Unit { get; set; }
            public List<IsTradeItemAVariableUnit> Is_Trade_Item_A_Variable_Unit { get; set; }
            public List<IsTradeItemAnInvoiceUnit> Is_Trade_Item_An_Invoice_Unit { get; set; }
        }

        public class LogisticsUnitDepth
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class LogisticsUnitGrossWeight
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class LogisticsUnitHeight
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class LogisticsUnitWidth
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class MainFields
        {
            public string GTIN { get; set; }
            public string Trade_Item_Description { get; set; }
            public string Short_Description { get; set; }
            public NetContent Net_Content { get; set; }
            public string Supplier_Catalog_Number { get; set; }
            public string GLN { get; set; }
            public string BrandName { get; set; }
            public string Sub_Brand_Name { get; set; }
            public string functionalName { get; set; }
            public string Variant { get; set; }
            public List<CountryOfOrigin> Country_of_Origin { get; set; }
            public string Effective_Date_Time { get; set; }
            public string GPC_Category_Code { get; set; }
            public List<TradeItemUnitDescriptor> Trade_Item_Unit_Descriptor { get; set; }
            public string internal_product_description { get; set; }
            public string user_company { get; set; }
        }

        public class MarketingInformation
        {
            public string Trade_Item_Marketing_Message { get; set; }
            public string Trade_Item_Marketing_Message_2 { get; set; }
            public string Trade_Item_Marketing_Message_3 { get; set; }
            public string Trade_Item_Marketing_Message_4 { get; set; }
            public string Trade_Item_Marketing_Message_5 { get; set; }
            public string Trade_Item_Marketing_Message_6 { get; set; }
            public string Trade_Item_Marketing_Message_7 { get; set; }
        }

        public class MediaAsset
        {
            public string id { get; set; }
            public string retailer_gln { get; set; }
            public string GTIN { get; set; }
            public string GLN { get; set; }
            public string filename { get; set; }
            public string filename_display { get; set; }
            public string file_extension { get; set; }
            public string width { get; set; }
            public string height { get; set; }
            public string show_in_gallery { get; set; }
            public string drawing_key { get; set; }
            public string hidden { get; set; }
            public string language_code { get; set; }
            public string translation_of_file_id { get; set; }
            public string file_size { get; set; }
            public string source { get; set; }
            public string modification_timestamp { get; set; }
            public string system_id { get; set; }
            public string image_type { get; set; }
            public string facing { get; set; }
            public string plunge_angle { get; set; }
            public string arc_position_number { get; set; }
            public string default_image { get; set; }
            public string product_pic_designation { get; set; }
            public string publication_timestamp { get; set; }
            public string publish_file { get; set; }
            public string creation_timestamp { get; set; }
        }

        public class NetContent
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class NetWeight
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class NewItemClassification
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class NutritionalValues
        {
            public Table table { get; set; }
        }

        public class OrderingLeadTime
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class OrderQuantityMaximumSize
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class OrderQuantityMinimumSize
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class PackageDepth
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class PackageGrossWeight
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class PackageHeight
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class PackageVolume
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class PackageWidth
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class PackagingTypeCode
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class PalletOrLogisticUnitDimensions
        {
            public LogisticsUnitHeight Logistics_Unit_Height { get; set; }
            public LogisticsUnitDepth Logistics_Unit_Depth { get; set; }
            public LogisticsUnitWidth Logistics_Unit_Width { get; set; }
            public LogisticsUnitGrossWeight Logistics_Unit_Gross_Weight { get; set; }
            public PalletVolume Pallet_Volume { get; set; }
            public string Quantity_Of_TradeItems_Per_Pallet_Layer { get; set; }
            public string Quantity_Of_Layers_Per_Pallet { get; set; }
            public string Quantity_Of_Trade_Items_Per_Pallet { get; set; }
        }

        public class PalletVolume
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class ParallelImport
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class PriceComparisonContent
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class PrivateBrand
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class ProductComponentsAndInstructionsGeneral
        {
            public List<DietInformation> Diet_Information { get; set; }
            public string Ingredient_Sequence_and_Name { get; set; }
            public List<AllergenTypeCodeAndContainment> Allergen_Type_Code_and_Containment { get; set; }
            public List<AllergenTypeCodeAndContainmentMayContain> Allergen_Type_Code_and_Containment_May_Contain { get; set; }
            public string Healthy_Product { get; set; }
            public List<IngredientDisplay> Ingredient_Display { get; set; }
            public List<AllergenDisplayed> Allergen_Displayed { get; set; }
        }

        public class ProductDepth
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class ProductDimensions
        {
            public ProductHeight Product_Height { get; set; }
            public ProductDepth Product_Depth { get; set; }
            public ProductWidth Product_Width { get; set; }
            public NetWeight Net_Weight { get; set; }
            public ProductGrossWeight Product_Gross_Weight { get; set; }
            public PriceComparisonContent Price_Comparison_Content { get; set; }
        }

        public class ProductGrossWeight
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class ProductHeight
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class ProductInfo
        {
            public MainFields Main_Fields { get; set; }
            public GeneralInformation General_Information { get; set; }
            public MarketingInformation Marketing_Information { get; set; }
            public LogisticsAndCommercialPropertiesGeneral Logistics_and_Commercial_Properties_General { get; set; }
            public LogisticsAndCommercialPropertiesAdditional Logistics_and_Commercial_Properties_Additional { get; set; }
            public ProductDimensions Product_Dimensions { get; set; }
            public TrayDimensions Tray_Dimensions { get; set; }
            public CaseOrCartonDimensions Case_or_Carton_Dimensions { get; set; }
            public PalletOrLogisticUnitDimensions Pallet_or_Logistic_Unit_Dimensions { get; set; }
            public PromotionalProductInformation Promotional_Product_Information { get; set; }
            public Kashrut Kashrut { get; set; }
            public ProductComponentsAndInstructionsGeneral Product_Components_and_Instructions_General { get; set; }
            public AdditionalInformation Additional_Information { get; set; }
            public SystemFeatures System_Features { get; set; }
            public AdditionalFeatures Additional_Features { get; set; }
            public InternalSystemFields Internal_System_Fields { get; set; }
            ///public NutritionalValues Nutritional_Values { get; set; }  // TODO->> causes json ex
        }

        public class ProductShelfLife
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class ProductStatus
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class ProductWidth
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class PromotionalProductInformation
        {
            public List<PromotionTypeCode> Promotion_Type_Code { get; set; }
            public string Free_Quantity_Of_Product { get; set; }
            public string promotion_of { get; set; }
        }

        public class PromotionTypeCode
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class Rabbinate
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class RegulatedPrice
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class Row
        {
            public string label { get; set; }
            public string code { get; set; }
            public List<Field> fields { get; set; }
        }

        public class SabbathObservingPlant
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class ServingSizeDescription
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class SystemFeatures
        {
            public string Publication_Date_Time { get; set; }
            public string modification_timestamp { get; set; }
            public string Creation_Date_Time { get; set; }
        }

        public class Table
        {
            public string tableName { get; set; }
            public int numberOfRows { get; set; }
            public int numberOfCols { get; set; }
            public List<ColLabel> colLabels { get; set; }
            public List<Row> rows { get; set; }
        }

        public class TargetMarket
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class TradeItemUnitDescriptor
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class TrayDepth
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class TrayDimensions
        {
            public List<TrayTypeCode> Tray_Type_Code { get; set; }
            public string Amount_of_Products_in_Tray { get; set; }
            public TrayHeight Tray_Height { get; set; }
            public TrayDepth Tray_Depth { get; set; }
            public TrayWidth Tray_Width { get; set; }
            public TrayVolume Tray_Volume { get; set; }
            public TrayGrossWeight Tray_Gross_Weight { get; set; }
        }

        public class TrayGrossWeight
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class TrayHeight
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class TrayTypeCode
        {
            public string value { get; set; }
            public string code { get; set; }
        }

        public class TrayVolume
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class TrayWidth
        {
            public string text { get; set; }
            public string UOM { get; set; }
            public string value { get; set; }
        }

        public class UnavailabilityType
        {
            public string value { get; set; }
            public string code { get; set; }
        }
        #endregion
        
        public class Product
        {
            public ProductInfo product_info { get; set; }
            ///public List<List<object>> private_data { get; set; }
            public List<MediaAsset> media_assets { get; set; }
            ///public List<object> multi_pack { get; set; }
        }
    }

    public interface IGS1Manager
    {
        Task<IEnumerable<FieldInfo>> GetProductFields();
        Task<FieldInfo> GetProductField(string FieldName);
        Task<IEnumerable<Message>> GetUnReadMessages();
        Task<IEnumerable<Message>> GetMessagesByDateRange(DateTime DateFrom);
        Task<IEnumerable<Message>> GetMessagesByDateRange(DateTime DateFrom, DateTime DateTo);
        Task<Product> GetProductDetails(string GTIN);
        Task<string> DownloadProductMediaAsset(string GTIN, string AssetId, string AssetName);
        Task<string> DownloadProductMediaAsset(string GTIN, string AssetId, string AssetName, string AssetsTempFolder);
        Task<string> DownloadProductMediaAssetsAsZip(string GTIN, string AssetsDestZipPath);
        Task<string> DownloadProductMediaAssetsAsZip(string GTIN, string AssetsDestZipPath, string AssetsTempFolder);
    }

    public class GS1Manager : IGS1Manager
    {        
        protected GS1Config Config { get; set; }
        protected HttpServiceHelper HttpService { get; set; }

        public GS1Manager(GS1Config Config)
        {
            this.Config = Config;
            this.HttpService = new HttpServiceHelper();

            if (string.IsNullOrEmpty(this.Config.AssetsTempFolder))
                this.Config.AssetsTempFolder = $"{AppDomain.CurrentDomain.BaseDirectory}TEMP\\";
        }

        public async Task<IEnumerable<FieldInfo>> GetProductFields() {
            var response = await this.HttpService.GET_ASYNC<IEnumerable<FieldInfo>>(
                $"{this.Config.ServerURL}/product/fieldInfo.json?field=All",
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{this.Config.ApiUserName}:{this.Config.ApiPassword}"))}"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model;
        }

        public async Task<FieldInfo> GetProductField(string FieldName)
        {
            var response = await this.HttpService.GET_ASYNC<IEnumerable<FieldInfo>>(
                $"{this.Config.ServerURL}/product/fieldInfo.json?field={FieldName?.ToLower()}",
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{this.Config.ApiUserName}:{this.Config.ApiPassword}"))}"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return response.Model?.FirstOrDefault();
        }

        public async Task<IEnumerable<Message>> GetUnReadMessages()
        {
            var response = await this.HttpService.GET_ASYNC<IEnumerable<Message>>(
                $"{this.Config.ServerURL}/messages_queue/get",
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{this.Config.ApiUserName}:{this.Config.ApiPassword}"))}"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            // handle no messages response
            var message = this.ParseMessageResponse(response.Content);
            if (!string.IsNullOrEmpty(message))
                throw new APIException(this.ParseError(response.Content));            

            return response.Model;
        }

        public async Task<IEnumerable<Message>> GetMessagesByDateRange(DateTime DateFrom) {
            return await this.GetMessagesByDateRange(DateFrom, DateTime.Now);
        }
        public async Task<IEnumerable<Message>> GetMessagesByDateRange(DateTime DateFrom, DateTime DateTo)
        {
            var DATE_FORMAT = "yyyy-MM-dd";
            var response = await this.HttpService.GET_ASYNC<IEnumerable<Message>>(
                $"{this.Config.ServerURL}/messages_queue/get_by_date/from/{DateFrom.ToString(DATE_FORMAT)}/to/{DateTo.ToString(DATE_FORMAT)}",
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{this.Config.ApiUserName}:{this.Config.ApiPassword}"))}"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));
            
            // handle no messages response
            var message = this.ParseMessageResponse(response.Content);
            if (!string.IsNullOrEmpty(message)) {
                if (message.Contains("No inbox Messages")) {
                    response.Model = null;
                }
                else throw new APIException(this.ParseError(response.Content));
            }

            return response.Model;
        }

        public async Task<Product> GetProductDetails(string GTIN) {
            var response = await this.HttpService.GET_ASYNC<IEnumerable<Product>>(
                $"{this.Config.ServerURL}/product/{GTIN}.json?hq=1",
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{this.Config.ApiUserName}:{this.Config.ApiPassword}"))}"
                }
            );

            // response.StatusCode: Unauthorized (401)
            // response.StatusCode: NotFound (404)
            if (!response.Success)
            {
                if (response.StatusCode == HttpStatusCode.NotFound) 
                    return null;
                throw new APIException(this.ParseError(response.Content));
            }
            
            // handle no messages response
            var message = this.ParseMessageResponse(response.Content);
            if (!string.IsNullOrEmpty(message))
                throw new APIException(this.ParseError(response.Content));

            return response.Model?.FirstOrDefault();
        }

        public async Task<string> DownloadProductMediaAsset(string GTIN, string AssetId, string AssetName) {
            return await this.DownloadProductMediaAsset(GTIN, AssetId, AssetName, this.Config.AssetsTempFolder);
        }
        public async Task<string> DownloadProductMediaAsset(string GTIN, string AssetId, string AssetName, string AssetsTempFolder)
        {
            if (!Directory.Exists(AssetsTempFolder))
                Directory.CreateDirectory(AssetsTempFolder);

            var assetDestPath = $"{AssetsTempFolder}{AssetName}";
            var response = await this.HttpService.DOWNLOAD_DATA_ASYNC(
                $"{this.Config.ServerURL}/product/{GTIN}/files?media={AssetId}&hq=1",
                assetDestPath,
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{this.Config.ApiUserName}:{this.Config.ApiPassword}"))}"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return assetDestPath;
        }

        public async Task<string> DownloadProductMediaAssetsAsZip(string GTIN, string ZipName) {
            return await this.DownloadProductMediaAssetsAsZip(GTIN, ZipName, this.Config.AssetsTempFolder);
        }
        public async Task<string> DownloadProductMediaAssetsAsZip(string GTIN, string ZipName, string AssetsTempFolder)
        {
            if (!Directory.Exists(AssetsTempFolder))
                Directory.CreateDirectory(AssetsTempFolder);

            var assetsDestZipPath = $"{AssetsTempFolder}{ZipName}";
            var response = await this.HttpService.DOWNLOAD_DATA_ASYNC(
                $"{this.Config.ServerURL}/product/{GTIN}/files?media=all&hq=1",
                assetsDestZipPath,
                null,
                new Dictionary<string, string>
                {
                    ["Accept"] = "application/json",
                    ["Content-Type"] = "application/json",
                    ["Authorization"] = $"Basic {Convert.ToBase64String(Encoding.Default.GetBytes($"{this.Config.ApiUserName}:{this.Config.ApiPassword}"))}"
                }
            );

            // response.StatusCode: Unauthorized (401)
            if (!response.Success)
                throw new APIException(this.ParseError(response.Content));

            return assetsDestZipPath;
        }

        public string ExtractGTINFromMessageId(string MessageId) {
            try {
                return MessageId.Split('_')[2];
            }
            catch {
                return MessageId;
            }
        }

        // ---

        private string ParseMessageResponse(string ErrorRaw) 
        {
            // [{"msg":"No un-Read Messages in inbox for GLN 7290388300004"}]
            var msgSchema = new[] {
                new {
                    msg = string.Empty
                }
            };

            var result = JsonConvert.DeserializeAnonymousType(ErrorRaw, msgSchema);
            return result?.FirstOrDefault()?.msg;
        }

        private APIErrorResponse ParseError(string ErrorRaw)
        {
            /*
                // schema
                <Http-Error>|<Request-Error>

                ---
                
                // schema types
                {
                    "error": true,
                    "message": "No authorized user available. You don't have permissions to access / on this request.",
                    "code": 401
                }            

                -

                [{
                    "msg":"No un-Read Messages in inbox for GLN 7290388300004"
                }]

                -

                Cannot deserialize the current JSON array (e.g. [1,2,3]) into type 'GS1.GS1Entities+NutritionalValues' because the type requires a JSON object (e.g. {"name":"value"}) to deserialize correctly.
                To fix this error either change the JSON to a JSON object (e.g. {"name":"value"}) or change the deserialized type to an array or a type that implements a collection interface (e.g. ICollection, IList) like List<T> that can be deserialized from a JSON array. JsonArrayAttribute can also be added to the type to force it to deserialize from a JSON array.
                Path '[0].product_info.Nutritional_Values', line 1, position 8859.
            */
            var errorRawParts = ErrorRaw.Split('|');
            string httpError = "000", requestError = "UNKNOWN";
            if (errorRawParts.Count() == 1)                
                requestError = errorRawParts[0];            
            else
            {
                httpError = errorRawParts[0];
                requestError = errorRawParts[1];
            }

            var result = new APIErrorResponse {
                Message = httpError?.Trim()                
            };

            // parse by Schema type
            if (requestError.Contains("error") && requestError.Contains("code"))
            {
                var errorSchema = new
                {
                    error = true,
                    message = string.Empty,
                    code = 0
                };

                var exData = JsonConvert.DeserializeAnonymousType(requestError, errorSchema);
                result.InnerMessage = (
                    exData?.code.ToString(),
                    exData?.message?.Trim() ?? string.Empty
                );
            }
            else if(requestError.Contains("msg")) {
                var msg = this.ParseMessageResponse(requestError);
                result.InnerMessage = (msg, msg);
            }            
            else result.InnerMessage = (requestError, string.Empty);

            return result;
        }
    }
}
