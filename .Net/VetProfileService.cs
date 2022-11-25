
using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Appointments;
using Sabio.Models.Domain.VetProfiles;
using Sabio.Models.Requests.VetProfiles;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Location = Sabio.Models.Domain.Locations.Location;
using Practice = Sabio.Models.Domain.VetProfiles.Practice;

namespace Sabio.Services
{
    public class VetProfileService : IVetProfileService
    {
        IDataProvider _data = null;
        private static ILocationService _locationService = null;
        public VetProfileService(IDataProvider data, ILocationService locationService)
        {
            _data = data;
            _locationService = locationService;
        }


        public Boolean CheckIfVetHasProfile(int id)
        {
            string procName = "[dbo].[VetProfiles_Exists]";
            Boolean result = true;

            _data.ExecuteCmd(procName
                , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@UserId", id);
                }, singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    result = reader.GetSafeBool(startingIndex);
                });

            return result;
        }


        public VetProfileV2 GetVetProfileById(int id)
        {
            string procName = "[dbo].[VetProfiles_Select_ByIdV3]";
            VetProfileV2 aVetProfile = null;

            _data.ExecuteCmd(procName, inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);

            }, singleRecordMapper: delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;

                aVetProfile = MapSingleVetProfile(reader, ref startingIndex);

            }, returnParameters: null);
            return aVetProfile;
        }


        public int AddVetProfile(VetProfileAddRequest model, int userId)
        {
            int id = 0;
            string procName = "[dbo].[VetProfiles_InsertV2]";
            _data.ExecuteNonQuery(procName,
           inputParamMapper: delegate (SqlParameterCollection col)
           {
               AddCommonParams(model, col, userId);

               SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
               idOut.Direction = ParameterDirection.Output;
               col.Add(idOut);
           },
           returnParameters: delegate (SqlParameterCollection returnCollection)
           {
               object oId = returnCollection["@Id"].Value;
               int.TryParse(oId.ToString(), out id);
           });
            return id;
        }


        public void UpdateVetProfile(VetProfileUpdateRequest model, int userId)
        {
            string procName = "[dbo].[VetProfiles_UpdateV2]";
            _data.ExecuteNonQuery(procName,
            inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParamsV2(model, col, userId);
                col.AddWithValue("@Id", model.Id);
            },
            returnParameters: null);
        }


        public void DeleteVetProfile(int id)
        {
            string procName = "[dbo].[VetProfiles_Delete_ById]";
            _data.ExecuteNonQuery(procName,
            inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
            },
            returnParameters: null);
        }


        public Paged<VetProfileV2> GetVetProfileByCreated(int pageIndex, int pageSize, int id)
        {
            Paged<VetProfileV2> pagedList = null;
            List<VetProfileV2> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(
                "dbo.VetProfiles_Select_ByCreatedByV3",
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                    param.AddWithValue("@Id", id);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    VetProfileV2 aVetProfile = MapSingleVetProfile(reader, ref startingIndex);
                    totalCount = reader.GetSafeInt32(startingIndex);

                    if (list == null)
                    {
                        list = new List<VetProfileV2>();
                    }

                    list.Add(aVetProfile);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<VetProfileV2>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }


        public Paged<VetProfileV2> GetAll(int pageIndex, int pageSize)
        {
            Paged<VetProfileV2> pagedList = null;
            List<VetProfileV2> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(
                "dbo.VetProfiles_SelectAllV3",
                (param) =>
                {
                    param.AddWithValue("PageIndex", pageIndex);
                    param.AddWithValue("PageSize", pageSize);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    VetProfileV2 aVetProfile = MapSingleVetProfile(reader, ref startingIndex);
                    totalCount = reader.GetSafeInt32(startingIndex);

                    if (list == null)
                    {
                        list = new List<VetProfileV2>();
                    }

                    list.Add(aVetProfile);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<VetProfileV2>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }
        public List<VetProfileV2> GetAllV2()
        {
            List<VetProfileV2> list = null;
            string procName = "[dbo].[VetProfiles_SelectAllV4]";

            _data.ExecuteCmd(procName, null,
                 delegate (IDataReader reader, short set)
                 {
                     int startingIndex = 0;
                     VetProfileV2 aProfile = MapSingleVetProfile(reader, ref startingIndex);


                     if (list == null)
                     {
                         list = new List<VetProfileV2>();
                     }
                     list.Add(aProfile);
                 });
            if (list != null)
            {
                list = new List<VetProfileV2>(list);
            }

            return list;
        }


        public Paged<VetProfileV2> SearchPaginated(int pageIndex, int pageSize, string query)
        {
            Paged<VetProfileV2> pagedList = null;
            List<VetProfileV2> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(
                "dbo.VetProfiles_SearchV3",
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                    param.AddWithValue("@Query", query);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    VetProfileV2 aVetProfile = MapSingleVetProfile(reader, ref startingIndex);
                    totalCount = reader.GetSafeInt32(startingIndex);

                    if (list == null)
                    {
                        list = new List<VetProfileV2>();
                    }

                    list.Add(aVetProfile);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<VetProfileV2>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }


        public Paged<VetProfileV2> SearchByServiceTypeIdPaginated(int pageIndex, int pageSize, string query, int categoryId)
        {
            Paged<VetProfileV2> pagedList = null;
            List<VetProfileV2> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(
                "dbo.VetProfiles_Search_ByServiceTypeIdV2",
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                    if (query == null)
                    {
                        query = "";
                    }
                    param.AddWithValue("@Query", query);
                    param.AddWithValue("@categoryId", categoryId);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    VetProfileV2 aVetProfile = MapSingleVetProfile(reader, ref startingIndex);
                    totalCount = reader.GetSafeInt32(startingIndex);

                    if (list == null)
                    {
                        list = new List<VetProfileV2>();
                    }

                    list.Add(aVetProfile);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<VetProfileV2>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }


        public Paged<VetProfileV2> GetVetProfileByPractice(int pageIndex, int pageSize, int id)
        {
            Paged<VetProfileV2> pagedList = null;
            List<VetProfileV2> list = null;
            int totalCount = 0;

            _data.ExecuteCmd(
                "dbo.VetProfiles_Select_ByPracticeId",
                (param) =>
                {
                    param.AddWithValue("@PageIndex", pageIndex);
                    param.AddWithValue("@PageSize", pageSize);
                    param.AddWithValue("@Id", id);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    VetProfileV2 aVetProfile = MapSingleVetProfile(reader, ref startingIndex);
                    totalCount = reader.GetSafeInt32(startingIndex);

                    if (list == null)
                    {
                        list = new List<VetProfileV2>();
                    }

                    list.Add(aVetProfile);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<VetProfileV2>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }


        private static VetProfileV2 MapSingleVetProfile(IDataReader reader, ref int startingIndex)
        {
            VetProfileV2 aVetProfile = new VetProfileV2();
            aVetProfile.CreatedBy = new User();
            aVetProfile.ServiceTypes = new LookUp();
            Location aLocation = new Location();
            LookUp locationType = new LookUp();
            LookUp state = new LookUp();

            aVetProfile.Id = reader.GetSafeInt32(startingIndex++);
            aVetProfile.Services = reader.DeserializeObject<List<Service>>(startingIndex++);
            aVetProfile.ServiceTypes.Id = reader.GetSafeInt32(startingIndex++);
            aVetProfile.ServiceTypes.Name = reader.GetSafeString(startingIndex++);
            aVetProfile.Bio = reader.GetSafeString(startingIndex++);
            aVetProfile.Phone = reader.GetSafeString(startingIndex++);
            aVetProfile.BusinessEmail = reader.GetSafeString(startingIndex++);
            aVetProfile.DateCreated = reader.GetDateTime(startingIndex++);
            aVetProfile.DateModified = reader.GetDateTime(startingIndex++);
            aVetProfile.CreatedBy.Id = reader.GetSafeInt32(startingIndex++);
            aVetProfile.CreatedBy.FirstName = reader.GetSafeString(startingIndex++);
            aVetProfile.CreatedBy.LastName = reader.GetSafeString(startingIndex++);
            aVetProfile.CreatedBy.UserImage = reader.GetSafeString(startingIndex++);
            aVetProfile.Practices = reader.DeserializeObject<List<Practice>>(startingIndex++);

            aVetProfile.Location = _locationService.MapSingleLocation(reader, ref startingIndex);

            aVetProfile.ModifiedBy = reader.GetSafeInt32(startingIndex++);
            aVetProfile.IsActive = reader.GetSafeBool(startingIndex++);
            aVetProfile.EmergencyLine = reader.GetSafeString(startingIndex++);
            return aVetProfile;
        }


        private static void AddCommonParams(VetProfileAddRequest model, SqlParameterCollection col, int userId)
        {
            col.AddWithValue("@Bio", model.Bio);
            col.AddWithValue("@Phone", model.Phone);
            col.AddWithValue("@BusinessEmail", model.BusinessEmail);
            col.AddWithValue("@CreatedBy", userId);
            col.AddWithValue("@ModifiedBy", userId);
            col.AddWithValue("@IsActive", model.IsActive);
            col.AddWithValue("@EmergencyLine", model.EmergencyLine);
        }


        private static void AddCommonParamsV2(VetProfileUpdateRequest model, SqlParameterCollection col, int userId)
        {
            col.AddWithValue("@Bio", model.Bio);
            col.AddWithValue("@Phone", model.Phone);
            col.AddWithValue("@BusinessEmail", model.BusinessEmail);
            col.AddWithValue("@ModifiedBy", userId);
            col.AddWithValue("@IsActive", model.IsActive);
            col.AddWithValue("@EmergencyLine", model.EmergencyLine);
        }


    }
}
