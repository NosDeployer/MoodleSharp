using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web; // Agigunta a mano reference a System.Web
using Newtonsoft.Json;


/*
    Riferimenti:

    https://moodle.org/mod/forum/discuss.php?d=210866
    http://cipcnet.insa-lyon.fr/Members/ppollet/public/moodlews/
    http://forums.asp.net/t/1814455.aspx?Trouble+consuming+Moodle+web+services+from+ASP+NET+website

    http://moodle.apexnet.it/webservice/soap/server.php?wsdl=1&wstoken=c9554589d32bd8afbda4ca87119e75ad
    http://www.azhowto.com/creating-a-moodle-user-via-web-services/
    http://www.moodletips.com/soap-web-services-wsdl
    https://delog.wordpress.com/2010/08/31/integrating-a-c-app-with-moodle-using-xml-rpc/
    https://docs.moodle.org/dev/Web_services

    http://moodle.apexnet.it/login/token.php?username=admin&password=password-999&service=moodle_mobile_app (questo e' un servizio)
 
    https://github.com/zikzakmedia/python-moodle
 
 */

 

namespace MoodleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            String token = "7e708f0bf5b3911ace4e4922d220fbb0x";

            string base_url = @"http://moodle.apexnet.it";

            MoodleUser user = new MoodleUser();
            user.username = HttpUtility.UrlEncode("mrossi");
            user.password = HttpUtility.UrlEncode("fakepassword");
            user.firstname = HttpUtility.UrlEncode("Mario");
            user.lastname = HttpUtility.UrlEncode("Rossi");
            user.email = HttpUtility.UrlEncode("mariorossi@wedo-fake.com");

            List<MoodleUser> userList = new List<MoodleUser>();
            userList.Add(user);

            Array arrUsers = userList.ToArray();

            String postData = String.Format("users[0][username]={0}&users[0][password]={1}&users[0][firstname]={2}&users[0][lastname]={3}&users[0][email]={4}",  user.username, user.password, user.firstname, user.lastname, user.email);

            string createRequest = string.Format(base_url + "/webservice/rest/server.php?wstoken={0}&wsfunction={1}&moodlewsrestformat=json", token, "core_user_create_users");

            // Call Moodle REST Service
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(createRequest);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            // Encode the parameters as form data:
            byte[] formData = UTF8Encoding.UTF8.GetBytes(postData);
            req.ContentLength = formData.Length;

            // Write out the form Data to the request:
            using (Stream post = req.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }


            // Get the Response
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream resStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            string contents = reader.ReadToEnd();

            // Deserialize
            JsonSerializer serializer = new JsonSerializer();

            if (contents.Contains("exception"))
            {
                // Error
                MoodleException moodleError = JsonConvert.DeserializeObject<MoodleException>(contents);
            }
            else
            {
                // Success
                List<MoodleCreateUserResponse> newUsers = JsonConvert.DeserializeObject<List<MoodleCreateUserResponse>>(contents);
            }
            
        }

        public class MoodleUser
        {
            public string username { get; set; }
            public string password { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string email { get; set; }
        }

        public class MoodleCreateUserResponse
        {
            public string id { get; set; }
            public string username { get; set; }
        }

        public class MoodleException
        {
            public string exception { get; set; }
            public string errorcode { get; set; }
            public string message { get; set; }
            public string debuginfo { get; set; }
        }

   
    }
}