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

namespace Nop.Services
{
    public partial class InfotracDbService : IInfotracDbService
    {

        public InfotracDbService(IRepositoryInfotrac repositoryInfotrac)
        {
        

        }

        public async Task<IEnumerable<string>> GetData(string sql, params object[] parameters)
        {
            //using (var conn = new NpgsqlConnection("Host=dezine.fortiddns.com:5433;Username=website;Password=32@D4$wo5asfd2;Database=infotrac"))
            //{
            //    await  conn.OpenAsync();

            //    using (var cmd = new NpgsqlCommand("select 'PPK' as \"Type\", round((288 / ppk_ppc)::decimal, 2) as \"Qty\", ppk_ppc as \"Pcs / Cart\", ppk_masterctnid as \"Carton\",\r\n\r\nppk_l As \"L\", ppk_w As \"W\", ppk_h As \"H\", ppk_cartonwht As \"Weight\", ppk_layersperskid as \"Crtns /Skid Layer\"\r\n\r\nfrom oeinvtitems\r\n\r\nwhere id = '01C3421WH'", conn))

            //        using (var reader = await cmd.ExecuteReaderAsync())

            //            while (await reader.ReadAsync())
            //                reader.GetString(0);
            //}
            return null;
        }
    }
}
