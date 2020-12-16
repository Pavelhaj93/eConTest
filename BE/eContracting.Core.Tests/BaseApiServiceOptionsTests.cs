using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace eContracting.Core.Tests
{
    public class BaseApiServiceOptionsTests
    {
        [Fact]
        public void Constructor_Throws_ArgumentException_When_Url_Null()
        {
            string url = null;
            string username = Convert.ToBase64String(Encoding.UTF8.GetBytes("username"));
            string password = Convert.ToBase64String(Encoding.UTF8.GetBytes("password"));

            Assert.Throws<ArgumentException>(() => { new CacheApiServiceOptions(url, username, password); });
            Assert.Throws<ArgumentException>(() => { new SignApiServiceOptions(url, username, password); });
        }

        [Fact]
        public void Constructor_Throws_ArgumentException_When_Url_Empty_String()
        {
            string url = string.Empty;
            string username = Convert.ToBase64String(Encoding.UTF8.GetBytes("username"));
            string password = Convert.ToBase64String(Encoding.UTF8.GetBytes("password"));

            Assert.Throws<ArgumentException>(() => { new CacheApiServiceOptions(url, username, password); });
            Assert.Throws<ArgumentException>(() => { new SignApiServiceOptions(url, username, password); });
        }

        [Fact]
        public void Constructor_Throws_UriFormatException_When_Url_Relative()
        {
            string url = "/mypage";
            string username = Convert.ToBase64String(Encoding.UTF8.GetBytes("username"));
            string password = Convert.ToBase64String(Encoding.UTF8.GetBytes("password"));

            Assert.Throws<UriFormatException>(() => { new CacheApiServiceOptions(url, username, password); });
            Assert.Throws<UriFormatException>(() => { new SignApiServiceOptions(url, username, password); });
        }

        [Fact]
        public void Constructor_Throws_ArgumentException_When_Username_Null()
        {
            string url = "http://localhost";
            string username = null;
            string password = Convert.ToBase64String(Encoding.UTF8.GetBytes("password"));

            Assert.Throws<ArgumentException>(() => { new CacheApiServiceOptions(url, username, password); });
            Assert.Throws<ArgumentException>(() => { new SignApiServiceOptions(url, username, password); });
        }

        [Fact]
        public void Constructor_Throws_ArgumentException_When_Username_EmptyString()
        {
            string url = "http://localhost";
            string username = string.Empty;
            string password = Convert.ToBase64String(Encoding.UTF8.GetBytes("password"));

            Assert.Throws<ArgumentException>(() => { new CacheApiServiceOptions(url, username, password); });
            Assert.Throws<ArgumentException>(() => { new SignApiServiceOptions(url, username, password); });
        }

        /// <summary>
        /// Not always it can throw FormatException! This test force to do it.
        /// </summary>
        [Fact]
        public void Constructor_Can_Throws_FormatException_When_Username_Not_Base64_Format()
        {
            string url = "http://localhost";
            string username = "myusername";
            string password = Convert.ToBase64String(Encoding.UTF8.GetBytes("password"));

            Assert.Throws<FormatException>(() => { new CacheApiServiceOptions(url, username, password); });
            Assert.Throws<FormatException>(() => { new SignApiServiceOptions(url, username, password); });
        }

        [Fact]
        public void Constructor_Throws_ArgumentException_When_Password_Null()
        {
            string url = "http://localhost";
            string username = Convert.ToBase64String(Encoding.UTF8.GetBytes("username"));
            string password = null;

            Assert.Throws<ArgumentException>(() => { new CacheApiServiceOptions(url, username, password); });
            Assert.Throws<ArgumentException>(() => { new SignApiServiceOptions(url, username, password); });
        }

        [Fact]
        public void Constructor_Throws_ArgumentException_When_Password_EmptyString()
        {
            string url = "http://localhost";
            string username = Convert.ToBase64String(Encoding.UTF8.GetBytes("username"));
            string password = string.Empty;

            Assert.Throws<ArgumentException>(() => { new CacheApiServiceOptions(url, username, password); });
            Assert.Throws<ArgumentException>(() => { new SignApiServiceOptions(url, username, password); });
        }

        /// <summary>
        /// Not always it can throw FormatException! This test force to do it.
        /// </summary>
        [Fact]
        public void Constructor_Can_Throws_FormatException_When_Password_Not_Base64_Format()
        {
            string url = "http://localhost";
            string username = Convert.ToBase64String(Encoding.UTF8.GetBytes("username"));
            string password = "mypassword";

            Assert.Throws<FormatException>(() => { new CacheApiServiceOptions(url, username, password); });
            Assert.Throws<FormatException>(() => { new SignApiServiceOptions(url, username, password); });
        }
    }
}
