using PKHeX.Core;
using SysBot.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace SysBot.Pokemon;

public class TradeSettings : IBotStateSettings, ICountSettings
{
    private const string CountStats = nameof(CountStats);
    private const string HOMELegality = nameof(HOMELegality);
    private const string TradeConfig = nameof(TradeConfig);
    private const string AutoCorrectShowdownConfig = nameof(AutoCorrectShowdownConfig);
    private const string VGCPastesConfig = nameof(VGCPastesConfig);
    private const string Miscellaneous = nameof(Miscellaneous);
    private const string RequestFolders = nameof(RequestFolders);
    private const string EmbedSettings = nameof(EmbedSettings);
    public override string ToString() => "Trade Configuration Settings";

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class EmojiInfo
    {
        [Description("The full string for the emoji.")]
        public string EmojiString { get; set; } = string.Empty;

        public override string ToString()
        {
            return string.IsNullOrEmpty(EmojiString) ? "Not Set" : EmojiString;
        }
    }

    [Category(TradeConfig), Description("Settings related to Trade Configuration."), DisplayName("Trade Configuration"), Browsable(true)]
    public TradeSettingsCategory TradeConfiguration { get; set; } = new();

    [Category(VGCPastesConfig), Description("Settings related to VGCPastes Configuration."), DisplayName("VGC Pastes Configuration"), Browsable(true)]
    public VGCPastesCategory VGCPastesConfiguration { get; set; } = new();

    [Category(AutoCorrectShowdownConfig), Description("Settings related to Auto Correcting Showdown Sets."), DisplayName("Auto Correct Showdown Settings"), Browsable(true)]
    public AutoCorrectShowdownCategory AutoCorrectConfig { get; set; } = new();

    [Category(EmbedSettings), Description("Settings related to the Trade Embed in Discord."), DisplayName("Trade Embed Settings"), Browsable(true)]
    public TradeEmbedSettingsCategory TradeEmbedSettings { get; set; } = new();

    [Category(RequestFolders), Description("Settings related to Request Folders."), DisplayName("Request Folder Settings"), Browsable(true)]
    public RequestFolderSettingsCategory RequestFolderSettings { get; set; } = new();

    [Category(CountStats), Description("Settings related to Trade Count Statistics."), DisplayName("Trade Count Statistics Settings"), Browsable(true)]
    public CountStatsSettingsCategory CountStatsSettings { get; set; } = new();


    [Category(TradeConfig), TypeConverter(typeof(CategoryConverter<TradeSettingsCategory>))]
    public class TradeSettingsCategory
    {
        public override string ToString() => "Trade Configuration Settings";

        [Category(TradeConfig), Description("Minimum Link Code."), DisplayName("Minimum Trade Link Code")]
        public int MinTradeCode { get; set; } = 0;

        [Category(TradeConfig), Description("Maximum Link Code."), DisplayName("Maximum Trade Link Code")]
        public int MaxTradeCode { get; set; } = 9999_9999;

        [Category(TradeConfig), Description("If set to True, Discord Users trade code will be stored and used repeatedly without changing."), DisplayName("Store and Reuse Trade Codes")]
        public bool StoreTradeCodes { get; set; } = true;

        [Category(TradeConfig), Description("Time to wait for a trade partner in seconds."), DisplayName("Trade Partner Wait Time (seconds)")]
        public int TradeWaitTime { get; set; } = 30;

        [Category(TradeConfig), Description("Max amount of time in seconds pressing A to wait for a trade to process."), DisplayName("Maximum Trade Confirmation Time (seconds)")]
        public int MaxTradeConfirmTime { get; set; } = 25;

        [Category(TradeConfig), Description("Select default species for \"ItemTrade\", if configured."), DisplayName("Default Species for Item Trades")]
        public Species ItemTradeSpecies { get; set; } = Species.None;

        [Category(TradeConfig), Description("Default held item to send if none is specified."), DisplayName("Default Held Item for Trades")]
        public HeldItem DefaultHeldItem { get; set; } = HeldItem.None;

        [Category(TradeConfig), Description("If set to True, each valid Pokemon will come with all suggested Relearnable Moves without the need for a batch command."), DisplayName("Suggest Relearnable Moves by Default")]
        public bool SuggestRelearnMoves { get; set; } = true;

