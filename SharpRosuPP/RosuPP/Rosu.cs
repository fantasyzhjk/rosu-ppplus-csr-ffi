using System;
using RosuPP;

#nullable enable

namespace RosuPP;

public static class Extensions {
    public static double Acc(ref this ScoreState state, ref DifficultyAttributes attr, OsuScoreOrigin origin) {
        return RosuLibrary.calculate_accuacy(ref state, ref attr, origin);
    }

    public static double Acc(ref this ScoreState state, ref DifficultyAttributes attr) {
        return RosuLibrary.calculate_accuacy(ref state, ref attr, OsuScoreOrigin.WithSliderAcc);
    }
}

public static class Utils
{
    public enum Mods : uint
    {
        None = 1 >> 1,
        NoFail = 1 << 0,
        Easy = 1 << 1,
        TouchDevice = 1 << 2,
        Hidden = 1 << 3,
        HardRock = 1 << 4,
        SuddenDeath = 1 << 5,
        DoubleTime = 1 << 6,
        Relax = 1 << 7,
        HalfTime = 1 << 8,
        Nightcore = 1 << 9 | DoubleTime, // Only set along with DoubleTime. i.e: NC only gives 576
        Flashlight = 1 << 10,
        Autoplay = 1 << 11,
        SpunOut = 1 << 12,
        Relax2 = 1 << 13, // Autopilot
        Perfect = 1 << 14 | SuddenDeath, // Only set along with SuddenDeath. i.e: PF only gives 16416
        Key4 = 1 << 15,
        Key5 = 1 << 16,
        Key6 = 1 << 17,
        Key7 = 1 << 18,
        Key8 = 1 << 19,
        FadeIn = 1 << 20,
        Random = 1 << 21,
        Cinema = 1 << 22,
        Target = 1 << 23,
        Key9 = 1 << 24,
        KeyCoop = 1 << 25,
        Key1 = 1 << 26,
        Key3 = 1 << 27,
        Key2 = 1 << 28,
        ScoreV2 = 1 << 29,
        Mirror = 1 << 30,
        KeyMod = Key1 | Key2 | Key3 | Key4 | Key5 | Key6 | Key7 | Key8 | Key9 | KeyCoop,
        FreeModAllowed =
            NoFail
            | Easy
            | Hidden
            | HardRock
            | SuddenDeath
            | Flashlight
            | FadeIn
            | Relax
            | Relax2
            | SpunOut
            | KeyMod,
        ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn
    };
}

public partial struct ScoreState
{
    public uint TotalHits(Mode mode)
    {
        var amount = n300 + n100 + misses;

        if (mode is not Mode.Taiko)
        {
            amount += n50;

            if (mode is not Mode.Osu)
            {
                amount += n_katu;
                amount += mode is Mode.Catch ? n_geki : 0;
            }
        }

        return amount;
    }
}

public partial struct OsuDifficultyAttributes
{
    public readonly uint n_objects => n_circles + n_sliders + n_spinners;
    public readonly double od => (80.0 - great_hit_window) / 6.0;
}

public partial struct OsuPerformanceAttributes
{
    public readonly double stars => difficulty.stars;
    public readonly uint max_combo => difficulty.max_combo;
    public readonly uint n_objects => difficulty.n_objects;
}

public partial struct ManiaPerformanceAttributes
{
    public readonly double stars => difficulty.stars;
    public readonly uint max_combo => difficulty.max_combo;
    public readonly uint n_objects => difficulty.n_objects;
    public readonly bool is_convert => difficulty.is_convert;
}

public partial struct CatchDifficultyAttributes
{
    public readonly uint max_combo => n_droplets + n_fruits;
}

public partial struct CatchPerformanceAttributes
{
    public readonly double stars => difficulty.stars;
    public readonly uint max_combo => difficulty.max_combo;
    public readonly bool is_convert => difficulty.is_convert;
}

public partial struct TaikoPerformanceAttributes
{
    public readonly double stars => difficulty.stars;
    public readonly uint max_combo => difficulty.max_combo;
    public readonly bool is_convert => difficulty.is_convert;
}

public partial struct DifficultyAttributes
{
    public override string ToString()
    {
        using var str = OwnedString.Empty();
        RosuLibrary.debug_difficylty_attributes(ref this, str.Context);
        return str.ToString();
    }
}

public partial struct PerformanceAttributes
{
    public override string ToString()
    {
        using var str = OwnedString.Empty();
        RosuLibrary.debug_performance_attributes(ref this, str.Context);
        return str.ToString();
    }
}

public partial struct OptionDifficultyAttributes {
    public DifficultyAttributes Unwrap() => this.ToNullable() ?? throw new NullReferenceException($"{nameof(DifficultyAttributes)} is null");
}

public partial struct OptionOsuDifficultyAttributes {
    public OsuDifficultyAttributes Unwrap() => this.ToNullable() ?? throw new NullReferenceException($"{nameof(OsuDifficultyAttributes)} is null");
}

public partial struct OptionTaikoDifficultyAttributes {
    public TaikoDifficultyAttributes Unwrap() => this.ToNullable() ?? throw new NullReferenceException($"{nameof(TaikoDifficultyAttributes)} is null");
}

public partial struct OptionCatchDifficultyAttributes {
    public CatchDifficultyAttributes Unwrap() => this.ToNullable() ?? throw new NullReferenceException($"{nameof(CatchDifficultyAttributes)} is null");
}

