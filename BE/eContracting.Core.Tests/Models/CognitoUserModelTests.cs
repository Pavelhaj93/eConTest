using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eContracting.Models;
using Xunit;

namespace eContracting.Core.Tests.Models
{
    public class CognitoUserModelTests
    {
        [Fact]
        public void When_Constructed_All_Properties_Are_Correctly_Set()
        {
            string sub = "lkjg840292j8f3";
            IEnumerable<string> customSegments = new List<string>() { "B2C_Customers", "B2B_Customers" };
            bool emailVerified = true;
            string preferredUsername = "blazena_z_hostic";
            string givenName = "Blažena";
            string familyName = "Škopková";
            string email = "blazena.skopkova@hostice.cz";
            string username = "lkjg840292j8f3";

            var model = new CognitoUserModel(sub, customSegments, emailVerified, preferredUsername, givenName, familyName, email, username);

            Assert.Equal(sub, model.Sub);
            Assert.Equal(customSegments, model.CustomSegments);
            Assert.Equal(emailVerified, model.EmailVerified);
            Assert.Equal(preferredUsername, model.PreferredUsername);
            Assert.Equal(givenName, model.GivenName);
            Assert.Equal(familyName, model.FamilyName);
            Assert.Equal(email, model.Email);
            Assert.Equal(username, model.Username);
        }

        [Fact]
        public void When_Constructed_With_UserData_All_Properties_Are_Correctly_Set()
        {
            var userData = new CognitoUserDataModel();
            userData.UserAttributes = new List<CognitoUserDataModel.UserAttribute>()
            {
                new CognitoUserDataModel.UserAttribute() { Name = "sub", Value = "ae974d48-7307-4228-a623-3ce3a1070090" },
                new CognitoUserDataModel.UserAttribute() { Name = "custom:segment", Value = "B2C_Customers" },
                new CognitoUserDataModel.UserAttribute() { Name = "email_verified", Value = "true" },
                new CognitoUserDataModel.UserAttribute() { Name = "preferred_username", Value = "ae974d48-7307-4228-a623-3ce3a1070091" },
                new CognitoUserDataModel.UserAttribute() { Name = "given_name", Value = "Lukáš" },
                new CognitoUserDataModel.UserAttribute() { Name = "family_name", Value = "Dvořák" },
                new CognitoUserDataModel.UserAttribute() { Name = "email", Value = "lukas.dvorak@actumdigital.com" }
            }.ToArray();
            userData.Username = "ae974d48-7307-4228-a623-3ce3a1070093";

            var model = new CognitoUserModel(userData);

            Assert.Equal(userData.UserAttributes.First(x => x.Name == "sub").Value, model.Sub);
            Assert.Contains(userData.UserAttributes.First(x => x.Name == "custom:segment").Value, model.CustomSegments);
            Assert.Equal(userData.UserAttributes.First(x => x.Name == "email_verified").Value, model.EmailVerified ? "true" : "false");
            Assert.Equal(userData.UserAttributes.First(x => x.Name == "preferred_username").Value, model.PreferredUsername);
            Assert.Equal(userData.UserAttributes.First(x => x.Name == "given_name").Value, model.GivenName);
            Assert.Equal(userData.UserAttributes.First(x => x.Name == "family_name").Value, model.FamilyName);
            Assert.Equal(userData.UserAttributes.First(x => x.Name == "email").Value, model.Email);
            Assert.Equal(userData.Username, model.Username);
        }
    }
}
