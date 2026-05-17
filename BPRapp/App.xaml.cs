using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Authentication;

namespace BPRapp
{
    public partial class App : Application
    {
        public App()
        {
            // 🔥 ВАЖНО: Устанавливаем TLS 1.2 ДО любого сетевого запроса
            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls12 |
                System.Net.SecurityProtocolType.Tls11 |
                System.Net.SecurityProtocolType.Tls;
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (s, cert, chain, sslPolicyErrors) => true;
        }
    }
}