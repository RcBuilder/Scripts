using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// > Install-Package TLSharp
using TLSharp.Core;
using TLSharp.Core.Utils;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Account;
using TeleSharp.TL.Photos;
using TeleSharp.TL.Messages;
using TeleSharp.TL.Updates;
using TeleSharp.TL.Contacts;


// TODO ->> QA
namespace Telegram
{
    /*
        USING
        -----
        var telegramClient = new TelegramUserAPI(8611873, "xxxxxxxxxxxxxxxxxxx", "054-5614020");
        await telegramClient.Connect();

        -

        // authorize bot - only needed once 
        if(!telegramClient.IsAuthorized){
            var hash = await telegramClient.SendAuthorizationCode();
            var code = Console.ReadLine();  // provide the code sent to your phone
            var success = await telegramClient.Authorize(hash, code);
        }

        -

        var contacts = await telegramClient.GetContacts();


    */

    // TODO ->> Cache Mechanism to reduce api calls, use flag to clear 
    public class TelegramUserAPI : ITelegramUserAPI, IDisposable
    {
        private TelegramClient Client { get; set; } = null;

        private int ApiId { get; set; }
        private string ApiHash { get; set; }
        private string Phone { get; set; }        

        public bool IsConnected {
            get { return this.Client != null && this.Client.IsConnected; }
        }

        public bool IsAuthorized {
            get { return this.Client != null && this.Client.IsUserAuthorized(); }
        }

        public TelegramUserAPI(int ApiId, string ApiHash, string Phone)
        {
            this.ApiId = ApiId;
            this.ApiHash = ApiHash;
            this.Phone = this.FixPhone(Phone);
        }

        public async Task Connect()
        {
            if(this.Client == null || !this.Client.IsConnected)
                this.Client = new TelegramClient(this.ApiId, this.ApiHash);            
            await this.Client.ConnectAsync();
        }

        // authorize bot - only needed once 
        public async Task<string> SendAuthorizationCode() {
            return await this.Client.SendCodeRequestAsync(this.Phone);
        }

        public async Task<bool> Authorize(string AuthorizationHash, string AuthorizationCode) {            
            var user = await this.Client.MakeAuthAsync(this.Phone, AuthorizationHash, AuthorizationCode);
            return user != null;            
        }

        public async Task<SearchResult> Search(string Query, bool IncludeLocals = true)
        {            
            var found = await this.Client.SearchUserAsync($"@{Query}");
            var result = (SearchResult)found;

            if (IncludeLocals) {
                result.LocalContacts = await this.GetContactsByQuery(Query);
                result.LocalGroups = await this.GetGroupsByQuery(Query);
                result.LocalChannels = await this.GetChannelsByQuery(Query);

                /*
                result.LocalContacts = await this.GetContactsByIds(result.LocalResults.Where(x => x.PeerType == ePeerType.User).Select(x => x.PeerId).ToList());
                result.LocalGroups = await this.GetGroupsByIds(result.LocalResults.Where(x => x.PeerType == ePeerType.Group).Select(x => x.PeerId).ToList());
                result.LocalChannels = await this.GetChannelsByIds(result.LocalResults.Where(x => x.PeerType == ePeerType.Channel).Select(x => x.PeerId).ToList());
                */
            }

            return result;
        }

        // TODO ->> ClearCache
        public async Task<IEnumerable<Contact>> GetContacts(bool ClearCache = false) {
            var response = await this.Client.GetContactsAsync();
            var contacts = response.Users?.Where(x => x is TLUser).Select(x => (Contact)x) ?? new List<Contact>();
            return contacts;
        }

        // TODO ->> ClearCache
        public async Task<IEnumerable<Channel>> GetChannels(bool ClearCache = false, bool Extended = false)
        {
            var dialogChats = await this.GetDialogs(ClearCache);
            var channels = dialogChats.Where(x => x is TLChannel).Select(x => (Channel)x) ?? new List<Channel>();

            if (Extended)
                foreach (var channel in channels)
                    channel.Extended = await this.GetChannelExtended(channel);

            return channels;
        }

        // TODO ->> ClearCache
        public async Task<IEnumerable<Group>> GetGroups(bool ClearCache = false, bool Extended = false)
        {
            var dialogChats = await this.GetDialogs(ClearCache);
            var groups = dialogChats.Where(x => x is TLChat).Select(x => (Group)x) ?? new List<Group>();

            if(Extended)
                foreach(var group in groups)
                    group.Extended = await this.GetGroupExtended(group.Id);

            return groups;
        }

