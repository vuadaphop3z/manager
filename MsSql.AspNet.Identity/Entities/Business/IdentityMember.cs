using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MsSql.AspNet.Identity.Entities
{
    public class IdentityMember : CommonIdentity
    {
        public virtual int Id { get; set; }

        [JsonIgnore]
        public virtual string PasswordHash { get; set; }
        [JsonIgnore]
        public virtual string SecurityStamp { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        [JsonIgnore]
        public virtual int AccessFailedCount { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual DateTime? LockoutEndDate { get; set; }
        [JsonIgnore]
        public virtual bool TwoFactorAuthEnabled { get; set; }

        //Entends
        public virtual string UserName { get; set; }
        public virtual DateTime? CreatedDateUtc { get; set; }
        [JsonIgnore]
        public virtual string PasswordHash2 { get; set; }
        public virtual string FullName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Avatar { get; set; }
        public virtual string OTPType { get; set; }
        public virtual DateTime? Birthday { get; set; }
        public virtual int Sex { get; set; }
        public virtual string Address { get; set; }
        public virtual string IDCard { get; set; }
        public virtual string Note { get; set; }
        public virtual string SocialProvider { get; set; }
        public virtual int Status { get; set; }


        public virtual string TokenKey { get; set; }
        public virtual DateTime? TokenCreatedDate { get; set; }
        public virtual DateTime? TokenExpiredDate { get; set; }
        [JsonIgnore]
        public virtual int LoginDurations { get; set; }
        [JsonIgnore]
        public virtual string Domain { get; set; }
        [JsonIgnore]
        public virtual string NewPassword { get; set; }

        public bool IsOwner { get; set; }

        public virtual List<string> Roles { get; set; }

        public virtual IdentityMemberData MetaData { get; set; }
        
        public IdentityMember()
        {
            this.Roles = new List<string>();
            this.MetaData = new IdentityMemberData();
            LockoutEnabled = true;
        }

    }

    public class IdentityMemberData
    {
        public int UserId { get; set; }
        public int FollowingCount { get; set; }
        public int MessageCount { get; set; }
        public int LikePostCount { get; set; }
        public int FollowerCount { get; set; }
        public int PostCount { get; set; }
        public int PhotoCount { get; set; }

    }

    public class WebAuthUserLoginIdentity
    {
        public virtual int Id { get; set; }
        public virtual string UserName { get; set; }

        [JsonIgnore]
        public virtual string PasswordHash { get; set; }

        public virtual string Email { get; set; }
        public virtual string FullName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Avatar { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual DateTime? CreatedDateUtc { get; set; }

        public virtual string TokenKey { get; set; }
        public virtual DateTime? TokenCreatedDate { get; set; }
        public virtual DateTime? TokenExpiredDate { get; set; }
        public virtual int LoginDurations { get; set; }
        public virtual string OTPType { get; set; }
    }

    public class ApiAuthUserLoginIdentity
    {
        public virtual int Id { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string UserName { get; set; }
        public virtual string FullName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Avatar { get; set; }
        [JsonIgnore]
        public virtual string PasswordHash { get; set; }

        public virtual string TokenKey { get; set; }
        public virtual DateTime? TokenCreatedDate { get; set; }
        public virtual DateTime? TokenExpiredDate { get; set; }

        public virtual int LoginDurations { get; set; }

        public virtual string OTPType { get; set; }
        public virtual DateTime? Birthday { get; set; }
        public virtual int Sex { get; set; }
        public virtual string Address { get; set; }
        public virtual string IDCard { get; set; }
        public virtual string Note { get; set; }

        public virtual int EmailConfirmed { get; set; }
        public virtual int PhoneNumberConfirmed { get; set; }
        public virtual DateTime LoginDate { get; set; }
        public virtual int SocialProviderId { get; set; }
    }

    public class UserTokenIdentity
    {
        public virtual int UserId { get; set; }
        public virtual string TokenKey { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual DateTime? ExpiredDate { get; set; }
        public virtual string Method { get; set; }
        public virtual string Domain { get; set; }
    }

    public class UserLogIdentity
    {
        public virtual int Id { get; set; }
        public string ActionType { get; set; }
        public virtual string UserIp { get; set; }
        public virtual string UserAgent { get; set; }
        public virtual string Method { get; set; }
        public virtual string Domain { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual string ActionDesc { get; set; }
        public string RawData { get; set; }
    }

    public class UserCodeIdentity
    {
        public virtual string Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual string Code { get; set; }
        public virtual string CodeType { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual DateTime? ExpiredDate { get; set; }
        public int IsUsed { get; set; }
        public string Action { get; set; }

        public string TargetData { get; set; }

        //For checking user token
        public string TokenKey { get; set; }
    }

    public class IdentityUserAction
    {
        public virtual int UserId { get; set; }
        public virtual string TokenKey { get; set; }
        public virtual int UserActionId { get; set; }
        public virtual int Status { get; set; }
        public virtual string ActionType { get; set; }
    }

    public class IdentityActiveAccount
    {
        public virtual string UserName { get; set; }
        public virtual string UserId { get; set; }
        public virtual string ActiveMethod { get; set; }
        public virtual string OTPCode { get; set; }
        public virtual string HashingData { get; set; }
    }
}
