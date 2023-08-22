using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactManager.Models
{
    public class TestDataGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ContactContext(serviceProvider.GetRequiredService<DbContextOptions<ContactContext>>()))
            {
                if (context.Contacts.Any()) { return; }

                context.Contacts.AddRange(
                    new Contact
                    {
                        ID = 1,
                        FirstName = "David",
                        LastName = "Rose",
                        Company = "Rose Apothecary",
                        ProfileImage = "",
                        Email = "david.rose@roseapothecary.com",
                        BirthDate = DateTime.Parse("August 9, 1983"),
                        WorkPhone = "(111) 111-1111",
                        HomePhone = "(222) 222-2222",
                        Address1 = "123 Main Street",
                        Address2 = "",
                        City = "Schitts Creek",
                        State = "ON",
                        Zip = "M4K 1A2",
                        Country = "CA",
                        Notes = "Ew, David!"
                    },
                    new Contact
                    {
                        ID = 2,
                        FirstName = "Leslie",
                        LastName = "Knope",
                        Company = "National Park Service",
                        ProfileImage = "",
                        Email = "leslie.knope@gryzzl.com",
                        BirthDate = DateTime.Parse("January 18, 1975"),
                        WorkPhone = "(333) 333-3333",
                        HomePhone = "(444) 444-4444",
                        Address1 = "345 Main Street",
                        Address2 = "",
                        City = "Pawnee",
                        State = "IN",
                        Zip = "12345",
                        Country = "US",
                        Notes = "Happy Galentines Day!"
                    },
                    new Contact
                    {
                        ID = 3,
                        FirstName = "Sterling",
                        LastName = "Archer",
                        Company = "Figgis Agency",
                        ProfileImage = "",
                        Email = "archer@dangerzone.com",
                        BirthDate = null,
                        WorkPhone = "(555) 555-5555",
                        HomePhone = "",
                        Address1 = "123 Mystery Ave",
                        Address2 = "",
                        City = "New York",
                        State = "NY",
                        Zip = "",
                        Country = "US",
                        Notes = "World's greatest secret agent"
                    },
                    new Contact
                    {
                        ID = 4,
                        FirstName = "Frank",
                        LastName = "Reynolds",
                        Company = "Wolf Cola",
                        ProfileImage = "",
                        Email = "frank@wolfcola.com",
                        BirthDate = DateTime.Parse("November 1, 1944"),
                        WorkPhone = "(666) 666-6666",
                        HomePhone = "(777) 777-7777",
                        Address1 = "Paddy's Pub",
                        Address2 = "",
                        City = "Philadelphia",
                        State = "PA",
                        Zip = "19103",
                        Country = "US",
                        Notes = "The Warthog"
                    },
                    new Contact
                    {
                        ID = 5,
                        FirstName = "Frankie",
                        LastName = "Bergstein",
                        Company = "Vibrant",
                        ProfileImage = "",
                        Email = "frankie@deltaco.com",
                        BirthDate = DateTime.Parse("September 1, 1939"),
                        WorkPhone = "",
                        HomePhone = "(888) 888-8888",
                        Address1 = "12345 Beach Street",
                        Address2 = "",
                        City = "Los Angeles",
                        State = "CA",
                        Zip = "90009",
                        Country = "US",
                        Notes = "Not a fan of Sante Fe"
                    }
                    );

                context.SaveChanges();
            }
        }

    }
}
