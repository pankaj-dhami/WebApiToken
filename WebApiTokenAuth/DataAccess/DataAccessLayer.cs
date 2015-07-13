using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
                    string b64PicData = "";

                    if (user.Pic64Data != null)
                    {
                        b64PicData = string.Join("/", user.Pic64Data);
                    }

                    tblappUser tbluser = (from item in entity.tblappUsers
                                          where item.MobileNo == user.MobileNo
                                          select item).FirstOrDefault();
                    if (tbluser == null)
                    {
                        tbluser = new tblappUser();
                        tbluser.MobileNo = user.MobileNo;
                        tbluser.Name = user.Name;
                        tbluser.Password = user.Password;
                        tbluser.MyStatus = user.MyStatus;
                        entity.tblappUsers.Add(tbluser);
                        entity.SaveChanges();
                        if (!string.IsNullOrEmpty(b64PicData))
                        {
                            user.PictureUrl = BlobUploadUtility.UploadFileStreamToBlob(BlobUploadUtility.Base64Decode(b64PicData), tbluser.UserID + tbluser.MobileNo, BlobUploadUtility.CONTAINER_IMAGE);
                        }
                        tbluser.PicUrl = user.PictureUrl;
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
        /// <param name="user"></param>
        /// <returns></returns>
        public AppResultModel UpdateUser(UserModel user)
        {
            AppResultModel result = new AppResultModel();
            using (AndroidMessengerEntities entity = new AndroidMessengerEntities())
            {
                try
                {
                    string b64PicData = "";

                    if (user.Pic64Data != null)
                    {
                        b64PicData = string.Join("/", user.Pic64Data);
                    }


                    tblappUser tbluser = (from item in entity.tblappUsers
                                          where item.UserID == user.UserID
                                          select item).FirstOrDefault();
                    if (tbluser != null)
                    {
                        if ( !string.IsNullOrEmpty(user.Name))
                        {
                            tbluser.Name = user.Name;
                        }
                        if (!string.IsNullOrEmpty(user.MyStatus))
                        {
                            tbluser.MyStatus = user.MyStatus;
                        }
                        if (!string.IsNullOrEmpty(b64PicData))
                        {
                            user.PictureUrl = BlobUploadUtility.UploadFileStreamToBlob(BlobUploadUtility.Base64Decode(b64PicData), tbluser.UserID + tbluser.MobileNo, BlobUploadUtility.CONTAINER_IMAGE);
                            tbluser.PicUrl = user.PictureUrl;
                            tbluser.IsPicUpdate += 1;
                        }
                        entity.SaveChanges();
                        result.ResultStatus = (int)AppResultStatus.SUCCESS;
                        result.ResultMessage = ApplicationConstants.Success;
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
                    tblappUser tbluser = (from item in entity.tblappUsers
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
                    tblappUser tbluser = (from item in entity.tblappUsers
                                          where item.MobileNo == mobileNo
                                          select item).FirstOrDefault();
                    if (tbluser != null)
                    {
                        result = new UserModel();
                        result.MobileNo = tbluser.MobileNo;
                        result.Name = tbluser.Name;
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
        public List<UserModel> RegisterFriends(List<TelephoneNumberModel> friendList, int userID)
        {
            List<UserModel> tblExistinguser = null;


            DataTable tableInput = ToDataTable(friendList);
            using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["AndroidMessengerEntitiesADO"].ConnectionString))
            {
                try
                {
                    SqlCommand Cmd = new SqlCommand();
                    Cmd.CommandText = "uspAppUsers";
                    Cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@TblData", tableInput);
                    Cmd.Parameters.AddWithValue("@userID", userID);
                    Conn.Open();
                    Cmd.Connection = Conn;
                    SqlDataReader reader = Cmd.ExecuteReader();
                    tblExistinguser = new List<UserModel>();
                    while (reader.Read())
                    {
                        UserModel model = new UserModel();
                        model.UserID = (int)reader["UserID"];
                        model.MobileNo = reader["MobileNo"] == System.DBNull.Value ? string.Empty : (string)reader["MobileNo"];
                        model.Name = reader["Name"] == System.DBNull.Value ? string.Empty : (string)reader["Name"];
                        model.MyStatus = reader["MyStatus"] == System.DBNull.Value ? string.Empty : (string)reader["MyStatus"];
                        model.PictureUrl = reader["PicUrl"] == System.DBNull.Value ? string.Empty : (string)reader["PicUrl"];
                        model.IsPicUpdate = (int)reader["IsPicUpdate"];
                        tblExistinguser.Add(model);
                    }
                }
                catch (Exception)
                {
                    tblExistinguser = null;
                }
                finally
                {
                    Conn.Close();
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
                    tblExistinguser = (from friend in entity.tblappFriends
                                       join user in entity.tblappUsers on friend.FriendID equals user.UserID
                                       where friend.UserID == userID
                                       select new UserModel
                                       {
                                           UserID = user.UserID,
                                           MyStatus = user.MyStatus,
                                           Name = user.Name,
                                           MobileNo = user.MobileNo,
                                          PictureUrl=user.PicUrl

                                       }).ToList();

                   
                }
                catch (Exception)
                {
                }
            }
            return tblExistinguser;
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
        public static DataTable ToDataTable<TSource>(IList<TSource> data)
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
        public static List<TSource> ToList<TSource>(DataTable dataTable) where TSource : new()
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