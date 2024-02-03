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


namespace Nop.Services
{
    public partial class InfotracDbService : IInfotracDbService
    {
        public string connectionStr = "Server=dezine.fortiddns.com;Port=5433;Database=infotrac;User Id=website;Password=32@D4$wo5asfd2;";
        public InfotracDbService()
        {
        

        }

        public async Task<IEnumerable<QuoteDependentData>> GetData(string sku)
        {
            var query = "select 'PPK' as Type, round((288 / ppk_ppc)::decimal, 2) as Qty, ppk_ppc as PcsPerCart, ppk_masterctnid as Carton, ppk_l As L, ppk_w As W, ppk_h As H, ppk_cartonwht As Weight, ppk_layersperskid as CrtnsPerSkidLayer from oeinvtitems where id = @p1";


            var result = new List<QuoteDependentData>();
            using (var conn = new NpgsqlConnection(connectionStr))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter("p1", sku));

                    using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                            result.Add(new QuoteDependentData
                            {
                                Type = reader.GetString(0),
                                Quantity = reader.GetInt32(1),
                                PiecePerCart = reader.GetDouble(2),
                                Cartoon = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Length = reader.GetDouble(4),
                                Width = reader.GetDouble(5),
                                Height = reader.GetDouble(6),
                                Weight = reader.GetDouble(7),
                                CartoonPerSkidLayer = reader.IsDBNull(8) ? null : reader.GetString(8),
                            });


                }

            }

            return result;
        }
    }
}
