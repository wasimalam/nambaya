using System;
using System.Collections.Generic;

namespace FileSharing.Wrapper.IDgard
{
    internal class LoginResponse
    {
        public int statusCode { get; set; }
        public string statusMsg { get; set; }
        public string serverToken { get; set; }
        public string sessionId { get; set; }
        public int? uid { get; set; }
    }

    internal class LoginPayload
    {
        public string username { get; set; }
        public string password { get; set; }
        public string clientToken { get; set; }
    }

    internal class LoginRequest
    {
        public LoginPayload payload { get; set; }
    }

    internal class BodyRequest
    {
        public string serverToken { get; set; }
        public string clientToken { get; set; }
    }

    internal class GeneralResponse
    {
        public string id { get; set; }
        public string data { get; set; }

    }

    internal class DhownloadNodePayload
    {
        public string serverToken { get; set; }
        public string downloadKey { get; set; }
    }

    internal class UserBox
    {
        public List<BoxFolder> folders { get; set; }
        public List<BoxFile> files { get; set; }
        public List<BoxNote> notes { get; set; } //child notes
        public string nodeid { get; set; }
        public string name { get; set; }
        public string creator { get; set; }
        public string pathId { get; set; }
        public bool own { get; set; }
        public long ts { get; set; }
        public long d { get; set; }
        public string desc { get; set; }
    }

    internal class BoxPermission
    {
        public bool fileR { get; set; }//can read/download file/notes
        public bool fileW { get; set; }//can write (create folder, add note, upload files
        public bool fileD { get; set; }//can delete files/notes/folder of others
        public bool chatRW { get; set; }//can read & write chats
        public bool userR { get; set; }//can view member list of this box
        public bool journalR { get; set; }//can view journal if this box is dataroom
    }

    internal class BoxSummary
    {
        public string id { get; set; }
        public string name { get; set; }
        public int owner { get; set; }
        public string ownerName { get; set; }
        public string joinName { get; set; }
        public int contingent { get; set; }
        public int view { get; set; }
        public int guests { get; set; }
        public long bytes { get; set; }
        public int flag { get; set; }
        public long? created { get; set; }
        public string desc { get; set; }
        public BoxPermission permission { get; set; }
        public long? autoDelete { get; set; }
    }

    internal class BoxMeta
    {

        public String id { get; set; }//boxId
        public String name { get; set; }//boxname
        public String owner { get; set; }//owners nick for this box
        public bool dr { get; set; }//is dataroom box
        public bool ro { get; set; }//is readonlybox
        public BoxConfig config { get; set; }//
        public BoxUser user { get; set; }//current user (with his permission on this box. not relevant if current    user is owner or manager
        public BoxFolder root { get; set; }//rootfolder of box
        public BoxPermission permission { get; set; }//default permission of this box (this will apply to newbox member on first join.only available for manager or owner

    }

    internal class BoxUser
    {
        public long uid { get; set; }
        public string name { get; set; }
        public int flag { get; set; }// check section Box User for more information
        public /*STATUS*/ object status { get; set; } // OWNER||MANAGER||IN(invitee)||TEMP||INACTIVE
        public long? firstAccess { get; set; }
        public long? lastAccess { get; set; }
        public BoxPermission permission { get; set; } //permission of user on this box (not relevant for owner or manager)
    }

    internal class BoxConfig
    {
        public int contingent { get; set; }
        public string key { get; set; }//boxKey
        public bool drable { get; set; }//can be upgrade to dataroom
        public int members { get; set; }//guests counts
        public string joinCode { get; set; }//passcode
        public bool forceCode { get; set; }// true means admin enforce passcode for the box if it has contingent> 0.
        public int backupConfig { get; set; } //bitflag : 0x00., the most-right bit tells if box is to be backed up    or not.the next bit tells if this option is booked or not.
        public DRBoxConfig drConfig { get; set; }//config for Dataroombox if applicable
    }

    internal class DRBoxConfig
    {
        public bool alertOn { get; set; }
        public int downloads { get; set; }
        public int mins { get; set; }
        public bool journalPriv { get; set; }
        public bool ctaOn { get; set; } //clich-thu-agreement on or off
        public int ctaVersion { get; set; }//ctaVersion
        public string ctaText { get; set; }// ctaText, if ctaOn=true, this must be not empty
    }

    internal class BoxFolder
    {
        public List<BoxFolder> folders { get; set; }//child folders
        public List<BoxNote> notes { get; set; } //child noted
        public List<BoxFile> files { get; set; } //child files
        public string nodeId { get; set; }//id in tree
        public string name { get; set; }//name
        public string creator { get; set; }//
        public string pathId { get; set; }//internal. dont use or care
        public bool own { get; set; } //is current user owner of this folder
        public long? ts { get; set; }//created
        public int d { get; set; }//internal for webdav, dont use or care
    }

    internal class BoxFile
    {
        public /*DRFileStatus*/ object drStat { get; set; } //dataroom status if applicable
        public /*BoxFileLock*/ object @lock { get; set; }//lock status if avail
        public string fileKey { get; set; }//internal, dont use
        public string iv { get; set; }//internal, dont use
        public long size { get; set; }//size in bytes
        public string nodeId { get; set; }//id in tree
        public string name { get; set; }//name
        public string creator { get; set; }//creator
        public string pathId { get; set; }//internal. dont use or care
        public bool own { get; set; } //is current user owner of this folder
        public long? ts { get; set; }//created
        public int d { get; set; }//internal for webdav, dont use or care
    }

    internal class BoxNote
    {
        public string content { get; set; }//note content
        public string HCol { get; set; }//internal, dont use
        public string nodeId { get; set; }//id in tree
        public string name { get; set; }//name
        public string creator { get; set; }//creator
        public string pathId { get; set; }//internal. dont use or care
        public bool own { get; set; } //is current user owner of this folder
        public long? ts { get; set; }//created
        public int d { get; set; }//internal for webdav, dont use or care
    }
}