        [Category(TradeConfig), Description("Toggle to allow or disallow batch trades."), DisplayName("Allow Batch Trades")]
        public bool AllowBatchTrades { get; set; } = true;

        [Category(TradeConfig), Description("Maximum pokemons of single trade. Batch mode will be closed if this configuration is less than 1"), DisplayName("Maximum Pokémon per Trade")]
        public int MaxPkmsPerTrade { get; set; } = 1;

        [Category(TradeConfig), Description("Dump Trade: Dumping routine will stop after a maximum number of dumps from a single user."), DisplayName("Maximum Dumps per Trade")]
        public int MaxDumpsPerTrade { get; set; } = 20;

        [Category(TradeConfig), Description("Dump Trade: Dumping routine will stop after spending x seconds in trade."), DisplayName("Maximum Dump Trade Time (seconds)")]
        public int MaxDumpTradeTime { get; set; } = 180;

        [Category(TradeConfig), Description("Toggle to give users the option to use the BatchNormalizer, which utilizes Showdown in place of batch commands. See https://shorter.me/62oaO for details."), DisplayName("Batch Commands to Showdown")]
        public bool BatchNormalizer { get; set; } = true;

        [Category(TradeConfig), Description("Dump Trade: If enabled, Dumping routine will output legality check information to the user."), DisplayName("Dump Trade Legality Check")]
        public bool DumpTradeLegalityCheck { get; set; } = true;
        public bool EnableSpamCheck { get; set; }

        [Category(TradeConfig), Description("LGPE Setting.")]
        public int TradeAnimationMaxDelaySeconds = 25;

        public enum HeldItem
        {
            None = 0,
            AbilityPatch = 1606,
            RareCandy = 50,
            AbilityCapsule = 645,
            BottleCap = 795,
            expCandyL = 1127,
            expCandyXL = 1128,
            MasterBall = 1,
            Nugget = 92,
            BigPearl = 89,
            GoldBottleCap = 796,
            ppUp = 51,
            ppMax = 53,
            FreshStartMochi = 2479,
        }
    }

    [Category(nameof(AutoCorrectShowdownConfig)), TypeConverter(typeof(CategoryConverter<AutoCorrectShowdownCategory>))]
    public class AutoCorrectShowdownCategory
    {
        public override string ToString() => "Auto Correct Showdown Settings";

        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, each failed showdown set will go through auto correction."), DisplayName("Enable Auto Correct")]
        public bool EnableAutoCorrect { get; set; } = true;

        private bool _autoCorrectEmbedIndicator = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, we will put an indicator on Trade Embeds showing a trade was Auto Corrected."), DisplayName("Show Trade Embed Indicator?")]
        public bool AutoCorrectEmbedIndicator
        {
            get => EnableAutoCorrect && _autoCorrectEmbedIndicator;
            set => _autoCorrectEmbedIndicator = value;
        }

        private bool _autoCorrectNickname = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct illegal nicknames."), DisplayName("Auto Correct Nicknames?")]
        public bool AutoCorrectNickname
        {
            get => EnableAutoCorrect && _autoCorrectNickname;
            set => _autoCorrectNickname = value;
        }

        private string _fixedNickname = string.Empty;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("Set a default Nickname. If none provided, it will just be blank."), DisplayName("Rename Invalid Nicknames to...")]
        public string FixedNickname
        {
            get => EnableAutoCorrect ? _fixedNickname : string.Empty;
            set => _fixedNickname = value;
        }

        private bool _autoCorrectSpeciesAndForm = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct wrong species and form."), DisplayName("Auto Correct Species and Form")]
        public bool AutoCorrectSpeciesAndForm
        {
            get => EnableAutoCorrect && _autoCorrectSpeciesAndForm;
            set => _autoCorrectSpeciesAndForm = value;
        }

        private bool _autoCorrectHeldItem = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct wrong held item."), DisplayName("Auto Correct Held Item")]
        public bool AutoCorrectHeldItem
        {
            get => EnableAutoCorrect && _autoCorrectHeldItem;
            set => _autoCorrectHeldItem = value;
        }

        private bool _autoCorrectNature = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct wrong nature."), DisplayName("Auto Correct Nature")]
        public bool AutoCorrectNature
        {
            get => EnableAutoCorrect && _autoCorrectNature;
            set => _autoCorrectNature = value;
        }

