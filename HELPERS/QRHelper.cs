using System;
using QRCoder;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;

namespace HYDSWMAPI.HELPERS
{
    public class QRHelper
    {
        public static Bitmap VehicleQrCodeTest(string PBID)
        {
            string QrCode = Convert.ToString(PBID);

            Bitmap bmp = null;
            string level = "L";
            QRCodeGenerator.ECCLevel eccLevel = (QRCodeGenerator.ECCLevel)(level == "L" ? 0 : level == "M" ? 1 : level == "Q" ? 2 : 3);
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {

                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(QrCode, eccLevel))
                {
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        bmp = qrCode.GetGraphic(10, Color.Black, Color.White, null, 5, 2, true);
                        Graphics graphicImage = Graphics.FromImage(bmp);
                        graphicImage.SmoothingMode = SmoothingMode.HighQuality;

                        //graphicImage.DrawString("UID: " + QrCode, new Font("Arial", 12, FontStyle.Bold), SystemBrushes.WindowText, new Point(0, 2));
                    }
                }
            }
            return bmp;
        }
    }
}
