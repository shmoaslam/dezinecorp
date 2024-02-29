using Nop.Core.Data;
using Nop.Data;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using OfficeOpenXml.FormulaParsing.Utilities;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;

namespace Nop.Services
{
    public partial class InfotracDbService : IInfotracDbService
    {
        public string connectionStr = "Server=dezine.fortiddns.com;Port=5433;Database=infotrac;User Id=website;Password=32@D4$wo5asfd2;";
        public InfotracDbService()
        {
        

        }

        public async Task<IEnumerable<QuoteDependentData>> GetDimentionData(string sku, int quantity)
        {
            var query = "select 'PPK' as Type, round((@p2 / ppk_ppc)::decimal, 2) as Qty, ppk_ppc as PcsPerCart, ppk_masterctnid as Carton, ppk_l As L, ppk_w As W, ppk_h As H, ppk_cartonwht As Weight, ppk_layersperskid as CrtnsPerSkidLayer from oeinvtitems where id = @p1";


            var result = new List<QuoteDependentData>();
            using (var conn = new NpgsqlConnection(connectionStr))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p1", $"01{sku}"));
                    cmd.Parameters.Add(new NpgsqlParameter("p2", quantity));
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                            result.Add(new QuoteDependentData
                            {
                                Type = reader.IsDBNull(0) ? null : reader.GetString(0),
                                Quantity = reader.IsDBNull(1) ? new int?() : reader.GetInt32(1),
                                PiecePerCart = reader.IsDBNull(2) ? new double?() : reader.GetDouble(2),
                                Cartoon = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Length = reader.IsDBNull(4) ? new double?() : reader.GetDouble(4),
                                Width = reader.IsDBNull(5) ? new double?() : reader.GetDouble(5),
                                Height = reader.IsDBNull(6) ? new double?() : reader.GetDouble(6),
                                Weight = reader.IsDBNull(7) ? new double?() : reader.GetDouble(7),
                                CartoonPerSkidLayer = reader.IsDBNull(8) ? new double?() : reader.GetDouble(8),
                            });


                }

            }

            return result;
        }

        public async Task<IEnumerable<InfotracFreightFactor>> GetInfotracFreightFactors()
        {
            var query = "select section, key, data from settings where category = 'FREIGHTFACTORS.INI' order by 1, 2, 3";


            var result = new List<InfotracFreightFactor>();
            using (var conn = new NpgsqlConnection(connectionStr))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                            result.Add(new InfotracFreightFactor
                            {
                                Section = reader.IsDBNull(0) ? null : reader.GetString(0),
                                Key = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Data = reader.IsDBNull(2) ? new float?() : Convert.ToSingle( reader.GetString(2)),
                            });


                }

            }

            return result;
        }
    }
}
