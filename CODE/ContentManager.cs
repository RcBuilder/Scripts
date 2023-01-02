using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLL
{
    /*        
	    CREATE TABLE [dbo].[Contents] (
	        [RowId] [int] IDENTITY(1,1) NOT NULL,
	        [Key] [varchar](50) NOT NULL,
	        [HE] [nvarchar](max) NOT NULL,
	        [EN] [nvarchar](max) NOT NULL,
	        [UpdatedDate] [datetime] NOT NULL,
	        PRIMARY KEY CLUSTERED ([RowId] ASC) ON [PRIMARY]
        ) 
        ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	    ALTER TABLE [dbo].[Contents] ADD  DEFAULT ('') FOR [HE]	    
	    ALTER TABLE [dbo].[Contents] ADD  DEFAULT ('') FOR [EN]	    
	    ALTER TABLE [dbo].[Contents] ADD  DEFAULT (getdate()) FOR [UpdatedDate]	

	    -

	    ALTER PROCEDURE [dbo].[sp_Contents_Get]
			@sIds VARCHAR(MAX)
		AS
		BEGIN	
			SET NOCOUNT ON;    
			
			IF(@sIds = '') -- ALL --
			BEGIN
				SELECT	*
				FROM	[dbo].[Contents] WITH(NOLOCK)
			END 
			ELSE -- By Ids filter --
			BEGIN
				;WITH CTE_Ids (Id) AS (		
					SELECT * FROM STRING_SPLIT(@sIds, ',')
				)		
				SELECT	*
				FROM	[dbo].[Contents] WITH(NOLOCK)		
				WHERE	RowId IN (SELECT Id FROM CTE_Ids)
			END 
		END

        -----

        public class ContentsDAL : IContentsDAL {
            protected string ConnStr { get; set; }
            public ContentsDAL(string ConnStr) {
                this.ConnStr = ConnStr;
            }

            public async Task<IEnumerable<Content>> Get() {
                return await this.Get(null);
            }

            public async Task<IEnumerable<Content>> Get(IEnumerable<int> Ids) {
                using (var conn = new SqlConnection(ConnStr)) {
                    SqlMapper.GridReader reader = await conn.QueryMultipleAsync(
                        "sp_Contents_Get",
                        commandType: CommandType.StoredProcedure,
                        param: new { sIds = string.Join(",", Ids ?? Enumerable.Empty<int>()) }
                    );
                    return reader.Read<Content>();
                }
            }
        }

        -----

        LOAD:
        public class Global : HttpApplication {
            void Application_Start(object sender, EventArgs e) {                                
                BLL.ContentManager.LoadContents();
            }
        }

        USING:
        @BLL.ContentManager.Translate("<KEY>")
    */

    public class ContentManager
    {
        public static Dictionary<string, Entities.Content> SiteContentMap = null;

        public static void LoadContents()
        {
            var contents = new ContentBLL().Get().Result;
            SiteContentMap = contents.ToDictionary(c => c.Key, c => c);
        }

        public static string Translate(string Key)
        {
            if (!SiteContentMap.ContainsKey(Key)) return "[" + Key + "]";
            var content = SiteContentMap[Key];
            string lang = CookiesManager.GetSelectedLang(HttpContext.Current);
            switch (lang)
            {
                default:
                case "he":
                    return content.HE;
                case "en":
                    return content.EN;
            }
        }
    }
}
