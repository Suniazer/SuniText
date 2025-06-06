using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SuniText.SuniLib.Text
{
    public static class Processor
    {
        /// <summary> 去掉每一行开头与末尾的空白字符 </summary>
        public static string TrimPro(string text)
        {
            string[] t = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = t[i].Trim();
            }
            return string.Join("\r\n", t);
        }//保留换行
        public static string TrimPlus(string text)
        {
            return Regex.Replace(text, @"\s+", string.Empty);
        }//全部删除
        public static string ToPath(string folder, string ext)
        {
            return string.Format($@"{folder}\{DateTime.Now}.{ext}");
        }
        public static string ToUnicode(string text)
        {
            char[] strs = text.ToCharArray();
            byte[] buffer;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strs.Length; i++)
            {
                buffer = Encoding.Unicode.GetBytes(strs[i].ToString());
                _ = sb.Append(string.Format("\\u{0:X2}{1:X2}", buffer[1], buffer[0]));
            }
            return sb.ToString();
        }
        public static string FromUnicode(string text)
        {
            StringBuilder sb = new StringBuilder();
            int len = text.Length / 6;
            for (int i = 0; i < len; i++)
            {
                string str = text.Substring(i + 2, 4);
                byte[] b = new byte[2];
                b[0] = byte.Parse(int.Parse(str.Substring(0, 2), NumberStyles.HexNumber).ToString());
                b[1] = byte.Parse(int.Parse(str.Substring(2, 2), NumberStyles.HexNumber).ToString());
                _ = sb.Append(Encoding.Unicode.GetChars(b));
            }
            return sb.ToString();
        }
        public static string ToMD5(string text)
        {
            MD5 md5 = MD5.Create();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(text));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                _ = sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }
        /// <summary>  mode==true => UTF8 </summary>
        public static string ToBase64(string text, bool mode)
        {
            Encoding enc = mode ? Encoding.UTF8 : Encoding.Unicode;
            return Convert.ToBase64String(enc.GetBytes(text));
        }
        public static string FromBase64(string base64, bool mode)
        {
            Encoding enc = mode ? Encoding.UTF8 : Encoding.Unicode;
            return enc.GetString(Convert.FromBase64String(base64));
        }
        public static string TimeToMD5()
        {
            return ToMD5(DateTime.Now.ToString());
        }
        /// <summary> 反转字符串 </summary>
        public static string Reverse(string text)
        {
            return new string(text.ToCharArray().Reverse().ToArray());
        }
        public static class ChnToPh
        {
            private static readonly int[] getValue = new int[] {
        -20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036,
        -20032,-20026,-20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763,
        -19756,-19751,-19746,-19741,-19739,-19728,-19725,-19715,-19540,-19531,-19525,-19515,
        -19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,-19261,-19249,
        -19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006,
        -19003,-18996,-18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735,
        -18731,-18722,-18710,-18697,-18696,-18526,-18518,-18501,-18490,-18478,-18463,-18448,
        -18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183, -18181,-18012,
        -17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752,
        -17733,-17730,-17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482,
        -17468,-17454,-17433,-17427,-17417,-17202,-17185,-16983,-16970,-16942,-16915,-16733,
        -16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,-16452,-16448,
        -16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216,
        -16212,-16205,-16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944,
        -15933,-15920,-15915,-15903,-15889,-15878,-15707,-15701,-15681,-15667,-15661,-15659,
        -15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,-15408,-15394,
        -15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150,
        -15149,-15144,-15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109,
        -14941,-14937,-14933,-14930,-14929,-14928,-14926,-14922,-14921,-14914,-14908,-14902,
        -14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,-14663,-14654,
        -14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345,
        -14170,-14159,-14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112,
        -14109,-14099,-14097,-14094,-14092,-14090,-14087,-14083,-13917,-13914,-13910,-13907,
        -13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,-13611,-13601,
        -13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343,
        -13340,-13329,-13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076,
        -13068,-13063,-13060,-12888,-12875,-12871,-12860,-12858,-12852,-12849,-12838,-12831,
        -12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,-12320,-12300,
        -12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798,
        -11781,-11604,-11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067,
        -11055,-11052,-11045,-11041,-11038,-11024,-11020,-11019,-11018,-11014,-10838,-10832,
        -10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,-10329,-10328,
        -10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254
            };
            private static readonly string[] getName = new string[] {
        "A","Ai","An","Ang","Ao","Ba","Bai","Ban","Bang","Bao","Bei","Ben",
        "Beng","Bi","Bian","Biao","Bie","Bin","Bing","Bo","Bu","Ba","Cai","Can",
        "Cang","Cao","Ce","Ceng","Cha","Chai","Chan","Chang","Chao","Che","Chen","Cheng",
        "Chi","Chong","Chou","Chu","Chuai","Chuan","Chuang","Chui","Chun","Chuo","Ci","Cong",
        "Cou","Cu","Cuan","Cui","Cun","Cuo","Da","Dai","Dan","Dang","Dao","De",
        "Deng","Di","Dian","Diao","Die","Ding","Diu","Dong","Dou","Du","Duan","Dui",
        "Dun","Duo","E","En","Er","Fa","Fan","Fang","Fei","Fen","Feng","Fo",
        "Fou","Fu","Ga","Gai","Gan","Gang","Gao","Ge","Gei","Gen","Geng","Gong",
        "Gou","Gu","Gua","Guai","Guan","Guang","Gui","Gun","Guo","Ha","Hai","Han",
        "Hang","Hao","He","Hei","Hen","Heng","Hong","Hou","Hu","Hua","Huai","Huan",
        "Huang","Hui","Hun","Huo","Ji","Jia","Jian","Jiang","Jiao","Jie","Jin","Jing",
        "Jiong","Jiu","Ju","Juan","Jue","Jun","Ka","Kai","Kan","Kang","Kao","Ke",
        "Ken","Keng","Kong","Kou","Ku","Kua","Kuai","Kuan","Kuang","Kui","Kun","Kuo",
        "La","Lai","Lan","Lang","Lao","Le","Lei","Leng","Li","Lia","Lian","Liang",
        "Liao","Lie","Lin","Ling","Liu","Long","Lou","Lu","Lv","Luan","Lue","Lun",
        "Luo","Ma","Mai","Man","Mang","Mao","Me","Mei","Men","Meng","Mi","Mian",
        "Miao","Mie","Min","Ming","Miu","Mo","Mou","Mu","Na","Nai","Nan","Nang",
        "Nao","Ne","Nei","Nen","Neng","Ni","Nian","Niang","Niao","Nie","Nin","Ning",
        "Niu","Nong","Nu","Nv","Nuan","Nue","Nuo","O","Ou","Pa","Pai","Pan",
        "Pang","Pao","Pei","Pen","Peng","Pi","Pian","Piao","Pie","Pin","Ping","Po",
        "Pu","Qi","Qia","Qian","Qiang","Qiao","Qie","Qin","Qing","Qiong","Qiu","Qu",
        "Quan","Que","Qun","Ran","Rang","Rao","Re","Ren","Reng","Ri","Rong","Rou",
        "Ru","Ruan","Rui","Run","Ruo","Sa","Sai","San","Sang","Sao","Se","Sen",
        "Seng","Sha","Shai","Shan","Shang","Shao","She","Shen","Sheng","Shi","Shou","Shu",
        "Shua","Shuai","Shuan","Shuang","Shui","Shun","Shuo","Si","Song","Sou","Su","Suan",
        "Sui","Sun","Suo","Ta","Tai","Tan","Tang","Tao","Te","Teng","Ti","Tian",
        "Tiao","Tie","Ting","Tong","Tou","Tu","Tuan","Tui","Tun","Tuo","Wa","Wai",
        "Wan","Wang","Wei","Wen","Weng","Wo","Wu","Xi","Xia","Xian","Xiang","Xiao",
        "Xie","Xin","Xing","Xiong","Xiu","Xu","Xuan","Xue","Xun","Ya","Yan","Yang",
        "Yao","Ye","Yi","Yin","Ying","Yo","Yong","You","Yu","Yuan","Yue","Yun",
        "Za", "Zai","Zan","Zang","Zao","Ze","Zei","Zen","Zeng","Zha","Zhai","Zhan",
        "Zhang","Zhao","Zhe","Zhen","Zheng","Zhi","Zhong","Zhou","Zhu","Zhua","Zhuai","Zhuan",
        "Zhuang","Zhui","Zhun","Zhuo","Zi","Zong","Zou","Zu","Zuan","Zui","Zun","Zuo"
            };
            //建立一个convertCh方法用于将汉字转换成全拼的拼音，其中，参数代表汉字字符串，此方法的返回值是转换后的拼音字符串
            public static string ConvertCh(string Chstr)
            {
                Regex reg = new Regex("^[\u4e00-\u9fa5]$");//验证是否输入汉字
                string pystr = string.Empty;
                char[] mChar = Chstr.ToCharArray();//获取汉字对应的字符数组
                for (int j = 0; j < mChar.Length; j++)
                {
                    //如果输入的是汉字
                    if (reg.IsMatch(mChar[j].ToString()))
                    {
                        byte[] arr = Encoding.Default.GetBytes(mChar[j].ToString());
                        int asc = (arr[0] * 256) + arr[1] - 65536;
                        if (asc > 0 && asc < 160)
                        {
                            pystr += mChar[j];
                        }
                        else
                        {
                            switch (asc)
                            {
                                case -9254:
                                    pystr += "Zhen"; break;
                                case -8985:
                                    pystr += "Qian"; break;
                                case -5463:
                                    pystr += "Jia"; break;
                                case -8274:
                                    pystr += "Ge"; break;
                                case -5448:
                                    pystr += "Ga"; break;
                                case -5447:
                                    pystr += "La"; break;
                                case -4649:
                                    pystr += "Chen"; break;
                                case -5436:
                                    pystr += "Mao"; break;
                                case -5213:
                                    pystr += "Mao"; break;
                                case -3597:
                                    pystr += "Die"; break;
                                case -5659:
                                    pystr += "Tian"; break;
                                default:
                                    for (int i = getValue.Length - 1; i >= 0; i--)
                                    {
                                        if (getValue[i] <= asc)//判断汉字的拼音区编码是否在指定范围内
                                        {
                                            pystr += getName[i];//如果不超出范围则获取对应的拼音
                                            break;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    else//如果不是汉字
                    {
                        pystr += mChar[j].ToString();//如果不是汉字则返回
                    }
                }
                return pystr;//返回获取到的汉字拼音
            }
        }
        public static string ToExtInfo(string ext)
        {
            string info = string.Empty;
            ext = ext.ToUpper();
            string extf = ext.Substring(0, 1);
            switch (extf)
            {
                case "1":
                    break;
                case "2":
                    break;
                case "3":
                    switch (ext)
                    {
                        case "3GP":
                            info = "第三代合作伙伴项目计划,多媒体容器格式";
                            break;
                    }
                    break;
                case "4":
                    break;
                case "5":
                    break;
                case "6":
                    break;
                case "7":
                    switch (ext)
                    {
                        case "7Z":
                            info = "7-zip压缩文件";
                            break;
                    }
                    break;
                case "8":
                    break;
                case "9":
                    break;
                case "0":
                    break;
                case "A":
                    switch (ext)
                    {
                        case "A":
                            info = "对象代码库文件";
                            break;
                        case "AAC":
                            info = "高级音频编码(Advanced Audio Codin)";
                            break;
                        case "AAM":
                            info = "Authorware shocked文件";
                            break;
                        case "AAS":
                            info = "Authorware shocked包";
                            break;
                        case "ABF":
                            info = "Adobe二进制屏幕字体";
                            break;
                        case "ABK":
                            info = "CorelDRAW自动备份文件";
                            break;
                        case "ABS":
                            info = "该类文件有时用于指示一个摘要（就像在一篇有关科学方面的文章的一个摘要或概要，取自abstract）";
                            break;
                        case "ACCOUNTPICTURE - MS":
                            info = "用户头像";
                            break;
                        case "ACE":
                            info = "Ace压缩文件格式";
                            break;
                        case "AM":
                            info = "Windows系统目录文件";
                            break;
                        case "ACP":
                            info = "Microsoft office助手预览文件";
                            break;
                        case "ACT":
                            info = "Microsoft office助手文件";
                            break;
                        case "AD":
                            info = "After Dark屏幕保护程序";
                            break;
                        case "ADA":
                            info = "Ada源文件（非 - GNAT）";
                            break;
                        case "ADB":
                            info = "Ada源文件主体（GNAT）；HP100LX组织者的约定数据库";
                            break;
                        case "ADF":
                            info = "Amiga磁盘文件";
                            break;
                        case "ADI":
                            info = "AutoCAD设备无关二进制绘图仪格式";
                            break;
                        case "ADM":
                            info = "After Dark多模块屏幕保护；Windows NT策略模板";
                            break;
                        case "ADP":
                            info = "FaxWork用于传真调制解调器的交互安装文件；Astound Dynamite文件";
                            break;
                        case "ADS":
                            info = "After Dark随机屏幕保护；Smart Address的地址簿";
                            break;
                        case "AFM":
                            info = "Ada源文件说明书（GNAT）";
                            break;
                        case "AF2 / AF3":
                            info = "Adobe的字体尺度";
                            break;
                        case "AI":
                            info = "ABC的FlowChat文件";
                            break;
                        case "AIAIF / AIFF":
                            info = "Adobe Illustrator格式图形";
                            break;
                        case "AIFC":
                            info = "音频互交换文件，Silicon Graphic and Macintosh应用程序的声音格式";
                            break;
                        case "AIM":
                            info = "压缩AIF";
                            break;
                        case "AIS":
                            info = "AOL即时信息传送";
                            break;
                        case "AKW":
                            info = "ACDSee图形序列文件；Velvet Studio设备文件";
                            break;
                        case "ALB":
                            info = "RoboHELP的帮助工程中所有A - 关键词";
                            break;
                        case "ALL":
                            info = "JASC Image Commander相册";
                            break;
                        case "AMS":
                            info = "艺术与书信库";
                            break;
                        case "ANC":
                            info = "Velvet Studio音乐模块（MOD）文件；Extreme的Tracker模块文件";
                            break;
                        case "ANI":
                            info = "Canon Computer的调色板文件，包含一系列可选的颜色板";
                            break;
                        case "ANS":
                            info = "Windows系统中的动画光标";
                            break;
                        case "ANT":
                            info = "ANSI文本文件</>SimAnt For Windows中保存的游戏文件";
                            break;
                        case "API":
                            info = "Adobe Acrobat使用的应用程序设计接口文件";
                            break;
                        case "APK":
                            info = "安卓安装包";
                            break;
                        case "APP":
                            info = "用于创建Mac OS X的应用程序文件集成数据压缩标准和程序逻辑编译规格可执行文件";
                            break;
                        case "APS":
                            info = "Microsoft Visual C++文件";
                            break;
                        case "ARI":
                            info = "Aristotle声音文件";
                            break;
                        case "ARJ":
                            info = "Robert Jung ARJ压缩包文件";
                            break;
                        case "ART":
                            info = "Xara Studio绘画文件；Canon Crayola美术文件；Clip Art文件格式；另一种光线跟踪格式；AOL使用的用Johnson-Grace压缩算法压缩的标记文件";
                            break;
                        case "ASA":
                            info = "Microsoft Visual InterDev文件";
                            break;
                        case "ASC":
                            info = "ASCⅡ文本文件；PGP算法加密文件";
                            break;
                        case "ASD":
                            info = "Microsoft Word的自动保存文件；Microsoft高级流媒体格式（microsoft advanced streaming format，ASF）的描述文件；可用NSREX打开 Velvet Studio例子文件";
                            break;
                        case "ASE":
                            info = "Velvet Studio采样文件";
                            break;
                        case "ASF":
                            info = "Microsoft高级流媒体格式文件";
                            break;
                        case "ASM":
                            info = "汇编语言源文件，Pro/E装配文件";
                            break;
                        case "ASO":
                            info = "Astound Dynamite对象文件";
                            break;
                        case "ASP":
                            info = "动态网页文件；ProComm Plus安装与连接脚本文件；Astound介绍文件";
                            break;
                        case "ASV":
                            info = "DataCAD自动保存文件";
                            break;
                        case "ASX":
                            info = "Cheyenne备份脚本文件；Microsoft高级流媒体重定向器文件，视频文件";
                            break;
                        case "ATW":
                            info = "来自个人软件的Any Time Deluxe For Windows个人信息管理员文件";
                            break;
                        case "AU":
                            info = "Sun/NeXT/DEC/UNIX声音文件；音频U-Law（读作\"mu - law\"）文件格式";
                            break;
                        case "AVB":
                            info = "Computer Associates Inoculan反病毒软件的病毒感染后文件";
                            break;
                        case "AVI":
                            info = "Microsoft Audio Video Interleave电影格式";
                            break;
                        case "AVR":
                            info = "Audio Visual Research文件格式";
                            break;
                        case "AVS":
                            info = "应用程序可视化格式";
                            break;
                        case "AWD":
                            info = "FaxVien文档";
                            break;
                        case "AWR":
                            info = "Telsis数字储存音频文件扩展名格式";
                            break;
                        case "Axx":
                            info = "ARJ压缩文件的分包序号文件，用于将一个大文件压至几个小的压缩包中（xx取01-99的数字）";
                            break;
                        case "A3M":
                            info = "Authorware Macintosh未打包文件";
                            break;
                        case "A4M":
                            info = "Authorware Macintosh未打包文件";
                            break;
                        case "A4P":
                            info = "Authorware无运行时间的打包文件";
                            break;
                        case "A3W":
                            info = "未打包的Authorware Windows文件";
                            break;
                        case "A4W":
                            info = "未打包的Authorware Windows文件";
                            break;
                        case "A5W":
                            info = "未打包的Authorware Windows文件";
                            break;
                    }
                    break;
                case "B":
                    switch (ext)
                    {
                        case "BAK":
                            info = "备份文件";
                            break;
                        case "BAS":
                            info = "BASIC源文件";
                            break;
                        case "BAT":
                            info = "批处理文件";
                            break;
                        case "BDF":
                            info = "West Point Bridger Designer文件";
                            break;
                        case "BGL":
                            info = "Microsoft Flight Simulator（微软飞行模拟器）的视景文件";
                            break;
                        case "BI":
                            info = "二进制文件";
                            break;
                        case "BIF":
                            info = "Group Wise的初始化文件";
                            break;
                        case "BIFF":
                            info = "XLIFE 3D格式文件";
                            break;
                        case "BIN":
                            info = "二进制文件";
                            break;
                        case "BK":
                            info = "有时用于代表备份版本";
                            break;
                        case "BK$":
                            info = "有时用于代表备份版本";
                            break;
                        case "BKS":
                            info = "IBM BookManager Read书架文件";
                            break;
                        case "BMK":
                            info = "书签文件";
                            break;
                        case "BMP":
                            info = "Windows或OS/2位图文件";
                            break;
                        case "BOOK":
                            info = "Adobe FrameMaker Book文件";
                            break;
                        case "BRX":
                            info = "用于查看多媒体对象目录的文件";
                            break;
                        case "BSP":
                            info = "Quake图形文件";
                            break;
                        case "BTM":
                            info = "Norton 应用程序使用的批处理文件";
                            break;
                        case "BUD":
                            info = "Quicken的备份磁盘";
                            break;
                        case "BUN":
                            info = "CakeWalk 声音捆绑文件（一种MIDI程序）";
                            break;
                        case "BW":
                            info = "SGI黑白图像文件";
                            break;
                        case "BWV":
                            info = "商业波形文件";
                            break;
                    }
                    break;
                case "C":
                    switch (ext)
                    {
                        case "C":
                            info = "C代码文件";
                            break;
                        case "CAB":
                            info = "Microsoft压缩文件";
                            break;
                        case "CAD":
                            info = "Softdek的Drafix CAD文件";
                            break;
                        case "CAL":
                            info = "CALS压缩位图；日历计划表数据";
                            break;
                        case "CAP":
                            info = "压缩音乐文件格式";
                            break;
                        case "CAS":
                            info = "逗号分开的ASCⅡ文件";
                            break;
                        case "CB":
                            info = "Microsoft干净引导文件";
                            break;
                        case "CC":
                            info = "Visual dBASE用户自定义类文件";
                            break;
                        case "CCB":
                            info = "Visual Basic动态按钮配置文件";
                            break;
                        case "CCH":
                            info = "Corel图表文件";
                            break;
                        case "CCO":
                            info = "CyberChat数据文件";
                            break;
                        case "CCT":
                            info = "Macromedia Director Shockwave投影";
                            break;
                        case "CDA":
                            info = "CD音频轨道";
                            break;
                        case "CDF":
                            info = "Microsoft频道定义格式文件";
                            break;
                        case "CDI":
                            info = "Philip的高密盘交互格式";
                            break;
                        case "CDM":
                            info = "Visual dBASE自定义数据模块文件";
                            break;
                        case "CDR":
                            info = "CorelDRAW绘图文件；原始音频CD数据文件";
                            break;
                        case "CDT":
                            info = "CorelDRAW模板";
                            break;
                        case "CDX":
                            info = "CorelDRAW压缩绘图文件；Microsoft Visual FoxPro索引文件";
                            break;
                        case "CFG":
                            info = "配置文件";
                            break;
                        case "CGI":
                            info = "公共网关接口脚本文件";
                            break;
                        case "CGM":
                            info = "计算机图形元文件";
                            break;
                        case "CHK":
                            info = "由Windows磁盘碎片整理器或磁盘扫描保存的文件碎片";
                            break;
                        case "CHM":
                            info = "编译过的HTML文件";
                            break;
                        case "CHR":
                            info = "字符集（字体文件）";
                            break;
                        case "CIF":
                            info = "Adaptec CD 创建器 CD映像文件";
                            break;
                        case "CIL":
                            info = "Clip Gallery下载包";
                            break;
                        case "CLASS":
                            info = "Java类文件";
                            break;
                        case "CLL":
                            info = "Crick Software Clicker文件";
                            break;
                        case "CLP":
                            info = "Windows 剪贴板文件";
                            break;
                        case "CLS":
                            info = "Visual Basic类文件";
                            break;
                        case "CLSS":
                            info = "Java可执行文件";
                            break;
                        case "CMD":
                            info = "Windows批处理文件";
                            break;
                        case "CMF":
                            info = "Corel元文件";
                            break;
                        case "CMV":
                            info = "Corel Move动画文件";
                            break;
                        case "CMX":
                            info = "Corel Presentation Exchange图像";
                            break;
                        case "CNF":
                            info = "Telnet，Windows和其他其内格式会发生改变的应用程序使用的配置文件";
                            break;
                        case "CNM":
                            info = "Windows应用程序菜单选项和安装文件";
                            break;
                        case "CNT":
                            info = "Windows（或其他）系统用于帮助索引或其他目的内容文件";
                            break;
                        case "COD":
                            info = "Microsoft C编译器产生的可显示机器码/汇编代码文件，其中附有源C代码作为注释";
                            break;
                        case "COM":
                            info = "命令文件（程序）";
                            break;
                        case "CPL":
                            info = "控制面板扩展名，Corel颜色板";
                            break;
                        case "CPO":
                            info = "Corel打印存储文件";
                            break;
                        case "CPP":
                            info = "C++代码文件";
                            break;
                        case "CPR":
                            info = "Corel提供说明书文件";
                            break;
                        case "CPT":
                            info = "Corel 照片-绘画图像";
                            break;
                        case "CPX":
                            info = "Corel Presentation Exchange压缩图形文件";
                            break;
                        case "CRD":
                            info = "Windows Cardfile文件";
                            break;
                        case "CRP":
                            info = "Corel 提供的运行时介绍文件；Visual dBASE自定义报表文件";
                            break;
                        case "CRT":
                            info = "认证文件";
                            break;
                        case "CSC":
                            info = "Corel脚本文件";
                            break;
                        case "CSP":
                            info = "PC Emcee On_Screen图像";
                            break;
                        case "CST":
                            info = "Macromedia Director Cast文件";
                            break;
                        case "CSV":
                            info = "逗号分隔的值文件";
                            break;
                        case "CTL":
                            info = "通常用于表示一个包含控件信息的文件；FaxWork用它来保持有关每个传真收到或发出的信息";
                            break;
                        case "CUR":
                            info = "Windows光标文件";
                            break;
                        case "CV":
                            info = "Corel版本的档案文件；Microsoft CodeView信息屏幕文件";
                            break;
                        case "CXX":
                            info = "C++源代码文件";
                            break;
                    }
                    break;
                case "D":
                    switch (ext)
                    {
                        case "DMG":
                            info = "MAC磁盘镜像";
                            break;
                        case "DBC":
                            info = "数据文件；WrodPerfect合并数据文件；用于一些MPEG格式的文件";
                            break;
                        case "DBF":
                            info = "Borland的Paradox 7表";
                            break;
                        case "DBX":
                            info = "Microsoft Visual FoxPro数据库容器文件";
                            break;
                        case "DCM":
                            info = "dBASE文件";
                            break;
                        case "DCP":
                            info = "数字电影包";
                            break;
                        case "DCS":
                            info = "DataBearn图像；Microsoft Visual FoxPro表格文件";
                            break;
                        case "DCT":
                            info = "DCM模块格式文件";
                            break;
                        case "DCU":
                            info = "桌面颜色分隔文件";
                            break;
                        case "DEM":
                            info = "Delphi编译单元文件</>用于表示数字高度模型的USGS基准的文件";
                            break;
                        case "DCX":
                            info = "Microsoft Visual FoxPro数据库容器；基于PCX的传真图像；宏";
                            break;
                        case "DER":
                            info = "认证文件";
                            break;
                        case "DEWF":
                            info = "Macintosh Sound Cap/Sound Edit录音设备格式";
                            break;
                        case "DIB":
                            info = "设备无关位图";
                            break;
                        case "DIC":
                            info = "目录";
                            break;
                        case "DIF":
                            info = "可进行数据互换的电子表格";
                            break;
                        case "DIG":
                            info = "DigiLink格式；Sound DesignerⅠ音频文件";
                            break;
                        case "DIR":
                            info = "Macromedia Director文件";
                            break;
                        case "DIZ":
                            info = "描述文件";
                            break;
                        case "DLG":
                            info = "C++对话框脚本文件";
                            break;
                        case "DLL":
                            info = "应用程序扩展动态链接库";
                            break;
                        case "DLS":
                            info = "可下载声音文件";
                            break;
                        case "DMD":
                            info = "Visual dBASE数据模块文件";
                            break;
                        case "DMF":
                            info = "X-Trakker音乐模块（MOD）文件";
                            break;
                        case "DOC":
                            info = "Microsoft Office Word 97-2003 文档";
                            break;
                        case "DOCX":
                            info = "Microsoft Office Word 2007 文档";
                            break;
                        case "DOT":
                            info = "Microsoft Word文档模板";
                            break;
                        case "DPX":
                            info = "光栅图像格式";
                            break;
                        case "DRAW":
                            info = "Acorn的基于对象的矢量图像文件";
                            break;
                        case "DRV":
                            info = "驱动程序";
                            break;
                        case "DRW":
                            info = "Micrografx Designer/Draw；Pro/E绘画文件";
                            break;
                        case "DSG":
                            info = "DOOM保存的文件";
                            break;
                        case "DSM":
                            info = "Dynamic Studio音乐模块（MOD）文件";
                            break;
                        case "DSP":
                            info = "Microsoft Developer Studio工程文件";
                            break;
                        case "DSQ":
                            info = "Corel QUERY（查询）文件";
                            break;
                        case "DSW":
                            info = "Microsoft Developer Studio工作区文件";
                            break;
                        case "DTD":
                            info = "SGML文档类型定义（DTD）文件";
                            break;
                        case "DUN":
                            info = "Microsoft拔号网络导出文件";
                            break;
                        case "DV":
                            info = "数字视频文件（MIME）";
                            break;
                        case "DXF":
                            info = "可进行互交换的绘图文件格式，二进制的DWG格式的文本表示；数据交换文件";
                            break;
                        case "DXR":
                            info = "Macromedia Director受保护（不可编辑）电影文件";
                            break;
                        case "DWG":
                            info = "AutoCAD以及基于AutoCAD的软件保存设计数据所用的一种专有文件格式";
                            break;
                    }
                    break;
                case "E":
                    switch (ext)
                    {
                        case "EDA":
                            info = "Ensoniq ASR磁盘映像";
                            break;
                        case "EDD":
                            info = "元素定义文档（FrameMaker+SGML文档）";
                            break;
                        case "EMD":
                            info = "ABT扩展模块";
                            break;
                        case "EMF":
                            info = "Windows增强元文件";
                            break;
                        case "EML":
                            info = "Microsoft Outlook Express邮件消息（MIME RFC822）文件";
                            break;
                        case "EPHTML":
                            info = "Perl解释增强HTML文件";
                            break;
                        case "EPS":
                            info = "压缩的PostScript图像";
                            break;
                        case "EXE":
                            info = "可执行文件（程序）";
                            break;
                    }
                    break;
                case "F":
                    switch (ext)
                    {
                        case "FAV":
                            info = "Microsoft Outlook导航条";
                            break;
                        case "FAX":
                            info = "传真类型图像";
                            break;
                        case "FCD":
                            info = "虚拟CD-ROM";
                            break;
                        case "FDF":
                            info = "A dobe Acrobat表单文档文件";
                            break;
                        case "FFA":
                            info = "Microsoft快速查找文件";
                            break;
                        case "FFL":
                            info = "Microsoft快速查找文件";
                            break;
                        case "FFO":
                            info = "Microsoft快速查找文件";
                            break;
                        case "FFK":
                            info = "Microsoft快速查找文件";
                            break;
                        case "FIF":
                            info = "Fractal图像文件";
                            break;
                        case "FLA":
                            info = "Macromedia Flash电影";
                            break;
                        case "FLC":
                            info = "Autodesk FLIC动画文件";
                            break;
                        case "FLI":
                            info = "Autodesk FLIC动画";
                            break;
                        case "FLV":
                            info = "Adobe Systems开发的Flash视频编码规格和视频数据的压缩标准的分布式轻量级视频嵌入对Web页面和流媒体在互联网上的数字视频。";
                            break;
                        case "FM":
                            info = "Adobe FrameMaker文档";
                            break;
                        case "FML":
                            info = "文件镜象列表（GetRight）";
                            break;
                        case "FNG":
                            info = "字体组文件（字体导航器，Font Navigator）";
                            break;
                        case "FNK":
                            info = "Funk Tracker模块格式";
                            break;
                        case "FON":
                            info = "系统字体";
                            break;
                        case "FOT":
                            info = "字体相关文件";
                            break;
                        case "FRM":
                            info = "Visual Basic From File";
                            break;
                        case "FRT":
                            info = "Microsoft FoxPro报表文件";
                            break;
                        case "FRX":
                            info = "Visual Basic表单文本；Microsoft FoxPro报表文件";
                            break;
                        case "FTG":
                            info = "全文本搜索组文件，由Windows帮助系统查找时产生--可以删除，并在需要时重建起来";
                            break;
                        case "FTS":
                            info = "全文本搜索引文件，由Windows帮助系统查找时产生";
                            break;
                    }
                    break;
                case "G":
                    switch (ext)
                    {
                        case "GZ":
                            info = "GNU压缩归档文件";
                            break;
                        case "GAL":
                            info = "Corel多媒体管理器相集";
                            break;
                        case "GDB":
                            info = "InterBase数据库文件";
                            break;
                        case "GDM":
                            info = "铃声、口哨声和声音板模块格式";
                            break;
                        case "GEM":
                            info = "GEM元文件";
                            break;
                        case "GEN":
                            info = "Ventura产生的文本文件";
                            break;
                        case "GETRIGHT":
                            info = "GetRight未完成的下载文件";
                            break;
                        case "GFI":
                            info = "Genigraphics图形链接表示文件";
                            break;
                        case "GFX":
                            info = "Genigraphics图形链接表示文件";
                            break;
                        case "GHO":
                            info = "Norton 克隆磁盘映像";
                            break;
                        case "GIF":
                            info = "CompuServe位图文件";
                            break;
                        case "GIM":
                            info = "Genigraphics图形链接介绍文件";
                            break;
                        case "GKH":
                            info = "Ensoniq EPS家簇磁盘映像文件";
                            break;
                        case "GKS":
                            info = "Gravis Grip Key文档";
                            break;
                        case "GL":
                            info = "动画格式";
                            break;
                        case "GNA":
                            info = "Genigraphics图形链接介绍文件";
                            break;
                        case "GNT":
                            info = "生成代码，Micro Focus属性格式里的可执行代码";
                            break;
                        case "GNX":
                            info = "Genigraphics图形链接介绍文件";
                            break;
                        case "GRA":
                            info = "Microsoft Graph文件";
                            break;
                        case "GRF":
                            info = "Grapher（Golden Software公司）图形文件";
                            break;
                        case "GRP":
                            info = "程序管理组";
                            break;
                        case "GO":
                            info = "Go语言源文件";
                            break;
                    }
                    break;
                case "H":
                    switch (ext)
                    {
                        case "HCOM":
                            info = "声音工具HCOM格式";
                            break;
                        case "HGL":
                            info = "HP图形语言绘图文件";
                            break;
                        case "HLP":
                            info = "帮助文件；Date CAD Windows帮助文件";
                            break;
                        case "HPJ":
                            info = "Visual Basic帮助工程";
                            break;
                        case "HPP":
                            info = "C++程序头文件";
                            break;
                        case "HST":
                            info = "历史文件";
                            break;
                        case "HT":
                            info = "HyperTerminal（超级终端）";
                            break;
                        case "HTM":
                            info = "超文本文档";
                            break;
                        case "HTML":
                            info = "超文本文档";
                            break;
                        case "HTT":
                            info = "Microsoft超文本模板";
                            break;
                        case "HTX":
                            info = "扩展HTML模板";
                            break;
                        case "HTA":
                            info = "HTML应用程序";
                            break;
                    }
                    break;
                case "I":
                    switch (ext)
                    {
                        case "ICB":
                            info = "Targa位图文件";
                            break;
                        case "ICC":
                            info = "Kodak打印机格式文件";
                            break;
                        case "ICL":
                            info = "图标库文件";
                            break;
                        case "ICM":
                            info = "图形颜色匹配配置文件";
                            break;
                        case "ICO":
                            info = "Windows图标";
                            break;
                        case "IDD":
                            info = "MIDI设备定义";
                            break;
                        case "IDF":
                            info = "MIDI设备定义（Windows 95需要的文件）";
                            break;
                        case "IDQ":
                            info = "Internet数据查询文件";
                            break;
                        case "IDX":
                            info = "Microsoft FoxPro相关数据库索引文件；Symantec Q&A相关数据库索引文件；Microsoft Outlook Express文件";
                            break;
                        case "IFF":
                            info = "交换格式文件；Amiga ILBM";
                            break;
                        case "IGS":
                            info = "初始图形交换说明文件";
                            break;
                        case "IGF":
                            info = "插入系统元文件";
                            break;
                        case "ILBM":
                            info = "位图图形文件";
                            break;
                        case "IMA":
                            info = "WinImage磁盘映像文件";
                            break;
                        case "INF":
                            info = "信息文件";
                            break;
                        case "INI":
                            info = "初始化文件";
                            break;
                        case "INRS":
                            info = "INRS远程通信声频";
                            break;
                        case "INS":
                            info = "InstallShield安装脚本";
                            break;
                        case "INT":
                            info = "中间代码，当一个源程序经过语法检查后编译产生一个可执行代码";
                            break;
                        case "IQY":
                            info = "Microsoft Internet查询文件";
                            break;
                        case "ISO":
                            info = "根据ISD 9660有关CD-ROM文件系统标准列出CD-ROM上的文件";
                            break;
                        case "IST":
                            info = "数字跟踪设备文件";
                            break;
                        case "ISU":
                            info = "InstallShield卸装脚本";
                            break;
                        case "IWC":
                            info = "Install Watch文档";
                            break;
                    }
                    break;
                case "J":
                    switch (ext)
                    {
                        case "J62":
                            info = "Ricoh照相机格式";
                            break;
                        case "JAR":
                            info = "Java档案文件";
                            break;
                        case "JAVA":
                            info = "Java源文件";
                            break;
                        case "JBF":
                            info = "Paint Shop Pro图像浏览文件";
                            break;
                        case "JFF":
                            info = "JPEG文件";
                            break;
                        case "JFIF":
                            info = "JPEG文件";
                            break;
                        case "JIF":
                            info = "JPEG文件";
                            break;
                        case "JMP":
                            info = "SAS的JMPDiscovery表格统计文件";
                            break;
                        case "JPE":
                            info = "JPEG图形文件";
                            break;
                        case "JPEG":
                            info = "JPEG图形文件";
                            break;
                        case "JPG":
                            info = "JPEG图形文件";
                            break;
                        case "JS":
                            info = "Javascript源文件";
                            break;
                        case "JSON":
                            info = "JavaScript Object Notation";
                            break;
                        case "JSP":
                            info = "HTML网页，其中包含有对一个Java servlet的参考";
                            break;
                        case "JTF":
                            info = "JPEG位图文件";
                            break;
                    }
                    break;
                case "K":
                    switch (ext)
                    {
                        case "K25":
                            info = "Kurzweil 2500抽样文件";
                            break;
                        case "KAR":
                            info = "卡拉OK MIDI文件";
                            break;
                        case "KDC":
                            info = "Kodak光增强器";
                            break;
                        case "KEY":
                            info = "DataCAD图标工具条文件";
                            break;
                        case "KFX":
                            info = "KoFak Group 4图像文件";
                            break;
                        case "KIZ":
                            info = "Kodak数字明信片文件";
                            break;
                        case "KKW":
                            info = "RoboHELP帮助工程索引设计器中与主题无关的K开头的所有关键字";
                            break;
                        case "KMP":
                            info = "Korg Trinity KeyMap文件";
                            break;
                        case "KQP":
                            info = "Konica照相机本地文件";
                            break;
                    }
                    break;
                case "L":
                    switch (ext)
                    {
                        case "LAB":
                            info = "Visual dBASE标签文件";
                            break;
                        case "LBM":
                            info = "Deluxe Paint位图文件";
                            break;
                        case "LBT":
                            info = "Microsoft FoxPro标签文件";
                            break;
                        case "LBX":
                            info = "Microsoft FoxPro标签文件";
                            break;
                        case "LDB":
                            info = "Microsoft Access加锁文件";
                            break;
                        case "LDL":
                            info = "Corel Paradox分发库";
                            break;
                        case "LEG":
                            info = "Legacy文档";
                            break;
                        case "LFT":
                            info = "3D Studio（DOS）放样文件";
                            break;
                        case "LGO":
                            info = "Paintbrush（Microsoft画图应用程序）的徽标文件";
                            break;
                        case "LHA":
                            info = "LZH更换文件后缀";
                            break;
                        case "LIB":
                            info = "库文件";
                            break;
                        case "LIN":
                            info = "DataCAD线型文件";
                            break;
                        case "LIS":
                            info = "结构化查询报告（SQR）程序产生的输出文件";
                            break;
                        case "LLX":
                            info = "Laplink交换代理";
                            break;
                        case "LNK":
                            info = "Windows快捷方式文件";
                            break;
                        case "LOG":
                            info = "日志文件";
                            break;
                        case "LST":
                            info = "列表文件";
                            break;
                        case "LU":
                            info = "ThoughtWing库单元文件";
                            break;
                        case "LYR":
                            info = "DataCAD层文件";
                            break;
                        case "LZH":
                            info = "LH ARC压缩档案";
                            break;
                        case "LZS":
                            info = "Skyroads数据文件";
                            break;
                    }
                    break;
                case "M":
                    switch (ext)
                    {
                        case "M1V":
                            info = "MPEG相关文件";
                            break;
                        case "M2":
                            info = "使用MPEG-2压缩编码的视频文件";
                            break;
                        case "M3U":
                            info = "MPEG URL（MIME声音文件）";
                            break;
                        case "M4V":
                            info = "苹果公司创造的视频文件格式";
                            break;
                        case "M4A":
                            info = "MPEG-4音频文件";
                            break;
                        case "MAD":
                            info = "Microsoft Access模块文件";
                            break;
                        case "MAF":
                            info = "Microsoft Access表单文件";
                            break;
                        case "MAM":
                            info = "Microsoft Access宏";
                            break;
                        case "MAP":
                            info = "映射文件；Duke Nukem 3D WAD游戏文件";
                            break;
                        case "MAQ":
                            info = "Microsoft Access查询文件";
                            break;
                        case "MAR":
                            info = "Microsoft Access报表文件";
                            break;
                        case "MAT":
                            info = "Microsoft Access表；3D Studio MAX材料库";
                            break;
                        case "MB1":
                            info = "Apogee Monster Bash数据文件";
                            break;
                        case "MBX":
                            info = "Microsoft Outlook保存email格式；Eudora邮箱";
                            break;
                        case "MCR":
                            info = "DataCAD键盘宏文件";
                            break;
                        case "MDB":
                            info = "Microsoft Access数据库";
                            break;
                        case "MDE":
                            info = "Microsoft Access MDE文件";
                            break;
                        case "MDL":
                            info = "数字跟踪器音乐模块（MOD）文件；Quake模块文件";
                            break;
                        case "MDN":
                            info = "Microsoft Access空数据库模板";
                            break;
                        case "MDW":
                            info = "Microsoft Access工作组文件";
                            break;
                        case "MDZ":
                            info = "Microsoft Access向导模板文件";
                            break;
                        case "MIC":
                            info = "Microsoft Image Composer文件";
                            break;
                        case "MID":
                            info = "MIDI音乐";
                            break;
                        case "MIM":
                            info = "Internet邮件扩展格式的多用途文件，经常作为发送e-mail时在AOL里附件而创建的文件；在一个多区MIM文件里的文件能用WinZip或其他类似程序打开";
                            break;
                        case "MME":
                            info = "Internet邮件扩展格式的多用途文件，经常作为发送e-mail时在AOL里附件而创建的文件；在一个多区MIM文件里的文件能用WinZip或其他类似程序打开";
                            break;
                        case "MIME":
                            info = "Internet邮件扩展格式的多用途文件，经常作为发送e-mail时在AOL里附件而创建的文件；在一个多区MIM文件里的文件能用WinZip或其他类似程序打开";
                            break;
                        case "MLI":
                            info = "3D Studio的材料库格式文件";
                            break;
                        case "MNG":
                            info = "多映像网络图形";
                            break;
                        case "MNU":
                            info = "Visual dBASE菜单文件；Intertel Systems Interact菜单文件";
                            break;
                        case "MOD":
                            info = "Fast Tracker、Star Trekker、Noise Tracker（等等）音乐模块文件；Microsoft多计划电子表格；Amiga/PC磁道文件";
                            break;
                        case "MOV":
                            info = "QuickTime for Windows电影";
                            break;
                        case "MP2":
                            info = "第二层MPEG音频文件";
                            break;
                        case "MP3":
                            info = "第三层MPEG音频文件";
                            break;
                        case "MP4":
                            info = "第四层MPEG音频文件";
                            break;
                        case "MPA":
                            info = "MPEG相关文件，MIME\"mpeg类型\"";
                            break;
                        case "MPE":
                            info = "MPEG动画文件";
                            break;
                        case "MPEG":
                            info = "MPEG动画文件";
                            break;
                        case "MPG":
                            info = "MPEG动画文件";
                            break;
                        case "MPP":
                            info = "Microsoft工程文件；CAD绘图文件格式";
                            break;
                        case "MPR":
                            info = "Microsoft FoxPro菜单（已编译）";
                            break;
                        case "MSG":
                            info = "Microsoft邮件消息";
                            break;
                        case "MSI":
                            info = "Windows Installe安装文件包";
                            break;
                        case "MSN":
                            info = "Microsoft网络文档；Descent Mission文件";
                            break;
                        case "MSP":
                            info = "Microsoft Paint（画图）位图文件；Windows Installer路径文件";
                            break;
                        case "MST":
                            info = "Windows 安装器传输文件";
                            break;
                        case "MTM":
                            info = "Multi 跟踪器音乐模块（MOD）文件";
                            break;
                        case "MXF":
                            info = "素材交换格式";
                            break;
                    }
                    break;
                case "N":
                    switch (ext)
                    {
                        case "NAN":
                            info = "Nanoscope文件（Raw Grayscale）";
                            break;
                        case "NAP":
                            info = "NAP元文件";
                            break;
                        case "NCB":
                            info = "Microsoft Developer Studio文件";
                            break;
                        case "NCD":
                            info = "Norton改变目录";
                            break;
                        case "NCF":
                            info = "NetWare命令文件；Lotus Notes内部剪切板";
                            break;
                        case "NFF":
                            info = "中性文件格式";
                            break;
                        case "NFT":
                            info = "NetObject Fusion模板文件";
                            break;
                        case "NIL":
                            info = "Norton光标库文件（EasyIcons-兼容）";
                            break;
                        case "NIST":
                            info = "NIST Sphere声音";
                            break;
                        case "NLS":
                            info = "用于本地化的国家语言支持文件（例如，Uniscape）";
                            break;
                        case "NLU":
                            info = "Norton Live Update e-mail 触发器文件";
                            break;
                        case "NTX":
                            info = "CA-Clipper索引文件";
                            break;
                        case "NWC":
                            info = "Noteworthy Composer歌曲文件";
                            break;
                        case "NWS":
                            info = "Microsoft Outlook Express新闻消息";
                            break;
                    }
                    break;
                case "O":
                    switch (ext)
                    {
                        case "ODT":
                            info = "OpenOffice创建的文件";
                            break;
                        case "OBJ":
                            info = "对象文件";
                            break;
                        case "OCX":
                            info = "Microsoft对象链接与嵌入定制控件";
                            break;
                        case "ODS":
                            info = "Microsoft Outlook Express邮箱文件";
                            break;
                        case "OFN":
                            info = "Microsoft Office FileNew文件";
                            break;
                        case "OFT":
                            info = "Microsoft Outlook模板";
                            break;
                        case "OLB":
                            info = "OLE对象库";
                            break;
                        case "OLE":
                            info = "OLE对象";
                            break;
                        case "OOGL":
                            info = "面向对象图形库";
                            break;
                        case "OPO":
                            info = "OPL输出可执行文件";
                            break;
                    }
                    break;
                case "P":
                    switch (ext)
                    {
                        case "P65":
                            info = "PageMaker 6.5文件";
                            break;
                        case "PAB":
                            info = "Microsoft个人地址簿";
                            break;
                        case "PART":
                            info = "Go!Zilla部分下载文件";
                            break;
                        case "PAS":
                            info = "Pascal源代码";
                            break;
                        case "PY":
                            info = "Python源代码";
                            break;
                        case "PYC":
                            info = "Python字节码文件";
                            break;
                        case "PYW":
                            info = "Python图形窗口文件";
                            break;
                        case "PBD":
                            info = "PowerBuilder动态库，作为本地DLL的一个替代物";
                            break;
                        case "PBL":
                            info = "用于在PowerBuilder开发环境中的PowerBuilder动态库";
                            break;
                        case "PBM":
                            info = "可导出位图";
                            break;
                        case "PBR":
                            info = "PowerBuilder资源";
                            break;
                        case "PCD":
                            info = "Kodak Photo-CD映像；P-Code编译器测试脚本，由Microsoft测试与Microsoft Visual测试";
                            break;
                        case "PCL":
                            info = "Hewlett-Packard 打印机控制语言文件（打印机备用位图）";
                            break;
                        case "PCM":
                            info = "声音文件格式；OKI MSM6376 合成芯片 PCM格式";
                            break;
                        case "PDD":
                            info = "可以用Paint Shop Pro或其他图像处理软件打开的图形图像";
                            break;
                        case "PDF":
                            info = "Adobe Acrobat 可导出文档格式文件（可用Web浏览器显示）；Microsoft系统管理服务器包定义文件；NetWare打印机定义文件";
                            break;
                        case "PFM":
                            info = "打印机字体尺度";
                            break;
                        case "PGL":
                            info = "HP绘图仪绘图文件";
                            break;
                        case "PGM":
                            info = "可输出灰度图（位图）";
                            break;
                        case "PH":
                            info = "由Microsoft帮助文件编译器产生的临时文件";
                            break;
                        case "PHP":
                            info = "包含有PHP脚本的HTML网页";
                            break;
                        case "PHP3":
                            info = "包含有PHP脚本的HTML网页";
                            break;
                        case "PHTML":
                            info = "包含有PHP脚本的HTML网页；由Perl分析解释的HTML";
                            break;
                        case "PIC":
                            info = "PC画图位图；Lotus图片；Macintosh PICT绘图";
                            break;
                        case "PJT":
                            info = "Microsoft Visual FoxPro工程文件";
                            break;
                        case "PJX":
                            info = "Microsoft Visual FoxPro工程文件";
                            break;
                        case "PKG":
                            info = "Microsoft Developer Studio应用程序扩展（与DLL文件类似）";
                            break;
                        case "PNG":
                            info = "可移植的网络图形位图；Paint Shop Pro浏览器目录";
                            break;
                        case "POT":
                            info = "Microsoft Powerpoint模块";
                            break;
                        case "PPA":
                            info = "Microsoft Powerpoint内插器";
                            break;
                        case "PPF":
                            info = "Turtle Beach的Pinnacle程序文件";
                            break;
                        case "PPM":
                            info = "可移植的象素映射位图";
                            break;
                        case "PPS":
                            info = "Microsoft Powerpoint幻灯片放映";
                            break;
                        case "PPT":
                            info = "Microsoft Powerpoint 97-2003 幻灯片演示文稿";
                            break;
                        case "PPTX":
                            info = "Microsoft Powerpoint2007 幻灯片演示文稿";
                            break;
                        case "PRF":
                            info = "Windows系统文件，Macromedia导演设置文件";
                            break;
                        case "PRG":
                            info = "dBASE Clipper和FoxPro程序源文件；WAVmaker程序";
                            break;
                        case "PRJ":
                            info = "3D Studio（DOS）工程文件";
                            break;
                        case "PRN":
                            info = "打印表格（用空格分隔的文本）；DataCAD Windows打印机文件";
                            break;
                        case "PRPROJ":
                            info = "Adobe Premiere Pro 项目文件";
                            break;
                        case "PRT":
                            info = "打印格式化文件；Pro/ENGINEER元件文件";
                            break;
                        case "PSD":
                            info = "Adobe photoshop位图文件";
                            break;
                        case "PSP":
                            info = "Paint Shop Pro图像文件";
                            break;
                        case "PST":
                            info = "Microsoft Outlook个人文件夹文件";
                            break;
                        case "PWZ":
                            info = "Microsoft Powerpoint向导";
                            break;
                    }
                    break;
                case "Q":
                    switch (ext)
                    {
                        case "QIC":
                            info = "Microsoft备份文件";
                            break;
                        case "QIF":
                            info = "QuickTime相关图像（MIME）；Quicken导入文件";
                            break;
                        case "QLB":
                            info = "Quick库";
                            break;
                        case "QRY":
                            info = "Microsoft查询文件";
                            break;
                        case "QTP":
                            info = "QuickTime优先文件";
                            break;
                        case "QTX":
                            info = "QuickTime相关图像";
                            break;
                        case "QW":
                            info = "Symantec Q&A Write程序文件";
                            break;
                    }
                    break;
                case "R":
                    switch (ext)
                    {
                        case "RA":
                            info = "RealAudio声音文件";
                            break;
                        case "RAM":
                            info = "RealAudio元文件";
                            break;
                        case "RAR":
                            info = "WINRAR压缩档案（Eugene Roshall格式）";
                            break;
                        case "RDF":
                            info = "资源描述框架文件（涉及XML和元数据）";
                            break;
                        case "REG":
                            info = "注册表文件";
                            break;
                        case "REP":
                            info = "Visual dBASE报表文件";
                            break;
                        case "RES":
                            info = "Microsoft Visual C++资源文件";
                            break;
                        case "RESMONCFG":
                            info = "资源监视器配置";
                            break;
                        case "RFT":
                            info = "可修订的表单文本（IBM的DCA一部分或文档内容框架结构一部分）";
                            break;
                        case "RGB":
                            info = "Silicon图形RGB文件";
                            break;
                        case "SGI":
                            info = "Silicon图形RGB文件";
                            break;
                        case "RM":
                            info = "RealAudio视频文件";
                            break;
                        case "RMD":
                            info = "Microsoft RegMaid文档";
                            break;
                        case "RPT":
                            info = "Microsoft Visual Basic Crystal报表文件";
                            break;
                        case "RTF":
                            info = "Rich Text格式文档";
                            break;
                        case "RUL":
                            info = "InstallShield使用的扩展名";
                            break;
                        case "RVP":
                            info = "Microsoft Scan配置文件（MIME）";
                            break;
                    }
                    break;
                case "S":
                    switch (ext)
                    {
                        case "S":
                            info = "汇编源代码文件";
                            break;
                        case "SAV":
                            info = "游戏保存文件";
                            break;
                        case "SB2":
                            info = "scratch2.0文件";
                            break;
                        case "SB3":
                            info = "scratch3.0文件";
                            break;
                        case "SPRITE2":
                            info = "scratch2.0角色文件";
                            break;
                        case "SPRITE3":
                            info = "scratch3.0角色文件";
                            break;
                        case "SBL":
                            info = "Shockwave Flash对象文件";
                            break;
                        case "SCC":
                            info = "Microsoft Source Safe文件";
                            break;
                        case "SCF":
                            info = "Windows Explorer命令文件";
                            break;
                        case "SCP":
                            info = "拨号网络脚本文件";
                            break;
                        case "SCR":
                            info = "Windows屏幕保护；传真图像；脚本文件";
                            break;
                        case "SCT":
                            info = "SAS目录（DOS）；Scitex CT位图；Microsoft FoxPro表单";
                            break;
                        case "SCX":
                            info = "Microsoft FoxPro表单文件";
                            break;
                        case "SDT":
                            info = "SmartDraw模板";
                            break;
                        case "SDV":
                            info = "分号分隔的值文件";
                            break;
                        case "SDX":
                            info = "由SDX压缩的MIDI抽样转储标准文件";
                            break;
                        case "SEARCH-MS":
                            info = "已保存的搜索";
                            break;
                        case "SEARCHCONNECTOR-MS":
                            info = "搜索连接器";
                            break;
                        case "SEP":
                            info = "标签图像文件格式（TIFF）位图";
                            break;
                        case "SETTINGCONTENT-MS":
                            info = "设置内容";
                            break;
                        case "SFD":
                            info = "SoundStage声音文件数据";
                            break;
                        case "SFI":
                            info = "Sound Stage声音文件信息";
                            break;
                        case "SFR":
                            info = "Sonic Foundry Sample资源";
                            break;
                        case "SFX":
                            info = "RAR自解压文件";
                            break;
                        case "SGML":
                            info = "标准通用标签语言";
                            break;
                        case "SHG":
                            info = "热点位图";
                            break;
                        case "SHTML":
                            info = "含有服务器端包括（SSI）的HTML文件";
                            break;
                        case "SHW":
                            info = "Corel Show演示文稿";
                            break;
                        case "SIG":
                            info = "符号文件";
                            break;
                        case "SKA":
                            info = "PGP秘钥";
                            break;
                        case "SKL":
                            info = "Macromedia导演者资源文件";
                            break;
                        case "SL":
                            info = "PACT的保存布局扩展名";
                            break;
                        case "SO":
                            info = "Linux的共享库";
                            break;
                        case "SPL":
                            info = "Shockwave Flash对象；DigiTrakker抽样";
                            break;
                        case "SQC":
                            info = "结构化查询语言（SQR）普通代码文件";
                            break;
                        case "SQR":
                            info = "结构化查询语言（SQR）程序文件";
                            break;
                        case "STR":
                            info = "屏幕保护文件";
                            break;
                        case "SWA":
                            info = "在Macromedia导演文件（MP3文件）中的Shockwave声音文件";
                            break;
                        case "SWF":
                            info = "Shockwave Flash对象";
                            break;
                        case "SYS":
                            info = "系统文件";
                            break;
                        case "SYW":
                            info = "Yamaha SY系列波形文件";
                            break;
                        case "SLDPRT":
                            info = "SolidWorks零件";
                            break;
                        case "SLDASM":
                            info = "SolidWorks装配";
                            break;
                    }
                    break;
                case "T":
                    switch (ext)
                    {
                        case "TAR":
                            info = "磁带存档文件，通常用于服务器为先导";
                            break;
                        case "TAZ":
                            info = "UNIX gzip/tape档案";
                            break;
                        case "TGA":
                            info = "Targa位图";
                            break;
                        case "THEME":
                            info = "Windows桌面主题文件";
                            break;
                        case "THN":
                            info = "Graphics WorkShop for Windows速写";
                            break;
                        case "TIFF":
                            info = "标签图像文件格式（TIFF）位图";
                            break;
                        case "TIF":
                            info = "标签图像文件格式（TIFF）位图";
                            break;
                        case "TIG":
                            info = "虎形文件，美国政府用于分发地图";
                            break;
                        case "TLB":
                            info = "OLE类型库";
                            break;
                        case "TMP":
                            info = "Windows临时文件";
                            break;
                        case "TOL":
                            info = "Kodak照片增强器";
                            break;
                        case "TPL":
                            info = "CakeWalk声音模板文件；DataCAD模板文件";
                            break;
                        case "TRM":
                            info = "终端文件";
                            break;
                        case "TRN":
                            info = "MKS源完整性工程用法日志文件";
                            break;
                        case "TTF":
                            info = "TrueType字体文件";
                            break;
                        case "TXT":
                            info = "文本文档；ASCⅡ文本格式的声音数据";
                            break;
                        case "TXW":
                            info = "Yamaha TX16W波形文件";
                            break;
                    }
                    break;
                case "U":
                    switch (ext)
                    {
                        case "UDF":
                            info = "Windows NT/2000唯一性数据库文件";
                            break;
                        case "ULT":
                            info = "Ultra Tracker音乐模块（MOD）文件";
                            break;
                        case "URL":
                            info = "Internet快捷方式文件";
                            break;
                        case "USE":
                            info = "MKS源完整性文件";
                            break;
                        case "UWF":
                            info = "Ultra racker波形文件";
                            break;
                    }
                    break;
                case "V":
                    switch (ext)
                    {
                        case "VBP":
                            info = "Microsoft Visual Basic工程文件";
                            break;
                        case "VBW":
                            info = "Microsoft Visual Basic工作区文件";
                            break;
                        case "VBX":
                            info = "Microsoft Visual Basic用户定制控件";
                            break;
                        case "VCT":
                            info = "Microsoft FoxPro类库";
                            break;
                        case "VCX":
                            info = "Microsoft FoxPro类库";
                            break;
                        case "VDA":
                            info = "Targa位图";
                            break;
                        case "VIR":
                            info = "Norton Anti-Virus或其他杀毒产品用于标识被病毒感染的文件";
                            break;
                        case "VIV":
                            info = "VivoActive Player流视频文件";
                            break;
                        case "VSD":
                            info = "Visio绘画文件（流程图或图解）";
                            break;
                        case "VSL":
                            info = "下载列表文件（GetRight）";
                            break;
                        case "VSS":
                            info = "Visio模板文件";
                            break;
                        case "VST":
                            info = "Targa位图";
                            break;
                        case "VSW":
                            info = "Visio工作区文件";
                            break;
                        case "VXD":
                            info = "Microsoft Windows虚拟设备驱动程序";
                            break;
                        case "VBS":
                            info = "Microsoft Visual Basic Script Edition微软可视化BASIC脚本";
                            break;
                        case "VBA":
                            info = "Visual Basic的一种宏语言";
                            break;
                        case "VQF":
                            info = "Yamaha Sound-VQ文件（可能出现标准）";
                            break;
                    }
                    break;
                case "W":
                    switch (ext)
                    {
                        case "W3L":
                            info = "W3Launch文件";
                            break;
                        case "WAB":
                            info = "Microsoft Outlook文件";
                            break;
                        case "WAD":
                            info = "包含有视频、玩家水平和其他信息的DOOM游戏的大文件";
                            break;
                        case "WAV":
                            info = "Windows波形声形";
                            break;
                        case "WBK":
                            info = "Microsoft Word备份文件";
                            break;
                        case "WCM":
                            info = "WordPerfect宏";
                            break;
                        case "WDB":
                            info = "Microsoft Works数据库";
                            break;
                        case "WFM":
                            info = "Visual dBASE Windows表单";
                            break;
                        case "WFN":
                            info = "在CorelDRAW中使用的符号";
                            break;
                        case "WIL":
                            info = "WinImage文件";
                            break;
                        case "WIZ":
                            info = "Microsoft Word向导";
                            break;
                        case "WLL":
                            info = "Microsoft Word内插器";
                            break;
                        case "WMA":
                            info = "Windows 音频格式";
                            break;
                        case "WMF":
                            info = "Windows元文件";
                            break;
                        case "WMV":
                            info = "Windows 媒体视频格式";
                            break;
                        case "WOW":
                            info = "Grave Composer音乐模块（MOD）文件";
                            break;
                        case "WP":
                            info = "WordPerfect文档";
                            break;
                        case "WPD":
                            info = "WordPerfect文档或演示";
                            break;
                        case "WPF":
                            info = "可字处理文档";
                            break;
                        case "WPG":
                            info = "WordPerfect图形";
                            break;
                        case "WPS":
                            info = "Microsoft Works文档";
                            break;
                        case "WPT":
                            info = "WordPerfect模板";
                            break;
                        case "WR1":
                            info = "书写器文档";
                            break;
                        case "WRK":
                            info = "Cakewalk音乐声音工程文件";
                            break;
                        case "WRL":
                            info = "虚拟现实模型";
                            break;
                        case "WRZ":
                            info = "VRML文件对象";
                            break;
                    }
                    break;
                case "X":
                    switch (ext)
                    {
                        case "X":
                            info = "AVS图像格式";
                            break;
                        case "XAR":
                            info = "CorelXARA绘画";
                            break;
                        case "XBM":
                            info = "MIME\"xbitmap\"图像";
                            break;
                        case "XI":
                            info = "Scream Tracker设备抽样文件";
                            break;
                        case "XLA":
                            info = "Microsoft Excel内插器";
                            break;
                        case "XLB":
                            info = "Microsoft Excel工具条";
                            break;
                        case "XLC":
                            info = "Microsoft Excel图表";
                            break;
                        case "XLD":
                            info = "Microsoft Excel对话框";
                            break;
                        case "XLK":
                            info = "Microsoft Excel备份";
                            break;
                        case "XLL":
                            info = "Microsoft Excel内插器文件";
                            break;
                        case "XLM":
                            info = "Microsoft Excel宏";
                            break;
                        case "XLS":
                            info = "Microsoft Office Excel 97-2003 工作表";
                            break;
                        case "XLSX":
                            info = "Microsoft Office Excel 2007 工作表";
                            break;
                        case "XLT":
                            info = "Microsoft Excel模板";
                            break;
                        case "XLV":
                            info = "Microsoft Excel VBA模块";
                            break;
                        case "XLW":
                            info = "Microsoft Excel工作簿/工作区";
                            break;
                        case "XNK":
                            info = "Microsoft Exchange快捷方式文件";
                            break;
                        case "XPM":
                            info = "X位图格式";
                            break;
                        case "XWD":
                            info = "X Windows转储格式";
                            break;
                        case "XWF":
                            info = "Yamaha XG Works文件（MIDI序列）";
                            break;
                        case "X16":
                            info = "宏媒体扩展（程序扩展），16位";
                            break;
                        case "X32":
                            info = "宏媒体扩展（程序扩展），32位";
                            break;
                    }
                    break;
                case "Y":
                    switch (ext)
                    {
                        case "YAL":
                            info = "Arts& Letters剪贴艺术库";
                            break;
                    }
                    break;
                case "Z":
                    switch (ext)
                    {
                        case "Z":
                            info = "UNIX gzip文件";
                            break;
                        case "ZAP":
                            info = "Windows软件安装配置文件";
                            break;
                        case "ZIP":
                            info = "Zip压缩文件";
                            break;
                    }
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(info)) { ext = info; }
            return ext;
        }
    }
}
