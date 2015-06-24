using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using WebApiTokenAuth.Model;
using WebApiTokenAuth.Utility;

namespace WebApiTokenAuth.DataAccess
{
    public class DataAccessLayer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public AppResultModel RegisterUser(UserModel user)
        {
            AppResultModel result = new AppResultModel();
            using (AndroidMessengerEntities entity = new AndroidMessengerEntities())
            {
                try
                {
                    tblAppUser tbluser = (from item in entity.tblAppUsers
                                          where item.MobileNo == user.MobileNo
                                          select item).FirstOrDefault();
                    if (tbluser == null)
                    {
                        tbluser = new tblAppUser();
                        tbluser.MobileNo = user.MobileNo;
                        tbluser.Name = user.Name;
                        tbluser.Password = user.Password;
                        tbluser.MyStatus = user.MyStatus;
                        // tbluser.AboutMe = user.AboutMe;
                        tbluser.ProfilePicUrl = user.PictureUrl;
                        string b64PicData = string.Empty;
                        //foreach (var item in user.Pic64Data)
                        //{
                        //    b64PicData += item;
                        //}
                        b64PicData = string.Join("/", user.Pic64Data);


                        tbluser.ProfilePicData = Base64Decode(b64PicData);
                        entity.tblAppUsers.Add(tbluser);
                        entity.SaveChanges();
                        result.ResultStatus = (int)AppResultStatus.SUCCESS;
                        result.ResultMessage = ApplicationConstants.Success;
                    }
                    else
                    {
                        result.ResultStatus = (int)AppResultStatus.DUPLICATE;
                        result.ResultMessage = ApplicationConstants.User_Already_Exists;
                    }
                }
                catch (Exception)
                {

                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public AppResultModel FindUser(string userName, string password)
        {
            AppResultModel result = new AppResultModel();
            using (AndroidMessengerEntities entity = new AndroidMessengerEntities())
            {
                try
                {
                    tblAppUser tbluser = (from item in entity.tblAppUsers
                                          where item.MobileNo == userName && item.Password == password
                                          select item).FirstOrDefault();
                    if (tbluser != null)
                    {
                        result.ResultStatus = (int)AppResultStatus.SUCCESS;
                        result.ResultMessage = ApplicationConstants.Success;
                    }
                    else
                    {
                        result.ResultStatus = (int)AppResultStatus.UNAUTHORIZED;
                        result.ResultMessage = ApplicationConstants.User_Unauthorized;
                    }
                }
                catch (Exception)
                {

                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserModel GetUser(string mobileNo)
        {
            UserModel result = null;
            using (AndroidMessengerEntities entity = new AndroidMessengerEntities())
            {
                try
                {
                    tblAppUser tbluser = (from item in entity.tblAppUsers
                                          where item.MobileNo == mobileNo
                                          select item).FirstOrDefault();
                    if (tbluser != null)
                    {
                        result = new UserModel();
                        result.MobileNo = tbluser.MobileNo;
                        result.Name = tbluser.Name;
                        result.PicData = tbluser.ProfilePicData;
                        result.MyStatus = tbluser.MobileNo;
                        result.UserID = tbluser.UserID;
                        result.Password = tbluser.Password;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<UserModel> RegisterFriends(List<RegisterFriendsMdoel> friendList, int userID)
        {
            List<UserModel> tblExistinguser = null;
            using (AndroidMessengerEntities entity = new AndroidMessengerEntities())
            {
                try
                {
                    DataTable friendsData = ToDataTable(friendList);

                    tblExistinguser = new List<UserModel>();
                    foreach (var item in friendList)
                    {
                        //item.MobileNo = Regex.Replace(item.MobileNo, @"\s+", "");
                        //if (item.MobileNo.StartsWith("0"))
                        //{
                        //    item.MobileNo = countryCode + item.MobileNo.Remove(0, 1) ;
                            
                        //}

                        //var dbuser = (from user in entity.tblAppUsers
                        //              where user.MobileNo == item.MobileNo
                        //              select user).AsQueryable().FirstOrDefault();
                        //if (dbuser != null)
                        //{
                        //    var existingFriend = (from user in entity.tblAppFriends
                        //                          where user.FriendUserID == dbuser.UserID && user.UserID == userID
                        //                          select user).AsQueryable().FirstOrDefault();
                        //    if (existingFriend == null)
                        //    {
                        //        item.UserID = dbuser.UserID;
                        //        item.MyStatus = dbuser.MyStatus;
                        //        item.PictureUrl = Base64Encode(dbuser.ProfilePicData);
                        //        tblExistinguser.Add(item);
                        //        tblAppFriend appFriend = new tblAppFriend();
                        //        appFriend.UserID = userID;
                        //        appFriend.FriendUserID = item.UserID;
                        //        appFriend.FriendName = item.Name;
                        //        entity.tblAppFriends.Add(appFriend);
                        //    }

                        //}
                    }
                    entity.SaveChanges();
                }
                catch (Exception)
                {
                    tblExistinguser = null;
                }
            }

            return tblExistinguser;
        }

        public List<UserModel> GetUpdatedFriendsList(int userID)
        {
            List<UserModel> tblExistinguser = null;
            using (AndroidMessengerEntities entity = new AndroidMessengerEntities())
            {
                try
                {
                    tblExistinguser = (from friend in entity.tblAppFriends
                                       join user in entity.tblAppUsers on friend.FriendUserID equals user.UserID
                                       where friend.UserID == userID
                                       select new UserModel
                                       {
                                           UserID = user.UserID,
                                           MyStatus = user.MyStatus,
                                           Name = friend.FriendName,
                                           MobileNo = user.MobileNo,
                                           PicData = user.ProfilePicData

                                       }).ToList();

                    foreach (var item in tblExistinguser)
                    {
                        item.PictureUrl = Base64Encode(item.PicData);
                        item.PicData = null;
                    }
                }
                catch (Exception)
                {
                }
            }
            return tblExistinguser;
        }


        public static string Base64Encode(byte[] bytes)
        {
            if (bytes == null)
            {
                return "";
            }
            else
            {
                return System.Convert.ToBase64String(bytes);
            }
        }

        public static byte[] Base64Decode(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
            {
                return null;
            }
            else
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return base64EncodedBytes;
            }
        }

        public static byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory,
                CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }

        static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        /*Converts List To DataTable*/
        public static DataTable ToDataTable<TSource>(this IList<TSource> data)
        {
            DataTable dataTable = new DataTable(typeof(TSource).Name);
            PropertyInfo[] props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ??
                    prop.PropertyType);
            }

            foreach (TSource item in data)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }

        /*Converts DataTable To List*/
        public static List<TSource> ToList<TSource>(this DataTable dataTable) where TSource : new()
        {
            var dataList = new List<TSource>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
                                 select new
                                 {
                                     Name = aProp.Name,
                                     Type = Nullable.GetUnderlyingType(aProp.PropertyType) ??
                             aProp.PropertyType
                                 }).ToList();
            var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
                                     select new
                                     {
                                         Name = aHeader.ColumnName,
                                         Type = aHeader.DataType
                                     }).ToList();
            var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var aTSource = new TSource();
                foreach (var aField in commonFields)
                {
                    PropertyInfo propertyInfos = aTSource.GetType().GetProperty(aField.Name);
                    var value = (dataRow[aField.Name] == DBNull.Value) ?
                    null : dataRow[aField.Name]; //if database field is nullable
                    propertyInfos.SetValue(aTSource, value, null);
                }
                dataList.Add(aTSource);
            }
            return dataList;
        } 
    }
}