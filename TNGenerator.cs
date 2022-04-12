#pragma warning disable CA1416 // 验证平台兼容性
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Json;
using TravelNotesGenerator.TravelNotes;
using Xunkong.Core.Wish;
using Xunkong.Core.XunkongApi;

namespace TravelNotesGenerator
{
    internal class TNGenerator
    {


        private readonly HttpClient _httpClient;


        Font font_version = new Font("微软雅黑", 28);
        Font font_nickname = new Font("微软雅黑", 16);
        Font font_stats = new Font("微软雅黑", 14);
        Font font_max = new Font("微软雅黑", 12);
        SolidBrush brush_blue = new SolidBrush(Color.FromArgb(0x28, 0x38, 0x4d));
        SolidBrush brush_gold = new SolidBrush(Color.FromArgb(0xa4, 0x8e, 0x73));
        Pen pen_tick = new Pen(Color.FromArgb(0x28, 0x38, 0x4d), 1.5f);

        Pen pen_huodong = new Pen(Color.FromArgb(95, 131, 165), 24f);
        Pen pen_meiri = new Pen(Color.FromArgb(189, 157, 96), 24f);
        Pen pen_shenyuan = new Pen(Color.FromArgb(120, 156, 118), 24f);
        Pen pen_youjian = new Pen(Color.FromArgb(128, 115, 169), 24f);
        Pen pen_maoxian = new Pen(Color.FromArgb(211, 108, 110), 24f);
        Pen pen_renwu = new Pen(Color.FromArgb(109, 179, 179), 24f);
        Pen pen_qita = new Pen(Color.FromArgb(113, 169, 200), 24f);

        Dictionary<string, Pen> pendic = new Dictionary<string, Pen>();


        public TNGenerator()
        {
            _httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = System.Net.DecompressionMethods.All });
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Xunkong.TravelNotesGenerator");

