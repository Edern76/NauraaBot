using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NauraaBot.Core.Utils;

namespace NauraaBot.Tests.Core.Utils;

internal struct CardUtilsTestCase
{
    public string BaseID;
    public string Set;
    public ulong Number;
    public string ExpectedUniqueID;
}

[TestClass]
[TestSubject(typeof(CardUtils))]
public class CardUtilsTest
{
    [TestMethod]
    public void GetUniqueIDFromBaseID_CorrectOutput()
    {
        CardUtilsTestCase[] testCases =
        {
            new CardUtilsTestCase()
            {
                BaseID = "ALT_CORE_B_MU_12_C",
                Set = "COREKS",
                Number = 5111,
                ExpectedUniqueID = "ALT_COREKS_B_MU_12_U_5111",
            },
            new CardUtilsTestCase()
            {
                BaseID = "ALT_COREKS_B_MU_12_R1",
                Set = "COREKS",
                Number = 5111,
                ExpectedUniqueID = "ALT_COREKS_B_MU_12_U_5111",
            },
            new CardUtilsTestCase()
            {
                BaseID = "ALT_COREKS_B_AX_18_R2",
                Set = "COREKS",
                Number = 3233,
                ExpectedUniqueID = "ALT_COREKS_B_AX_18_U_3233",
            },
            new CardUtilsTestCase()
            {
                BaseID = "ALT_CORE_B_BR_11_C",
                Set = "COREKS",
                Number = 4730,
                ExpectedUniqueID = "ALT_COREKS_B_BR_11_U_4730",
            },
            new CardUtilsTestCase()
            {
                BaseID = "ALT_CORE_B_MU_05_C",
                Set = "CORE",
                Number = 2143,
                ExpectedUniqueID = "ALT_CORE_B_MU_05_U_2143",
            },
            new CardUtilsTestCase()
            {
                BaseID = "ALT_COREKS_B_MU_05_C",
                Set = "CORE",
                Number = 2143,
                ExpectedUniqueID = "ALT_CORE_B_MU_05_U_2143",
            },
        };

        foreach (CardUtilsTestCase testCase in testCases)
        {
            Assert.AreEqual(testCase.ExpectedUniqueID,
                CardUtils.GetUniqueIDFromBaseID(testCase.BaseID, testCase.Set, testCase.Number));
        }
    }
}