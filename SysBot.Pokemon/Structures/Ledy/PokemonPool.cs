using PKHeX.Core;
using SysBot.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SysBot.Pokemon;

public class PokemonPool<T>(BaseConfig Settings) : List<T>
    where T : PKM, new()
{
    private readonly int ExpectedSize = new T().Data.Length;
    private bool Randomized => Settings.Shuffled;

    public readonly Dictionary<string, (LedyRequest<T> Request, string OriginalFileName)> Files = new();
    public readonly Dictionary<string, int> TradeCodes = new();
    private int Counter;

    public T GetRandomPoke()
    {
        var choice = this[Counter];
        Counter = (Counter + 1) % Count;
        if (Counter == 0 && Randomized)
            Shuffle(this, 0, Count, Util.Rand);
        return choice;
    }

    public static void Shuffle(IList<T> items, int start, int end, Random rnd)
    {
        for (int i = start; i < end; i++)
        {
            int index = i + rnd.Next(end - i);
            (items[index], items[i]) = (items[i], items[index]);
        }
    }

    public T GetRandomSurprise()
    {
        while (true)
        {
            var rand = GetRandomPoke();
            if (DisallowRandomRecipientTrade(rand))
                continue;
            return rand;
        }
    }

    public bool Reload(string path, SearchOption opt = SearchOption.AllDirectories)
    {
        if (!Directory.Exists(path))
            return false;
        Clear();
        Files.Clear();
        TradeCodes.Clear();
        return LoadFolder(path, opt);
    }

    public bool LoadFolder(string path, SearchOption opt = SearchOption.AllDirectories)
    {
        if (!Directory.Exists(path))
            return false;

        var loadedAny = false;
        var files = Directory.EnumerateFiles(path, "*", opt);
        var matchFiles = LoadUtil.GetFilesOfSize(files, ExpectedSize);

        int surpriseBlocked = 0;
        foreach (var file in matchFiles)
        {
            var data = File.ReadAllBytes(file);
            var prefer = EntityFileExtension.GetContextFromExtension(file);
            var pkm = EntityFormat.GetFromBytes(data, prefer);
            if (pkm is null)
                continue;
            if (pkm is not T)
                pkm = EntityConverter.ConvertToType(pkm, typeof(T), out _);
            if (pkm is not T dest)
                continue;

            if (dest.Species == 0)
            {
                LogUtil.LogInfo("SKIPPED: Provided file is not valid: " + dest.FileName, nameof(PokemonPool<T>));
                continue;
            }

            if (!dest.CanBeTraded())
            {
                LogUtil.LogInfo("SKIPPED: Provided file cannot be traded: " + dest.FileName, nameof(PokemonPool<T>));
                continue;
            }

            var la = new LegalityAnalysis(dest);
            if (!la.Valid)
            {
                var reason = la.Report();
                LogUtil.LogInfo($"SKIPPED: Provided file is not legal: {dest.FileName} -- {reason}", nameof(PokemonPool<T>));
                continue;
            }

            if (Settings.Legality.ResetHOMETracker && dest is IHomeTrack h)
                h.Tracker = 0;

            var fn = Path.GetFileNameWithoutExtension(file);
            fn = StringsUtil.Sanitize(fn);

            // Extract trade code
            var originalFilename = Path.GetFileNameWithoutExtension(file);
            var tradeCode = ExtractTradeCode(originalFilename);

            // Since file names can be sanitized to the same string, only add one of them.
            if (!Files.ContainsKey(fn))
            {
                Add(dest);
                Files.Add(fn, (new LedyRequest<T>(dest, fn), originalFilename));
                if (tradeCode.HasValue)
                    TradeCodes[originalFilename] = tradeCode.Value;
            }
            else
            {
                LogUtil.LogInfo("Provided file was not added due to duplicate name: " + dest.FileName, nameof(PokemonPool<T>));
            }
            loadedAny = true;
        }
        if (surpriseBlocked == Count)
            LogUtil.LogInfo("Surprise trading will fail; failed to load any compatible files.", nameof(PokemonPool<T>));

        return loadedAny;
    }

    public static bool DisallowRandomRecipientTrade(T pk)
    {
        // Surprise Trade currently bans Mythicals and Legendaries, not Sub-Legendaries.
        if (SpeciesCategory.IsLegendary(pk.Species))
            return true;
        if (SpeciesCategory.IsMythical(pk.Species))
            return true;

        // Can't surprise trade fused stuff.
        if (FormInfo.IsFusedForm(pk.Species, pk.Form, pk.Format))
            return true;

        return false;
    }

    private static int? ExtractTradeCode(string filename)
    {
        var match = Regex.Match(filename, @"\b\d{8}\b");
        return match.Success ? int.Parse(match.Value) : (int?)null;
    }
}