        public async Task<IEnumerable<Contact>> GetTopContacts(int Limit = 5) {
            var response = await this.Client.SendRequestAsync<TLTopPeers>(new TLRequestGetTopPeers { 
                Limit = Limit
            });
            
            var contacts = response.Users?.Where(x => x is TLUser).Select(x => (Contact)x) ?? new List<Contact>();
            return contacts;
        }

        public async Task<IEnumerable<Channel>> GetTopChannels(int Limit = 5) {
            var response = await this.Client.SendRequestAsync<TLTopPeers>(new TLRequestGetTopPeers
            {
                Limit = Limit
            });

            var channels = response.Chats?.Where(x => x is TLChannel).Select(x => (Channel)x) ?? new List<Channel>();
            return channels;
        }

        public async Task<IEnumerable<Group>> GetTopGroups(int Limit = 5) {
            var response = await this.Client.SendRequestAsync<TLTopPeers>(new TLRequestGetTopPeers
            {
                Limit = Limit
            });

            var groups = response.Chats?.Where(x => x is TLChat).Select(x => (Group)x) ?? new List<Group>();
            return groups;
        }

        public async Task<Contact> GetContact(int Id) {
            var contacts = await this.GetContacts();            
            return contacts.FirstOrDefault(x => x.Id == Id);
        }

        public async Task<Contact> GetContactByPhone(string Phone) {
            var contacts = await this.GetContacts();
            // note that the api return the phones without the '+' prefix 
            return contacts.FirstOrDefault(x => $"+{x.Phone}" == this.FixPhone(Phone));
        }

        public async Task<IEnumerable<Contact>> GetContactsByName(string Name) {
            var contacts = await this.GetContacts();
            return contacts.Where(x => string.Equals(x.FirstName, Name, StringComparison.OrdinalIgnoreCase) || string.Equals(x.LastName, Name, StringComparison.OrdinalIgnoreCase)); 
        }

        public async Task<IEnumerable<Contact>> GetContactsByIds(IEnumerable<int> Ids) {
            var contacts = await this.GetContacts();
            return contacts.Where(x => Ids.Contains(x.Id));
        }

        public async Task<IEnumerable<Contact>> GetContactsByQuery(string Query)
        {            
            var contacts = await this.GetContacts();
            Query = Query.ToLower();
            return contacts.Where(x => x.FullName.ToLower().Contains(Query));
        }

        public async Task<Group> GetGroup(int Id, bool Extended = false) {
            var groups = await this.GetGroups();
            var group = groups.FirstOrDefault(x => x.Id == Id);

            if (Extended && group.Extended == null)
                group.Extended = await this.GetGroupExtended(group.Id);

            return group;
        }

