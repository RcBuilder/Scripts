using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Telegram
{
    public interface ITelegramUserAPI
    {
        Task Connect();
        Task<string> SendAuthorizationCode();
        Task<bool> Authorize(string AuthorizationHash, string AuthorizationCode);

        Task<SearchResult> Search(string Query, bool IncludeLocals = true);        

        Task<IEnumerable<Contact>> GetContacts(bool ClearCache = false);
        Task<IEnumerable<Channel>> GetChannels(bool ClearCache = false, bool Extended = false);
        Task<IEnumerable<Group>> GetGroups(bool ClearCache = false, bool Extended = false);

        Task<IEnumerable<Contact>> GetTopContacts(int Limit = 5);
        Task<IEnumerable<Channel>> GetTopChannels(int Limit = 5);
        Task<IEnumerable<Group>> GetTopGroups(int Limit = 5);        

        Task<Contact> GetContact(int Id);
        Task<Contact> GetContactByPhone(string Phone);
        Task<IEnumerable<Contact>> GetContactsByName(string Name);
        Task<IEnumerable<Contact>> GetContactsByIds(IEnumerable<int> Ids);
        Task<IEnumerable<Contact>> GetContactsByQuery(string Query);

        Task<Group> GetGroup(int Id, bool Extended = false);
        Task<Group> GetGroupByTitle(string Title);
        Task<IEnumerable<Group>> GetGroupsByIds(IEnumerable<int> Ids);
        Task<IEnumerable<Group>> GetGroupsByQuery(string Query);

        Task<Channel> GetChannel(int Id, bool Extended = false);
        Task<Channel> GetChannelByTitle(string Title);
        Task<IEnumerable<Channel>> GetChannelsByIds(IEnumerable<int> Ids);
        Task<IEnumerable<Channel>> GetChannelsByQuery(string Query);

        Task<bool> SendMessageToMe(string Message);
        Task<bool> SendMessageToUser(int Id, string Message);
        Task<bool> SendMessageToGroup(int Id, string Message);
        Task<bool> SendMessageToChannel(int Id, string Message);

        Task<bool> SendPhotoToMe(IPhotoItem Photo);
        Task<bool> SendPhotoToUser(int Id, IPhotoItem Photo);
        Task<bool> SendPhotoToGroup(int Id, IPhotoItem Photo);
        Task<bool> SendPhotoToChannel(int Id, IPhotoItem Photo);

        // send photo
        // send video
        // send document
        // send file                

        Task<AccountState> GetAccountState();
        Task<ContactsState> GetContactsState();
        Task<bool> InviteToChannel(Channel Channel, IEnumerable<Contact> UsersToInvite);
        Task<bool> JoinAChannel(Channel Channel);
        Task<bool> AddToGroup(int GroupId, Contact Contact);

        Task<FileResult> UploadFile(string FileName, string FilePath);
        Task<IEnumerable<long>> GetUploadedFiles(bool ClearCache = false);

        Task<bool> PinContactMessage(int MessageId, int ContactId);
        Task<bool> PinGroupMessage(int MessageId, int GroupId);
        Task<bool> PinChannelMessage(int MessageId, int ChannelId);
        Task<bool> UnPinContactMessage(int MessageId, int ContactId);
        Task<bool> UnPinGroupMessage(int MessageId, int GroupId);
        Task<bool> UnPinChannelMessage(int MessageId, int ChannelId);

        // Task<bool> ClearContactPinnedMessage(int ContactId);
        // Task<bool> ClearGroupPinnedMessage(int GroupId);
        // Task<bool> ClearChannelPinnedMessage(int ChannelId);

        Task<IEnumerable<Contact>> GetBlockedContacts(bool ClearCache = false);
        Task<bool> BlockContact(Contact Contact);
        Task<bool> UnBlockContact(Contact Contact);        

        Task<bool> CreateChannel(Channel Channel);
        Task<bool> CreateGroup(Group Group);
        Task<bool> CreateContact(Contact Contact);

        Task<bool> UpdateChannelTitle(Channel Channel);
        Task<bool> UpdateChannelPhoto(Channel Channel, string PhotoPath);
        Task<bool> UpdateGroupTitle(Group Group);
        Task<bool> UpdateGroupPhoto(Group Group, string PhotoPath);

        Task<bool> UpdateMyProfile(Me Me);
        Task<bool> UpdateMyProfilePhoto(string PhotoPath);
        Task<bool> UpdateMyStatus(bool Online);

        Task<IEnumerable<Message>> GetMyHistory(int Limit = 100);
        Task<IEnumerable<Message>> GetUserHistory(int Id, int Limit = 100);
        Task<IEnumerable<Message>> GetGroupHistory(int Id, int Limit = 100);
        Task<IEnumerable<Message>> GetChannelHistory(int Id, int Limit = 100);
    }
}
