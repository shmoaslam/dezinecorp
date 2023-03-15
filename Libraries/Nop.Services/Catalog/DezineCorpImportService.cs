
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using MaxMind.GeoIP2.Model;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace Nop.Services.Catalog
{

    public class ImportStatus
    {
        public int TotalRecord { get; set; }
        public int RecordProcessSucessfully { get; set; }
        public string RecordFailed { get; set; }

        public bool IsImportFinish { get; set; }
    }

    public class DezineCorpImportService
    {
        private string _path;

        public DezineCorpImportService(string path)
        {
            _path = path;
        }


        public void UpdateStatus(ImportStatus status)
        {
            string importStatusQuery = "delete from ImportStatus;insert into ImportStatus values('" + status.TotalRecord + "', '" + status.RecordProcessSucessfully + "', '" + status.RecordFailed + "', '" + status.IsImportFinish + "')";

            SqlCommand cmd = new SqlCommand(importStatusQuery);

            ExecuteQuery(cmd);
        }


        public ImportStatus GetImportStatus()
        {
            string importQuery = "Select * from ImportStatus";

            SqlCommand cmd = new SqlCommand(importQuery);

            string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ConnectionString;
            SqlConnection con = new SqlConnection(strConnString);
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return new ImportStatus
                        {
                            TotalRecord = Convert.ToInt32(reader["TotalRecord"]),
                            RecordProcessSucessfully = Convert.ToInt32(reader["RecordProcessSucessfully"]),
                            IsImportFinish = Convert.ToBoolean(reader["ImportFinish"]),
                            RecordFailed = FormatHtml(reader["RecordFailed"].ToString())
                        };
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
            return null;

        }

        private string FormatHtml(string v)
        {
            if (string.IsNullOrEmpty(v))
            {
                return null;
            }
            var resultHtml = string.Empty;
            var list = v.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            resultHtml += "<table border='1'  style='width:100%;'><tr><th style='width:30%'>SKU</th><th  style='width:70%'>Error Message</th></tr>";
            foreach (var item in list)
            {
                var content = item.Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
                if (content.Length >= 2)
                {
                    resultHtml += "<tr>";
                    resultHtml += "<td>" + content[0] + "</td><td>" + content[1] + "</td>";
                    resultHtml += "</tr>";
                }
                else
                {
                    resultHtml += "<tr>";
                    resultHtml += "<td> </td><td>" + content[0] + "</td>";
                    resultHtml += "</tr>";

                }
            }
            resultHtml += "</table>";
            return resultHtml;
        }

        public int ExcelColumnNameToNumber(string columnName)
        {
            try
            {
                if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");
                columnName = columnName.ToUpperInvariant();
                int sum = 0;
                for (int i = 0; i < columnName.Length; i++)
                {
                    sum *= 26;
                    sum += (columnName[i] - 'A' + 1);
                }
                return sum;

            }
            catch (Exception ex)
            {
                throw new Exception("Error in reading column " + columnName, ex);
            }
        }


        private string GetExcelColumnName(int columnNumber)
        {
            try
            {
                int dividend = columnNumber;
                string columnName = String.Empty;
                int modulo;

                while (dividend > 0)
                {
                    modulo = (dividend - 1) % 26;
                    columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                    dividend = (int)((dividend - modulo) / 26);
                }

                return columnName;

            }
            catch (Exception ex)
            {
                return columnNumber.ToString();
            }
        }

        public string GetCellValue(ICell cell)
        {
            try
            {
                if (cell == null)
                {
                    return string.Empty;
                }
                if (cell.CellType == CellType.Numeric)
                {
                    return cell.NumericCellValue.ToString();
                }
                else if (cell.CellType == CellType.Formula)
                {
                    if (cell.CachedFormulaResultType == CellType.Error)
                    {
                        throw new Exception("Error in reading formula cell for column " + GetExcelColumnName(cell.ColumnIndex + 1));
                    }
                    else if (cell.CachedFormulaResultType == CellType.Numeric)
                    {
                        return cell.NumericCellValue.ToString();
                    }
                    else
                    {
                        if (cell.StringCellValue == null)
                        {
                            return string.Empty;
                        }
                        else
                        {
                            return cell.StringCellValue.Trim();
                        }
                    }
                }
                else
                {
                    if (cell.StringCellValue == null)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return cell.StringCellValue.Trim();
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public void READExcel()
        {


            ////Instance reference for Excel Application
            //Microsoft.Office.Interop.Excel.Application objXL = null;
            ////Workbook refrence
            //Microsoft.Office.Interop.Excel.Workbook objWB = null;
            ImportStatus status = new ImportStatus();

            try
            {
                FileStream stream = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite);
                HSSFWorkbook wb = new HSSFWorkbook(stream);
                ISheet ws = wb.GetSheet("MASTER");
                int startFromRowNumber = 3; // starting from 0, ignore first 3 rows
                int rows = ws.LastRowNum;
                status.TotalRecord = rows - 2;
                status.IsImportFinish = false;
                string skus = string.Empty;
                int skucount = 0;
                //File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), status.TotalRecord.ToString());

                //Instancing Excel using COM services
                //objXL = new Microsoft.Office.Interop.Excel.Application();
                //Adding WorkBook
                if (string.IsNullOrEmpty(_path)) return;

                //objWB = objXL.Workbooks.Open(_path);

                //Microsoft.Office.Interop.Excel.Worksheet objSHT = objWB.Sheets["MASTER"];
                //int rows = objSHT.UsedRange.Rows.Count;
                //int cols = objSHT.UsedRange.Columns.Count;

                List<string> skusFromSheet = new List<string>();



                //throw new Exception("Custom Message");


                var colors = GetColorValueFromColorCodes();
                //File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), "color extracted");

                for (int r = startFromRowNumber; r <= rows; r++)
                {
                    //File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), "inside product iteration 1" + Environment.NewLine);

                    var sku = string.Empty;
                    try
                    {
                        var row = ws.GetRow(r);
                        if (row == null)
                        {
                            continue;
                        }
                        //File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), "inside product iteration 2" + Environment.NewLine);

                        sku = GetCellValue(row.GetCell(ExcelColumnNameToNumber("A") - 1));
                        var product = GetProductBySKU(sku);
                        skusFromSheet.Add(sku);

                        //File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), "inside product iteration 3" + Environment.NewLine);

                        //File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), sku);

                        //File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), (product == null).ToString());

                        //File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), (product.Rows.Count).ToString());

                        if (product == null)
                            continue;

                        if (product.Rows.Count <= 0)
                            continue;

                        //File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), "inside product iteration 4" + Environment.NewLine);


                        int productid = Convert.ToInt32(product.Rows[0]["Id"].ToString());

                        if (productid == 0)
                            continue;

                        //File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), "processing " + productid.ToString() + " sku " +  sku);



                        // code to set color and image in the master sheet
                        try
                        {

                            var colorCodes = GetColorCodeFromSku(sku);

                            var color_1 = string.Empty;
                            var color_2 = string.Empty;
                            if (colorCodes.Any())
                            {
                                if (colorCodes.Count() >= 2)
                                {
                                    color_1 = colors.FirstOrDefault(x => x.Key == colorCodes[0]).Value;
                                    color_2 = colors.FirstOrDefault(x => x.Key == colorCodes[1]).Value;

                                }
                                if (colorCodes.Count() >= 1)
                                {
                                    color_1 = colors.FirstOrDefault(x => x.Key == colorCodes[0]).Value;
                                }
                            }


                            if (!string.IsNullOrEmpty(color_1))
                            {
                                var column_index = ExcelColumnNameToNumber("EZ") - 1;
                                var color1_cell = row.GetCell(column_index);
                                if (color1_cell == null)
                                {
                                    var cell = row.CreateCell(column_index);
                                    cell.SetCellValue(color_1);
                                }
                                else
                                {
                                    color1_cell.SetCellValue(color_1);
                                }
                            }
                            if (!string.IsNullOrEmpty(color_2))
                            {
                                var column_index = ExcelColumnNameToNumber("FA") - 1;
                                var color2_cell = row.GetCell(column_index);
                                if (color2_cell == null)
                                {
                                    var cell = row.CreateCell(column_index);
                                    cell.SetCellValue(color_2);
                                }
                                else
                                {
                                    color2_cell.SetCellValue(color_2);
                                }
                            }

                            var pictureUrl = GetPictures(productid);
                            if (!string.IsNullOrEmpty(pictureUrl))
                            {
                                var column_index = ExcelColumnNameToNumber("FB") - 1;
                                var imageUrl_Cell = row.GetCell(column_index);
                                if (imageUrl_Cell == null)
                                {
                                    var cell = row.CreateCell(column_index);
                                    cell.SetCellValue(pictureUrl);
                                }
                                else
                                {
                                    imageUrl_Cell.SetCellValue(pictureUrl);
                                }
                            }
                        }
                        catch
                        {

                        }




                        #region fetch excel row cell

                        bool isChangesMade = false;

                        string NewPage = GetCellValue(row.GetCell(ExcelColumnNameToNumber("C") - 1));
                        string eNewPage = product.Rows[0]["NewPage"].ToString();
                        if (NewPage != eNewPage)
                            isChangesMade = true;


                        string ShortDescription = GetCellValue(row.GetCell(ExcelColumnNameToNumber("D") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["ShortDescription"].ToString();
                            if (ShortDescription != e)
                                isChangesMade = true;
                        }

                        string ItemIsNew = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AI") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["ItemIsNew"].ToString();
                            if (ItemIsNew != e)
                                isChangesMade = true;
                        }

                        string GuarenteedStock = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AJ") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["GuarenteedStock"].ToString();
                            if (GuarenteedStock != e)
                                isChangesMade = true;
                        }

                        string Materials = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AK") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Materials"].ToString();
                            if (Materials != e)
                                isChangesMade = true;
                        }

                        string Features = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AM") - 1));
                        if (isChangesMade == false)
                        {
                            string existingFeatures = product.Rows[0]["Features"].ToString();
                            if (Features != existingFeatures)
                                isChangesMade = true;
                        }

                        string Includes = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AN") - 1));
                        if (isChangesMade == false)
                        {
                            string existingIncludes = product.Rows[0]["Includes"].ToString();
                            if (Includes != existingIncludes)
                                isChangesMade = true;
                        }

                        string ShortName = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AL") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Name"].ToString();
                            if (ShortName != e)
                                isChangesMade = true;
                        }

                        string SpecailPackaging = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AO") - 1));
                        if (isChangesMade == false)
                        {
                            string existingSpecailPackaging = product.Rows[0]["SpecailPackaging"].ToString();
                            if (SpecailPackaging != existingSpecailPackaging)
                                isChangesMade = true;
                        }

                        string Capacity = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AP") - 1));
                        if (isChangesMade == false)
                        {
                            string existingCapacity = product.Rows[0]["Capacity"].ToString();
                            if (Capacity != existingCapacity)
                                isChangesMade = true;
                        }

                        string Size = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AQ") - 1));
                        if (isChangesMade == false)
                        {
                            string existingSize = product.Rows[0]["Size"].ToString();
                            if (Size != existingSize)
                                isChangesMade = true;
                        }

                        string ImprintAreaInOutboard = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AR") - 1));
                        if (isChangesMade == false)
                        {
                            string existingImprintAreaInOutboard = product.Rows[0]["ImprintAreaInOutboard"].ToString();
                            if (ImprintAreaInOutboard != existingImprintAreaInOutboard)
                                isChangesMade = true;
                        }

                        string ImprintAreaWrapAround = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AS") - 1));
                        if (isChangesMade == false)
                        {
                            string existingImprintAreaWrapAround = product.Rows[0]["ImprintAreaWrapAround"].ToString();
                            if (ImprintAreaWrapAround != existingImprintAreaWrapAround)
                                isChangesMade = true;
                        }

                        string DecoratingOption = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AT") - 1));
                        if (isChangesMade == false)
                        {
                            string existingDecoratingOption = product.Rows[0]["DecoratingOption"].ToString();
                            if (DecoratingOption != existingDecoratingOption)
                                isChangesMade = true;
                        }

                        string PeicesPerCartoon = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AU") - 1));
                        if (isChangesMade == false)
                        {
                            string existingPeicesPerCartoon = product.Rows[0]["PeicesPerCartoon"].ToString();
                            if (PeicesPerCartoon != existingPeicesPerCartoon)
                                isChangesMade = true;
                        }

                        string WeightPerCartoon = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AV") - 1));
                        if (isChangesMade == false)
                        {
                            string existingWeightPerCartoon = product.Rows[0]["WeightPerCartoon"].ToString();
                            if (WeightPerCartoon != existingWeightPerCartoon)
                                isChangesMade = true;
                        }

                        string BlankLine = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AW") - 1));
                        if (isChangesMade == false)
                        {
                            string existingBlankLine = product.Rows[0]["BlankLine"].ToString();
                            if (BlankLine != existingBlankLine)
                                isChangesMade = true;
                        }

                        string ProtectivePackaging = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AX") - 1));
                        if (isChangesMade == false)
                        {
                            string existingProtectivePackaging = product.Rows[0]["ProtectivePackaging"].ToString();
                            if (ProtectivePackaging != existingProtectivePackaging)
                                isChangesMade = true;
                        }

                        string ReferToCataloguePage = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AY") - 1));
                        if (isChangesMade == false)
                        {
                            string existingReferToCataloguePage = product.Rows[0]["ReferToCataloguePage"].ToString();
                            if (ReferToCataloguePage != existingReferToCataloguePage)
                                isChangesMade = true;
                        }

                        string PricingFlag = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AZ") - 1));
                        if (isChangesMade == false)
                        {
                            string existingPricingFlag = product.Rows[0]["PricingFlag"].ToString();
                            if (PricingFlag != existingPricingFlag)
                                isChangesMade = true;
                        }

                        string MadeinCanada = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BA") - 1));
                        if (isChangesMade == false)
                        {
                            string existingMadeinCanada = product.Rows[0]["MadeinCanada"].ToString();
                            if (MadeinCanada != existingMadeinCanada)
                                isChangesMade = true;
                        }

                        string MadeinNorthAmerica = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BB") - 1));
                        if (isChangesMade == false)
                        {
                            string existingMadeinNorthAmerica = product.Rows[0]["MadeinNorthAmerica"].ToString();
                            if (MadeinNorthAmerica != existingMadeinNorthAmerica)
                                isChangesMade = true;
                        }

                        string InventoryFlag = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BC") - 1));
                        if (isChangesMade == false)
                        {
                            string existingInventoryFlag = product.Rows[0]["InventoryFlag"].ToString();
                            if (InventoryFlag != existingInventoryFlag)
                                isChangesMade = true;
                        }

                        string PricingCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BD") - 1));
                        if (isChangesMade == false)
                        {
                            string existingPricingCode = product.Rows[0]["PricingCode"].ToString();
                            if (PricingCode != existingPricingCode)
                                isChangesMade = true;
                        }

                        string PricingFooterNote = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BW") - 1));
                        if (isChangesMade == false)
                        {
                            string existingPricingFooterNote = product.Rows[0]["PricingFooterNote"].ToString();
                            if (PricingFooterNote != existingPricingFooterNote)
                                isChangesMade = true;
                        }

                        string SetupPerColour = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BX") - 1));
                        if (isChangesMade == false)
                        {
                            string existingSetupPerColour = product.Rows[0]["SetupPerColour"].ToString();
                            if (SetupPerColour != existingSetupPerColour)
                                isChangesMade = true;
                        }

                        string RepeatSetup = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BY") - 1));
                        if (isChangesMade == false)
                        {
                            string existingRepeatSetup = product.Rows[0]["RepeatSetup"].ToString();
                            if (RepeatSetup != existingRepeatSetup)
                                isChangesMade = true;
                        }

                        string DebossSetup = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BZ") - 1));
                        if (isChangesMade == false)
                        {
                            string existingDebossSetup = product.Rows[0]["DebossSetup"].ToString();
                            if (DebossSetup != existingDebossSetup)
                                isChangesMade = true;
                        }

                        string RepeatDeboss = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CA") - 1));
                        if (isChangesMade == false)
                        {
                            string existingRepeatDeboss = product.Rows[0]["RepeatDeboss"].ToString();
                            if (RepeatDeboss != existingRepeatDeboss)
                                isChangesMade = true;
                        }

                        string DecalSetup = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CB") - 1));
                        if (isChangesMade == false)
                        {
                            string existingDecalSetup = product.Rows[0]["DecalSetup"].ToString();
                            if (DecalSetup != existingDecalSetup)
                                isChangesMade = true;
                        }

                        string RepeatDecal = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CC") - 1));
                        if (isChangesMade == false)
                        {
                            string existingRepeatDecal = product.Rows[0]["RepeatDecal"].ToString();
                            if (RepeatDecal != existingRepeatDecal)
                                isChangesMade = true;
                        }

                        string LaserSetup = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CD") - 1));
                        if (isChangesMade == false)
                        {
                            string eLaserSetup = product.Rows[0]["LaserSetup"].ToString();
                            if (LaserSetup != eLaserSetup)
                                isChangesMade = true;
                        }

                        string RepeatLaser = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CE") - 1));
                        if (isChangesMade == false)
                        {
                            string eRepeatLaser = product.Rows[0]["RepeatLaser"].ToString();
                            if (RepeatLaser != eRepeatLaser)
                                isChangesMade = true;
                        }

                        string AdditionalCharge1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CF") - 1));
                        if (isChangesMade == false)
                        {
                            string eAdditionalCharge1 = product.Rows[0]["AdditionalCharge1"].ToString();
                            if (AdditionalCharge1 != eAdditionalCharge1)
                                isChangesMade = true;
                        }

                        string AdditionalCharge2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CG") - 1));
                        if (isChangesMade == false)
                        {
                            string eAdditionalCharge2 = product.Rows[0]["AdditionalCharge2"].ToString();
                            if (AdditionalCharge2 != eAdditionalCharge2)
                                isChangesMade = true;
                        }

                        string AdditionalCharge3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CH") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["AdditionalCharge3"].ToString();
                            if (AdditionalCharge3 != e)
                                isChangesMade = true;
                        }

                        string AdditionalCharge4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CI") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["AdditionalCharge4"].ToString();
                            if (AdditionalCharge4 != e)
                                isChangesMade = true;
                        }

                        string RepeatTerm = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CJ") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["RepeatTerm"].ToString();
                            if (RepeatTerm != e)
                                isChangesMade = true;
                        }

                        string FinalNote = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CK") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["FinalNote"].ToString();
                            if (FinalNote != e)
                                isChangesMade = true;
                        }

                        string RegularPrice = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CU") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["RegularPrice"].ToString();
                            if (RegularPrice != e)
                                isChangesMade = true;
                        }

                        string RegularPrice1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CV") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["RegularPrice1"].ToString();
                            if (RegularPrice1 != e)
                                isChangesMade = true;
                        }

                        string RegularPrice2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CW") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["RegularPrice2"].ToString();
                            if (RegularPrice2 != e)
                                isChangesMade = true;
                        }

                        string RegularPrice3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CX") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["RegularPrice3"].ToString();
                            if (RegularPrice3 != e)
                                isChangesMade = true;
                        }

                        string RegularPrice4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CY") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["RegularPrice4"].ToString();
                            if (RegularPrice4 != e)
                                isChangesMade = true;
                        }

                        string RegularPriceCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CZ") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["RegularPriceCode"].ToString();
                            if (RegularPriceCode != e)
                                isChangesMade = true;
                        }

                        string SpecialPriceEnds = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DA") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["SpecialPriceEnds"].ToString();
                            if (SpecialPriceEnds != e)
                                isChangesMade = true;
                        }

                        string CartonDimensions = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DB") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["CartonDimensions"].ToString();
                            if (CartonDimensions != e)
                                isChangesMade = true;
                        }

                        string VisualHeading = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CR") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["VisualHeading"].ToString();
                            if (VisualHeading != e)
                                isChangesMade = true;
                        }

                        string FamilyCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CT") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["FamilyCode"].ToString();
                            if (FamilyCode != e)
                                isChangesMade = true;
                        }

                        string VisualPrice = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CS") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["VisualPrice"].ToString();
                            if (VisualPrice != e)
                                isChangesMade = true;
                        }


                        string OldPage2012 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("B") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["OldPage2012"].ToString();
                            if (OldPage2012 != e)
                                isChangesMade = true;
                        }

                        string Net1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("O") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Net1"].ToString();
                            if (Net1 != e)
                                isChangesMade = true;
                        }

                        string Net2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("P") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Net2"].ToString();
                            if (Net2 != e)
                                isChangesMade = true;
                        }

                        string Net3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("Q") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Net3"].ToString();
                            if (Net3 != e)
                                isChangesMade = true;
                        }

                        string Net4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("R") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Net4"].ToString();
                            if (Net4 != e)
                                isChangesMade = true;
                        }

                        string Net5 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("S") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Net5"].ToString();
                            if (Net5 != e)
                                isChangesMade = true;
                        }

                        string Net6 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("T") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Net6"].ToString();
                            if (Net6 != e)
                                isChangesMade = true;
                        }

                        string Net7 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("U") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Net7"].ToString();
                            if (Net7 != e)
                                isChangesMade = true;
                        }

                        string Net8 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("V") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Net8"].ToString();
                            if (Net8 != e)
                                isChangesMade = true;
                        }

                        string LOWESTINVOICEVALUEEQPMOQ = GetCellValue(row.GetCell(ExcelColumnNameToNumber("W") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["LOWESTINVOICEVALUEEQPMOQ"].ToString();
                            if (LOWESTINVOICEVALUEEQPMOQ != e)
                                isChangesMade = true;
                        }

                        string CurrentEQP = GetCellValue(row.GetCell(ExcelColumnNameToNumber("X") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["CurrentEQP"].ToString();
                            if (CurrentEQP != e)
                                isChangesMade = true;
                        }

                        string CurrentEQPLess5PerCent = GetCellValue(row.GetCell(ExcelColumnNameToNumber("Y") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["CurrentEQPLess5PerCent"].ToString();
                            if (CurrentEQPLess5PerCent != e)
                                isChangesMade = true;
                        }

                        string Change2010to2011EQPtoEQP = GetCellValue(row.GetCell(ExcelColumnNameToNumber("Z") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Change2010to2011EQPtoEQP"].ToString();
                            if (Change2010to2011EQPtoEQP != e)
                                isChangesMade = true;
                        }

                        string CountryofOrigin = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AA") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["CountryofOrigin"].ToString();
                            if (CountryofOrigin != e)
                                isChangesMade = true;
                        }

                        string HSCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AB") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["HSCode"].ToString();
                            if (HSCode != e)
                                isChangesMade = true;
                        }

                        string MasterPack = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AC") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["MasterPack"].ToString();
                            if (MasterPack != e)
                                isChangesMade = true;
                        }

                        string L = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AD") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["L"].ToString();
                            if (L != e)
                                isChangesMade = true;
                        }

                        string W = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AE") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["W"].ToString();
                            if (W != e)
                                isChangesMade = true;
                        }

                        string H = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AF") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["H"].ToString();
                            if (H != e)
                                isChangesMade = true;
                        }

                        string Volume = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AG") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Volume"].ToString();
                            if (Volume != e)
                                isChangesMade = true;
                        }

                        string FreightUnit = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AH") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["FreightUnit"].ToString();
                            if (FreightUnit != e)
                                isChangesMade = true;
                        }

                        string DateRevised = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DC") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["DateRevised"].ToString();
                            if (DateRevised != e)
                                isChangesMade = true;
                        }

                        string RevisedBy = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DD") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["RevisedBy"].ToString();
                            if (RevisedBy != e)
                                isChangesMade = true;
                        }

                        string InternalComments = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DE") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["InternalComments"].ToString();
                            if (InternalComments != e)
                                isChangesMade = true;
                        }

                        string PPPCNotes = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DF") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["PPPCNotes"].ToString();
                            if (PPPCNotes != e)
                                isChangesMade = true;
                        }

                        string DezineCategory = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DG") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["DezineCategory"].ToString();
                            if (DezineCategory != e)
                                isChangesMade = true;
                        }

                        string INFOtracImportResultifError = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DP") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["INFOtracImportResultifError"].ToString();
                            if (INFOtracImportResultifError != e)
                                isChangesMade = true;
                        }


                        string Keyword_1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DH") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Keyword_1"].ToString();
                            if (Keyword_1 != e)
                                isChangesMade = true;
                        }

                        string Keyword_2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DI") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Keyword_2"].ToString();
                            if (Keyword_2 != e)
                                isChangesMade = true;
                        }

                        string Keyword_3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DJ") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Keyword_3"].ToString();
                            if (Keyword_3 != e)
                                isChangesMade = true;
                        }

                        string Keyword_4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DK") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Keyword_4"].ToString();
                            if (Keyword_4 != e)
                                isChangesMade = true;
                        }

                        string Keyword_5 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DL") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Keyword_5"].ToString();
                            if (Keyword_5 != e)
                                isChangesMade = true;
                        }

                        string Keyword_6 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DM") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Keyword_6"].ToString();
                            if (Keyword_6 != e)
                                isChangesMade = true;
                        }

                        string Keyword_Color = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DM") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Keyword_Color"].ToString();
                            if (Keyword_Color != e)
                                isChangesMade = true;
                        }

                        string keyword_Linename = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DO") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["keyword_Linename"].ToString();
                            if (keyword_Linename != e)
                                isChangesMade = true;
                        }

                        string Keyword_Colour_Primary = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DQ") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Keyword_Colour_Primary"].ToString();
                            if (Keyword_Colour_Primary != e)
                                isChangesMade = true;
                        }

                        string Keyword_Colour_Secondary = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DR") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Keyword_Colour_Secondary"].ToString();
                            if (Keyword_Colour_Secondary != e)
                                isChangesMade = true;
                        }


                        string Related_1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CL") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Related_1"].ToString();
                            if (Related_1 != e)
                                isChangesMade = true;
                        }

                        string Related_2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CM") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Related_2"].ToString();
                            if (Related_2 != e)
                                isChangesMade = true;
                        }

                        string Related_3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CN") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Related_3"].ToString();
                            if (Related_3 != e)
                                isChangesMade = true;
                        }

                        string Related_4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CO") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Related_4"].ToString();
                            if (Related_4 != e)
                                isChangesMade = true;
                        }

                        string Related_5 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CP") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Related_5"].ToString();
                            if (Related_5 != e)
                                isChangesMade = true;
                        }

                        string Related_6 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CQ") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Related_6"].ToString();
                            if (Related_6 != e)
                                isChangesMade = true;
                        }


                        string QuantityLevel = GetCellValue(row.GetCell(ExcelColumnNameToNumber("E") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["QuantityLevel"].ToString();
                            if (QuantityLevel != e)
                                isChangesMade = true;
                        }

                        string Price1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("F") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Price1"].ToString();
                            if (Price1 != e)
                                isChangesMade = true;
                        }

                        string Price2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("G") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Price2"].ToString();
                            if (Price2 != e)
                                isChangesMade = true;
                        }

                        string Price3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("H") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Price3"].ToString();
                            if (Price3 != e)
                                isChangesMade = true;
                        }

                        string Price4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("I") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Price4"].ToString();
                            if (Price4 != e)
                                isChangesMade = true;
                        }

                        string Price5 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("J") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Price5"].ToString();
                            if (Price5 != e)
                                isChangesMade = true;
                        }

                        string Price6 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("K") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Price6"].ToString();
                            if (Price6 != e)
                                isChangesMade = true;
                        }

                        string Price7 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("L") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Price7"].ToString();
                            if (Price7 != e)
                                isChangesMade = true;
                        }

                        string Price8 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("M") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Price8"].ToString();
                            if (Price8 != e)
                                isChangesMade = true;
                        }

                        string DiscountCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("N") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["DiscountCode"].ToString();
                            if (DiscountCode != e)
                                isChangesMade = true;
                        }


                        string AddColourOption = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BE") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["AddColourOption"].ToString();
                            if (AddColourOption != e)
                                isChangesMade = true;
                        }

                        string AddCol_1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BF") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["AddCol_1"].ToString();
                            if (AddCol_1 != e)
                                isChangesMade = true;
                        }

                        string AddCol_2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BG") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["AddCol_2"].ToString();
                            if (AddCol_2 != e)
                                isChangesMade = true;
                        }

                        string AddCol_3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BH") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["AddCol_3"].ToString();
                            if (AddCol_3 != e)
                                isChangesMade = true;
                        }

                        string AddCol_4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BI") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["AddCol_4"].ToString();
                            if (AddCol_4 != e)
                                isChangesMade = true;
                        }

                        string AddColPriceCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BJ") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["AddColPriceCode"].ToString();
                            if (AddColPriceCode != e)
                                isChangesMade = true;
                        }

                        string DecalOption = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BK") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["DecalOption"].ToString();
                            if (DecalOption != e)
                                isChangesMade = true;
                        }

                        string Decal_1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BL") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Decal_1"].ToString();
                            if (Decal_1 != e)
                                isChangesMade = true;
                        }

                        string Decal_2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BM") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Decal_2"].ToString();
                            if (Decal_2 != e)
                                isChangesMade = true;
                        }

                        string Decal_3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BN") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Decal_3"].ToString();
                            if (Decal_3 != e)
                                isChangesMade = true;
                        }

                        string Decal_4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BO") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Decal_4"].ToString();
                            if (Decal_4 != e)
                                isChangesMade = true;
                        }

                        string DecalPriceCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BP") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["DecalPriceCode"].ToString();
                            if (DecalPriceCode != e)
                                isChangesMade = true;
                        }

                        string LaserEngravingOption = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BQ") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["LaserEngravingOption"].ToString();
                            if (LaserEngravingOption != e)
                                isChangesMade = true;
                        }

                        string Laser_1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BR") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Laser_1"].ToString();
                            if (Laser_1 != e)
                                isChangesMade = true;
                        }

                        string Laser_2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BS") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Laser_2"].ToString();
                            if (Laser_2 != e)
                                isChangesMade = true;
                        }

                        string Laser_3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BT") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Laser_3"].ToString();
                            if (Laser_3 != e)
                                isChangesMade = true;
                        }

                        string Laser_4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BU") - 1)).Replace("$", "").Trim();
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["Laser_4"].ToString();
                            if (Laser_4 != e)
                                isChangesMade = true;
                        }

                        string LaserPriceCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BV") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["LaserPriceCode"].ToString();
                            if (LaserPriceCode != e)
                                isChangesMade = true;
                        }



                        string SpecialCommisionAdder = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DS") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["SpecialCommisionAdder"].ToString();
                            if (SpecialCommisionAdder != e)
                                isChangesMade = true;
                        }

                        string IsShopifyEnabled = GetCellValue(row.GetCell(ExcelColumnNameToNumber("ES") - 1));
                        //if (isChangesMade == false)
                        //{
                        //    string e = product.Rows[0]["IsShopifyEnabled"].ToString();
                        //    if (IsShopifyEnabled != e)
                        //        isChangesMade = true;
                        //}

                        string UseAlternateImprint = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DT") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["UseAlternateImprintType"].ToString();
                            if (UseAlternateImprint != e)
                                isChangesMade = true;
                        }

                        string SageProductSizing = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DU") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["SageProductSize"].ToString();
                            if (SageProductSizing != e)
                                isChangesMade = true;
                        }

                        string SageDescription = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DV") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["SageDescription"].ToString();
                            if (SageDescription != e)
                                isChangesMade = true;
                        }

                        string BrandingA = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DW") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingA"].ToString();
                            if (BrandingA != e)
                                isChangesMade = true;
                        }

                        string BrandingAProductNumber = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DX") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingAProductNumber"].ToString();
                            if (BrandingAProductNumber != e)
                                isChangesMade = true;
                        }

                        string BrandingALocation1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DY") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingALocation1"].ToString();
                            if (BrandingALocation1 != e)
                                isChangesMade = true;
                        }

                        string BrandingALocation1MeasurementType = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DZ") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingALocation1MeasurementType"].ToString();
                            if (BrandingALocation1MeasurementType != e)
                                isChangesMade = true;
                        }

                        string BrandingALocation1Heigth = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EA") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingALocation1Heigth"].ToString();
                            if (BrandingALocation1Heigth != e)
                                isChangesMade = true;
                        }

                        string BrandingALocation1Width = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EB") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingALocation1Width"].ToString();
                            if (BrandingALocation1Width != e)
                                isChangesMade = true;
                        }

                        string BrandingALocation2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EC") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingALocation2"].ToString();
                            if (BrandingALocation2 != e)
                                isChangesMade = true;
                        }

                        string BrandingALocation2MeasurementType = GetCellValue(row.GetCell(ExcelColumnNameToNumber("ED") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingALocation2MeasurementType"].ToString();
                            if (BrandingALocation2MeasurementType != e)
                                isChangesMade = true;
                        }

                        string BrandingALocation2Heigth = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EE") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingALocation2Heigth"].ToString();
                            if (BrandingALocation2Heigth != e)
                                isChangesMade = true;
                        }

                        string BrandingALocation2Width = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EF") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingALocation2Width"].ToString();
                            if (BrandingALocation2Width != e)
                                isChangesMade = true;
                        }

                        string BrandingB = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EG") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingB"].ToString();
                            if (BrandingB != e)
                                isChangesMade = true;
                        }

                        string BrandingBProductNumber = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EH") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingBProductNumber"].ToString();
                            if (BrandingBProductNumber != e)
                                isChangesMade = true;
                        }

                        string BrandingBLocation1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EI") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingBLocation1"].ToString();
                            if (BrandingBLocation1 != e)
                                isChangesMade = true;
                        }

                        string BrandingBLocation1MeasurementType = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EJ") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingBLocation1MeasurementType"].ToString();
                            if (BrandingBLocation1MeasurementType != e)
                                isChangesMade = true;
                        }

                        string BrandingBLocation1Heigth = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EK") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingBLocation1Heigth"].ToString();
                            if (BrandingBLocation1Heigth != e)
                                isChangesMade = true;
                        }

                        string BrandingBLocation1Width = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EL") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingBLocation1Width"].ToString();
                            if (BrandingBLocation1Width != e)
                                isChangesMade = true;
                        }

                        string BrandingBLocation2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EM") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingBLocation2"].ToString();
                            if (BrandingBLocation2 != e)
                                isChangesMade = true;
                        }

                        string BrandingBLocation2MeasurementType = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EN") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingBLocation2MeasurementType"].ToString();
                            if (BrandingBLocation2MeasurementType != e)
                                isChangesMade = true;
                        }

                        string BrandingBLocation2Heigth = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EO") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingBLocation2Heigth"].ToString();
                            if (BrandingBLocation2Heigth != e)
                                isChangesMade = true;
                        }

                        string BrandingBLocation2Width = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EP") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingBLocation2Width"].ToString();
                            if (BrandingBLocation2Width != e)
                                isChangesMade = true;
                        }

                        string BrandingC = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EQ") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingC"].ToString();
                            if (BrandingC != e)
                                isChangesMade = true;
                        }

                        string BrandingCProductNumber = GetCellValue(row.GetCell(ExcelColumnNameToNumber("ER") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingCProductNumber"].ToString();
                            if (BrandingCProductNumber != e)
                                isChangesMade = true;
                        }

                        string BrandingD = GetCellValue(row.GetCell(ExcelColumnNameToNumber("ES") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingD"].ToString();
                            if (BrandingD != e)
                                isChangesMade = true;
                        }

                        string BrandingDProductNumber = GetCellValue(row.GetCell(ExcelColumnNameToNumber("ET") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingDProductNumber"].ToString();
                            if (BrandingDProductNumber != e)
                                isChangesMade = true;
                        }

                        string BrandingE = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EU") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingE"].ToString();
                            if (BrandingE != e)
                                isChangesMade = true;
                        }

                        string BrandingEProductNumber = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EV") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingEProductNumber"].ToString();
                            if (BrandingEProductNumber != e)
                                isChangesMade = true;
                        }

                        string BrandingF = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EW") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingF"].ToString();
                            if (BrandingF != e)
                                isChangesMade = true;
                        }

                        string BrandingFProductNumber = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EX") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingFProductNumber"].ToString();
                            if (BrandingFProductNumber != e)
                                isChangesMade = true;
                        }

                        string BrandingFamily = GetCellValue(row.GetCell(ExcelColumnNameToNumber("EY") - 1));
                        if (isChangesMade == false)
                        {
                            string e = product.Rows[0]["BrandingFamily"].ToString();
                            if (BrandingFamily != e)
                                isChangesMade = true;
                        }
                        #endregion



                        string MappedItemNumber = string.Empty;

                        #region update database

                        if (isChangesMade == true)
                        {

                            if (productid != 0)
                            {
                                string productQuery = "Update Product set Name = @Name, ShortDescription = @ShortDescription, UpdatedOnUtc = getutcdate(), HasTierPrices = 1, FamilyCode = @FamilyCode, Price = @price where sku = @sku";
                                SqlCommand cmdpro = new SqlCommand(productQuery);
                                cmdpro.Parameters.AddWithValue("@sku", sku);
                                cmdpro.Parameters.AddWithValue("@FamilyCode", FamilyCode);
                                cmdpro.Parameters.AddWithValue("@Name", ShortName);
                                cmdpro.Parameters.AddWithValue("@ShortDescription", ShortDescription);
                                if (string.IsNullOrEmpty(VisualPrice))
                                {
                                    throw new Exception("Visual price is empty");
                                }
                                cmdpro.Parameters.AddWithValue("@price", VisualPrice);


                                int rowCount = ExecuteQuery(cmdpro);

                                if (rowCount <= 0)
                                {
                                    throw new Exception("Error in executing sql query for product ");
                                }

                                cmdpro.Dispose();



                                #region DezinecorpData
                                string DezineCorpDataQuery = string.Empty;
                                var isDataExist = CheckDezineCorpDataRecordExist(productid);
                                if (!isDataExist)
                                    DezineCorpDataQuery = "insert into DezineCorpData values( @ProductId, @NewPage, @ItemIsNew, @GuarenteedStock, @Materials, @Features, @Includes, @SpecailPackaging, @Capacity, @Size, @ImprintAreaInOutboard, @ImprintAreaWrapAround, @DecoratingOption, @PeicesPerCartoon, @WeightPerCartoon, @BlankLine, @ProtectivePackaging, @ReferToCataloguePage, @PricingFlag, @MadeinCanada, @MadeinNorthAmerica, @InventoryFlag, @PricingCode, @PricingFooterNote, @SetupPerColour, @RepeatSetup, @DebossSetup, @RepeatDeboss, @DecalSetup, @RepeatDecal, @LaserSetup, @RepeatLaser, @AdditionalCharge1, @AdditionalCharge2, @AdditionalCharge3, @AdditionalCharge4, @RepeatTerm, @FinalNote, @RegularPrice, @RegularPrice1, @RegularPrice2, @RegularPrice3, @RegularPrice4, @RegularPriceCode, @SpecialPriceEnds, @CartonDimensions, @VisualHeading, @FamilyCode, @VisualPrice)";
                                else
                                    DezineCorpDataQuery = "update DezineCorpData set NewPage = @NewPage, ItemIsNew = @ItemIsNew, GuarenteedStock = @GuarenteedStock, Materials = @Materials, Features = @Features, Includes = @Includes, SpecailPackaging = @SpecailPackaging, Capacity = @Capacity, Size = @Size, ImprintAreaInOutboard = @ImprintAreaInOutboard, ImprintAreaWrapAround = @ImprintAreaWrapAround, DecoratingOption = @DecoratingOption, PeicesPerCartoon = @PeicesPerCartoon, WeightPerCartoon = @WeightPerCartoon, BlankLine = @BlankLine, ProtectivePackaging = @ProtectivePackaging, ReferToCataloguePage = @ReferToCataloguePage, PricingFlag = @PricingFlag, MadeinCanada = @MadeinCanada, MadeinNorthAmerica = @MadeinNorthAmerica, InventoryFlag = @InventoryFlag, PricingCode = @PricingCode, PricingFooterNote = @PricingFooterNote, SetupPerColour = @SetupPerColour, RepeatSetup = @RepeatSetup, DebossSetup = @DebossSetup, RepeatDeboss = @RepeatDeboss, DecalSetup = @DecalSetup, RepeatDecal = @RepeatDecal, LaserSetup = @LaserSetup, RepeatLaser = @RepeatLaser, AdditionalCharge1 = @AdditionalCharge1, AdditionalCharge2 = @AdditionalCharge2, AdditionalCharge3 = @AdditionalCharge3, AdditionalCharge4 = @AdditionalCharge4, RepeatTerm = @RepeatTerm, FinalNote = @FinalNote, RegularPrice = @RegularPrice, RegularPrice1 = @RegularPrice1, RegularPrice2 = @RegularPrice2, RegularPrice3 = @RegularPrice3, RegularPrice4 = @RegularPrice4, RegularPriceCode = @RegularPriceCode, SpecialPriceEnds = @SpecialPriceEnds, CartonDimensions = @CartonDimensions, VisualHeading = @VisualHeading, FamilyCode = @FamilyCode, VisualPrice = @VisualPrice where ProductId = @ProductId";

                                SqlCommand cmd = new SqlCommand(DezineCorpDataQuery);
                                cmd.Parameters.AddWithValue("@ProductId", productid);
                                cmd.Parameters.AddWithValue("@NewPage", NewPage);
                                cmd.Parameters.AddWithValue("@ItemIsNew", ItemIsNew);
                                cmd.Parameters.AddWithValue("@GuarenteedStock", GuarenteedStock);
                                cmd.Parameters.AddWithValue("@Materials", Materials);
                                cmd.Parameters.AddWithValue("@Features", Features);
                                cmd.Parameters.AddWithValue("@Includes", Includes);
                                cmd.Parameters.AddWithValue("@SpecailPackaging", SpecailPackaging);
                                cmd.Parameters.AddWithValue("@Capacity", Capacity);
                                cmd.Parameters.AddWithValue("@Size", Size);
                                cmd.Parameters.AddWithValue("@ImprintAreaInOutboard", ImprintAreaInOutboard);
                                cmd.Parameters.AddWithValue("@ImprintAreaWrapAround", ImprintAreaWrapAround);
                                cmd.Parameters.AddWithValue("@DecoratingOption", DecoratingOption);
                                cmd.Parameters.AddWithValue("@PeicesPerCartoon", PeicesPerCartoon);
                                cmd.Parameters.AddWithValue("@WeightPerCartoon", WeightPerCartoon);
                                cmd.Parameters.AddWithValue("@BlankLine", BlankLine);
                                cmd.Parameters.AddWithValue("@ProtectivePackaging", ProtectivePackaging);
                                cmd.Parameters.AddWithValue("@ReferToCataloguePage", ReferToCataloguePage);
                                cmd.Parameters.AddWithValue("@PricingFlag", PricingFlag);
                                cmd.Parameters.AddWithValue("@MadeinCanada", MadeinCanada);
                                cmd.Parameters.AddWithValue("@MadeinNorthAmerica", MadeinNorthAmerica);
                                cmd.Parameters.AddWithValue("@InventoryFlag", InventoryFlag);
                                cmd.Parameters.AddWithValue("@PricingCode", PricingCode);
                                cmd.Parameters.AddWithValue("@PricingFooterNote", PricingFooterNote);
                                cmd.Parameters.AddWithValue("@SetupPerColour", SetupPerColour);
                                cmd.Parameters.AddWithValue("@RepeatSetup", RepeatSetup);
                                cmd.Parameters.AddWithValue("@DebossSetup", DebossSetup);
                                cmd.Parameters.AddWithValue("@RepeatDeboss", RepeatDeboss);
                                cmd.Parameters.AddWithValue("@DecalSetup", DecalSetup);
                                cmd.Parameters.AddWithValue("@RepeatDecal", RepeatDecal);
                                cmd.Parameters.AddWithValue("@LaserSetup", LaserSetup);
                                cmd.Parameters.AddWithValue("@RepeatLaser", RepeatLaser);
                                cmd.Parameters.AddWithValue("@AdditionalCharge1", AdditionalCharge1);
                                cmd.Parameters.AddWithValue("@AdditionalCharge2", AdditionalCharge2);
                                cmd.Parameters.AddWithValue("@AdditionalCharge3", AdditionalCharge3);
                                cmd.Parameters.AddWithValue("@AdditionalCharge4", AdditionalCharge4);
                                cmd.Parameters.AddWithValue("@RepeatTerm", RepeatTerm);
                                cmd.Parameters.AddWithValue("@FinalNote", FinalNote);
                                cmd.Parameters.AddWithValue("@RegularPrice", RegularPrice);
                                cmd.Parameters.AddWithValue("@RegularPrice1", RegularPrice1);
                                cmd.Parameters.AddWithValue("@RegularPrice2", RegularPrice2);
                                cmd.Parameters.AddWithValue("@RegularPrice3", RegularPrice3);
                                cmd.Parameters.AddWithValue("@RegularPrice4", RegularPrice4);
                                cmd.Parameters.AddWithValue("@RegularPriceCode", RegularPriceCode);
                                cmd.Parameters.AddWithValue("@SpecialPriceEnds", SpecialPriceEnds);
                                cmd.Parameters.AddWithValue("@CartonDimensions", CartonDimensions);
                                cmd.Parameters.AddWithValue("@VisualHeading", VisualHeading);
                                cmd.Parameters.AddWithValue("@FamilyCode", FamilyCode);
                                cmd.Parameters.AddWithValue("@VisualPrice", VisualPrice);
                                rowCount = ExecuteQuery(cmd);

                                if (rowCount <= 0)
                                {
                                    throw new Exception("Error in executing sql query for product ");
                                }


                                cmd.Dispose();
                                #endregion

                                #region DezineCorpDataRefOnly
                                string DezineCorpDataRefQuery = string.Empty;
                                var isDezineCorpDataRefQuery = CheckisDezineCorpDataRefQueryRecordExist(productid);
                                if (!isDezineCorpDataRefQuery)
                                    DezineCorpDataRefQuery = "insert into DezineCorpDataRefOnly values(  @ProductId, @OldPage2012, @Net1, @Net2, @Net3, @Net4, @Net5, @Net6, @Net7, @Net8, @LOWESTINVOICEVALUEEQPMOQ, @CurrentEQP, @CurrentEQPLess5PerCent, @Change2010to2011EQPtoEQP, @CountryofOrigin, @HSCode, @MasterPack, @L, @W, @H, @Volume, @FreightUnit, @DateRevised, @RevisedBy, @InternalComments, @PPPCNotes, @DezineCategory, @INFOtracImportResultifError,@SpecialCommisionAdder,@IsShopifyEnabled)";
                                else
                                    DezineCorpDataRefQuery = "update DezineCorpDataRefOnly set OldPage2012=@OldPage2012, Net1=@Net1, Net2=@Net2, Net3=@Net3, Net4=@Net4, Net5=@Net5, Net6=@Net6, Net7=@Net7, Net8=@Net8, LOWESTINVOICEVALUEEQPMOQ=@LOWESTINVOICEVALUEEQPMOQ, CurrentEQP=@CurrentEQP, CurrentEQPLess5PerCent=@CurrentEQPLess5PerCent, Change2010to2011EQPtoEQP=@Change2010to2011EQPtoEQP, CountryofOrigin=@CountryofOrigin, HSCode=@HSCode, MasterPack=@MasterPack, L=@L, W=@W, H=@H, Volume=@Volume, FreightUnit=@FreightUnit, DateRevised=@DateRevised, RevisedBy=@RevisedBy, InternalComments=@InternalComments, PPPCNotes=@PPPCNotes, DezineCategory=@DezineCategory, INFOtracImportResultifError=@INFOtracImportResultifError,SpecialCommisionAdder=@SpecialCommisionAdder,IsShopifyEnabled=@IsShopifyEnabled where ProductId = @ProductId";

                                SqlCommand cmdref = new SqlCommand(DezineCorpDataRefQuery);
                                cmdref.Parameters.AddWithValue("@ProductId", productid);
                                cmdref.Parameters.AddWithValue("@OldPage2012", OldPage2012);
                                cmdref.Parameters.AddWithValue("@Net1", Net1);
                                cmdref.Parameters.AddWithValue("@Net2", Net2);
                                cmdref.Parameters.AddWithValue("@Net3", Net3);
                                cmdref.Parameters.AddWithValue("@Net4", Net4);
                                cmdref.Parameters.AddWithValue("@Net5", Net5);
                                cmdref.Parameters.AddWithValue("@Net6", Net6);
                                cmdref.Parameters.AddWithValue("@Net7", Net7);
                                cmdref.Parameters.AddWithValue("@Net8", Net8);
                                cmdref.Parameters.AddWithValue("@LOWESTINVOICEVALUEEQPMOQ", LOWESTINVOICEVALUEEQPMOQ);
                                cmdref.Parameters.AddWithValue("@CurrentEQP", CurrentEQP);
                                cmdref.Parameters.AddWithValue("@CurrentEQPLess5PerCent", CurrentEQPLess5PerCent);
                                cmdref.Parameters.AddWithValue("@Change2010to2011EQPtoEQP", Change2010to2011EQPtoEQP);
                                cmdref.Parameters.AddWithValue("@CountryofOrigin", CountryofOrigin);
                                cmdref.Parameters.AddWithValue("@HSCode", HSCode);
                                cmdref.Parameters.AddWithValue("@MasterPack", MasterPack);
                                cmdref.Parameters.AddWithValue("@L", L);
                                cmdref.Parameters.AddWithValue("@W", W);
                                cmdref.Parameters.AddWithValue("@H", H);
                                cmdref.Parameters.AddWithValue("@Volume", Volume);
                                cmdref.Parameters.AddWithValue("@FreightUnit", FreightUnit);
                                cmdref.Parameters.AddWithValue("@DateRevised", DateRevised);
                                cmdref.Parameters.AddWithValue("@RevisedBy", RevisedBy);
                                cmdref.Parameters.AddWithValue("@InternalComments", InternalComments.Length >= 450 ? InternalComments.Substring(0, 450) + "..." : InternalComments);
                                cmdref.Parameters.AddWithValue("@PPPCNotes", PPPCNotes);
                                cmdref.Parameters.AddWithValue("@DezineCategory", DezineCategory);
                                cmdref.Parameters.AddWithValue("@INFOtracImportResultifError", INFOtracImportResultifError);
                                cmdref.Parameters.AddWithValue("@SpecialCommisionAdder", SpecialCommisionAdder);
                                cmdref.Parameters.AddWithValue("@IsShopifyEnabled", IsShopifyEnabled);

                                rowCount = ExecuteQuery(cmdref);

                                if (rowCount <= 0)
                                {
                                    throw new Exception("Error in executing sql query for product ");
                                }

                                cmdref.Dispose();
                                #endregion

                                #region DezineCorpProductKeyword

                                string DezineCorpProductKeywordQuery = string.Empty;
                                var isDezineCorpProductKeywordQueryQuery = CheckisDezineCorpProductKeywordQueryQueryRecordExist(productid);
                                if (!isDezineCorpProductKeywordQueryQuery)
                                    DezineCorpProductKeywordQuery = "insert into DezineCorpProductKeyword values(@ProductId, @Keyword_1, @Keyword_2, @Keyword_3, @Keyword_4, @Keyword_5, @Keyword_6, @Keyword_Color, @keyword_Linename, @Keyword_Colour_Primary, @Keyword_Colour_Secondary)";
                                else
                                    DezineCorpProductKeywordQuery = "update DezineCorpProductKeyword Set Keyword_1= @Keyword_1, Keyword_2= @Keyword_2, Keyword_3= @Keyword_3, Keyword_4= @Keyword_4, Keyword_5= @Keyword_5, Keyword_6= @Keyword_6, Keyword_Color= @Keyword_Color, keyword_Linename= @keyword_Linename, Keyword_Colour_Primary= @Keyword_Colour_Primary, Keyword_Colour_Secondary= @Keyword_Colour_Secondary  where ProductId = @ProductId";

                                SqlCommand cmdkey = new SqlCommand(DezineCorpProductKeywordQuery);

                                cmdkey.Parameters.AddWithValue("@ProductId", productid);
                                cmdkey.Parameters.AddWithValue("@Keyword_1", Keyword_1);
                                cmdkey.Parameters.AddWithValue("@Keyword_2", Keyword_2);
                                cmdkey.Parameters.AddWithValue("@Keyword_3", Keyword_3);
                                cmdkey.Parameters.AddWithValue("@Keyword_4", Keyword_4);
                                cmdkey.Parameters.AddWithValue("@Keyword_5", Keyword_5);
                                cmdkey.Parameters.AddWithValue("@Keyword_6", Keyword_6);
                                cmdkey.Parameters.AddWithValue("@Keyword_Color", Keyword_Color);
                                cmdkey.Parameters.AddWithValue("@keyword_Linename", keyword_Linename);
                                cmdkey.Parameters.AddWithValue("@Keyword_Colour_Primary", Keyword_Colour_Primary);
                                cmdkey.Parameters.AddWithValue("@Keyword_Colour_Secondary", Keyword_Colour_Secondary);

                                rowCount = ExecuteQuery(cmdkey);

                                if (rowCount <= 0)
                                {
                                    throw new Exception("Error in executing sql query for product ");
                                }


                                cmdkey.Dispose();
                                #endregion

                                #region DezineCorpRelatedProduct
                                string DezineCorpRelatedProductQuery = string.Empty;
                                var isDezineCorpRelatedProductQuery = CheckisDezineCorpRelatedProductQueryRecordExist(productid);
                                if (!isDezineCorpRelatedProductQuery)
                                    DezineCorpRelatedProductQuery = "insert into DezineCorpRelatedProduct values(@ProductId, @Related_1, @Related_2, @Related_3, @Related_4, @Related_5, @Related_6)";
                                else
                                    DezineCorpRelatedProductQuery = "update DezineCorpRelatedProduct Set Related_1 =  @Related_1, Related_2 =  @Related_2, Related_3 =  @Related_3, Related_4 =  @Related_4, Related_5 =  @Related_5, Related_6 =  @Related_6 where ProductId = @ProductId";

                                SqlCommand cmdrel = new SqlCommand(DezineCorpRelatedProductQuery);

                                cmdrel.Parameters.AddWithValue("@ProductId", productid);
                                cmdrel.Parameters.AddWithValue("@Related_1", Related_1);
                                cmdrel.Parameters.AddWithValue("@Related_2", Related_2);
                                cmdrel.Parameters.AddWithValue("@Related_3", Related_3);
                                cmdrel.Parameters.AddWithValue("@Related_4", Related_4);
                                cmdrel.Parameters.AddWithValue("@Related_5", Related_5);
                                cmdrel.Parameters.AddWithValue("@Related_6", Related_6);
                                rowCount = ExecuteQuery(cmdrel);
                                if (rowCount <= 0)
                                {
                                    throw new Exception("Error in executing sql query for product ");
                                }

                                cmdrel.Dispose();
                                #endregion

                                #region DezineCorpTierPrice
                                string DezineCorpTierPriceQuery = string.Empty;
                                var isDezineCorpTierPrice = CheckisDezineCorpTierPriceRecordExist(productid);
                                if (!isDezineCorpTierPrice)
                                    DezineCorpTierPriceQuery = "insert into DezineCorpTierPrice values( @ProductId, @QuantityLevel, @Price1, @Price2, @Price3, @Price4, @Price5, @Price6, @Price7, @Price8, @DiscountCode)";
                                else
                                    DezineCorpTierPriceQuery = "update DezineCorpTierPrice Set  QuantityLevel=  @QuantityLevel, Price1=  @Price1, Price2=  @Price2, Price3=  @Price3, Price4=  @Price4, Price5=  @Price5, Price6=  @Price6, Price7=  @Price7, Price8=  @Price8, DiscountCode=  @DiscountCode where ProductId=  @ProductId";

                                SqlCommand cmdtier = new SqlCommand(DezineCorpTierPriceQuery);
                                cmdtier.Parameters.AddWithValue("@ProductId", productid);
                                cmdtier.Parameters.AddWithValue("@QuantityLevel", QuantityLevel);
                                cmdtier.Parameters.AddWithValue("@Price1", Price1);
                                cmdtier.Parameters.AddWithValue("@Price2", Price2);
                                cmdtier.Parameters.AddWithValue("@Price3", Price3);
                                cmdtier.Parameters.AddWithValue("@Price4", Price4);
                                cmdtier.Parameters.AddWithValue("@Price5", Price5);
                                cmdtier.Parameters.AddWithValue("@Price6", Price6);
                                cmdtier.Parameters.AddWithValue("@Price7", Price7);
                                cmdtier.Parameters.AddWithValue("@Price8", Price8);
                                cmdtier.Parameters.AddWithValue("@DiscountCode", DiscountCode);
                                rowCount = ExecuteQuery(cmdtier);

                                if (rowCount <= 0)
                                {
                                    throw new Exception("Error in executing sql query for product ");
                                }


                                cmdtier.Dispose();
                                #endregion

                                #region DezineCorpAdditionalPricing
                                string DezineCorpAdditionalPricingQuery = string.Empty;
                                var isDezineCorpAdditionalPricing = CheckisDezineCorpAdditionalPricingExist(productid);
                                if (!isDezineCorpAdditionalPricing)
                                    DezineCorpAdditionalPricingQuery = "insert into DezineCorpAdditionalPricing values(@ProductId, @AddColourOption, @AddCol_1, @AddCol_2, @AddCol_3, @AddCol_4, @AddColPriceCode, @DecalOption, @Decal_1, @Decal_2, @Decal_3, @Decal_4, @DecalPriceCode, @LaserEngravingOption, @Laser_1, @Laser_2, @Laser_3, @Laser_4, @LaserPriceCode)";
                                else
                                    DezineCorpAdditionalPricingQuery = "update DezineCorpAdditionalPricing Set AddColourOption =  @AddColourOption, AddCol_1 =  @AddCol_1, AddCol_2 =  @AddCol_2, AddCol_3 =  @AddCol_3, AddCol_4 =  @AddCol_4, AddColPriceCode =  @AddColPriceCode, DecalOption =  @DecalOption, Decal_1 =  @Decal_1, Decal_2 =  @Decal_2, Decal_3 =  @Decal_3, Decal_4 =  @Decal_4, DecalPriceCode =  @DecalPriceCode, LaserEngravingOption =  @LaserEngravingOption, Laser_1 =  @Laser_1, Laser_2 =  @Laser_2, Laser_3 =  @Laser_3, Laser_4 =  @Laser_4, LaserPriceCode =  @LaserPriceCode where ProductId =  @ProductId";

                                SqlCommand cmdadpr = new SqlCommand(DezineCorpAdditionalPricingQuery);

                                cmdadpr.Parameters.AddWithValue("@ProductId", productid);
                                cmdadpr.Parameters.AddWithValue("@AddColourOption", AddColourOption);
                                cmdadpr.Parameters.AddWithValue("@AddCol_1", AddCol_1);
                                cmdadpr.Parameters.AddWithValue("@AddCol_2", AddCol_2);
                                cmdadpr.Parameters.AddWithValue("@AddCol_3", AddCol_3);
                                cmdadpr.Parameters.AddWithValue("@AddCol_4", AddCol_4);
                                cmdadpr.Parameters.AddWithValue("@AddColPriceCode", AddColPriceCode);
                                cmdadpr.Parameters.AddWithValue("@DecalOption", DecalOption);
                                cmdadpr.Parameters.AddWithValue("@Decal_1", Decal_1);
                                cmdadpr.Parameters.AddWithValue("@Decal_2", Decal_2);
                                cmdadpr.Parameters.AddWithValue("@Decal_3", Decal_3);
                                cmdadpr.Parameters.AddWithValue("@Decal_4", Decal_4);
                                cmdadpr.Parameters.AddWithValue("@DecalPriceCode", DecalPriceCode);
                                cmdadpr.Parameters.AddWithValue("@LaserEngravingOption", LaserEngravingOption);
                                cmdadpr.Parameters.AddWithValue("@Laser_1", Laser_1);
                                cmdadpr.Parameters.AddWithValue("@Laser_2", Laser_2);
                                cmdadpr.Parameters.AddWithValue("@Laser_3", Laser_3);
                                cmdadpr.Parameters.AddWithValue("@Laser_4", Laser_4);
                                cmdadpr.Parameters.AddWithValue("@LaserPriceCode", LaserPriceCode);

                                rowCount = ExecuteQuery(cmdadpr);

                                if (rowCount <= 0)
                                {
                                    throw new Exception("Error in executing sql query for product ");
                                }

                                cmdadpr.Dispose();
                                #endregion


                                #region DezineCorpSageandBrandingData
                                string DezineCorpSageAndBrandingQuery = string.Empty;
                                var isDezineCorpSageandBranding = CheckisDezineCorpSageandBrandingExist(productid);
                                if (!isDezineCorpSageandBranding)
                                    DezineCorpSageAndBrandingQuery = "insert into DezinecorpSageandBrandingData (ProductId, UseAlternateImprintType, SageProductSize, SageDescription, BrandingA, BrandingALocation1, BrandingALocation1MeasurementType, BrandingALocation1Heigth, BrandingALocation1Width , BrandingALocation2, BrandingALocation2MeasurementType, BrandingALocation2Heigth, BrandingALocation2Width, BrandingB , BrandingBLocation1, BrandingBLocation1MeasurementType, BrandingBLocation1Heigth, BrandingBLocation1Width , BrandingBLocation2, BrandingBLocation2MeasurementType, BrandingBLocation2Heigth, BrandingBLocation2Width, BrandingC, BrandingCProductNumber, BrandingD, BrandingDProductNumber, MappedItemNumber, BrandingAProductNumber, BrandingBProductNumber, BrandingE, BrandingEProductNumber, BrandingF, BrandingFProductNumber, BrandingFamily )  values(@ProductId, @UseAlternateImprintType, @SageProductSize, @SageDescription, @BrandingA, @BrandingALocation1, @BrandingALocation1MeasurementType, @BrandingALocation1Heigth, @BrandingALocation1Width, @BrandingALocation2, @BrandingALocation2MeasurementType, @BrandingALocation2Heigth, @BrandingALocation2Width, @BrandingB, @BrandingBLocation1, @BrandingBLocation1MeasurementType, @BrandingBLocation1Heigth, @BrandingBLocation1Width, @BrandingBLocation2, @BrandingBLocation2MeasurementType, @BrandingBLocation2Heigth, @BrandingBLocation2Width, @BrandingC, @BrandingCProductNumber, @BrandingD, @BrandingDProductNumber, @MappedItemNumber, @BrandingAProductNumber, @BrandingBProductNumber, @BrandingE, @BrandingEProductNumber, @BrandingF, @BrandingFProductNumber, @BrandingFamily)";
                                else
                                    DezineCorpSageAndBrandingQuery = "update DezinecorpSageandBrandingData Set UseAlternateImprintType =  @UseAlternateImprintType, SageProductSize =  @SageProductSize, SageDescription =  @SageDescription, BrandingA =  @BrandingA, BrandingALocation1 =  @BrandingALocation1, BrandingALocation1MeasurementType =  @BrandingALocation1MeasurementType, BrandingALocation1Heigth =  @BrandingALocation1Heigth, BrandingALocation1Width =  @BrandingALocation1Width, BrandingALocation2 =  @BrandingALocation2, BrandingALocation2MeasurementType =  @BrandingALocation2MeasurementType, BrandingALocation2Heigth =  @BrandingALocation2Heigth, BrandingALocation2Width =  @BrandingALocation2Width, BrandingB =  @BrandingB, BrandingBLocation1 =  @BrandingBLocation1, BrandingBLocation1MeasurementType =  @BrandingBLocation1MeasurementType, BrandingBLocation1Heigth =  @BrandingBLocation1Heigth, BrandingBLocation1Width =  @BrandingBLocation1Width, BrandingBLocation2 =  @BrandingBLocation2, BrandingBLocation2MeasurementType =  @BrandingBLocation2MeasurementType, BrandingBLocation2Heigth =  @BrandingBLocation2Heigth, BrandingBLocation2Width =  @BrandingBLocation2Width, BrandingC =  @BrandingC, BrandingCProductNumber =  @BrandingCProductNumber, BrandingD =  @BrandingD, BrandingDProductNumber =  @BrandingDProductNumber, MappedItemNumber =  @MappedItemNumber, BrandingAProductNumber = @BrandingAProductNumber, BrandingBProductNumber = @BrandingBProductNumber, BrandingE = @BrandingE, BrandingEProductNumber = @BrandingEProductNumber, BrandingF = @BrandingF, BrandingFProductNumber = @BrandingFProductNumber, BrandingFamily = @BrandingFamily where ProductId =  @ProductId";

                                SqlCommand cmdsage = new SqlCommand(DezineCorpSageAndBrandingQuery);

                                cmdsage.Parameters.AddWithValue("@ProductId", productid);
                                cmdsage.Parameters.AddWithValue("@UseAlternateImprintType", UseAlternateImprint);
                                cmdsage.Parameters.AddWithValue("@SageProductSize", SageProductSizing);
                                cmdsage.Parameters.AddWithValue("@SageDescription", SageDescription);
                                cmdsage.Parameters.AddWithValue("@BrandingA", BrandingA);
                                cmdsage.Parameters.AddWithValue("@BrandingALocation1", BrandingALocation1);
                                cmdsage.Parameters.AddWithValue("@BrandingALocation1MeasurementType", BrandingALocation1MeasurementType);
                                cmdsage.Parameters.AddWithValue("@BrandingALocation1Heigth", BrandingALocation1Heigth);
                                cmdsage.Parameters.AddWithValue("@BrandingALocation1Width", BrandingALocation1Width);
                                cmdsage.Parameters.AddWithValue("@BrandingALocation2", BrandingALocation2);
                                cmdsage.Parameters.AddWithValue("@BrandingALocation2MeasurementType", BrandingALocation2MeasurementType);
                                cmdsage.Parameters.AddWithValue("@BrandingALocation2Heigth", BrandingALocation2Heigth);
                                cmdsage.Parameters.AddWithValue("@BrandingALocation2Width", BrandingALocation2Width);
                                cmdsage.Parameters.AddWithValue("@BrandingB", BrandingB);
                                cmdsage.Parameters.AddWithValue("@BrandingBLocation1", BrandingBLocation1);
                                cmdsage.Parameters.AddWithValue("@BrandingBLocation1MeasurementType", BrandingBLocation1MeasurementType);
                                cmdsage.Parameters.AddWithValue("@BrandingBLocation1Heigth", BrandingBLocation1Heigth);
                                cmdsage.Parameters.AddWithValue("@BrandingBLocation1Width", BrandingBLocation1Width);
                                cmdsage.Parameters.AddWithValue("@BrandingBLocation2", BrandingBLocation2);
                                cmdsage.Parameters.AddWithValue("@BrandingBLocation2MeasurementType", BrandingBLocation2MeasurementType);
                                cmdsage.Parameters.AddWithValue("@BrandingBLocation2Heigth", BrandingBLocation2Heigth);
                                cmdsage.Parameters.AddWithValue("@BrandingBLocation2Width", BrandingBLocation2Width);
                                cmdsage.Parameters.AddWithValue("@BrandingC", BrandingC);
                                cmdsage.Parameters.AddWithValue("@BrandingCProductNumber", BrandingCProductNumber);
                                cmdsage.Parameters.AddWithValue("@BrandingD", BrandingD);
                                cmdsage.Parameters.AddWithValue("@BrandingDProductNumber", BrandingDProductNumber);
                                cmdsage.Parameters.AddWithValue("@MappedItemNumber", MappedItemNumber);
                                cmdsage.Parameters.AddWithValue("@BrandingAProductNumber", BrandingAProductNumber);
                                cmdsage.Parameters.AddWithValue("@BrandingBProductNumber", BrandingBProductNumber);
                                cmdsage.Parameters.AddWithValue("@BrandingE", BrandingE);
                                cmdsage.Parameters.AddWithValue("@BrandingEProductNumber", BrandingEProductNumber);
                                cmdsage.Parameters.AddWithValue("@BrandingF", BrandingF);
                                cmdsage.Parameters.AddWithValue("@BrandingFProductNumber", BrandingFProductNumber);
                                cmdsage.Parameters.AddWithValue("@BrandingFamily", BrandingFamily);

                                rowCount = ExecuteQuery(cmdsage);

                                if (rowCount <= 0)
                                {
                                    throw new Exception("Error in executing sql query for product ");
                                }

                                cmdadpr.Dispose();


                                #endregion

                            }
                            else
                            {
                                skus += sku + "=>SKU Not available" + ", ";
                                skucount++;
                            }

                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        skus += sku + "=>" + ex.Message + ", ";
                    }
                    status.RecordProcessSucessfully = r - 2;
                    status.RecordFailed = skus;
                    UpdateStatus(status);

                }
                //ds.Tables.Add(dt);
                //ws.Dispose();
                //File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), "product iteration completed");

                var skusFromNopCommerce = GetSkuFromNopCommerce();

                var skutosetdelete = skusFromNopCommerce.Except(skusFromSheet);

                SetProductToDelete(skutosetdelete);

                status.IsImportFinish = true;
                UpdateStatus(status);
                //Closing workbook
                //objWB.Close();
                //Closing excel application
                //objXL.Quit();
                var newFile = Path.Combine(Path.GetDirectoryName(_path), "DezineCorpCatalogueWithColorAndImage.xls");
                using (FileStream s = new FileStream(newFile, FileMode.Create, FileAccess.Write))
                    wb.Write(s);

            }
            catch (Exception ex)
            {
                //objWB.Saved = true;
                //Closing work book
                //objWB.Close();
                //Closing excel application
                //objXL.Quit();
                //Response.Write("Illegal permission");
                var STRING = ex.Message + (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : string.Empty);

                // File.AppendAllText(Path.Combine(Path.GetDirectoryName(_path), "ImportErrorLog.txt"), STRING);

                // UpdateStatus(new ImportStatus { IsImportFinish = true, RecordFailed = ex.Message });
            }
        }


        public void READFrieghtExcel()
        {
            ImportStatus status = new ImportStatus();
            try
            {
                FileStream stream = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite);
                HSSFWorkbook wb = new HSSFWorkbook(stream);
                ISheet ws = wb.GetSheetAt(0);
                int startFromRowNumber = 3; // starting from 0, ignore first 3 rows
                int rows = ws.LastRowNum;
                status.TotalRecord = rows - 2;
                status.IsImportFinish = false;
                string skus = string.Empty;
                int skucount = 0;

                if (string.IsNullOrEmpty(_path)) return;



                for (int r = startFromRowNumber; r <= rows; r++)
                {
                    var sku = string.Empty;
                    try
                    {
                        var row = ws.GetRow(r);
                        if (row == null)
                        {
                            continue;
                        }

                        sku = GetCellValue(row.GetCell(ExcelColumnNameToNumber("A") - 1));
                        var product = GetProductBySKU(sku);


                        if (product == null)
                            continue;

                        if (product.Rows.Count <= 0)
                            continue;


                        int productid = Convert.ToInt32(product.Rows[0]["Id"].ToString());

                        if (productid == 0)
                            continue;


                        string SKU = GetCellValue(row.GetCell(ExcelColumnNameToNumber("A") - 1));
                        string Year1_Catalogue_page_number = GetCellValue(row.GetCell(ExcelColumnNameToNumber("B") - 1));
                        string Year2_Catalogue_page_number = GetCellValue(row.GetCell(ExcelColumnNameToNumber("C") - 1));
                        string OP_MasterPackQty = GetCellValue(row.GetCell(ExcelColumnNameToNumber("E") - 1));
                        string OP_MasterPackWghtLBS = GetCellValue(row.GetCell(ExcelColumnNameToNumber("F") - 1));
                        string OP_L = GetCellValue(row.GetCell(ExcelColumnNameToNumber("G") - 1));
                        string OP_W = GetCellValue(row.GetCell(ExcelColumnNameToNumber("H") - 1));
                        string OP_H = GetCellValue(row.GetCell(ExcelColumnNameToNumber("I") - 1));
                        string OP_CW_LBS = GetCellValue(row.GetCell(ExcelColumnNameToNumber("J") - 1));
                        string PP_MasterPackQty = GetCellValue(row.GetCell(ExcelColumnNameToNumber("K") - 1));
                        string PP_BoxID = GetCellValue(row.GetCell(ExcelColumnNameToNumber("L") - 1));
                        string SP_ProdWght_LBS = GetCellValue(row.GetCell(ExcelColumnNameToNumber("M") - 1));
                        string SP_PcsPerSet = GetCellValue(row.GetCell(ExcelColumnNameToNumber("N") - 1));
                        string SP_GiftBox_LBS = GetCellValue(row.GetCell(ExcelColumnNameToNumber("O") - 1));
                        string SP_ProdWght_and_GB_Wght = GetCellValue(row.GetCell(ExcelColumnNameToNumber("P") - 1));
                        string SP_WghtOfMaster_Carton_with_product = GetCellValue(row.GetCell(ExcelColumnNameToNumber("Q") - 1));
                        string SP_Carton_Foam_Wght_Est_LBS = GetCellValue(row.GetCell(ExcelColumnNameToNumber("R") - 1));
                        string PPMasterPack_Total_Wght_LBS = GetCellValue(row.GetCell(ExcelColumnNameToNumber("S") - 1));
                        string PP_L = GetCellValue(row.GetCell(ExcelColumnNameToNumber("T") - 1));
                        string PP_W = GetCellValue(row.GetCell(ExcelColumnNameToNumber("U") - 1));
                        string PP_H = GetCellValue(row.GetCell(ExcelColumnNameToNumber("V") - 1));
                        string PP_Cubed_Wght_LBS = GetCellValue(row.GetCell(ExcelColumnNameToNumber("W") - 1));
                        string CartonsPerSkidLayer = GetCellValue(row.GetCell(ExcelColumnNameToNumber("X") - 1));
                        string ProductFactor = GetCellValue(row.GetCell(ExcelColumnNameToNumber("Y") - 1));
                        string Comments = GetCellValue(row.GetCell(ExcelColumnNameToNumber("Z") - 1));



                        if (productid != 0)
                        {
                            #region DezineCorpFreight
                            string DezineCorpFreightQuery = string.Empty;
                            var isDezineCorpTierPrice = CheckisDezineCorpFreightRecordExist(productid);
                            if (!isDezineCorpTierPrice)
                                DezineCorpFreightQuery = "INSERT INTO [dbo].[DezineCorpFreightDimensions] (ProductId, [SKU],[Year1_Catalogue_page_number],[Year2_Catalogue_page_number],[OP_MasterPackQty],[OP_MasterPackWghtLBS],[OP_L],[OP_W],[OP_H],[OP_CW_LBS],[PP_MasterPackQty],[PP_BoxID],[PP_L],[PP_W],[PP_H],[PP_Cubed_Wght_LBS],[SP_ProdWght_LBS],[SP_PcsPerSet],[SP_GiftBox_LBS],[SP_ProdWght_and_GB_Wght],[SP_WghtOfMaster_Carton_with_product],[SP_Carton_Foam_Wght_Est_LBS],[PPMasterPack_Total_Wght_LBS],[CartonsPerSkidLayer],[ProductFactor],[Comments]) VALUES(@ProductId, @SKU, @Year1_Catalogue_page_number, @Year2_Catalogue_page_number, @OP_MasterPackQty, @OP_MasterPackWghtLBS, @OP_L, @OP_W, @OP_H, @OP_CW_LBS, @PP_MasterPackQty, @PP_BoxID, @PP_L, @PP_W, @PP_H, @PP_Cubed_Wght_LBS, @SP_ProdWght_LBS, @SP_PcsPerSet, @SP_GiftBox_LBS, @SP_ProdWght_and_GB_Wght, @SP_WghtOfMaster_Carton_with_product, @SP_Carton_Foam_Wght_Est_LBS, @PPMasterPack_Total_Wght_LBS, @CartonsPerSkidLayer, @ProductFactor, @Comments)";
                            else
                                DezineCorpFreightQuery = "UPDATE [dbo].[DezineCorpFreightDimensions] SET [SKU] = @SKU, [Year1_Catalogue_page_number] = @Year1_Catalogue_page_number, [Year2_Catalogue_page_number] = @Year2_Catalogue_page_number, [OP_MasterPackQty] = @OP_MasterPackQty, [OP_MasterPackWghtLBS] = @OP_MasterPackWghtLBS, [OP_L] = @OP_L, [OP_W] = @OP_W, [OP_H] = @OP_H, [OP_CW_LBS] = @OP_CW_LBS, [PP_MasterPackQty] = @PP_MasterPackQty, [PP_BoxID] = @PP_BoxID, [PP_L] = @PP_L, [PP_W] = @PP_W, [PP_H] = @PP_H, [PP_Cubed_Wght_LBS] = @PP_Cubed_Wght_LBS, [SP_ProdWght_LBS] = @SP_ProdWght_LBS, [SP_PcsPerSet] = @SP_PcsPerSet, [SP_GiftBox_LBS] = @SP_GiftBox_LBS, [SP_ProdWght_and_GB_Wght] = @SP_ProdWght_and_GB_Wght, [SP_WghtOfMaster_Carton_with_product] = @SP_WghtOfMaster_Carton_with_product, [SP_Carton_Foam_Wght_Est_LBS] = @SP_Carton_Foam_Wght_Est_LBS, [PPMasterPack_Total_Wght_LBS] = @PPMasterPack_Total_Wght_LBS, [CartonsPerSkidLayer] = @CartonsPerSkidLayer, [ProductFactor] = @ProductFactor, [Comments] = @Comments WHERE ProductId = @ProductId";

                            SqlCommand cmdtier = new SqlCommand(DezineCorpFreightQuery);
                            cmdtier.Parameters.AddWithValue("@ProductId", productid);
                            cmdtier.Parameters.AddWithValue("@SKU", SKU);
                            cmdtier.Parameters.AddWithValue("@Year1_Catalogue_page_number", Year1_Catalogue_page_number);
                            cmdtier.Parameters.AddWithValue("@Year2_Catalogue_page_number", Year2_Catalogue_page_number);
                            cmdtier.Parameters.AddWithValue("@OP_MasterPackQty", OP_MasterPackQty);
                            cmdtier.Parameters.AddWithValue("@OP_MasterPackWghtLBS", OP_MasterPackWghtLBS);
                            cmdtier.Parameters.AddWithValue("@OP_L", OP_L);
                            cmdtier.Parameters.AddWithValue("@OP_W", OP_W);
                            cmdtier.Parameters.AddWithValue("@OP_H", OP_H);
                            cmdtier.Parameters.AddWithValue("@OP_CW_LBS", OP_CW_LBS);
                            cmdtier.Parameters.AddWithValue("@PP_MasterPackQty", PP_MasterPackQty);
                            cmdtier.Parameters.AddWithValue("@PP_BoxID", PP_BoxID);
                            cmdtier.Parameters.AddWithValue("@PP_L", PP_L);
                            cmdtier.Parameters.AddWithValue("@PP_W", PP_W);
                            cmdtier.Parameters.AddWithValue("@PP_H", PP_H);
                            cmdtier.Parameters.AddWithValue("@PP_Cubed_Wght_LBS", PP_Cubed_Wght_LBS);
                            cmdtier.Parameters.AddWithValue("@SP_ProdWght_LBS", SP_ProdWght_LBS);
                            cmdtier.Parameters.AddWithValue("@SP_PcsPerSet", SP_PcsPerSet);
                            cmdtier.Parameters.AddWithValue("@SP_GiftBox_LBS", SP_GiftBox_LBS);
                            cmdtier.Parameters.AddWithValue("@SP_ProdWght_and_GB_Wght", SP_ProdWght_and_GB_Wght);
                            cmdtier.Parameters.AddWithValue("@SP_WghtOfMaster_Carton_with_product", SP_WghtOfMaster_Carton_with_product);
                            cmdtier.Parameters.AddWithValue("@SP_Carton_Foam_Wght_Est_LBS", SP_Carton_Foam_Wght_Est_LBS);
                            cmdtier.Parameters.AddWithValue("@PPMasterPack_Total_Wght_LBS", PPMasterPack_Total_Wght_LBS);
                            cmdtier.Parameters.AddWithValue("@CartonsPerSkidLayer", CartonsPerSkidLayer);
                            cmdtier.Parameters.AddWithValue("@ProductFactor", ProductFactor);
                            cmdtier.Parameters.AddWithValue("@Comments", Comments);
                            int rowCount = ExecuteQuery(cmdtier);

                            if (rowCount <= 0)
                            {
                                throw new Exception("Error in executing sql query for product ");
                            }


                            cmdtier.Dispose();
                            #endregion


                        }
                        else
                        {
                            skus += sku + "=>SKU Not available" + ", ";
                            skucount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        skus += sku + "=>" + ex.Message + ", ";
                    }
                    status.RecordProcessSucessfully = r - 2;
                    status.RecordFailed = skus;
                    UpdateStatus(status);
                }
                status.IsImportFinish = true;
                UpdateStatus(status);
            }
            catch (Exception ex)
            {
                var STRING = ex.Message + (ex.InnerException != null ? Environment.NewLine + ex.InnerException.Message : string.Empty);
            }
        }

        private Dictionary<string, string> GetColorValueFromColorCodes()
        {
            var colorCodeMapping = new Dictionary<string, string>();
            try

            {
                var colorCodeFile = Path.Combine(Path.GetDirectoryName(_path), "Dezinecorp_Extracted_Colours.csv");
                var importDirectory = File.ReadAllLines(colorCodeFile).Skip(1);

                foreach (var item in importDirectory)
                {
                    var data = item.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (data.Length <= 2)
                    {
                        colorCodeMapping.Add(data[0].Trim(), data[1].Trim());
                    }
                }

            }
            catch
            {

            }
            return colorCodeMapping;
        }

        private List<string> GetColorCodeFromSku(string sku)
        {
            var result = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(sku))
                    return result;

                if (sku.Contains("-"))
                    sku = sku.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0];


                var sku_color = sku.ToArray().SkipWhile(x => !char.IsDigit(x)).SkipWhile(x => char.IsDigit(x)).ToList();
                if (sku_color.Any())
                {
                    var totalColorCode = new string(sku_color.ToArray());
                    if (totalColorCode.Length >= 2)
                        result.Add(new string(totalColorCode.Take(2).ToArray()));
                    if (totalColorCode.Length < 4 && totalColorCode.Length > 2)
                        result.Add(new string(totalColorCode.Skip(2).Take(1).ToArray()));
                    if (totalColorCode.Length >= 4)
                        result.Add(new string(totalColorCode.Skip(2).Take(2).ToArray()));

                }
            }
            catch
            {

            }

            return result;
        }

        private void SetProductToDelete(IEnumerable<string> skutosetdelete)
        {
            using (SqlConnection con = new SqlConnection())
            {
                try
                {
                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    con.Open();

                    foreach (var sku in skutosetdelete)
                    {
                        cmd.CommandText = "update Product set Deleted = 1 where SKU = '@SKU'";
                        cmd.Parameters.AddWithValue("@SKU", sku);
                        var res = cmd.ExecuteNonQuery();
                    }
                    con.Close();
                    con.Dispose();
                }
                catch (Exception ex)
                {
                    con.Close();
                    con.Dispose();
                }
            }
        }

        private List<string> GetSkuFromNopCommerce()
        {
            using (SqlConnection con = new SqlConnection())
            {
                List<string> result = new List<string>();

                try
                {
                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "select sku from product";
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                        while (reader.Read())
                            result.Add(Convert.ToString(reader["sku"]));
                    con.Close();
                    con.Dispose();
                }
                catch (Exception ex)
                {
                    con.Close();
                    con.Dispose();
                }
                return result;

            }
        }

        private DataTable GetProductBySKU(string sku)
        {
            int i = 0;
            using (SqlConnection con = new SqlConnection())
            {
                try
                {
                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    //cmd.CommandText = "select * from Product where SKU = @SKU and Deleted = 0 order by CreatedOnUtc desc";
                    //"SELECT ProductID FROM dbo.Nop_ProductVariant AS npv WHERE SKU COLLATE Latin1_General_CS_AS ='" + sku + "'";
                    cmd.CommandText = "exec [dbo].[getProductForDezineCorpImport] @SKU";
                    //cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SKU", sku);
                    cmd.Connection = con;
                    con.Open();
                    DataTable dataTable = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    // this will query your database and return the result to your datatable
                    da.Fill(dataTable);
                    con.Close();
                    da.Dispose();

                    return dataTable;
                    //i = Convert.ToInt32(cmd.ExecuteScalar());
                    //con.Close();
                    //con.Dispose();
                }

                catch (SqlException Ex)
                {

                    con.Close();
                    con.Dispose();
                    System.Diagnostics.Trace.WriteLine("CommonDBClass Has Exception " + Ex.Message);
                }
            }
            return null;
        }

        private string GetPictures(int id)
        {

            var picUrl = string.Empty;
            using (SqlConnection con = new SqlConnection())
            {
                try
                {
                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    //cmd.CommandText = "select * from Product where SKU = @SKU and Deleted = 0 order by CreatedOnUtc desc";
                    //"SELECT ProductID FROM dbo.Nop_ProductVariant AS npv WHERE SKU COLLATE Latin1_General_CS_AS ='" + sku + "'";
                    cmd.CommandText = "select * from Product_Picture_Mapping ppm join Picture p on ppm.PictureId = p.Id where ppm.ProductId =  @productId";
                    //cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@productId", id);
                    cmd.Connection = con;
                    con.Open();
                    DataTable dataTable = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    // this will query your database and return the result to your datatable
                    da.Fill(dataTable);

                    if (dataTable != null)
                    {
                        if (dataTable.Rows.Count > 0)
                        {
                            var seoFileName = dataTable.Rows[0]["SeoFilename"] ?? string.Empty;
                            var mimeType = dataTable.Rows[0]["MimeType"] ?? string.Empty;
                            var pictureId = Convert.ToInt32(dataTable.Rows[0]["PictureId"]);
                            string lastPart = GetFileExtensionFromMimeType(mimeType.ToString());
                            picUrl = !string.IsNullOrEmpty(seoFileName.ToString()) ?
                                          string.Format("http://dezinecorp.com/content/images/thumbs/{0}_{1}.{2}", pictureId.ToString("0000000"), seoFileName, lastPart) :
                                          string.Format("http://dezinecorp.com/content/images/thumbs/{0}.{1}", pictureId.ToString("0000000"), lastPart);
                        }
                    }

                    con.Close();
                    da.Dispose();

                    return picUrl;
                    //i = Convert.ToInt32(cmd.ExecuteScalar());
                    //con.Close();
                    //con.Dispose();
                }

                catch (SqlException Ex)
                {

                    con.Close();
                    con.Dispose();
                    System.Diagnostics.Trace.WriteLine("CommonDBClass Has Exception " + Ex.Message);
                }
            }
            return null;

        }

        protected static string GetFileExtensionFromMimeType(string mimeType)
        {
            if (mimeType == null)
                return null;

            //also see System.Web.MimeMapping for more mime types

            string[] parts = mimeType.Split('/');
            string lastPart = parts[parts.Length - 1];
            switch (lastPart)
            {
                case "pjpeg":
                    lastPart = "jpg";
                    break;
                case "x-png":
                    lastPart = "png";
                    break;
                case "x-icon":
                    lastPart = "ico";
                    break;
            }
            return lastPart;
        }

        private int ExecuteQuery(SqlCommand cmd)
        {
            string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ConnectionString;
            SqlConnection con = new SqlConnection(strConnString);
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            try
            {
                con.Open();
                var id = cmd.ExecuteNonQuery();
                return Convert.ToInt32(id);
            }
            catch (Exception e)
            {
                return 0;
            }
            finally
            {
                con.Close();
                cmd.Dispose();
            }
        }

        private bool CheckisDezineCorpSageandBrandingExist(int productid)
        {
            int i = 0;
            using (SqlConnection con = new SqlConnection())
            {
                try
                {

                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "select Id from  DezinecorpSageandBrandingData where ProductId = @productid";
                    cmd.Parameters.AddWithValue("@productid", productid);
                    cmd.Connection = con;
                    con.Open();


                    i = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                    con.Dispose();
                }

                catch (SqlException Ex)
                {
                    con.Close();
                    con.Dispose();
                    System.Diagnostics.Trace.WriteLine("CommonDBClass Has Exception " + Ex.Message);
                }
            }
            return i == 0 ? false : true;
        }

        private bool CheckisDezineCorpAdditionalPricingExist(int productid)
        {
            int i = 0;
            using (SqlConnection con = new SqlConnection())
            {
                try
                {

                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "select Id from  DezineCorpAdditionalPricing where ProductId = @productid";
                    cmd.Parameters.AddWithValue("@productid", productid);
                    cmd.Connection = con;
                    con.Open();


                    i = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                    con.Dispose();
                }

                catch (SqlException Ex)
                {
                    con.Close();
                    con.Dispose();
                    System.Diagnostics.Trace.WriteLine("CommonDBClass Has Exception " + Ex.Message);
                }
            }
            return i == 0 ? false : true;
        }

        private bool CheckisDezineCorpTierPriceRecordExist(int productid)
        {
            int i = 0;
            using (SqlConnection con = new SqlConnection())
            {
                try
                {

                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "select Id from  DezineCorpTierPrice where ProductId = @productid";
                    //"SELECT ProductID FROM dbo.Nop_ProductVariant AS npv WHERE SKU COLLATE Latin1_General_CS_AS ='" + sku + "'";
                    cmd.Parameters.AddWithValue("@productid", productid);
                    cmd.Connection = con;
                    con.Open();


                    i = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                    con.Dispose();
                }

                catch (SqlException Ex)
                {

                    con.Close();
                    con.Dispose();
                    System.Diagnostics.Trace.WriteLine("CommonDBClass Has Exception " + Ex.Message);
                }
            }
            return i == 0 ? false : true;
        }

        private bool CheckisDezineCorpFreightRecordExist(int productid)
        {
            int i = 0;
            using (SqlConnection con = new SqlConnection())
            {
                try
                {

                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "select Id from  DezineCorpFreightDimensions where ProductId = @productid";
                    //"SELECT ProductID FROM dbo.Nop_ProductVariant AS npv WHERE SKU COLLATE Latin1_General_CS_AS ='" + sku + "'";
                    cmd.Parameters.AddWithValue("@productid", productid);
                    cmd.Connection = con;
                    con.Open();


                    i = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                    con.Dispose();
                }

                catch (SqlException Ex)
                {

                    con.Close();
                    con.Dispose();
                    System.Diagnostics.Trace.WriteLine("CommonDBClass Has Exception " + Ex.Message);
                }
            }
            return i == 0 ? false : true;
        }
        
        private bool CheckisDezineCorpRelatedProductQueryRecordExist(int productid)
        {
            int i = 0;
            using (SqlConnection con = new SqlConnection())
            {
                try
                {

                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "select Id from  DezineCorpRelatedProduct where ProductId = @productid";
                    //"SELECT ProductID FROM dbo.Nop_ProductVariant AS npv WHERE SKU COLLATE Latin1_General_CS_AS ='" + sku + "'";
                    cmd.Parameters.AddWithValue("@productid", productid);
                    cmd.Connection = con;
                    con.Open();


                    i = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                    con.Dispose();
                }

                catch (SqlException Ex)
                {

                    con.Close();
                    con.Dispose();
                    System.Diagnostics.Trace.WriteLine("CommonDBClass Has Exception " + Ex.Message);
                }
            }
            return i == 0 ? false : true;
        }

        private bool CheckisDezineCorpProductKeywordQueryQueryRecordExist(int productid)
        {
            int i = 0;
            using (SqlConnection con = new SqlConnection())
            {
                try
                {

                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "select Id from  DezineCorpProductKeyword where ProductId = @productid";
                    //"SELECT ProductID FROM dbo.Nop_ProductVariant AS npv WHERE SKU COLLATE Latin1_General_CS_AS ='" + sku + "'";
                    cmd.Parameters.AddWithValue("@productid", productid);
                    cmd.Connection = con;
                    con.Open();


                    i = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                    con.Dispose();
                }

                catch (SqlException Ex)
                {

                    con.Close();
                    con.Dispose();
                    System.Diagnostics.Trace.WriteLine("CommonDBClass Has Exception " + Ex.Message);
                }
            }
            return i == 0 ? false : true;
        }

        private bool CheckisDezineCorpDataRefQueryRecordExist(int productid)
        {
            int i = 0;
            using (SqlConnection con = new SqlConnection())
            {
                try
                {

                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "select Id from  DezineCorpDataRefOnly where ProductId = @productid";
                    //"SELECT ProductID FROM dbo.Nop_ProductVariant AS npv WHERE SKU COLLATE Latin1_General_CS_AS ='" + sku + "'";
                    cmd.Parameters.AddWithValue("@productid", productid);
                    cmd.Connection = con;
                    con.Open();


                    i = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                    con.Dispose();
                }

                catch (SqlException Ex)
                {

                    con.Close();
                    con.Dispose();
                    System.Diagnostics.Trace.WriteLine("CommonDBClass Has Exception " + Ex.Message);
                }
            }
            return i == 0 ? false : true;
        }

        private bool CheckDezineCorpDataRecordExist(int productid)
        {
            int i = 0;
            using (SqlConnection con = new SqlConnection())
            {
                try
                {

                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "select Id from  DezineCorpData where ProductId = @productid";
                    //"SELECT ProductID FROM dbo.Nop_ProductVariant AS npv WHERE SKU COLLATE Latin1_General_CS_AS ='" + sku + "'";
                    cmd.Parameters.AddWithValue("@productid", productid);
                    cmd.Connection = con;
                    con.Open();


                    i = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();
                    con.Dispose();
                }

                catch (SqlException Ex)
                {

                    con.Close();
                    con.Dispose();
                    System.Diagnostics.Trace.WriteLine("CommonDBClass Has Exception " + Ex.Message);
                }
            }
            return i == 0 ? false : true;
        }
    }
}
