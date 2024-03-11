using System.Text;

namespace hallocDoc.Controllers
{
    public static class password
    {

        public static string encry(string pass)
        {
            if (pass == null)
            {
                return null;
            }
            else
            {
                byte[] storePass = ASCIIEncoding.ASCII.GetBytes(pass);
                string encryptedPass = Convert.ToBase64String(storePass);
                return encryptedPass;
            }
            
        }

        public static string decry(string pass)
        {
            if(pass == null)
            {
                return null;
            }
            else
            {
                byte[] encryptedPass = Convert.FromBase64String(pass);
                string decryptedPass = ASCIIEncoding.ASCII.GetString(encryptedPass);
                return decryptedPass;
            }
        }
    }
}
