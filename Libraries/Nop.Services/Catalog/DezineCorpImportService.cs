
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
                FileStream stream = new FileStream(_path, FileMode.Open, FileAccess.Read);
                HSSFWorkbook wb = new HSSFWorkbook(stream);
                ISheet ws = wb.GetSheet("MASTER");
                int startFromRowNumber = 3; // starting from 0, ignore first 3 rows
                int rows = ws.LastRowNum;
                status.TotalRecord = rows - 3;
                status.IsImportFinish = false;
                string skus = string.Empty;
                int skucount = 0;



                //Instancing Excel using COM services
                //objXL = new Microsoft.Office.Interop.Excel.Application();
                //Adding WorkBook
                if (string.IsNullOrEmpty(_path)) return;

                //objWB = objXL.Workbooks.Open(_path);

                //Microsoft.Office.Interop.Excel.Worksheet objSHT = objWB.Sheets["MASTER"];
                //int rows = objSHT.UsedRange.Rows.Count;
                //int cols = objSHT.UsedRange.Columns.Count;



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

                        int productid = GetProductIdBySKU(sku);

                        string NewPage = GetCellValue(row.GetCell(ExcelColumnNameToNumber("C") - 1));
                        string ShortDescription = GetCellValue(row.GetCell(ExcelColumnNameToNumber("D") - 1));
                        string ItemIsNew = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AI") - 1));
                        string GuarenteedStock = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AJ") - 1));
                        string Materials = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AK") - 1));
                        string Features = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AM") - 1));
                        string Includes = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AN") - 1));
                        string ShortName = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AL") - 1));
                        string SpecailPackaging = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AO") - 1));
                        string Capacity = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AP") - 1));
                        string Size = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AQ") - 1));
                        string ImprintAreaInOutboard = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AR") - 1));
                        string ImprintAreaWrapAround = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AS") - 1));
                        string DecoratingOption = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AT") - 1));
                        string PeicesPerCartoon = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AU") - 1));
                        string WeightPerCartoon = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AV") - 1));
                        string BlankLine = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AW") - 1));
                        string ProtectivePackaging = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AX") - 1));
                        string ReferToCataloguePage = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AY") - 1));
                        string PricingFlag = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AZ") - 1));
                        string MadeinCanada = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BA") - 1));
                        string MadeinNorthAmerica = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BB") - 1));
                        string InventoryFlag = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BC") - 1));
                        string PricingCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BD") - 1));
                        string PricingFooterNote = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BW") - 1));
                        string SetupPerColour = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BX") - 1));
                        string RepeatSetup = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BY") - 1));
                        string DebossSetup = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BZ") - 1));
                        string RepeatDeboss = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CA") - 1));
                        string DecalSetup = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CB") - 1));
                        string RepeatDecal = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CC") - 1));
                        string LaserSetup = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CD") - 1));
                        string RepeatLaser = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CE") - 1));
                        string AdditionalCharge1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CF") - 1));
                        string AdditionalCharge2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CG") - 1));
                        string AdditionalCharge3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CH") - 1));
                        string AdditionalCharge4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CI") - 1));
                        string RepeatTerm = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CJ") - 1));
                        string FinalNote = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CK") - 1));
                        string RegularPrice = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CU") - 1));
                        string RegularPrice1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CV") - 1));
                        string RegularPrice2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CW") - 1));
                        string RegularPrice3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CX") - 1));
                        string RegularPrice4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CY") - 1));
                        string RegularPriceCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CZ") - 1));
                        string SpecialPriceEnds = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DA") - 1));
                        string CartonDimensions = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DB") - 1));
                        string VisualHeading = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CR") - 1));
                        string FamilyCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CT") - 1));
                        string VisualPrice = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CS") - 1));

                        string OldPage2012 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("B") - 1));
                        string Net1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("O") - 1));
                        string Net2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("P") - 1));
                        string Net3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("Q") - 1));
                        string Net4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("R") - 1));
                        string Net5 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("S") - 1));
                        string Net6 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("T") - 1));
                        string Net7 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("U") - 1));
                        string Net8 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("V") - 1));
                        string LOWESTINVOICEVALUEEQPMOQ = GetCellValue(row.GetCell(ExcelColumnNameToNumber("W") - 1));
                        string CurrentEQP = GetCellValue(row.GetCell(ExcelColumnNameToNumber("X") - 1));
                        string CurrentEQPLess5PerCent = GetCellValue(row.GetCell(ExcelColumnNameToNumber("Y") - 1));
                        string Change2010to2011EQPtoEQP = GetCellValue(row.GetCell(ExcelColumnNameToNumber("Z") - 1));
                        string CountryofOrigin = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AA") - 1));
                        string HSCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AB") - 1));
                        string MasterPack = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AC") - 1));
                        string L = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AD") - 1));
                        string W = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AE") - 1));
                        string H = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AF") - 1));
                        string Volume = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AG") - 1));
                        string FreightUnit = GetCellValue(row.GetCell(ExcelColumnNameToNumber("AH") - 1));
                        string DateRevised = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DC") - 1));
                        string RevisedBy = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DD") - 1));
                        string InternalComments = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DE") - 1));
                        string PPPCNotes = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DF") - 1));
                        string DezineCategory = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DG") - 1));
                        string INFOtracImportResultifError = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DP") - 1));

                        string Keyword_1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DH") - 1));
                        string Keyword_2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DI") - 1));
                        string Keyword_3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DJ") - 1));
                        string Keyword_4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DK") - 1));
                        string Keyword_5 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DL") - 1));
                        string Keyword_6 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DM") - 1));
                        string Keyword_Color = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DM") - 1));
                        string keyword_Linename = GetCellValue(row.GetCell(ExcelColumnNameToNumber("DO") - 1));
                        string Keyword_Colour_Primary = string.Empty;
                        string Keyword_Colour_Secondary = string.Empty;

                        string Related_1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CL") - 1));
                        string Related_2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CM") - 1));
                        string Related_3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CN") - 1));
                        string Related_4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CO") - 1));
                        string Related_5 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CP") - 1));
                        string Related_6 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("CQ") - 1));

                        string QuantityLevel = GetCellValue(row.GetCell(ExcelColumnNameToNumber("E") - 1));
                        string Price1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("F") - 1));
                        string Price2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("G") - 1));
                        string Price3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("H") - 1));
                        string Price4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("I") - 1));
                        string Price5 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("J") - 1));
                        string Price6 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("K") - 1));
                        string Price7 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("L") - 1));
                        string Price8 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("M") - 1));
                        string DiscountCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("N") - 1));

                        string AddColourOption = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BE") - 1));
                        string AddCol_1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BF") - 1)).Replace("$", "").Trim();
                        string AddCol_2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BG") - 1)).Replace("$", "").Trim();
                        string AddCol_3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BH") - 1)).Replace("$", "").Trim();
                        string AddCol_4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BI") - 1)).Replace("$", "").Trim();
                        string AddColPriceCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BJ") - 1));
                        string DecalOption = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BK") - 1)).Replace("$", "").Trim();
                        string Decal_1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BL") - 1)).Replace("$", "").Trim();
                        string Decal_2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BM") - 1)).Replace("$", "").Trim();
                        string Decal_3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BN") - 1)).Replace("$", "").Trim();
                        string Decal_4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BO") - 1)).Replace("$", "").Trim();
                        string DecalPriceCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BP") - 1));
                        string LaserEngravingOption = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BQ") - 1));
                        string Laser_1 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BR") - 1)).Replace("$", "").Trim();
                        string Laser_2 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BS") - 1)).Replace("$", "").Trim();
                        string Laser_3 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BT") - 1)).Replace("$", "").Trim();
                        string Laser_4 = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BU") - 1)).Replace("$", "").Trim();
                        string LaserPriceCode = GetCellValue(row.GetCell(ExcelColumnNameToNumber("BV") - 1));



                        if (productid != 0)
                        {
                            string productQuery = "Update Product set Name = @Name, ShortDescription = @ShortDescription, HasTierPrices = 1, FamilyCode = @FamilyCode, Price = @price where sku = @sku";
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
                                DezineCorpDataRefQuery = "insert into DezineCorpDataRefOnly values(  @ProductId, @OldPage2012, @Net1, @Net2, @Net3, @Net4, @Net5, @Net6, @Net7, @Net8, @LOWESTINVOICEVALUEEQPMOQ, @CurrentEQP, @CurrentEQPLess5PerCent, @Change2010to2011EQPtoEQP, @CountryofOrigin, @HSCode, @MasterPack, @L, @W, @H, @Volume, @FreightUnit, @DateRevised, @RevisedBy, @InternalComments, @PPPCNotes, @DezineCategory, @INFOtracImportResultifError)";
                            else
                                DezineCorpDataRefQuery = "update DezineCorpDataRefOnly set OldPage2012=@OldPage2012, Net1=@Net1, Net2=@Net2, Net3=@Net3, Net4=@Net4, Net5=@Net5, Net6=@Net6, Net7=@Net7, Net8=@Net8, LOWESTINVOICEVALUEEQPMOQ=@LOWESTINVOICEVALUEEQPMOQ, CurrentEQP=@CurrentEQP, CurrentEQPLess5PerCent=@CurrentEQPLess5PerCent, Change2010to2011EQPtoEQP=@Change2010to2011EQPtoEQP, CountryofOrigin=@CountryofOrigin, HSCode=@HSCode, MasterPack=@MasterPack, L=@L, W=@W, H=@H, Volume=@Volume, FreightUnit=@FreightUnit, DateRevised=@DateRevised, RevisedBy=@RevisedBy, InternalComments=@InternalComments, PPPCNotes=@PPPCNotes, DezineCategory=@DezineCategory, INFOtracImportResultifError=@INFOtracImportResultifError where ProductId = @ProductId";

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
                            cmdref.Parameters.AddWithValue("@InternalComments", InternalComments.Length >= 450 ? InternalComments.Substring(0,450) + "..." : InternalComments);
                            cmdref.Parameters.AddWithValue("@PPPCNotes", PPPCNotes);
                            cmdref.Parameters.AddWithValue("@DezineCategory", DezineCategory);
                            cmdref.Parameters.AddWithValue("@INFOtracImportResultifError", INFOtracImportResultifError);
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
                    status.RecordProcessSucessfully = r;
                    status.RecordFailed = skus;
                    UpdateStatus(status);

                }
                //ds.Tables.Add(dt);
                //ws.Dispose();

                status.IsImportFinish = true;
                UpdateStatus(status);
                //Closing workbook
                //objWB.Close();
                //Closing excel application
                //objXL.Quit();

            }
            catch (Exception ex)
            {
                //objWB.Saved = true;
                //Closing work book
                //objWB.Close();
                //Closing excel application
                //objXL.Quit();
                //Response.Write("Illegal permission");
                var STRING = ex.Message;
            }
        }

        private int GetProductIdBySKU(string sku)
        {
            int i = 0;
            using (SqlConnection con = new SqlConnection())
            {
                try
                {
                    con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conString"].ToString();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = "select Id from Product where SKU = @SKU and Deleted = 0 order by CreatedOnUtc desc";
                    //"SELECT ProductID FROM dbo.Nop_ProductVariant AS npv WHERE SKU COLLATE Latin1_General_CS_AS ='" + sku + "'";
                    cmd.Parameters.AddWithValue("@SKU", sku);
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
            return i;
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
