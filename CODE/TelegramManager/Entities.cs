using System;
using System.Collections.Generic;
using System.Linq;
using TeleSharp.TL;
using TeleSharp.TL.Contacts;
using TeleSharp.TL.Updates;

namespace Telegram
{
    public enum ePeerType : byte { NULL, Self, User, Group, Channel }

    public interface IMediaItem { }
    public interface IPhotoItem : IMediaItem { }
    public interface IVideoItem : IMediaItem { }
    public interface IDocumentItem : IMediaItem { }

    public class PhotoURL : IPhotoItem {        
        public string Caption { get; set; }
        public string URL { get; set; }
        
        public static explicit operator PhotoURL(TLInputMediaPhotoExternal Source) {
            return new PhotoURL {
                Caption = Source.Caption,
                URL = Source.Url                
            };
        }

        public override string ToString() {
            return $"{this.Caption})";
        }
    }
    public class PhotoFile : IPhotoItem { } // TODO ->> IPhotoItem

    public class VideoURL : IVideoItem { } // TODO ->> IVideoItem
    public class VideoFile : IVideoItem { } // TODO ->> IVideoItem

    public class DocumentURL : IDocumentItem { } // TODO ->> IDocumentItem
    public class DocumentFile : IDocumentItem { } // TODO ->> IDocumentItem

    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public long? AccessHash { get; set; }
        public ProfilePhoto Profile { get; set; }

        public string FullName {
            get {
                return $"{this.FirstName ?? ""} {this.LastName ?? ""}";
            }
        }

        public static explicit operator Contact(TLUser Source)
        {
            return new Contact {                
                Id = Source.Id,
                FirstName = Source.FirstName,
                LastName = Source.LastName,
                Phone = Source.Phone,
                AccessHash = Source.AccessHash,
                Profile = (ProfilePhoto)Source.Photo
            };
        }

        public static explicit operator TLInputUser(Contact Source)
        {
            return new TLInputUser
            {
                UserId = Source.Id,
                AccessHash = Source.AccessHash ?? 0
            };
        }

        public static explicit operator TLInputPhoneContact(Contact Source)
        {
            return new TLInputPhoneContact
            {
                ClientId = Source.Id,
                FirstName = Source.FirstName,
                LastName = Source.LastName,
                Phone = Source.Phone
            };
        }

