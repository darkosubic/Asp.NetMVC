using DarkoMVCApp.Helpers;
using DarkoMVCApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Web;
using System.Web.Mvc;
using DarkoMVCApp.ViewModels;
using PagedList;

namespace DarkoMVCApp.Controllers
{
    public class ContactsController : Controller
    {
        private readonly IWebConfigReader _webConfigReader;

        public ContactsController(IWebConfigReader webConfigReader)
        {
            _webConfigReader = webConfigReader;
        }
        
        public ViewResult Index(
                bool? idSortOrder,
                bool? firstNameSortOrder,
                bool? lastNameSortOrder,
                bool? telephoneSortOrder,
                bool? mailSortOrder,
                bool? dateCreatedSortOrder,
                int? page,
                string searchQuery)
        {
            ContactSortDto sortDto = new ContactSortDto();

            if (page == null || page == 0)
                sortDto.CurrentPage = 1;
            else
                sortDto.CurrentPage = page.Value;

            sortDto.MaxContactsPerPage = _webConfigReader.MaxContactsPerPage;
            sortDto.IdSortOrder = idSortOrder;
            sortDto.FirstNameOrder = firstNameSortOrder;
            sortDto.LastNameOrder = lastNameSortOrder;
            sortDto.TelephoneOrder = telephoneSortOrder;
            sortDto.MailOrder = mailSortOrder;
            sortDto.CreatedDateOrder = dateCreatedSortOrder;


            if (searchQuery == "" || searchQuery == string.Empty)
                searchQuery = null;

            StaticPagedList<ContactDto> pagedListData = GetAllContacts(sortDto, searchQuery);

            ContactListViewModel vm = new ContactListViewModel();
            vm.Contacts = pagedListData;
            vm.Sort = sortDto;

            // keeping Textbox populated
            ViewBag.searchQuery = searchQuery;

            return View(vm);
        }

        private StaticPagedList<ContactDto> GetAllContacts(ContactSortDto dto, string searchQuery)
        {
            StaticPagedList<ContactDto> pagedList;

            using (SqlConnection conn = new SqlConnection(_webConfigReader.DefaultCollectionReader))
            using (SqlCommand cmd = new SqlCommand("dbo.Contacts_Get", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@currentPage", SqlDbType.Int).Value = dto.CurrentPage;
                cmd.Parameters.AddWithValue("@maxContactsPerPage", SqlDbType.Int).Value = dto.MaxContactsPerPage;
                cmd.Parameters.AddWithValue("@searchQuery", SqlDbType.VarChar).Value = searchQuery;
                cmd.Parameters.AddWithValue("@idOrder", SqlDbType.Bit).Value = dto.IdSortOrder;
                cmd.Parameters.AddWithValue("@firstNameOrder", SqlDbType.Bit).Value = dto.FirstNameOrder;
                cmd.Parameters.AddWithValue("@lastNameOrder", SqlDbType.Bit).Value = dto.LastNameOrder;
                cmd.Parameters.AddWithValue("@telephoneOrder", SqlDbType.Bit).Value = dto.TelephoneOrder;
                cmd.Parameters.AddWithValue("@mailOrder", SqlDbType.Bit).Value = dto.MailOrder;
                cmd.Parameters.AddWithValue("@createdDateOrder", SqlDbType.Bit).Value = dto.CreatedDateOrder;

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                cmd.Parameters.Add("maxPages", SqlDbType.Int);

                int totalItems = 0;
                List<ContactDto> contacts = new List<ContactDto>();

                while (reader.Read())
                {
                    ContactDto contact = new ContactDto();

                    contact.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                    contact.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                    contact.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                    contact.Telephone = reader.GetString(reader.GetOrdinal("Telephone"));
                    contact.Mail = reader.GetString(reader.GetOrdinal("Mail"));
                    contact.DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated"));
                    contacts.Add(contact);
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        int maxRowCount = reader.GetOrdinal("MaxRowCount");
                        totalItems = reader.IsDBNull(maxRowCount) ? 0 : reader.GetInt32(maxRowCount);
                    }
                }

                pagedList = new StaticPagedList<ContactDto>(contacts, dto.CurrentPage, _webConfigReader.MaxContactsPerPage, totalItems);

                // populating search result counter
                if (searchQuery != null && totalItems > 0)
                    ViewBag.ResultsFound = totalItems;

                conn.Close();

            }
            return pagedList;
        }

