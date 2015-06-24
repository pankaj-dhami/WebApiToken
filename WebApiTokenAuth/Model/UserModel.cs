using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiTokenAuth.Model
{
    public class UserModel
    {
        //[Display(Name = "Name")]
        public string Name { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public int UserID { get; set; }
        //[Display(Name = "Mobile Number")]
        //[Required]
        public string MobileNo { get; set; }
        public string ConnectionID { get; set; }
        public string MyStatus { get; set; }
        public string PictureUrl { get; set; }
        public byte[] PicData { get; set; }
        public IEnumerable<string> Pic64Data { get; set; }
        public int _id { get; set; }

        public int PicImg { get; set; }

    }


    public class UserMsg
    {
        public string ConnectionID { get; set; }
        public string MobileNo { get; set; }
        public string MyStatus { get; set; }
        public string Name { get; set; }
        public string PicImg { get; set; }
        public string PictureUrl { get; set; }
        public int UserID { get; set; }

    }
    public class MessageModel
    {
        public UserMsg UserModel { get; set; }
        public string AttachmentUrl { get; set; }
        public string AttachmentName { get; set; }
        public byte[] AttachmentData { get; set; }
        public string TextMessage { get; set; }
        public int UserID { get; set; }
    }

    public class SendMsgModel
    {
        public MessageModel Message { get; set; }
        public UserMsg FromUser { get; set; }
    }

    public class TelephoneNumberModel
    {
        public string Name { get; set; }
        public string MobileNo { get; set; }
    }
}