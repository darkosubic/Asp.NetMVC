using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DarkoMVCApp.Models;
using PagedList;

namespace DarkoMVCApp.ViewModels
{
    public class ContactListViewModel
    {
        public StaticPagedList<ContactDto> Contacts { get; set; }
        public ContactSortDto Sort { get; set; }
    }
}