        public ViewResult Insert()
        {
            return View();
        }


        [HttpPost]
        [ActionName("Insert")]
        [ValidateAntiForgeryToken]
        public ActionResult InsertContact(ContactBaseDto dto)
        {
            int insertedContactId = 0;

            using (SqlConnection conn = new SqlConnection(_webConfigReader.DefaultCollectionReader))
            using (SqlCommand cmd = new SqlCommand("dbo.Contacts_Ins", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@firstName", SqlDbType.VarChar).Value = dto.FirstName;
                cmd.Parameters.AddWithValue("@lastName", SqlDbType.VarChar).Value = dto.LastName;
                cmd.Parameters.AddWithValue("@telephone", SqlDbType.VarChar).Value = dto.Telephone;
                cmd.Parameters.AddWithValue("@mail", SqlDbType.VarChar).Value = dto.Mail;

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                    insertedContactId = reader.GetInt32(0);

                conn.Close();
            }

            return RedirectToAction("Update", new { id = insertedContactId });
        }

        [HttpPost]
        [ActionName("Update")]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateContact(ContactDto dto)
        {
            using (SqlConnection conn = new SqlConnection(_webConfigReader.DefaultCollectionReader))
            using (SqlCommand cmd = new SqlCommand("dbo.Contacts_Upd", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", SqlDbType.Int).Value = dto.Id;
                cmd.Parameters.AddWithValue("@firstName", SqlDbType.VarChar).Value = dto.FirstName;
                cmd.Parameters.AddWithValue("@lastName", SqlDbType.VarChar).Value = dto.LastName;
                cmd.Parameters.AddWithValue("@telephone", SqlDbType.VarChar).Value = dto.Telephone;
                cmd.Parameters.AddWithValue("@mail", SqlDbType.VarChar).Value = dto.Mail;

                conn.Open();

                cmd.ExecuteNonQuery();

                conn.Close();
            }

            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public ActionResult Update(int id)
        {
            ContactDto dto = Get(id);

            if (dto == null)
            {
                return HttpNotFound();
            }
            return View(dto);
        }

        private ContactDto Get(int id)
        {
            ContactDto dto = new ContactDto();

            using (SqlConnection conn = new SqlConnection(_webConfigReader.DefaultCollectionReader))
            using (SqlCommand cmd = new SqlCommand("dbo.Contact_GetById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@contactId", SqlDbType.Int).Value = id;

                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    dto.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                    dto.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                    dto.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                    dto.Telephone = reader.GetString(reader.GetOrdinal("Telephone"));
                    dto.Mail = reader.GetString(reader.GetOrdinal("Mail"));
                    dto.DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated"));
                }

                conn.Close();
            }

            return dto;
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_webConfigReader.DefaultCollectionReader))
            using (SqlCommand cmd = new SqlCommand("dbo.Contacts_Del", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@contactId", SqlDbType.Int).Value = id;

                conn.Open();

                cmd.ExecuteNonQuery();

                conn.Close();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            using (SqlConnection conn = new SqlConnection(_webConfigReader.DefaultCollectionReader))
            using (SqlCommand cmd = new SqlCommand("dbo.Contacts_Del", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@contactId", SqlDbType.Int).Value = id;

                conn.Open();

                cmd.ExecuteNonQuery();

                conn.Close();
            }

            return RedirectToAction("Index");
        }
    }
}