        private bool _autoCorrectAbility = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct wrong ability."), DisplayName("Auto Correct Ability")]
        public bool AutoCorrectAbility
        {
            get => EnableAutoCorrect && _autoCorrectAbility;
            set => _autoCorrectAbility = value;
        }

        private bool _autoCorrectBall = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct wrong Ball Name."), DisplayName("Auto Correct Ball")]
        public bool AutoCorrectBall
        {
            get => EnableAutoCorrect && _autoCorrectBall;
            set => _autoCorrectBall = value;
        }

        private bool _autoCorrectLevel = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct wrong level."), DisplayName("Auto Correct Level")]
        public bool AutoCorrectLevel
        {
            get => EnableAutoCorrect && _autoCorrectLevel;
            set => _autoCorrectLevel = value;
        }

        private bool _autoCorrectGender = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct wrong gender."), DisplayName("Auto Correct Gender")]
        public bool AutoCorrectGender
        {
            get => EnableAutoCorrect && _autoCorrectGender;
            set => _autoCorrectGender = value;
        }

        private bool _autoCorrectMovesLearnset = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct wrong moves and learnset."), DisplayName("Auto Correct Moves/Learnset")]
        public bool AutoCorrectMovesLearnset
        {
            get => EnableAutoCorrect && _autoCorrectMovesLearnset;
            set => _autoCorrectMovesLearnset = value;
        }

        private bool _autoCorrectEVs = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct wrong EVs."), DisplayName("Auto Correct EVs")]
        public bool AutoCorrectEVs
        {
            get => EnableAutoCorrect && _autoCorrectEVs;
            set => _autoCorrectEVs = value;
        }

