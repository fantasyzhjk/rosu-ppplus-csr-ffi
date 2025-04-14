using System.Diagnostics;
using System.Reflection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Engine.ClientProtocol;
using Xunit.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Game.Rulesets.Taiko;

namespace RosuPP.Tests;

public class PPUnitTest(ITestOutputHelper output)
{
    private readonly ITestOutputHelper output = output;

    private void TestPP(string beatmapPath, string modstr, bool isLazer, Mode? mode = null, double compareRange = 0.0000001) {
        var b = File.ReadAllBytes(beatmapPath);
        using var beatmap = Beatmap.FromBytes(b);

        if (mode is not null) {
            using var m = Mods.FromAcronyms(modstr, mode.Value);
            var convertSuccess = beatmap.Convert(mode.Value, m);
            Assert.True(convertSuccess, "convert failed");
        } else {
            mode = beatmap.Mode();
        }

        using var mods = Mods.FromAcronyms(modstr, beatmap.Mode());
        using var difficulty = Difficulty.New();
        difficulty.Lazer(isLazer);
        difficulty.Mods(mods);
        var dattr = difficulty.Calculate(beatmap);

        using var performance = Performance.New();
        performance.Lazer(isLazer);
        performance.Mods(mods);

        var state = performance.GenerateStateFromDifficulty(dattr);
        var attr = performance.CalculateFromDifficulty(dattr);
        var origin = OsuScoreOrigin.Stable;
        if (isLazer) {
            if (mods.Contains("CL")) {
                origin = OsuScoreOrigin.WithoutSliderAcc;
            } else {
                origin = OsuScoreOrigin.WithSliderAcc;
            }
        }
        var acc = state.Acc(ref dattr, origin) * 100;
        output.WriteLine("{0}", attr);
        output.WriteLine("{0}", state);
        output.WriteLine("{0}", acc);
        
        var ruleset = OsuPP.Utils.ParseRuleset((int)beatmap.Mode())!;
        var osubm = OsuPP.Calculater.New(ruleset, new OsuPP.CalculatorWorkingBeatmap(b));
        var attr2 = osubm.Mods(mods).LoadState(state, dattr).Acc(acc).Calculate();

        var pp = mode switch {
            Mode.Osu => attr.osu.Unwarp().pp,
            Mode.Taiko => attr.taiko.Unwarp().pp,
            Mode.Catch => attr.fruit.Unwarp().pp,
            Mode.Mania => attr.mania.Unwarp().pp,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        Assert.InRange(pp, attr2.Total - compareRange, attr2.Total + compareRange);
    }

    [Fact]
    public void GradualTest() {
        var d = Assembly.GetExecutingAssembly().Location;
        var b = File.ReadAllBytes("../../../resources/657916.osu");
        using var beatmap = Beatmap.FromBytes(b);
        using var difficulty = Difficulty.New();
        using var gradual = GradualPerformance.New(difficulty, beatmap);

        var totalLen = gradual.Len();

        var calculated = 10;

        var state = new ScoreState();
        for (var i = 0; i < calculated; i++) {
            state.n300 += 1;
            state.max_combo += 1;
            var attrs = gradual.Next(state).Unwarp();
            output.WriteLine("{0}", attrs);
            output.WriteLine("pp: {0}", attrs.osu.Unwarp().pp);
        }

        var remainingObjects = gradual.Len();

        Assert.Equal(totalLen, remainingObjects + calculated);
    }

    [Fact]
    public void Convert() {
        TestPP(
            beatmapPath: "../../../resources/657916.osu",
            modstr: "",
            isLazer: false, 
            mode: Mode.Taiko
        );

        TestPP(
            beatmapPath: "../../../resources/657916.osu",
            modstr: "",
            isLazer: false, 
            mode: Mode.Catch
        );

        TestPP(
            beatmapPath: "../../../resources/657916.osu",
            modstr: "",
            isLazer: false, 
            mode: Mode.Mania
        );
    }


    [Fact]
    public void TestPPStable()
    {
        TestPP(
            beatmapPath: "../../../resources/657916.osu",
            modstr: "CL",
            isLazer: false
        );
        
        TestPP(
            beatmapPath: "../../../resources/1028484.osu",
            modstr: "CL",
            isLazer: false
        );

        TestPP(
            beatmapPath: "../../../resources/1638954.osu",
            modstr: "CL",
            isLazer: false
        );

        TestPP(
            beatmapPath: "../../../resources/2118524.osu",
            modstr: "CL",
            isLazer: false
        );

        TestPP(
            beatmapPath: "../../../resources/2785319.osu",
            modstr: "CL",
            isLazer: false
        );
    }

    [Fact]
    public void TestPPLazer()
    {
        TestPP(
            beatmapPath: "../../../resources/657916.osu",
            modstr: "",
            isLazer: true
        );

        TestPP(
            beatmapPath: "../../../resources/1028484.osu",
            modstr: "",
            isLazer: true
        );

        TestPP(
            beatmapPath: "../../../resources/1638954.osu",
            modstr: "",
            isLazer: true
        );

        TestPP(
            beatmapPath: "../../../resources/2118524.osu",
            modstr: "",
            isLazer: true
        );

        TestPP(
            beatmapPath: "../../../resources/2785319.osu",
            modstr: "",
            isLazer: true
        );
    }

    [Fact]
    public void TestPPLazerWithCL()
    {
        TestPP(
            beatmapPath: "../../../resources/657916.osu",
            modstr: "CL",
            isLazer: true
        );

        TestPP(
            beatmapPath: "../../../resources/1028484.osu",
            modstr: "CL",
            isLazer: true
        );

        TestPP(
            beatmapPath: "../../../resources/1638954.osu",
            modstr: "CL",
            isLazer: true
        );

        TestPP(
            beatmapPath: "../../../resources/2118524.osu",
            modstr: "CL",
            isLazer: true
        );

        TestPP(
            beatmapPath: "../../../resources/2785319.osu",
            modstr: "CL",
            isLazer: true
        );
    }

    [Fact]
    public void TestState()
    {
        var d = Assembly.GetExecutingAssembly().Location;
        var b = File.ReadAllBytes("../../../resources/657916.osu");
        using var beatmap = Beatmap.FromBytes(b);
        using var difficulty = Difficulty.New();
        var dattr = difficulty.Calculate(beatmap);

        using var performance = Performance.New();
        performance.Accuracy(97.35);
        performance.Misses(1);

        var state = performance.GenerateStateFromDifficulty(dattr);
        var attr = performance.CalculateFromDifficulty(dattr);
        output.WriteLine("{0}", state);

        using var performance2 = Performance.New();
        performance2.State(state);
        var attr2 = performance.CalculateFromDifficulty(dattr);

        Assert.Equal(attr2.osu.Unwarp().pp, attr.osu.Unwarp().pp);
    }

    [Fact]
    public void TestDiffTaiko()
    {
        var d = Assembly.GetExecutingAssembly().Location;
        var b = File.ReadAllBytes("../../../resources/2785319.osu");
        using var beatmap = Beatmap.FromBytes(b);
        beatmap.Convert(Mode.Taiko);

        using var difficulty = Difficulty.New();
        var attr = difficulty.Calculate(beatmap);
        output.WriteLine("{0}", attr);

        var ruleset = OsuPP.Utils.ParseRuleset((int)beatmap.Mode())!;
        var osubm = OsuPP.Calculater.New(ruleset, new OsuPP.CalculatorWorkingBeatmap(b));
        var attr2 = osubm.CalculateDifficulty();
        
        output.WriteLine("{0}", JsonConvert.SerializeObject(attr2, Formatting.Indented));
        Assert.Equal(attr2.StarRating, attr.taiko.Unwarp().stars);
    }

    [Fact]
    public void TestDiffFruit()
    {
        var d = Assembly.GetExecutingAssembly().Location;
        var b = File.ReadAllBytes("../../../resources/2785319.osu");
        using var beatmap = Beatmap.FromBytes(b);
        beatmap.Convert(Mode.Catch, Mods.New(Mode.Catch));

        using var difficulty = Difficulty.New();
        var attr = difficulty.Calculate(beatmap);
        output.WriteLine("{0}", attr);

        var ruleset = OsuPP.Utils.ParseRuleset((int)beatmap.Mode())!;
        var osubm = OsuPP.Calculater.New(ruleset, new OsuPP.CalculatorWorkingBeatmap(b));
        var attr2 = osubm.CalculateDifficulty();
        
        output.WriteLine("{0}", JsonConvert.SerializeObject(attr2, Formatting.Indented));
        Assert.Equal(attr2.StarRating, attr.fruit.Unwarp().stars);
    }

    [Fact]
    public void TestDiffMania()
    {
        var d = Assembly.GetExecutingAssembly().Location;
        var b = File.ReadAllBytes("../../../resources/2785319.osu");
        using var beatmap = Beatmap.FromBytes(b);
        beatmap.Convert(Mode.Mania);

        using var difficulty = Difficulty.New();
        var attr = difficulty.Calculate(beatmap);
        output.WriteLine("{0}", attr);

        var ruleset = OsuPP.Utils.ParseRuleset((int)beatmap.Mode())!;
        var osubm = OsuPP.Calculater.New(ruleset, new OsuPP.CalculatorWorkingBeatmap(b));
        var attr2 = osubm.CalculateDifficulty();
        
        output.WriteLine("{0}", JsonConvert.SerializeObject(attr2, Formatting.Indented));
        Assert.Equal(attr2.StarRating, attr.mania.Unwarp().stars);
    }

    [Fact]
    public void TestBeatmapAttr()
    {
        using var beatmap = Beatmap.FromPath("../../../resources/2785319.osu");
        using var builder = BeatmapAttributesBuilder.New();
        builder.Mods("DT");

        var bmattr = builder.Build(beatmap);
        output.WriteLine("cs: {0}", bmattr.cs);
        output.WriteLine("od: {0}", bmattr.od);
        output.WriteLine("hp: {0}", bmattr.hp);
        output.WriteLine("ar: {0}", bmattr.ar);
        output.WriteLine("cr: {0}", bmattr.clock_rate);

        Assert.Equal(4.5, bmattr.cs);
        Assert.Equal(10.311111238267687, bmattr.od);
        Assert.Equal(5, bmattr.hp);
        Assert.Equal(10.53333346048991, bmattr.ar);
        Assert.Equal(1.5, bmattr.clock_rate);
    }

    [Fact]
    public void ModsTest()
    {
        using var s = OwnedString.Empty();
        var j = """
                        [
                            { "acronym": "HD" },
                            { "acronym": "CL" },
                            { "acronym": "DT", "settings": { "speed_change": 1.5 } }
                        ]
                        """;

        using var mods = Mods.FromJson(j, Mode.Taiko);
        Assert.Equal((uint)3, mods.Len());
        Assert.True(mods.Contains("DT"));

        Assert.Equal((double?)1.5, mods.ClockRate().ToNullable());

        mods.Json(s);
        var res = s.ToString();
        output.WriteLine(res);

        var parsed_json = JsonConvert.DeserializeObject<JArray>(res);
        Assert.NotNull(parsed_json);
        Assert.Equal(3, parsed_json!.Count);
        var dt_node = parsed_json.First(x => x?["acronym"]?.ToString() == "DT");
        Assert.NotNull(dt_node);
        Assert.Equal(1.5, dt_node["settings"]?["speed_change"]?.ToObject<double>());
        
        mods.Insert("HR");
        Assert.Equal((uint)4, mods.Len());
    }
}