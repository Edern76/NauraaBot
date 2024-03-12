using Microsoft.VisualStudio.TestTools.UnitTesting;
using NauraaBot.Core.Utils;

namespace NauraaBot.Tests.Core.Utils;

[TestClass]
public class StringUtilsTest
{
    [TestMethod]
    public void SplitIntoGroups_CorrectOutput()
    {
        string test1 = "The Spindle, Muna Bastion";
        string test2 = "The Monolith, Ordis Bastion";
        string test3 = "Yong-Su, Tisserand Verdoyant";
        string test4 = "Yong Su Tisserand Verdoyant";

        string[] result1 = StringUtils.SplitIntoGroups(test1.Split(" "), 2);
        string[] result2 = StringUtils.SplitIntoGroups(test2.Split(" "), 2);
        string[] result3 = StringUtils.SplitIntoGroups(test3.Split(" "), 2);
        string[] result4 = StringUtils.SplitIntoGroups(test1.Split(" "), 3);
        string[] result5 = StringUtils.SplitIntoGroups(test1.Split(" "), 1);
        string[] result6 = StringUtils.SplitIntoGroups(test4.Split(" "), 2);

        Assert.AreEqual(result1.Length, 3);
        Assert.AreEqual(result2.Length, 3);
        Assert.AreEqual(result3.Length, 2);
        Assert.AreEqual(result4.Length, 2);
        Assert.AreEqual(result5.Length, 4);
        Assert.AreEqual(result6.Length, 3);

        Assert.AreEqual(result1[0], "The Spindle");
        Assert.AreEqual(result1[1], "Spindle Muna");
        Assert.AreEqual(result1[2], "Muna Bastion");

        Assert.AreEqual(result2[0], "The Monolith");
        Assert.AreEqual(result2[1], "Monolith Ordis");
        Assert.AreEqual(result2[2], "Ordis Bastion");

        Assert.AreEqual("Yong-Su Tisserand", result3[0]);
        Assert.AreEqual("Tisserand Verdoyant", result3[1]);

        Assert.AreEqual("The Spindle Muna", result4[0]);
        Assert.AreEqual("Spindle Muna Bastion", result4[1]);

        Assert.AreEqual("The", result5[0]);
        Assert.AreEqual("Spindle", result5[1]);
        Assert.AreEqual("Muna", result5[2]);
        Assert.AreEqual("Bastion", result5[3]);

        Assert.AreEqual("Yong Su", result6[0]);
        Assert.AreEqual("Su Tisserand", result6[1]);
        Assert.AreEqual("Tisserand Verdoyant", result6[2]);
    }

    [TestMethod]
    public void ReplaceSpecialCharacters_CorrectOutput()
    {
        string test1 = "The Spindle, Muna Bastion";
        string test2 = "The Monolith, Ordis Bastion";
        string test3 = "Yong-Su, Tisserand Verdoyant";
        string test4 = "Entraînement à capella";
        string test5 = "Nuée de guêpes";

        string result1 = StringUtils.ReplaceSpecialCharacters(test1);
        string result2 = StringUtils.ReplaceSpecialCharacters(test2);
        string result3 = StringUtils.ReplaceSpecialCharacters(test3);
        string result4 = StringUtils.ReplaceSpecialCharacters(test4);
        string result5 = StringUtils.ReplaceSpecialCharacters(test5);

        Assert.AreEqual(result1, "The Spindle Muna Bastion");
        Assert.AreEqual(result2, "The Monolith Ordis Bastion");
        Assert.AreEqual(result3, "Yong Su Tisserand Verdoyant");
        Assert.AreEqual(result4, "Entrainement a capella");
        Assert.AreEqual(result5, "Nuee de guepes");
    }
}