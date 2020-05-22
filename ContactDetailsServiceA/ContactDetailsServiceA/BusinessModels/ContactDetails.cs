using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace ContactDetailsServiceA.BusinessModels
{
    public class ContactDetailsModel
    {
        public string Name { get; private set;}

        public ContactDetailsModel() { }

        public bool TrySetName(string responseName)
        {
            if (ValidateName(responseName))
            {
                Name = responseName;
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ValidateName(string name)
        {
            bool pass = false;
            if (!String.IsNullOrEmpty(name) && !String.IsNullOrWhiteSpace(name))
            {
                if (name.Length <= 35)
                {
                    if (!name.Contains(" "))
                    {
                        char[] letter = name.ToCharArray();
                        if (letter[0].ToString().Any(char.IsUpper))
                        {
                            pass = Regex.IsMatch(name, @"^[a-zA-Z]+$");
                        }
                    }
                }
            }
            return pass;

        }
    }
}
