using SOS.OrderTracking.Web.Shared.CIT.Shipments;

namespace SOS.OrderTracking.Web.Common.StaticClasses
{
    public static class HTMLEmailFormats
    {
        public static string PasswordExpiryFormat(string username, string token, string expireDate)
        {
            return "<p style=\"text-align: center;\"><span style=\"color: rgb(255, 51, 51);\">Welcome to SOS Digital<br></span>" +
                "<span style=\"color: rgb(0, 0, 0);\">Where excellence is not an exception</span></p>\r\n" +
                "<p style=\"text-align: left;\"><span style=\"color: rgb(0, 0, 0);\">Dear Valued Customer</span><br></p>\r\n " +
                $"<p style=\"text-align: left;\">Your password for account <strong><span style='color:#1EA653;'>{username}&nbsp;</span>" +
                $"</strong>will expire on {expireDate}." +
                $"<a href='{token}' target=\"_blank\"><strong>Click here</strong></a> to change your password Now.</p>" +
                $"    <p>Please reach out to our Support Team if you have any questions at<br>" +
                $"<strong>digitalsupport@sospakistan.net&nbsp;</strong> or call <strong>&nbsp;our Help Centre +923000 341 900&nbsp;</strong> for more information.</p>\r\n" +
                "    <p><br></p>\r\n" +
                "    <p><img height='90' src = 'http://sosgroup.com.pk/Images/SOSLogo.png' /></p>\r\n" +
                "    <p>Thank you!<br><span style=\"color: rgb(255, 51, 51);\">SOS Digital Team</span></p>\r\n" +
                "    <p><br></p>\r\n";
        }

        public static string InTransitEmailFormat(ConsignmentReportViewModel report, string EmailFor)
        {
            return "<p style=\"text-align: center;\"><span style=\"color: rgb(255, 51, 51);\">Welcome to SOS Digital<br></span>" +
                "<span style=\"color: rgb(0, 0, 0);\">Where excellence is not an exception</span></p>\r\n" +
                "<p style=\"text-align: left;\"><span style=\"color: rgb(0, 0, 0);\">Dear Valued Customer</span><br>" +
                "<span style=\"color: rgb(0, 0, 0);\">Thank you for choosing SOS Pakistan." +
                $"We have {EmailFor} your order. You can now enjoy expanded access to Cash-in-Transit Services.</span></p>\r\n " +
                "<p style=\"text-align: left;\"><span style=\"color: rgb(0, 0, 0);\"><strong>The order details are as under:</strong></span>" +
                "</p>\r\n  " + ReceiptTable(report) +
                "    <p><img height='90' src = 'http://sosgroup.com.pk/Images/SOSLOGOV2.png' /></p>\r\n" +
                "    <p>Thank you!<br><span style=\"color: rgb(255, 51, 51);\">SOS Digital Team</span></p>\r\n" +
                "    <p><br></p>\r\n";
        }

