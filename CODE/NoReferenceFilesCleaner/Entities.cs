
namespace App_Code
{
    public class Entities
    {
        public enum eExecutionMode : byte { REPORT, TEST, LIVE }
        public enum eResourceType : byte { NULL, VIDEO, BOOK }

        public abstract class ResourceData {
            public int Id { get; set; }
            public string FileName { get; set; }            

            public abstract eResourceType ResourceType { get; protected set; }
        }

        public class VideoResourceData : ResourceData {
            public override eResourceType ResourceType { get; protected set; } = eResourceType.VIDEO;
        }

        public class BookResourceData : ResourceData
        {
            public override eResourceType ResourceType { get; protected set; } = eResourceType.BOOK;
        }
    }
}