        public async Task<Group> GetGroupByTitle(string Title)
        {
            var groups = await this.GetGroups();            
            return groups.FirstOrDefault(x => string.Equals(x.Title, Title, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Group>> GetGroupsByIds(IEnumerable<int> Ids) {
            var groups = await this.GetGroups();
            return groups.Where(x => Ids.Contains(x.Id));
        }

        public async Task<IEnumerable<Group>> GetGroupsByQuery(string Query)
        {
            var groups = await this.GetGroups();
            Query = Query.ToLower();
            return groups.Where(x => x.Title.ToLower().Contains(Query));
        }

        public async Task<Channel> GetChannel(int Id, bool Extended = false) {
            var channels = await this.GetChannels();
            var channel = channels.FirstOrDefault(x => x.Id == Id);

            if (Extended && channel.Extended == null)
                channel.Extended = await this.GetChannelExtended(channel);

            return channel;
        }

        public async Task<Channel> GetChannelByTitle(string Title)
        {
            var channels = await this.GetChannels();
            return channels.FirstOrDefault(x => string.Equals(x.Title, Title, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Channel>> GetChannelsByIds(IEnumerable<int> Ids) {
            var channels = await this.GetChannels();
            return channels.Where(x => Ids.Contains(x.Id));
        }

        public async Task<IEnumerable<Channel>> GetChannelsByQuery(string Query)
        {
            var channels = await this.GetChannels();
            Query = Query.ToLower();
            return channels.Where(x => x.Title.ToLower().Contains(Query));
        }

        public async Task<bool> SendMessageToMe(string Message)
        {
            return await this.SendMessage(0, Message, ePeerType.Self);
        }

        public async Task<bool> SendMessageToUser(int Id, string Message) {
            return await this.SendMessage(Id, Message, ePeerType.User);
        }

        public async Task<bool> SendMessageToGroup(int Id, string Message)
        {
            return await this.SendMessage(Id, Message, ePeerType.Group);
        }

        public async Task<bool> SendMessageToChannel(int Id, string Message)
        {
            return await this.SendMessage(Id, Message, ePeerType.Channel);
        }

        public async Task<bool> SendPhotoToMe(IPhotoItem Photo)
        {
            return await this.SendMedia(0, Photo, ePeerType.Self);
        }

        public async Task<bool> SendPhotoToUser(int Id, IPhotoItem Photo)
        {
            return await this.SendMedia(Id, Photo, ePeerType.User);
        }

        public async Task<bool> SendPhotoToGroup(int Id, IPhotoItem Photo)
        {
            return await this.SendMedia(Id, Photo, ePeerType.Group);
        }

        public async Task<bool> SendPhotoToChannel(int Id, IPhotoItem Photo)
        {
            return await this.SendMedia(Id, Photo, ePeerType.Channel);
        }

        public async Task<IEnumerable<Message>> GetUserHistory(int Id, int Limit = 100)
        {
            return await this.GetHistory(Id, Limit, ePeerType.User);
        }

        public async Task<IEnumerable<Message>> GetGroupHistory(int Id, int Limit = 100)
        {
            return await this.GetHistory(Id, Limit, ePeerType.Group);
        }

        public async Task<IEnumerable<Message>> GetChannelHistory(int Id, int Limit = 100)
        {
            return await this.GetHistory(Id, Limit, ePeerType.Channel);
        }

        public async Task<IEnumerable<Message>> GetMyHistory(int Limit = 100)
        {
            return await this.GetHistory(0, Limit, ePeerType.Self);
        }
        
        public async Task<AccountState> GetAccountState()
        {
            var state = await this.Client.SendRequestAsync<TLState>(new TLRequestGetState());
            return (AccountState)state;
        }

        public async Task<ContactsState> GetContactsState()
        {
            var state = await this.Client.SendRequestAsync<IEnumerable<TLContactStatus>>(new TLRequestGetStatuses());
            return (ContactsState)state.ToList();
        }
        
        public async Task<bool> InviteToChannel(Channel Channel, IEnumerable<Contact> UsersToInvite) {                       
            var response = await this.Client.SendRequestAsync<TLUpdates>(new TLRequestInviteToChannel
            {
                Channel = (TLInputChannel)Channel,
                Users = new TLVector<TLAbsInputUser>(UsersToInvite.Select(x => (TLInputUser)x))
            });
            return true;
        }        

        public async Task<bool> JoinAChannel(Channel Channel) {            
            var response = await this.Client.SendRequestAsync<TLUpdates>(new TLRequestJoinChannel
            {
                Channel = (TLInputChannel)Channel                
            });
            return true;
        }

        public async Task<bool> AddToGroup(int GroupId, Contact Contact)
        {
            await this.Client.SendRequestAsync<TLUpdates>(new TLRequestAddChatUser
            {
                ChatId = GroupId,
                UserId = (TLInputUser)Contact
            });

            return true;
        }

        public async Task<FileResult> UploadFile(string FileName, string FilePath) {
            var fileResult = (FileResult)(await this.Client.UploadFile(FileName, new StreamReader(FilePath)));
            return fileResult;
        }

        public async Task<IEnumerable<long>> GetUploadedFiles(bool ClearCache = false) {
            // TODO ->> GetUploadedFiles
            return null;
        }

        public async Task<bool> PinContactMessage(int MessageId, int ContactId) {
            return await this.UpdateMessagePinState(MessageId, true, ContactId, ePeerType.User);
        }

        public async Task<bool> PinGroupMessage(int MessageId, int GroupId) {
            return await this.UpdateMessagePinState(MessageId, true, GroupId, ePeerType.Group);
        }

        public async Task<bool> PinChannelMessage(int MessageId, int ChannelId) {
            return await this.UpdateMessagePinState(MessageId, true, ChannelId, ePeerType.Channel);
        }

        public async Task<bool> UnPinContactMessage(int MessageId, int ContactId) {
            return await this.UpdateMessagePinState(MessageId, false, ContactId, ePeerType.User);
        }

        public async Task<bool> UnPinGroupMessage(int MessageId, int GroupId) {
            return await this.UpdateMessagePinState(MessageId, false, GroupId, ePeerType.Group);
        }

        public async Task<bool> UnPinChannelMessage(int MessageId, int ChannelId) {
            return await this.UpdateMessagePinState(MessageId, false, ChannelId, ePeerType.Channel);
        }
        
        // TODO ->> ClearCache
        public async Task<IEnumerable<Contact>> GetBlockedContacts(bool ClearCache = false)
        {
            var response = await this.Client.SendRequestAsync<TLBlocked>(new TLRequestGetBlocked { 
                
            });

            var contacts = response.Users?.Where(x => x is TLUser).Select(x => (Contact)x) ?? new List<Contact>();
            return contacts;
        }

        public async Task<bool> BlockContact(Contact Contact) {
            return await this.Client.SendRequestAsync<bool>(new TLRequestBlock
            {
                Id = (TLInputUser)Contact
            });            
        }

        public async Task<bool> UnBlockContact(Contact Contact) {
            return await this.Client.SendRequestAsync<bool>(new TLRequestUnblock
            {               
                Id = (TLInputUser)Contact
            });
        }

        public async Task<bool> CreateChannel(Channel Channel) {
            await this.Client.SendRequestAsync<TLUpdates>(new TLRequestCreateChannel { 
                Megagroup = false,
                Broadcast = true,
                Title = Channel.Title,
                About = Channel.About
            });

            return true;
        }

        public async Task<bool> CreateGroup(Group Group)
        {
            await this.Client.SendRequestAsync<TLUpdates>(new TLRequestCreateChat
            {
                Title = Group.Title
            });

            return true;
        }

        public async Task<bool> CreateContact(Contact Contact) {
            var response = await this.Client.SendRequestAsync<TLImportedContacts>(new TLRequestImportContacts
            {
                Contacts = new TLVector<TLInputPhoneContact> { 
                    (TLInputPhoneContact)Contact 
                }
            });

            return response.Imported.Count == 0;
        }

        public async Task<bool> UpdateChannelTitle(Channel Channel) {
            // update title
            await this.Client.SendRequestAsync<TLUpdates>(new TLRequestEditTitle { 
                Channel = (TLInputChannel)Channel,
                Title = Channel.Title
            });

            return true;
        }

        public async Task<bool> UpdateChannelPhoto(Channel Channel, string PhotoPath)
        {
            // update profile photo
            var file = await this.UploadFile("", PhotoPath);
            await this.Client.SendRequestAsync<TLUpdates>(new TLRequestEditPhoto
            {
                Channel = (TLInputChannel)Channel,
                Photo = new TLInputChatUploadedPhoto {
                    File = (TLInputFile)file
                }
            });

            return true;
        }

        public async Task<bool> UpdateGroupTitle(Group Group) {
            // update title
            await this.Client.SendRequestAsync<TLUpdates>(new TLRequestEditChatTitle
            {
                ChatId = Group.Id,
                Title = Group.Title                
            });

            return true;
        }

        public async Task<bool> UpdateGroupPhoto(Group Group, string PhotoPath)
        {
            // update profile photo
            var file = await this.UploadFile("", PhotoPath);
            await this.Client.SendRequestAsync<TLUpdates>(new TLRequestEditChatPhoto
            {
                ChatId = Group.Id,                
                Photo = new TLInputChatUploadedPhoto {
                    File = (TLInputFile)file
                }
            });

            return true;
        }

        public async Task<bool> UpdateMyProfile(Me Me)
        {         
            // update details
            await this.Client.SendRequestAsync<TLUser>(new TLRequestUpdateProfile
            {
                FirstName = Me.FirstName,
                LastName = Me.LastName,
                About = Me.About                
            });

            return true;
        }

        public async Task<bool> UpdateMyProfilePhoto(string PhotoPath)
        {
            // update profile photo
            var file = await this.UploadFile("", PhotoPath);
            await this.Client.SendRequestAsync<TeleSharp.TL.Photos.TLPhoto>(new TLRequestUploadProfilePhoto
            {                
                File = (TLInputFile)file
            });

            return true;
        }

        public async Task<bool> UpdateMyStatus(bool Online) {
            return await this.Client.SendRequestAsync<bool>(new TLRequestUpdateStatus
            {
                Offline = !Online                
            });
        }

        public void Dispose()
        {
            if (this.Client != null)
                this.Client.Dispose();
        }

        // ----

        private string FixPhone(string Phone) {            
            Phone = Phone.Replace("-", "").Replace(" ", "").Replace("_", "").Trim();
            if (Phone.StartsWith("0")) Phone = $"+972{Phone.Substring(1)}";
            if (!Phone.StartsWith("+")) Phone = $"+{Phone}";
            return Phone;
        }

        private async Task<bool> SendMessage(int Id, string Message, ePeerType PeerType = ePeerType.User) {
            var peer = this.GetPeer(Id, PeerType);
            await this.Client.SendMessageAsync(peer, Message);
            return true;
        }

        private async Task<IEnumerable<Message>> GetHistory(int Id, int Limit = 100, ePeerType PeerType = ePeerType.User) {         
            var historyAbs = await this.Client.SendRequestAsync<TLAbsMessages>(new TLRequestGetHistory
            {
                Peer = this.GetPeer(Id, PeerType),
                Limit = Limit, 
            });

            var historyMessagesAbs = (historyAbs is TLMessages) ? (historyAbs as TLMessages).Messages : (historyAbs as TLMessagesSlice).Messages;
            var historyMessages = historyMessagesAbs.Where(x => x is TLMessage).Select(x => (Message)x) ?? new List<Message>();
            return historyMessages;
        }

        private async Task<IEnumerable<TLAbsChat>> GetDialogs(bool ClearCache = false) {            
            // get all dialogs (channels and groups)
            var dialogs = (TLDialogsSlice)await this.Client.GetUserDialogsAsync();
            return dialogs?.Chats?.ToList() ?? new List<TLAbsChat>();
        }
        
        private async Task<GroupExtended> GetGroupExtended(int Id) {

            // get extended information
            var groupInfo = await this.Client.SendRequestAsync<TeleSharp.TL.Messages.TLChatFull>(new TLRequestGetFullChat()
            {
                ChatId = Id
            });
            
            return (GroupExtended)groupInfo;
        }

        private async Task<ChannelExtended> GetChannelExtended(Channel Channel)
        {
            // get extended information
            var channelInfo = await this.Client.SendRequestAsync<TeleSharp.TL.Messages.TLChatFull>(new TLRequestGetFullChannel()
            {
                Channel = (TLInputChannel)Channel
            });

            return (ChannelExtended)channelInfo;
        }

        // TODO ->> Cast
        private async Task<bool> SendMedia(int Id, IMediaItem MediaItem, ePeerType PeerType = ePeerType.User) {
            TLAbsInputMedia media = new TLInputMediaEmpty();
            /// TODO ->> ??
            /// this.Client.SendUploadedDocument(<inputPeer>, <inputFile>, <title>, <mimeType>, <attributes>)
            /// this.Client.SendUploadedPhoto(<inputPeer>, <inputFile>, <title>)

            if (MediaItem is PhotoURL)
                media = (TLInputMediaPhotoExternal)MediaItem;               

            await this.Client.SendRequestAsync<TLUpdates>(new TLRequestSendMedia
            {
                Peer = this.GetPeer(Id, PeerType),
                Media = media
            });

            return true;
        }

        private TLAbsInputPeer GetPeer(int Id, ePeerType PeerType) {            
            switch (PeerType)
            {
                case ePeerType.Self: return new TLInputPeerSelf { };                                    
                case ePeerType.User: return new TLInputPeerUser { UserId = Id };                    
                case ePeerType.Group: return new TLInputPeerChat { ChatId = Id };                    
                case ePeerType.Channel: return new TLInputPeerChannel { ChannelId = Id };
                default:
                case ePeerType.NULL: return new TLInputPeerEmpty();
            }
        }

        private async Task<bool> UpdateMessagePinState(int MessageId, bool PinState, int PeerId, ePeerType PeerType = ePeerType.User)
        {
            await this.Client.SendRequestAsync<TLUpdates>(new TLRequestToggleDialogPin
            {
                MessageId = MessageId,
                Peer = this.GetPeer(PeerId, PeerType),
                Pinned = PinState               
            });
            return true;
        }
    }

    public class TelegramBotAPI { }
}
