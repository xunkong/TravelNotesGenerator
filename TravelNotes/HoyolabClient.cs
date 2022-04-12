namespace TravelNotesGenerator.TravelNotes
{
    public class HoyolabClient
    {



        private readonly HttpClient _httpClient;

        private readonly JsonSerializerOptions _options;


        #region Constant

        // http header key
        private const string Accept = "Accept";
        private const string Cookie = "Cookie";
        private const string UserAgent = "User-Agent";
        private const string X_Reuqest_With = "X-Requested-With";
        private const string DS = "DS";
        private const string Referer = "Referer";
        private const string Application_Json = "application/json";
        private const string com_mihoyo_hyperion = "com.mihoyo.hyperion";
        private const string x_rpc_app_version = "x-rpc-app_version";
        private const string x_rpc_device_id = "x-rpc-device_id";
        private const string x_rpc_client_type = "x-rpc-client_type";

        // http header value
        private const string UA2101 = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) miHoYoBBS/2.10.1";
        private const string UA2111 = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) miHoYoBBS/2.11.1";
        private const string UA2161 = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) miHoYoBBS/2.16.1";
        private const string BBS_Referer = "https://bbs.mihoyo.com/";
        private const string Record_Referer = "https://webstatic.mihoyo.com/app/community-game-records/index.html?v=6";
        private const string TravelRecord_Referer = "https://webstatic.mihoyo.com/bbs/event/e20200709ysjournal/index.html?bbs_presentation_style=fullscreen&bbs_auth_required=true&utm_source=bbs&utm_medium=mys&utm_campaign=icon";
        private const string TravelRecord_Query = "bbs_presentation_style=fullscreen&bbs_auth_required=true&utm_source=bbs&utm_medium=mys&utm_campaign=icon";
        private const string SignIn_Referer = $"https://webstatic.mihoyo.com/bbs/event/signin-ys/index.html?bbs_auth_required=true&act_id={SignIn_ActivityId}&utm_source=bbs&utm_medium=mys&utm_campaign=icon";
        private static readonly string DeviceId = Guid.NewGuid().ToString("D");

        // base url
        private const string SignIn_ActivityId = "e202009291139501";
        private const string ApiTakumi = "https://api-takumi.mihoyo.com";
        private const string BbsApi = "https://bbs-api.mihoyo.com/user/wapi";
        private const string ApiTakumiRecord = "https://api-takumi-record.mihoyo.com/game_record/app/genshin/api";
        private const string Hk4eApi = "https://hk4e-api.mihoyo.com/event/ys_ledger";


        #endregion


        #region Api url


        /// <summary>
        /// 游戏角色
        /// </summary>
        private static readonly string UserGameRoleUrl = $"{ApiTakumi}/binding/api/getUserGameRolesByCookie?game_biz=hk4e_cn";


        /// <summary>
        /// 旅行者札记详细
        /// </summary>
        /// <param name="type">1原石，2摩拉</param>
        /// <param name="month">月份</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页限制几项，最多100</param>
        private static string TravelRecordDetailUrl(UserGameRoleInfo role, int month, TravelRecordAwardType type, int page, int limit = 10) => $"{Hk4eApi}/monthDetail?type={(int)type}&month={month}&page={page}&limit={limit}&bind_uid={role.Uid}&bind_region={role.Region}&{TravelRecord_Query}";

        #endregion




        public HoyolabClient(HttpClient? httpClient = null, JsonSerializerOptions? options = null)
        {
            if (httpClient is null)
            {
                _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.All });
            }
            else
            {
                _httpClient = httpClient;
            }
        }




        private async Task<T> CommonSendAsync<T>(HttpRequestMessage request) where T : class
        {
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize(content, typeof(HoyolabBaseWrapper<T>), HoyolabJsonContext.Default) as HoyolabBaseWrapper<T>;
            if (responseData is null)
            {
                throw new HoyolabException(-1, "Can not parse the response body.");
            }
            if (responseData.ReturnCode != 0)
            {
                throw new HoyolabException(responseData.ReturnCode, responseData.Message);
            }
            // warning 不确定是否应该判断响应data为null的情况
            if (responseData.Data is null)
            {
                throw new HoyolabException(-1, "Response data is null.");
            }
            return responseData.Data;
        }





        /// <summary>
        /// 玩家信息（服务器，uid，昵称，等级）
        /// </summary>
        /// <returns></returns>
        public async Task<List<UserGameRoleInfo>> GetUserGameRoleInfosAsync(string cookie)
        {
            if (string.IsNullOrWhiteSpace(cookie))
            {
                throw new ArgumentNullException(nameof(cookie));
            }
            var request = new HttpRequestMessage(HttpMethod.Get, UserGameRoleUrl);
            request.Headers.Add(Accept, Application_Json);
            request.Headers.Add(UserAgent, UA2101);
            request.Headers.Add(X_Reuqest_With, com_mihoyo_hyperion);
            request.Headers.Add(Cookie, cookie);
            var data = await CommonSendAsync<UserGameRoleWrapper>(request);
            data.List?.ForEach(x => x.Cookie = cookie);
            return data.List ?? new List<UserGameRoleInfo>();
        }



        /// <summary>
        /// 旅行记录原石摩拉详情
        /// </summary>
        /// <param name="role"></param>
        /// <param name="month">月份</param>
        /// <param name="type">1原石，2摩拉</param>
        /// <param name="page">第几页</param>
        /// <param name="limit">每页几条，最多100</param>
        /// <returns></returns>
        private async Task<TravelRecordDetail> GetTravelRecordDetailByPageAsync(UserGameRoleInfo role, int month, TravelRecordAwardType type, int page, int limit = 100)
        {
            var url = TravelRecordDetailUrl(role, month, type, page, limit);
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add(Accept, Application_Json);
            request.Headers.Add(UserAgent, UA2101);
            request.Headers.Add(Cookie, role.Cookie);
            request.Headers.Add(Referer, TravelRecord_Referer);
            request.Headers.Add(X_Reuqest_With, com_mihoyo_hyperion);
            var data = await CommonSendAsync<TravelRecordDetail>(request);
            foreach (var item in data.List)
            {
                item.Type = type;
            }
            return data;
        }


        /// <summary>
        /// 旅行记录原石摩拉详情
        /// </summary>
        /// <param name="role"></param>
        /// <param name="month">月份</param>
        /// <param name="type">1原石，2摩拉</param>
        /// <param name="limit">每页几条，最多100</param>
        /// <returns></returns>
        public async Task<TravelRecordDetail> GetTravelRecordDetailAsync(UserGameRoleInfo role, int month, TravelRecordAwardType type, int limit = 100)
        {
            var data = await GetTravelRecordDetailByPageAsync(role, month, type, 1, limit);
            if (data.List.Count < limit)
            {
                return data;
            }
            for (int i = 2; ; i++)
            {
                var addData = await GetTravelRecordDetailByPageAsync(role, month, type, i, limit);
                data.List.AddRange(addData.List);
                if (addData.List.Count < limit)
                {
                    break;
                }
            }
            return data;
        }





    }
}
