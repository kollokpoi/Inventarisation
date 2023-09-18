using Inventarisation.Interfaces;
using Inventarisation.Services;
using Inventarisation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace Inventarisation.Controllers
{
    public class ScanController : Controller
    {
        private readonly IHubContext<ScanHub> _hubContext;
        private IBDWork bDWork;
        // GET: ScanController1
        public ScanController(IHubContext<ScanHub> hubContext,IBDWork bDWork)
        {
            _hubContext = hubContext;
            this.bDWork = bDWork;
        }
        /// <summary>
        /// Страница сканирования
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            Scan();
            return View();
        }
        /// <summary>
        /// Начало скана
        /// </summary>
        [HttpPost]
        public async void Scan()
        {
           string ipAddressBase = "192.168.1.";
           int startRange = 1;
           int endRange = 6;

           for (int i = startRange; i <= endRange; i++)
           {
               string ipAddress = ipAddressBase + i.ToString();
               string macAddress = GetMacAddress(ipAddress);

               if (!string.IsNullOrEmpty(macAddress))
               {
                    PingViewModel pingViewModel = new PingViewModel();
                    pingViewModel.Ip = ipAddress;
                    pingViewModel.Mac= macAddress;

                    pingViewModel.equipment = await bDWork.GetEquipmentSettings(ipAddress);
                   await _hubContext.Clients.All.SendAsync("ReceiveScanResult", JsonSerializer.Serialize(pingViewModel));
                   await Task.Delay(TimeSpan.FromSeconds(5));
                }
           }
        }
       
        public static string GetMacAddress(string ipAddress)
        {
            try
            {
                var arp = new ArpUtility();
                string macAddress = arp.GetMacAddress(ipAddress);
                return macAddress;
            }
            catch (Exception)
            {
                // Обработка ошибок при получении MAC-адреса
                return null;
            }
        }
        public class ArpUtility
        {
            public string GetMacAddress(string ipAddress)
            {
                IPAddress target = IPAddress.Parse(ipAddress);
                byte[] macAddr = new byte[6];
                int macAddrLen = macAddr.Length;
                uint destIp = BitConverter.ToUInt32(target.GetAddressBytes(), 0);

                if (SendARP(destIp, 0, macAddr, ref macAddrLen) == 0)
                {
                    string macAddress = string.Join(":", macAddr
                        .Take(macAddrLen)
                        .Select(b => b.ToString("X2")));
                    return macAddress;
                }

                return null;
            }

            [System.Runtime.InteropServices.DllImport("iphlpapi.dll", ExactSpelling = true)]
            public static extern int SendARP(uint DestIP, uint SrcIP, byte[] pMacAddr, ref int PhyAddrLen);
        }
    }
}
