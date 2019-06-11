#if TESTING
using System;
using System.Collections.Generic;
using System.Text;

namespace JSON
{
    class Tester
    {
        public static int Main(string[] args)
        {
            AnyContainerTest();
            return 0;
        }

        private static void AnyContainerTest()
        {
            bool kill = false;
            while (!kill)
            {
                Console.WriteLine("Paste in Test JSON. Type \"go\" in a single line to start parsing. Type \"exit\" to quit program");
                string containerJSON = string.Empty;
                while (true)
                {
                    string line = Console.ReadLine();
                    if (line == "go")
                    {
                        break;
                    }
                    else if (line == "exit")
                    {
                        kill = true;
                        break;
                    }
                    else
                    {
                        containerJSON += line + '\n';
                    }
                }
                if (containerJSON == "exit")
                {
                    break;
                }
                if (!string.IsNullOrEmpty(containerJSON))
                {
                    if (JSONContainer.TryParse(containerJSON, out JSONContainer result, out string errormessage))
                    {
                        Console.WriteLine("\nParseSuccess!\n\n" + result.Build(true));
                        result.TryGetField("BotAdminIDs", out JSONContainer test);
                    }
                    else
                    {
                        Console.WriteLine(errormessage);
                    }
                }
                Console.WriteLine();
            }
        }

        private static void CustomContainerTest()
        {
            JSONContainer json = JSONContainer.NewObject();
            json.TryAddField("test", 0f);
            json.TryAddField("test2", 0.656d);
            json.TryAddField("test3", (ulong)231748324821394);
            json.TryAddField("test4", "this is a mean string \n as it contains bad characters\t");
            List<JSONField> arrayTest = new List<JSONField>();
            for (int i = 0; i < 5; i++)
            {
                arrayTest.Add(new JSONField("ayyyyy " + (i + 1)));
            }
            json.TryAddField("array", arrayTest);
            JSONContainer nested = JSONContainer.NewObject();
            nested.TryAddField("booltest", true);
            nested.TryAddField("booltest2", false);
            nested.TryAddField("test", '\0');
            json.TryAddField("nested", nested);
            JSONContainer nullstring = null;
            json.TryAddField("nulltest", nullstring);

            string builded = json.Build(true);
            Console.WriteLine(builded);

            bool success = JSONContainer.TryParse(builded, out JSONContainer parsed, out string errormessage);

            Console.WriteLine(success ? "Success\n" + parsed.Build(true) : "Failure: " + errormessage);

            Console.ReadLine();
        }
    }
}
#endif