            pendic.Add("活动奖励", pen_huodong);
            pendic.Add("每日奖励", pen_meiri);
            pendic.Add("深境螺旋", pen_shenyuan);
            pendic.Add("邮件奖励", pen_youjian);
            pendic.Add("冒险奖励", pen_maoxian);
            pendic.Add("任务奖励", pen_renwu);
            pendic.Add("其他", pen_qita);
        }


        private TravelNoteAsset? asset;

        private string bg;

        private List<string> emos;


        private List<WishEventInfo> wishEvents;

        private List<TravelRecordAwardItem> awardItems;

        private UserGameRoleInfo user;



        public async Task GetAssetsAsync()
        {
            asset = await _httpClient.GetFromJsonAsync("https://file.xunkong.cc/static/travelnotes/asset.json", AssetJsonContext.Default.TravelNoteAsset);
            if (asset is null)
            {
                throw new NullReferenceException("Asset response is null.");
            }
            bg = asset.Background;
            emos = asset.Emotions;
            const string url = "https://api.xunkong.cc/v0.1/genshindata/wishevent";
            var response = await _httpClient.GetFromJsonAsync(url, XunkongJsonContext.Default.ResponseBaseWrapper);
            wishEvents = response.Data.List;
        }


        private async Task<Image> GetImageAsync()
        {
            var file = Path.Combine(AppContext.BaseDirectory, $@"cache\{Path.GetFileName(bg)}");
            if (!File.Exists(file))
            {
                var bytes = await _httpClient.GetByteArrayAsync(bg);
                Directory.CreateDirectory(Path.GetDirectoryName(file)!);
                await File.WriteAllBytesAsync(file, bytes);
            }
            return Image.FromFile(file);
        }

        private async Task<Image> GetEmoAsync()
        {
            var url = emos[Random.Shared.Next(emos.Count)];
            var file = Path.Combine(AppContext.BaseDirectory, $@"cache\{Path.GetFileName(url)}");
            if (!File.Exists(file))
            {
                var bytes = await _httpClient.GetByteArrayAsync(url);
                Directory.CreateDirectory(Path.GetDirectoryName(file)!);
                await File.WriteAllBytesAsync(file, bytes);
            }
            return Image.FromFile(file);
        }



        public void SetData(UserGameRoleInfo user, List<TravelRecordAwardItem> awardItems)
        {
            this.user = user;
            this.awardItems = awardItems;
        }




        public async Task GenerateImageAsync()
        {
            var versions = wishEvents.Select(x => x.Version).Distinct().ToList();
            foreach (var version in versions)
            {
                await DrawImageAsync(version);
            }
        }





        private async Task DrawImageAsync(string version)
        {
            var versionevent = wishEvents.Where(x => x.Version == version).FirstOrDefault();
            if (versionevent is null)
            {
                throw new NullReferenceException("Selected version is not found.");
            }
            var startTime = versionevent.StartTime.UtcDateTime.AddHours(8); ;
            var endtime = versionevent.StartTime.AddDays(42).UtcDateTime.AddHours(8);
            var uid = user.Uid;
            var rawlist_primo = awardItems.Where(x => x.Uid == uid & x.Type == TravelRecordAwardType.Primogems && x.Time >= startTime && x.Time <= endtime).ToList();
            var rawlist_mora = awardItems.Where(x => x.Uid == uid & x.Type == TravelRecordAwardType.Mora && x.Time >= startTime && x.Time <= endtime).ToList();

            if (!rawlist_primo.Any() && !rawlist_mora.Any())
            {
                return;
            }

            var image_bg = await GetImageAsync();
            using var g = Graphics.FromImage(image_bg);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.DrawString($"v{version}", font_version, brush_blue, 316, 394);
            g.DrawString(s: $"{startTime:yyyy/MM/dd}  -  {endtime:yyyy/MM/dd}", font_nickname, brush_blue, 160, 458);
            g.DrawString($"昵称：{user.Nickname}", font_nickname, brush_blue, 160, 494);
            g.DrawString(rawlist_primo.Sum(x => x.Number).ToString(), font_nickname, brush_gold, 310, 594);
            g.DrawString(rawlist_mora.Sum(x => x.Number).ToString(), font_nickname, brush_gold, 310, 668);

            var image_emo = await GetEmoAsync();
            g.RotateTransform(5);
            g.DrawImage(image_emo, 504, 284, 237, 375);
            g.RotateTransform(-5);


            var groupList = new List<string> { "每日奖励", "活动奖励", "深境螺旋", "邮件奖励", "任务奖励", "冒险奖励", "其他" };
            var stats = groupList.GroupJoin(rawlist_primo, x => x, x => ActionNameToGroup(x.ActionName), (x, y) => new Stats(x, y.Sum(y => y.Number))).ToList();
            static string ActionNameToGroup(string? name)
            {
                return name switch
                {
                    "每日委托奖励" => "每日奖励",
                    "活动奖励" => "活动奖励",
                    "深境螺旋奖励" => "深境螺旋",
                    "邮件奖励" => "邮件奖励",
                    "任务奖励" => "任务奖励",
                    "成就奖励" or "宝箱奖励" or "传送点解锁奖励" => "冒险奖励",
                    "教学阅读奖励" or "其他" or _ => "其他"
                };
            }
            var sum = stats.Sum(x => x.Number);

            foreach (var item in stats)
            {
                item.Percent = (float)item.Number / sum;
            }

            var stats_qita = stats.Where(x => x.Name == "其他").FirstOrDefault();
            stats = stats.Where(x => x.Name != "其他").OrderByDescending(x => x.Number).ToList();
            if (stats_qita is null)
            {
                stats_qita = new Stats("其他", 0);
            }
            stats.Add(stats_qita);



            var angle = -90f;
            foreach (var item in stats)
            {
                var swipetAngle = 360 * item.Percent;
                g.DrawArc(pendic[item.Name], 192, 816, 200, 200, angle - 1, swipetAngle + 1);
                angle += swipetAngle;
            }


            var index = 0;
            foreach (var item in stats)
            {
                g.FillRectangle(pendic[item.Name].Brush, 454, 800 + 36 * index, 14, 14);
                g.DrawString(item.Name, font_stats, brush_blue, 480, 794 + 36 * index);
                g.DrawString(item.Percent.ToString("P0"), font_stats, brush_blue, 572, 794 + 36 * index);
                index++;
            }


            var days = Enumerable.Range(0, 43).Select(x => startTime.AddDays(x)).ToList();
            var groupbyday = days.GroupJoin(rawlist_primo, x => x.Date, x => x.Time.Date, (x, y) => y.Sum(z => z.Number)).ToList();
            if (groupbyday.Last() == 0)
            {
                groupbyday.RemoveAt(42);
            }
            var max = groupbyday.Max();
            index = 0;
            bool hasPointMax = false;
            foreach (var item in groupbyday)
            {
                var height = (float)item / max * 160;
                g.FillRectangle(pen_huodong.Brush, 165 + 10.5f * index, 1270 - height, 10, height);
                if ((index + 3) % 7 == 0)
                {
                    g.DrawLine(pen_tick, 165 + 10.5f * index + 10.25f, 1270, 165 + 10.5f * index + 10.25f, 1275);
                }
                if (item == max && !hasPointMax)
                {
                    var delta = (int)Math.Log10(max) * 5.5f;
                    g.DrawString(max.ToString(), font_max, brush_blue, 165 + 10.5f * index - delta, 1270 - height - 22);
                    hasPointMax = true;
                }
                index++;
            }
            g.DrawLine(pen_tick, 165, 1270, 165 + 10.5f * groupbyday.Count - 0.5f, 1270);

            g.Save();
            var savePath = Path.Combine(AppContext.BaseDirectory, $@"output\TravelNotes_{uid}_v{version}_{DateTime.Now:yyMMddHHmmss}.png");
            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
            image_bg.Save(savePath, ImageFormat.Png);


        }



    }
}
