using PAT.Common;
using PAT.Common.Classes.ModuleInterface;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PAT.GenPred
{
  public class Program
  {
    private static void Main(string[] args)
    {
      string inputDir = args[0];
      if (!Path.IsPathRooted(inputDir))
        inputDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), inputDir);
      string outputFile = args[1];
      if (!Path.IsPathRooted(outputFile))
        outputFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), outputFile);
      StringBuilder sb = new StringBuilder();
      StreamWriter sw = new StreamWriter(outputFile);
      sw.WriteLine("date,P1Name,P2Name,P1WinProb,P2WinProb");
      foreach (string file in Directory.GetFiles(inputDir))
      {
        try
        {
          System.Console.Out.WriteLine("Start running file: " + file);
          string date = Regex.Match(file, "\\d{4}?-\\d{2}?-\\d{2}?").Value;
          sb.Append(date + ",");
          Match match = Regex.Match(file, "_[A-Z][a-z]+(-[A-Z][a-z]+)*");
          string p1 = match.Value.Substring(1).Replace('-', ' ');
          match = match.NextMatch();
          string p2 = match.Value.Substring(1).Replace('-', ' ');
          sb.Append(p1 + "," + p2 + ",");
          sw.Write(sb);
          sb = new StringBuilder();
          string end = new StreamReader(file).ReadToEnd();
          Program.RunOneSpec("PCSP", end, sw, sb, file);
          sb = new StringBuilder();
        }
        catch (Exception ex)
        {
          System.Console.Out.WriteLine("Error:" + ex.Message);
        }
        sw.Write(sb.ToString());
        sw.Flush();
      }
      System.Console.Out.WriteLine("PAT finished successfully.");
    }

    private static void RunOneSpec(
      string module,
      string specString,
      StreamWriter sw,
      StringBuilder sb,
      string filePath)
    {
      try
      {
        foreach (AssertionBase assertionBase in PAT.Common.Ultility.Ultility.LoadModule(module).ParseSpecification(specString, "", filePath).AssertionDatabase.Values)
        {
          try
          {
            assertionBase.UIInitialize(null, 0, 0);
            assertionBase.InternalStart();
            string verificationStatistics = assertionBase.GetVerificationStatistics();
            assertionBase.VerificationMode = false;
            assertionBase.InternalStart();
            string result = assertionBase.VerificationOutput.ResultString;
            Match match = Regex.Match(result, "\\d\\.\\d*");
            double p1min = double.Parse(match.Value);
            double p2max = 1 - double.Parse(match.Value);
            match = match.NextMatch();
            double p1max = double.Parse(match.Value);
            double p2min = 1 - double.Parse(match.Value);
            double p1prob = (p1min + p1max) / 2;
            double p2prob = (p2min + p2max) / 2;
            sb.AppendLine(p1prob + "," + p2prob);
            sw.Write(sb.ToString());
            sw.Flush();
            sb = new StringBuilder();
          }
          catch (Exception ex)
          {
            System.Console.Out.WriteLine("Error occurred: " + ex.Message);
          }
        }
      }
      catch (Exception ex)
      {
        System.Console.Out.WriteLine("Error occurred: " + ex.Message);
      }
    }
  }
}
