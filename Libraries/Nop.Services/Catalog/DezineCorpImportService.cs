using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
            string importStatusQuery = "delete from ImportStatus;insert into ImportStatus values('" + status.TotalRecord+"', '"+status.RecordProcessSucessfully+"', '"+status.RecordFailed+"', '"+status.IsImportFinish+"')";

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
                            RecordFailed = reader["RecordFailed"].ToString()
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

        public void READExcel()
        {
            //Instance reference for Excel Application
            Microsoft.Office.Interop.Excel.Application objXL = null;
            //Workbook refrence
            Microsoft.Office.Interop.Excel.Workbook objWB = null;
            ImportStatus status = new ImportStatus();
            try
            {
                //Instancing Excel using COM services
                objXL = new Microsoft.Office.Interop.Excel.Application();
                //Adding WorkBook
                if (string.IsNullOrEmpty(_path)) return;

                objWB = objXL.Workbooks.Open(_path);

                Microsoft.Office.Interop.Excel.Worksheet objSHT = objWB.Sheets["MASTER"];
                int rows = objSHT.UsedRange.Rows.Count;
                int cols = objSHT.UsedRange.Columns.Count;
                int noofrow = 2;
                status.TotalRecord = rows;
                status.IsImportFinish = false;
                string skus = string.Empty;
                int skucount = 0;
                for (int r = noofrow; r <= rows; r++)
                {
                    var sku = objSHT.Cells[r, "A"].Text;

                    if (string.IsNullOrEmpty(sku)) continue;

                    int productid = GetProductIdBySKU(sku);

                    string NewPage = objSHT.Cells[r, "C"].Text;
                    string ItemIsNew = objSHT.Cells[r, "AI"].Text;
                    string GuarenteedStock = objSHT.Cells[r, "AJ"].Text;
                    string Materials = objSHT.Cells[r, "AK"].Text;
                    string Features = objSHT.Cells[r, "AM"].Text;
                    string Includes = objSHT.Cells[r, "AN"].Text;
                    string SpecailPackaging = objSHT.Cells[r, "AO"].Text;
                    string Capacity = objSHT.Cells[r, "AP"].Text;
                    string Size = objSHT.Cells[r, "AQ"].Text;
                    string ImprintAreaInOutboard = objSHT.Cells[r, "AR"].Text;
                    string ImprintAreaWrapAround = objSHT.Cells[r, "AS"].Text;
                    string DecoratingOption = objSHT.Cells[r, "AT"].Text;
                    string PeicesPerCartoon = objSHT.Cells[r, "AU"].Text;
                    string WeightPerCartoon = objSHT.Cells[r, "AV"].Text;
                    string BlankLine = objSHT.Cells[r, "AW"].Text;
                    string ProtectivePackaging = objSHT.Cells[r, "AX"].Text;
                    string ReferToCataloguePage = objSHT.Cells[r, "AY"].Text;
                    string PricingFlag = objSHT.Cells[r, "AZ"].Text;
                    string MadeinCanada = objSHT.Cells[r, "BA"].Text;
                    string MadeinNorthAmerica = objSHT.Cells[r, "BB"].Text;
                    string InventoryFlag = objSHT.Cells[r, "BC"].Text;
                    string PricingCode = objSHT.Cells[r, "BD"].Text;
                    string PricingFooterNote = objSHT.Cells[r, "BW"].Text;
                    string SetupPerColour = objSHT.Cells[r, "BX"].Text;
                    string RepeatSetup = objSHT.Cells[r, "BY"].Text;
                    string DebossSetup = objSHT.Cells[r, "BZ"].Text;
                    string RepeatDeboss = objSHT.Cells[r, "CA"].Text;
                    string DecalSetup = objSHT.Cells[r, "CB"].Text;
                    string RepeatDecal = objSHT.Cells[r, "CC"].Text;
                    string LaserSetup = objSHT.Cells[r, "CD"].Text;
                    string RepeatLaser = objSHT.Cells[r, "CE"].Text;
                    string AdditionalCharge1 = objSHT.Cells[r, "CF"].Text;
                    string AdditionalCharge2 = objSHT.Cells[r, "CG"].Text;
                    string AdditionalCharge3 = objSHT.Cells[r, "CH"].Text;
                    string AdditionalCharge4 = objSHT.Cells[r, "CI"].Text;
                    string RepeatTerm = objSHT.Cells[r, "CJ"].Text;
                    string FinalNote = objSHT.Cells[r, "CK"].Text;
                    string RegularPrice = objSHT.Cells[r, "CU"].Text;
                    string RegularPrice1 = objSHT.Cells[r, "CV"].Text;
                    string RegularPrice2 = objSHT.Cells[r, "CW"].Text;
                    string RegularPrice3 = objSHT.Cells[r, "CX"].Text;
                    string RegularPrice4 = objSHT.Cells[r, "CY"].Text;
                    string RegularPriceCode = objSHT.Cells[r, "CZ"].Text;
                    string SpecialPriceEnds = objSHT.Cells[r, "DA"].Text;
                    string CartonDimensions = objSHT.Cells[r, "DB"].Text;
                    string VisualHeading = objSHT.Cells[r, "CR"].Text;
                    string FamilyCode = objSHT.Cells[r, "CT"].Text;
                    string VisualPrice = objSHT.Cells[r, "CS"].Text;

                    string OldPage2012 = objSHT.Cells[r, "B"].Text;
                    string Net1 = objSHT.Cells[r, "O"].Text;
                    string Net2 = objSHT.Cells[r, "P"].Text;
                    string Net3 = objSHT.Cells[r, "Q"].Text;
                    string Net4 = objSHT.Cells[r, "R"].Text;
                    string Net5 = objSHT.Cells[r, "S"].Text;
                    string Net6 = objSHT.Cells[r, "T"].Text;
                    string Net7 = objSHT.Cells[r, "U"].Text;
                    string Net8 = objSHT.Cells[r, "V"].Text;
                    string LOWESTINVOICEVALUEEQPMOQ = objSHT.Cells[r, "W"].Text;
                    string CurrentEQP = objSHT.Cells[r, "X"].Text;
                    string CurrentEQPLess5PerCent = objSHT.Cells[r, "Y"].Text;
                    string Change2010to2011EQPtoEQP = objSHT.Cells[r, "Z"].Text;
                    string CountryofOrigin = objSHT.Cells[r, "AA"].Text;
                    string HSCode = objSHT.Cells[r, "AB"].Text;
                    string MasterPack = objSHT.Cells[r, "AC"].Text;
                    string L = objSHT.Cells[r, "AD"].Text;
                    string W = objSHT.Cells[r, "AE"].Text;
                    string H = objSHT.Cells[r, "AF"].Text;
                    string Volume = objSHT.Cells[r, "AG"].Text;
                    string FreightUnit = objSHT.Cells[r, "AH"].Text;
                    string DateRevised = objSHT.Cells[r, "DC"].Text;
                    string RevisedBy = objSHT.Cells[r, "DD"].Text;
                    string InternalComments = objSHT.Cells[r, "DE"].Text;
                    string PPPCNotes = objSHT.Cells[r, "DF"].Text;
                    string DezineCategory = objSHT.Cells[r, "DG"].Text;
                    string INFOtracImportResultifError = objSHT.Cells[r, "DP"].Text;

                    string Keyword_1 = objSHT.Cells[r, "DH"].Text;
                    string Keyword_2 = objSHT.Cells[r, "DI"].Text;
                    string Keyword_3 = objSHT.Cells[r, "DJ"].Text;
                    string Keyword_4 = objSHT.Cells[r, "DK"].Text;
                    string Keyword_5 = objSHT.Cells[r, "DL"].Text;
                    string Keyword_6 = objSHT.Cells[r, "DM"].Text;
                    string Keyword_Color = objSHT.Cells[r, "DM"].Text;
                    string keyword_Linename = objSHT.Cells[r, "DO"].Text;
                    string Keyword_Colour_Primary = string.Empty;
                    string Keyword_Colour_Secondary = string.Empty;

                    string Related_1 = objSHT.Cells[r, "CL"].Text;
                    string Related_2 = objSHT.Cells[r, "CM"].Text;
                    string Related_3 = objSHT.Cells[r, "CN"].Text;
                    string Related_4 = objSHT.Cells[r, "CO"].Text;
                    string Related_5 = objSHT.Cells[r, "CP"].Text;
                    string Related_6 = objSHT.Cells[r, "CQ"].Text;

                    string QuantityLevel = objSHT.Cells[r, "E"].Text;
                    string Price1 = objSHT.Cells[r, "F"].Text;
                    string Price2 = objSHT.Cells[r, "G"].Text;
                    string Price3 = objSHT.Cells[r, "H"].Text;
                    string Price4 = objSHT.Cells[r, "I"].Text;
                    string Price5 = objSHT.Cells[r, "J"].Text;
                    string Price6 = objSHT.Cells[r, "K"].Text;
                    string Price7 = objSHT.Cells[r, "L"].Text;
                    string Price8 = objSHT.Cells[r, "M"].Text;
                    string DiscountCode = objSHT.Cells[r, "N"].Text;

                    string AddColourOption = objSHT.Cells[r, "BE"].Text;
                    string AddCol_1 = objSHT.Cells[r, "BF"].Text;
                    string AddCol_2 = objSHT.Cells[r, "BG"].Text;
                    string AddCol_3 = objSHT.Cells[r, "BH"].Text;
                    string AddCol_4 = objSHT.Cells[r, "BI"].Text;
                    string AddColPriceCode = objSHT.Cells[r, "BJ"].Text;
                    string DecalOption = objSHT.Cells[r, "BK"].Text;
                    string Decal_1 = objSHT.Cells[r, "BL"].Text;
                    string Decal_2 = objSHT.Cells[r, "BM"].Text;
                    string Decal_3 = objSHT.Cells[r, "BN"].Text;
                    string Decal_4 = objSHT.Cells[r, "BO"].Text;
                    string DecalPriceCode = objSHT.Cells[r, "BP"].Text;
                    string LaserEngravingOption = objSHT.Cells[r, "BQ"].Text;
                    string Laser_1 = objSHT.Cells[r, "BR"].Text;
                    string Laser_2 = objSHT.Cells[r, "BS"].Text;
                    string Laser_3 = objSHT.Cells[r, "BT"].Text;
                    string Laser_4 = objSHT.Cells[r, "BU"].Text;
                    string LaserPriceCode = objSHT.Cells[r, "BV"].Text;

                    if (productid != 0)
                    {

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
                        ExecuteQuery(cmd);

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
                        cmdref.Parameters.AddWithValue("@InternalComments", InternalComments);
                        cmdref.Parameters.AddWithValue("@PPPCNotes", PPPCNotes);
                        cmdref.Parameters.AddWithValue("@DezineCategory", DezineCategory);
                        cmdref.Parameters.AddWithValue("@INFOtracImportResultifError", INFOtracImportResultifError);
                        ExecuteQuery(cmdref);
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

                        ExecuteQuery(cmdkey);

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
                        ExecuteQuery(cmdrel);

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
                        ExecuteQuery(cmdtier);
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

                        ExecuteQuery(cmdadpr);

                        cmdadpr.Dispose();
                        #endregion
                        
                    }
                    else
                    {
                        skus +=  sku + ", ";
                        skucount++;
                    }

                    status.RecordProcessSucessfully = r;
                    status.RecordFailed = skus;
                    UpdateStatus(status);
                }
                //ds.Tables.Add(dt);

                status.IsImportFinish = true;
                UpdateStatus(status);
                //Closing workbook
                objWB.Close();
                //Closing excel application
                objXL.Quit();

            }
            catch (Exception ex)
            {
                objWB.Saved = true;
                //Closing work book
                objWB.Close();
                //Closing excel application
                objXL.Quit();
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
                    cmd.CommandText = "select Id from Product where SKU = @SKU and Deleted = 0 order by UpdatedOnUtc";
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