public partial struct OptionManiaDifficultyAttributes {
    public ManiaDifficultyAttributes Unwrap() => this.ToNullable() ?? throw new NullReferenceException($"{nameof(ManiaDifficultyAttributes)} is null");
}

public partial struct OptionPerformanceAttributes {
    public PerformanceAttributes Unwrap() => this.ToNullable() ?? throw new NullReferenceException($"{nameof(PerformanceAttributes)} is null");
}

public partial struct OptionOsuPerformanceAttributes {
    public OsuPerformanceAttributes Unwrap() => this.ToNullable() ?? throw new NullReferenceException($"{nameof(OsuPerformanceAttributes)} is null");
}

public partial struct OptionTaikoPerformanceAttributes {
    public TaikoPerformanceAttributes Unwrap() => this.ToNullable() ?? throw new NullReferenceException($"{nameof(TaikoPerformanceAttributes)} is null");
}

public partial struct OptionCatchPerformanceAttributes {
    public CatchPerformanceAttributes Unwrap() => this.ToNullable() ?? throw new NullReferenceException($"{nameof(CatchPerformanceAttributes)} is null");
}

public partial struct OptionManiaPerformanceAttributes
{
    public ManiaPerformanceAttributes Unwrap() => this.ToNullable() ?? throw new NullReferenceException($"{nameof(ManiaPerformanceAttributes)} is null");
}

public partial struct OptionTooSuspicious {
    public TooSuspicious Unwrap() => this.ToNullable() ?? throw new NullReferenceException($"{nameof(TooSuspicious)} is null");
}


public partial struct ScoreState
{
    public override string ToString()
    {
        using var str = OwnedString.Empty();
        RosuLibrary.debug_score_state(ref this, str.Context);
        return str.ToString();
    }
}

public partial class HitObjects
{
    public static HitObjects New(Beatmap beatmap)
    {
        return New(beatmap.Context);
    }
}

public partial class Difficulty
{
    public DifficultyAttributes Calculate(Beatmap beatmap)
    {
        return Calculate(beatmap.Context);
    }

    public void Mods(uint mods)
    {
        IMods(mods);
    }

    public void Mods(string mods)
    {
        SMods(mods);
    }

    public void Mods(string[] mods)
    {
        SMods(string.Concat(mods));
    }

    public void Mods(Mods mods)
    {
        PMods(mods.Context);
    }
}

public partial class Performance
{
    public PerformanceAttributes Calculate(Beatmap beatmap)
    {
        return Calculate(beatmap.Context);
    }
    
    public ScoreState GenerateState(Beatmap beatmap)
    {
        return GenerateState(beatmap.Context);
    }

    public void Mods(uint mods)
    {
        IMods(mods);
    }

    public void Mods(string mods)
    {
        SMods(mods);
    }

    public void Mods(string[] mods)
    {
        SMods(string.Concat(mods));
    }

    public void Mods(Mods mods)
    {
        PMods(mods.Context);
    }
}

public partial class GradualDifficulty
{
    public static GradualDifficulty New(Difficulty difficulty, Beatmap beatmap) {
        return GradualDifficulty.New(difficulty.Context, beatmap.Context);
    }

    public static GradualDifficulty NewWithMode(Difficulty difficulty, Beatmap beatmap, Mode mode) {
        return GradualDifficulty.NewWithMode(difficulty.Context, beatmap.Context, mode);
    }
}

public partial class GradualPerformance
{
    public static GradualPerformance New(Difficulty difficulty, Beatmap beatmap) {
        return GradualPerformance.New(difficulty.Context, beatmap.Context);
    }

    public static GradualPerformance NewWithMode(Difficulty difficulty, Beatmap beatmap, Mode mode) {
        return GradualPerformance.NewWithMode(difficulty.Context, beatmap.Context, mode);
    }
}

public partial class BeatmapAttributesBuilder
{
    public BeatmapAttributes Build(Beatmap beatmap)
    {
        return Build(beatmap.Context);
    }

    public void Mods(uint mods)
    {
        IMods(mods);
    }

    public void Mods(string mods)
    {
        SMods(mods);
    }

    public void Mods(string[] mods)
    {
        SMods(string.Concat(mods));
    }

    public void Mods(Mods mods)
    {
        PMods(mods.Context);
    }
}

public partial class OwnedString
{
    public override string ToString()
    {
        return ToCstr();
    }
}

public partial class Beatmap
{
    public static Beatmap FromBytes(byte[] data)
    {
        var self = new Beatmap();
        RosuLibrary.beatmap_from_bytes(ref self._context, data);
        return self;
    }

    public static Beatmap FromClone(Beatmap beatmap)
    {
        return FromClone(beatmap.Context);
    }

    /// Convert a Beatmap to the specified mode
    public bool Convert(Mode mode, Mods mods)
    {
        return Convert(mode, mods.Context);
    }

    /// Convert a Beatmap to the specified mode
    public bool Convert(Mode mode)
    {
        return Convert(mode, Mods.New(mode));
    }

}

public partial class Mods
{
    public void Json(OwnedString str) {
        this.Json(str.Context);
    }

    public static Mods FromJson(string str, Mode mode) {
        return Mods.FromJson(str, mode, false);
    }

    public bool InsertJson(string str) {
        return this.InsertJson(str, false);
    }
}