        private bool _autoCorrectIVs = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct wrong IVs."), DisplayName("Auto Correct IVs")]
        public bool AutoCorrectIVs
        {
            get => EnableAutoCorrect && _autoCorrectIVs;
            set => _autoCorrectIVs = value;
        }
        private bool _autoCorrectMarks = true;
        [Category(nameof(AutoCorrectShowdownCategory)), Description("If set to True, auto correction will correct wrong Marks/Ribbons."), DisplayName("Auto Correct Marks/Ribbons")]
        public bool AutoCorrectMarks
        {
            get => EnableAutoCorrect && _autoCorrectMarks;
            set => _autoCorrectMarks = value;
        }
    }

    [Category(EmbedSettings), TypeConverter(typeof(CategoryConverter<TradeEmbedSettingsCategory>))]
    public class TradeEmbedSettingsCategory
    {
        public override string ToString() => "Trade Embed Configuration Settings";

        private bool _useEmbeds = true;
        [Category(EmbedSettings), Description("If true, will show beautiful embeds in your discord trade channels of what the user is trading. False will show default text."), DisplayName("Use Embeds")]
        public bool UseEmbeds
        {
            get => _useEmbeds;
            set
            {
                _useEmbeds = value;
                OnUseEmbedsChanged();
            }
        }

        private void OnUseEmbedsChanged()
        {
            if (!_useEmbeds)
            {
                PreferredImageSize = ImageSize.Size256x256;
                MoveTypeEmojis = false;
                ShowScale = false;
                ShowTeraType = false;
                ShowLevel = false;
                ShowMetDate = false;
                ShowAbility = false;
                ShowNature = false;
                ShowIVs = false;
            }
        }

        [Category(EmbedSettings), Description("Preferred Species Image Size for Embeds."), DisplayName("Species Image Size")]
        public ImageSize PreferredImageSize { get; set; } = ImageSize.Size256x256;

        [Category(EmbedSettings), Description("Will show Move Type Icons next to moves in trade embed (Discord only). Requires user to upload the emojis to their server."), DisplayName("Show Move Type Emojis")]
        public bool MoveTypeEmojis { get; set; } = true;

        [Category(EmbedSettings), Description("Custom Emoji information for the move types."), DisplayName("Custom Type Emojis")]
        public List<MoveTypeEmojiInfo> CustomTypeEmojis { get; set; } = new List<MoveTypeEmojiInfo>
    {
        new(MoveType.Bug),
        new(MoveType.Fire),
        new(MoveType.Flying),
        new(MoveType.Ground),
        new(MoveType.Water),
        new(MoveType.Grass),
        new(MoveType.Ice),
        new(MoveType.Rock),
        new(MoveType.Ghost),
        new(MoveType.Steel),
        new(MoveType.Fighting),
        new(MoveType.Electric),
        new(MoveType.Dragon),
        new(MoveType.Psychic),
        new(MoveType.Dark),
        new(MoveType.Normal),
        new(MoveType.Poison),
        new(MoveType.Fairy),
        new(MoveType.Stellar)
    };

        [Category(EmbedSettings), Description("The full string for the male gender emoji."), DisplayName("Male Emoji")]
        public EmojiInfo MaleEmoji { get; set; } = new EmojiInfo();

        [Category(EmbedSettings), Description("The full string for the female gender emoji."), DisplayName("Female Emoji")]
        public EmojiInfo FemaleEmoji { get; set; } = new EmojiInfo();

        [Category(EmbedSettings), Description("The emoji information for displaying mystery gift status."), DisplayName("Mystery Gift Emoji")]
        public EmojiInfo MysteryGiftEmoji { get; set; } = new EmojiInfo();

        [Category(EmbedSettings), Description("The emoji information for displaying the alpha mark."), DisplayName("Alpha Mark Emoji")]
        public EmojiInfo AlphaMarkEmoji { get; set; } = new EmojiInfo();

        [Category(EmbedSettings), Description("The emoji information for displaying the mightiest mark."), DisplayName("Mightiest Mark Emoji")]
        public EmojiInfo MightiestMarkEmoji { get; set; } = new EmojiInfo();

        [Category(EmbedSettings), Description("The emoji information for displaying the alpha emoji in Legends: Arceus."), DisplayName("Alpha PLA Emoji")]
        public EmojiInfo AlphaPLAEmoji { get; set; } = new EmojiInfo();

        [Category(EmbedSettings), Description("Will show Move Type Icons next to moves in trade embed (Discord only). Requires user to upload the emojis to their server."), DisplayName("Show Tera Type Emojis?")]
        public bool UseTeraEmojis { get; set; } = true;

        [Category(EmbedSettings), Description("Tera Type Emoji information for the tera types."), DisplayName("Custom Tera Type Emojis")]
        public List<TeraTypeEmojiInfo> TeraTypeEmojis { get; set; } = new List<TeraTypeEmojiInfo>
    {
        new(MoveType.Bug),
        new(MoveType.Fire),
        new(MoveType.Flying),
        new(MoveType.Ground),
        new(MoveType.Water),
        new(MoveType.Grass),
        new(MoveType.Ice),
        new(MoveType.Rock),
        new(MoveType.Ghost),
        new(MoveType.Steel),
        new(MoveType.Fighting),
        new(MoveType.Electric),
        new(MoveType.Dragon),
        new(MoveType.Psychic),
        new(MoveType.Dark),
        new(MoveType.Normal),
        new(MoveType.Poison),
        new(MoveType.Fairy),
        new(MoveType.Stellar)
    };

        [Category(EmbedSettings), Description("Will show Level in trade embed (Discord only)."), DisplayName("Show Level")]
        public bool ShowLevel { get; set; } = true;

        [Category(EmbedSettings), Description("Will show Ball in trade embed (Discord only)."), DisplayName("Show Ball")]
        public bool ShowBall { get; set; } = true;

        [Category(EmbedSettings), Description("Will show Met Level in trade embed (Discord only)."), DisplayName("Show Met Level")]
        public bool ShowMetLevel { get; set; } = true;

        [Category(EmbedSettings), Description("Will show MetDate in trade embed (Discord only)."), DisplayName("Show Met Date")]
        public bool ShowMetDate { get; set; } = true;

        [Category(EmbedSettings), Description("Will show MetLocation in trade embed (Discord only)."), DisplayName("Show Met Location")]
        public bool ShowMetLocation { get; set; } = true;

        [Category(EmbedSettings), Description("Will show Ability in trade embed (Discord only)."), DisplayName("Show Ability")]
        public bool ShowAbility { get; set; } = true;

        [Category(EmbedSettings), Description("Will show Nature in trade embed (Discord only)."), DisplayName("Show Nature")]
        public bool ShowNature { get; set; } = true;

        [Category(EmbedSettings), Description("Will show Language in trade embed (Discord only)."), DisplayName("Show Language")]
        public bool ShowLanguage { get; set; } = true;

        [Category(EmbedSettings), Description("Will show IVs in trade embed (Discord only)."), DisplayName("Show IVs")]
        public bool ShowIVs { get; set; } = true;

        [Category(EmbedSettings), Description("Will show EVs in trade embed (Discord only)."), DisplayName("Show EVs")]
        public bool ShowEVs { get; set; } = true;

        [Category(EmbedSettings), Description("Will show Scale in trade embed (SV & Discord only). Requires user to upload the emojis to their server."), DisplayName("Show Scale")]
        public bool ShowScale { get; set; } = true;

        [Category(EmbedSettings), Description("Will show Tera Type in trade embed (SV & Discord only)."), DisplayName("Show Tera Type")]
        public bool ShowTeraType { get; set; } = true;
    }

    [Category(VGCPastesConfig), TypeConverter(typeof(CategoryConverter<VGCPastesCategory>))]
    public class VGCPastesCategory
    {
        public override string ToString() => "VGCPastes Configuration Settings";

        [Category(VGCPastesConfig), Description("Allow users to request and generate teams using the VGCPastes Spreadsheet."), DisplayName("Allow VGC Paste Requests")]
        public bool AllowRequests { get; set; } = true;

        [Category(VGCPastesConfig), Description("GID of Spreadsheet tab you would like to pull from. Hint: https://docs.google.com/spreadsheets/d/ID/gid=1837599752"), DisplayName("GID of Spreadsheet Tab")]
        public int GID { get; set; } = 1837599752; // Reg F Tab
    }

    [Category(RequestFolders), TypeConverter(typeof(CategoryConverter<RequestFolderSettingsCategory>))]
    public class RequestFolderSettingsCategory
    {
        public override string ToString() => "Request Folders Settings";

        [Category("RequestFolders"), Description("Path to your Events Folder. Create a new folder called 'events' and copy the path here."), DisplayName("Events Folder Path")]
        public string EventsFolder { get; set; } = string.Empty;

        [Category("RequestFolders"), Description("Path to your BattleReady Folder. Create a new folder called 'battleready' and copy the path here."), DisplayName("Battle-Ready Folder Path")]
        public string BattleReadyPKMFolder { get; set; } = string.Empty;

        [Category("RequestFolders"), Description("Path to your HOME-Ready Folder. Create a new folder called 'homeready' and copy the path here."), DisplayName("HOME-Ready Folder Path")]
        public string HOMEReadyPKMFolder { get; set; } = string.Empty;
    }

    [Category(Miscellaneous), Description("Miscellaneous Settings"), DisplayName("Miscellaneous")]
    public bool ScreenOff { get; set; } = false;

    /// <summary>
    /// Gets a random trade code based on the range settings.
    /// </summary>
    public int GetRandomTradeCode() => Util.Rand.Next(TradeConfiguration.MinTradeCode, TradeConfiguration.MaxTradeCode + 1);

    public static List<Pictocodes> GetRandomLGTradeCode(bool randomtrade = false)
    {
        var lgcode = new List<Pictocodes>();
        if (randomtrade)
        {
            for (int i = 0; i <= 2; i++)
            {
                // code.Add((pictocodes)Util.Rand.Next(10));
                lgcode.Add(Pictocodes.Pikachu);

            }
        }
        else
        {
            for (int i = 0; i <= 2; i++)
            {
                lgcode.Add((Pictocodes)Util.Rand.Next(10));
                // code.Add(pictocodes.Pikachu);

            }
        }
        return lgcode;
    }


    [Category(CountStats), TypeConverter(typeof(CategoryConverter<CountStatsSettingsCategory>))]
    public class CountStatsSettingsCategory
    {
        public override string ToString() => "Trade Count Statistics";

        private int _completedSurprise;
        private int _completedDistribution;
        private int _completedTrades;
        private int _completedSeedChecks;
        private int _completedClones;
        private int _completedDumps;
        private int _completedFixOTs;

        [Category(CountStats), Description("Completed Surprise Trades")]
        public int CompletedSurprise
        {
            get => _completedSurprise;
            set => _completedSurprise = value;
        }

        [Category(  ), Description("Completed Link Trades (Distribution)")]
        public int CompletedDistribution
        {
            get => _completedDistribution;
            set => _completedDistribution = value;
        }

        [Category(CountStats), Description("Completed Link Trades (Specific User)")]
        public int CompletedTrades
        {
            get => _completedTrades;
            set => _completedTrades = value;
        }

        [Category(CountStats), Description("Completed FixOT Trades (Specific User)")]
        public int CompletedFixOTs
        {
            get => _completedFixOTs;
            set => _completedFixOTs = value;
        }

        [Browsable(false)]
        [Category(CountStats), Description("Completed Seed Check Trades")]
        public int CompletedSeedChecks
        {
            get => _completedSeedChecks;
            set => _completedSeedChecks = value;
        }

        [Category(CountStats), Description("Completed Clone Trades (Specific User)")]
        public int CompletedClones
        {
            get => _completedClones;
            set => _completedClones = value;
        }

        [Category(CountStats), Description("Completed Dump Trades (Specific User)")]
        public int CompletedDumps
        {
            get => _completedDumps;
            set => _completedDumps = value;
        }

        [Category(CountStats), Description("When enabled, the counts will be emitted when a status check is requested.")]
        public bool EmitCountsOnStatusCheck { get; set; }

        public void AddCompletedTrade() => Interlocked.Increment(ref _completedTrades);
        public void AddCompletedSeedCheck() => Interlocked.Increment(ref _completedSeedChecks);
        public void AddCompletedSurprise() => Interlocked.Increment(ref _completedSurprise);
        public void AddCompletedDistribution() => Interlocked.Increment(ref _completedDistribution);
        public void AddCompletedDumps() => Interlocked.Increment(ref _completedDumps);
        public void AddCompletedClones() => Interlocked.Increment(ref _completedClones);
        public void AddCompletedFixOTs() => Interlocked.Increment(ref _completedFixOTs);

        public IEnumerable<string> GetNonZeroCounts()
        {
            if (!EmitCountsOnStatusCheck)
                yield break;
            if (CompletedSeedChecks != 0)
                yield return $"Seed Check Trades: {CompletedSeedChecks}";
            if (CompletedClones != 0)
                yield return $"Clone Trades: {CompletedClones}";
            if (CompletedDumps != 0)
                yield return $"Dump Trades: {CompletedDumps}";
            if (CompletedTrades != 0)
                yield return $"Link Trades: {CompletedTrades}";
            if (CompletedDistribution != 0)
                yield return $"Distribution Trades: {CompletedDistribution}";
            if (CompletedFixOTs != 0)
                yield return $"FixOT Trades: {CompletedFixOTs}";
            if (CompletedSurprise != 0)
                yield return $"Surprise Trades: {CompletedSurprise}";
        }
    }

    public bool EmitCountsOnStatusCheck
    {
        get => CountStatsSettings.EmitCountsOnStatusCheck;
        set => CountStatsSettings.EmitCountsOnStatusCheck = value;
    }

    public IEnumerable<string> GetNonZeroCounts()
    {
        // Delegating the call to CountStatsSettingsCategory
        return CountStatsSettings.GetNonZeroCounts();
    }

    public class CategoryConverter<T> : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => true;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes) => TypeDescriptor.GetProperties(typeof(T));

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) => destinationType != typeof(string) && base.CanConvertTo(context, destinationType);
    }

    public enum ImageSize
    {
        Size256x256,
        Size128x128
    }

    public enum MoveType
    {
        Normal,
        Fighting,
        Flying,
        Poison,
        Ground,
        Rock,
        Bug,
        Ghost,
        Steel,
        Fire,
        Water,
        Grass,
        Electric,
        Psychic,
        Ice,
        Dragon,
        Dark,
        Fairy,
        Stellar
    }

    public class MoveTypeEmojiInfo
    {
        [Description("The type of move.")]
        public MoveType MoveType { get; set; }

        [Description("The Discord emoji string for this move type.")]
        public string? EmojiCode { get; set; } = string.Empty;

        public MoveTypeEmojiInfo() { }

        public MoveTypeEmojiInfo(MoveType moveType)
        {
            MoveType = moveType;
            EmojiCode = string.Empty;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(EmojiCode))
                return MoveType.ToString();

            return $"{EmojiCode}";
        }
    }

    public class TeraTypeEmojiInfo
    {
        [Description("The Tera Type.")]
        public MoveType MoveType { get; set; }

        [Description("The Discord emoji string for this tera type.")]
        public string EmojiCode { get; set; } = ""; // Initialize EmojiCode to an empty string

        public TeraTypeEmojiInfo() { }

        public TeraTypeEmojiInfo(MoveType teraType)
        {
            MoveType = teraType;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(EmojiCode))
                return MoveType.ToString();

            return $"{EmojiCode}";
        }
    }
}
