using TravelNotesGenerator;
using Xunkong.Hoyolab;
using Xunkong.Hoyolab.TravelNotes;
using Xunkong.Hoyolab.Wishlog;

var info =
"""
寻空 旅行札记生成器 v0.2.0  by Scighost
模仿米游社的旅行者札记生成可用于分享的图片

寻空项目网站：https://xunkong.cc
本项目开源地址：https://github.com/Xunkong/TravelNotesGenerator

""";
WriteLine(info);
WriteLine("正在获取资源信息。。。");
var generator = new TNGenerator();
await generator.GetAssetsAsync();
WriteLine("资源信息获取完毕");
WriteLine();
WriteLine("请输入米游社 Cookie，回车确认：");
WriteLine("如何获取米游社 Cookie？\n参考帮助文档：https://xunkong.cc/help/desktop/account.html");
string? cookie = null;
while (true)
{
    cookie = ReadLine();
    if (cookie.Contains("cookie_token") && cookie.Contains("account_id"))
    {
        break;
    }
    WriteLine("Cookie 中不包含 cookie_token 或 account_id，请重新输入：");
}
var hoyolabClient = new HoyolabClient();
WriteLine();
WriteLine("正在获取玩家信息。。。");
var users = await hoyolabClient.GetGenshinRoleInfosAsync(cookie);
WriteLine($"此账号下绑定了 {users.Count} 个玩家账号");
foreach (var user in users)
{
    WriteLine();
    WriteLine($"正在获取玩家 {user.Nickname} 的旅行札记信息。。。");
    var items = new List<TravelNotesAwardItem>();
    var now = DateTime.UtcNow.AddHours(4);
    WriteLine($"正在获取 {now.Month} 月的记录");
    var detail = await hoyolabClient.GetTravelNotesDetailAsync(user, now.Month, TravelNotesAwardType.Primogems);
    items.AddRange(detail.List);
    detail = await hoyolabClient.GetTravelNotesDetailAsync(user, now.Month, TravelNotesAwardType.Mora);
    items.AddRange(detail.List);
    now = now.AddMonths(-1);
    WriteLine($"正在获取 {now.Month} 月的记录");
    detail = await hoyolabClient.GetTravelNotesDetailAsync(user, now.Month, TravelNotesAwardType.Primogems);
    items.AddRange(detail.List);
    detail = await hoyolabClient.GetTravelNotesDetailAsync(user, now.Month, TravelNotesAwardType.Mora);
    items.AddRange(detail.List);
    now = now.AddMonths(-1);
    WriteLine($"正在获取 {now.Month} 月的记录");
    detail = await hoyolabClient.GetTravelNotesDetailAsync(user, now.Month, TravelNotesAwardType.Primogems);
    items.AddRange(detail.List);
    detail = await hoyolabClient.GetTravelNotesDetailAsync(user, now.Month, TravelNotesAwardType.Mora);
    items.AddRange(detail.List);
    generator.SetData(user, items);
    WishEventInfo.RegionType = user.Region;
    WriteLine($"正在生成图片。。。");
    await generator.GenerateImageAsync();
}
WriteLine();
WriteLine("已完成，图片保存在 output 文件夹中");
WriteLine("按下任意键结束程序");
ReadKey();