        public static string ReceiptTable(ConsignmentReportViewModel report)
        {
            return "<table style=\"margin-left: auto; margin-right:auto; border-collapse: collapse; border: 1pt solid lightgrey;\">\r\n" +
                "<tbody>\r\n        " +
                "<tr>\r\n            " +
                "<td colspan=\"1\" style=\"padding: 0in; height: 14.75pt; vertical-align: top;\">\r\n" +
                $"<p style='margin-top:1.5pt;margin-bottom:.0001pt;text-align:left;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:15px; padding:1pt;\">SECURITY ORGANIZING SYSTEM PAKISTAN (PVT) LTD.</span></strong></p>\r\n" +
                "</td>\r\n" +
                "</tr>\r\n\r\n" +
                "<tr>\r\n            " +
                "<td colspan=\"1\" style=\"padding: 0in; height: 14.75pt; vertical-align: top; border-bottom: 1pt solid lightgrey;\">\r\n" +
                $"<p style='margin-top:1.5pt;margin-bottom:.0001pt;text-align:left;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:11px; padding:1pt;\">SHIPMENT RECIEPT No. {report.ShipmentRecieptNo}</span></strong></p>\r\n" +
                "</td>\r\n" +
                "</tr>\r\n\r\n" +
                "<tr>\r\n" +
                "<td style=\"padding: 0in;vertical-align: top;\">\r\n" +
                "<table>\r\n" +
                "<tbody>\r\n" +
                "<tr>\r\n" +
                "<td style=\"min-width: 85pt; width: 18%; padding: 1pt; border-bottom: 1px dotted #000; vertical-align: top; \">\r\n" +
                "<p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "<strong>\r\n" +
                "<span style=\"font-size: 11px; \">BILL TO </span>\r\n" +
                "</strong>\r\n" +
                "</p>\r\n" +
                "</td>\r\n\r\n" +
                "<td colspan=\"3\" style=\" min-width: 210pt; width: 40%; padding: 1pt; border-bottom: 1px solid #000; vertical-align: top;\">\r\n" +
                "<p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style='font-size: 11px; font-family: \"Arial MT\",\"sans-serif\";'>{report.CustomerToBeBilled}</span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\" min-width: 85pt; width: 22%; padding: 1pt; border-bottom: 1px dotted #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">VEHICLE NO.</span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\" min-width: 85pt; width: 20%; padding: 1pt; border-bottom: 1px solid #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style='font-size:11px;font-family:\"Arial MT\",\"sans-serif\";'>{report.VehicleNo}</span>\r\n" +
                "                                </p>\r\n\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n\r\n\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"width: 18%; padding: 1pt; border-bottom: 1px dotted #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">COLLECTION </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td colspan=\"3\" style=\"width: 40%; padding: 1pt; border-bottom: 1px solid #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style='font-size:11px;font-family:\"Arial MT\",\"sans-serif\";'>{report.PickupBranch}</span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\"width: 22%; padding: 1pt; border-bottom: 1px dotted #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Creation date/time</span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\"width: 20%; padding: 1pt; border-bottom: 1px solid #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style='font-size:11px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Date}</span>\r\n" +
                "                                </p>\r\n\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"width: 18%; padding: 1pt; border-bottom: 1px dotted #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Delivery </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td colspan=\"3\" style=\"width: 40%; padding: 1pt; border-bottom: 1px solid #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style='font-size:11px;font-family:\"Arial MT\",\"sans-serif\";'>{report.DeliveryBranch}</span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\"width: 22%; padding: 1pt; border-bottom: 1px dotted #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Collection date/time</span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\"width: 20%; padding: 1pt; border-bottom: 1px solid #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style='font-size:11px;font-family:\"Arial MT\",\"sans-serif\";'>{report.PickupTime}</span>\r\n" +
                "                                </p>\r\n\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"width: 18%; padding: 1pt; border-bottom: 1px dotted #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Waiting Time </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\"width: 13%; padding: 1pt; border-bottom: 1px solid #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <span style='font-size:11px;font-family:\"Arial MT\",\"sans-serif\";'>_</span>\r\n" +
                "                                </p>\r\n\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"width: 13%; padding: 1pt; border-bottom: 1px solid #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <span style='font-size:11px;font-family:\"Arial MT\",\"sans-serif\";'>Seals</span>\r\n" +
                $"                                    <strong><span style=\"font-size:11px;\">{report.NoOfSeals}</span></strong>\r\n" +
                "                                </p>\r\n\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"width: 13%; padding: 1pt; border-bottom: 1px solid #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <span style='font-size:11px;font-family:\"Arial MT\",\"sans-serif\";'>Bags</span>\r\n" +
                $"                                    <strong><span style=\"font-size:11px;\">{report.NoOfBags}</span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\"width: 22%; padding: 1pt; border-bottom: 1px dotted #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Delivery date/time </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\"width: 20%; padding: 1pt; border-bottom: 1px solid #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style='font-size:11px;font-family:\"Arial MT\",\"sans-serif\";'>{report.DeliveryTime}</span>\r\n" +
                "                                </p>\r\n\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"width: 18%; padding: 1pt; border-bottom: 1px dotted #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Seal No </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td colspan=\"5\" style=\"width: 40%; padding: 1pt; border-bottom: 1px solid #000; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style='font-size:11px;font-family:\"Arial MT\",\"sans-serif\";'>{report.SealNo}</span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                        </tr>\r\n\r\n" +
                "                    </tbody>\r\n" +
                "                </table>\r\n\r\n" +
                "                <table style=\"width: 100%; border-collapse: collapse; margin-top: 5px; \">\r\n" +
                "                    <tbody>\r\n" +
                "                        <tr>\r\n" +
                "                            <td colspan=\"2\" style=\"padding: 2pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: center; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size: 13px;\">Consigned By </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td colspan=\"2\" style=\"padding: 2pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: center; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size: 13px;\">Accepted By SOS</span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td colspan=\"2\" style=\"padding: 2pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: center; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:13px;\">Received By</span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"width: 16.6%; padding: 2pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Name </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\"width: 16.6%; padding: 2pt; border: 1pt solid lightgrey; vertical-align: top; \">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Stamp &amp; Signature </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"width: 16.6%; padding: 2pt; border: 1pt solid lightgrey; vertical-align: top; \">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Name </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\"width: 16.6%; padding: 2pt; border: 1pt solid lightgrey; vertical-align: top; \">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Stamp &amp; Signature </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"width: 16.6%; padding: 2pt; border: 1pt solid lightgrey; vertical-align: top; \">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Name </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n\r\n" +
                "                            <td style=\"width:16.6%;padding: 2pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <strong><span style=\"font-size:11px;\">Stamp &amp; Signature </span></strong>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style=\"font-size:11px;\">{report.ConsignedByName1} </span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <span style=\"font-size:11px;\"></span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style=\"font-size:11px;\">{report.AcceptedByName1}</span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <span style=\"font-size:11px;\"> </span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style=\"font-size:11px;\">{report.RecievedByName1} </span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <span style=\"font-size:11px;\"> </span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style=\"font-size:11px;\">{report.ConsignedByName2} </span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <span style=\"font-size:11px;\"> </span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style=\"font-size:11px;\">{report.AcceptedByName2} </span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <span style=\"font-size:11px;\"> </span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                    <span style=\"font-size:11px;\">{report.RecievedByName2} </span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\"padding: 1pt; border: 1pt solid lightgrey; vertical-align: top;\">\r\n" +
                "                                <p style='margin-top: .05pt; margin-bottom: .0001pt; text-align: left; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                "                                    <span style=\"font-size:11px;\"> </span>\r\n" +
                "                                </p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n\r\n" +
                "                    </tbody>\r\n" +
                "                </table>\r\n" +
                "            </td>\r\n\r\n" +
                "            <td style=\"padding: 0in; vertical-align: top;\">\r\n" + GetDenomTable(report) +
                "            </td>\r\n" +
                "        </tr>\r\n\r\n" +
                "    </tbody>\r\n</table>";
        }