        public override string ToString()
        {
            return $"{this.FirstName} {this.LastName} ({this.Phone})";
        }
    }

    public class Me : Contact { 
        public string About { get; set; }
    }

    public class ProfilePhoto {
        public static explicit operator ProfilePhoto(TLAbsUserProfilePhoto Source)
        {
            return new ProfilePhoto
            {
                // TODO ->> ProfilePhoto
            };
        }

        public static explicit operator ProfilePhoto(TLAbsChatPhoto Source)
        {
            return new ProfilePhoto
            {
                // TODO ->> ProfilePhoto
            };
        }
    }

    public class FileResult {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Parts { get; set; }
        public string Md5Checksum { get; set; }

        public static explicit operator FileResult(TLInputFile Source)
        {
            return new FileResult
            {
                Id = Source.Id,
                Name = Source.Name,
                Parts = Source.Parts,
                Md5Checksum = Source.Md5Checksum
            };
        }

        public static explicit operator TLInputFile(FileResult Source)
        {
            return new TLInputFile
            {
                Id = Source.Id,
                Name = Source.Name,
                Parts = Source.Parts,
                Md5Checksum = Source.Md5Checksum
            };
        }
    }

    public class Message
    {
        public int Id { get; set; }
        public int? FromId { get; set; }
        public string Body { get; set; }        
        public bool IsOutgoing { get; set; }  // outgoing or incoming message     
        public DateTime CreatedDateUTC { get; set; }
       
        public static explicit operator Message(TLMessage Source)
        {            
            return new Message
            {
                Id = Source.Id,
                FromId = Source.FromId,
                Body = Source.Message,
                IsOutgoing = Source.Out,
                CreatedDateUTC = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Source.Date)  // unixTime (seconds count since 1970-01-01)
            };
        }

        public override string ToString()
        {
            return $"{this.FromId}: {this.Body})";
        }
    }

    public class Channel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string About { get; set; }
        public string UserName { get; set; }
        public long? AccessHash { get; set; }
        public ProfilePhoto Profile { get; set; }
        public ChannelExtended Extended { get; set; }

        // TODO ->> complete
        public static explicit operator Channel(TLChannel Source)
        {
            return new Channel
            {
                Id = Source.Id,
                Title = Source.Title,
                AccessHash = Source.AccessHash,
                // About = Source.About,
                UserName = Source.Username,
                Profile = (ProfilePhoto)Source.Photo
            };
        }

        public static explicit operator TLInputChannel(Channel Source)
        {
            return new TLInputChannel
            {
                ChannelId = Source.Id, 
                AccessHash = Source.AccessHash ?? 0                
            };
        }

        public override string ToString()
        {
            return $"{this.Id}: {this.Title})";
        }
    }

    public class Group
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
        public ProfilePhoto Profile { get; set; }
        public GroupExtended Extended { get; set; }

        // TODO ->> complete
        public static explicit operator Group(TLChat Source)
        {
            return new Group
            {
                Id = Source.Id,
                Title = Source.Title,
                Count = Source.ParticipantsCount,
                Profile = (ProfilePhoto)Source.Photo
            };
        }

        public override string ToString()
        {
            return $"{this.Id}: {this.Title})";
        }
    }

    public class GroupExtended
    {        
        public IEnumerable<Contact> Users { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<Channel> Channels { get; set; }

        // TODO ->> complete - Source.FullChat
        public static explicit operator GroupExtended(TeleSharp.TL.Messages.TLChatFull Source)
        {            
            var fullChat = ((TLChatFull)Source.FullChat);
            return new GroupExtended
            {
                Users = Source.Users?.Where(x => x is TLUser).Select(x => (Contact)x) ?? new List<Contact>(),
                Groups = Source.Chats?.Where(x => x is TLChat).Select(x => (Group)x) ?? new List<Group>(),
                Channels = Source.Chats?.Where(x => x is TLChannel).Select(x => (Channel)x) ?? new List<Channel>()
            };
        }
    }

    public class ChannelExtended : GroupExtended { }

    public class AccountState { 
        public int Unread { get; set; }        
        
        public static explicit operator AccountState(TLState Source)
        {
            return new AccountState
            {
                Unread = Source.UnreadCount
            };
        }

        public override string ToString()
        {
            return $"{this.Unread} Unread Messages";
        }
    }

    public class ContactsState
    { 
        public int Online { get; set; }
        public int Offline { get; set; }
        public int SeenRecently { get; set; }
        public int SeenLastWeek { get; set; }
        public int SeenLastMonth { get; set; }

        public static explicit operator ContactsState(List<TLContactStatus> Source)
        {
            return new ContactsState
            {
                Online = Source.Count(x => x.Status is TLUserStatusOnline),
                Offline = Source.Count(x => x.Status is TLUserStatusOffline),
                SeenRecently = Source.Count(x => x.Status is TLUserStatusRecently),
                SeenLastWeek = Source.Count(x => x.Status is TLUserStatusLastWeek),
                SeenLastMonth = Source.Count(x => x.Status is TLUserStatusLastMonth)
            };
        }

        public override string ToString()
        {
            return $"{this.Online} Online | {this.Offline} Offline";
        }
    }

    public class SearchResult {
        public IEnumerable<(int PeerId, ePeerType PeerType)> Results { get; set; }

        public IEnumerable<Contact> Contacts { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<Channel> Channels { get; set; }

        public IEnumerable<Contact> LocalContacts { get; set; }
        public IEnumerable<Group> LocalGroups { get; set; }
        public IEnumerable<Channel> LocalChannels { get; set; }

        public static explicit operator SearchResult(TLFound Source)
        {
            return new SearchResult
            {
                Results = Source.Results?.Select(x => {
                    if (x is TLPeerUser) return (((TLPeerUser)x).UserId, ePeerType.User);
                    if (x is TLPeerChat) return (((TLPeerChat)x).ChatId, ePeerType.Group);
                    if (x is TLPeerChannel) return (((TLPeerChannel)x).ChannelId, ePeerType.Channel);
                    return (0, ePeerType.NULL);
                }) ?? new List<(int, ePeerType)>(),
                Contacts = Source.Users?.Where(x => x is TLUser).Select(x => (Contact)x) ?? new List<Contact>(),
                Groups = Source.Chats?.Where(x => x is TLChat).Select(x => (Group)x) ?? new List<Group>(),
                Channels = Source.Chats?.Where(x => x is TLChannel).Select(x => (Channel)x) ?? new List<Channel>()
            };
        }
    }
}
