﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyKTV_SongMgr
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            SongQuery_DataGridView.MakeDoubleBuffered(true);
            SongAdd_DataGridView.MakeDoubleBuffered(true);
            SingerMgr_DataGridView.MakeDoubleBuffered(true);
            Cashbox_DataGridView.MakeDoubleBuffered(true);


        }

        
        private void MainForm_Load(object sender, EventArgs e)
        {
            string CurVer = " v" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileMajorPart + "." +
                                   FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileMinorPart + "." +
                                   FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileBuildPart;

            if (FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FilePrivatePart > 0) CurVer += "." + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FilePrivatePart;

            this.Text += CurVer;

            if (CommonFunc.IsAdministrator())
            {
                this.Text += " (系統管理員)";
                SongAdd_Tooltip_Label.Text = "請勿使用管理員身份執行加歌程式，否則將無法用檔案總管來拖曳檔案。";
            }
                


            #if DEBUG
                Cashbox_EditMode_CheckBox.Visible = true;
            #else
                Debug_TabPage.Hide();
                MainTabControl.TabPages.Remove(Debug_TabPage);
            #endif

            Global.DPIScalingFactor = Common_GetDPIScalingFactor();
            SongAddResult_SplitContainer.SplitterDistance = Convert.ToInt32(SongAddResult_SplitContainer.SplitterDistance * Global.DPIScalingFactor);

            // 檢查加歌程式參考資料庫檔案
            if (!File.Exists(Global.CrazyktvSongMgrDatabaseFile))
            {
                if (File.Exists(Application.StartupPath + @"\CrazySongMgr.mdb"))
                {
                    try
                    {
                        File.Move(Application.StartupPath + @"\CrazySongMgr.mdb", Global.CrazyktvSongMgrDatabaseFile);
                    }
                    catch
                    {
                        if (MessageBox.Show("請將 CrazySongMgr.mdb 移至 SongMgr 資料夾裡。" + Environment.NewLine + "或是重新下載並執行 CrazyKTV_WebUpdater.exe 來重建所有檔案。", @"SongMgr\CrazySongMgr.mdb 參考資料庫檔案不存在!", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                        {
                            Application.Exit();
                        }
                    }
                }
                else
                {
                    if (MessageBox.Show("請將 CrazySongMgr.mdb 移至 SongMgr 資料夾裡。" + Environment.NewLine + "或是重新下載並執行 CrazyKTV_WebUpdater.exe 來重建所有檔案。", @"SongMgr\CrazySongMgr.mdb 參考資料庫檔案不存在!", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {
                        Application.Exit();
                    }
                }
            }

            // 歌庫版本資訊
            if (!File.Exists(Global.CrazyktvSongDBUpdateFile))
            {
                if (!Directory.Exists(Application.StartupPath + @"\SongMgr\Update")) Directory.CreateDirectory(Application.StartupPath + @"\SongMgr\Update");
                CommonFunc.CreateConfigXmlFile(Global.CrazyktvSongDBUpdateFile);
                CommonFunc.SaveConfigXmlFile(Global.CrazyktvSongDBUpdateFile, "SongDBVer", Global.CrazyktvSongDBVer);
            }

            // CrazyKTV 設定
            if (!File.Exists(Global.CrazyktvCfgFile))
            {
                CommonFunc.CreateConfigXmlFile(Global.CrazyktvCfgFile);
            }

            // 載入歌庫設定
            if (!File.Exists(Global.SongMgrCfgFile))
            {
                CommonFunc.CreateConfigXmlFile(Global.SongMgrCfgFile);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvDatabaseFile", Global.CrazyktvDatabaseFile);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSupportFormat", Global.SongMgrSupportFormat);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrDestFolder", Global.SongMgrDestFolder);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongAddMode", Global.SongMgrSongAddMode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrChorusMerge", Global.SongMgrChorusMerge);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrMaxDigitCode", Global.SongMgrMaxDigitCode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrLangCode", Global.SongMgrLangCode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongType", Global.SongMgrSongType);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongInfoSeparate", Global.SongMgrSongInfoSeparate);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrChorusSeparate", Global.SongMgrChorusSeparate);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongLang", Global.SongAddDefaultSongLang);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSingerType", Global.SongAddDefaultSingerType);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongTrack", Global.SongAddDefaultSongTrack);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongType", Global.SongAddDefaultSongType);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongVolume", Global.SongAddDefaultSongVolume);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddSpecialStr", Global.SongAddSpecialStr);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrFolderStructure", Global.SongMgrFolderStructure);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrFileStructure", Global.SongMgrFileStructure);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongTrackMode", Global.SongMgrSongTrackMode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddSongIdentificationMode", Global.SongAddSongIdentificationMode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddDupSongMode", Global.SongAddDupSongMode);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangStr", string.Join(",", Global.CrazyktvSongLangList));
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangKeyWord", string.Join("|", Global.CrazyktvSongLangKeyWordList));
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrBackupRemoveSong", Global.SongMgrBackupRemoveSong);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddEngSongNameFormat", Global.SongAddEngSongNameFormat);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgAlwaysOnTop", Global.MainCfgAlwaysOnTop);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongDBConverterTabPage", Global.MainCfgHideSongDBConverterTabPage);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongAddResultTabPage", Global.MainCfgHideSongAddResultTabPage);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongLogTabPage", Global.MainCfgHideSongLogTabPage);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgBackupRemoveSongDays", Global.MainCfgBackupRemoveSongDays);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "DBVerEnableDBVerUpdate", Global.DBVerEnableDBVerUpdate);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgUIScale", Global.MainCfgUIScale);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMaintenanceEnableMultiSongPath", Global.SongMaintenanceEnableMultiSongPath); // 已無用處
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMaintenanceMultiSongPath", string.Join(",", Global.SongMaintenanceMultiSongPathList)); // 已無用處
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddUseCustomSongID", Global.SongAddUseCustomSongID);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgUIFont", Global.MainCfgUIFont);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgEnableUIScale", Global.MainCfgEnableUIScale);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrCustomSingerTypeStructure", Global.SongMgrCustomSingerTypeStructure);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgEnableAutoUpdate", Global.MainCfgEnableAutoUpdate);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongAddEnableConvToTC", Global.SongAddEnableConvToTC);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrEnableMonitorFolders", Global.SongMgrEnableMonitorFolders);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrMonitorFolders", string.Join(",", Global.SongMgrMonitorFoldersList));
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SingerMgrSyncSongSinger", Global.SingerMgrSyncSongSinger);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSingerGroup", Global.SongMgrSingerGroup);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSingerGroup", Global.SongMgrSingerGroup);
                CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideApplyCashboxIdButton", Global.MainCfgHideApplyCashboxIdButton);
            }

            List<string> list = new List<string>()
            { 
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvDatabaseFile"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSupportFormat"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrDestFolder"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongAddMode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrChorusMerge"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrMaxDigitCode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrLangCode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongType"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongInfoSeparate"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrChorusSeparate"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongLang"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSingerType"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongTrack"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongType"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDefaultSongVolume"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddSpecialStr"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrFolderStructure"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrFileStructure"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSongTrackMode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddSongIdentificationMode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddDupSongMode"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangStr"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangKeyWord"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrBackupRemoveSong"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddEngSongNameFormat"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgAlwaysOnTop"),
                CommonFunc.LoadConfigXmlFile(Global.CrazyktvSongDBUpdateFile, "SongDBVer"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongDBConverterTabPage"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongAddResultTabPage"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideSongLogTabPage"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgBackupRemoveSongDays"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SingerMgrDefaultSingerDataTable"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "DBVerEnableDBVerUpdate"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgUIScale"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMaintenanceEnableMultiSongPath"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMaintenanceMultiSongPath"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddUseCustomSongID"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgUIFont"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgEnableUIScale"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrCustomSingerTypeStructure"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgEnableAutoUpdate"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongAddEnableConvToTC"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrEnableMonitorFolders"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrMonitorFolders"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SingerMgrSyncSongSinger"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "SongMgrSingerGroup"),
                CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "MainCfgHideApplyCashboxIdButton")
        };

            foreach (TabPage MainTabPage in MainTabControl.TabPages)
            {
                MainTabPage.Show();
            }

            foreach (TabPage SongQueryTabPage in SongQuery_TabControl.TabPages)
            {
                SongQueryTabPage.Show();
            }

            foreach (TabPage SongMgrCfgTabPage in SongMgrCfg_TabControl.TabPages)
            {
                SongMgrCfgTabPage.Show();
            }

            foreach (TabPage SongMaintenanceTabPage in SongMaintenance_TabControl.TabPages)
            {
                SongMaintenanceTabPage.Show();
            }

            foreach (TabPage CashboxTabPage in Cashbox_TabControl.TabPages)
            {
                CashboxTabPage.Show();
            }

            if (list[21] != "") Global.CrazyktvSongLangList = new List<string>(list[21].Split(','));
            if (Global.CrazyktvSongLangList.Count < 10) Global.CrazyktvSongLangList = new List<string>() { "國語", "台語", "粵語", "日語", "英語", "客語", "原住民語", "韓語", "兒歌", "其它" };

            if (list[0] != "") Global.CrazyktvDatabaseFile = list[0];
            SongMgrCfg_DBFile_TextBox.Text = Global.CrazyktvDatabaseFile;

            if (list[1] != "") Global.SongMgrSupportFormat = list[1];
            SongMgrCfg_SupportFormat_TextBox.Text = Global.SongMgrSupportFormat;

            if (list[2] != "") Global.SongMgrDestFolder = list[2];
            SongMgrCfg_DestFolder_TextBox.Text = Global.SongMgrDestFolder;
            
            if (list[4] != "") Global.SongMgrChorusMerge = list[4];
            SongMgrCfg_CrtchorusMerge_CheckBox.Checked = bool.Parse(Global.SongMgrChorusMerge);

            if (list[5] != "") Global.SongMgrMaxDigitCode = list[5];
            SongMgrCfg_MaxDigitCode_ComboBox.DataSource = SongMgrCfg.GetMaxDigitCodeList();
            SongMgrCfg_MaxDigitCode_ComboBox.DisplayMember = "Display";
            SongMgrCfg_MaxDigitCode_ComboBox.ValueMember = "Value";
            SongMgrCfg_MaxDigitCode_ComboBox.SelectedValue = int.Parse(Global.SongMgrMaxDigitCode);

            if (list[6] != "") { Global.SongMgrLangCode = list[6]; SongMgrCfg_LoadSongMgrLangCode(); } else { SongMgrCfg_RefreshSongMgrLangCode(); }

            if (list[7] != "") Global.SongMgrSongType = list[7];
            SongMgrCfg_SongType_ListBox.DataSource = SongMgrCfg.GetSongTypeList();
            SongMgrCfg_SongType_ListBox.DisplayMember = "Display";
            SongMgrCfg_SongType_ListBox.ValueMember = "Value";

            if (list[8] != "") Global.SongMgrSongInfoSeparate = list[8];
            SongMgrCfg_SongInfoSeparate_ComboBox.DataSource = SongMgrCfg.GetSongInfoSeparateList();
            SongMgrCfg_SongInfoSeparate_ComboBox.DisplayMember = "Display";
            SongMgrCfg_SongInfoSeparate_ComboBox.ValueMember = "Value";
            SongMgrCfg_SongInfoSeparate_ComboBox.SelectedValue = int.Parse(Global.SongMgrSongInfoSeparate);

            if (list[9] != "") Global.SongMgrChorusSeparate = list[9];
            SongMgrCfg_CrtchorusSeparate_ComboBox.DataSource = SongMgrCfg.GetCrtchorusSeparateList();
            SongMgrCfg_CrtchorusSeparate_ComboBox.DisplayMember = "Display";
            SongMgrCfg_CrtchorusSeparate_ComboBox.ValueMember = "Value";
            SongMgrCfg_CrtchorusSeparate_ComboBox.SelectedValue = int.Parse(Global.SongMgrChorusSeparate);

            if (list[10] != "") Global.SongAddDefaultSongLang = list[10];
            SongAdd_DefaultSongLang_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongLang", false, false);
            SongAdd_DefaultSongLang_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSongLang_ComboBox.ValueMember = "Value";
            SongAdd_DefaultSongLang_ComboBox.SelectedValue = int.Parse(Global.SongAddDefaultSongLang);

            if (list[11] != "") Global.SongAddDefaultSingerType = list[11];
            SongAdd_DefaultSingerType_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSingerType", false, false);
            SongAdd_DefaultSingerType_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSingerType_ComboBox.ValueMember = "Value";
            SongAdd_DefaultSingerType_ComboBox.SelectedValue = int.Parse(Global.SongAddDefaultSingerType);

            if (list[12] != "") Global.SongAddDefaultSongTrack = list[12];
            SongAdd_DefaultSongTrack_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongTrack", false, false);
            SongAdd_DefaultSongTrack_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSongTrack_ComboBox.ValueMember = "Value";
            SongAdd_DefaultSongTrack_ComboBox.SelectedValue = int.Parse(Global.SongAddDefaultSongTrack);

            if (list[13] != "") Global.SongAddDefaultSongType = list[13];
            SongAdd_DefaultSongType_ComboBox.DataSource = SongAdd.GetDefaultSongInfo("DefaultSongType", false, false);
            SongAdd_DefaultSongType_ComboBox.DisplayMember = "Display";
            SongAdd_DefaultSongType_ComboBox.ValueMember = "Value";
            SongAdd_DefaultSongType_ComboBox.SelectedValue = int.Parse(Global.SongAddDefaultSongType);

            if (list[14] != "") Global.SongAddDefaultSongVolume = list[14];
            SongAdd_DefaultSongVolume_TextBox.Text = Global.SongAddDefaultSongVolume;

            if (list[15] != "") Global.SongAddSpecialStr = list[15];
            SongAdd_SpecialStr_ListBox.DataSource = SongAdd.GetDefaultSongInfo("SpecialStr", false, false);
            SongAdd_SpecialStr_ListBox.DisplayMember = "Display";
            SongAdd_SpecialStr_ListBox.ValueMember = "Value";

            if (list[16] != "") Global.SongMgrFolderStructure = list[16];
            SongMgrCfg_FolderStructure_ComboBox.DataSource = SongMgrCfg.GetFolderStructureList();
            SongMgrCfg_FolderStructure_ComboBox.DisplayMember = "Display";
            SongMgrCfg_FolderStructure_ComboBox.ValueMember = "Value";
            SongMgrCfg_FolderStructure_ComboBox.SelectedValue = Global.SongMgrFolderStructure;

            if (list[17] != "") Global.SongMgrFileStructure = list[17];
            SongMgrCfg_FileStructure_ComboBox.DataSource = SongMgrCfg.GetFileStructureList();
            SongMgrCfg_FileStructure_ComboBox.DisplayMember = "Display";
            SongMgrCfg_FileStructure_ComboBox.ValueMember = "Value";
            SongMgrCfg_FileStructure_ComboBox.SelectedValue = Global.SongMgrFileStructure;

            if (list[18] != "") Global.SongMgrSongTrackMode = list[18];
            SongMgrCfg_SongTrackMode_CheckBox.Checked = bool.Parse(Global.SongMgrSongTrackMode);
            if(Global.SongMgrSongTrackMode == "True")
            {
                Global.CrazyktvSongTrackWordList = new List<string>() { "立體聲", "右聲道 / 音軌2", "左聲道 / 音軌1", "音軌3", "音軌4", "音軌5" };
                Global.CrazyktvSongTrackList = new List<string>() { "V0", "VR", "VL", "V3", "V4", "V5" };
                Global.CrazyktvSongTrackKeyWordList = new List<string>() { "v0,立體", "vr,r,右", "vl,l,左", "v3", "v4", "v5" };
            }
            else
            {
                Global.CrazyktvSongTrackWordList = new List<string>() { "立體聲", "左聲道 / 音軌1", "右聲道 / 音軌2", "音軌3", "音軌4", "音軌5" };
                Global.CrazyktvSongTrackList = new List<string>() { "V0", "VL", "VR", "V3", "V4", "V5" };
                Global.CrazyktvSongTrackKeyWordList = new List<string>() { "v0,立體", "vl,l,左", "vr,r,右", "v3", "v4", "v5" };
            }

            if (list[19] != "") Global.SongAddSongIdentificationMode = list[19];
            SongAdd_SongIdentificationMode_ComboBox.DataSource = SongAdd.GetSongIdentificationModeList();
            SongAdd_SongIdentificationMode_ComboBox.DisplayMember = "Display";
            SongAdd_SongIdentificationMode_ComboBox.ValueMember = "Value";
            SongAdd_SongIdentificationMode_ComboBox.SelectedValue = Global.SongAddSongIdentificationMode;

            if (list[20] != "") Global.SongAddDupSongMode = list[20];
            SongAdd_DupSongMode_ComboBox.DataSource = SongAdd.GetDupSongModeList();
            SongAdd_DupSongMode_ComboBox.DisplayMember = "Display";
            SongAdd_DupSongMode_ComboBox.ValueMember = "Value";
            SongAdd_DupSongMode_ComboBox.SelectedValue = Global.SongAddDupSongMode;

            if (list[22] != "")
            {
                Global.CrazyktvSongLangKeyWordList = new List<string>(list[22].Split('|'));
            }
            else
            {
                string str = CommonFunc.LoadConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangIDStr");
                Global.CrazyktvSongLangKeyWordList = new List<string>(str.Split('*'));
                if (Global.CrazyktvSongLangKeyWordList.Count < 10)
                {
                    Global.CrazyktvSongLangKeyWordList = new List<string>() { "國語,國", "台語,台,閩南,閩", "粵語,粵,廣東", "日語,日文,日", "英語,英文,英", "客語,客", "原住民語,民謠", "韓語,韓", "兒歌,兒", "其它" };
                }
                else
                {
                    CommonFunc.SaveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangKeyWord", string.Join("|", Global.CrazyktvSongLangKeyWordList));
                    CommonFunc.RemoveConfigXmlFile(Global.SongMgrCfgFile, "CrazyktvSongLangIDStr");
                }
            }

            if (list[23] != "") Global.SongMgrBackupRemoveSong = list[23];
            SongMgrCfg_BackupRemoveSong_CheckBox.Checked = bool.Parse(Global.SongMgrBackupRemoveSong);

            if (list[24] != "") Global.SongAddEngSongNameFormat = list[24];
            SongAdd_EngSongNameFormat_CheckBox.Checked = bool.Parse(Global.SongAddEngSongNameFormat);

            if (list[25] != "") Global.MainCfgAlwaysOnTop = list[25];
            MainCfg_AlwaysOnTop_CheckBox.Checked = bool.Parse(Global.MainCfgAlwaysOnTop);

            if (list[26] != "") Global.CrazyktvSongDBVer = list[26];

            if (list[27] != "") Global.MainCfgHideSongDBConverterTabPage = list[27];
            MainCfg_HideSongDBConvTab_CheckBox.Checked = bool.Parse(Global.MainCfgHideSongDBConverterTabPage);

            if (list[28] != "") Global.MainCfgHideSongAddResultTabPage = list[28];
            MainCfg_HideSongAddResultTab_CheckBox.Checked = bool.Parse(Global.MainCfgHideSongAddResultTabPage);

            if (list[29] != "") Global.MainCfgHideSongLogTabPage = list[29];
            MainCfg_HideSongLogTab_CheckBox.Checked = bool.Parse(Global.MainCfgHideSongLogTabPage);

            if (list[30] != "") Global.MainCfgBackupRemoveSongDays = list[30];
            MainCfg_BackupRemoveSongDays_ComboBox.DataSource = MainCfg.GetBackupRemoveSongDaysList();
            MainCfg_BackupRemoveSongDays_ComboBox.DisplayMember = "Display";
            MainCfg_BackupRemoveSongDays_ComboBox.ValueMember = "Value";
            MainCfg_BackupRemoveSongDays_ComboBox.SelectedValue = Global.MainCfgBackupRemoveSongDays;

            if (list[31] != "") Global.SingerMgrDefaultSingerDataTable = list[31];
            SingerMgr_DefaultSingerDataTable_ComboBox.DataSource = SingerMgr.GetDefaultSingerDataTableList();
            SingerMgr_DefaultSingerDataTable_ComboBox.DisplayMember = "Display";
            SingerMgr_DefaultSingerDataTable_ComboBox.ValueMember = "Value";

            #if DEBUG
                SingerMgr_DefaultSingerDataTable_ComboBox.SelectedValue = (Global.SingerMgrDefaultSingerDataTable == "ktv_Singer") ? 1 : 2;
            #else
                Global.SingerMgrDefaultSingerDataTable = "ktv_Singer";
                SingerMgr_DefaultSingerDataTable_ComboBox.SelectedValue = 1;
                SingerMgr_DefaultSingerDataTable_ComboBox.Enabled = false;
            #endif

            if (list[32] != "") Global.DBVerEnableDBVerUpdate = list[32];
            SongMaintenance_EnableDBVerUpdate_CheckBox.Checked = bool.Parse(Global.DBVerEnableDBVerUpdate);

            if (list[33] != "") Global.MainCfgUIScale = list[33];
            MainCfg_UIScale_ComboBox.DataSource = MainCfg.GetUIScaleList();
            MainCfg_UIScale_ComboBox.DisplayMember = "Display";
            MainCfg_UIScale_ComboBox.ValueMember = "Value";
            MainCfg_UIScale_ComboBox.SelectedValue = Global.MainCfgUIScale;

            if (list[34] != "") Global.SongMaintenanceEnableMultiSongPath = list[34]; // 已無用處
            SongMaintenance_EnableMultiSongPath_CheckBox.Checked = bool.Parse(Global.SongMaintenanceEnableMultiSongPath);
            SongMaintenance_MultiSongPath_ListBox.Enabled = bool.Parse(Global.SongMaintenanceEnableMultiSongPath);
            SongMaintenance_MultiSongPath_Button.Enabled = bool.Parse(Global.SongMaintenanceEnableMultiSongPath);

            if (list[35] != "") Global.SongMaintenanceMultiSongPathList = new List<string>(list[35].Split(',')); // 已無用處
            SongMaintenance_MultiSongPath_ListBox.DataSource = SongMaintenance.GetMultiSongPathList();
            SongMaintenance_MultiSongPath_ListBox.DisplayMember = "Display";
            SongMaintenance_MultiSongPath_ListBox.ValueMember = "Value";

            if (list[36] != "") Global.SongAddUseCustomSongID = list[36];
            SongAdd_UseCustomSongID_CheckBox.Checked = bool.Parse(Global.SongAddUseCustomSongID);

            if (list[37] != "") Global.MainCfgUIFont = list[37];
            MainCfg_UIFont_ComboBox.DataSource = MainCfg.GetUIFontList();
            MainCfg_UIFont_ComboBox.DisplayMember = "Display";
            MainCfg_UIFont_ComboBox.ValueMember = "Value";
            MainCfg_UIFont_ComboBox.Text = Global.MainCfgUIFont;

            if (list[38] != "") Global.MainCfgEnableUIScale = list[38];
            MainCfg_EnableUIScale_CheckBox.Checked = bool.Parse(Global.MainCfgEnableUIScale);

            if (list[39] != "") Global.SongMgrCustomSingerTypeStructure = list[39];
            SongMgrCfg_SetCustomSingerTypeStructureCbox();

            if (list[40] != "") Global.MainCfgEnableAutoUpdate = list[40];
            MainCfg_EnableAutoUpdate_CheckBox.Checked = bool.Parse(Global.MainCfgEnableAutoUpdate);

            if (list[41] != "") Global.SongAddEnableConvToTC = list[41];
            SongAdd_EnableConvToTC_CheckBox.Checked = bool.Parse(Global.SongAddEnableConvToTC);

            if (list[42] != "") Global.SongMgrEnableMonitorFolders = list[42];
            SongMgrCfg_MonitorFolders_CheckBox.Checked = bool.Parse(Global.SongMgrEnableMonitorFolders);

            if (list[43] != "") Global.SongMgrMonitorFoldersList = new List<string>(list[43].Split(','));

            if (list[44] != "") Global.SingerMgrSyncSongSinger = list[44];
            SingerMgr_EditSyncSongSinger_CheckBox.Checked = bool.Parse(Global.SingerMgrSyncSongSinger);

            if (list[45] != "") Global.SongMgrSingerGroup = list[45];
            SongMgrCfg_SingerGroup_ListBox.DataSource = SongMgrCfg.GetSingerGroupList();
            SongMgrCfg_SingerGroup_ListBox.DisplayMember = "Display";
            SongMgrCfg_SingerGroup_ListBox.ValueMember = "Value";

            if (list[46] != "") Global.MainCfgHideApplyCashboxIdButton = list[46];
            MainCfg_HideApplyCashboxIdButton_CheckBox.Checked = bool.Parse(Global.MainCfgHideApplyCashboxIdButton);

            if (list[3] != "") Global.SongMgrSongAddMode = list[3];
            SongMgrCfg_SongAddMode_ComboBox.DataSource = SongMgrCfg.GetSongAddModeList();
            SongMgrCfg_SongAddMode_ComboBox.DisplayMember = "Display";
            SongMgrCfg_SongAddMode_ComboBox.ValueMember = "Value";
            SongMgrCfg_SongAddMode_ComboBox.SelectedValue = int.Parse(Global.SongMgrSongAddMode);

            // 建立歌曲操作記錄資料表
            Global.SongLogDT = new DataTable();
            Global.SongLogDT.Columns.Add(new DataColumn("Display", typeof(string)));
            Global.SongLogDT.Columns.Add(new DataColumn("Value", typeof(int)));

            // 檢查程式更新
            Common_CheckSongMgrVer();

            // 檢查資料庫檔案是否為舊版資料庫
            SongDBUpdate_CheckDatabaseFile();

            // 初始化所需資料
            Common_InitializeSongData(true, true, true, true, true);

            // 歌庫監視
            SongMgrCfg_MonitorFolders_CheckBox.Enabled = false;
            foreach (string MonitorFolder in Global.SongMgrMonitorFoldersList)
            {
                if (MonitorFolder != "") SongMgrCfg_MonitorFolders_CheckBox.Enabled = true;
            }
            if (!SongMgrCfg_MonitorFolders_CheckBox.Enabled) SongMgrCfg_MonitorFolders_CheckBox.Checked = false;
            SongMonitor_CheckCurSong();

            // 歌曲查詢 - 載入下拉選單清單及設定
            SongQuery_QueryType_ComboBox.DataSource = SongQuery.GetSongQueryTypeList();
            SongQuery_QueryType_ComboBox.DisplayMember = "Display";
            SongQuery_QueryType_ComboBox.ValueMember = "Value";
            SongQuery_QueryType_ComboBox.SelectedValue = 1;

            SongQuery_QueryFilter_ComboBox.DataSource = SongQuery.GetSongQueryFilterList(Global.CrazyktvSongLangList);
            SongQuery_QueryFilter_ComboBox.DisplayMember = "Display";
            SongQuery_QueryFilter_ComboBox.ValueMember = "Value";
            SongQuery_QueryFilter_ComboBox.SelectedValue = 1;

            SongQuery_FuzzyQuery_CheckBox.Checked = bool.Parse(Global.SongQueryFuzzyQuery);
            SongQuery_SynonymousQuery_CheckBox.Checked = Global.SongQuerySynonymousQuery;

            SongQuery_ExceptionalQuery_ComboBox.DataSource = SongQuery.GetSongQueryExceptionalList();
            SongQuery_ExceptionalQuery_ComboBox.DisplayMember = "Display";
            SongQuery_ExceptionalQuery_ComboBox.ValueMember = "Value";
            SongQuery_ExceptionalQuery_ComboBox.SelectedValue = 1;

            // 歌手管理 - 載入下拉選單清單及設定
            SingerMgr_QueryType_ComboBox.DataSource = SingerMgr.GetSingerTypeList(false);
            SingerMgr_QueryType_ComboBox.DisplayMember = "Display";
            SingerMgr_QueryType_ComboBox.ValueMember = "Value";
            SingerMgr_QueryType_ComboBox.SelectedValue = 1;
            SingerMgr_QueryValue_TextBox.ImeMode = ImeMode.OnHalf;

            SingerMgr_SingerAddType_ComboBox.DataSource = SingerMgr.GetSingerTypeList(false);
            SingerMgr_SingerAddType_ComboBox.DisplayMember = "Display";
            SingerMgr_SingerAddType_ComboBox.ValueMember = "Value";
            SingerMgr_SingerAddType_ComboBox.SelectedValue = 1;
            SingerMgr_SingerAddName_TextBox.ImeMode = ImeMode.OnHalf;

            SingerMgr_SingerLastName_ComboBox.DataSource = SingerMgr.GetSingerLastNameList();
            SingerMgr_SingerLastName_ComboBox.DisplayMember = "Display";
            SingerMgr_SingerLastName_ComboBox.ValueMember = "Value";
            SingerMgr_SingerLastName_ComboBox.SelectedValue = 1;

            // 錢櫃資料 - 載入下拉選單清單及設定
            Cashbox_QueryType_ComboBox.DataSource = Cashbox.GetQueryTypeList();
            Cashbox_QueryType_ComboBox.DisplayMember = "Display";
            Cashbox_QueryType_ComboBox.ValueMember = "Value";
            Cashbox_QueryType_ComboBox.SelectedValue = 1;

            Cashbox_QueryFilter_ComboBox.DataSource = Cashbox.GetQueryFilterList(Global.CashboxSongLangList);
            Cashbox_QueryFilter_ComboBox.DisplayMember = "Display";
            Cashbox_QueryFilter_ComboBox.ValueMember = "Value";
            Cashbox_QueryFilter_ComboBox.SelectedValue = 1;

            Cashbox_OtherQuery_ComboBox.DataSource = Cashbox.GetOtherQueryList();
            Cashbox_OtherQuery_ComboBox.DisplayMember = "Display";
            Cashbox_OtherQuery_ComboBox.ValueMember = "Value";
            Cashbox_OtherQuery_ComboBox.SelectedValue = 1;

            Cashbox_DateQuery_ComboBox.DataSource = Cashbox.GetDateQueryList();
            Cashbox_DateQuery_ComboBox.DisplayMember = "Display";
            Cashbox_DateQuery_ComboBox.ValueMember = "Value";
            Cashbox_DateQuery_ComboBox.SelectedValue = 1;

            Cashbox_FuzzyQuery_CheckBox.Checked = Global.CashboxFuzzyQuery;
            Cashbox_SynonymousQuery_CheckBox.Checked = Global.CashboxSynonymousQuery;

            // 歌庫轉換 - 載入下拉選單清單
            SongDBConverter_SrcDBType_ComboBox.DataSource = SongDBConverter.GetSrcDBTypeList();
            SongDBConverter_SrcDBType_ComboBox.DisplayMember = "Display";
            SongDBConverter_SrcDBType_ComboBox.ValueMember = "Value";
            SongDBConverter_SrcDBType_ComboBox.SelectedValue = 1;

            // 歌庫轉換 - 載入說明
            SongDBConverter_SetRtfText("來源資料庫: ", "請選擇你要轉換的點歌軟體歌曲資料庫檔案。" + Environment.NewLine);
            SongDBConverter_SetRtfText("資料庫類型: ", "請選擇你要轉換的點歌軟體資料庫類型。" + Environment.NewLine);
            SongDBConverter_SetRtfText("目的資料庫: ", "請選擇本工具所附的 CrazySongEmpty.mdb 空白資料庫檔案。" + Environment.NewLine);

            // Debug - 載入下拉選單清單
            #if DEBUG
                Debug_ConfigData_ComboBox.DataSource = Debug.GetConfigDataList();
                Debug_ConfigData_ComboBox.DisplayMember = "Display";
                Debug_ConfigData_ComboBox.ValueMember = "Value";
                Debug_ConfigData_ComboBox.SelectedValue = 1;
            #endif

            if (!Global.CrazyktvDatabaseStatus)
            {
                if (MainTabControl.SelectedIndex != 4) MainTabControl.SelectedIndex = 3;
            }
            else
            {
                MainTabControl_SelectedIndexChanged(new TabControl(), new EventArgs());
            }

            if (Global.MainCfgEnableUIScale == "True") Common_ScalingUI();

            Global.SongMgrInitializeStatus = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (File.Exists(Global.CrazyktvDatabaseFile))
            {
                try
                {
                    CommonFunc.CompactAccessDB("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Global.CrazyktvDatabaseFile + ";", Global.CrazyktvDatabaseFile);
                    CommonFunc.CompactAccessDB("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Global.CrazyktvSongMgrDatabaseFile + ";", Global.CrazyktvSongMgrDatabaseFile);
                }
                catch { }
            }
        }

        // 頁籤切換處理
        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label[] Tooltip_Label =
            {
                SongQuery_QueryStatus_Label,
                SongAdd_Tooltip_Label,
                SingerMgr_Tooltip_Label,
                SongMgrCfg_Tooltip_Label,
                SongMaintenance_Tooltip_Label,
                Cashbox_QueryStatus_Label
            };

            int i = -1;

            switch (MainTabControl.SelectedTab.Name)
            {
                case "SongQuery_TabPage":
                    i = 0;
                    SongQuery_QueryValue_TextBox.Focus();
                    SongQuery_QueryValue_TextBox.ImeMode = ImeMode.OnHalf;
                    break;
                case "SongAdd_TabPage":
                    i = 1;
                    break;
                case "SingerMgr_TabPage":
                    i = 2;
                    break;
                case "SongMgrCfg_TabPage":
                    i = 3;
                    break;
                case "SongMaintenance_TabPage":
                    i = 4;
                    switch (SongMaintenance_TabControl.SelectedTab.Name)
                    {
                        case "SongMaintenance_CustomLang_TabPage":
                        case "SongMaintenance_MultiSongPath_TabPage":
                        case "SongMaintenance_DBVer_TabPage":
                            if (SongMaintenance_TabControl.Enabled == true) SongMaintenance_Save_Button.Enabled = true;
                            break;
                        default:
                            SongMaintenance_Save_Button.Enabled = false;
                            break;
                    }
                    break;
                case "Cashbox_TabPage":
                    i = 5;
                    Cashbox_QueryValue_TextBox.Focus();
                    Cashbox_QueryValue_TextBox.ImeMode = ImeMode.OnHalf;
                    Cashbox_UpdDate_Button.Enabled = Cashbox.GetUpdDateButtonEnableStatus();
                    break;
            }

            if (i >= 0)
            {
                if (!Global.CrazyktvDatabaseStatus)
                {
                    if (!File.Exists(Global.CrazyktvDatabaseFile)) Tooltip_Label[i].Text = "CrazyKTV 資料庫檔案不存在!";
                    else if (!File.Exists(Global.CrazyktvSongMgrDatabaseFile)) Tooltip_Label[i].Text = "加歌程式參考資料庫檔案不存在!";
                    else if (Global.CrazyktvDatabaseError) Tooltip_Label[i].Text = "資料庫檔案不是 CrazyKTV 資料庫!";
                    else if (Global.SongMgrDatabaseError) Tooltip_Label[i].Text = "加歌程式參考資料庫不是最新版本!";
                    else if (Global.CrazyktvDatabaseIsOld) Tooltip_Label[i].Text = "資料庫檔案為舊版本!";
                    else if (!Global.CrazyktvDatabaseMaxDigitCode) Tooltip_Label[i].Text = "歌庫編碼混雜 5 及 6 位數編碼!";
                    else if (!Directory.Exists(Global.SongMgrDestFolder)) Tooltip_Label[i].Text = "請先設定歌庫資料夾!";
                }
                else
                {
                    List<string> ErrorTextList = new List<string>()
                    {
                        "CrazyKTV 資料庫檔案不存在!",
                        "加歌程式參考資料庫檔案不存在!",
                        "資料庫檔案不是 CrazyKTV 資料庫!",
                        "加歌程式參考資料庫不是最新版本!",
                        "資料庫檔案為舊版本!",
                        "歌庫編碼混雜 5 及 6 位數編碼!",
                        "請先設定歌庫資料夾!"
                    };
                    if (ErrorTextList.IndexOf(Tooltip_Label[i].Text) >= 0) Tooltip_Label[i].Text = "";
                    ErrorTextList.Clear();
                }
            }
        }

        private void MainForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Global.PlayerUpdateSongValueList.Count > 0 )
            {
                int i = Convert.ToInt32(Global.PlayerUpdateSongValueList[1]);
                switch (Global.PlayerUpdateSongValueList[0])
                {
                    case "SongQuery":
                        string OldSongId = SongQuery_DataGridView.Rows[i].Cells["Song_Id"].Value.ToString();
                        string SongId = SongQuery_DataGridView.Rows[i].Cells["Song_Id"].Value.ToString();
                        string SongLang = SongQuery_DataGridView.Rows[i].Cells["Song_Lang"].Value.ToString();
                        string SongSingerType = SongQuery_DataGridView.Rows[i].Cells["Song_SingerType"].Value.ToString();
                        string SongSinger = SongQuery_DataGridView.Rows[i].Cells["Song_Singer"].Value.ToString();
                        string SongSongName = SongQuery_DataGridView.Rows[i].Cells["Song_SongName"].Value.ToString();
                        string SongTrack = Global.PlayerUpdateSongValueList[2];
                        string SongSongType = SongQuery_DataGridView.Rows[i].Cells["Song_SongType"].Value.ToString();
                        string SongVolume = SongQuery_DataGridView.Rows[i].Cells["Song_Volume"].Value.ToString();
                        string SongWordCount = SongQuery_DataGridView.Rows[i].Cells["Song_WordCount"].Value.ToString();
                        string SongPlayCount = SongQuery_DataGridView.Rows[i].Cells["Song_PlayCount"].Value.ToString();
                        string SongMB = SongQuery_DataGridView.Rows[i].Cells["Song_MB"].Value.ToString();
                        string SongCreatDate = SongQuery_DataGridView.Rows[i].Cells["Song_CreatDate"].Value.ToString();
                        string SongFileName = SongQuery_DataGridView.Rows[i].Cells["Song_FileName"].Value.ToString();
                        string SongPath = SongQuery_DataGridView.Rows[i].Cells["Song_Path"].Value.ToString();
                        string SongSpell = SongQuery_DataGridView.Rows[i].Cells["Song_Spell"].Value.ToString();
                        string SongSpellNum = SongQuery_DataGridView.Rows[i].Cells["Song_SpellNum"].Value.ToString();
                        string SongSongStroke = SongQuery_DataGridView.Rows[i].Cells["Song_SongStroke"].Value.ToString();
                        string SongPenStyle = SongQuery_DataGridView.Rows[i].Cells["Song_PenStyle"].Value.ToString();
                        string SongPlayState = SongQuery_DataGridView.Rows[i].Cells["Song_PlayState"].Value.ToString();

                        List<string> UpdateList = new List<string>();
                        UpdateList.Add(SongId + "|" + SongLang + "|" + SongSingerType + "|" + SongSinger + "|" + SongSongName + "|" + SongTrack + "|" + SongSongType + "|" + SongVolume + "|" + SongWordCount + "|" + SongPlayCount + "|" + SongMB + "|" + SongCreatDate + "|" + SongFileName + "|" + SongPath + "|" + SongSpell + "|" + SongSpellNum + "|" + SongSongStroke + "|" + SongPenStyle + "|" + SongPlayState + "|" + OldSongId);

                        SongQuery.CreateSongDataTable();
                        Common_SwitchSetUI(false);

                        Global.TotalList = new List<int>() { 0, 0, 0, 0 };
                        Global.SongQueryDataGridViewRestoreSelectList = new List<string>();
                        Global.SongQueryDataGridViewRestoreCurrentRow = OldSongId;
                        SongQuery_DataGridView.Sorted -= new EventHandler(SongQuery_DataGridView_Sorted);
                        SongQuery_QueryStatus_Label.Text = "正在更新音軌資料,請稍待...";

                        using (DataTable UpdateDT = (DataTable)SongQuery_DataGridView.DataSource)
                        {
                            var tasks = new List<Task>();
                            tasks.Add(Task.Factory.StartNew(() => SongQuery_SongUpdate(UpdateList, UpdateDT)));

                            Task.Factory.ContinueWhenAll(tasks.ToArray(), EndTask =>
                            {
                                this.BeginInvoke((Action)delegate()
                                {
                                    SongQuery_DataGridView.Sorted += new EventHandler(SongQuery_DataGridView_Sorted);
                                    SongQuery_DataGridView_Sorted(new object(), new EventArgs());
                                    SongQuery_QueryStatus_Label.Text = "已成功更新 " + Global.TotalList[0] + " 首歌曲的音軌資料。";
                                    Common_SwitchSetUI(true);
                                });
                                UpdateList.Clear();
                                SongQuery.DisposeSongDataTable();
                            });
                        }
                        break;
                    case "SongQueryEdit":
                        SongQuery_EditSongTrack_ComboBox.SelectedValue = Global.PlayerUpdateSongValueList[2];
                        break;
                    case "SongAdd":
                        SongAdd_EditSongTrack_ComboBox.SelectedValue = Global.PlayerUpdateSongValueList[2];
                        break;
                }
                Global.PlayerUpdateSongValueList.Clear();
            }
        }




























































































































        // End
    }
}
