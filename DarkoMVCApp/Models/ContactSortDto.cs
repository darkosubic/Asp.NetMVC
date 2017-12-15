using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DarkoMVCApp.Models
{
    public class ContactSortDto
    {
        public int CurrentPage { get; set; }
        public int MaxContactsPerPage { get; set; }
        public bool? IdSortOrder { get; set; }
        public bool? FirstNameOrder { get; set; }
        public bool? LastNameOrder { get; set; }
        public bool? TelephoneOrder { get; set; }
        public bool? MailOrder { get; set; }
        public bool? CreatedDateOrder { get; set; }

        public bool? ReturnMe(bool? test)
        {
            bool? direction = null;

            if (test == null)
                direction = true;

            else if (test == true)
                direction = false;

            else if (test == false)
                direction = null;

            return direction;
        }
    }
}