        private static string GetDenomTable(ConsignmentReportViewModel report)
        {
            if (report.CurrencySymbol_ == Shared.Enums.CurrencySymbol.MixCurrency || report.CurrencySymbol_ == Shared.Enums.CurrencySymbol.Other)
                return MixCurrencyDenom(report);
            else if (report.CurrencySymbol_ == Shared.Enums.CurrencySymbol.PrizeBond)
                return PrizebondDenom(report);
            else
                return PkrDenom(report);
        }
        private static string MixCurrencyDenom(ConsignmentReportViewModel report)
        {
            return $"<p style=\"\r\n    border: 1pt solid lightgrey;\r\n    white-space: pre-wrap;\r\n    width: 180px;\r\n    margin-top: 0pt;\r\n    margin-right: 0.85pt;\r\n    margin-bottom: 0.0001pt;\r\n    margin-left: 0in;\r\n    text-align: center;\r\n    font-size: 15px;\r\n    font-family: &quot;Arial&quot;,&quot;sans-serif&quot;;\r\n    min-height: 200px;\r\n\"><span style=\"font-size:9px\">{report.Valueables}</span></p>" +
                "<p style=\"width:180px;margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:&quot;Arial&quot;,&quot;sans-serif&quot;\">" +
                "<strong>" +
                $"<span style=\"font-size:9px; white-space: pre-wrap;\">{report.AmountInWords}</span>" +
                "</strong>" +
                "</p>";
        }
        private static string PrizebondDenom(ConsignmentReportViewModel report)
        {
            return "                <table style=\"width:100%; border-collapse: collapse;\">\r\n" +
                            "                    <tbody>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td colspan=\"4\" style='border: 1pt solid lightgrey; margin-top: 2.5pt; margin-bottom: .0001pt; text-align: center; font-size: 15px; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                            $"                                <span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>DENOMINATION {report.AmountType}</span>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>Qty</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            "                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">Denom</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            "                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">Prize</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>Total</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency40000x}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            "                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x40000</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">+{report.PrizeMoney40000x}</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency40000xAmount}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency25000x}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            "                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x25000</span></strong></p>\r\n" +
                            "                            </td>\r\n" +

                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">+{report.PrizeMoney25000x}</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency25000xAmount}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency15000x}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            "                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x15000</span></strong></p>\r\n" +
                            "                            </td>\r\n" +

                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">+{report.PrizeMoney15000x}</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency15000xAmount}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency7500x}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            "                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x7500</span></strong></p>\r\n" +
                            "                            </td>\r\n" +

                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">+{report.PrizeMoney7500x}</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency7500xAmount}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency1500x}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            "                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x1500</span></strong></p>\r\n" +
                            "                            </td>\r\n" +

                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">+{report.PrizeMoney1500x}</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency1500xAmount}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency750x}</span></p>\r\n" +
                            "                            </td>\r\n " +
                            "                           <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            "                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x750</span></strong></p>\r\n" +
                            "                            </td>\r\n" +

                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">+{report.PrizeMoney750x}</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency750xAmount}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency200x}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            "                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x200</span></strong></p>\r\n" +
                            "                            </td>\r\n" +

                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">+{report.PrizeMoney200x}</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency200xAmount}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency100x}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            "                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x100</span></strong></p>\r\n" +
                            "                            </td>\r\n" +

                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">+{report.PrizeMoney100x}</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency100xAmount}</span></p>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                            "                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">Total</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                            <td colspan=\"3\" style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">{report.CurrencySymbol} {report.Amount}</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n" +
                            "                        <tr>\r\n" +
                            "                            <td colspan=\"4\" style=\" min-width: 60px; padding: 0in; vertical-align: top;\">\r\n" +
                            $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">{report.AmountInWords}</span></strong></p>\r\n" +
                            "                            </td>\r\n" +
                            "                        </tr>\r\n\r\n" +
                            "                    </tbody>\r\n" +
                            "                </table>\r\n";
        }
        private static string PkrDenom(ConsignmentReportViewModel report)
        {
            return "                <table style=\"width:100%; border-collapse: collapse;\">\r\n" +
                "                    <tbody>\r\n" +
                "                        <tr>\r\n" +
                "                            <td colspan=\"3\" style='border: 1pt solid lightgrey; margin-top: 2.5pt; margin-bottom: .0001pt; text-align: center; font-size: 15px; font-family: \"Arial\",\"sans-serif\";'>\r\n" +
                $"                                <span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>DENOMINATION {report.AmountType}</span>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency5000x}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x&nbsp;{report.CurrencySymbol}&nbsp;5000=</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency5000xAmount}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency1000x}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x&nbsp;{report.CurrencySymbol}&nbsp;1000=</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency1000xAmount}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency500x}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x&nbsp;{report.CurrencySymbol}&nbsp;500=</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency500xAmount}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency100x}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x&nbsp;{report.CurrencySymbol}&nbsp;100=</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency100xAmount}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency75x}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x&nbsp;{report.CurrencySymbol}&nbsp;75=</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency75xAmount}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency50x}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x&nbsp;{report.CurrencySymbol}&nbsp;50=</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency50xAmount}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency20x}</span></p>\r\n" +
                "                            </td>\r\n " +
                "                           <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x&nbsp;{report.CurrencySymbol}&nbsp;20=</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency20xAmount}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency10x}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x&nbsp;{report.CurrencySymbol}&nbsp;10=</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency10xAmount}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency5x}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x&nbsp;{report.CurrencySymbol}&nbsp;5=</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency5xAmount}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency2x}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x&nbsp;{report.CurrencySymbol}&nbsp;2=</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency2xAmount}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency1x}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">x&nbsp;{report.CurrencySymbol}&nbsp;1=</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><span style='font-size:9px;font-family:\"Arial MT\",\"sans-serif\";'>{report.Currency1xAmount}</span></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td style=\"min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top; \">\r\n" +
                "                                <p style='margin-top:1.5pt;margin-right:.95pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">Total</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                            <td colspan=\"2\" style=\" min-width: 60px; border: 1pt solid lightgrey; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px;\">{report.CurrencySymbol} {report.Amount}</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n" +
                "                        <tr>\r\n" +
                "                            <td colspan=\"3\" style=\" min-width: 60px; padding: 0in; vertical-align: top;\">\r\n" +
                $"                                <p style='margin-top:1.5pt;margin-right:.85pt;margin-bottom:.0001pt;margin-left:0in;text-align:right;font-size:15px;font-family:\"Arial\",\"sans-serif\";'><strong><span style=\"font-size:9px; white-space: pre-wrap;\">{report.AmountInWords.Replace("\r","")}</span></strong></p>\r\n" +
                "                            </td>\r\n" +
                "                        </tr>\r\n\r\n" +
                "                    </tbody>\r\n" +
                "                </table>\r\n";
        }
    }
}
