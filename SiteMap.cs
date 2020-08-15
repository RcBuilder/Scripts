using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BLL
{
    /*  USING
        -----
        <system.webServer>
            <handlers>      
              <add name="siteMap" path="SiteMap.xml" verb="GET" type="BLL.HttpHandlers.SiteMap" preCondition="integratedMode,runtimeVersionv4.0" />
            </handlers> 
        </system.webServer>

        <system.web>
            <handlers>      
              <add path="SiteMap.xml" type="BLL.HttpHandlers.SiteMap" verb="GET" validate="false" />
            </handlers> 
        </system.web>        

        ---


        public class RouteConfig {
            public static void RegisterRoutes(RouteCollection routes) {
                routes.IgnoreRoute("SiteMap.xml");
            }
        }        

        ---

        namespace BLL.HttpHandlers
        {
            public class SiteMap : IHttpHandler
            {
                private readonly string filePath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "SiteMap.xml");

                public void ProcessRequest(HttpContext context)
                {
                    XDocument xDoc = null;
                    try
                    {
                        var sm = new BLL.SiteMap();
                        xDoc = sm.Generate();
                        if (xDoc == null)
                            throw new NullReferenceException("SiteMap Generator returns NULL");

                        // save updated xml as physical file
                        try
                        {
                            xDoc.Save(filePath);
                        }
                        catch { }
                    }
                    catch
                    { 
                        // sitemap is not generated properly - load physical copy 
                        xDoc = XDocument.Load(filePath);
                    }
                    finally {
                        context.Response.Clear();
                        context.Response.ClearContent();
                        context.Response.ClearHeaders();

                        var strXml = string.Concat(xDoc.Declaration.ToString() + Environment.NewLine, xDoc.ToString());
                        context.Response.ContentType = "text/xml";
                        context.Response.Write(strXml);
                        context.Response.Flush();
                        context.Response.End();
                    }
                }

                public bool IsReusable { get { return true; } }
            } 
        }
    */

    public class SiteMap
    {
        private XDocument xDoc;
        private string baseURL;
        private XNamespace ns;

        public SiteMap(){
            this.xDoc = new XDocument();
            this.baseURL = "http://openbook.co.il/"; // Config.SiteURL;
            this.ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        }

        public XDocument Generate()
        {
            try
            {                
                xDoc.Declaration = new XDeclaration("1.0", "UTF-8", "yes");
                var root = new XElement(ns + "urlset");
                xDoc.Add(root);

                HandleStaticContent();
                HandlePages();
                ///HandleLecturers();
                ///HandleStudents();                                                
                HandleCourses();
                HandleReviews();
                HandleOnlineCourses();
                HandleMarathons();
                HandleSearchEngine();                
            
                return xDoc;
            }
            catch(Exception ex)
            {                
                return null;
            }
        }

        #region HandleStaticContent:
        private void HandleStaticContent() {
            var root = this.xDoc.Root;

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", baseURL),
                    new XElement(ns + "priority", "1.0"),
                    new XElement(ns + "changefreq", "weekly")
                )
            );

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Account")),
                    new XElement(ns + "priority", "0.4"),
                    new XElement(ns + "changefreq", "weekly")
                )
            );

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Home/About")),
                    new XElement(ns + "priority", "0.8"),
                    new XElement(ns + "changefreq", "weekly")
                )
            );

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Home/Contact")),
                    new XElement(ns + "priority", "0.8"),
                    new XElement(ns + "changefreq", "weekly")
                )
            );
        }
        #endregion

        #region HandlePages:
        private void HandlePages() {
            var root = this.xDoc.Root;

            var pages = Pages.Get(false);
            foreach(var page in pages)
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "Page/", page.Name)),
                        new XElement(ns + "priority", "1.0"),
                        new XElement(ns + "changefreq", "daily")
                    )
                );
        }
        #endregion

        #region HandleLecturers:
        private void HandleLecturers() {
            var root = this.xDoc.Root;

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Lecturer/Register")),
                    new XElement(ns + "priority", "0.5"),
                    new XElement(ns + "changefreq", "weekly")
                )
            );

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Lecturer/Login")),
                    new XElement(ns + "priority", "0.5"),
                    new XElement(ns + "changefreq", "weekly")
                )
            );

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Search/Lecturers")),
                    new XElement(ns + "priority", "1.0"),
                    new XElement(ns + "changefreq", "daily")
                )
            );

            var lecturers = Lecturers.Get(true);
            foreach (var lecturer in lecturers)
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "Lecturer/", lecturer.Id)),
                        new XElement(ns + "priority", "1.0"),
                        new XElement(ns + "changefreq", "daily")
                    )
                );        
        }
        #endregion

        #region HandleStudents:
        private void HandleStudents() {
            var root = this.xDoc.Root;

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Student/Register")),
                    new XElement(ns + "priority", "0.5"),
                    new XElement(ns + "changefreq", "weekly")
                )
            );

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Student/Login")),
                    new XElement(ns + "priority", "0.5"),
                    new XElement(ns + "changefreq", "weekly")
                )
            );

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Search/Students")),
                    new XElement(ns + "priority", "1.0"),
                    new XElement(ns + "changefreq", "daily")
                )
            );

            var students = Students.Get(true);
            foreach (var student in students)
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "Student/", student.Id)),
                        new XElement(ns + "priority", "1.0"),
                        new XElement(ns + "changefreq", "daily")
                    )
                );        
        }
        #endregion

        #region HandleCourses:
        private void HandleCourses() {
            var root = this.xDoc.Root;

            var courses = Courses.Get();
            foreach (var course in courses)                
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "Course/", course.Id)),
                        new XElement(ns + "priority", "0.8"),
                        new XElement(ns + "changefreq", "daily")
                    )
                );   
        }
        #endregion

        #region HandleReviews:
        private void HandleReviews() {
            var root = this.xDoc.Root;

            var courses = Courses.Get();
            foreach (var course in courses.Where(x => x.Reviews.Count > 0))                
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "Review/Course/", course.Id)),
                        new XElement(ns + "priority", "0.5"),
                        new XElement(ns + "changefreq", "daily")
                    )
                );

            var lecturers = Lecturers.Get(true);
            foreach (var lecturer in lecturers.Where(x => x.Reviews.Count > 0))
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "Review/Lecturer/", lecturer.Id)),
                        new XElement(ns + "priority", "0.5"),
                        new XElement(ns + "changefreq", "daily")
                    )
                );   
        }
        #endregion

        #region HandleSearchEngine:
        private void HandleSearchEngine() {
            var root = this.xDoc.Root;

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Search")),
                    new XElement(ns + "priority", "0.1"),
                    new XElement(ns + "changefreq", "daily")
                )
            );

            var categories = Categories.Get();
            foreach (var category in categories)                
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "Search?CategoryId=", category.Id)),
                        new XElement(ns + "priority", "0.7"),
                        new XElement(ns + "changefreq", "daily")
                    )
                );

            var cities = Cities.Get();
            foreach (var city in cities)
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "Search?CategoryId=", city.Id)),
                        new XElement(ns + "priority", "0.7"),
                        new XElement(ns + "changefreq", "daily")
                    )
                );
        }
        #endregion

        #region HandleMarathons:
        private void HandleMarathons() {
            var root = this.xDoc.Root;

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Search/Marathons")),
                    new XElement(ns + "priority", "1.0"),
                    new XElement(ns + "changefreq", "weekly")
                )
            );   

            var marathons = Marathons.Get(false);
            foreach (var marathon in marathons)
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "Marathon/", marathon.Id)),
                        new XElement(ns + "priority", "1.0"),
                        new XElement(ns + "changefreq", "weekly")
                    )
                );   
        }
        #endregion

        #region HandleOnlineCourses:
        private void HandleOnlineCourses()
        {
            var root = this.xDoc.Root;

            root.Add(
                    new XElement(ns + "url",
                    new XElement(ns + "loc", string.Concat(baseURL, "Search/OnlineCourses")),
                    new XElement(ns + "priority", "1.0"),
                    new XElement(ns + "changefreq", "daily")
                )
            );

            var onlineCourses = OnlineCourses.Get();
            foreach (var onlineCourse in onlineCourses)
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "OnlineCourse/", onlineCourse.Id)),
                        new XElement(ns + "priority", "1.0"),
                        new XElement(ns + "changefreq", "daily")
                    )
                );

            var onlineChapters = OnlineCourses.GetChapters();
            foreach (var onlineChapter in onlineChapters)
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "OnlineCourse/Chapter/", onlineChapter.Id)),
                        new XElement(ns + "priority", "1.0"),
                        new XElement(ns + "changefreq", "daily")
                    )
                );

            var onlineMovies = OnlineCourses.GetMovies();
            foreach (var onlineMovie in onlineMovies)
                root.Add(
                        new XElement(ns + "url",
                        new XElement(ns + "loc", string.Concat(baseURL, "OnlineCourse/Movie/", onlineMovie.Id)),
                        new XElement(ns + "priority", "1.0"),
                        new XElement(ns + "changefreq", "daily")
                    )
                );
        }
        #endregion
    }
}
