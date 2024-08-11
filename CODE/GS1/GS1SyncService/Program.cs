using GS1;
using Konimbo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Konimbo.KonimboEntities;

namespace GS1SyncService
{
    class Program
    {
        static string GS1_SERVER_URL = ConfigurationManager.AppSettings["GS1ServerURL"].Trim();
        static string GS1_API_USERNAME = ConfigurationManager.AppSettings["GS1ApiUserName"].Trim();
        static string GS1_API_PASSWORD = ConfigurationManager.AppSettings["GS1ApiPassword"].Trim();
        static int GS1_MESSAGES_NUM_DAYS_TO_FETCH = Convert.ToInt32(ConfigurationManager.AppSettings["GS1MessagesNumDaysToFetch"]);

        static string KONIMBO_SERVER_URL = ConfigurationManager.AppSettings["KonimboServerURL"].Trim();
        static string KONIMBO_TOKEN = ConfigurationManager.AppSettings["KonimboToken"].Trim();

        static string FOLDER_HTTP = ConfigurationManager.AppSettings["ImagesFolderHttp"].Trim();
        static string FOLDER_LOCAL = ConfigurationManager.AppSettings["ImagesFolderLocal"].Trim();

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding("Windows-1255");
            /// Console.WriteLine(new String("אבגד".Reverse().ToArray()));
            
            var gs1Manager = new GS1Manager(new GS1Entities.GS1Config
            {
                ServerURL = GS1_SERVER_URL,
                ApiUserName = GS1_API_USERNAME,
                ApiPassword = GS1_API_PASSWORD
            });

            var konimboManager = new KonimboManager(new KonimboConfig
            {
                ServerURL = KONIMBO_SERVER_URL,                
                Token = KONIMBO_TOKEN
            });

            /*
            var messages = await gs1Manager.GetUnReadMessages();
            if (messages != null)
            {
                Console.WriteLine($"{messages.Count()} messages:");
                foreach (var message in messages) Console.WriteLine($"{message.message_type} | #{message.message_identifier}");
            }
            */

            Console.WriteLine("fetching data from GS1 provider...");

            
            /*   
            // Test 
            var messages = await gs1Manager.GetMessagesByDateRange(DateTime.Parse("2023-05-15"), DateTime.Parse("2023-05-15"));
            messages = messages.Where(x => x.message_identifier.Contains("7290011498993"));
            */

            var messages = await gs1Manager.GetMessagesByDateRange(DateTime.Now.AddDays(-GS1_MESSAGES_NUM_DAYS_TO_FETCH));

            if (messages != null)
            {
                var ignoreTypes = new List<string> { "Termination_Of_Product_Supply", "Hand_Shake_Accept" }; // Update_Product | New_Publish | Termination_Of_Product_Supply | Hand_Shake_Accept
                var filtered = messages.Where(x => !ignoreTypes.Contains(x.message_type)).ToList();

                Console.WriteLine($"{filtered.Count} ({messages.Count()}) messages:");
                Logger.WriteInfo("GetMessagesByDateRange", $"{filtered.Count} ({messages.Count()}) messages:");

                foreach (var message in filtered) {                    
                    Console.WriteLine($"{message.message_type} | #{message.message_identifier}");
                    await ProcessProduct(message, gs1Manager, konimboManager);
                }
            }

            Logger.WriteHR();
            Console.WriteLine("completed! (press any key to close)");
            Console.ReadKey();                            
        }

