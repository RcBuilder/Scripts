using CliClap.Core.Common;
using CliClap.Core.Data;
using CliClap.Library.Helpers;
using CliClap.Crawler;
using CliClap.Crawler.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliClap.Library.Logging;
using System.IO;

namespace CliClap.Core.BusinessLogicLayer
{
    public class CrawlerBLL
    {
        public bool WidgetAutoGenerationProcess(string UserId, List<string> URLs)
        {
            var brandSettingsBLL = new BrandSettingsBLL();
            var widgetsBLL = new WidgetsBLL();
            var contentsBLL = new ContentsBLL();
            var tagsBLL = new TagsBLL();
            var predictiveContentBLL = new PredictiveContentBLL();
            var dropZoneBLL = new DropZoneBLL();

            var user = new UserDataSource().FindAspNetUserById(UserId);
            var brandSettings = brandSettingsBLL.GetBrandSettingsByUser(user).FirstOrDefault();
            var processId = widgetsBLL.CreateCrawlHistory(user.Id, brandSettings.Id);
            
            var numOfValidWidgets = 0;
            var numOfWidgets = 0;
            var DISPLAY_WIDGETS_COUNT = 10;
            var MIN_WORDS_COUNT = 100;

            try
            {
                var contentPatterns = predictiveContentBLL.GetByBrand(brandSettings.Id);

                var pagesFilters = new List<ICrawlerFilter>();
                foreach (var contentPattern in contentPatterns.Where(x => x.IsScannable))
                    pagesFilters.Add(new CrawlerWildcardFilter(contentPattern.FullPattern));

                var pages = new List<CrawlerPage>();
                var pagesBot = new AbotProvider();

                var pagesTasks = new List<Task<List<CrawlerPage>>>();
                foreach(var URL in URLs.Where(x => !string.IsNullOrWhiteSpace(x)))
                    pagesTasks.Add(pagesBot.CollectLinks(URL, pagesFilters));                
                Task.WaitAll(pagesTasks.ToArray());
                
                foreach (var task in pagesTasks)
                    pages = pages.Union(task.Result).ToList();

                var filtersInclude = new List<ICrawlerFilter>();
                filtersInclude.Add(new CrawlerVimeoVideoFilter());
                filtersInclude.Add(new CrawlerWistiaVideoFilter());
                filtersInclude.Add(new CrawlerYoutubeVideoFilter());
                filtersInclude.Add(new CrawlerPDFFilter());

                var filtersExclude = new List<ICrawlerFilter>();
                filtersExclude.Add(new CrawlerImageFilter());
                filtersExclude.Add(new CrawlerSocialMediaFilter());

                foreach (var contentPattern in contentPatterns)
                    filtersInclude.Add(new CrawlerWildcardFilter(contentPattern.FullPattern));

                var links = new List<CrawlerPage>();
                var linksBot = new RbotProvider();

                var linksTasks = new List<Task<List<CrawlerPage>>>();
                foreach (var page in pages)
                    linksTasks.Add(linksBot.CollectLinks(page.URL, filtersInclude, filtersExclude));
                Task.WaitAll(linksTasks.ToArray());

                foreach (var task in linksTasks)
                    links = links.Union(task.Result, new CrawlerPageComparer()).ToList();
                
                widgetsBLL.UpdateCrawlHistoryCounters(processId, links?.Count ?? 0, 0);
                
                var contentIdsHashSet = new HashSet<string>(widgetsBLL.GetContentIds(UserId));                

                Action<CrawlerPage> actionAddWidget = item =>
                {
                    try
                    {

                        // deduplicaion
                        var hashItemContentId = UrlHelper.GenerateUrlId(UrlHelper.GetManipulatedUrl(item.URL, AppSettings.CaseSensitiveDomains));
                        if (contentIdsHashSet.Contains(hashItemContentId)) return;

                        var tags = tagsBLL.GetSuggestedTagsByUrlOrDefault(user.UserName, item.URL);
                        var tagIds = tags == null ? null : tags.Select(t => t.Id).ToList();

                        // execute "preview" function
                        var contentTask = contentsBLL.GetContentEntity(item.URL);
                        contentTask.Wait();
                        var content = contentTask.Result;

                        if (string.IsNullOrWhiteSpace(content.Title))
                            content.Title = Path.GetFileName(new Uri(item.URL).LocalPath);

                        // crop main image                    
                        var imageToCrop = content.ImagesUrls.FirstOrDefault();
                        var croppedImage = ImageHelper.SmartCrop(imageToCrop, 150, 150);

                        var croppedImageURL = string.Empty;
                        if (croppedImage != null)
                            croppedImageURL = brandSettingsBLL.UploadImage(croppedImage, user.UserName);

                        /*
                            build MetaData as JsonStr: 

                            [7] CONTENT                   
                            { content_text, content_image_url, content_url, cta_text, ImagesUrls: [], widget_title }                    

                            [8] VIDEO                    
                            { video_text, video_image_url, video_url, cta_text, ImagesUrls: [], video_type, widget_title }                    
                        */
                        var MetaData = string.Empty;  //  JSON PER TYPE (See DB structure)  

                        if (item.ContentType == eCrawlerContentType.VIDEO)
                            MetaData = JsonConvert.SerializeObject(new
                            {
                                video_text = content.Description,
                                video_image_url = croppedImageURL,
                                video_url = item.URL,
                                cta_text = "Watch Now",
                                content.ImagesUrls,
                                video_type = content.ContentType,
                                widget_title = content.Title
                            });
                        else if (item.ContentType == eCrawlerContentType.CONTENT)
                        {
                            MetaData = JsonConvert.SerializeObject(new
                            {
                                content_text = content.Description,
                                content_image_url = croppedImageURL,
                                content_url = item.URL,
                                cta_text = "Read More",
                                content.ImagesUrls,
                                widget_title = content.Title
                            });
                        }

                        var IsValid = (croppedImage != null && !string.IsNullOrEmpty(content.Title) && content.WordsCount > MIN_WORDS_COUNT); // content has image and valid title
                        numOfValidWidgets += IsValid ? 1 : 0;
                        var IsDraft = true; // !IsValid || numOfValidWidgets < DISPLAY_WIDGETS_COUNT; 
                        if (IsValid && numOfValidWidgets < DISPLAY_WIDGETS_COUNT) // only present the top 10 valid widgets                      
                            IsDraft = false;                        

                        if (IsValid) {
                            var task = dropZoneBLL.AddPagesToURLMap(item.URL, brandSettings.WebstieScriptKey);
                            task.Wait();
                        }

                        long id = 0;
                        try {
                            id = widgetsBLL.Add(content.Title, MetaData, item.ContentType == eCrawlerContentType.VIDEO ? 8 : 7, true, user.UserName, tagIds, item.URL, null, IsDraft, IsValid);
                        }
                        catch (Exception ex) {
                            Log4NetTraceHelper.Error(typeof(CrawlerBLL), "Failed to generate a widget, error: ", ex);
                        }

                        if (id > 0) numOfWidgets++;
                        widgetsBLL.UpdateCrawlHistoryCounters(processId, null, numOfWidgets);

                    }
                    catch(Exception ex) {
                        Log4NetTraceHelper.Error(typeof(CrawlerBLL), $"generic exception, url: '{item.URL}', error: ", ex);
                    }
                };

                links.AsParallel().ForAll(actionAddWidget);  // Create Widget                        

                widgetsBLL.SaveCrawlHistory(processId, DateTime.UtcNow, true, string.Empty, null, numOfWidgets);
                return true;
            }
            catch (Exception ex)
            {
                widgetsBLL.SaveCrawlHistory(processId, DateTime.UtcNow, false, ex.Message, null, numOfWidgets);
                return false;
            }
        }
    }
}