        static async Task<bool> ProcessProduct(GS1Entities.Message message, GS1Manager gs1Manager, KonimboManager konimboManager) {
            try
            {
                var product = await gs1Manager.GetProductDetails(message.message_identifier);
                if (product == null)
                {
                    var extructed = gs1Manager.ExtractGTINFromMessageId(message.message_identifier);
                    Console.WriteLine($"Extract GTIN: IN = {message.message_identifier}, OUT = {extructed}");
                    product = await gs1Manager.GetProductDetails(extructed);
                }

                var productGTIN = product?.product_info?.Main_Fields?.GTIN;
                Console.WriteLine($"gs1: {productGTIN} | {product?.product_info?.Main_Fields?.Trade_Item_Description?.Trim()}");

                // note! in konimbo for this client there's a config change to pull items by code (instead of id)
                // therefore, use GetItem(GTIN) instead of GetItemByCode(GTIN)
                var konimboItem = await konimboManager.GetItem(productGTIN);
                return false;
                /*
                if (konimboItem == null) 
                {
                    // CREATE
                    konimboItem = new Item { 
                        title = product?.product_info?.Main_Fields?.Short_Description,
                        brand = product?.product_info?.Main_Fields?.BrandName,
                        code  = productGTIN,
                        ///price = 0,
                        desc = product?.product_info?.Main_Fields?.Trade_Item_Description,
                        position = "999",
                        zap_visable = true,
                        store_visable = true,
                        delivery_time = 7,
                        default_shipping = true,
                        zap_product_type = "מוצר חדש",
                        store_category_title = "GS1"
                    };                    

                    Console.WriteLine($"konimbo: CREATE Item");
                    await konimboManager.CreateItem(konimboItem);
                    return true;
                }
                */

                if (konimboItem == null) {
                    Console.WriteLine($"not exists in konimbo store - skip...");
                    return true;
                }

                var konimboItemDetails = konimboItem.ToItemDetails();
                var konimboItemImages = konimboItem.images;

                var mainFields = product?.product_info?.Main_Fields;
                var generalInfo = product?.product_info?.General_Information;
                var instractions = product?.product_info?.Product_Components_and_Instructions_General;
                var kashrut = product?.product_info?.Kashrut;
                var media = product?.media_assets;

                // UPDATE
                konimboItemDetails.title = mainFields?.Short_Description;
                konimboItemDetails.brand = mainFields?.BrandName;                
                konimboItemDetails.promotions = $@"{generalInfo?.Search_Words}, {generalInfo?.Product_Description_English}";

                var Allergen_Type_Code_and_Containment = instractions?.Allergen_Type_Code_and_Containment?.FirstOrDefault();
                var Allergen_Type_Code_and_Containment_May_Contain = instractions?.Allergen_Type_Code_and_Containment_May_Contain?.FirstOrDefault();

                konimboItemDetails.desc = $@"          
                    <p>{generalInfo?.Additional_Trade_Item_Description_1}</p>
                    <p>רכיבים: {instractions?.Ingredient_Sequence_and_Name}</p>
                ";

                var dietInformation = instractions?.Diet_Information?.FirstOrDefault()?.value;
                var hasDietInformation = !string.IsNullOrEmpty(dietInformation);

                if (hasDietInformation) 
                    konimboItemDetails.desc = $@"
                        {konimboItemDetails.desc}                        
                        <p>מידע תזונתי: {dietInformation}</p>                                      
                    ";

                var hasAllergen = !string.IsNullOrEmpty(Allergen_Type_Code_and_Containment?.value);
                var hasAllergen2 = !string.IsNullOrEmpty(Allergen_Type_Code_and_Containment_May_Contain?.value);

                if (hasAllergen || hasAllergen2)
                    konimboItemDetails.desc = $@"
                        {konimboItemDetails.desc}                        
                        <p style=""padding-top: 20px;"">מידע על אלרגנים:</p>                        
                    ";

                if (hasAllergen)
                    konimboItemDetails.desc = $@"
                        {konimboItemDetails.desc}                                                
                        <p>מכיל {Allergen_Type_Code_and_Containment?.value}</p>                        
                    ";

                if (hasAllergen2)
                    konimboItemDetails.desc = $@"
                        {konimboItemDetails.desc}
                        <p>עלול להכיל {Allergen_Type_Code_and_Containment_May_Contain?.value}</p>
                    ";

                var hasKashrut = kashrut.Rabbinate != null && kashrut.Rabbinate.Count > 0 && !string.IsNullOrEmpty(kashrut.Rabbinate.FirstOrDefault()?.value);
                if (hasKashrut)
                    konimboItemDetails.desc = $@"
                        {konimboItemDetails.desc}
                        
                        <p style=""padding-top: 20px;"">כשרות:</p>
                        <p>בהשגחת {kashrut.Rabbinate?.FirstOrDefault()?.value}, {kashrut.Board_of_Supervision?.FirstOrDefault()?.value}</p>                        
                    ";

                Logger.WriteInfo("ProcessProduct", $"konimbo: UPDATE Details for Item #{konimboItemDetails?.id}");
                Console.WriteLine($"konimbo: UPDATE Details for Item #{konimboItemDetails?.id}");
                await konimboManager.UpdateItemDetails(productGTIN, konimboItemDetails);

                                
                if (media != null && media.Count > 0) {

                    var onlyImages = media.Where(x => !x.filename.EndsWith(".zip")).GroupBy(x => x.filename_display).Select(g => g.OrderBy(x => x.file_size).FirstOrDefault()); // only images!
                    var hasImageUpdates = onlyImages
                        .Select(img => $"{img.filename_display}.{img.file_extension}")
                        .Intersect(konimboItemImages.Select(img => img.alt))
                        .Count() != onlyImages.Count();


                    if (hasImageUpdates) {
                        konimboItemImages.Clear();
                        foreach (var asset in onlyImages) {
                            var assetIndex = 0;
                            var imageName = $"{asset.filename_display}.{asset.file_extension}";
                            var localMediaPath = await gs1Manager.DownloadProductMediaAsset(productGTIN, asset.id, imageName, $"{FOLDER_LOCAL}");
                            var httpMediaPath = $"{FOLDER_HTTP}{imageName}";

                            konimboItemImages.Add(new ItemImage
                            {                                
                                alt = imageName,
                                position = assetIndex++,  // asset.arc_position_number
                                url = httpMediaPath
                            });
                        }

                        Logger.WriteInfo("ProcessProduct", $"konimbo: UPDATE Assets for Item #{konimboItemDetails?.id}");
                        Console.WriteLine($"konimbo: UPDATE Assets for Item #{konimboItemDetails?.id}");
                        await konimboManager.UpdateItemImages(productGTIN, new ItemImagesWrapper(konimboItemImages));
                    }
                }
                
                Console.WriteLine($"konimbo item: #{konimboItemDetails.id}");
                return true;
            }
            catch (Exception ex) {
                Logger.WriteError("ProcessProduct", $"{ex.Message}");
                Console.WriteLine($"[ERROR] {ex.Message}");
                return false;
            }
        }
    }
}

